require([
    "dojo/_base/array",
    "dojo/_base/window",
    "dojo/_base/lang",
    "dojo/dom",
    "dojo/_base/fx",
    "dojo/fx",
    "dojo/dom-style",
    "dojo/dom-class",
    "dojo/dom-construct",
    "dojo/on",
    "dojo/query",
    "dojo/topic",
    "dojo/request/xhr",
    "dojo/data/ItemFileWriteStore",
    "dijit/registry",
    "dijit/Tree",
    "dijit/tree/ForestStoreModel",
    "dojox/fx/style",
    "dojox/charting/Chart",
    "dojox/charting/plot2d/Pie",
    "dojox/charting/plot2d/Columns",
    "dojox/charting/action2d/Highlight",
    "dojox/charting/action2d/MoveSlice",
    "dojox/charting/action2d/Tooltip",
    "dojox/charting/action2d/Shake",
    "dojox/charting/action2d/Magnify",
    "dojox/charting/themes/ThreeD",
    "dojox/charting/themes/CubanShirts",
    "dojox/charting/themes/PlotKit/blue",
    "dojox/charting/widget/SelectableLegend",
    "esri/map",
    "esri/Color",
    "esri/graphic",
    "esri/SpatialReference",
    "esri/dijit/Scalebar",
    "esri/dijit/OverviewMap",
    "esri/geometry/Extent",
    "esri/geometry/Point",
    "esri/layers/GraphicsLayer",
    "esri/layers/FeatureLayer",
    "esri/layers/ArcGISTiledMapServiceLayer",
    "esri/layers/ArcGISDynamicMapServiceLayer",
    "esri/symbols/SimpleFillSymbol",
    "esri/symbols/SimpleLineSymbol",
    "esri/symbols/SimpleMarkerSymbol",
    "esri/renderers/ClassBreaksRenderer",
    "esri/toolbars/navigation",
    'esri/tasks/query',
    'esri/tasks/QueryTask',
    "esri/renderers/ClassBreaksRenderer",
    "widget/layer/BaseMap",
    "widget/layer/VecLayer",
    "widget/navigation/Nav",
    "widget/cluster/ClusterFeatureLayer",
    "dojo/domReady!"],
    function (arrayUtils, baseWin, baseLang, dom, baseFx, fx, domStyle, domClass, domConstruct, on, query, topic, xhr,
        ItemFileWriteStore, registry, Tree, ForestStoreModel, fxStyle, Chart, Pie, Columns, Highlight, MoveSlice,
        Tooltip, Shake, Magnify, ThreeD, CubanShirts, Blue, SelectableLegend, Map, Color, Graphic, SpatialReference, Scalebar, OverviewMap,
        Extent, Point, GraphicsLayer, FeatureLayer, ArcGISTiledMapServiceLayer, ArcGISDynamicMapServiceLayer,
        SimpleFillSymbol, SimpleLineSymbol, SimpleMarkerSymbol, ClassBreaksRenderer,
        EsriNavigation, Query, QueryTask, ClassBreaksRenderer, BaseMap, VecLayer, Nav, ClusterFeatureLayer) {

        xhr.get("../webservice/WebServiceMap.asmx/GetLoginUser", {
            handleAs: "json",
            timeout: 10000
        }).then(function (suc) {
            dom.byId("currentUserName").innerHTML = suc.User.UserName;
        });

        /* 面板的关闭和打开事件 */
        var clickEvt = on.pausable(dom.byId("collpse-left"), "click", function (e) {
            clickEvt.pause();
            var left = dom.byId("leftNav");
            var right = dom.byId("rightNav");
            var leftMargin = domStyle.get(left, "left");
            if (leftMargin === 0) {
                fx.combine([
                    baseFx.animateProperty({
                        duration: 1000,
                        node: left,
                        properties: {
                            left: { start: 0, end: 0 - 315 }
                        }
                    }),
                    baseFx.animateProperty({
                        duration: 1000,
                        node: right,
                        properties: {
                            left: { start: 0 + 315, end: 0 }
                        },
                        onEnd: function () {
                            JZ.mc.resize(true);
                            clickEvt.resume();
                        }
                    })
                ]).play();
            } else {
                fx.combine([
                    baseFx.animateProperty({
                        duration: 1000,
                        node: left,
                        properties: {
                            left: { start: 0 - 315, end: 0 }
                        }
                    }),
                    baseFx.animateProperty({
                        duration: 1000,
                        node: right,
                        properties: {
                            left: { start: 0, end: 0 + 315 }
                        },
                        onEnd: function () {
                            JZ.mc.resize(true);
                            clickEvt.resume();
                        }
                    })
                ]).play();
            }
        });

        /* 图例 */
        on(dom.byId("thematicLegend"), "click", function (e) {
            require(['widget/dijit/Legend'], function (clazz) {
                clazz.getInstance().startup(layerInfo);
            });
        });

        /* 图例 */
        on(dom.byId("currentUserName"), "mouseover", function (e) {
            require(['widget/dijit/UserInfoThe'], function (clazz) {
                clazz.getInstance().startup();
            });
        });

        JZ.mc = new Map("mapDiv", {
            logo: false,
            fadeOnZoom: true,
            navigationMode: "css-transforms",
            sliderStyle: "large"
        });

        var baseMapLayer = null,
            waiter = null,
            featureLayer = null,
            legendDijit = null,
            mapServiceLayer = null,
            chartLayer = null,
            graphics = [],
            pieCharts = [],
            mapPanStart = null,
            mapPanEnd = null,
            mapZoomStart = null,
            mapZoomEnd = null,
            featureMouseOver = null,
            featureMouseOut = null;

        topic.subscribe("topic/basemap-complete", initApp);
        BaseMap.getBaseLayer();

        function initApp(data) {
            addBaseLayer(data);

            initMapDijits();
            /* 获取用户权限下的专题图 */
            var defer = xhr.get("../webservice/WebServiceMap.asmx/GetMenuSpecDetailLayer", {
                handleAs: "json",
                timeout: 5000,
                query: { classid: "" }
            });

            defer.then(function (suc) {
                /* 构造树形 */
                createThematicTree(suc);
            }, function (err) {
                /* 提示用户出错或者当前用户未登录直接跳转到登录界面 */
                console.log("调用获取用户菜单权限的WebService出现错误，请检查！" + err);
            });
        }

        function addBaseLayer(baseLayer) {
            /* 向地图中添加底图 */
            var sr = new SpatialReference(baseLayer.COORSYS);
            JZ.ie = new Extent(parseFloat(baseLayer.XMIN), parseFloat(baseLayer.YMIN), parseFloat(baseLayer.XMAX), parseFloat(baseLayer.YMAX), sr);
            baseMapLayer = new VecLayer(baseLayer.YEAR);
            //baseMapLayer = new ArcGISTiledMapServiceLayer(baseLayer.SERVICEURL, {
            //    opacity: 1
            //});
            JZ.mc.addLayer(baseMapLayer);
            JZ.mc.setExtent(JZ.ie);
        }

        function initMapDijits() {
            /* 绘制导航条 */
            var nav = new Nav({
                map: JZ.mc,
                initExtent: JZ.ie,
                domNode: dom.byId('cNavigation')
            });

            var scaleBar = new Scalebar({
                map: JZ.mc,
                attachTo: "bottom-left"
            });
            scaleBar.show();

            /* 导航工具栏 */
            JZ.en = new EsriNavigation(JZ.mc);
        }

        function createThematicTree(suc) {
            var data_1 = {
                identifier: 'id',
                label: 'cnName',
                items: suc
            };
            var store_1 = new ItemFileWriteStore({
                data: data_1
            });

            var model_1 = new ForestStoreModel({
                store: store_1,
                query: {
                    type: 'root'
                }
            });

            var tree = new Tree({
                model: model_1,
                persist: true,
                showRoot: false,
                style: { color: "#FFFFFF" },
                openOnClick: true,
                getIconClass: getIconCls,
                getLabelClass: getLabelCls,
                getLabel: getLabelResult,
                getTooltip: getTooltipResult
            });
            tree.placeAt(dom.byId("layerTree"));
            tree.startup();

            //加载事件
            on(tree, "click", toggleThematicMap);
        }

        /* 设置隐藏底图 */
        function setBaseMapOpacity(value) {
            if (baseMapLayer.opacity !== value) {
                baseMapLayer.setOpacity(value);
            }
        }

        var layerInfo = null;

        on(JZ.mc, "layers-add-result", function (evt) {
            //隐藏底图的显示
            setBaseMapOpacity(0);
            
            layerInfo = arrayUtils.map(evt.layers, function (layer, index) {
                return { layer: layer.layer, title: "当前地图图例" };
            });
        });

        var i = 0, thematicLayer = null, chartResults = [], chartType = -1;
        function toggleThematicMap(item, node, evt) {
            chartResults = [];
            chartType = -1;
            removeMapEvent();
            if (item && item["specshowmode"]) {
                if (Object.prototype.toString.call(item["specshowmode"]) === "[object Array]") {
                    //0:直接显示1：柱状图2：饼图3：聚类图
                    switch (item["specshowmode"][0]) {
                        case "0":
                            directAddLayer(item["mapServerUrl"][0], item["featureServerUrl"][0], item["featureServerIndex"][0]);
                            break;
                        case "1":
                            chartType = 1;
                            addColumnChartLayer(item);
                            break;
                        case "2":
                            chartType = 2;
                            addPieChartLayer(item);
                            break;
                        case "3":
                            addClusterLayer(item);
                            break;
                        default:
                            removeThematicLayer();
                            setBaseMapOpacity(1);
                            legendDijit.refresh(null);
                            break;
                    }
                }
            }
        }

        function removeThematicLayer() {
            if(mapServiceLayer){
                JZ.mc.removeLayer(mapServiceLayer);
                mapServiceLayer = null;
            }
            if (featureLayer) {
                JZ.mc.removeLayer(featureLayer);
                featureLayer = null;
            }
            if (chartLayer) {
                JZ.mc.removeLayer(chartLayer);
                chartLayer = null;
            }
            if (pieCharts && pieCharts.length > 0) {
                clearChart();
            }
            if (graphics.length > 0) {
                graphics = [];
            }
        }

        function directAddLayer(mapServerUrl, featureServiceUrl, index) {
            removeThematicLayer();
            mapServiceLayer = new ArcGISDynamicMapServiceLayer(mapServerUrl);
            featureLayer = new FeatureLayer(featureServiceUrl + "/" + index);
            JZ.mc.addLayers([mapServiceLayer, featureLayer]);
        }

        function getGraphics(item, flag) {
            removeThematicLayer();
            addMapEvent();
            mapServiceLayer = new ArcGISDynamicMapServiceLayer(item["mapServerUrl"][0]);
            JZ.mc.addLayers([mapServiceLayer]);

            chartLayer = new GraphicsLayer();
            JZ.mc.addLayer(chartLayer);


            /* 获取用户权限下的专题图 */
            var defer = xhr.get("../webservice/WebServiceMap.asmx/GetSpecData", {
                handleAs: "json",
                timeout: 5000,
                query: { layerid: item["layerid"][0], statyear: '' }
            });

            defer.then(function (suc) {
                getFeatures(item["featureServerUrl"][0] + "/" + item["featureServerIndex"][0]);
                chartResults = suc;
                on(chartLayer, "graphic-node-add", function (evt) {
                    var id = evt.graphic.attributes["OBJECTID"];
                    graphics.push({ ID: id, GRA: evt.graphic });
                    if (flag === 0) {
                        drawPieChart(evt.graphic, chartResults);
                    } else if (flag === 1) {
                        drawColumnChart(evt.graphic, chartResults);
                    }
                });

                on(chartLayer, "graphic-node-remove", function (evt) {
                    var id = evt.graphic.attributes["OBJECTID"];
                    var count = graphics.length;
                    for (var i = 0; i < count; i++) {
                        if (graphics[i].ID === id) {
                            graphics.splice(i, 1);
                        }
                    }
                });
            }, function (err) {
                /* 提示用户出错或者当前用户未登录直接跳转到登录界面 */
                console.log("调用获取用户菜单权限的WebService出现错误，请检查！" + err);
            });
        }

        function addColumnChartLayer(item) {
            getGraphics(item, 1);
        }

        function addMapEvent() {
            mapPanStart = on(JZ.mc, "pan-start", function () {
                clearChart();
            });
            mapZoomStart = on(JZ.mc, "zoom-start", function () {
                clearChart();
            });
            mapPanEnd = on(JZ.mc, "pan-end", function () {
                if (graphics.length) {
                    reDraw();
                }
            });
            mapZoomEnd = on(JZ.mc, "zoom-end", function () {
                waiter = setTimeout(reDraw, 200);
            });
        }

        function removeMapEvent() {
            if (mapPanStart) {
                mapPanStart.remove();
            }
            if (mapZoomStart) {
                mapZoomStart.remove();
            }
            if (mapPanEnd) {
                mapPanEnd.remove();
            }
            if (mapZoomEnd) {
                mapZoomEnd.remove();
            }
            if (featureMouseOver) {
                featureMouseOver.remove();
            }
            if (featureMouseOut) {
                featureMouseOut.remove();
            }
        }

        function addPieChartLayer(item) {
            getGraphics(item, 0);
        }


        function addClusterLayer(item) {
            removeThematicLayer();

            mapServiceLayer = new ArcGISDynamicMapServiceLayer(item["mapServerUrl"][0]);
            var url = item["featureServerUrl"][0] + "/" + item["featureServerIndex"][0];

            var defaultSym = new SimpleMarkerSymbol("circle", 10,
                        new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([102, 0, 0, 0.55]), 1),
                        new Color([255, 255, 255, 1]));
            featureLayer = new ClusterFeatureLayer({
                "url": url,
                "distance": 100,
                "id": "clusters",
                "labelColor": "#fff",
                "resolution": JZ.mc.extent.getWidth() / JZ.mc.width,
                //"singleColor": "#888",
                "singleSymbol": defaultSym,
                "singleTemplate": null,
                "useDefaultSymbol": false,
                "zoomOnClick": true,
                "showSingles": true,
                "objectIdField": "OBJECTID",
                outFields: ["*"]
            });

            var renderer = new ClassBreaksRenderer(defaultSym, "clusterCount");

            var sms1 = new SimpleMarkerSymbol("circle", 20,
                        new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 45, 45, 0.5]), 15),
                        new Color([47, 0, 0, 0.75]));

            var sms2 = new SimpleMarkerSymbol("circle", 30,
                        new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 0, 0, 0.5]), 15),
                        new Color([77, 0, 0, 0.75]));

            var sms3 = new SimpleMarkerSymbol("circle", 40,
                        new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([210, 0, 0, 0.5]), 15),
                        new Color([96, 0, 0, 0.75]));

            var sms4 = new SimpleMarkerSymbol("circle", 50,
                        new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([170, 0, 0, 0.5]), 15),
                        new Color([140, 0, 0, 0.75]));

            var sms5 = new SimpleMarkerSymbol("circle", 60,
                        new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([140, 0, 0, 0.5]), 15),
                        new Color([170, 0, 0, 0.75]));
            renderer.addBreak(2, 100, sms1);
            renderer.addBreak(100, 300, sms2);
            renderer.addBreak(300, 600, sms3);
            renderer.addBreak(600, 1100, sms4);
            renderer.addBreak(1100, 50000, sms5);
            featureLayer.setRenderer(renderer);

            JZ.mc.addLayers([mapServiceLayer, featureLayer]);
            addClusterLayerEvents();
        }


        function addClusterLayerEvents() {
            featureMouseOver = on(featureLayer, "mouse-over", onMouseOverCluster);
            featureMouseOut = on(featureLayer, "mouse-out", onMouseOutCluster);
        };
        
        var activeClusterElement;

        function onMouseOverCluster(e) {
            if (e.graphic.attributes.clusterCount === 1) {
                e.graphic._graphicsLayer.onClick(e);
            } else {
                if (e.target.nodeName === "circle") {
                    activeClusterElement = e.target;
                    setActiveClusterOpacity(activeClusterElement, 1, 1);
                } else {
                    setActiveClusterOpacity(activeClusterElement, 1, 1);
                }
            }
        }

        function onMouseOutCluster(e) {
            if (e.graphic.attributes.clusterCount > 1) {
                if (e.target.nodeName === "circle" || e.target.nodeName === "text") {
                    setActiveClusterOpacity(activeClusterElement, 0.75, 0.5);
                    setActiveClusterOpacity(e.target, 0.75, 0.5);
                }
            }
        }

        function setActiveClusterOpacity(elem, fillOpacity, strokeOpacity) {
            var textElm;
            if (elem) {
                elem.setAttribute("fill-opacity", fillOpacity);
                elem.setAttribute("stroke-opacity", strokeOpacity);
                textElm = elem.nextElementSibling;
                if (textElm && textElm.nodeName === "text") {
                    textElm.setAttribute("fill-opacity", 1);
                }
            }
        }


        function getFeatures(url) {
            /* 添加饼图 */
            var queryTask = new QueryTask(url);
            var query = new Query();
            query.outSpatialReference = JZ.mc.spatialReference;
            query.returnGeometry = true;
            query.outFields = ["*"];
            query.where = "1=1";
            queryTask.execute(query).then(addGraphics, onError);
        }

        function addGraphics(results) {
            var features = results.features;
            var sls = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 0, 0, 0]), 2);
            var sfs = new SimpleFillSymbol(SimpleFillSymbol.STYLE_SOLID, sls, new Color([255, 0, 0, 0]));
            arrayUtils.forEach(features, function (feature) {
                var g = new Graphic(feature.geometry, sfs, feature.attributes);
                chartLayer.add(g);
            });
        }

        function onError(e) {
            console.log(e);
        }

        function drawColumnChart(gra, suc) {
            var svgDojoShape = gra.getDojoShape();
            /* 添加饼图 */
            var svgl = svgDojoShape.bbox.l;
            var svgt = svgDojoShape.bbox.t;
            var svgr = svgDojoShape.bbox.r;
            var svgb = svgDojoShape.bbox.b;

            var svgWidth = svgr - svgl;
            var svgHeight = svgb - svgt;
            var value = 0;
            var dw = 0;
            var dh = 0;
            if (svgWidth >= svgHeight) {
                value = svgHeight;
            } else {
                value = svgWidth;
            }
            if (value > 92) {
                value = 92;
            }

            var svgTransform = svgDojoShape.parent.matrix;
            var pieX = (svgWidth - value) / 2 + svgl + svgTransform.dx;
            var pieY = (svgHeight - value) / 2 + svgt + svgTransform.dy;
            var pieDiv = _setDiv(value, pieX, pieY);
            if (value / 2 > 40) {
                value = 40;
            } else {
                value = value / 2 - 1;
            }
            //绘制柱状图
            _drawColumnChart(pieDiv, value, suc, gra);
        }

        function _drawColumnChart(node, r, suc, gra) {
            var myChart = new Chart(node);
            CubanShirts.chart.fill = "transparent";
            CubanShirts.chart.stroke = "transparent";
            CubanShirts.plotarea.fill = "transparent";
            CubanShirts.plotarea.stroke = null;
            CubanShirts.axis.stroke = null;
            myChart.setTheme(CubanShirts);
            myChart.addPlot("default", {
                type: Columns
            });

            var series = [];

            arrayUtils.forEach(suc, function (s) {
                if (s.name === gra.attributes["NAME"]) {
                    series = s.children;
                }
            });
            var id = gra.attributes["OBJECTID"];
            myChart.addSeries(id, series);
            new MoveSlice(myChart, "default");
            new Highlight(myChart, "default");
            new Tooltip(myChart, "default");
            myChart.render();

            pieCharts.push({ ID: id, CHART: myChart });
        }

        function drawPieChart(gra, suc) {
            var svgDojoShape = gra.getDojoShape();
            /* 添加饼图 */
            var svgl = svgDojoShape.bbox.l;
            var svgt = svgDojoShape.bbox.t;
            var svgr = svgDojoShape.bbox.r;
            var svgb = svgDojoShape.bbox.b;

            var svgWidth = svgr - svgl;
            var svgHeight = svgb - svgt;
            var value = 0;
            var dw = 0;
            var dh = 0;
            if (svgWidth >= svgHeight) {
                value = svgHeight;
            } else {
                value = svgWidth;
            }
            if (value > 82) {
                value = 82;
            }

            var svgTransform = svgDojoShape.parent.matrix;
            var pieX = (svgWidth - value) / 2 + svgl + svgTransform.dx;
            var pieY = (svgHeight - value) / 2 + svgt + svgTransform.dy;
            var pieDiv = _setDiv(value, pieX, pieY);
            if (value / 2 > 40) {
                value = 40;
            } else {
                value = value / 2 - 1;
            }
            _drawPieChart(pieDiv, value, suc, gra);
        }

        function _setDiv(value, pieX, pieY) {
            var pieDiv = baseWin.doc.createElement("div");
            domStyle.set(pieDiv, {
                "left": pieX + "px",
                "top": pieY + "px",
                "position": "absolute",
                "width": value + "px",
                "height": value + "px",
                "margin": "0",
                "padding": "0",
                "z-index": 2
            });
            dom.byId("chart").appendChild(pieDiv);
            return pieDiv;
        }

        function _drawPieChart(node, r, suc, gra) {
            var myChart = new Chart(node);
            Blue.chart.fill = "transparent";
            Blue.chart.stroke = "transparent";
            Blue.plotarea.fill = "transparent";
            Blue.plotarea.stroke = null;
            Blue.axis.stroke = null;
            myChart.setTheme(Blue);
            myChart.addPlot("default", {
                type: Pie,
                fontColor: '#FFFFFF',
                radius: r - 8,
                labels: false
            });

            var series = [];

            arrayUtils.forEach(suc, function (s) {
                if (s.name === gra.attributes["NAME"]) {
                    series = s.children;
                }
            });
            var id = gra.attributes["OBJECTID"];
            myChart.addSeries(id, series, { stroke: {width: 1}});
            new MoveSlice(myChart, "default");
            new Highlight(myChart, "default");
            new Tooltip(myChart, "default");
            myChart.render();

            pieCharts.push({ ID: id, CHART: myChart });
        }

        function reDraw() {
            if (waiter) {
                clearTimeout(waiter);
                waiter = null;
            }
            arrayUtils.forEach(graphics, function (g) {
                if (chartType === 1) {
                    drawColumnChart(g.GRA, chartResults);
                } else if (chartType === 2) {
                    drawPieChart(g.GRA, chartResults);
                }
            });
        }

        function clearChart() {
            if (graphics.length) {
                arrayUtils.forEach(pieCharts, function (chart) {
                    chart.CHART.destroy();
                });
                pieCharts = [];
                domConstruct.empty(dom.byId("chart"));
            }
        }

        function getTooltipResult(item) {
            if (!item.root && item["cnName"]) {
                return item["cnName"][0];
            }
        }

        function getLabelResult(item) {
            if (!item.root) {
                if (item && item["children"]) {
                    return item["cnName"][0] + "（" + item["children"].length + "个）";
                } else {
                    return item["cnName"][0];
                }
            }
        }

        function getLabelCls(item) {
            if (!item.root) {
                if (item && item["children"]) {
                    return "coll-label";
                } else {
                    return "layer-label";
                }
            }
        }

        function getIconCls(item, opened) {
            if (!item.root) {
                if (item && item["children"]) {
                    return opened ? "coll-clsed" : "coll-cls";
                } else {
                    if (item["shptype"]) {
                        switch (item["shptype"][0]) {
                            case "2":
                                return "point-clsed";
                            case "1":
                                return "line-clsed";
                            case "0":
                                return "polygon-clsed";
                        }
                    }
                }
            }
        }
    });