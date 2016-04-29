/**
 * @author Administrator
 */
define(['dojo/_base/declare',
        'dojo/_base/lang',
        'dijit/_WidgetBase',
        'dijit/_TemplatedMixin',
        'dojo/dom-construct',
        'dojo/_base/array',
        'esri/layers/FeatureLayer',
        'esri/tasks/query',
        'esri/tasks/GeometryService',
        'esri/symbols/SimpleFillSymbol',
        'esri/symbols/SimpleLineSymbol',
        'esri/Color',
        'esri/graphic',
        'dojo/topic',
        'esri/tasks/BufferParameters',
        'dojo/text!./BufferAnalyst.html'],
function (declare, lang, _WidgetBase, _TemplatedMixin, domConstruct,
    arrayUtils, FeatureLayer, EsriQuery, GeometryService, SimpleFillSymbol, SimpleLineSymbol,
    Color, Graphic, topic, BufferParameters, template) {
    var instance = null, clazz;
    var sfs = new SimpleFillSymbol(SimpleFillSymbol.STYLE_SOLID,
                            new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID,
                            new Color([8, 234, 8]), 1), new Color([4, 78, 23, 0.3]));
    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        pNode: null,
        map: null,
        startup: function () {
            this.inherited(arguments);
            domConstruct.place(this.domNode, this.pNode);
            topic.subscribe("topic/RefreshSearchList", lang.hitch(this, this.refreshOptions));
        },

        refreshOptions: function () {
            var that = this;
            arrayUtils.forEach(JZ.layersList, function (layer) {
                if (layer.id === JZ.currentLayerID) {
                    that.jzBufferLayerName.value = layer.cnName;
                    if (JZ.currentYear !== '') {
                        that.jzBufferYear.value = JZ.currentYear;
                    } else {
                        that.jzBufferYear.value = "所有年份";
                    }
                }
            });
        },

        postCreate: function () {
            this.refreshOptions();
        },
        onStart: function () {
            var that = this;
            JZ.loading.show();
            var bufferLayer = null;
            arrayUtils.forEach(JZ.layersList, function (layer) {
                if (layer.id === JZ.currentLayerID) {
                    bufferLayer = layer;
                }
            });
            if (bufferLayer) {
                var featureUrl = bufferLayer.mapServerUrl + "/" + bufferLayer.serverindex;
                var featureLayer = new FeatureLayer(featureUrl, {
                    mode: FeatureLayer.MODE_AUTO
                });
                var eQuery = new EsriQuery();
                if (JZ.currentYear !== '') {
                    eQuery.where = "OBJYEAR = '" + JZ.currentYear + "'";
                } else {
                    eQuery.where = "1 like 1";
                }
                eQuery.geometry = that.map.extent;
                eQuery.num = 1000;
                eQuery.outFields = ["*"];
                var params = new BufferParameters();
                featureLayer.queryFeatures(eQuery, function (featureSet) {
                    var features = featureSet.features;
                    if (features.length > 0) {
                        var geometries = [];
                        arrayUtils.forEach(features, function (f) {
                            geometries.push(f.geometry);
                        });
                        params.geometries = geometries;
                        var dis = that.jzBufferRadius.value;
                        params.distances = [dis];
                        params.unionResults = false;
                        if (that.jzBufferUnit.options[0].selected) {
                            params.unit = GeometryService.UNIT_METER;
                        } else {
                            params.unit = GeometryService.UNIT_KILOMETER;
                        }
                        params.outSpatialReference = that.map.spatialReference;
                        JZ.gs.buffer(params, function (geometries) {
                            arrayUtils.forEach(geometries, function (geo) {
                                var graphic = new Graphic(geo, sfs);
                                JZ.mc.graphics.add(graphic);
                            });
                            JZ.loading.hide();
                        }, function (err) {
                            console.log(err);
                            JZ.loading.hide();
                        });
                    } else {
                        JZ.loading.hide();
                    }
                });
            }
        }
    });

    clazz.getInstance = function () {
        if (instance === null) {
            instance = new clazz({
                map: JZ.mc,
                pNode: "BufferAnalyst"
            });
        }
        return instance;
    };
    return clazz;

});
