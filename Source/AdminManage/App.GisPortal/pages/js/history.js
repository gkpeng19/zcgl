require(["dojo/on", "dojo/_base/declare", "dojo/io-query", 
    "dojo/_base/event", "dojo/dom-attr", "dojo/dom-class", "dojo/when",
    "dojo/DeferredList", "dojo/query", "dojo/request/xhr", "dojo/_base/lang",
    "dijit/focus", "dojo/dom-construct", "dijit/registry", "esri/map", "esri/geometry/Extent",
    "esri/SpatialReference", "esri/layers/ArcGISTiledMapServiceLayer",
    "esri/symbols/PictureMarkerSymbol", "esri/graphic",
    "esri/geometry/Point", "esri/tasks/GeometryService", "esri/tasks/ProjectParameters",
    "esri/layers/DynamicMapServiceLayer", "dojo/_base/array", "dojo/dom-style",
    "dojo/dom", "dojo/dom-geometry", "dojo/sniff", "dojox/widget/Dialog",
    "widget/layer/VecLayer",
    "widget/layer/ImgLayer",
    "dojo/fx/easing", "dijit/layout/BorderContainer",
    "dijit/layout/LayoutContainer", "dijit/layout/TabContainer",
    "dijit/layout/AccordionContainer", "dijit/layout/ContentPane",
    "dijit/layout/AccordionPane",
    "dojo/domReady!"],
    function (on, declare, ioQuery, Event, domAttr, domClass,
        when, DeferredList, query, xhr, lang, focus, domConstruct, registry,
        Map, Extent, SpatialReference, ArcGISTiledMapServiceLayer, PictureMarkerSymbol, Graphic,
        Point, GeometryService, ProjectParameters,
        DynamicMapServiceLayer,
        arrayUtils, domStyle, dom, domGeometry, sniff, Dialog, VecLayer, ImgLayer) {

        //计算每个div占用的大小
        var box = dom.byId("mapBox");
        var eles = query("#mapBox .hc-map-size");
        var boxWidth = 0;
        if (box.currentStyle) {
            boxWidth = parseInt(box.currentStyle['width'], 10);
        }
        if (document.defaultView && document.defaultView.getComputedStyle) {
            var width = parseInt(boxWidth ? boxWidth : document.defaultView.getComputedStyle(box, false)['width'], 10);
            for (var i = eles.length - 1; i >= 0; i--) {
                eles[i].style.width = (width / eles.length) + 'px';
            };
        } else {
            var width = parseInt(boxWidth ? boxWidth : window.getComputedStyle(box)['width'], 10);
            for (var i = eles.length - 1; i >= 0; i--) {
                eles[i].style.width = (width / eles.length) + 'px';
            };
        }

    var gs = new GeometryService("http://10.246.0.83:6080/arcgis/rest/services/Utilities/Geometry/GeometryServer");
    var isExists = false;

    on(dom.byId("sel-view"), "change", function (e) {
        var value = parseInt(dom.byId("sel-view").options[dom.byId("sel-view").selectedIndex].value, 10);
        if (value === 1) {
            if (domClass.contains(dom.byId("mapBox1"), "show")) {
                domClass.remove(dom.byId("mapBox1"), "show");
            }
            domClass.add(dom.byId("mapBox"), "show");
        } else if (value === 2) {
            if (domClass.contains(dom.byId("mapBox"), "show")) {
                domClass.remove(dom.byId("mapBox"), "show");
            }
            domClass.add(dom.byId("mapBox1"), "show");
            if (!isExists) {
                //计算每个div占用的大小
                var box2 = dom.byId("mapBox2");
                var eles2 = query("#mapBox2 .hc-map-size"), eles3 = query("#mapBox3 .hc-map-size");
                var boxWidth = 0;
                if (box.currentStyle) {
                    boxWidth = parseInt(box.currentStyle['width'], 10);
                }
                var width = parseInt(boxWidth ? boxWidth : document.defaultView.getComputedStyle(box2, false)['width'], 10);
                for (var i = eles2.length - 1; i >= 0; i--) {
                    eles2[i].style.width = (width / eles2.length) + 'px';
                    eles3[i].style.width = (width / eles3.length) + 'px';
                };
                createFourMap();
            }
        }
    });

    function createFourMap() {
        resizeDiv(2);
        createMap("mapDiv3", initExtent, maps2, baseMap3, resizeFour, baseLayers1);
        createMap("mapDiv4", initExtent, maps2, baseMap4, resizeFour, baseLayers2);
        createMap("mapDiv5", initExtent, maps2, baseMap5, resizeFour, baseLayers3);
        createMap("mapDiv6", initExtent, maps2, baseMap6, resizeFour, baseLayers4);
        addMenuBar("basemapSwitcher3", "mapDiv3", 3);
        addMenuBar("basemapSwitcher4", "mapDiv4", 4);
        addMenuBar("basemapSwitcher5", "mapDiv5", 5);
        addMenuBar("basemapSwitcher6", "mapDiv6", 6);
        isExists = true;
    }

    function resizeDiv(tab) {
        //if (sniff("ie") <= 10) {
        //    if (tab === 1) {
        //        var node = query(".tab1")[0];
        //        dom.byId("map1C").style.height = domStyle.getComputedStyle(node).height;
        //        dom.byId("map2C").style.height = domStyle.getComputedStyle(node).height;
        //    } else if (tab === 2) {
        //        var node1 = query(".tab2")[0];
        //        var box = domGeometry.getMarginBox(node1, domStyle.getComputedStyle(node1));
        //        dom.byId("map3C").style.height = (box.h - 5) + "px";
        //        dom.byId("map4C").style.height = (box.h - 5) + "px";
        //        var node2 = query(".tab3")[0];
        //        var box1 = domGeometry.getMarginBox(node1, domStyle.getComputedStyle(node2));
        //        dom.byId("map5C").style.height = (box1.h - 5) + "px";
        //        dom.byId("map6C").style.height = (box1.h - 5) + "px";
        //    }
        //}
    }
    resizeDiv(1);
    window.onresize = resizeDiv;

    //var RcLayer = declare([DynamicMapServiceLayer], {
    //    constructor: function (extent) {
    //        this.initialExtent = this.fullExtent = extent;
    //        this.loaded = true;
    //        this.onLoad(this);
    //    },
    //    getImageUrl: function (extent, width, height, callback) {
    //        var params = {
    //            request: "GetMap",
    //            format: "JPEG",
    //            layers: "41",
    //            styles: "default",
    //            bbox: extent.xmin + "," + extent.ymin + "," + extent.xmax + "," + extent.ymax,
    //            srs: '',
    //            width: width,
    //            height: height
    //        };
    //        callback("http://172.24.57.47/service/RSImage/wms?username=syllhjxxjx&password=syllhjxxjx123&" + ioQuery.objectToQuery(params));
    //    }
    //});

    var maps1 = [];
    var maps2 = [];
    var active1 = false;
    var active2 = false;
    var baseLayerInfos = null, baseMap1 = null, baseMap2 = null, baseMap3 = null, baseMap4 = null, baseMap5 = null, baseMap6 = null, initExtent = null;

    function createMap(id, extent, maps, baselayer, resizeCb, baseLayers) {
        var map = new Map(id, {
            logo: false,
            fadeOnZoom: true,
            autoResize: true,
            extent: extent
        });
        on(map, "load", function () {
            map.disableScrollWheelZoom();
        });
        on(map, "pan-end", resizeCb);
        on(map, "zoom-end", resizeCb);
        map.addLayer(baselayer);
        baseLayers.push(baselayer);
        maps.push(map);
    }

    /* 获取数据库中配置的底图 */
    var defer = xhr.get("../webservice/WebServiceMap.asmx/GetBaseMapLayer", {
        handleAs: "json",
        timeout: 10000
    });

    /* 请求成功事件 */
    defer.then(function (suc) {
        baseLayerInfos = suc;
        showTwoMap();
    }, function (err) {
        console.log("底图服务GetMapLayer()出现错误，请查看后台日志！");
    });

    function showTwoMap() {
        var baseMapInfo = baseLayerInfos[0];
        var sr = new SpatialReference(baseMapInfo.COORSYS);
        initExtent = new Extent(parseFloat(baseMapInfo.XMIN), parseFloat(baseMapInfo.YMIN), parseFloat(baseMapInfo.XMAX), parseFloat(baseMapInfo.YMAX), sr);
        baseMap1 = new VecLayer(baseMapInfo.YEAR);
        baseMap2 = new VecLayer(baseMapInfo.YEAR);
        baseMap3 = new VecLayer(baseMapInfo.YEAR);
        baseMap4 = new VecLayer(baseMapInfo.YEAR);
        baseMap5 = new VecLayer(baseMapInfo.YEAR);
        baseMap6 = new VecLayer(baseMapInfo.YEAR);
        createMap("mapDiv1", initExtent, maps1, baseMap1, resizeTwo, baseLayers1);
        createMap("mapDiv2", initExtent, maps1, baseMap2, resizeTwo, baseLayers2);
        addMenuBar("basemapSwitcher1", "mapDiv1", 1);
        addMenuBar("basemapSwitcher2", "mapDiv2", 2);
    }

    function resizeTwo(options) {
        if (!active1) {
            active1 = true;
            var def = [];
            for (var i = 0; i < maps1.length; i++) {
                if (options.target.id !== maps1[i].id && options.target.loaded && options.target.extent !== maps1[i].extent) {
                    def.push(maps1[i].setExtent(options.target.extent));
                }
            }
            var defs = new DeferredList(def);
            defs.then(function () {
                active1 = false;
            });
        }
    }
    
    function goToLocation(e, flag) {
        Event.stop(e);
        var map = null;
        var lat = parseFloat(dom.byId("latCoor").value);
        var lon = parseFloat(dom.byId("lonCoor").value);
        if (flag === 0) {
            map = maps1[0];
        } else if (flag === 1) {
            map = maps2[0];
        }
        if (lat && lon) {
            var inSR = new SpatialReference({
                wkid: 4326
            });
            var point = new Point(lat, lon, inSR);
            var projectParams = new ProjectParameters();
            projectParams.geometries = [point];
            projectParams.outSR = map.spatialReference;
            gs.project(projectParams, function (suc) {
                map.graphics.clear();

                var fillSymbol = new PictureMarkerSymbol('/images/marker/red.png', 35, 42);
                var gra = new Graphic(suc[0], fillSymbol);
                map.graphics.add(gra);

                map.centerAndZoom(suc[0], 7);
            }, function (err) {
                console.log(err);
            });
        } else {
            alert("请输入正确的经纬度坐标！");
        }
    }

    on(dom.byId("btnStartLocate"), "click", function (e) {
        var value = parseInt(dom.byId("sel-view").options[dom.byId("sel-view").selectedIndex].value, 10);
        if (value === 1) {
            goToLocation(e, 0);
        } else if (value === 2) {
            goToLocation(e, 1);
        }
    });

    function resizeFour(options) {
        if (!active2) {
            active2 = true;
            var def = [];
            for (var i = 0; i < maps2.length; i++) {
                if (options.target.id !== maps2[i].id && options.target.loaded && options.target.extent !== maps2[i].extent) {
                    def.push(maps2[i].setExtent(options.target.extent));
                }
            }
            var defs = new DeferredList(def);
            defs.then(function () {
                active2 = false;
            });
        }
    }

    function addMenuBar(id, mapID, flag) {
        var htmlStr = "<ul>", vecHtml = "", imgHtml = "", mixHtml = "", vecs = [], imgs = [], mixs = [];

        arrayUtils.forEach(baseLayerInfos, function (baseLayerInfo) {
            if (baseLayerInfo.DATATYPE === 0) {
                vecs.push(baseLayerInfo);
            } else if (baseLayerInfo.DATATYPE === 1) {
                imgs.push(baseLayerInfo);
            } else if (baseLayerInfo.DATATYPE === 3) {
                mixs.push(baseLayerInfo);
            } else if (baseLayerInfo.DATATYPE === 2) {
                mixvec = baseLayerInfo;
            }
        });

        var vLength = vecs.length, iLength = imgs.length, mLength = mixs.length;

        for (var v = 0; v < vLength; v++) {
            var baseLayerInfo = vecs[v];
            if (v === 0) {
                vecHtml += "<li data-dojo-node='" + mapID + "' data-dojo-type='0' data-dojo-value='" + baseLayerInfo.ID + "'><a class='basemap selected' title='矢量' style='border-left:1px #B0B0AE solid'><span id='" + id + "_veclabel'>矢量(" + baseLayerInfo.YEAR + ")</span></a>";
                vecHtml += "<ul><li data-dojo-node='" + mapID + "' data-dojo-value='" + baseLayerInfo.ID + "'><a class='basemap'>" + baseLayerInfo.YEAR + "</a></li>";
            } else if (v > 0) {
                vecHtml += "<li data-dojo-node='" + mapID + "' data-dojo-value='" + baseLayerInfo.ID + "'><a class='basemap'>" + baseLayerInfo.YEAR + "</a></li>";
            }
        }
        vecHtml += "</ul></li>";

        for (var i = 0; i < iLength; i++) {
            var baseLayerInfo = imgs[i];
            if (i === 0) {
                imgHtml += "<li data-dojo-node='" + mapID + "' data-dojo-type='1' data-dojo-value='" + baseLayerInfo.ID + "'><a class='basemap' title='卫片'><span id='" + id + "_imglabel'>卫片(" + baseLayerInfo.YEAR + ")</span></a>";
                imgHtml += "<ul><li data-dojo-node='" + mapID + "' data-dojo-value='" + baseLayerInfo.ID + "'><a class='basemap'>" + baseLayerInfo.YEAR + "</a></li>";
            } else if (i > 0) {
                imgHtml += "<li data-dojo-node='" + mapID + "' data-dojo-value='" + baseLayerInfo.ID + "'><a class='basemap'>" + baseLayerInfo.YEAR + "</a></li>";
            }
        }
        imgHtml += "</ul></li>";

        for (var m = 0; m < mLength; m++) {
            var baseLayerInfo = mixs[m];
            if (m === 0) {
                imgHtml += "<li data-dojo-node='" + mapID + "' data-dojo-type='2' data-dojo-value='" + baseLayerInfo.ID + "'><a class='basemap' title='航片'><span id='" + id + "_mixlabel'>航片(" + baseLayerInfo.YEAR + ")</span></a>";
                imgHtml += "<ul><li data-dojo-node='" + mapID + "' data-dojo-value='" + baseLayerInfo.ID + "'><a class='basemap'>" + baseLayerInfo.YEAR + "</a></li>";
            } else if (m > 0) {
                imgHtml += "<li data-dojo-node='" + mapID + "' data-dojo-value='" + baseLayerInfo.ID + "'><a class='basemap'>" + baseLayerInfo.YEAR + "</a></li>";
            }
        }
        mixHtml += "</ul></li>";
        htmlStr += vecHtml + imgHtml + mixHtml + "</ul>";
        domConstruct.place(htmlStr, id);

        var nodeList = query("#" + id + " ul li");
        switch (flag) {
            case 1:
                nodeList.on("click", changeBaseMap1);
                break;
            case 2:
                nodeList.on("click", changeBaseMap2);
                break;
            case 3:
                nodeList.on("click", changeBaseMap3);
                break;
            case 4:
                nodeList.on("click", changeBaseMap4);
                break;
            case 5:
                nodeList.on("click", changeBaseMap5);
                break;
            case 6:
                nodeList.on("click", changeBaseMap6);
                break;
        }
    }

    function changeLabel(that, nodeID) {
        if (that.innerHTML == '全屏') {
            that.innerHTML = "取消全屏";
            showFull(nodeID);
        } else {
            that.innerHTML = "全屏";
            unShowFull(nodeID);
        }
    }

    function showFull(nodeID) {
        var node = dom.byId(nodeID);
        if (!domClass.contains(node, "full")) {
            domClass.add(node, "full");
        }
    }

    function unShowFull(nodeID) {
        var node = dom.byId(nodeID);
        if (domClass.contains(node, "full")) {
            domClass.remove(node, "full");
        }
    }

    on(dom.byId("oneMapScreen"), "click", function () {
        if (this.innerHTML == '全屏') {
            if (!domClass.contains(dom.byId("map2C"), "hide")) {
                domClass.add(dom.byId("map2C"), "hide");
            }
            this.innerHTML = "取消全屏";
            showFull("map1C");
        } else {
            if (domClass.contains(dom.byId("map2C"), "hide")) {
                domClass.remove(dom.byId("map2C"), "hide");
            }
            this.innerHTML = "全屏";
            unShowFull("map1C");
        }
        for (var i = 0; i < maps1.length; i++) {
            if ("mapDiv1" === maps1[i].id) {
                maps1[i].resize();
            }
        }
    });
    on(dom.byId("twoMapScreen"), "click", function () {
        if (this.innerHTML == '全屏') {
            if (!domClass.contains(dom.byId("map1C"), "hide")) {
                domClass.add(dom.byId("map1C"), "hide");
            }
            this.innerHTML = "取消全屏";
            showFull("map2C");
        } else {
            if (domClass.contains(dom.byId("map1C"), "hide")) {
                domClass.remove(dom.byId("map1C"), "hide");
            }
            this.innerHTML = "全屏";
            unShowFull("map2C");
        }
        for (var i = 0; i < maps1.length; i++) {
            if ("mapDiv2" === maps1[i].id) {
                maps1[i].resize();
            }
        }
    });

    function setDisplayNone(divId) {
        var node3 = dom.byId("map3C");
        var node4 = dom.byId("map4C");
        var node5 = dom.byId("map5C");
        var node6 = dom.byId("map6C");
        if (divId != "map3C") {
            if (!domClass.contains(node3, "hide")) {
                domClass.add(node3, "hide");
            }
        }
        if (divId != "map4C") {
            if (!domClass.contains(node4, "hide")) {
                domClass.add(node4, "hide");
            }
        }
        if (divId != "map5C") {
            if (!domClass.contains(node5, "hide")) {
                domClass.add(node5, "hide");
            }
        }
        if (divId != "map6C") {
            if (!domClass.contains(node6, "hide")) {
                domClass.add(node6, "hide");
            }
        }
    }
    function setDisplayVisible() {
        var node3 = dom.byId("map3C");
        var node4 = dom.byId("map4C");
        var node5 = dom.byId("map5C");
        var node6 = dom.byId("map6C");
        if (domClass.contains(node3, "hide")) {
            domClass.remove(node3, "hide");
        }
        if (domClass.contains(node4, "hide")) {
            domClass.remove(node4, "hide");
        }
        if (domClass.contains(node5, "hide")) {
            domClass.remove(node5, "hide");
        }
        if (domClass.contains(node6, "hide")) {
            domClass.remove(node6, "hide");
        }
    }

    on(dom.byId("threeMapScreen"), "click", function () {
        if (this.innerHTML == '全屏') {
            setDisplayNone("map3C");
            this.innerHTML = "取消全屏";
            showFull("map3C");
            domStyle.set(dom.byId("mapBox2"), "height", "98%");
            domStyle.set(dom.byId("mapBox3"), "height", "0");
        } else {
            setDisplayVisible();
            this.innerHTML = "全屏";
            unShowFull("map3C");
            domStyle.set(dom.byId("mapBox2"), "height", "49%");
            domStyle.set(dom.byId("mapBox3"), "height", "49%");
        }
        for (var i = 0; i < maps2.length; i++) {
            if ("mapDiv3" === maps2[i].id) {
                maps2[i].resize();
            }
        }
    });
    on(dom.byId("fourMapScreen"), "click", function () {
        if (this.innerHTML == '全屏') {
            setDisplayNone("map4C");
            this.innerHTML = "取消全屏";
            showFull("map4C");
            domStyle.set(dom.byId("mapBox2"), "height", "98%");
            domStyle.set(dom.byId("mapBox3"), "height", "0");
        } else {
            setDisplayVisible();
            this.innerHTML = "全屏";
            unShowFull("map4C");
            domStyle.set(dom.byId("mapBox2"), "height", "49%");
            domStyle.set(dom.byId("mapBox3"), "height", "49%");
        }
        for (var i = 0; i < maps2.length; i++) {
            if ("mapDiv4" === maps2[i].id) {
                maps2[i].resize();
            }
        }
    });
    on(dom.byId("fiveMapScreen"), "click", function () {
        if (this.innerHTML == '全屏') {
            setDisplayNone("map5C");
            this.innerHTML = "取消全屏";
            showFull("map5C");
            domStyle.set(dom.byId("mapBox3"), "height", "98%");
            domStyle.set(dom.byId("mapBox2"), "height", "0");
        } else {
            setDisplayVisible();
            this.innerHTML = "全屏";
            unShowFull("map5C");
            domStyle.set(dom.byId("mapBox3"), "height", "49%");
            domStyle.set(dom.byId("mapBox2"), "height", "49%");
        }
        for (var i = 0; i < maps2.length; i++) {
            if ("mapDiv5" === maps2[i].id) {
                maps2[i].resize();
            }
        }
    });
    on(dom.byId("sixMapScreen"), "click", function () {
        if (this.innerHTML == '全屏') {
            setDisplayNone("map6C");
            this.innerHTML = "取消全屏";
            showFull("map6C");
            domStyle.set(dom.byId("mapBox3"), "height", "98%");
            domStyle.set(dom.byId("mapBox2"), "height", "0");
        } else {
            setDisplayVisible();
            this.innerHTML = "全屏";
            unShowFull("map6C");
            domStyle.set(dom.byId("mapBox3"), "height", "49%");
            domStyle.set(dom.byId("mapBox2"), "height", "49%");
        }
        for (var i = 0; i < maps2.length; i++) {
            if ("mapDiv6" === maps2[i].id) {
                maps2[i].resize();
            }
        }
    });

    var baseLayers1 = [], baseLayers2 = [], baseLayers3 = [], baseLayers4 = [], baseLayers5 = [], baseLayers6 = [], currentID1 = -1, currentID2 = -1, currentID3 = -1, currentID4 = -1, currentID5 = -1, currentID6 = -1;

    function addLayer(myMap, baseLayers, basemapID, currentBaseID) {
        var year = 2015;
        arrayUtils.forEach(baseLayerInfos, function (layerInfo) {
            //layerInfo.DATATYPE=0表示矢量底图,1表示卫片底图,2表示混合底图,3表示航片底图
            if (layerInfo.ID === basemapID && layerInfo.DATATYPE === 0) {
                var baseMapLayer = new VecLayer(layerInfo.YEAR);
                myMap.addLayer(baseMapLayer, 0);
                baseLayers.push(baseMapLayer);
                currentBaseID = layerInfo.ID;
                year = layerInfo.YEAR;
            } else if (layerInfo.ID === basemapID && layerInfo.DATATYPE === 1) {
                var baseMapLayer = new ArcGISTiledMapServiceLayer(layerInfo.SERVICEURL);
                myMap.addLayer(baseMapLayer, 0);
                baseLayers.push(baseMapLayer);
                arrayUtils.forEach(baseLayerInfos, function (mixLayer) {
                    if (mixLayer.DATATYPE === 2 && mixLayer.YEAR === layerInfo.YEAR) {
                        var baseMapLayer1 = new ArcGISTiledMapServiceLayer(mixLayer.SERVICEURL);
                        myMap.addLayer(baseMapLayer1, 1);
                        baseLayers.push(baseMapLayer1);
                    }
                });
                currentBaseID = layerInfo.ID;
                year = layerInfo.YEAR;
            }  else if (layerInfo.ID === basemapID && layerInfo.DATATYPE === 3) {
                var rc = new ImgLayer(layerInfo.YEAR);
                myMap.addLayer(rc);
                myMap.reorderLayer(rc, 0);
                baseLayers.push(rc);
                currentBaseID = layerInfo.ID;
                year = layerInfo.YEAR;
            }
        });
        return year;
    }

    function setSelected(node, switcherID, year) {
        var type = domAttr.get(node.parentNode.parentNode, "data-dojo-type");
        var nodes = query("#" + switcherID + " > ul > li > a").removeClass("selected");
        switch (type) {
            case "0":
                dom.byId(switcherID + "_veclabel").innerText = "矢量(" + year + ")";
                domClass.add(nodes[0], "selected");
                break;
            case "1":
                dom.byId(switcherID + "_imglabel").innerText = "卫片(" + year + ")";
                domClass.add(nodes[1], "selected");
                break;
            case "2":
                dom.byId(switcherID + "_mixlabel").innerText = "航片(" + year + ")";
                domClass.add(nodes[2], "selected");
                break;
        }
    }

    function changeBaseMap1(e) {
        var result = changeBaseMap(e);
        if (!result.TYPE && currentID1 !== result.BASEMAP) {
            result.MAP.removeAllLayers();
            baseLayers1 = [];
            var year = addLayer(result.MAP, baseLayers1, result.BASEMAP, currentID1);
            setSelected(result.NODE, "basemapSwitcher1", year);
        }
    }

    function changeBaseMap2(e) {
        var result = changeBaseMap(e);
        if (!result.TYPE && currentID2 !== result.BASEMAP) {
            //removeBaseLayers(result.MAP, baseLayers2);
            result.MAP.removeAllLayers();
            baseLayers2 = [];
            var year = addLayer(result.MAP, baseLayers2, result.BASEMAP, currentID2);
            setSelected(result.NODE, "basemapSwitcher2", year);
        }
    }

    function changeBaseMap3(e) {
        var result = changeBaseMap(e);
        if (!result.TYPE && currentID3 !== result.BASEMAP) {
            //removeBaseLayers(result.MAP, baseLayers3);
            result.MAP.removeAllLayers();
            baseLayers3 = [];
            var year = addLayer(result.MAP, baseLayers3, result.BASEMAP, currentID3);
            setSelected(result.NODE, "basemapSwitcher3", year);
        }
    }

    function changeBaseMap4(e) {
        var result = changeBaseMap(e);
        if (!result.TYPE && currentID4 !== result.BASEMAP) {
            //removeBaseLayers(result.MAP, baseLayers4);
            result.MAP.removeAllLayers();
            baseLayers4 = [];
            var year = addLayer(result.MAP, baseLayers4, result.BASEMAP, currentID4);
            setSelected(result.NODE, "basemapSwitcher4", year);
        }
    }

    function changeBaseMap5(e) {
        var result = changeBaseMap(e);
        if (!result.TYPE && currentID5 !== result.BASEMAP) {
            //removeBaseLayers(result.MAP, baseLayers5);
            result.MAP.removeAllLayers();
            baseLayers5 = [];
            var year = addLayer(result.MAP, baseLayers5, result.BASEMAP, currentID5);
            setSelected(result.NODE, "basemapSwitcher5", year);
        }
    }

    function changeBaseMap6(e) {
        var result = changeBaseMap(e);
        if (!result.TYPE && currentID6 !== result.BASEMAP) {
            //removeBaseLayers(result.MAP, baseLayers6);
            result.MAP.removeAllLayers();
            baseLayers6 = [];
            var year = addLayer(result.MAP, baseLayers6, result.BASEMAP, currentID6);
            setSelected(result.NODE, "basemapSwitcher6", year);
        }
    }

    function changeBaseMap(e) {
        Event.stop(e);
        var node = e.currentTarget;
        var basemapID = parseInt(domAttr.get(node, "data-dojo-value"));
        var mapID = domAttr.get(node, "data-dojo-node");
        var type = domAttr.get(node, "data-dojo-type");

        var map = null;
        for (var i = 0; i < maps2.length; i++) {
            if (mapID === maps2[i].id) {
                map = maps2[i];
            }
        }

        for (var i = 0; i < maps1.length; i++) {
            if (mapID === maps1[i].id) {
                map = maps1[i];
            }
        }
        return {
            MAP: map,
            NODE: node,
            BASEMAP: basemapID,
            TYPE: type
        };
    }
});