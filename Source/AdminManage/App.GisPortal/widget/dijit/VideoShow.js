/**
 * @author Administrator
 */
define(['dojo/_base/declare', 'dijit/_WidgetBase',
    'dijit/_TemplatedMixin', 'dojo/on', 'dojo/dom',
    'dojo/dom-construct', 'dojo/dom-class', 'dojo/dom-style',
    'dojox/av/FLVideo',
    'dojo/text!./template/VideoShow.html'],
function (declare, _WidgetBase, _TemplatedMixin, on, dom, domConstruct, domClass, domStyle, FLVideo, template) {
    var instance = null;
    var clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        baseClass: "jz-slider-show",
        mixin: false,
        startup: function () {
            this.inherited(arguments);
            if (!this.mixin) {
                domConstruct.place(this.domNode, document.body);
                this.mixin = true;
                this.showVideo();
            } else {
                //domConstruct.empty(this.containerNode);
                //domConstruct.empty(this.jzSliderIndex);
                domStyle.set(this.domNode, 'display', '');
            }
        },
        showVideo: function () {
            var url = "images/upload/1.flv";
            var video = new FLVideo({ initialVolume: .1, mediaUrl: url, autoPlay: true, isDebug: false }, this.containerNode);
        },
        onCloseSlider: function () {
            domStyle.set(this.domNode, 'display', 'none');
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
