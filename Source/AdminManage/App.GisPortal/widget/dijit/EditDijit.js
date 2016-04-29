/**
 * @author Administrator
 */
define(['dojo/_base/declare', 'dojo/topic', 'dojo/_base/lang', 'dijit/_WidgetBase',
    'esri/layers/FeatureLayer', 'esri/layers/GraphicsLayer',
    'dojo/dom-class',
    'dojo/query',
    'dojo/request/xhr',
    'dojo/_base/array', 'esri/tasks/query',
    'dijit/_TemplatedMixin', 'dojo/on', 'dojo/dom', 'dojo/dom-construct',
    'dgrid/OnDemandGrid',
    'dgrid/Keyboard',
    'dgrid/Selection',
    'dgrid/editor',
    'dgrid/test/data/createHierarchicalStore',
    'dstore/Memory',
    'dstore/Trackable',
    'dojo/dnd/Moveable',
    'esri/SpatialReference',
    'esri/graphic',
    'esri/geometry/Polygon',
    'esri/geometry/Point',
    'esri/tasks/ProjectParameters',
    'esri/tasks/GeometryService',
    'esri/tasks/AreasAndLengthsParameters',
    'dojo/dnd/Mover',
    'dgrid/extensions/DnD',
    'esri/Color',
    'esri/symbols/SimpleFillSymbol',
    'esri/symbols/PictureMarkerSymbol',
    'esri/symbols/SimpleLineSymbol',
    'esri/symbols/TextSymbol',
    'dgrid/extensions/ColumnResizer',
    'dojo/dom-style', 'dojo/dom-attr',
    'dijit/Menu',
    'dijit/MenuItem',
    'dijit/MenuSeparator',
    'esri/toolbars/draw',
    'esri/toolbars/edit',
    'dijit/form/Button',
    'widget/dijit/Dialog',
    'dojo/text!./template/EditDijit.html'],
function (declare, topic, lang, _WidgetBase, FeatureLayer, GraphicsLayer, domClass, query, xhr, arrayUtils, EsriQuery,
    _TemplatedMixin, on, dom,
    domConstruct, OnDemandGrid, Keyboard, Selection, Editor, createHierarchicalStore,
    Memory, Trackable, Moveable, SpatialReference, Graphic, Polygon, Point, ProjectParameters, GeometryService, AreasAndLengthsParameters,
    Mover, DnD, Color, SimpleFillSymbol, PictureMarkerSymbol,
    SimpleLineSymbol, TextSymbol, ColumnResizer,
    domStyle, domAttr, Menu, MenuItem, MenuSeparator, DrawTool, EsriEditor, Button, Dialog,
    template) {
    var instance = null, clazz;
    var pms = new PictureMarkerSymbol('../images/marker/red.png', 24, 28);
    var sls = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 0, 0, 0.5]), 2);
    var sfs = new SimpleFillSymbol(SimpleFillSymbol.STYLE_SOLID, sls, new Color([5, 90, 5, 0.5]));
    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        editTool: null,
        featureIndex: -1,
        featureOID: -1,
        featureLayer: null,
        map: null,
        geometryService: null,
        updateGraphic: null,
        textSymbolGraphic: null,
        isDataChanged: false,
        cbk: null,
        startup: function (index, oid, cbk, id) {
            this.featureIndex = index;
            this.featureOID = oid;
            this.cbk = cbk;
            this.layerID = id;
            this.inherited(arguments);
            domConstruct.place(this.domNode, document.body);
            this.editTool = new EsriEditor(this.map);
            this.showFeatures();
            this.attachEvent();
        },
        showFeatures: function () {
            var that = this;
            that.featureLayer = new FeatureLayer(JZ.featureUrl + that.featureIndex, {
                outFields: ['*']
            });
            var where = "OBJECTID = " + that.featureOID;
            that.searchFeature(where, that.featureLayer);
        },
        searchFeature: function (where, layer) {
            var that = this;
            //查询并定位成功后，调用回调函数
            var query = new EsriQuery();
            query.outFields = ['*'];
            query.returnGeometry = true;
            query.where = where;
            layer.queryFeatures(query, function (featureSet) {
                var features = featureSet.features;
                if(features.length === 1){
                    //that.getCols(features[0]);
                    that.createDataGrid(features[0]);
                }
            }, function (err) {
                console.log(err);
            });
        },
        createDataGrid: function (feature) {
            var that = this;
            if (!that.locateResultGrid) {
                that.locateResultGrid = new (declare([OnDemandGrid, Selection, ColumnResizer, Editor]))({
                    collection: null,
                    columns: {
                        PROPERTYNAME: { label: '属性名', sortable: false },
                        PROPERTYVALUE: { label: '属性值', sortable: false, editor: "text", autoSave: true },
                    }
                }, that.jzFeatureGrid);
                that.locateResultGrid.startup();
            }
            that.getCols(feature);
            //that.setDGStore(feature);
        },
        getCols: function (feature) {
            var that = this;
            var _lcols = window.AllLayerCols.get(that.layerID);
            if (_lcols) {
                that.createStore(_lcols, feature);
            } else {
                xhr.get("../webservice/WebServiceMap.asmx/GetColsBylayer", {
                    handleAs: "json",
                    timeout: 10000,
                    query: { layerid: that.layerID}
                }).then(function (suc) {
                    var o = new layercols(that.layerID, suc);
                    window.AllLayerCols.add(o);
                    that.createStore(suc, feature);
                    //that.createDataGrid(that.createStore(suc, feature));
                }, function (err) {
                    console.log(err);
                });
            }
        },




        createStore: function (cols, feature) {
            var that = this;
            var data = [];
            var attributes = feature.attributes;
            var i = 0;
            for (var key in attributes) {
                if (!(key === "OBJECTID" || key === "FID") && key != 'SHAPE') {
                    //if (attributes[key] &&
                    //    attributes[key].toString().replace(/(^\s*)|(\s*$)/g, '') != "" &&
                    //    attributes[key].toString() != 'null' &&
                    //    attributes[key].toString() != 'Null' &&
                    //    attributes[key].toString() != 'undefined') {
                        data.push({});
                        data[i]["id"] = i;
                        data[i]["PROPERTYENAME"] = key;//英文名
                        if (key == 'SHAPE.AREA') {
                            data[i]["PROPERTYNAME"] = '面积';//中文名
                        } else if (key == 'SHAPE.LEN') {
                            data[i]["PROPERTYNAME"] = '周长';//中文名
                        } else {
                            data[i]["PROPERTYNAME"] = key;
                        }
                        data[i]["PROPERTYVALUE"] = attributes[key];
                        i = i + 1;
                    //}
                }
            }
            if (cols) {
                for (var j = 0; j < cols.length; j++) {
                    var col = cols[j];
                    arrayUtils.forEach(data, function (info) {
                        if (info.PROPERTYENAME === col.COLCODE) {
                            info.PROPERTYNAME = col.COLNAME;
                        }
                    });
                }
            }
            var pattern = new RegExp("^[a-zA-Z_0-9]*$");
            var flag = 0;
            for (var m = 0, length = data.length; m < length; m++) {
                if (pattern.test(data[flag].PROPERTYNAME)) {
                    data.splice(flag, 1);
                } else {
                    flag = flag + 1;
                }
            }












            var store = new (declare([Memory, Trackable]))({
                data: data
            });
            that.locateResultGrid.set("collection", store);

            on(that.locateResultGrid, "dgrid-datachange", function (event) {
                that.isDataChanged = true;
                console.log("数据发生了变化：" + event.cell.row.data);
            });

            var graphic = null;
            if (feature.geometry.type === "point" || feature.geometry.type === "multipoint") {
                graphic = new Graphic(feature.geometry, pms, feature.attributes)
            } else if (feature.geometry.type === "polyline") {
                graphic = new Graphic(feature.geometry, sls, feature.attributes)
            } else {
                graphic = new Graphic(feature.geometry, sfs, feature.attributes)
            }
            that.updateGraphic = graphic;
            that.map.graphics.add(graphic);
            that.centerMap(feature);
            that.editTool.activate(EsriEditor.EDIT_TEXT | EsriEditor.EDIT_VERTICES | EsriEditor.MOVE | EsriEditor.ROTATE | EsriEditor.SCALE, graphic);
        },

        
        

        attachEvent: function () {
            var that = this;
            on(that.editTool, "scale-stop", function (e) {
                that.reCalculate(e.graphic);
                that.updateGraphic = e.graphic;
            });
            on(that.editTool, "rotate-stop", function (e) {
                that.reCalculate(e.graphic);
                that.updateGraphic = e.graphic;
            });
            on(that.editTool, "graphic-move-stop", function (e) {
                that.reCalculate(e.graphic);
                that.updateGraphic = e.graphic;
            });
            on(that.editTool, "vertex-move-stop", function (e) {
                that.reCalculate(e.graphic);
                that.updateGraphic = e.graphic;
            });
        },
        reCalculate: function (graphic) {
            var that = this;
            var geometry = graphic.geometry;
            if (that.textSymbolGraphic) {
                that.map.graphics.remove(that.textSymbolGraphic);
                that.textSymbolGraphic = null;
            }
            if (geometry.type === "polygon" || geometry.type === "extent") {
                that.calculateArea(graphic);
            }
        },
        calculateArea: function (graphic) {
            var that = this;
            var areasAndLengthParams = new AreasAndLengthsParameters();
            areasAndLengthParams.lengthUnit = GeometryService.UNIT_METER;
            areasAndLengthParams.areaUnit = GeometryService.UNIT_SQUARE_METERS;
            that.geometryService.simplify([graphic.geometry], function (simplifiedGeometries) {
                areasAndLengthParams.polygons = simplifiedGeometries;
                that.geometryService.areasAndLengths(areasAndLengthParams, function (res) {
                    var area = res.areas[0];
                    var length = res.lengths[0];
                    var textSymbol = new TextSymbol("面积:" + area.toFixed(3) + "平方米").setColor(new Color([18, 23, 8]));
                    that.geometryService.labelPoints(simplifiedGeometries, function (pois) {
                        arrayUtils.forEach(pois, function (poi) {
                            that.textSymbolGraphic = new Graphic(poi, textSymbol);
                            that.map.graphics.add(that.textSymbolGraphic);
                        });
                    });
                });
            });
        },
        //更新要素信息
        onUpdateFeature: function () {
            var that = this;
            if (that.updateGraphic) {
                if (that.isDataChanged) {
                    that.setGraAttr();
                }
                Dialog.getInstance().startup(function () {
                    that.featureLayer.applyEdits(null, [that.updateGraphic], null, function (suc) {
                        Dialog.getInstance().startup(null, "提示", "要素更新成功！", 1);
                    }, function (err) {
                        Dialog.getInstance().startup(null, "提示", "要素更新失败，请重试！", 2);
                    });
                    //that.editTool.deactivate();
                    //if (that.textSymbolGraphic) {
                    //    that.map.graphics.remove(that.textSymbolGraphic);
                    //}
                }, "警告", "确定要更新要素属性吗？", 0);
            }
        },
        setGraAttr: function () {
            var that = this;
            var coll = that.locateResultGrid.get("collection");
            for (var key in that.updateGraphic.attributes) {
                arrayUtils.forEach(coll.data, function (obj) {
                    if (key === obj.PNAME) {
                        that.updateGraphic.attributes[key] = obj.PVALUE;
                    }
                });
            }
        },
        //删除要素信息
        onDeleteFeature: function () {
            var that = this;
            if (that.updateGraphic) {
                Dialog.getInstance().startup(function () {
                    that.featureLayer.applyEdits(null, null, [that.updateGraphic], function (suc) {
                        that.map.graphics.remove(that.updateGraphic);
                        that.updateGraphic = null;
                        Dialog.getInstance().startup(function () {
                            if (that.cbk) {
                                top[that.cbk]();
                            }
                        }, "提示", "要素删除成功！", 1);
                    }, function (err) {
                        Dialog.getInstance().startup(null, "提示", "要素删除失败，请重试！", 2);
                    });
                    that.editTool.deactivate();
                    if (that.textSymbolGraphic) {
                        that.map.graphics.remove(that.textSymbolGraphic);
                    }
                }, "警告", "删除后将无法恢复，请谨慎操作！", 0);
            }
        },
        centerMap: function (feature) {
            var cp = null;
            if (feature.geometry.type === "polygon") {
                cp = feature.geometry.getCentroid();
            } else if (feature.geometry.type === "point") {
                cp = feature.geometry;
            } else if (feature.geometry.type === "polyline") {
                cp = feature.geometry.getPoint(0, 0);
            } else if (feature.geometry.type === "multipoint") {
                cp = feature.geometry.getPoint(0);
            } else if (feature.geometry.type === "extent") {
                cp = feature.geometry.getCenter();
            }
            if (cp) {
                this.map.centerAt(cp);
            }
        }
        //附件管理
        //onAttachManage: function () {
        //    require(['widget/dijit/Attachments'], function (clazz) {
        //        clazz.getInstance().startup();
        //    });
        //}
    });

    clazz.getInstance = function (map, gs) {
        if (instance === null) {
            instance = new clazz({
                map: map,
                geometryService: gs
            });
        }
        return instance;
    };
    return clazz;

});
