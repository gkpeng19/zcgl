/**
 * @author Administrator
 */
define(['dojo/_base/declare', 'dojo/_base/lang', 'dojo/topic', 'dojo/_base/array',
    'dijit/_WidgetBase', 'dijit/_TemplatedMixin', 'dojo/on', 'esri/toolbars/draw',
    'esri/Color',
    'esri/symbols/SimpleFillSymbol',
    'esri/symbols/PictureMarkerSymbol',
    'esri/symbols/SimpleLineSymbol',
    'esri/graphic',
    'dgrid/OnDemandGrid',
    'dgrid/Selection',
    'dgrid/editor',
    'dojo/request/xhr',
    'dstore/Memory',
    'dstore/Trackable',
    'dgrid/extensions/ColumnResizer',
    'dojo/dom', 'dojo/dom-construct', 'dojo/dom-style',
    'esri/layers/FeatureLayer',
    'esri/toolbars/edit',
    'dojo/text!./template/NewMarker.html'],
function (declare, lang, topic, arrayUtils, _WidgetBase, _TemplatedMixin,
    on, DrawTool, Color, SimpleFillSymbol, PictureMarkerSymbol,
    SimpleLineSymbol, Graphic, OnDemandGrid, Selection, Editor, xhr,
    Memory, Trackable, ColumnResizer,
    dom, domConstruct, domStyle, FeatureLayer, EsriEditor, template) {
    var instance = null, clazz;
    var pms = new PictureMarkerSymbol('../images/marker/red.png', 24, 28);
    var sls = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 0, 0, 0.5]), 2);
    var sfs = new SimpleFillSymbol(SimpleFillSymbol.STYLE_SOLID, sls, new Color([5, 90, 5, 0.5]));

    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template, _open: false, pNode: null, mixin: false, layerinfo: null,
        drawTool: null, drawGraphic: null, editTool:null,
        startup: function () {
            this.inherited(arguments);
            this.titleNode.innerHTML = "地图标注";
            if (!this.mixin) {
                domConstruct.place(this.domNode, this.pNode);
                this.mixin = true;
                topic.subscribe("topic/RefreshSearchList", lang.hitch(this, this.refreshOptions));
                this.setLayerValue();
                this.drawTool = new DrawTool(JZ.mc);
                this.editTool = new EsriEditor(JZ.mc);
                this.own(on(this.drawTool, "draw-end", lang.hitch(this, this.addGraphicToMap)));
            } else {
                this.onOpen();
            }
        },
        addGraphicToMap: function (feature) {
            var geometry = feature.geometry;
            var that = this;
            if (that.layerinfo.shptype === 0) {
                that.drawGraphic = new Graphic(geometry, sfs, []);
            } else if (that.layerinfo.shptype === 1) {
                that.drawGraphic = new Graphic(geometry, sls, []);
            } else if (that.layerinfo.shptype === 2) {
                that.drawGraphic = new Graphic(geometry, pms, []);
            }
            JZ.mc.graphics.add(that.drawGraphic);
            that.drawTool.deactivate();

            if (that.drawGraphic.geometry.type === "point" || that.drawGraphic.geometry.type === "multipoint") {
                that.editTool.activate(EsriEditor.MOVE | EsriEditor.EDIT_VERTICES | EsriEditor.EDIT_TEXT, that.drawGraphic);
            } else {
                that.editTool.activate(EsriEditor.EDIT_TEXT | EsriEditor.EDIT_VERTICES | EsriEditor.MOVE | EsriEditor.ROTATE | EsriEditor.SCALE, that.drawGraphic);
            }
            //that.editTool.deactivate();

            //that.editTool.activate(EsriEditor.EDIT_TEXT | EsriEditor.EDIT_VERTICES | EsriEditor.MOVE | EsriEditor.ROTATE | EsriEditor.SCALE, that.drawGraphic);

            that.showAttrTable();
        },
        showAttrTable: function () {
            domStyle.set(this.jzEditFStart, 'display', 'none');
            domStyle.set(this.jzEditFCancel, 'display', '');
            //显示属性窗口，便于输入属性值
            var _lcols = window.AllLayerCols.get(this.layerinfo.id);
            var that = this;
            if (!_lcols) {
                xhr.get("webservice/WebServiceMap.asmx/GetColsBylayer", {
                    handleAs: "json",
                    timeout: 10000,
                    query: { layerid: that.layerinfo.id }
                }).then(function (suc) {
                    var o = new layercols(that.layerinfo.id, suc);
                    window.AllLayerCols.add(o);
                    that.createStore(suc);
                }, function (err) {
                    console.log(err);
                });
            } else {
                that.createStore(_lcols);
            }
        },
        createStore: function (cols) {
            var data = [];
            var j = 0;
            for (var i = 0; i < cols.length; i++) {
                if (!(cols[i].COLCODE === "OBJECTID" || cols[i].COLCODE === "FID") && cols[i].COLTYPE == '3') {
                    data.push({});
                    data[j]["id"] = j;
                    data[j]["PENAME"] = cols[i].COLCODE;//英文名
                    data[j]["PNAME"] = cols[i].COLNAME;//中文名
                    data[j]["PVALUE"] = "";
                    j = j + 1;
                }
            }
            this.createDataGrid(data);
        },
        createDataGrid: function (data) {
            var that = this;
            domStyle.set(this.jzEditFeatureAttrs, 'display', '');
            if (!that.locateResultGrid) {
                that.locateResultGrid = new (declare([OnDemandGrid, Selection, ColumnResizer, Editor]))({
                    collection: null,
                    columns: {
                        PNAME: { label: '属性名', sortable: false },
                        PVALUE: { label: '属性值', sortable: false, editor: "text", autoSave: true },
                    }
                }, that.jzEditFeatureAttrs);
                that.locateResultGrid.startup();
            }
            var store = new (declare([Memory, Trackable]))({
                data: data
            });
            that.locateResultGrid.set("collection", store);
        },
        setLayerValue: function () {
            var that = this;
            arrayUtils.forEach(JZ.layersList, function (layer) {
                if (layer.id === JZ.currentLayerID) {
                    that.jzEditLayerName.value = layer.cnName;
                    that.layerinfo = layer;
                    var url = layer.mapServerUrl + '/' + layer.serverindex;
                    that.featureLayer = new FeatureLayer(url.replace('MapServer', 'FeatureServer'));
                    if (layer.shptype == 0) {//面图层
                        domStyle.set(that.jzEditPolygonMethod, "display", "");
                        domStyle.set(that.jzEditLineMethod, "display", "none");
                        domStyle.set(that.jzEditPointMethod, "display", "none");
                    } else if (layer.shptype == 1) {//线图层
                        domStyle.set(that.jzEditPolygonMethod, "display", "none");
                        domStyle.set(that.jzEditLineMethod, "display", "");
                        domStyle.set(that.jzEditPointMethod, "display", "none");
                    } else if (layer.shptype == 2) {//点图层
                        domStyle.set(that.jzEditPolygonMethod, "display", "none");
                        domStyle.set(that.jzEditLineMethod, "display", "none");
                        domStyle.set(that.jzEditPointMethod, "display", "");
                    }
                    domStyle.set(that.jzEditFStart, 'display', '');
                    domStyle.set(that.jzEditFCancel, 'display', 'none');
                }
            });
        },
        refreshOptions: function () {
            this.setLayerValue();
            this.onCancelDraw();
        },
        pauseEvent: function () {
            if (JZ.mapclick) {
                JZ.mapclick.pause();
            }
        },
        onOpen: function () {
            if (domStyle.get(this.domNode, "display") !== "none") {
                return;
            } else {
                domStyle.set(this.domNode, "display", "inline");
            }
        },
        onResume: function () {
            if (JZ.mapclick) {
                JZ.mapclick.resume();
            }
            this.drawTool.deactivate();
            this.editTool.deactivate();
            JZ.mc.graphics.clear();
        },
        onCancel: function () {
            this.onCancelDraw();
            domStyle.set(this.domNode, "display", "none");
            //this.onResume();
        },
        getDrawMethod: function (name) {
            var chks = document.getElementsByName(name);
            for (var i = 0; i < chks.length; i++) {
                if (chks[i].checked) {
                    return chks[i].value;
                }
            }
        },
        onSaveDraw: function () {
            var that = this;
            var coll = that.locateResultGrid.get("collection");
            arrayUtils.forEach(coll.data, function (obj) {
                that.drawGraphic.attributes[obj.PENAME] = obj.PVALUE;
            });
            that.featureLayer.applyEdits([that.drawGraphic], null, null, function (suc) {
                alert("标注成功！");
                that.onCancelDraw();
            }, function (err) {
                alert("标注失败，请重试！");
            });
        },
        //开始绘制图形
        onStartDraw: function () {
            this.pauseEvent();
            JZ.mc.graphics.clear();
            var drawMethod = "";
            if (this.layerinfo.shptype == 0) {//面图层
                drawMethod = this.getDrawMethod("drawRectMethod");
            } else if (this.layerinfo.shptype == 1) {//线图层
                drawMethod = this.getDrawMethod("drawLineMethod");
            } else if (this.layerinfo.shptype == 2) {//点图层
                drawMethod = "POINT";
            }
            this.drawTool.activate(DrawTool[drawMethod]);
        },
        onCancelDraw: function () {
            domStyle.set(this.jzEditFStart, 'display', '');
            domStyle.set(this.jzEditFCancel, 'display', 'none');
            if (this.locateResultGrid) {
                this.locateResultGrid.set("collection", null);
                domStyle.set(this.jzEditFeatureAttrs, 'display', 'none');
            }
            JZ.mc.graphics.clear();
            this.onResume();
        }
    });

    clazz.getInstance = function () {
        if (instance === null) {
            instance = new clazz({
                pNode: "mapDiv"
            });
        }
        return instance;
    };
    return clazz;

});
