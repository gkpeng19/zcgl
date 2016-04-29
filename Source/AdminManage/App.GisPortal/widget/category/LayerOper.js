/**
* @author wangyafei
* 对于图层的操作
*/
define('widget/category/LayerOper',
    ['dojo/_base/declare',
     'dojo/dom',
     'dojo/on',
     'dojo/topic',
     'dojo/dom-class',
     'dojo/dom-style',
     'dojo/dom-construct',
     'dojo/query',
     'dojo/_base/lang',
     'dojo/_base/array',
     'esri/request',
     'esri/tasks/QueryTask',
     'esri/tasks/query',
     'esri/geometry/Point',
     'esri/geometry/Polyline',
     'esri/geometry/Polygon',
     'esri/layers/ArcGISDynamicMapServiceLayer',
     'widget/utils/PagingUtils',
     'widget/dijit/LegendDijit'
    ], function (declare, dom, on, topic, domClass, domStyle, domConstruct, query, lang,
        arrayUtils, esriRequest, QueryTask, EsriQuery, Point, Polyline, Polygon,
        ArcGISDynamicMapServiceLayer, PagingUtils, LegendDijit) {
        "use strict"
        var instance = null, clazz;

        clazz = declare(null, {
            constructor: function () {
                topic.subscribe("topic/RefreshSearchList", lang.hitch(this, this.refreshDataList));
                LegendDijit.getInstance(JZ.mc).startup();
            },
            refreshDataList: function (text) {
                var that = this;
                arrayUtils.forEach(JZ.layersList, function (layer) {
                    if (layer.id === JZ.currentLayerID) {
                        JZ.loading.show();
                        var featureUrl = layer.mapServerUrl + "/" + layer.serverindex;
                        var queryTask = new QueryTask(featureUrl);
                        var esriQuery = new EsriQuery();
                        var qwhere = "";
                        if (JZ.currentYear && JZ.currentYear !== '') {
                            qwhere += " OBJYEAR ='" + JZ.currentYear + "' ";
                        }
                        if (text) {
                            if (qwhere) {
                                qwhere += " and ";
                            }
                            qwhere += " (( OBJNAME like '%" + text + "%') or (OBJCODE like '%" + text + "%')) ";
                        }
                        if (JZ.district.length === 1) {
                            if (qwhere) {
                                qwhere += " and ";
                            }
                            qwhere += " OBJQXCODE='" + JZ.district[0].CODE + "' ";
                        } else {
                            if (qwhere) {
                                qwhere += " and ";
                            }
                            qwhere += " 1 like 1 ";
                        }
                        esriQuery.where = qwhere;
                        esriQuery.returnGeometry = true;
                        esriQuery.outFields = ["*"];
                        queryTask.executeForIds(esriQuery, lang.hitch(that, that.successQuery), lang.hitch(that, that.errorQuery));
                    }
                });
            },

            showFeatureInfo: function (evt) {
                //要素图层点击事件
                JZ.mc.graphics.clear();
                require(['widget/utils/GraphicUtils'], function (GraphicUtils) {
                    GraphicUtils.getInstance().addGraphic(JZ.mc.graphics, evt.graphic.geometry, evt.graphic.attributes);
                });
            },

            refreshLayer: function () {
                var indexes = [];
                arrayUtils.forEach(JZ.layerinfos, function (info) {
                    indexes.push(info.IDX);
                });
                if (indexes.length > 0) {
                    JZ.dynamicLayer.setVisibleLayers(indexes);
                    if (JZ.district.length === 1) {
                        var layerDefinitions = [];
                        arrayUtils.forEach(indexes, function (idx) {
                            layerDefinitions[idx] = "OBJQXCODE='" + JZ.district[0].CODE + "'";
                        });
                        JZ.dynamicLayer.setLayerDefinitions(layerDefinitions);
                    }
                } else {
                    indexes.push(-1);
                    JZ.dynamicLayer.setVisibleLayers(indexes);
                }
                //var indexes = [];
                //arrayUtils.forEach(JZ.layerinfos, function (info) {
                //    indexes.push(info.IDX);
                //});
                //if (JZ.layerinfos.length > 0) {
                //    var layerDefinitions = [];
                //    if (JZ.district.length === 1) {
                //        arrayUtils.forEach(JZ.layerinfos, function (info) {
                //            if (info.ID == JZ.currentLayerID) {
                //                layerDefinitions[info.IDX] = "OBJYEAR='" + JZ.currentYear + "' and OBJQXCODE='" + JZ.district[0].CODE + "'";
                //            } else {
                //                layerDefinitions[info.IDX] = "OBJQXCODE='" + JZ.district[0].CODE + "'";
                //            }
                //        });
                //        JZ.dynamicLayer.setLayerDefinitions(layerDefinitions);
                //    } else {
                //        arrayUtils.forEach(JZ.layerinfos, function (info) {
                //            if (info.ID == JZ.currentLayerID) {
                //                layerDefinitions[info.IDX] = "OBJYEAR='" + JZ.currentYear + "'";
                //            }
                //        });
                //        JZ.dynamicLayer.setLayerDefinitions(layerDefinitions);
                //    }
                //    JZ.dynamicLayer.setVisibleLayers(indexes);
                //} else {
                //    indexes.push(-1);
                //    JZ.dynamicLayer.setVisibleLayers(indexes);
                //}
            },

            checkExists: function (layerid) {
                var isExist = false;
                arrayUtils.forEach(JZ.layerinfos, function (info) {
                    if (info.ID === layerid) {
                        isExist = true;
                    }
                });
                return isExist;
            },

            addDataYear: function (layer) {
                JZ.currentYear = '';
                if (layer.datayears && layer.datayears != "") {
                    var years = layer.datayears.split(',');
                    var pNode = dom.byId("ulYearsList");
                    domStyle.set(dom.byId("yearOrCategory"), "display", "");
                    domConstruct.empty(pNode);
                    //对年份进行由小到大的排序
                    years = years.sort(function (a, b) {
                        return b - a;
                    });
                    var html = "";
                    arrayUtils.forEach(years, function (year, idx) {
                        if (idx == 0) {
                            html += "<li><span class='selected'>" + year + "</span></li>";
                            JZ.currentYear = year;
                        } else {
                            html += "<li><span>" + year + "</span></li>";
                        }
                    });
                    domConstruct.place(html, pNode);
                    query("#ulYearsList li").on("click", function (e) {
                        var year = this.children[0].innerHTML;
                        query("#ulYearsList li span").removeClass("selected");
                        domClass.add(this.children[0], "selected");
                        JZ.currentYear = year;
                        topic.publish("topic/RefreshSearchList");
                    });
                } else {
                    var pNode = dom.byId("ulYearsList");
                    domConstruct.empty(pNode);
                    domStyle.set(dom.byId("yearOrCategory"), "display", "none");
                }
            },

            operate: function (layerID, flag) {
                var that = this;
                if (JZ.imageLayer) {
                    JZ.imageLayer.removeAllImages();
                }
                //根据当前图层，显示年份，不按年显示，则为空字符串
                arrayUtils.forEach(JZ.layersList, function (layer) {
                    if (layer.id === parseInt(layerID)) {
                        //保存当前图层的ID
                        if (JZ.dynamicLayer) {
                            if (!that.checkExists(layer.id)) {
                                JZ.layerinfos.push({ ID: layer.id, IDX: layer.serverindex });
                            }
                            that.refreshLayer();
                        } else {
                            JZ.dynamicLayer = new ArcGISDynamicMapServiceLayer(layer.mapServerUrl);
                            JZ.dynamicLayer.setVisibleLayers([layer.serverindex]);
                            if (JZ.district.length === 1) {
                                var layerDefinitions = [];
                                layerDefinitions[layer.serverindex] = "OBJQXCODE='" + JZ.district[0].CODE + "'";
                                JZ.dynamicLayer.setLayerDefinitions(layerDefinitions);
                            }
                            JZ.mc.addLayer(JZ.dynamicLayer);
                            if (!that.checkExists(layer.id)) {
                                JZ.layerinfos.push({ ID: layer.id, IDX: layer.serverindex });
                            }
                        }
                        if (flag === 0) {
                            JZ.currentLayerID = layer.id;
                            that.addDataYear(layer);
                            topic.publish("topic/RefreshSearchList");
                        }
                    }
                });

                topic.publish('topic/layer-legend-refresh');
            },

            successQuery: function (suc) {
                if (JZ.mc && JZ.mc.infoWindow) {
                    JZ.mc.infoWindow.hide();
                }
                //alert("共有" + suc.length + "条数据！");
                PagingUtils.getInstance().paging(suc);
                JZ.loading.hide();
            },

            errorQuery: function (err) {
                console.log(err);
                JZ.loading.hide();
            }
        });

        clazz.getInstance = function () {
            if (instance === null) {
                instance = new clazz();
            }
            return instance;
        };
        return clazz;
    });
