/**
* @author wangyafei
* 向地图控件中添加地图和业务图层
* 图层从数据库中动态获取
* 底图切换
*/
define("widget/layer/BaseMapAndLayer",
[
'dojo/_base/declare',
'dojo/_base/event',
'dojo/_base/lang',
'dojo/request/xhr',
'dojo/request/script',
'dojo/dom-construct',
'dojo/json',
'dojo/query',
'dojo/dom',
'dojo/on',
'dojo/dom-class',
'dojo/dom-attr',
'dojo/topic',
'dojo/_base/array',
'esri/layers/ArcGISTiledMapServiceLayer',
'esri/geometry/Extent',
'esri/SpatialReference',
'widget/layer/VecLayer',
'widget/layer/ImgLayer'
],
function (declare, Event, lang, xhr, script, domConstruct, JSON, query, dom, on, domClass, domAttr, topic, arrayUtils,
    ArcGISTiledMapServiceLayer, Extent, SpatialReference, VecLayer, ImgLayer) {
    "use strict";
    var instance = null, clazz;

    function addBaseLayerMenu(suc) {
        var htmlStr = "<ul class='clearfix'>", vecHtml = "", imgHtml = "", mixHtml = "", vecs = [], imgs = [], mixs = [], mixt = [];
        arrayUtils.forEach(suc, function (baseLayerInfo) {
            if (baseLayerInfo.DATATYPE === 0) {
                vecs.push(baseLayerInfo);
            } else if (baseLayerInfo.DATATYPE === 1) {
                imgs.push(baseLayerInfo);
            } else if (baseLayerInfo.DATATYPE === 3) {
                mixs.push(baseLayerInfo);
            }
        });
        var vLength = vecs.length, iLength = imgs.length, mLength = mixs.length;
        for (var v = 0; v < vLength; v++) {
            var baseLayerInfo = vecs[v];
            if (v === 0) {
                vecHtml += "<li data-dojo-type='0' data-dojo-value='" + baseLayerInfo.ID + "'><a class='basemap selected' title='矢量'><span id='veclabel'>矢量(" + baseLayerInfo.YEAR + ")</span></a>";
                vecHtml += "<ul><li data-dojo-type='0' data-dojo-value='" + baseLayerInfo.ID + "'><a class='basemap'>" + baseLayerInfo.YEAR + "</a></li>";
            } else if (v > 0) {
                vecHtml += "<li data-dojo-type='0' data-dojo-value='" + baseLayerInfo.ID + "'><a class='basemap'>" + baseLayerInfo.YEAR + "</a></li>";
            }
        }
        vecHtml += "</ul></li>";

        for (var i = 0; i < iLength; i++) {
            var baseLayerInfo = imgs[i];
            if (i === 0) {
                imgHtml += "<li data-dojo-type='1' data-dojo-value='" + baseLayerInfo.ID + "'><a class='basemap' title='卫片'><span id='imglabel'>卫片(" + baseLayerInfo.YEAR + ")</span></a>";
                imgHtml += "<ul><li data-dojo-type='1' data-dojo-value='" + baseLayerInfo.ID + "'><a class='basemap'>" + baseLayerInfo.YEAR + "</a></li>";
            } else if (i > 0) {
                imgHtml += "<li data-dojo-type='1' data-dojo-value='" + baseLayerInfo.ID + "'><a class='basemap'>" + baseLayerInfo.YEAR + "</a></li>";
            }
        }
        imgHtml += "</ul></li>";

        for (var m = 0; m < mLength; m++) {
            var baseLayerInfo = mixs[m];
            if (m === 0) {
                imgHtml += "<li data-dojo-type='2' data-dojo-value='" + baseLayerInfo.ID + "'><a class='basemap' title='航片'><span id='mixlabel'>航片(" + baseLayerInfo.YEAR + ")</span></a>";
                imgHtml += "<ul><li data-dojo-type='2' data-dojo-value='" + baseLayerInfo.ID + "'><a class='basemap'>" + baseLayerInfo.YEAR + "</a></li>";
            } else if (m > 0) {
                imgHtml += "<li data-dojo-type='2' data-dojo-value='" + baseLayerInfo.ID + "'><a class='basemap'>" + baseLayerInfo.YEAR + "</a></li>";
            }
        }
        mixHtml += "</ul></li>";
        htmlStr += vecHtml + imgHtml + mixHtml + "</ul>";
        domConstruct.place(htmlStr, dom.byId("toggleBaseLayer"));

        //发布底图添加成功事件
        topic.publish("topic/AddBaseMapComplete");
    }

    function addBaseLayer(index) {
        //xhr.get("http://10.246.0.81:9000/service/ImageEngine/picdis/abc?SERVICE=WMTS&REQUEST=GetTile&version=1.0.0&LAYER=Shiliang_2013&style=default&format=image/png&TileMatrixSet=nativeTileMatrixSet&TILEMATRIX=2&TILEROW=6&TILECOL=6");
        /* 初始范围 */
        var baseMap = JZ.baselayers[0];
        var sr = new SpatialReference(baseMap.COORSYS);
        JZ.ie = new Extent(parseFloat(baseMap.XMIN), parseFloat(baseMap.YMIN), parseFloat(baseMap.XMAX), parseFloat(baseMap.YMAX), sr);
        var baseMapLayer = new VecLayer(baseMap.YEAR);
        //var baseMapLayer = new ArcGISTiledMapServiceLayer(baseMap.SERVICEURL);
        on.once(JZ.mc, 'layer-add-result', function () {
            require(['esri/dijit/OverviewMap'], function (OverviewMap) {
                var index = 0;
                arrayUtils.forEach(JZ.baselayers, function (tiledLayer) {
                    if (tiledLayer.DATATYPE == 1 && index != 1) {
                        var overviewMap = new OverviewMap({
                            attachTo: "bottom-right",
                            map: JZ.mc,
                            color: "#0000FF",
                            width: 150,
                            height: 150,
                            baseLayer: new ArcGISTiledMapServiceLayer(tiledLayer.SERVICEURL),
                            visible: false
                        });
                        overviewMap.startup();
                        index = 1;
                    }
                });
                
            });
        });
        JZ.mc.addLayer(baseMapLayer);
        JZ.mc.setExtent(JZ.ie);
        clazz.getInstance().currentBaseLayerID = baseMap.ID;
        clazz.getInstance().baseLayers.push(baseMapLayer);
    }

    /* 底图切换功能尚未完成 */
    clazz = declare([], {
        currentBaseLayerID: -1,
        baseLayers: [],
        constructor: function () {
            //底图切换
            topic.subscribe("topic/ToggleLayerTopic", lang.hitch(this, this.toggleLayer));
        },

        setBaseMap: function () {
            /* 获取数据库中配置的底图 */
            var defer = script.get("webservice/WebServiceMap.asmx/GetBaseMapLayer", {
                jsonp: "callback",
                timeout: 100000
            });
            /* 请求成功事件 */
            defer.then(function (suc) {
                JZ.baselayers = suc;
                addBaseLayerMenu(suc);
            }, function (err) {
                console.log("底图服务GetMapLayer()出现错误，请查看后台日志！");
            });
        },

        toggleLayer: function () {
            addBaseLayer(0);
            //底图切换事件
            query("#toggleBaseLayer ul li").on("click", lang.hitch(this, this.changeBaseMap));
        },

        changeBaseMap: function (e) {
            Event.stop(e);
            var that = this;
            var node = e.currentTarget;
            var basemapID = parseInt(domAttr.get(node, "data-dojo-value"));
            var type = domAttr.get(node, "data-dojo-type");

            if (that.currentBaseLayerID != basemapID && that.currentBaseLayerID !== -1) {
                arrayUtils.forEach(that.baseLayers, function (bl) {
                    JZ.mc.removeLayer(bl);
                });
                that.baseLayers = [];
                var year = 2015;
                arrayUtils.forEach(JZ.baselayers, function (layerInfo) {
                    //layerInfo.DATATYPE=0表示矢量底图,1表示卫片底图,2表示混合底图,3表示航片底图
                    if (layerInfo.ID === basemapID && layerInfo.DATATYPE === 0) {
                        var baseMapLayer = new VecLayer(layerInfo.YEAR);
                        JZ.mc.addLayer(baseMapLayer, 0);
                        that.baseLayers.push(baseMapLayer);
                        that.currentBaseLayerID = layerInfo.ID;
                        year = layerInfo.YEAR;
                    } else if (layerInfo.ID === basemapID && layerInfo.DATATYPE === 1) {
                        var baseMapLayer = new ArcGISTiledMapServiceLayer(layerInfo.SERVICEURL);
                        JZ.mc.addLayer(baseMapLayer, 0);
                        that.baseLayers.push(baseMapLayer);
                        arrayUtils.forEach(JZ.baselayers, function (mixLayer) {
                            if (mixLayer.DATATYPE === 2 && mixLayer.YEAR === layerInfo.YEAR) {
                                var baseMapLayer1 = new ArcGISTiledMapServiceLayer(mixLayer.SERVICEURL);
                                JZ.mc.addLayer(baseMapLayer1, 1);
                                that.baseLayers.push(baseMapLayer1);
                            }
                        });
                        that.currentBaseLayerID = layerInfo.ID;
                        year = layerInfo.YEAR;
                    } else if (layerInfo.ID === basemapID && layerInfo.DATATYPE === 3) {
                        var rc = new ImgLayer(layerInfo.YEAR);
                        JZ.mc.addLayer(rc);
                        JZ.mc.reorderLayer(rc, 0);
                        that.baseLayers.push(rc);
                        //arrayUtils.forEach(JZ.baselayers, function (mixLayer) {
                        //    if (mixLayer.DATATYPE === 2 && mixLayer.YEAR === layerInfo.YEAR) {
                        //        var baseMapLayer1 = new ArcGISTiledMapServiceLayer(mixLayer.SERVICEURL);
                        //        JZ.mc.addLayer(baseMapLayer1, 1);
                        //        that.baseLayers.push(baseMapLayer1);
                        //    }
                        //});
                        that.currentBaseLayerID = layerInfo.ID;
                        year = layerInfo.YEAR;
                    }
                });

                query("#toggleBaseLayer > ul > li > a").removeClass("selected");
                switch (type) {
                    case "0":
                        dom.byId("veclabel").innerText = "矢量(" + year + ")";
                        domClass.add(dom.byId("veclabel").parentElement, "selected");
                        break;
                    case "1":
                        dom.byId("imglabel").innerText = "卫片(" + year + ")";
                        domClass.add(dom.byId("imglabel").parentElement, "selected");
                        break;
                    case "2":
                        dom.byId("mixlabel").innerText = "航片(" + year + ")";
                        domClass.add(dom.byId("mixlabel").parentElement, "selected");
                        break;
                }
            }
        }
    });

    clazz.getInstance = function () {
        if (instance === null) {
            instance = new clazz();
        }
        return instance;
    };
    return clazz;
});