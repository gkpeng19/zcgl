/**
 * @author wyf
 */
define(['dojo/_base/declare', 'dojo/_base/lang',
    'dijit/_WidgetBase',
    'dijit/_TemplatedMixin',
    'dojo/on',
    'dojo/dom',
    'dojo/dom-construct',
    'dojo/_base/array',
    'dojo/topic',
    'dojo/request/xhr',
    'dojo/dom-style',
    'esri/request',
    'dojo/text!./StatAnalyst.html'],
function (declare, lang, _WidgetBase, _TemplatedMixin, on, dom, domConstruct,
    arrayUtils, topic, xhr,
    domStyle, esriRequest, 
    template) {
    var instance = null, clazz;

    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        pNode: null,
        layerIndex: -1,
        startup: function () {
            this.inherited(arguments);
            domConstruct.place(this.domNode, this.pNode);
            topic.subscribe("topic/RefreshSearchList", lang.hitch(this, this.refreshOptions));
        },

        ajaxGet: function (id) {
            var that = this;
            var defer = xhr.get("webservice/WebServiceMap.asmx/GetColsBylayer", {
                handleAs: "json",
                timeout: 50000,
                query: { layerid: id }
            });
            /* 请求成功事件,获取到用户拥有的菜单功能 */
            defer.then(function (suc) {
                /* 为统计分析添加字段描述 */
                var groupOption = "";
                var analystOption = "";
                arrayUtils.forEach(suc, function (info) {
                    /* 分组字段 */
                    if (info.ISGROUP === "1" || info.ISGROUP === 1) {
                        groupOption += "<option value='" + info.COLCODE + "'>" + info.COLNAME + "</option>";
                    }
                    /* 统计字段 */
                    if (info.COLTYPE === "2" || info.COLTYPE === 2) {
                        analystOption += "<option value='" + info.COLCODE + "'>" + info.COLNAME + "</option>";
                    }
                });
                //统计字段
                //this.jzStatAnalystField.innerHTML = groupOption;
                //统计方式
                //that.jzStatAnalystMethod.innerHTML = analystOption;
                JZ.loading.hide();
            }, function (err) {
                JZ.loading.hide();
                console.log("调用获取当前业务图层数据的WebService出现错误，请检查！" + err);
            });
        },

        refreshOptions: function () {
            var that = this;
            JZ.loading.show();
            arrayUtils.forEach(JZ.layersList, function (layer) {
                if (layer.id === JZ.currentLayerID) {
                    that.jzStatAnalystLyr.value = layer.cnName;
                    if (JZ.currentYear !== '') {
                        that.jzStatAnalystYears.value = JZ.currentYear;
                    } else {
                        that.jzStatAnalystYears.value = "所有年份";
                    }
                    that.layerIndex = layer.serverindex;
                    that.ajaxGet(layer.id);
                }
            });
        },

        postCreate: function () {
            //统计区域
            var districtHtml = "";
            arrayUtils.forEach(JZ.district, function (d) {
                districtHtml += "<option value='" + d.ID + "'>" + d.Name + "</option>";
            });
            this.jzStatAnalystArea.innerHTML = districtHtml;
            this.refreshOptions();
        },

        onStart: function () {
            var geometryID = parseInt(this.jzStatAnalystArea.options[this.jzStatAnalystArea.selectedIndex].value);
            var analystFactor = this.jzStatAnalystField.options[this.jzStatAnalystField.selectedIndex].value;
            var analystMethod = this.jzStatAnalystMethod.options[this.jzStatAnalystMethod.selectedIndex].value
            var analystField = "";

            var req = esriRequest({
                url: JZ.config["StatAnalystSOE"][0].url,
                content: {
                    DistrictLayerID: 121, DistrictID: geometryID, LayerID: this.layerIndex, Group: analystFactor,
                    AnalystField: analystField, AnalystMethod: analystMethod, f: 'json'
                },
                handleAs: "json",
                callbackParamName: "callback"
            });
            req.then(lang.hitch(this, this.requestSuccess), lang.hitch(this, this.requestError));
        },
        requestError: function (err) {
            console.log(err);
        },
        requestSuccess: function (suc) {
            var that = this;
            require(['widget/stat/StatResult'], function (clazz) {
                clazz.getInstance().startup(suc, that.jzStatAnalystLyr.value);
            });
        }
    });

    clazz.getInstance = function () {
        if (instance === null) {
            instance = new clazz({
                map: JZ.mc,
                pNode: "StatAnalyst"
            });
        }
        return instance;
    };
    return clazz;

});
