/**
 * @author Administrator
 */
define(['dojo/_base/declare', 'dojo/_base/lang', 'dijit/_WidgetBase',
    'esri/layers/FeatureLayer', 'dojo/_base/array', 'esri/tasks/query',
    'dijit/_TemplatedMixin', 'dojo/on', 'dojo/dom', 'dojo/dom-construct',
    'dgrid/OnDemandGrid',
    'dgrid/Keyboard',
    'dgrid/Selection',
    'dgrid/editor',
    'dgrid/test/data/createHierarchicalStore',
    'dstore/Memory',
    'dstore/Trackable',
    'dojo/dnd/Moveable',
    'esri/graphic',
    'dojo/dnd/Mover',
    'dgrid/extensions/DnD',
    'esri/Color',
    'esri/symbols/SimpleFillSymbol',
    'esri/symbols/PictureMarkerSymbol',
    'esri/symbols/SimpleLineSymbol',
    'dgrid/extensions/ColumnResizer',
    'dojo/dom-style', 'dojo/dom-attr', 'dojo/text!./template/LocateResult.html'],
function (declare, lang, _WidgetBase, FeatureLayer, arrayUtils, EsriQuery,
    _TemplatedMixin, on, dom,
    domConstruct, OnDemandGrid, Keyboard, Selection, Editor, createHierarchicalStore,
    Memory, Trackable, Moveable, Graphic, Mover, DnD, Color, SimpleFillSymbol, PictureMarkerSymbol,
    SimpleLineSymbol, ColumnResizer,
    domStyle, domAttr, template) {
    var instance = null, clazz;

    var pms = new PictureMarkerSymbol('../images/marker/red.png', 24, 28);
    var sls = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 0, 0, 0.5]), 2);
    var sfs = new SimpleFillSymbol(SimpleFillSymbol.STYLE_SOLID, sls, new Color([5, 90, 5, 0.5]));

    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        locationLayer: null,
        searchInfo: null,
        map: null,
        layerIndex: 1,
        startup: function (info) {
            this.searchInfo = info;
            this.inherited(arguments);
            domConstruct.place(this.domNode, document.body);
            this.fetchData();
            var StepMover = declare([Mover], {
                onMouseMove: function (e) {
                    var m = this.marginBox;
                    if (e.ctrlKey) {
                        this.host.onMove(this, { l: parseInt((m.l + e.pageX) / 5) * 5, t: parseInt((m.t + e.pageY) / 5) * 5 });
                    } else {
                        this.host.onMove(this, { l: m.l + e.pageX, t: m.t + e.pageY });
                    }
                }
            });
            var moveNode = new Moveable(this.domNode, {
                mover: StepMover
            });
        },
        /* 根据业务类型获取图层索引 */
        GetBusinessIndexByType: function (info) {
            var type = -1, index = -1;
            try {
                type = parseInt(info.TYPE);
            } catch (e) {
                console.log("业务类型获取失败，请查看业务类型是否合法！");
            }
            arrayUtils.forEach(JZ.typeObj, function (obj) {
                if (type === obj.TYPE) {
                    index = obj.INDEX;
                }
            });
            if (index >= 0) {
                return index;
            } else {
                console.log("获取图层索引失败，请查看传递的业务类型是否有效！");
            }
        },
        fetchData: function () {
            var that = this;
            /* 获取图层索引 */
            var index = this.GetBusinessIndexByType(that.searchInfo);
            that.locationLayer = new FeatureLayer(JZ.mapserverUrl + index, {
                outFields: ['*']
            });
            that.layerIndex = index;
            //that.map.addLayer(that.locationLayer);
            if (that.searchInfo) {
                var searchFields = [];
                var searchValue = [];
                if (that.searchInfo.SF && that.searchInfo.SF.indexOf(",") !== -1) {
                    searchFields = that.searchInfo.SF.split(",");
                    searchValue = that.searchInfo.SV.split(",");
                } else {
                    searchFields.push(that.searchInfo.SF);
                    searchValue.push(that.searchInfo.SV);
                }
                var searchFields = that.searchInfo.SF.split(",");
                var searchValue = that.searchInfo.SV.split(",");
                var where = "";
                if (searchFields.length > 1) {
                    for (var i = 0; i < searchFields.length; i++) {
                        if (i === 0) {
                            where += searchFields[i] + " = '" + searchValue[i] + "'";
                        } else {
                            where += " and " + searchFields[i] + " = '" + searchValue[i] + "'";
                        }
                    }
                } else if (searchFields.length === 1) {
                    var value = "";
                    if (Object.prototype.toString.call(searchValue) === "[object Array]") {
                        arrayUtils.forEach(searchValue, function (sv, index) {
                            if (index === 0) {
                                value += "'" + sv + "'";
                            } else {
                                value += ",'" + sv + "'";
                            }
                        });
                    }
                    where += searchFields[0] + " in (" + value + ")";
                }
                that.searchFeature(where, that.locationLayer);
            } else {
                alert("未传递定位参数！");
            }
        },
        searchFeature: function (where, layer) {
            var that = this;
            /* 查询并定位成功后，调用回调函数 */
            var query = new EsriQuery();
            query.outFields = ['*'];
            query.returnGeometry = true;
            query.where = where;
            layer.queryFeatures(query, function (featureSet) {
                var features = featureSet.features;
                that.createDataGrid(features);
                //topic.publish("topic/searchcomplete", features);
            }, function (err) {
            });
        },
        createDataGrid: function (features) {
            var that = this;
            that.locateResultGrid = new (declare([OnDemandGrid, Selection, DnD, ColumnResizer]))({
                collection: null,
                columns: {
                    CODE: { label: '编码' },
                    NAME: { label: '名称' }
                }
            }, that.jzResultList);
            that.attachGridEvent();
            that.locateResultGrid.startup();
            that.setDGStore(features);
        },
        imageLayer: null,
        attachGridEvent: function () {
            var that = this;
            on(that.locateResultGrid, "dgrid-select", function (event) {
                var type = that.searchInfo.TYPE;
                if (type == 11) {
                    var url = event.rows[0].data.OBJ.attributes['OBJPIC'];
                    require(['esri/layers/MapImageLayer', 'esri/layers/MapImage'], function (MapImageLayer, MapImage) {
                        if (!that.imageLayer) {
                            that.imageLayer = new MapImageLayer();
                            that.map.addLayer(that.imageLayer, 10);
                        }
                        that.imageLayer.removeAllImages();
                        var img = new MapImage({
                            'extent': event.rows[0].data.OBJ.geometry.getExtent(),
                            'href': "../" + url
                        });
                        that.imageLayer.addImage(img);
                    });
                } else {
                    var feature = event.rows[0].data.OBJ;
                    that.centerMap(feature);
                }
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
            //定位以后，显示要素的属性信息
            this.showInfoWindow(feature);
            if (cp) {
                this.map.centerAt(cp);
            }
        },
        showInfoWindow: function (feature) {
            var that = this;
            require(['widget/prop/Prop'], function (PropertyInfo) {
                var infoWin = new PropertyInfo(that.map, feature, that.layerIndex);
                infoWin.show();
            });
        },
        setDGStore: function (features) {
            var that = this;
            var data = [];
            for (var i = 0; i < features.length; i++) {
                data.push({});
                data[i].id = features[i].attributes.OBJECTID;
                data[i]["CODE"] = features[i].attributes.OBJCODE;
                data[i]["NAME"] = features[i].attributes.OBJNAME;
                data[i]["OBJ"] = features[i];
                var graphic = null;
                if (features[i].geometry.type === "point" || features[i].geometry.type === "multipoint") {
                    graphic = new Graphic(features[i].geometry, pms, features[i].attributes)
                } else if (features[i].geometry.type === "polyline") {
                    graphic = new Graphic(features[i].geometry, sls, features[i].attributes)
                } else {
                    graphic = new Graphic(features[i].geometry, sfs, features[i].attributes)
                }
                var type = that.searchInfo.TYPE;
                if (type != 11) {
                    that.map.graphics.add(graphic);
                }
            }
            var store = new (declare([Memory, Trackable]))({
                data: data
            });
            that.locateResultGrid.set("collection", store);
        }
    });

    clazz.getInstance = function (map) {
        if (instance === null) {
            instance = new clazz({
                map: map
            });
        }
        return instance;
    };
    return clazz;

});
