/**
 * @author Administrator
 */
define(['dojo/_base/declare', 'dojo/_base/lang', 
    'dijit/_WidgetBase',
    'dijit/_TemplatedMixin',
    'dojo/on',
    'dojo/dom',
    'dojo/dom-construct',
    'dojo/_base/array',
    'dojo/dom-style',
    'dojo/topic',
    'dojo/dom',
    'dojo/query',
    'dijit/popup',
    'dijit/ColorPalette',
    'esri/dijit/TimeSlider',
    'esri/TimeExtent',
    'esri/Color',
    'esri/symbols/SimpleMarkerSymbol',
    'esri/symbols/SimpleLineSymbol',
    'esri/symbols/SimpleFillSymbol',
    'esri/renderers/ClassBreaksRenderer',
    'esri/layers/FeatureLayer',
    'esri/layers/TimeInfo',
    'esri/symbols/PictureMarkerSymbol',
    'esri/renderers/SimpleRenderer',
    'dijit/TooltipDialog',
    'dojo/text!./HistoryAnalyst.html'],
function (declare, lang, _WidgetBase, _TemplatedMixin, on, dom, domConstruct,
    arrayUtils,
    domStyle,
    topic, dom, query, Popup, ColorPalette, TimeSlider, TimeExtent, Color,
    SimpleMarkerSymbol, SimpleLineSymbol, SimpleFillSymbol,
    ClassBreaksRenderer, FeatureLayer, TimeInfo,
    PictureMarkerSymbol, SimpleRenderer, TooltipDialog,
    template) {
    var instance = null, clazz;

    var sls = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 0, 0, 1]), 2);
    var sfs = new SimpleFillSymbol(SimpleFillSymbol.STYLE_SOLID, sls, new Color([255, 0, 0, 1]));
    var sms = new SimpleMarkerSymbol("circle", 20, sls, new Color([255, 0, 0, 1]));

    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        pNode: null,
        map: null,
        layer: null,
        tooltipDialog: null,
        colorPalette: null,
        startup: function () {
            this.inherited(arguments);
            this.tooltipDialog = new TooltipDialog({
                title: "区域切换",
                content: "<div id='jzHistoryColor'></div>"
            });
            domConstruct.place(this.domNode, this.pNode);
            this.own(on(this.jzHistoryViewMethod, "change", lang.hitch(this, this.toggleView)));
            topic.subscribe("topic/RefreshSearchList", lang.hitch(this, this.refreshOptions));
        },
        toggleView: function (e) {
            this.reset();
            if (this.jzHistoryViewMethod.selectedIndex === 1) {
                domStyle.set(this.jzHistoryViewTable, "display", "");
                this.showTimeTable();
            } else {
                domStyle.set(this.jzHistoryViewTable, "display", "none");
            }
        },
        refreshOptions: function () {
            JZ.dynamicLayer.setVisibility(true);
            var that = this;
            arrayUtils.forEach(JZ.layersList, function (layer) {
                if (layer.id === JZ.currentLayerID) {
                    that.jzHistoryLayerName.value = layer.cnName;
                    that.layer = layer;
                }
            });
            this.removeLayer();
            this.reset();
            this.jzHistoryViewMethod[0].selected = true;
            if (!this.layer.datayears) {
                this.jzHistoryViewMethod.disabled = true;
                this.jzHistoryBtnOK.disabled = true;
            } else {
                this.jzHistoryViewMethod.disabled = false;
                this.jzHistoryBtnOK.disabled = false;
            }
        },
        postCreate: function () {
            this.refreshOptions();
        },
        removeLayer: function () {
            domConstruct.empty(this.jzHistoryViewTable);
        },
        onStart: function () {
            this.reset();
            //JZ.dynamicLayer.setVisibility(false);
            if (!this.layer.datayears) {
                alert("当前选中图层没有历史数据，无法进行历史分析！");
            } else {
                if (this.jzHistoryViewMethod.selectedIndex === 0) {
                    this.showTimeBar();
                } else {
                    var years = this.layer.datayears.split(",");
                    if (years.length > 0) {
                        var url = this.layer.mapServerUrl + '/' + this.layer.serverindex;
                        this.featureLayer = new FeatureLayer(url, {
                            mode: FeatureLayer.MODE_ONDEMAND
                        });
                        var that = this;
                        on(that.featureLayer, "load", function () {
                            if (that.featureLayer.geometryType === 'esriGeometryPoint') {
                                var renderer = new ClassBreaksRenderer(sms, "OBJYEAR");
                                arrayUtils.forEach(years, function (year) {
                                    var y = parseInt(year);
                                    renderer.addBreak(y, y + 1, new SimpleMarkerSymbol("circle", 20, sls, Color.fromRgb(domStyle.get(dom.byId("jzHistory" + year), "backgroundColor"))));
                                });
                                that.featureLayer.setRenderer(renderer);
                            } else if (that.featureLayer.geometryType === 'esriGeometryPolyline') {
                                var renderer = new ClassBreaksRenderer(sls, "OBJYEAR");
                                arrayUtils.forEach(years, function (year) {
                                    var y = parseInt(year);
                                    renderer.addBreak(y, y + 1, new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, Color.fromRgb(domStyle.get(dom.byId("jzHistory" + year), "backgroundColor")), 2));
                                });
                                that.featureLayer.setRenderer(renderer);
                            } else {
                                var renderer = new ClassBreaksRenderer(sfs, "OBJYEAR");
                                arrayUtils.forEach(years, function (year) {
                                    var y = parseInt(year);
                                    renderer.addBreak(y, y + 1, new SimpleFillSymbol().setColor(Color.fromRgb(domStyle.get(dom.byId("jzHistory" + year), "backgroundColor"))));
                                });
                                that.featureLayer.setRenderer(renderer);
                            }
                        });
                        this.map.addLayer(this.featureLayer);
                    }
                }
            }
        },
        showTimeTable: function () {
            var years = this.layer.datayears.split(",");
            var htmlStr = "<tr><th>年份</th><th>颜色</th></tr>";
            arrayUtils.forEach(years, function (year) {
                htmlStr += "<tr><th>" + year + "</th><th><span style='display:block;height:18px;background:#F00' id='jzHistory" + year + "'></span></th></tr>";
            });
            this.jzHistoryViewTable.innerHTML = htmlStr;
            var that = this;
            query(".jzHistoryTable tr span").on("click", function (e) {
                var self = this;
                Popup.open({ popup: that.tooltipDialog, around: self });
                if (that.colorPalette) {
                    that.colorPalette.destroy(true);
                }
                that.colorPalette = new ColorPalette({
                    palette: "7x10", onChange: function (val) {
                        domStyle.set(self, "background", val);
                        Popup.close(that.tooltipDialog);
                    }
                }, "jzHistoryColor");
                that.colorPalette.startup();
            });
        },
        showTimeBar: function () {
            var history = "<div id='jz-history' class='history'><span id='historyCloseBtn'>X</span><div id='historyDiv'></div></div>";
            domConstruct.place(history, dom.byId('navContent'), 'last');
            var minyear = 2000;
            var maxyear = (new Date()).getYear();
            var years = this.layer.datayears.split(",");
            arrayUtils.forEach(years, function (year) {
                var c = parseInt(year);
                if (c < minyear) {
                    minyear = c;
                }
                if (c > maxyear) {
                    maxyear = c;
                }
            });
            var timeExtent = new TimeExtent();
            timeExtent.startTime = new Date(minyear, 1, 1);
            timeExtent.endTime = new Date(maxyear, 12, 31);
            /* 创建事件滑块 */
            this.timeSlider = new TimeSlider({
                style: 'width: 100%;'
            }, dom.byId('historyDiv'));
            this.timeSlider.setThumbCount(2);
            this.timeSlider.createTimeStopsByTimeInterval(timeExtent, 1, TimeInfo.UNIT_YEARS);
            this.timeSlider.setThumbIndexes([0, 1]);
            this.timeSlider.setThumbMovingRate(3000);
            this.timeSlider.setLoop(true);
            this.timeSlider.startup();
            var labels = arrayUtils.map(this.timeSlider.timeStops, function (timeStop, i) {
                return timeStop.getUTCFullYear();
            });
            var url = this.layer.mapServerUrl + '/' + this.layer.serverindex;
            this.featureLayer = new FeatureLayer(url, {
                mode: FeatureLayer.MODE_SNAPSHOT,
                outFields: ["*"]
            });
            this.featureLayer.setTimeDefinition(timeExtent);
            var that = this;
            on(that.featureLayer, 'load', function (e) {
                if (that.featureLayer.geometryType === 'esriGeometryPoint') {
                    var symbol = new PictureMarkerSymbol('images/marker/red.png', 21, 28);
                    var renderer = new SimpleRenderer(symbol);
                    that.featureLayer.setRenderer(renderer);
                }
            });
            this.map.addLayer(that.featureLayer);
            that.timeSlider.setLabels(labels);
            this.map.setTimeSlider(that.timeSlider);
            var that = this;
            on(dom.byId("historyCloseBtn"), "click", function (evt) {
                that.reset();
            });
        },
        reset: function () {
            if (this.featureLayer) {
                this.map.removeLayer(this.featureLayer);
                this.featureLayer = null;
            }
            if (this.colorPalette) {
                this.colorPalette.destroy(true);
            }
            if (dom.byId("jz-history")) {
                domConstruct.destroy(dom.byId("jz-history"));
            }
            if (this.timeSlider) {
                this.timeSlider.destroy();
                this.timeSlider = null;
            }
            if (this.tooltipDialog) {
                Popup.close(this.tooltipDialog);
            }
            this.map.setTimeSlider(null);
            this.map.setTimeExtent(null);
            //JZ.dynamicLayer.setVisibility(true);
        }
    });

    clazz.getInstance = function () {
        if (instance === null) {
            instance = new clazz({
                map: JZ.mc,
                pNode: "HistoryAnalyst"
            });
        }
        return instance;
    };
    return clazz;

});
