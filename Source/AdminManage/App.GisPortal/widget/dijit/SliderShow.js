/**
 * @author Administrator
 */
define(['dojo/_base/declare', 'dijit/_WidgetBase', 'dojo/_base/array',
    'dojo/request/xhr',
    'dijit/_TemplatedMixin', 'dojo/on', 'dojo/dom', 'dojo/dom-geometry',
    'dojo/dom-construct', 'dojo/dom-class', 'dojo/dom-style',
    'dojo/text!./template/SliderShow.html'],
function (declare, _WidgetBase, arrayUtils, xhr, _TemplatedMixin, on, dom, domGeometry, domConstruct, domClass, domStyle, template) {
    var instance = null;
    var clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        baseClass: "jz-slider-show",
        mixin: false,
        pics: [],
        startup: function (suc) {
            this.pics = suc;
            this.inherited(arguments);
            if (!this.mixin) {
                domConstruct.place(this.domNode, document.body);
                this.mixin = true;
            } else {
                domConstruct.empty(this.containerNode);
                domConstruct.empty(this.jzSliderIndex);
                domStyle.set(this.domNode, 'display', '');
            }
            this.createSlider();
        },
        createSlider: function () {
            var that = this;
            var margin = domGeometry.getMarginBox(that.containerNode);
            var ul = domConstruct.create("ul", null, that.containerNode);
            var ulIndex = domConstruct.create("ul", { style: 'width:' + that.pics.length * 50 + 'px' }, that.jzSliderIndex);
            arrayUtils.forEach(that.pics, function (item, idx) {
                var li = domConstruct.create("li", null, ul);
                var liIndex = domConstruct.create("li", { innerHTML: idx + 1}, ulIndex);
                var img = domConstruct.create('img', { src: item.PICURL, height: margin.h, width: margin.w }, li);
            });
            var lis = that.jzSliderIndex.getElementsByTagName('li');
            for (var i = 0; i < lis.length; i++) {
                lis[i].index = i;
                lis[i].onclick = function () {
                    that.index = this.index;
                    setTimeout(function () {
                        domStyle.set(ul, 'marginTop', -(that.index * margin.h) + 'px');
                    }, 30);
                }
            }
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
