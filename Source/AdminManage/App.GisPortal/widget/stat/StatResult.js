/**
 * @author Administrator
 */
define(['dojo/_base/declare',
    'dojo/_base/fx',
    'dojo/_base/lang',
    'dojo/topic',
    'dojo/parser',
    'dijit/_WidgetBase',
    'dijit/_TemplatedMixin',
    'dojo/on',
    'dojo/dom',
    'dojo/dom-construct',
    'dojo/dom-style',
    'dojox/charting/Chart',
    'dojox/charting/DataChart',
    'dojox/charting/action2d/Highlight',
    'dojox/charting/action2d/MoveSlice',
    'dojox/charting/action2d/Tooltip',
    'dojox/charting/action2d/Shake',
    'dojox/charting/action2d/Magnify',
    'dojox/charting/themes/ThreeD',
    'dojox/charting/plot2d/Columns',
    'dojo/data/ItemFileWriteStore',
    'dojo/text!./StatResult.html'],
function (declare, baseFx, lang, topic, parser, _WidgetBase, _TemplatedMixin, on, dom, domConstruct,
    domStyle, Chart, DataChart, Highlight, MoveSlice, Tooltip, Shake, Magnify,
    ThreeD, Columns, ItemFileWriteStore, template) {
    var instance = null, clazz;
    function _getLabels(arrData, value) {
        value = parseInt(value) - 1;
        if (arrData.length < 1 || arrData.length <= value || parseInt(value) != value) {
            return '无数据';
        } else {
            return arrData[value].DistrictName.toString();
        }
    }

    function _valTrans(arrData, yName) {
        var series = [];
        for (var obj in arrData) {
            var tmpItem = {};
            if (arrData[obj][yName] !== undefined) {
                tmpItem.y = parseFloat(arrData[obj][yName]);
                tmpItem.tooltip = arrData[obj]['DistrictName'] + ':' + arrData[obj][yName];
            } else {
                console.warn('series:', yName, '值不存在！');
            }

            series.push(tmpItem);
        };
        return series;
    }
    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        baseClass: "jz-stat-dialog",
        mixin: false,
        postCreate: function () {

        },
        startup: function (suc, title) {
            this.inherited(arguments);
            if (!this.mixin) {
                domConstruct.place(this.domNode, document.body);
                this.showDelay(suc);
                this.mixin = true;
            } else {
                this.onOpen(suc);
            }
            this.titleNode.innerHTML = title + "图层统计结果";
        },
        showDelay: function (suc) {
            var that = this;
            baseFx.animateProperty({
                duration: 500,
                node: that.jzStatDialogPane,
                properties: {
                    opacity: { start: 0, end: 1 }
                }
            }).play();
            that.createGraphic(suc);
        },
        createGraphic: function (suc) {
            var that = this;
            if (that.chartC) {
                that.jzStatGraResult.innerHTML = "";
                that.chartC = null;
            }
            var store = new ItemFileWriteStore({ data: suc });
            that.chartC = new Chart(that.jzStatGraResult, {
                margins: { l: 10, r: 10, t: 10, b: 10 }
            });
            that.chartC.setTheme(ThreeD);
            that.chartC.addAxis("x", {
                natural: true,
                labelFunc: function (value) {
                    var label = _getLabels(suc.items, value);
                    return label;
                },
                dropLabels: false,
                rotation: 40
            });
            that.chartC.addAxis("y", { vertical: true, includeZero: true });
            that.chartC.addPlot("default", { type: Columns, gap: 5 });
            that.chartC.addSeries("value", _valTrans(suc.items, "Nums"));
            new MoveSlice(that.chartC, "default");
            new Highlight(that.chartC, "default", { highlight: "#031F0A" });
            new Tooltip(that.chartC);
            new Magnify(that.chartC, "default");
            new Shake(that.chartC, "default");
            that.chartC.render();
        },
        hideDelay: function () {
            var that = this;
            baseFx.animateProperty({
                duration: 500,
                node: that.jzStatDialogPane,
                properties: {
                    opacity: { start: 1, end: 0 }
                },
                onEnd: function () {
                    domStyle.set(that.domNode, "display", "none");
                }
            }).play();
        },
        onOpen: function (suc) {
            if (domStyle.get(this.domNode, "display") !== "none") {
                return;
            } else {
                domStyle.set(this.domNode, "display", "inline");
                this.showDelay(suc);
            }
        },
        onCancel: function () {
            this.hideDelay();
        },
        onChangePwd: function () {
            this.hideDelay();
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
