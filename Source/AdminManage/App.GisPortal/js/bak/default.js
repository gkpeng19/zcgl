/**
* @author wangyafei
*/
require([
    "dojo/_base/declare",
    "dojo/_base/event",
    "dojo/_base/fx",
    "dojo/_base/array",
    "dojo/window",
    "dojo/request/xhr",
    "dojo/dom",
    "dojo/dom-attr",
    "dojo/dom-class",
    "dojo/dom-construct",
    "dojo/fx/easing",
    "dojo/on",
    "dojo/parser",
    "dojo/query",
    "dojo/topic",
    "dojo/dnd/Moveable",
    "dojo/dnd/Mover",
    "dijit/Menu",
    "dijit/MenuItem",
    "dijit/popup",
    "dijit/TooltipDialog",
    "dijit/registry",
    "esri/map",
    "esri/graphic",
    "esri/SpatialReference",
    "esri/units",
    "esri/tasks/GeometryService",
    "esri/tasks/ProjectParameters",
    "esri/layers/ArcGISTiledMapServiceLayer",
    "esri/dijit/Popup",
    "esri/dijit/Scalebar",
    "esri/dijit/OverviewMap",
    "esri/dijit/Geocoder",
    "esri/dijit/Measurement",
    "esri/symbols/PictureMarkerSymbol",
    "esri/toolbars/navigation",
    "esri/geometry/Extent",
    "esri/geometry/Point",
    "widget/navigation/Nav", 
    "widget/layer/BaseMapAndLayer",
    "widget/left/LeftControl",
    "dojo/domReady!"
], function (declare, Event, baseFx, arrayUtil, win, xhr, dom, domAttr, domClass, domConstruct, easing, on, parser,
    query, topic, Moveable, Mover, Menu, MenuItem, PopupManager, TooltipDialog, registry,
    Map, Graphic, SpatialReference, Units, GeometryService, 
    ProjectParameters, ArcGISTiledMapServiceLayer,
    Popup, Scalebar, OverviewMap, Geocoder, Measurement, PictureMarkerSymbol, 
    EsriNavigation, Extent, Point,
    Nav, BaseMapAndLayer, LeftControl) {
    "use strict";
    /* 解析页面中的dojo控件 */
    parser.parse();

    var StepMover = declare([Mover], {
        onMouseMove: function (e) {
            var m = this.marginBox;
            if (e.ctrlKey) {
                this.host.onMove(this, { l: parseInt((m.l + e.pageX) / 5) * 5, t: parseInt((m.t + e.pageY) / 5) * 5 });
            } else {
                this.host.onMove(this, { l: m.l + e.pageX, t: m.t + e.pageY });
            }
            Event.stop(e);
        }
    });

    /* 自定义InfoWindow的样式及定义InfoWindow可移动 */
    var customWindow = new Popup({
        fillSymbol: false,
        highlight: false,
        lineSymbol: false,
        markerSymbol: false
    }, domConstruct.create("div"));
    domClass.add(customWindow.domNode, "greyWindow");

    var moveNode = new Moveable(customWindow.domNode, {
        mover: StepMover
    });

    /* 获取项目的配置文件 */
    var defer = xhr.get("config/config.json", {
        handleAs: "json",
        timeout: 10000
    });

    /* 请求成功事件 */
    defer.then(function (suc) {
        JZ.config = suc;
        /* 获取区域列表 */
        require(["widget/district/DistrictExtent"], function (DistrictExtent) {
            var district = new DistrictExtent();
        });

        var overviewMap = null;
        /* 几何地图服务 */
        var geometryUrl = JZ.config["GeometryService"][0].url;
        esri.config.defaults.io.proxyUrl = "proxy/Handler.ashx";
        JZ.gs = new GeometryService(geometryUrl);
        esri.config.defaults.geometryService = JZ.gs;

        /* 创建地图控件 */
        JZ.mc = new Map("mapDiv", {
            logo: false,
            navigationMode: "css-transforms",
            fadeOnZoom: true,
            sliderStyle: "large",
            sliderPosition: "top-right",
            infoWindow: customWindow
        });

        /* 从数据库中读取底图并添加到地图控件上 */
        BaseMapAndLayer.setBaseMap();

        /* 绘制导航条 */
        var nav = new Nav({
            map: JZ.mc,
            initExtent: JZ.ie,
            domNode: dom.byId('nav')
        });

        on(JZ.mc, "load", function (e) {
            //添加比例尺
            var scaleBar = new Scalebar({
                attachTo: "bottom-left",
                map: JZ.mc,
                scalebarUnit: "metric"
            });
            scaleBar.show();

            /* 添加鹰眼图 */
            overviewMap = new OverviewMap({
                attachTo: "bottom-right",
                map: JZ.mc,
                color: "#0000FF",
                width: 150,
                height: 150,
                visible: false
            });
            overviewMap.startup();
            on(JZ.mc, "click", show3D);
        });

        function addStreetLayer() {
            var baseMapLayer = new ArcGISTiledMapServiceLayer(JZ.bms[0].SERVICEURL);
            JZ.smc.addLayer(baseMapLayer, 0);
        }

        function createStreetMap(px, py) {
            var sMapDiv = document.frames["streetMapFrame"].document.getElementById("jz-street-mapDiv");

            JZ.smc = new Map(sMapDiv, {
                logo: false,
                navigationMode: "css-transforms",
                fadeOnZoom: true,
                sliderStyle: "small",
                sliderPosition: "top-right",
                extent: JZ.ie
            });
            addStreetLayer();

            var overTimer = null, outTimer = null;
            on(JZ.smc, "load", function (e) {
                topic.publish("topic/StreetViewMoveComplete", px, py);
            });

            on(document.frames["streetMapFrame"].document.getElementById("jz-return-map"), "click", function (e) {
                closeStreetMap();
            });
            on(document.frames["returnToMapFrame"].document.getElementById("jz-return-map"), "click", function (e) {
                closeStreetMap();
            });
        }

        function closeStreetMap() {
            var viewport = win.getBox(win.doc);
            baseFx.animateProperty({
                easing: easing.linear,
                duration: 1000,
                node: dom.byId("jz-street"),
                properties: {
                    top: { start: 0, end: -2000 - dom.byId("jz-street").offsetHeight },
                    bottom: { start: 0, end: 2000 }
                }
            }).play();
        }

        function show3D(evt) {
            if (JZ.mc.getZoom() >= 9) {
                var projectParams = new ProjectParameters();
                projectParams.geometries = [evt.mapPoint];
                projectParams.outSR = new SpatialReference({
                    wkid: 4326
                });

                JZ.gs.project(projectParams, function (suc) {
                    var d = dom.byId("jz-street-object");
                    d.setAttribute("px", suc[0].x);
                    d.setAttribute("py", suc[0].y);
                    baseFx.animateProperty({
                        easing: easing.linear,
                        duration: 1000,
                        node: dom.byId("jz-street"),
                        properties: {
                            top: { start: -4000, end: 0 },
                            bottom: { start: 2000, end: 0 }
                        },
                        onEnd: function () {
                            if (!JZ.smc) {
                                createStreetMap(suc[0].x, suc[0].y);
                                var htmlStr = '<object id="PPVision" width="100%" height="100%" classid="CLSID:C2DF135F-24D6-49F7-820A-6CD1B264A60E"><table><tr><td><a href="/ppv/PPVision.msi">提示信息：请先下载并安装插件后，关闭浏览器后重新打开。</a></td></tr><tr><td><img src="/ppv/ppvset.png"/></td></tr></table></object>';
                                domConstruct.place(htmlStr, d, "first");
                            } else {
                                allViewLocate(suc[0].x, suc[0].y);
                            }
                        }
                    }).play();
                }, function (err) {
                    console.log(err);
                });
            }
        }

        function addOverviewMap() {
            overviewMap = new OverviewMap({
                attachTo: "bottom-right",
                map: JZ.mc,
                color: "#0000FF",
                width: 150,
                height: 150,
                visible: false
            });
            overviewMap.startup();
        }

        /* 地图右键菜单 */
        var esriNav = new EsriNavigation(JZ.mc);
        JZ.en = esriNav;
        var menu = new Menu({
            targetNodeIds: ["mapDiv"],
            style: {
                display: "none",
                width: "130px"
            }
        });

        menu.addChild(new MenuItem({
            label: "放大",
            iconClass: "dijitIcon dijitZoomInIcon",
            onClick: function (evt) {
                JZ.mc.setMapCursor("crosshair");
                esriNav.activate(EsriNavigation.ZOOM_IN);
            }
        }));

        menu.addChild(new MenuItem({
            label: "缩小",
            iconClass: "dijitIcon dijitZoomOutIcon",
            onClick: function (evt) {
                JZ.mc.setMapCursor("crosshair");
                esriNav.activate(EsriNavigation.ZOOM_OUT);
            }
        }));

        menu.addChild(new MenuItem({
            label: "平移",
            iconClass: "dijitIcon dijitPanIcon",
            onClick: function (evt) {
                JZ.mc.setMapCursor("default");
                esriNav.activate(EsriNavigation.PAN);
            }
        }));

        var prevMenuItem = new MenuItem({
            label: "上一视图",
            iconClass: "dijitIcon dijitPrevIcon",
            onClick: function (evt) {
                esriNav.zoomToPrevExtent();
            }
        })
        menu.addChild(prevMenuItem);

        var nextMenuItem = new MenuItem({
            label: "下一视图",
            iconClass: "dijitIcon dijitNextIcon",
            onClick: function (evt) {
                esriNav.zoomToNextExtent();
            }
        })
        menu.addChild(nextMenuItem);

        menu.addChild(new MenuItem({
            label: "初始范围",
            iconClass: "dijitIcon dijitFullIcon",
            onClick: function (evt) {
                JZ.mc.setExtent(JZ.ie);
            }
        }));

        menu.addChild(new MenuItem({
            label: "清除痕迹",
            iconClass: "dijitIcon dijitClearAllIcon",
            onClick: function (evt) {
                JZ.mc.graphics.clear();
                JZ.mc.infoWindow.hide();
            }
        }));

        /* 测量工具 */
        var measurement = new Measurement({
            defaultAreaUnit: Units.SQUARE_MILES,
            defaultLengthUnit: Units.KILOMETERS,
            map: JZ.mc
        }, dom.byId("measurementDiv"));

        measurement.startup();

        /* 地址库全文搜索 */
        var geocoders = [{
            url: JZ.config["GeocoderServer"][0].url,
            name: "北京市地址编码服务",
            SingleLine: "SingleLine"
        }];

        var geocoder = new Geocoder({
            map: JZ.mc,
            geocoders: geocoders,
            arcgisGeocoder: false,
            autoNavigate: true,
            autoComplete: true,
            maxLocations:5,
            minCharacters: 3,
            showResults: true
        }, "mapGeocoder");

        geocoder.startup();

        var tipDialog = new TooltipDialog({
            id: 'myHotwordDialog',
            style: 'width:227px;',
            content: ""
        });

        on(tipDialog, "click", function (e) {
            if (e.target.tagName === "SPAN") {
                var node = e.target;
                var value = domAttr.get(node, "data-dojo-value");
                geocoder.set("value", value);
                geocoder.find();
                PopupManager.close(tipDialog);
            }
        });

        on(geocoder, "mouse-over", function (e) {
            if (!geocoderResult.length) {
                var defer = xhr.get("webservice/WebServiceMap.asmx/GetHotWords", {
                    handleAs: "json",
                    timeout: 10000
                });

                var htmlStr = "<div><div>最近搜索热词:</div>";
                /* 请求成功事件 */
                defer.then(function (suc) {
                    setTimeout(function () {
                        if (suc.length) {
                            arrayUtil.forEach(suc, function (s) {
                                htmlStr += "<span style='color:#FF0000' data-dojo-value='" + s.QWORD + "'>" + s.QWORD + "</span></br>";
                            });
                            htmlStr += "</div>";
                            tipDialog.set("content", htmlStr);
                            PopupManager.open({
                                popup: tipDialog,
                                around: dom.byId('mapGeocoder')
                            });
                        }
                    }, 300);
                });
            }
        });

        on(geocoder, "select", function (results) {
            var defer = xhr.post("webservice/WebServiceMap.asmx/SetHotWords", {
                handleAs: "json",
                timeout: 10000,
                query: {qword:results.result.name, qsuccess:1}
            });
        });

        on(geocoder, "find-results", function (suc) {
            if (suc && suc.results && suc.results.results && suc.results.results.length > 0) {
                JZ.mc.setExtent(suc.results.results[0].extent);
            }
        });

        var geocoderResult = [];

        on(geocoder, "auto-complete", function (suc) {
            if (suc.results.results.length) {
                geocoderResult = suc.results.results;
                if (tipDialog) {
                    PopupManager.close(tipDialog);
                }
            }
        });

        on(document, "click", function () {
            if (tipDialog) {
                PopupManager.close(tipDialog);
            }
            geocoderResult = [];
        });

        on(dom.byId("collepse"), "click", function () {
            LeftControl.toggle();
        });

        query("#layerOperPanel .searchfuncs td").on("click", function (e) {
            var node = e.currentTarget;
            query("#layerOperPanel .searchfuncs td").removeClass("activate");
            domClass.add(node, "activate");
            var tab = registry.byId("tabStack");
            tab.selectChild(domAttr.get(node, "data-target"), true);
        });

        /* 计算当前鼠标位置处的经纬度坐标 */
        on(JZ.mc, "mouse-move", function (e) {
            convertWebMector(e);
        });

        function convertWebMector(e) {
            var projectParams = new ProjectParameters();
            projectParams.geometries = [e.mapPoint];
            projectParams.outSR = new SpatialReference({
                wkid: 4326
            });
            JZ.gs.project(projectParams, function (suc) {
                dom.byId("currentLocation").innerHTML = "经度:" + suc[0].x.toFixed(3) + ", 纬度" + suc[0].y.toFixed(3);
            }, function (err) {
                console.log(err);
            });
        }

        topic.subscribe("topic/StreetViewMoveComplete", function (px, py) {
            var projectParams = new ProjectParameters();
            var point = new Point(px, py);
            projectParams.geometries = [point];
            projectParams.outSR = JZ.mc.spatialReference;

            JZ.gs.project(projectParams, function (suc) {
                var center = new Point(suc[0].x, suc[0].y);
                JZ.smc.centerAt(center);
                JZ.smc.graphics.clear();
                var fillSymbol = new PictureMarkerSymbol('/images/allview.gif', 51, 51);
                var allviewGraphic = new Graphic(center, fillSymbol);
                JZ.smc.graphics.add(allviewGraphic);
            }, function (err) {
                console.log(err);
            });
        });
        domConstruct.destroy(dom.byId("loadingBar"));
    }, function (err) {
        console.log("获取配置文件出现错误，请重试！" + err);
    });
});
