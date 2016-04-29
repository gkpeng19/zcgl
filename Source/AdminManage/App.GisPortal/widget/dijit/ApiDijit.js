/**
* @author wangyafei
* 底图切换
*/
define(['dojo/_base/declare',
        'dijit/_WidgetBase',
        'dijit/_TemplatedMixin',
        'dojo/request/script',
        'dojo/dom',
        'dojo/dom-construct',
        'dojo/dom-attr',
        'dojo/_base/array',
        'dojo/query',
        'dojo/dom-class',
        'dojo/_base/lang',
        'esri/layers/ArcGISTiledMapServiceLayer',
        'esri/geometry/Extent',
        'esri/SpatialReference',
        'widget/layer/ImgLayer',
        'widget/layer/VecLayer',
        'dojo/text!./template/ApiDijit.html'],
function (declare, _WidgetBase, _TemplatedMixin, script, dom, domConstruct, domAttr,
    arrayUtils, query, domClass, lang, ArcGISTiledMapServiceLayer, Extent, SpatialReference,
    ImgLayer, VecLayer, template) {
    var instance = null, clazz;
    /* 根据用户的权限从数据库中获取用户的功能权限 */
    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        pNode: "",
        baseinfos: [],
        baseLayers: [],
        startup: function () {
            this.inherited(arguments);
            domConstruct.place(this.domNode, this.pNode, "last");
            this.ajaxGet();
        },
        ajaxGet: function () {
            var that = this;
            /* 获取数据库中配置的底图 */
            var defer = script.get("../webservice/WebServiceMap.asmx/GetBaseMapLayer", {
                jsonp: "callback",
                timeout: 5000
            });
            /* 请求成功事件 */
            defer.then(function (suc) {
                that.baseinfos = suc;
                that.addBaseMap(suc);
            }, function (err) {
                console.log("底图服务GetMapLayer()出现错误，请查看后台日志！");
            });
        },
        attachEvent: function () {
            this.addBaseLayer();
            query(".jz-basemap-content ul li").on("click", lang.hitch(this, this.changeBaseMap));
        },
        addBaseLayer: function () {
            /* 初始范围 */
            var baseMap = this.baseinfos[0];
            var sr = new SpatialReference(baseMap.COORSYS);
            JZ.initExtent = new Extent(parseFloat(baseMap.XMIN), parseFloat(baseMap.YMIN), parseFloat(baseMap.XMAX), parseFloat(baseMap.YMAX), sr);
            var baseMapLayer = new VecLayer(baseMap.YEAR);
            this.map.addLayer(baseMapLayer);
            this.map.setExtent(JZ.initExtent);
            this.currentBaseLayerID = baseMap.ID;
            this.baseLayers.push(baseMapLayer);
        },
        changeBaseMap: function (e) {
            JZ.stopEvent(e); JZ.stopBubble(e);
            var that = this;
            var node = e.currentTarget;
            var basemapID = parseInt(domAttr.get(node, "data-dojo-value"));
            var type = domAttr.get(node, "data-dojo-type");

            if (that.currentBaseLayerID != basemapID && that.currentBaseLayerID !== -1) {
                arrayUtils.forEach(that.baseLayers, function (bl) {
                    that.map.removeLayer(bl);
                });
                that.baseLayers = [];
                var year = 2015;
                arrayUtils.forEach(that.baseinfos, function (layerInfo) {
                    //layerInfo.DATATYPE=0表示矢量底图,1表示卫片底图,2表示混合底图,3表示航片底图
                    if (layerInfo.ID === basemapID && layerInfo.DATATYPE === 0) {
                        var baseMapLayer = new VecLayer(layerInfo.YEAR);
                        that.map.addLayer(baseMapLayer, 0);
                        that.baseLayers.push(baseMapLayer);
                        that.currentBaseLayerID = layerInfo.ID;
                        year = layerInfo.YEAR;
                    } else if (layerInfo.ID === basemapID && layerInfo.DATATYPE === 1) {
                        var baseMapLayer = new ArcGISTiledMapServiceLayer(layerInfo.SERVICEURL);
                        that.map.addLayer(baseMapLayer, 0);
                        that.baseLayers.push(baseMapLayer);
                        arrayUtils.forEach(that.baseinfos, function (mixLayer) {
                            if (mixLayer.DATATYPE === 2 && mixLayer.YEAR === layerInfo.YEAR) {
                                var baseMapLayer1 = new ArcGISTiledMapServiceLayer(mixLayer.SERVICEURL);
                                that.map.addLayer(baseMapLayer1, 1);
                                that.baseLayers.push(baseMapLayer1);
                            }
                        });
                        that.currentBaseLayerID = layerInfo.ID;
                        year = layerInfo.YEAR;
                    } else if (layerInfo.ID === basemapID && layerInfo.DATATYPE === 3) {
                        var rc = new ImgLayer(layerInfo.YEAR);
                        that.map.addLayer(rc);
                        that.map.reorderLayer(rc, 0);
                        that.baseLayers.push(rc);
                        //arrayUtils.forEach(that.baseinfos, function (mixLayer) {
                        //    if (mixLayer.DATATYPE === 2 && mixLayer.YEAR === layerInfo.YEAR) {
                        //        var baseMapLayer1 = new ArcGISTiledMapServiceLayer(mixLayer.SERVICEURL);
                        //        that.map.addLayer(baseMapLayer1, 1);
                        //        that.baseLayers.push(baseMapLayer1);
                        //    }
                        //});
                        that.currentBaseLayerID = layerInfo.ID;
                        year = layerInfo.YEAR;
                    }
                });

                query(".jz-basemap-content ul li a").removeClass("selected");
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
        },
        addBaseMap: function (suc) {
            var that = this;
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
            domConstruct.place(htmlStr, that.jzBaseLayer);

            that.attachEvent();
        }
    });
    clazz.getInstance = function (map) {
        if (instance === null) {
            instance = new clazz({
                pNode: "mapTools",
                map: map
            });
        }
        return instance;
    };
    return clazz;
});