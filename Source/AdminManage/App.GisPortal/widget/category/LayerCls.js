/**
* @author wangyafei
* 绿色产业类别
*/
define('widget/category/LayerCls',
    ['dojo/_base/declare',
     'dojo/request/xhr',
     'dojo/dom',
     'dojo/on',
     'dojo/topic',
     'dojo/dom-attr',
     'dojo/dom-class',
     'dojo/dom-construct',
     'dojo/query',
     'dojo/_base/lang',
     'dojo/_base/array',
     'widget/category/LayerOper'
    ], function (declare, xhr, dom, on, topic, domAttr, domClass, domConstruct, query, lang, arrayUtils, LayerOper) {
        "use strict"
        var instance = null, clazz;
        var deferred = null;
        clazz = declare(null, {
            featureLayers: [],
            pid: -1,//父类ID
            constructor: function () {
                on(dom.byId("btnChooseAll"), "click", lang.hitch(this, this.selectAll));
                on(dom.byId("btnChooseNone"), "click", lang.hitch(this, this.selectNone));
            },
            selectAll: function () {
                var that = this;
                var arr = [];
                var nodeList = query("#ulLayersList li input[type='checkbox']");
                arrayUtils.forEach(nodeList, function (node) {
                    domAttr.set(node, "checked", true);
                    var layerID = domAttr.get(node, "data-dojo-id");
                    arrayUtils.forEach(JZ.layersList, function (layer, i) {
                        arr.push({ ID: layer.id, IDX: layer.serverindex });
                    });
                });
                JZ.layerinfos = arr;
                LayerOper.getInstance().refreshLayer();

                topic.publish('topic/layer-legend-refresh');
            },
            selectNone: function () {
                var that = this;
                var nodeList = query("#ulLayersList li input[type='checkbox']");
                JZ.layerinfos = [];
                arrayUtils.forEach(nodeList, function (node) {
                    if (!domClass.contains(node.nextSibling, "layer-active")) {
                        domAttr.set(node, "checked", false);
                    } else {
                        var layerID = domAttr.get(node, "data-dojo-id");
                        arrayUtils.forEach(JZ.layersList, function (layer, i) {
                            if (layer.id == parseInt(layerID, 10)) {
                                JZ.layerinfos.push({ ID: layer.id, IDX: layer.serverindex });
                            }
                        });
                    }
                });
                LayerOper.getInstance().refreshLayer();

                topic.publish('topic/layer-legend-refresh');
            },
            ajaxGet: function (refclassid) {
                var that = this;
                that.pid = parseInt(refclassid, 10);
                arrayUtils.forEach(JZ.privilegeLayers, function (info) {
                    if (info.ID === that.pid) {
                        JZ.layerinfos = [];
                        JZ.layersList = info.LAYERS;
                        that.setTitle();
                        that.delayShow(info.LAYERS);
                    }
                });
            },
            setTitle: function () {
                dom.byId("sel-layers-num").innerHTML = JZ.layersList.length;
            },
            clearFeatureLayers: function () {
                if (JZ.featureLayers && JZ.featureLayers.length > 0) {
                    arrayUtils.forEach(JZ.featureLayers, function (layer) {
                        JZ.mc.removeLayer(layer.Layer);
                    });
                    JZ.featureLayers = [];
                }
            },
            delayShow: function (layersList) {
                //清除已经添加到地图中的要素图层
                this.clearFeatureLayers();
                /* 动态将图层信息添加到页面中 */
                var layerParentNode = dom.byId("ulLayersList");
                var htmlStr = "";
                var index = 0;
                var that = this;
                arrayUtils.forEach(layersList, function (layer) {
                    if (index === 0) {
                        htmlStr += "<li><input data-dojo-parent='" + that.pid + "' id='custom-layer-" + layer.id + "' data-dojo-id='" + layer.id + "' type='checkbox' checked/><label data-dojo-parent='" + that.pid + "' for='custom-layer-" + layer.id + "' data-dojo-id='" + layer.id + "' class='layer-active'>" + layer.cnName + "</label></li>";
                        LayerOper.getInstance().operate(layer.id, 0);
                    } else {
                        htmlStr += "<li><input data-dojo-parent='" + that.pid + "' id='custom-layer-" + layer.id + "' data-dojo-id='" + layer.id + "' type='checkbox'/><label data-dojo-parent='" + that.pid + "' for='custom-layer-" + layer.id + "' data-dojo-id='" + layer.id + "'>" + layer.cnName + "</label></li>";
                    }
                    index = index + 1;
                });
                domConstruct.empty(layerParentNode);
                if (htmlStr !== "") {
                    domConstruct.place(htmlStr, layerParentNode);
                    // 为添加的li标签添加click处理事件
                    query("#ulLayersList li label").on("click", lang.hitch(this, this.attachEvent));
                    query("#ulLayersList li input").on("click", lang.hitch(this, this.mulFeatureLayer));
                }
            },
            mulFeatureLayer: function (evt) {
                if (evt) {
                    var node = evt.currentTarget;
                    var slibNode = node.nextSibling;
                    var layerID = domAttr.get(node, "data-dojo-id");
                    var parentID = domAttr.get(node, "data-dojo-parent");
                    if (!domClass.contains(slibNode, "layer-active")) {
                        if (!node.checked) {
                            this.removeLayerID(layerID);
                        } else {
                            LayerOper.getInstance().operate(layerID, 1);
                        }
                    } else {
                        node.checked = true;
                    }
                }
            },
            removeLayerID: function (layerID) {
                var that = this;
                var arr = [];
                arrayUtils.forEach(JZ.layerinfos, function (layer, i) {
                    if (layer.ID !== parseInt(layerID)) {
                        arr.push({ID:layer.ID, IDX:layer.IDX});
                    }
                });
                JZ.layerinfos = arr;
                LayerOper.getInstance().refreshLayer();
            },
            addLayer: function (node) {
                var layerID = domAttr.get(node, "data-dojo-id");
                var parentID = domAttr.get(node, "data-dojo-parent");
                if (!domClass.contains(node, "layer-active")) {
                    query("#ulLayersList li label").removeClass("layer-active");
                    domClass.add(node, "layer-active");
                }
                LayerOper.getInstance().operate(layerID, 0);
            },
            attachEvent: function (evt) {
                var node = evt.currentTarget;
                this.addLayer(node);
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







