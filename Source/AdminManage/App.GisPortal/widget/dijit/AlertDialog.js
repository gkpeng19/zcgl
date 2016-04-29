/**
 * @author Administrator
 */
define(['dojo/_base/declare',
    'dojo/_base/fx',
    'dojo/_base/lang', 'dojo/topic', 'dojo/parser', 'dijit/_WidgetBase', 'dijit/_TemplatedMixin', 'dojo/on', 'dojo/dom', 'dojo/dom-construct',
'dojo/dom-style', 
'dojo/text!./template/AlertDialog.html'],
function (declare, baseFx, lang, topic, parser, _WidgetBase, _TemplatedMixin, on, dom, domConstruct, domStyle, template) {
    var instance = null, clazz;
    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        baseClass: "jz-notify-dialog",
        mixin: false,
        startup: function () {
            this.titleNode.innerHTML = this.title;
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
                node: that.jzNotifyDialogPane,
                properties: {
                    opacity: { start: 0, end: 1 }
                }
            }).play();
        },
        hideDelay: function () {
            var that = this;
            baseFx.animateProperty({
                duration: 500,
                node: that.jzNotifyDialogPane,
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
        }
    });

    clazz.getInstance = function () {
        if (instance === null) {
            instance = new clazz({
                title: "dfa",
            });
        }
        return instance;
    };
    return clazz;

});
