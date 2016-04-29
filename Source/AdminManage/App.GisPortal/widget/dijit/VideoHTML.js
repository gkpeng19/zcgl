/**
 * @author Administrator
 */
define(['dojo/_base/declare', 'dijit/_WidgetBase',
    'dijit/_TemplatedMixin', 'dojo/on', 'dojo/dom',
    'dojo/dom-construct', 'dojo/dom-class', 'dojo/dom-style',
    'dojo/text!./template/VideoHTML.html'],
function (declare, _WidgetBase, _TemplatedMixin, on, dom, domConstruct, domClass, domStyle, template) {
    var instance = null;
    var clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        baseClass: "jz-slider-show",
        mixin: false,
        startup: function (url) {
            this.inherited(arguments);
            if (!this.mixin) {
                domConstruct.place(this.domNode, document.body);
                this.mixin = true;
            } else {
                domStyle.set(this.domNode, 'display', '');
            }
            this.showVideo(url);
        },
        showVideo: function (url) {
            var iframe = domConstruct.create('iframe', {
                src: 'pages/video.html', width: '100%', height: '100%',
                frameborder: 'no', border: '0', marginwidth: '0',
                marginheight: '0', scrolling: 'no', allowtransparency: 'yes'
            }, this.containerNode);

            if (iframe.attachEvent) {
                iframe.attachEvent("onload", function () {
                    iframe.contentWindow.showVideo("../" + url);
                });
            } else {
                iframe.onload = function () {
                    iframe.contentWindow.showVideo("../" + url);
                };
            }
        },
        onCloseSlider: function () {
            domStyle.set(this.domNode, 'display', 'none');
            domConstruct.empty(this.containerNode);
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
