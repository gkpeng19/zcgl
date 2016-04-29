/**
 * @author Administrator
 */
define(['dojo/_base/declare',
    'dojo/_base/fx',
    'dojo/_base/lang', 'dojo/topic', 'dojo/parser', 'dijit/_WidgetBase', 'dijit/_TemplatedMixin', 'dojo/on', 'dojo/dom', 'dojo/dom-construct',
'dojo/dom-style', 'esri/dijit/Legend',
'dojo/text!./template/Legend.html'],
function (declare, baseFx, lang, topic, parser, _WidgetBase, _TemplatedMixin, on, dom, domConstruct,
    domStyle, Legend, template) {
    var instance = null, clazz;
    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        baseClass: "jz-legend-dialog",
        mixin: false,
        map: null,
        layerInfo: null,
        postCreate: function () {
            this.titleNode.innerHTML = "图例信息";
        },
        startup: function (layerInfo) {
            if (!layerInfo) {
                alert("没有图例信息！");
                return;
            }
            this.layerInfo = layerInfo;
            this.inherited(arguments);
            if (!this.mixin) {
                domConstruct.place(this.domNode, document.body);
                this.showDelay();
                this.mixin = true;
            } else {
                this.onOpen();
            }
        },
        showDelay: function () {
            var that = this;
            baseFx.animateProperty({
                duration: 500,
                node: that.jzLegendDialog,
                properties: {
                    opacity: { start: 0, end: 1 }
                },
                onEnd: function () {
                    if (that.legendDijit) {
                        that.legendDijit.refresh(that.layerInfo);
                    } else {
                        that.legendDijit = new Legend({
                            map: JZ.mc,
                            layerInfos: that.layerInfo
                        }, that.jzLegendDiv);
                        that.legendDijit.startup();
                    }
                }
            }).play();
        },
        hideDelay: function () {
            var that = this;
            baseFx.animateProperty({
                duration: 500,
                node: that.jzLegendDialog,
                properties: {
                    opacity: { start: 1, end: 0 }
                },
                onEnd: function () {
                    domStyle.set(that.domNode, "display", "none");
                }
            }).play();
        },
        onOpen: function () {
            if (domStyle.get(this.domNode, "display") !== "none") {
                return;
            } else {
                domStyle.set(this.domNode, "display", "inline");
                this.showDelay();
            }
        },
        onCancel: function () {
            this.hideDelay();
        },
        onChangePwd: function () {
            console.log(this.jzOldPwd.value);
            console.log(this.jzNewPwd.value);
            console.log(this.jzReNewPwd.value);
            this.hideDelay();
        }
    });

    clazz.getInstance = function () {
        if (instance === null) {
            instance = new clazz({
                map:JZ.mc
            });
        }
        return instance;
    };
    return clazz;
});
