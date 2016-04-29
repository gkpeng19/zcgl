require(["dojo/on", "dojo/_base/declare", "dojo/io-query",
    "dojo/_base/event", "dojo/dom-attr", "dojo/dom-class", "dojo/when",
    "dojo/DeferredList", "dojo/query", "dojo/request/xhr", "dojo/_base/lang",
    "dojo/dom-construct", "esri/map", "esri/geometry/Extent",
    "esri/SpatialReference", "esri/layers/ArcGISTiledMapServiceLayer",
    "esri/symbols/SimpleLineSymbol", "esri/symbols/SimpleFillSymbol",
    "esri/graphic", 'esri/Color',
    "esri/geometry/Point",  "dojo/_base/array", "dojo/dom-style",
    "dojo/dom", "dojox/widget/Dialog",
    "widget/layer/VecLayer",
    "widget/layer/ImgLayer",
    'esri/tasks/QueryTask',
     'esri/tasks/query',
     'esri/toolbars/draw',
     'widget/dijit/LoadingIndicator',
    "dojo/domReady!"],
    function (on, declare, ioQuery, Event, domAttr, domClass,
        when, DeferredList, query, xhr, lang, domConstruct,
        Map, Extent, SpatialReference, ArcGISTiledMapServiceLayer, SimpleLineSymbol, SimpleFillSymbol, Graphic, Color,
        Point, arrayUtils, domStyle, dom, Dialog, VecLayer, ImgLayer, QueryTask, EsriQuery, DrawTool, LoadingIndicator) {
        var isExists = false, active1 = false;
        var sls = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 0, 0, 0.5]), 3);
        var sfs = new SimpleFillSymbol(SimpleFillSymbol.STYLE_SOLID, sls, new Color([200, 200, 200, 0]));
        var baseLayerInfos = null, baseMap1 = null, baseMap2 = null, initExtent = null;
        var maps1 = [];
        var featureAll = [];
        /* 获取数据库中配置的底图 */
        var defer = xhr.get("../webservice/WebServiceMap.asmx/GetBaseMapLayer", {
            handleAs: "json",
            timeout: 10000
        });
        /* 请求成功事件 */
        defer.then(function (suc) {
            baseLayerInfos = suc;
            getArea();
            //showTwoMap();
        }, function (err) {
            console.log("底图服务GetMapLayer()出现错误，请查看后台日志！");
        });

        function getArea() {
            var featureUrl = "http://10.246.0.83:6080/arcgis/rest/services/feature/beijing/MapServer/74";
            var queryTask = new QueryTask(featureUrl);
            var esriQuery = new EsriQuery();
            esriQuery.where = "OBJECTID <> 0";
            esriQuery.returnGeometry = true;
            esriQuery.outFields = ["*"];
            queryTask.execute(esriQuery, function (featureSets) {
                featureAll = featureSets.features;
                var select = document.createElement("select");
                select.id = "selectArea";
                select.className = "form-control";
                var option = document.createElement("option");
                option.value = -1;
                option.appendChild(document.createTextNode("请选择风景区"));
                select.appendChild(option);
                arrayUtils.forEach(featureAll, function (feature, index) {
                    if (feature.attributes["OBJNAME"]) {
                        var option = document.createElement("option");
                        option.value = index;
                        option.appendChild(document.createTextNode(feature.attributes["OBJNAME"]));
                        select.appendChild(option);
                    }
                });
                document.getElementById("tabAreaFeatures").appendChild(select);
                showTwoMap();
                on(select, "change", selectArea)
            }, function (err) {
                console.log("风景区获取失败，请确认网络连接正常后继续！");
                showLogin();
            });
        }

        function showLogin() {
            window.location.href = "Login.aspx?ReturnUrl=" + window.location.href;
        }

        var selectGraphic, currentGeometry;

        function selectArea() {
            var select = document.getElementById("selectArea");
            var selectValue = select.options[select.selectedIndex].value;
            arrayUtils.forEach(featureAll, function (feature, index) {
                if (selectValue == index) {
                    arrayUtils.forEach(maps1, function (map) {
                        map.graphics.clear();
                        selectGraphic = null;
                        selectGraphic = new Graphic(feature.geometry, sfs);
                        map.graphics.add(selectGraphic);
                        currentGeometry = feature.geometry;
                        var extent = feature.geometry.getExtent();
                        map.setExtent(extent);
                    });
                }
            });
        }

        function showTwoMap() {
            var baseMapInfo = baseLayerInfos[0];
            var sr = new SpatialReference(baseMapInfo.COORSYS);
            initExtent = new Extent(parseFloat(baseMapInfo.XMIN), parseFloat(baseMapInfo.YMIN), parseFloat(baseMapInfo.XMAX), parseFloat(baseMapInfo.YMAX), sr);
            baseMap1 = new VecLayer(baseMapInfo.YEAR);
            baseMap2 = new VecLayer(baseMapInfo.YEAR);
            createMap("mapDiv1", initExtent, maps1, baseMap1, resizeTwo, baseLayers1);
            createMap("mapDiv2", initExtent, maps1, baseMap2, resizeTwo, baseLayers2);

            map1ID = baseMapInfo.ID;
            map1Year = baseMapInfo.YEAR;
            map1Type = baseMapInfo.DATATYPE;
            map2ID = baseMapInfo.ID;
            map2Year = baseMapInfo.YEAR;
            map2Type = baseMapInfo.DATATYPE;

            addMenuBar("mapTools1", "mapDiv1", 1);
            addMenuBar("mapTools2", "mapDiv2", 2);
        }
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
            }
        }

        var baseLayers1 = [], baseLayers2 = [], currentID1 = -1, currentID2 = -1, map1ID, map1Year, map1Type, map2ID, map2Year, map2Type;
        function addLayer(myMap, baseLayers, basemapID, currentBaseID, flag) {
            var year = 2015;
            arrayUtils.forEach(baseLayerInfos, function (layerInfo) {
                //layerInfo.DATATYPE=0表示矢量底图,1表示卫片底图,2表示混合底图,3表示航片底图
                if (layerInfo.ID === basemapID && layerInfo.DATATYPE === 0) {
                    var baseMapLayer = new VecLayer(layerInfo.YEAR);
                    myMap.addLayer(baseMapLayer, 0);
                    baseLayers.push(baseMapLayer);
                    currentBaseID = layerInfo.ID;
                    year = layerInfo.YEAR;
                    if (flag == 1) {
                        map1ID = layerInfo.ID;
                        map1Year = layerInfo.YEAR;
                        map1Type = layerInfo.DATATYPE;
                    } else if (flag == 2) {
                        map2ID = layerInfo.ID;
                        map2Year = layerInfo.YEAR;
                        map2Type = layerInfo.DATATYPE;
                    }
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

                    if (flag == 1) {
                        map1ID = layerInfo.ID;
                        map1Year = layerInfo.YEAR;
                        map1Type = layerInfo.DATATYPE;
                    } else if (flag == 2) {
                        map2ID = layerInfo.ID;
                        map2Year = layerInfo.YEAR;
                        map2Type = layerInfo.DATATYPE;
                    }
                } else if (layerInfo.ID === basemapID && layerInfo.DATATYPE === 3) {
                    var rc = new ImgLayer(layerInfo.YEAR);
                    myMap.addLayer(rc);
                    myMap.reorderLayer(rc, 0);
                    baseLayers.push(rc);
                    currentBaseID = layerInfo.ID;
                    year = layerInfo.YEAR;

                    if (flag == 1) {
                        map1ID = layerInfo.ID;
                        map1Year = layerInfo.YEAR;
                        map1Type = layerInfo.DATATYPE;
                    } else if (flag == 2) {
                        map2ID = layerInfo.ID;
                        map2Year = layerInfo.YEAR;
                        map2Type = layerInfo.DATATYPE;
                    }
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
                var year = addLayer(result.MAP, baseLayers1, result.BASEMAP, currentID1, 1);
                setSelected(result.NODE, "mapTools1", year);
            }
        }

        function changeBaseMap2(e) {
            var result = changeBaseMap(e);
            if (!result.TYPE && currentID2 !== result.BASEMAP) {
                //removeBaseLayers(result.MAP, baseLayers2);
                result.MAP.removeAllLayers();
                baseLayers2 = [];
                var year = addLayer(result.MAP, baseLayers2, result.BASEMAP, currentID2, 2);
                setSelected(result.NODE, "mapTools2", year);
            }
        }

        function changeBaseMap(e) {
            Event.stop(e);
            var node = e.currentTarget;
            var basemapID = parseInt(domAttr.get(node, "data-dojo-value"));
            var mapID = domAttr.get(node, "data-dojo-node");
            var type = domAttr.get(node, "data-dojo-type");
            var map = null;
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

        on(dom.byId("aAreaContrast"), "click", function () {
            if (!domClass.contains(dom.byId("liAreaContrast"), "active")) {
                domClass.add(dom.byId("liAreaContrast"), "active");
            }

            if (domClass.contains(dom.byId("liCustomContrast"), "active")) {
                domClass.remove(dom.byId("liCustomContrast"), "active");
            }

            if (!domClass.contains(dom.byId("tabAreaContrast"), "active")) {
                domClass.add(dom.byId("tabAreaContrast"), "active");
            }

            if (domClass.contains(dom.byId("tabCustomContrast"), "active")) {
                domClass.remove(dom.byId("tabCustomContrast"), "active");
            }
            if (drawTool1) {
                drawTool1.deactivate();
            }
            if (drawTool2) {
                drawTool2.deactivate();
            }
            if (customGraphic) {
                maps1[0].graphics.remove(customGraphic);
                maps1[1].graphics.remove(customGraphic);
            }
        });
        on(dom.byId("aCustomContrast"), "click", function () {
            if (domClass.contains(dom.byId("liAreaContrast"), "active")) {
                domClass.remove(dom.byId("liAreaContrast"), "active");
            }
            if (!domClass.contains(dom.byId("liCustomContrast"), "active")) {
                domClass.add(dom.byId("liCustomContrast"), "active");
            }
            if (domClass.contains(dom.byId("tabAreaContrast"), "active")) {
                domClass.remove(dom.byId("tabAreaContrast"), "active");
            }
            if (!domClass.contains(dom.byId("tabCustomContrast"), "active")) {
                domClass.add(dom.byId("tabCustomContrast"), "active");
            }
            maps1[0].graphics.clear();
            maps1[1].graphics.clear();
            selectGraphic = null;
            currentGeometry = null;
            document.getElementById("selectArea").selectedIndex = 0;
        });

        function getChecked() {
            if (map1ID === map2ID) {
                alert("不允许完全相同的两个底图进行对比分析！");
                return false;
            }

            if (map1Type !== map2Type) {
                alert("不允许不同类型的底图进行对比分析！");
                return false;
            }

            return true;
        }

        //var timer = null;
        var loadingBar = new LoadingIndicator();
        domConstruct.place(loadingBar.domNode, document.body);

        //区域对比开始
        on(dom.byId("btnAreaStartAnalyst"), "click", function () {
            if (selectGraphic && currentGeometry) {
                //开始比对
                if (getChecked()) {
                    loadingBar.show();
                    var map = maps1[1];
                    var extent = currentGeometry.getExtent();
                    var index = document.getElementById("selectArea").selectedIndex;
                    var xhrAreaStart = xhr.get("../webservice/WebServiceMap.asmx/GetTileUrl", {
                        handleAs: "json",
                        timeout: 300000,
                        query: {
                            level: map.getZoom(), minx: extent.xmin, miny: extent.ymin, maxx: extent.xmax, maxy: extent.ymax,
                            type: map1Type, year1: map1Year, year2: map2Year, objid:index
                        }
                    });
                    /* 请求成功事件 */
                    xhrAreaStart.then(function (suc) {
                        //timer = window.setTimeout(function () {
                            if (suc) {
                                window.open("result.html?id=" + suc);
                            } else {
                                alert("两个区块完全相同！");
                            }
                            loadingBar.hide();
                        //}, 100)
                    }, function (err) {
                        //timer = window.setTimeout(function () {
                            if (err.response.text) {
                                window.open("result.html?id=" + err.response.text);
                            }
                            loadingBar.hide();
                        //}, 100)
                    });
                }
            } else {
                alert("请选择风景区后开始比对！");
            }
        });
        //区域对比结束
        //on(dom.byId("btnAreaEndAnalyst"), "click", function () {
        //    if (xhrAreaStart) {
        //        xhrAreaStart.cancel("用户停止");
        //    }
        //    if (timer) {
        //        window.clearTimeout(timer);
        //    }
        //});
        //区域对比标记
        //on(dom.byId("btnAreaMarker"), "click", function () {

        //});
        //区域对比清除结果
        //on(dom.byId("btnAreaClear"), "click", function () {

        //});
        var drawTool1, drawTool2, drawGeometry;
        //自定义比对选择区域
        on(dom.byId("btnCustomSelect"), "click", function () {
            if (!drawTool1) {
                drawTool1 = new DrawTool(maps1[0]);
                on(drawTool1, "draw-end", function (feature) {
                    drawGeometry = feature.geometry;
                    customGraphic = new Graphic(drawGeometry, sfs);
                    maps1[0].graphics.add(customGraphic);
                    drawTool1.deactivate();

                    maps1[0].setExtent(drawGeometry.getExtent());
                });
            }
            if (!drawTool2) {
                drawTool2 = new DrawTool(maps1[1]);
                on(drawTool2, "draw-end", function (feature) {
                    drawGeometry = feature.geometry;
                    customGraphic = new Graphic(drawGeometry, sfs);
                    maps1[1].graphics.add(customGraphic);
                    drawTool2.deactivate();

                    maps1[1].setExtent(drawGeometry.getExtent());
                });
            }
            drawTool1.activate(DrawTool.RECTANGLE);
            drawTool2.activate(DrawTool.RECTANGLE);
        });
        //自定义比对开始
        on(dom.byId("btnCustomStart"), "click", function () {
            if (drawGeometry) {
                //开始比对
                if (getChecked()) {
                    loadingBar.show();
                    var map = maps1[1];
                    var extent = drawGeometry.getExtent();
                    var xhrAreaStart = xhr.get("../webservice/WebServiceMap.asmx/GetTileUrl", {
                        handleAs: "json",
                        timeout: 300000,
                        query: {
                            level: map.getZoom(), minx: extent.xmin, miny: extent.ymin, maxx: extent.xmax, maxy: extent.ymax,
                            type: map1Type, year1: map1Year, year2: map2Year, objid:-1
                        }
                    });
                    /* 请求成功事件 */
                    xhrAreaStart.then(function (suc) {
                        //timer = window.setTimeout(function () {
                            if (suc) {
                                window.open("result.html?id=" + suc);
                            } else {
                                alert("两个区块完全相同！");
                            }
                            loadingBar.hide();
                        //}, 100)
                    }, function (err) {
                        //timer = window.setTimeout(function () {
                            if (err.response.text) {
                                window.open("result.html?id=" + err.response.text);
                            }
                            loadingBar.hide();
                        //}, 100)
                    });
                }
            } else {
                alert("请绘制一个区域后开始比对！");
            }
        });
        //自定义对比标记
        //on(dom.byId("btnCustomMarker"), "click", function () {

        //});
        //自定义对比清除结果
        //on(dom.byId("btnCustomClear"), "click", function () {

        //});
    });