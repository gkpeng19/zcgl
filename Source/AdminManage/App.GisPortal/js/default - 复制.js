/**
* @author wangyafei
*/
require([
    "dojo/_base/declare",
    "dojo/_base/event",
    "dojo/_base/fx",
    "dojo/_base/array",
    "dojo/_base/window",
    "dojo/request/xhr",
    "dojo/dom",
    "dojo/dom-attr",
    "dojo/dom-class",
    "dojo/dom-construct",
    "dojo/dom-style",
    "dojo/keys",
    "dojo/fx",
    "dojo/on",
    "dojo/parser",
    "dojo/mouse",
    "dojo/query",
    "dojo/topic",
    "dojo/dnd/Moveable",
    "dojo/dnd/Mover",
    "dojo/store/Memory",
    "dojo/data/ItemFileWriteStore",
    "dojo/request/notify",
    "dojo/request/script",
    "dijit/Menu",
    "dijit/MenuItem",
    "dijit/Tree",
    "dijit/tree/ForestStoreModel",
    "dijit/tree/ObjectStoreModel",
    "dijit/registry",
    "dojox/form/FileUploader",
    "dojox/charting/Chart",
    "dojox/charting/plot2d/Pie",
    "dojox/charting/action2d/Highlight",
    "dojox/charting/action2d/MoveSlice",
    "dojox/charting/action2d/Tooltip",
    "dojox/charting/themes/MiamiNice",
    "dojox/charting/themes/ThreeD",
    "dojox/charting/widget/Legend",
    "esri/map",
    "esri/Color",
    "esri/graphic",
    "esri/SpatialReference",
    "esri/units",
    "esri/urlUtils",
    "esri/dijit/editing/Editor",
    "esri/dijit/editing/TemplatePicker",
    "esri/dijit/editing/AttachmentEditor",
    "esri/tasks/GeometryService",
    "esri/tasks/QueryTask",
    "esri/tasks/query",
    "esri/tasks/Geoprocessor",
    "esri/tasks/IdentifyTask",
    "esri/tasks/IdentifyParameters",
    "esri/tasks/ProjectParameters",
    "esri/tasks/BufferParameters",
    "esri/layers/ArcGISTiledMapServiceLayer",
    "esri/layers/ArcGISDynamicMapServiceLayer",
    "esri/layers/FeatureLayer",
    "esri/layers/DynamicLayerInfo",
    'esri/layers/WMSLayer1',
    'esri/layers/WMSLayerInfo',
    "esri/dijit/Popup",
    "esri/dijit/Scalebar",
    "esri/dijit/OverviewMap",
    "esri/dijit/Geocoder",
    "esri/dijit/Measurement",
    "esri/symbols/PictureMarkerSymbol",
    "esri/symbols/SimpleMarkerSymbol",
    "esri/symbols/SimpleLineSymbol",
    "esri/symbols/SimpleFillSymbol", 
    "esri/symbols/PictureFillSymbol", 
    "esri/renderers/ClassBreaksRenderer",
    "esri/toolbars/navigation",
    "esri/toolbars/draw",
    "esri/geometry/Extent",
    "esri/geometry/Point",
    "widget/PropertyWindow/PropertyInfo",
    "widget/navigation/Nav", 
    "widget/menu/MenuBar",
    "widget/Modal",
    "widget/layer/BaseMapAndLayer",
    "widget/left/LeftControl",
    "widget/district/DistrictExtent",
    "widget/rcLayer",
    "widget/Popover", 
    "widget/Tab",
    "widget/print/PrintMap",
    "dojo/NodeList-traverse",
    "dojo/domReady!"
], function (declare, Event, baseFx, Array, win, xhr, dom, domAttr, domClass, domConstruct, domStyle, keys, fx, on, parser,
    mouse, query, topic, Moveable, Mover, Memory, ItemFileWriteStore, notify, script, Menu, MenuItem, Tree, ForestStoreModel, ObjectStoreModel, registry,
    FileUploader, Chart, Pie, Highlight, MoveSlice, Tooltip, MiamiNice, ThreeD, Legend,
    Map, EsriColor, Graphic, SpatialReference, Units, UrlUtils, Editor, TemplatePicker, AttachMentEditor, GeometryService, QueryTask, EsriQuery, Geoprocessor,
    IdentifyTask, IdentifyParameters, ProjectParameters, BufferParameters, ArcGISTiledMapServiceLayer, ArcGISDynamicMapServiceLayer,
    FeatureLayer, DynamicLayerInfo, WMSLayer, WMSLayerInfo, Popup, Scalebar, OverviewMap, Geocoder, Measurement, PictureMarkerSymbol, SimpleMarkerSymbol, SimpleLineSymbol, SimpleFillSymbol,
    PictureFillSymbol, ClassBreaksRenderer, EsriNavigation, Draw, Extent, Point,
    PropertyInfo, Nav, MenuBar, Modal, BaseMapAndLayer, LeftControl, districtExtent, rcLayer) {
    "use strict";

    /* 解析页面中的dojo控件 */
    //parser.parse();

    /*
    var uploader = new FileUploader({
        hoverClass: "uploadHover",
        activeClass: "uploadBtn",
        pressClass: "uploadPress",
        disabledClass: "uploadDisable",
        uploadUrl: ""
    }, "upload");
    */

    var overviewMap = null;

    /* 几何地图服务 */
    var geometryUrl = "http://10.246.63.6:6080/arcgis/rest/services/Utilities/Geometry/GeometryServer";
    var districtUrl = "http://10.246.63.6:6080/arcgis/rest/services/feature/district/MapServer/exts/GetDistrictSOE/GetDistrictByParams";
    esri.config.defaults.io.proxyUrl = "proxy/Handler.ashx";
    /*
    UrlUtils.addProxyRule({
        proxyUrl: "proxy/Handler.ashx",
        urlPrefix: "http://172.24.57.47/service/RSImage/wms"
    });
    */
    //esri.config.defaults.io.alwaysUseProxy = true;

    JZ.gs = new GeometryService(geometryUrl);
    esri.config.defaults.geometryService = JZ.gs;
    

    var customWindow = new Popup({
        fillSymbol: false,
        highlight: false,
        lineSymbol: false,
        markerSymbol:false
    }, domConstruct.create("div"));

    domClass.add(customWindow.domNode, "greyWindow");

    var StepMover = declare([Mover], {
        onMouseMove: function (e) {
            //autoScroll(e);
            var m = this.marginBox;
            if (e.ctrlKey) {
                this.host.onMove(this, { l: parseInt((m.l + e.pageX) / 5) * 5, t: parseInt((m.t + e.pageY) / 5) * 5 });
            } else {
                this.host.onMove(this, { l: m.l + e.pageX, t: m.t + e.pageY });
            }
            Event.stop(e);
        }
    });

    var moveNode = new Moveable(customWindow.domNode, {
        mover: StepMover
    });



    /* 创建地图控件 */
    JZ.mc = new Map("mapDiv", {
        logo: false,
        navigationMode: "css-transforms",
        fadeOnZoom: true,
        sliderStyle: "large",
        sliderPosition: "top-right",
        infoWindow:customWindow
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

        /* 为底图切换添加事件*/
        basemapEvent();
        on(JZ.mc, "click", show3D);
    });

    function show3D(evt) {
        if (JZ.mc.getZoom() >= 9) {
            var projectParams = new ProjectParameters();
            projectParams.geometries = [evt.mapPoint];
            projectParams.outSR = new SpatialReference({
                wkid: 4326
            });

            JZ.gs.project(projectParams, function (suc) {
                window.open('/Cito360view.html?px=' + suc[0].x + '&py=' + suc[0].y);
                //window.open('/Cito360view.html?px=' + suc[0].x + '&py=' + suc[0].y, '全景浏览', 'height=600, width=800, top=100,left=200')
                dom.byId("currentLocation").innerHTML = "经度:" + suc[0].x.toFixed(3) + ", 纬度" + suc[0].y.toFixed(3);
            }, function (err) {
                console.log(err);
            });
        }
    }

    
    /*
    function drawComplete(feature) {
        var draw = new Draw(JZ.mc);
        on(draw, "draw-end", drawComplete)
        draw.activate(Draw.POINT);
        var geometry = feature.geometry;
        var graphic = null;
        var pms = new PictureMarkerSymbol('images/marker/marker.png', 32, 32);
        if (geometry.type === "point") {
            graphic = new Graphic(geometry, pms);
        }
        JZ.mc.graphics.add(graphic);
    }
    */

    /* 删除当前图层 */
    function removeLayers() {
        Array.forEach(JZ.bml, function (baseLayer) {
            JZ.mc.removeLayer(baseLayer);
        });
        JZ.bml = [];
    }

    /* 底图切换事件 */
    function basemapEvent() {
        query(".map-switcher ul li").on("click", function (e) {
            var node = e.currentTarget ? e.currentTarget : e.srcElement;
            var flag = parseInt(domAttr.get(node, "data-dojo-value"));
            if (JZ.cb !== flag) {
                if (JZ.bms.length > 0) {
                    removeLayers();
                    domConstruct.place(node, query(".map-switcher ul")[0], "first");
                    if (flag === 0) {
                        /* 矢量底图 */
                        addVecLayer();
                    } else if (flag === 1) {
                        /* 二十一世纪的卫片 */
                        addImgLayer();
                    } else if (flag === 2) {
                        /* 资源中心的航片 */
                        addRcLayer();
                    }
                }
            }
        });
    }
    /* 添加矢量底图 */
    function addVecLayer() {
        Array.forEach(JZ.bms, function (baseMap) {
            var index = parseInt(baseMap.DATATYPE);
            if (index === 0) {
                var baseMapLayer = new ArcGISTiledMapServiceLayer(baseMap.SERVICEURL);
                JZ.mc.addLayer(baseMapLayer, 0);
                JZ.cb = 0;
                JZ.bml.push(baseMapLayer);
                //overviewMap.destroy();
                //addOverviewMap();
            }
        });
    }
    /* 添加二十一世纪的影像底图 */
    function addImgLayer() {
        Array.forEach(JZ.bms, function (baseMap) {
            var index = parseInt(baseMap.DATATYPE);
            if (index === 2) {
                var baseMapLayer = new ArcGISTiledMapServiceLayer(baseMap.SERVICEURL);
                JZ.mc.addLayer(baseMapLayer, 1);
                JZ.bml.push(baseMapLayer);
            } else if (index === 1) {
                var baseMapLayer1 = new ArcGISTiledMapServiceLayer(baseMap.SERVICEURL);
                JZ.mc.addLayer(baseMapLayer1, 0);
                JZ.bml.push(baseMapLayer1);
            }
            JZ.cb = 1;
            //overviewMap.destroy();
            //addOverviewMap();
        });
    }

    /* 资源中心的航片 */
    var rc = null;
    function addRcLayer() {
        if (!rc) {
            //rc = new rcLayer();
            var layer2 = new WMSLayerInfo({
                name: '41',
                title: 'beijing'
            });
            var resourceInfo = {
                extent: JZ.mc.extent,
                layerInfos: [layer2]
            };
            rc = new WMSLayer('http://172.24.57.47/service/RSImage/wms', {
                resourceInfo: resourceInfo,
                transparent: false,
                srs:'',
                visibleLayers: ['41']
            });
            rc.setImageFormat("jpeg");
        }



        
        JZ.mc.addLayer(rc);









        //JZ.mc.addLayer(rc, 0);
        JZ.bml.push(rc);

        Array.forEach(JZ.bms, function (baseMap) {
            var index = parseInt(baseMap.DATATYPE);
            var baseMapLayer = null;
            if (index === 2) {
                baseMapLayer = new ArcGISTiledMapServiceLayer(baseMap.SERVICEURL);
                JZ.mc.addLayer(baseMapLayer, 1);
                JZ.bml.push(baseMapLayer);
            }
        });

        JZ.cb = 2;
        //overviewMap.destroy();
        //addOverviewMap();
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

    /* 根据用户权限为用户添加功能菜单 */
    var menubar = new MenuBar();
    menubar.show();
    topic.subscribe("topic/AddMenuComplete", function (evt) {
        menubar.defaultNode();
    });
    

    window.onresize = function () {
        menubar.ResizeMenuBar(menubar.objArray);
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
        url: "http://10.246.63.6:6080/arcgis/rest/services/ZCGY_AddressLocator/GeocodeServer",
        name: "Geocoder",
        singleLineFieldName: "City",
        categories: ["City", "Street", "Country"]
    }];
    var geocoder = new Geocoder({
        map: JZ.mc,
        geocoders: geocoders,
        arcgisGeocoder: false,
        autoComplete: true
    }, "mapGeocoder");
    geocoder.startup();

    /* 专题地图 */

    on(dom.byId("contentPane2_return"), "click", function (e) {
        if (JZ.ztlayer) {
            JZ.mc.removeLayer(JZ.ztlayer);
        }
        myStackContainer.selectChild('contentPane1', true);
    });

    on(dom.byId("contentPane1_ztspan"), "click", function (e) {
        Event.stop(e);//?
         var zt = dom.byId("contentPane1_ztspan");
         var oldclassid = 0;
        var classid = 0;
        if (zt) {
            classid = zt.attributes['data-dojo-customid'].value;
            var _a=zt.attributes['data-dojo-customoldid'];
            if(_a)
            {
                oldclassid = _a.value;
            }
        }
        if (oldclassid == classid)
        {
            return;
        }
        zt.setAttribute("data-dojo-customoldid", classid);

        //根据当前分类获取专题图配置，并加载；
       // var that = this;
        var defer = xhr.get("webservice/WebServiceMap.asmx/GetMenuSpecDetailLayer", {
            handleAs: "json",
            timeout: 50000,
            query: { 'classid': classid }
        });
        /* 请求成功事件,获取到用户拥有的菜单功能 */
        defer.then(function (suc) {

            domConstruct.empty("thematicMapPanel");
            // debugger;data_1
            if (!suc)
            {
                dom.byId("contentPane2_count").innerHTML = "0";
                return;
            }
            dom.byId("contentPane2_count").innerHTML = suc.length-1;
           
            
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
                showRoot: false,
                persist: true,
                style: { color: "#034D17" },
                openOnClick: true
            });
            tree.placeAt(dom.byId("thematicMapPanel"));
            tree.startup();

            //加载事件
            on(tree, "click", function (item, node, evt) {
                console.log(item.id[0]);
                
                var featureLayer = new FeatureLayer(item.mapServerUrl + "/" + item.serverindex);
                if (JZ.ztlayer) {
                    JZ.mc.removeLayer(JZ.ztlayer);
                }
                else {
                    JZ.ztlayer = featureLayer;
                }
                JZ.mc.addLayer(featureLayer);
            });

        }, function (err) {
            console.log("调用获取当前专题数据的WebService出现错误，请检查！" + err);
        });
       
    });
    /*
    var data_1 = {
        identifier: 'id',
        label: 'cnName',
        items: null
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
        showRoot: false,
        persist: true,
        style: { color: "#034D17" },
        openOnClick: true
    });
    tree.placeAt(dom.byId("thematicMapPanel"));
    tree.startup();


    on(tree, "click", function (item, node, evt) {
        // console.log(item.id[0]);
        if (item.mapServerUrl) {
            var featureLayer = new FeatureLayer(item.mapServerUrl + "/" + item.serverindex);
            if (JZ.ztlayer) {
                JZ.mc.removeLayer(JZ.ztlayer);
            }
            else {
                JZ.ztlayer = featureLayer;
            }
            JZ.mc.addLayer(featureLayer);
        }

        var featureLayer = new FeatureLayer("http://10.246.63.6:6080/arcgis/rest/services/ZCGY/MapServer/0");

        var symbol = new SimpleFillSymbol();
        symbol.setColor(new EsriColor([80, 255, 0, 0.8]));
        //唯一值渲染专题图
        var renderer = new ClassBreaksRenderer(symbol, "MJ");
        renderer.addBreak(0, 10000, new SimpleFillSymbol().setColor(new EsriColor([0, 255, 0, 0.8])));
        renderer.addBreak(10000, 20000, new SimpleFillSymbol().setColor(new EsriColor([0, 130, 0, 0.8])));
        renderer.addBreak(20000, 30000, new SimpleFillSymbol().setColor(new EsriColor([0, 0, 100, 0.8])));
        renderer.addBreak(30000, 40000, new SimpleFillSymbol().setColor(new EsriColor([0, 0, 200, 0.8])));
        renderer.addBreak(40000, 50000, new SimpleFillSymbol().setColor(new EsriColor([0, 0, 255, 0.8])));
        renderer.addBreak(50000, 60000, new SimpleFillSymbol().setColor(new EsriColor([100, 0, 0, 0.8])));
        renderer.addBreak(60000, 70000, new SimpleFillSymbol().setColor(new EsriColor([150, 0, 0, 0.8])));
        renderer.addBreak(70000, Infinity, new SimpleFillSymbol().setColor(new EsriColor([255, 0, 0, 0.8])));
        featureLayer.setRenderer(renderer);

        JZ.mc.addLayer(featureLayer);
        
    });
*/

    /* 异步请求状态事件 */
    /*
    notify("start", function () {
        console.log("开始异步请求");
    });

    notify("send", function (response, cancel) {
        console.log("发送异步请求，请求数据为：" + response);
    });

    notify("load", function (response) {
        console.log("异步请求正在处理，处理信息:" + response);
    });

    notify("error", function (response) {
        console.log("异步请求出现错误，错误信息:" + response);
    });

    notify("done", function (response) {
        console.log("异步请求结束！");
    });

    notify("stop", function () {
        console.log("异步请求停止！");
    });
    */

    /* 底图切换效果事件 */
    query(".map-switcher").on("mouseover", function (e) {
        domStyle.set(this, "height", "160px");
        query(".map-switcher li").removeClass("basemap-switcher");
    });

    query(".map-switcher").on("mouseout", function (e) {
        domStyle.set(this, "height", "45px");
        var nodes = query(".map-switcher li");
        for (var i = 1; i < nodes.length; i++) {
            domClass.add(nodes[i], "basemap-switcher");
        }
    });

    on(dom.byId("collepse"), "click", function () {
        LeftControl.toggle();


        var defer1 = script.get("http://localhost/webservic/service1.asmx/GetAuth", {
            jsonp: "callback",
            timeout: 10000
        });
        defer1.then(function (suc1) {
            console.log(suc1);
        }, function (err1) {
            console.log(err1);
        });

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



    /* 统计分析
    var chartTwo = new Chart("analystChart");
    chartTwo.setTheme(MiamiNice)//ThreeD
     .addPlot("default", {
         type: Pie,
         fontColor: "#044E18",
         labelOffset: -30,
         radius: 70
     }).addSeries("区域面积统计", [
        { y: 4, text: "海淀区", stroke: "black", tooltip: "500平方米" },
        { y: 2, text: "朝阳区", stroke: "black", tooltip: "1000平方米" },
        { y: 1, text: "东城区", stroke: "black", tooltip: "1500平方米" },
        { y: 1, text: "西城区", stroke: "black", tooltip: "800平方米" }
     ]);
    var anim_a = new MoveSlice(chartTwo, "default");
    var anim_b = new Highlight(chartTwo, "default");
    var anim_c = new Tooltip(chartTwo, "default");
    //chartTwo.resize({ l: 50, t: 50, w: 280, h: 300 });
    //chartTwo.setAxisWindow("区域面积统计", 1, 100, false)
    //chartTwo.setWindow(70, 40, 280, 300, false);
    chartTwo.render();
    domStyle.set(dom.byId("analystChart"), "margin-left", "-60px");
 */



    domConstruct.destroy(dom.byId("loadingBar"));
});
