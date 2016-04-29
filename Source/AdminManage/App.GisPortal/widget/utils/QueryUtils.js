/**
* @author wangyafei
* 绿色产业类别
*/
define('widget/utils/QueryUtils',
    ['dojo/_base/declare',
     'dojo/_base/array',
     'esri/InfoTemplate',
     'esri/tasks/IdentifyTask',
     'esri/tasks/IdentifyParameters',
     'widget/dijit/TabContainer',
     'widget/utils/GraphicUtils'
    ], function (declare, arrayUtils, InfoTemplate, IdentifyTask, IdentifyParameters, MyTabContainer, GraphicUtils) {
        "use strict"
        var instance = null, clazz;
        clazz = declare(null, {
            map: null,
            index: -1,
            constructor: function (map) {
                this.map = map;
            },
            search: function (evt) {
                if (JZ.dynamicLayer) {
                    var identifyTask = new IdentifyTask(JZ.dynamicLayer.url);
                    var params = new IdentifyParameters();
                    params.tolerance = 5;
                    params.returnGeometry = true;
                    this.getLayerIds();
                    if (this.index !== -1) {
                        params.layerIds = [this.index];
                        params.layerOption = IdentifyParameters.LAYER_OPTION_ALL;
                        params.width = this.map.width;
                        params.height = this.map.height;
                        params.geometry = evt.mapPoint;
                        params.mapExtent = this.map.extent;
                        //params.layerDefinitions = [];
                        //if (JZ.district.length === 1) {
                        //    arrayUtils.forEach(JZ.layerinfos, function (info) {
                        //        params.layerDefinitions[info.IDX] = "OBJCODE = '" + JZ.district[0].CODE + "' or 邮政编码 = '" + JZ.district[0].CODE + "'";
                        //    });
                        //}
                        var that = this;
                        identifyTask.execute(params, function (featureSet) {
                            var features = arrayUtils.map(featureSet, function (result) {
                                var feature = result.feature;
                                var title = feature.attributes["名称"] ? feature.attributes["名称"] : feature.attributes["OBJNAME"];
                                var infoTemplate = new InfoTemplate(title, function () {
                                    return "<div id='feature-info-window'></div>";
                                });
                                feature.setInfoTemplate(infoTemplate);
                                return feature;
                            });
                            if (features.length) {
                                that.map.infoWindow.setFeatures([features[0]]);
                                that.map.infoWindow.show(evt.mapPoint);
                                GraphicUtils.getInstance().addGraphic(JZ.mc.graphics, features[0].geometry, features[0].attributes);
                                var container = new MyTabContainer({
                                    map: that.map,
                                    feature: features[0],
                                    layerID: JZ.currentLayerID,
                                    type: 1
                                }, "feature-info-window");
                                container.startup();
                            }
                        });
                    }
                }
            },
            getLayerIds: function () {
                var that = this;
                arrayUtils.forEach(JZ.layerinfos, function (info) {
                    if (info.ID === JZ.currentLayerID) {
                        that.index = info.IDX;
                    }
                });
            }
        });
        clazz.getInstance = function () {
            if (instance === null) {
                instance = new clazz(JZ.mc);
            }
            return instance;
        };
        return clazz;
    });