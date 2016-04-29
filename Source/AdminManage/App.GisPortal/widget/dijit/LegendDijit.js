
/**
 * @author Administrator
 */
define(['dojo/_base/declare', 
    'dojo/_base/fx', 'dojo/_base/array',
    'dojo/_base/lang', 'dojo/topic', 'dojo/parser', 'dijit/_WidgetBase',
    'dijit/_TemplatedMixin', 'dojo/on', 'dojo/dom', 'dojo/dom-construct', 'dojo/dom-class',
'dojo/dom-style', 'esri/dijit/Legend',
'dojo/text!./template/LegendDijit.html'],
function (declare, baseFx, arrayUtils, lang, topic, parser, _WidgetBase, _TemplatedMixin, on, dom, domConstruct,
    domClass, domStyle, Legend, template) {
    var instance = null;
    var clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        baseClass: "jz-legend-dialog clearfix",
        legendDijit: null,
        layerInfo: null,
        map: null,
        startup: function () {
            domConstruct.place(this.domNode, document.body);
            topic.subscribe('topic/layer-legend-refresh', lang.hitch(this, this.refreshLegend));
            this.own(on(this.map, "layers-add-result", lang.hitch(this, this.getLayerInfo)));
        },
        getLayerInfo: function () {
            this.layerInfo = arrayUtils.map(evt.layers, function (layer, index) {
                return { layer: layer.layer, title: "当前地图图例" };
            });
        },
        setCNName: function () {
            arrayUtils.forEach(JZ.layersList, function (layer) {
                arrayUtils.forEach(JZ.dynamicLayer.layerInfos, function (layerInfo) {
                    if (layer.serverindex == layerInfo.id) {
                        layerInfo.name = layer.cnName;
                    }
                });
            });
        },
        refreshLegend: function () {
            var that = this;
            if (JZ.dynamicLayer && JZ.dynamicLayer.layerInfos && JZ.dynamicLayer.layerInfos.length > 0) {
                if (that.interval) {
                    clearInterval(that.interval);
                }
                that.setCNName();
                if (that.legendDijit) {
                    that.legendDijit.refresh();
                } else {
                    that.legendDijit = new Legend({
                        map: that.map,
                        layerInfos: [
                            {
                                layer: JZ.dynamicLayer,
                                title: '业务图层'
                            }
                        ]
                    }, that.containerNode);
                    that.legendDijit.startup();
                }
            } else {
                if (that.interval) {
                    clearInterval(that.interval);
                }
                that.interval = setInterval(function () {
                    that.refreshLegend();
                }, 100);
            }
        },
        smallDelay: function () {
            var that = this;
            baseFx.animateProperty({
                duration: 500,
                node: that.jzLegendDialogPane.parentNode,
                properties: {
                    height: { start: 250, end: 30 }
                },
                onEnd: function () {
                    domStyle.set(that.jzMinimalNode, 'display', 'none');
                    domStyle.set(that.jzMaxNode, 'display', '');
                }
            }).play();
        },
        largeDelay: function () {
            var that = this;
            baseFx.animateProperty({
                duration: 500,
                node: that.jzLegendDialogPane.parentNode,
                properties: {
                    height: { start: 30, end: 250 }
                },
                onEnd: function () {
                    domStyle.set(that.jzMinimalNode, 'display', '');
                    domStyle.set(that.jzMaxNode, 'display', 'none');
                }
            }).play();
        },
        onMinimal: function () {
            this.smallDelay();
        },
        onMaximum: function () {
            this.largeDelay();
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


