/**
 * @author Administrator
 */
define(['dojo/_base/declare',
    'dojo/topic',
    'dojo/_base/lang',
    'dijit/_WidgetBase',
    'esri/layers/FeatureLayer',
    'esri/layers/GraphicsLayer',
    'dojo/dom-class',
    'dojo/query',
    'dojo/_base/array',
    'esri/tasks/query',
    'dijit/_TemplatedMixin',
    'dojo/on',
    'dojo/dom',
    'dojo/dom-construct',
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
    'dojo/dom-style',
    'dojo/dom-attr',
    'dijit/Menu',
    'dijit/MenuItem',
    'dijit/MenuSeparator',
    'esri/toolbars/draw',
    'esri/toolbars/edit',
    'dijit/form/Button',
    'widget/dijit/Dialog',
    'dojo/text!./template/DrawDijit.html'],
function (declare, topic, lang, _WidgetBase, FeatureLayer, GraphicsLayer, domClass, query, arrayUtils, EsriQuery,
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
    /* 绘制时使用的GraphicsLayer */
    var drawGraphicLayer = new GraphicsLayer();
    /* 编辑时使用的GraphicsLayer */
    var editGraphicLayer = new GraphicsLayer();
    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        locationLayer: null,
        attrs: null,//传入属性信息
        map: null,
        arrayInfo: [],
        arrs: [],//传入坐标信息
        geometryService: null,
        drawGraphic: null,
        drawTool: null,
        editTool: null,
        updateGraphic: null,
        textSymbolGraphic: null,
        markeredFeatures: null,//已经标注过的要素
        layerType: -1,//0表示点图层，1表示线图层，2表示面图层
        detailShow: false,
        startup: function (info, arr) {
            this.attrs = info;
            this.arrs = arr;
            this.inherited(arguments);
            domConstruct.place(this.domNode, document.body);
            this.initApp();
        },
        initApp: function () {
            //添加编辑时使用的GraphicsLayer
            this.map.addLayer(editGraphicLayer);
            //添加绘图时使用的GraphicsLayer
            this.map.addLayer(drawGraphicLayer);
            //获取传递进来的坐标对
            this.drawTool = new DrawTool(this.map);
            this.editTool = new EsriEditor(this.map);
            this.attachEvent();
            this.fetchData();
            this.setValue();
            //var that = this;
            //require(["widget/dijit/GeoSearchDijit"], function (clazz) {
            //    clazz.getInstance(that.map).startup();
            //});
        },
        attachEvent: function () {
            var that = this;
            on(that.editTool, "scale-stop", function (e) {
                that.reCalculate(e.graphic);
                if (!that.drawGraphic) {
                    that.updateGraphic = e.graphic;
                }
            });
            on(that.editTool, "rotate-stop", function (e) {
                that.reCalculate(e.graphic);
                if (!that.drawGraphic) {
                    that.updateGraphic = e.graphic;
                }
            });
            on(that.editTool, "graphic-move-stop", function (e) {
                that.reCalculate(e.graphic);
                if (!that.drawGraphic) {
                    that.updateGraphic = e.graphic;
                }
            });
            on(that.editTool, "vertex-move-stop", function (e) {
                that.reCalculate(e.graphic);
                if (!that.drawGraphic) {
                    that.updateGraphic = e.graphic;
                }
            });
            on(that.drawTool, "draw-end", function (feature) {
                var geometry = feature.geometry;
                if (that.layerType === 0) {
                    that.drawGraphic = new Graphic(geometry, pms, that.attrs);
                } else if (that.layerType === 2) {
                    that.drawGraphic = new Graphic(geometry, sfs, that.attrs);
                    that.calculateArea(that.drawGraphic);
                }
                drawGraphicLayer.add(that.drawGraphic);
                that.drawTool.deactivate();
                that.editTool.activate(EsriEditor.EDIT_TEXT | EsriEditor.EDIT_VERTICES | EsriEditor.MOVE | EsriEditor.ROTATE | EsriEditor.SCALE, that.drawGraphic);
                that.setDrawBtnDisabled();
            });
        },
        fetchData: function () {
            var that = this;
            var reg = /[^\[+].+[^\]+]/g;
            //如果传入的有坐标，则获取坐标信息
            if (that.arrs) {
                var value = reg.exec(that.arrs);
                var arr = value[0].split("],[");
                arrayUtils.forEach(arr, function (items) {
                    var item = items.split(",");
                    that.arrayInfo.push([parseFloat(item[0]), parseFloat(item[1])]);
                });
            }
            /* 获取图层索引 */
            var index = this.GetBusinessIndexByType(that.attrs);
            /* 添加要素图层 */
            that.featureLayer = new FeatureLayer(JZ.featureUrl + index, {
                outFields: ['*']
            });
            //开始标注
            that.startMarker();
        },
        // 根据业务类型获取图层索引
        GetBusinessIndexByType: function (info) {
            var that = this;
            var type = -1, index = -1;
            try {
                type = parseInt(info.TYPE);
            } catch (e) {
                console.log("业务类型获取失败，请查看业务类型是否合法！");
            }
            arrayUtils.forEach(JZ.typeObj, function (obj) {
                if (type === obj.TYPE) {
                    index = obj.INDEX;
                    that.layerType = obj.GTYPE;
                }
            });
            if (index >= 0) {
                return index;
            } else {
                console.log("获取图层索引失败，请查看传递的业务类型是否有效！");
            }
        },
        setDrawBtnEnabled: function () {
            var that = this;
            that.jzStartDraw.disabled = false;
            that.jzBtnOK.disabled = true;
            that.jzBtnCancel.disabled = true;
        },
        setDrawBtnDisabled: function () {
            var that = this;
            that.jzStartDraw.disabled = true;
            that.jzBtnOK.disabled = false;
            that.jzBtnCancel.disabled = false;
        },
        startMarker: function () {
            var that = this;
            if (that.layerType === 0) {
                var pointOption = document.createElement("option");
                pointOption.value = "5";
                pointOption.innerText = "点";
                that.jzOptions.appendChild(pointOption);
                //that.jzOptions.innerHTML = "<option value='5'>点</option>";
                if (that.arrayInfo && that.arrayInfo.length === 1) {
                    var inPoint = new Point(that.arrayInfo[0], new SpatialReference({
                        wkid: 4326
                    }));
                    //将构造的多边形投影转换
                    var projectParams = new ProjectParameters();
                    projectParams.geometries = [inPoint];
                    projectParams.outSR = that.map.spatialReference;
                    that.geometryService.project(projectParams, function (suc) {
                        if (suc[0]) {
                            that.drawGraphic = new Graphic(suc[0], pms, that.attrs);
                            drawGraphicLayer.add(that.drawGraphic);
                            that.centerMap(that.drawGraphic);
                            that.setDrawBtnDisabled();
                            that.editTool.activate(EsriEditor.EDIT_TEXT | EsriEditor.EDIT_VERTICES | EsriEditor.MOVE | EsriEditor.ROTATE | EsriEditor.SCALE, that.drawGraphic);
                        } else {
                            Dialog.getInstance().startup(null, "提示", "传入的坐标数据错误，已自动忽略！", 1);
                        }
                    }, function (err) {
                        Dialog.getInstance().startup(null, "提示", "传入的坐标数据错误，已自动忽略！", 1);
                    });
                }
            } else if (that.layerType === 2) {
                var option0 = document.createElement("option");
                option0.value = "0";
                option0.innerText = "矩形";
                that.jzOptions.appendChild(option0);

                var option1 = document.createElement("option");
                option1.value = "1";
                option1.innerText = "椭圆";
                that.jzOptions.appendChild(option1);

                var option2 = document.createElement("option");
                option2.value = "2";
                option2.innerText = "圆形";
                that.jzOptions.appendChild(option2);

                var option3 = document.createElement("option");
                option3.value = "3";
                option3.innerText = "多边形";
                that.jzOptions.appendChild(option3);

                var option4 = document.createElement("option");
                option4.value = "4";
                option4.innerText = "自由绘制";
                that.jzOptions.appendChild(option4);

                //that.jzOptions.innerHTML = "<option value='0'>矩形</option><option value='1'>椭圆</option><option value='2'>圆形</option><option value='3'>多边形</option><option value='4'>自由绘制</option>";
                if (that.arrayInfo && that.arrayInfo.length > 2) {
                    //遍历点集列表构造多边形
                    var inPolygon = null;
                    if (that.arrayInfo[0][0] < 180 && that.arrayInfo[0][0] > -180) {
                        inPolygon = new Polygon(that.arrayInfo);
                        //将构造的多边形投影转换
                        var projectParams = new ProjectParameters();
                        projectParams.geometries = [inPolygon];
                        projectParams.outSR = that.map.spatialReference;
                        that.geometryService.project(projectParams, function (suc) {
                            if (suc[0]) {
                                that.drawGraphic = new Graphic(suc[0], sfs, that.attrs);
                                drawGraphicLayer.add(that.drawGraphic);
                                that.setDrawBtnDisabled();
                                that.centerMap(that.drawGraphic);
                                that.editTool.activate(EsriEditor.EDIT_TEXT | EsriEditor.EDIT_VERTICES | EsriEditor.MOVE | EsriEditor.ROTATE | EsriEditor.SCALE, that.drawGraphic);
                            } else {
                                Dialog.getInstance().startup(null, "提示", "传入的坐标数据错误，已自动忽略！", 1);
                            }
                        }, function (err) {
                            Dialog.getInstance().startup(null, "提示", "传入的坐标数据错误，已自动忽略！", 1);
                        });
                    } else {
                        inPolygon = new Polygon(that.arrayInfo);
                        inPolygon.setSpatialReference(that.map.spatialReference);
                        that.drawGraphic = new Graphic(inPolygon, sfs, that.attrs);
                        drawGraphicLayer.add(that.drawGraphic);
                        that.setDrawBtnDisabled();
                        that.centerMap(that.drawGraphic);
                        that.editTool.activate(EsriEditor.EDIT_TEXT | EsriEditor.EDIT_VERTICES | EsriEditor.MOVE | EsriEditor.ROTATE | EsriEditor.SCALE, that.drawGraphic);
                    }
                }
            }
            
            //显示已经标注过的要素
            that.showFeatures();
        },
        showFeatures: function () {
            var that = this;
            var where = "RELID = '" + that.attrs.RELID + "'";
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
                that.markeredFeatures = featureSet.features;
                that.jzAddLength.innerHTML = "已标注(" + featureSet.features.length + ")个";
                if (that.layerType === 0) {
                    if (featureSet.features.length >= 1) {
                        domStyle.set(that.jzDrawTool, "display", "none");
                        that.setNullValue();
                    }
                }
                //显示已经标注过的要素
                that.showGraphic(that.markeredFeatures);
            }, function (err) {
                console.log(err);
            });
        },
        showGraphic: function (features) {
            var that = this;
            if (editGraphicLayer) {
                editGraphicLayer.clear();
            }
            for (var i = 0; i < features.length; i++) {
                if (features[i].geometry.type === "point" || features[i].geometry.type === "multipoint") {
                    graphic = new Graphic(features[i].geometry, pms, features[i].attributes)
                } else if (features[i].geometry.type === "polyline") {
                    graphic = new Graphic(features[i].geometry, sls, features[i].attributes)
                } else {
                    graphic = new Graphic(features[i].geometry, sfs, features[i].attributes)
                }
                editGraphicLayer.add(graphic);
            }
        },
        onStartDraw: function () {
            var selectIndex = this.jzOptions.selectedIndex;
            var selectValue = this.jzOptions.options[selectIndex].value;
            switch (selectValue) {
                case "0"://矩形
                    this.drawTool.activate(DrawTool.RECTANGLE);
                    break;
                case "1"://椭圆
                    this.drawTool.activate(DrawTool.ELLIPSE);
                    break;
                case "2"://圆形
                    this.drawTool.activate(DrawTool.CIRCLE);
                    break;
                case "3"://多边形
                    this.drawTool.activate(DrawTool.POLYGON);
                    break;
                case "4"://自由多边形
                    this.drawTool.activate(DrawTool.FREEHAND_POLYGON);
                    break;
                case "5"://自由多边形
                    this.drawTool.activate(DrawTool.POINT);
                    break;
            }
            this.setValue();
            domStyle.set(this.jzAddNewFeature, "display", "");
            domStyle.set(this.jzUpdateExistFeature, "display", "none");
        },
        reCalculate: function (graphic) {
            var that = this;
            var geometry = graphic.geometry;
            if (that.textSymbolGraphic) {
                drawGraphicLayer.remove(that.textSymbolGraphic);
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
                            drawGraphicLayer.add(that.textSymbolGraphic);
                        });
                    });
                });
            });
        },
        setValue: function () {
            this.jzObjCode.value = this.attrs.OBJCODE;
            this.jzObjName.value = this.attrs.OBJNAME;
            this.jzObjYear.value = this.attrs.OBJYEAR;
            this.jzObjArea.value = this.attrs.OBJAREA;
            this.jzObjPic.value = this.attrs.OPIC;
        },
        setGraphicAttr: function (gra) {
            gra.attributes["OBJCODE"] = this.jzObjCode.value;
            gra.attributes["OBJNAME"] = this.jzObjName.value;
            gra.attributes["OBJYEAR"] = this.jzObjYear.value;
            gra.attributes["OBJAREA"] = this.jzObjArea.value ? this.jzObjArea.value : 0;
            gra.attributes["OBJPIC"] = this.jzObjPic.value;
            gra.attributes["STATE"] = 0;
        },
        //添加标注
        onAddFeature: function () {
            var that = this;
            if (that.drawGraphic) {
                that.setGraphicAttr(that.drawGraphic);
                that.featureLayer.applyEdits([that.drawGraphic], null, null, function (suc) {
                    //添加成功，更新DataGrid
                    that.showFeatures();
                    if (that.layerType !== 0) {
                        that.setDrawBtnEnabled();
                    } else {
                        domStyle.set(that.jzDrawTool, "display", "none");
                    }
                    Dialog.getInstance().startup(null, "提示", "标注成功！", 0);
                }, function (err) {
                    Dialog.getInstance().startup(null, "提示", "标注失败，请重试！", 2);
                });
                that.editTool.deactivate();
                drawGraphicLayer.remove(that.textSymbolGraphic);
            }
        },
        onCancelEditFeature: function () {
            var that = this;
            Dialog.getInstance().startup(function () {
                domStyle.set(that.jzAddNewFeature, "display", "");
                domStyle.set(that.jzUpdateExistFeature, "display", "none");
                that.editTool.deactivate();
                that.drawTool.deactivate();
                that.setDrawBtnEnabled();
                that.showFeatures();
                if (that.textSymbolGraphic) {
                    drawGraphicLayer.remove(that.textSymbolGraphic);
                }
            }, "确认", "确定要取消吗？", 0);
        },
        //取消标注，开始重新绘制
        onCancelFeature: function () {
            var that = this;
            Dialog.getInstance().startup(function () {
                that.removeDrawGraphic();
                that.editTool.deactivate();
                that.drawTool.deactivate();
                that.setDrawBtnEnabled();
            }, "确认", "确定要取消本次绘制结果？", 0);
        },
        //更新要素信息
        onUpdateFeature: function () {
            var that = this;
            if (that.updateGraphic) {
                that.setGraphicAttr(that.updateGraphic);
                that.featureLayer.applyEdits(null, [that.updateGraphic], null, function (suc) {
                    Dialog.getInstance().startup(null, "提示", "要素更新成功！", 1);
                    domStyle.set(that.jzAddNewFeature, "display", "");
                    domStyle.set(that.jzUpdateExistFeature, "display", "none");
                    that.setDrawBtnEnabled();
                }, function (err) {
                    Dialog.getInstance().startup(null, "提示", "要素更新失败，请重试！", 2);
                });
                that.editTool.deactivate();
                drawGraphicLayer.remove(that.textSymbolGraphic);
            }
        },
        //删除要素信息
        onDeleteFeature: function () {
            var that = this;
            if (that.updateGraphic) {
                that.featureLayer.applyEdits(null, null, [that.updateGraphic], function (suc) {
                    editGraphicLayer.remove(that.updateGraphic);
                    that.updateGraphic = null;
                    Dialog.getInstance().startup(null, "提示", "要素删除成功！", 1);
                    if (that.detailShow) {
                        that.showFeatures();
                    }
                    domStyle.set(that.jzAddNewFeature, "display", "");
                    domStyle.set(that.jzUpdateExistFeature, "display", "none");
                    that.setDrawBtnEnabled();
                }, function (err) {
                    Dialog.getInstance().startup(null, "提示", "要素更新失败，请重试！", 2);
                });
                that.editTool.deactivate();
                if (that.textSymbolGraphic) {
                    drawGraphicLayer.remove(that.textSymbolGraphic);
                }
            }
        },
        
        createDataGrid: function () {
            var that = this;
            if (!that.locateResultGrid) {
                that.locateResultGrid = new (declare([OnDemandGrid, Selection, ColumnResizer, Editor]))({
                    collection: null,
                    columns: {
                        CODE: { label: '编码', sortable:false },
                        NAME: { label: '名称', sortable:false },
                        OPER: { label: '操作', field: 'OPER', sortable: false, editor: 'button' },
                    }
                }, that.jzFeatureGrid);
                that.attachGridEvent();
                that.locateResultGrid.startup();
            }
            that.setDGStore();
        },

        selectGridFeature: null,
        selectGridGraphic: null,


        attachGridEvent: function () {
            var that = this;
            on(that.locateResultGrid, "dgrid-select", function (event) {
                that.selectGridFeature = event.rows[0].data.OBJ;
                that.selectGridGraphic = event.rows[0].data.GRA;
                //console.log(event.rows[0].data);
                that.centerMap(that.selectGridFeature);
            });
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
                this.map.setLevel(8);
                this.map.centerAt(cp);
            }
        },
        setNullValue: function () {
            this.jzObjCode.value = '';
            this.jzObjName.value = '';
            this.jzObjYear.value = '';
            this.jzObjArea.value = '';
            this.jzObjPic.value = '';
        },
        setValues: function (fea) {
            this.jzObjCode.value = fea.attributes.OBJCODE ? fea.attributes.OBJCODE : '';
            this.jzObjName.value = fea.attributes.OBJNAME ? fea.attributes.OBJNAME : '';
            this.jzObjYear.value = fea.attributes.OBJYEAR ? fea.attributes.OBJYEAR : '';
            this.jzObjArea.value = fea.attributes.OBJAREA ? fea.attributes.OBJAREA : '';
            this.jzObjPic.value = fea.attributes.OBJPIC ? fea.attributes.OBJPIC : '';
            domStyle.set(this.jzAddNewFeature, "display", "none");
            domStyle.set(this.jzUpdateExistFeature, "display", "");
        },
        showInfoWindow: function (feature) {
            var that = this;
            require(['widget/prop/Prop'], function (PropertyInfo) {
                var infoWin = new PropertyInfo(that.map, feature, 1);
                infoWin.show();
            });
        },
        onBasicShow: function () {
            domClass.add(this.jzBasic, "active");
            domClass.remove(this.jzDetail, "active");
            domClass.add(this.jzTabBasic, "jz-tab-show");
            domClass.remove(this.jzTabDetail, "jz-tab-show");
        },
        onDetailShow: function () {
            domClass.remove(this.jzBasic, "active");
            domClass.add(this.jzDetail, "active");
            domClass.remove(this.jzTabBasic, "jz-tab-show");
            domClass.add(this.jzTabDetail, "jz-tab-show");
            //创建列表
            this.createDataGrid();
            this.detailShow = true;
        },
        setDGStore: function () {
            var that = this;
            var features = that.markeredFeatures;
            var data = [];
            if (editGraphicLayer) {
                editGraphicLayer.clear();
            }
            for (var i = 0; i < features.length; i++) {
                data.push({});
                data[i].id = features[i].attributes.OBJECTID;
                data[i]["CODE"] = features[i].attributes.OBJCODE;
                data[i]["NAME"] = features[i].attributes.OBJNAME;
                data[i]["OPER"] = '编辑';
                data[i]["OBJ"] = features[i];
                var graphic = null;
                if (features[i].geometry.type === "point" || features[i].geometry.type === "multipoint") {
                    graphic = new Graphic(features[i].geometry, pms, features[i].attributes)
                } else if (features[i].geometry.type === "polyline") {
                    graphic = new Graphic(features[i].geometry, sls, features[i].attributes)
                } else {
                    graphic = new Graphic(features[i].geometry, sfs, features[i].attributes)
                }
                data[i]["GRA"] = graphic;
                editGraphicLayer.add(graphic);
            }
            var store = new (declare([Memory, Trackable]))({
                data: data
            });
            that.locateResultGrid.set("collection", store);
            setTimeout(function () {
                query(".dgrid-row-table td input.dgrid-input").on("click", function (e) {
                    that.centerMap(that.selectGridFeature);
                    Dialog.getInstance().startup(function () {
                        that.onBasicShow();
                        that.setValues(that.selectGridFeature);
                        that.updateGraphic = that.selectGridGraphic;
                        that.removeDrawGraphic();
                        that.setDrawBtnDisabled();
                        if (that.updateGraphic.geometry.type === "point" || that.updateGraphic.geometry.type === "multipoint") {
                            that.editTool.activate(EsriEditor.MOVE | EsriEditor.EDIT_VERTICES | EsriEditor.EDIT_TEXT, that.updateGraphic);
                        } else {
                            that.editTool.activate(EsriEditor.EDIT_TEXT | EsriEditor.EDIT_VERTICES | EsriEditor.MOVE | EsriEditor.ROTATE | EsriEditor.SCALE, that.updateGraphic);
                        }
                    }, "确认", "将取消正在编辑的要素，确定要开始编辑？", 0);
                });
            }, 500);
        },
        removeDrawGraphic: function () {
            var that = this;
            if (that.drawGraphic) {
                drawGraphicLayer.remove(that.drawGraphic);
                that.drawGraphic = null;
            }
            if (that.textSymbolGraphic) {
                drawGraphicLayer.remove(that.textSymbolGraphic);
                that.textSymbolGraphic = null;
            }
        }
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
