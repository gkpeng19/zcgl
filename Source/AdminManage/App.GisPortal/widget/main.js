define(['dojo/_base/declare',
        'dojo/request/xhr',
        'dojo/query',
        'dojo/dom',
        'dojo/dom-attr',
        'dojo/dom-style',
        'dojo/dom-class',
        'dojo/dom-construct',
        'dojo/_base/fx',
        'dojo/fx',
        'dojo/on',
        'dojo/_base/html',
        'dojo/topic',
        'dojo/_base/array',
        'esri/dijit/Scalebar',
        'esri/dijit/OverviewMap',
        'esri/dijit/Popup',
        'dojo/_base/event',
        'dojo/fx/easing',
        'esri/map',
        'widget/layer/VecLayer',
        'esri/layers/ArcGISTiledMapServiceLayer',
        'esri/layers/FeatureLayer',
        'esri/Color',
        'esri/symbols/SimpleLineSymbol',
        'esri/renderers/SimpleRenderer',
        'esri/tasks/ProjectParameters',
        'esri/geometry/Point',
        'esri/symbols/PictureMarkerSymbol',
        'esri/graphic',
        'esri/SpatialReference',
        'widget/menu/MenuBar',
        'widget/district/DistrictExtent',
        'widget/layer/BaseMapAndLayer'],
    function (declare, xhr, query, dom, domAttr, domStyle, domClass, domConstruct, baseFx, fx,
        on, htmlCls, topic, arrayUtils,
        Scalebar, OverviewMap, Popup, Event, easing, Map, VecLayer,
        ArcGISTiledMapServiceLayer, FeatureLayer, Color, SimpleLineSymbol, SimpleRenderer,
        ProjectParameters, Point,
        PictureMarkerSymbol, Graphic, SpatialReference,
        MenuBar, DistrictExtent, BaseMapAndLayer) {
        'use strict';
        var mo = {};

        xhr.get("webservice/WebServiceMap.asmx/GetLoginUser", {
            handleAs: "json",
            timeout: 10000
        }).then(function (suc) {
            dom.byId("currentUserName").innerHTML = suc.User.TrueName;
        });

        on(dom.byId("currentUserName"), "mouseover", function () {
            require(['widget/dijit/UserInfo'], function (clazz) {
                clazz.getInstance().startup();
            });
        });

        on(dom.byId("editMap"), "click", function () {
            require(['widget/dijit/NewMarker'], function (clazz) {
                clazz.getInstance().startup();
            });
        });

        //初始化横向菜单
        var menus = {
            //id="constrast-analyst" style="cursor: pointer"
            dbfx: '<li><a href="/pages/ConstrastAnalyst.aspx" target="_blank">对比分析</a></li>',
            clgj: '<li><a id="measure-tool" style="cursor: pointer">测量工具</a></li>',
            dtsc: '<li><a id="map-print" style="cursor: pointer">地图输出</a></li>',
            ztfx: '<li><a href="/pages/Thematic.aspx" target="_blank">专题分析</a></li>',
            dtapi: '<li><a href="/pages/API.aspx" target="_blank">地图API</a></li>',
            dtysj: '<li><a href="/pages/MetaData.aspx" target="_blank">地图元数据</a></li>',
            fwzy: '<li><a href="/pages/ResourceService.aspx" target="_blank">服务资源</a></li>',
            ptgl: '<li><a href="http://10.246.0.81:8090/" target="_blank">平台管理</a></li>',
            jclb: '<li><a id="error-tool" style="cursor: pointer">纠错列表</a></li>'
        };

        xhr.get("webservice/WebServiceMap.asmx/GetUserMenu", {
            handleAs: "json",
            timeout: 10000,
            query: { ran: Math.random() }
        }).then(function (suc) {
            var html = "";
            for (var i in suc) {
                html += menus[suc[i].ENAME];
            }
            dom.byId("ul_menu").innerHTML = html;

            initMenuFunc();
        }, function (err) {
            console.log(err);
        });

        function initMenuFunc() {
            //对比分析
            var ele = dom.byId("constrast-analyst");
            if (ele) {
                on(ele, "click", function (e) {
                    require(['widget/dijit/ConstrastAnalyst'], function (clazz) {
                        clazz.getInstance().startup();
                    });
                });
            }

            //测量工具
            ele = dom.byId("measure-tool");
            if (ele) {
                on(ele, "click", function (e) {
                    require(['widget/dijit/MeasureDialog'], function (clazz) {
                        clazz.getInstance().startup();
                    });
                });
            }

            //地图输出
            ele = dom.byId("map-print");
            if (ele) {
                on(ele, "click", function (e) {
                    require(['widget/dijit/PrintDialog'], function (clazz) {
                        clazz.getInstance().startup();
                    });
                });
            }

            //错误列表
            ele = dom.byId("error-tool");
            if (ele) {
                on(ele, "click", function () {
                    require(['widget/dijit/ErrorList'], function (clazz) {
                        clazz.getInstance().startup();
                    });

                    console.log(JZ.layersList);
                });
            }
        }

        //数据查询
        on(dom.byId("btnSearch"), "click", function (e) {
            var text = dom.byId("searchText").value;
            if (text) {
                require(['widget/utils/SearchingUtil'], function (clazz) {
                    clazz.getInstance().search(text);
                });
            }
            //topic.publish("topic/RefreshSearchList", dom.byId("searchText").value);
        });

        //进入全屏
        //function requestFullScreen() {
        //    var de = document.documentElement;
        //    if (de.requestFullscreen) {
        //        de.requestFullscreen();
        //    } else if (de.mozRequestFullScreen) {
        //        de.mozRequestFullScreen();
        //    } else if (de.webkitRequestFullScreen) {
        //        de.webkitRequestFullScreen();
        //    } else if (de.msRequestFullscreen) {
        //        de.msRequestFullscreen();
        //    } else {
        //        var wsh = new ActiveXObject("WScript.Shell");
        //        wsh.sendKeys("{F11}");
        //    }
        //}
        /*
        document.addEventListener("fullscreenchange", function () {  

    fullscreenState.innerHTML = (document.fullscreen)? "" : "not ";}, false);  

document.addEventListener("mozfullscreenchange", function () {  

    fullscreenState.innerHTML = (document.mozFullScreen)? "" : "not ";}, false);  

document.addEventListener("webkitfullscreenchange", function () {  

    fullscreenState.innerHTML = (document.webkitIsFullScreen)? "" : "not ";}, false);

document.addEventListener("msfullscreenchange", function () {

    fullscreenState.innerHTML = (document.msFullscreenElement)? "" : "not ";}, false);
        */
        //退出全屏
        //function exitFullscreen() {
        //    var de = document;
        //    if (de.exitFullscreen) {
        //        de.exitFullscreen();
        //    } else if (de.mozCancelFullScreen) {
        //        de.mozCancelFullScreen();
        //    } else if (de.webkitCancelFullScreen) {
        //        de.webkitCancelFullScreen();
        //    }
        //}

        //var StepMover = declare([Mover], {
        //    onMouseMove: function (e) {
        //        var m = this.marginBox;
        //        if (e.ctrlKey) {
        //            this.host.onMove(this, { l: parseInt((m.l + e.pageX) / 5) * 5, t: parseInt((m.t + e.pageY) / 5) * 5 });
        //        } else {
        //            this.host.onMove(this, { l: m.l + e.pageX, t: m.t + e.pageY });
        //        }
        //        Event.stop(e);
        //    }
        //});

        /* 自定义InfoWindow的样式及定义InfoWindow可移动 */
        var customWindow = new Popup({
            fillSymbol: false,
            highlight: false,
            lineSymbol: false,
            markerSymbol: false
        }, domConstruct.create("div"));
        domClass.add(customWindow.domNode, "greyWindow");
        //var moveNode = new Moveable(customWindow.domNode, {
        //    mover: StepMover
        //});
        //启动应用程序的流程
        function initApp() {
            /* 获取项目的配置文件 */
            var defer = xhr.get("js/config.json", {
                handleAs: "json",
                timeout: 10000
            });
            /* 请求成功事件 */
            defer.then(function (suc) {
                JZ.config = suc;
                getDistrict();
            }, function (err) {
                showLogin();
            });
        }

        function getDistrict() {
            //获取区域列表完成后添加底图选择功能
            topic.subscribe("topic/GetDistrictComplete", addBaseMap);
            DistrictExtent.getInstance().show();
        }

        function addBaseMap() {
            //底图列表加载完毕后初始化地图
            topic.subscribe("topic/AddBaseMapComplete", getCategoryPriv);
            BaseMapAndLayer.getInstance().setBaseMap();
        }

        //获取用户所拥有的图层组权限
        function getCategoryPriv() {
            //添加用户权限菜单完成后，获取用户所属的区域权限
            topic.subscribe("topic/AddMenuComplete", initMap);
            MenuBar.getInstance().show(dom.byId("ld-category"));
        }

        function showMap() {
            htmlCls.setStyle('main-loading', 'display', 'none');
            htmlCls.setStyle('main-page', 'display', 'block');
        }

        function initMap() {
            require(['esri/tasks/GeometryService', 'esri/toolbars/navigation', 'widget/dijit/LoadingIndicator'],
                function (GeometryService, EsriNavigation, LoadingIndicator) {
                    //先显示地图
                    showMap();

                    //显示loading条
                    JZ.loading = new LoadingIndicator();
                    domConstruct.place(JZ.loading.domNode, document.body);

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
                    JZ.en = new EsriNavigation(JZ.mc);
                    on(JZ.mc, "load", addNavigation);
                    on(JZ.mc, "zoom-end", addStreetMap);
                    JZ.mapclick = on.pausable(JZ.mc, "click", showInfoWindow);
                    topic.subscribe("topic/MeasureDialogShow", function () {
                        if (JZ.mapclick) {
                            JZ.mapclick.pause();
                        }
                    });
                    //添加底图切换功能
                    topic.publish("topic/ToggleLayerTopic");
                    //添加类别切换功能
                    topic.publish("topic/CategoryToggle");
                    //MenuBar.getInstance().getSelectedNode();
                });
        }
        function showInfoWindow(evt) {
            require(['widget/utils/QueryUtils'], function (clazz) {
                clazz.getInstance().search(evt);
            });
        }
        function showLogin() {
            window.location.href = "Login.aspx?ReturnUrl=" + window.location.href;
        }
        function addNavigation() {
            require(['widget/dijit/ContextMenu', 'widget/navigation/Nav'], function (clazz, Nav) {
                clazz.getInstance().show();
                /* 绘制导航条 */
                var nav = new Nav({
                    map: JZ.mc,
                    initExtent: JZ.ie,
                    en: JZ.en,
                    domNode: dom.byId('cNavigation')
                });
                //添加鹰眼图
                //addOverviewMap();
                //添加比例尺
                addScaleBar();
                //添加坐标
                addCoordinate();
            });
        }
        function addCoordinate() {
            require(['widget/coordinate/Coordinate'], function (Coordinate) {
                var coord = new Coordinate({
                    map: JZ.mc
                }, "real-coordinate");
                coord.startup();
            });
        }

        function addScaleBar() {
            var scaleBar = new Scalebar({
                attachTo: "bottom-left",
                map: JZ.mc,
                scalebarUnit: "metric"
            });
            scaleBar.show();
        }

        function addOverviewMap() {
            var overviewMap = new OverviewMap({
                attachTo: "bottom-right",
                map: JZ.mc,
                color: "#0000FF",
                width: 150,
                height: 150,
                visible: false
            });
            overviewMap.startup();
        }


        //面板切换事件
        query(".navigation li a").on("click", function (e) {
            var node = e.currentTarget;
            var pane = domAttr.get(node, "data-toggle");
            query(".navigation li").removeClass("active");
            domClass.add(node.parentNode, "active");
            query(".tab-content .tab-pane").removeClass("show-pane");
            domClass.add(dom.byId(pane), "show-pane");
            //缓冲区分析
            if (pane === "BufferAnalyst") {
                require(['widget/buffer/BufferAnalyst'], function (clazz) {
                    clazz.getInstance().startup();
                });
            } else if (pane === "StatAnalyst") {
                require(['widget/stat/StatAnalyst'], function (clazz) {
                    clazz.getInstance().startup();
                });
            } else if (pane === "HistoryAnalyst") {
                require(['widget/history/HistoryAnalyst'], function (clazz) {
                    clazz.getInstance().startup();
                });
            }
        });
        /* 面板的关闭和打开事件 */
        var clickEvt = on.pausable(dom.byId("collpse-left"), "click", function (e) {
            clickEvt.pause();
            var left = dom.byId("leftNav");
            var right = dom.byId("rightNav");
            var leftMargin = domStyle.get(left, "left");
            if (leftMargin === 70) {
                fx.combine([
                    baseFx.animateProperty({
                        duration: 1000,
                        node: left,
                        properties: {
                            left: { start: 70, end: 70 - 315 }
                        }
                    }),
                    baseFx.animateProperty({
                        duration: 1000,
                        node: right,
                        properties: {
                            left: { start: 70 + 315, end: 70 }
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
                            left: { start: 70 - 315, end: 70 }
                        }
                    }),
                    baseFx.animateProperty({
                        duration: 1000,
                        node: right,
                        properties: {
                            left: { start: 70, end: 70 + 315 }
                        },
                        onEnd: function () {
                            JZ.mc.resize(true);
                            clickEvt.resume();
                        }
                    })
                ]).play();
            }
        });

        topic.subscribe('topic/StreetViewMoveComplete', function (px, py) {
            var projectParams = new ProjectParameters();
            var point = new Point(px, py);
            projectParams.geometries = [point];
            projectParams.outSR = JZ.smc.spatialReference;
            JZ.gs.project(projectParams, function (suc) {
                var center = new Point(suc[0].x, suc[0].y);
                JZ.smc.centerAt(center);
                JZ.smc.graphics.clear();
                var fillSymbol = new PictureMarkerSymbol('/images/street.png', 12, 17);
                var allviewGraphic = new Graphic(center, fillSymbol);
                JZ.smc.graphics.add(allviewGraphic);
            }, function (err) {
                console.log(err);
            });
        });

        //添加街景图层
        var streetLayer = null;
        function addStreetMap(evt) {
            if (evt.level >= 10) {
                if (!streetLayer) {
                    streetLayer = new FeatureLayer(JZ.config["PPStreetLayer"][0].url);
                    var symbol = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([150, 204, 248, 0.8]), 30);
                    var renderer = new SimpleRenderer(symbol);
                    streetLayer.setRenderer(renderer);
                    JZ.mc.addLayer(streetLayer);
                    on(streetLayer, "click", function (e) {
                        createStreetMap(e.mapPoint);
                    });
                } else {
                    streetLayer.show();
                }
            } else {
                if (streetLayer != null) {
                    streetLayer.hide();
                }
            }
        }

        function createStreetMap(poi) {
            baseFx.animateProperty({
                easing: easing.linear,
                duration: 1000,
                node: dom.byId('jz-street'),
                properties: {
                    top: { start: -4000, end: 0 },
                    bottom: { start: 2000, end: 0 }
                },
                onEnd: function () {
                    startStreetMap(poi);
                }
            }).play();
        }

        function createMap(poi, px, py) {
            var returnDoc = top.frames['returnToMapFrame'].document ? top.frames['returnToMapFrame'].document : top.frames['returnToMapFrame'].contentDocument;
            var mapDoc = top.frames['streetMapFrame'].document ? top.frames['streetMapFrame'].document : top.frames['streetMapFrame'].contentDocument;
            var sMapDiv = mapDoc.getElementById('jz-street-mapDiv');
            if (!JZ.smc) {
                JZ.smc = new Map(sMapDiv, {
                    logo: false,
                    navigationMode: 'css-transforms',
                    fadeOnZoom: true,
                    sliderStyle: 'small',
                    sliderPosition: 'top-right',
                    extent: JZ.ie
                });

                on(JZ.smc, 'load', function (e) {
                    JZ.smc.setLevel(9);
                    JZ.smc.centerAt(poi);
                    var fillSymbol = new PictureMarkerSymbol('/images/marker/red.png', 35, 42);
                    var allviewGraphic = new Graphic(poi, fillSymbol);
                    JZ.smc.graphics.add(allviewGraphic);
                    require(['esri/toolbars/edit'], function (Editor) {
                        //var editor = new Editor(JZ.smc);
                        //editor.activate(Editor.MOVE, allviewGraphic);
                    });
                    showStreet(px, py);
                });

                var baseMapLayer = new VecLayer(JZ.baselayers[0].YEAR);
                JZ.smc.addLayer(baseMapLayer);
                on(returnDoc.getElementById('jz-return-map'), 'click', function (e) {
                    closeStreetMap();
                });
            } else {
                JZ.smc.graphics.clear();
                JZ.smc.setLevel(9);
                JZ.smc.centerAt(poi);
                var fillSymbol = new PictureMarkerSymbol('/images/marker/red.png', 35, 42);
                var allviewGraphic = new Graphic(poi, fillSymbol);
                JZ.smc.graphics.add(allviewGraphic);
            }
        }

        function startStreetMap(poi) {
            var params = new ProjectParameters();
            params.geometries = [poi];
            params.outSR = new SpatialReference({
                wkid: 4326
            });
            JZ.gs.project(params, function (suc) {
                createMap(poi, suc[0].x, suc[0].y);
            }, function (err) {
                alert("发生错误，请重试！");
            });
        }

        function showStreet(px, py) {
            var d = dom.byId('jz-street-object');
            d.setAttribute('px', px);
            d.setAttribute('py', py);
            var htmlStr = "<object id='PPVision' width='100%' height='100%' classid='CLSID:C2DF135F-24D6-49F7-820A-6CD1B264A60E'><table><tr><td><a href='/ppv/PPVision.msi'>提示信息：请先下载并安装插件后，关闭浏览器后重新打开。</a></td></tr><tr><td><img src='/ppv/ppvset.png'/></td></tr></table></object>";
            domConstruct.place(htmlStr, d, 'first');
        }

        function closeStreetMap() {
            baseFx.animateProperty({
                easing: easing.linear,
                duration: 1000,
                node: dom.byId('jz-street'),
                properties: {
                    top: { start: 0, end: -2000 - dom.byId('jz-street').offsetHeight },
                    bottom: { start: 0, end: 2000 }
                }
            }).play();
        }
        mo.initApp = initApp;
        return mo;
    });