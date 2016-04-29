/**
* @author wangyafei
* 公园风景区管理组
*/
define("widget/district/DistrictExtent",
    ["dojo/_base/declare",
     "dojo/_base/window",
     "dojo/_base/event",
     "dojo/_base/array",
     "dojo/_base/lang",
     "dojo/topic",
     "dojo/on",
     "dojo/dom",
     "dojo/dom-construct",
     "dojo/dom-class",
     "dojo/dom-attr",
     "dojo/query",
     "dojo/request/xhr",
     "dojo/data/ItemFileWriteStore",
     "dijit/TooltipDialog",
     "dijit/form/Button",
     "dijit/popup",
     "dijit/form/DropDownButton",
     "dojox/widget/Dialog",
     "esri/tasks/QueryTask",
     "esri/tasks/query",
     "esri/Color",
     "esri/geometry/Extent",
     "esri/request",
     "esri/graphic",
     "esri/dijit/TimeSlider",
     "esri/TimeExtent",
     "esri/layers/FeatureLayer",
     "esri/layers/TimeInfo"
    ], function (declare, win, Event, arrayUtils, lang, topic, on, dom, domConstruct, domClass,
        domAttr, query, xhr, ItemFileWriteStore, TooltipDialog, Button, Popup, DropDownButton,
        Dialog, QueryTask,
        EsriQuery, Color, Extent, esriRequest, Graphic, TimeSlider, TimeExtent, FeatureLayer, TimeInfo) {
        "use strict"
        var instance = null, clazz;

        function setMapExtent(id) {
            for (var t = 0; t < JZ.district.length; t++) {
                var g = JZ.district[t];
                if (g.ID === id) {
                    JZ.mc.setExtent(new Extent(g.XMin, g.YMin, g.XMax, g.YMax, JZ.mc.spatialReference));
                    break;
                }
            }
        }

        clazz = declare(null, {
            districts: null,
            constructor: function () {
            },
            toggleDistrict: function () {
                if (this.districts.length === 1) {
                    var result = JZ.config["DistrictSOE"][0];
                    this.showDialog(result.url, result.searchfield, result.namefield, result.layerindex);
                } else {
                    console.log("获取的区域个数不等于1，请检查函数返回值是否是一个！");
                }
            },
            showDialog: function (districtUrl, fieldName, nameField, layerindex) {
                var fieldValue = "";
                if (this.districts[0].AREACODE !== "110100") {
                    fieldValue = this.districts[0].AREACODE;
                }
                var req = esriRequest({
                    url: districtUrl,
                    content: { LayerID: layerindex, FieldName: fieldName, FieldValue: fieldValue, f: 'json' },
                    handleAs: "json",
                    callbackParamName: "callback"
                });
                req.then(lang.hitch(this, this.success), lang.hitch(this, this.error));
            },
            success: function (suc) {
                if (suc.geometries.length > 0) {
                    var arr = suc.geometries;
                    if (suc.geometries.length === 1) {
                        JZ.district = arr;
                        this.showDistrict();
                    } else {
                        arr.sort(function (a, b) {
                            var aCode = parseInt(a.CODE);
                            var bCode = parseInt(b.CODE);
                            return aCode > bCode ? 1 : -1;
                        });
                        JZ.district = arr;
                        this.showDistrictToggle();
                    }
                    topic.publish("topic/GetDistrictComplete");
                } else {
                    window.location.href = "Login.aspx?ReturnUrl=" + window.location.href;
                    console.log("获取区县数据失败，请重试！");
                }
            },
            error: function (err) {
                console.log(err);
            },
            show:function(){
                var that = this;
                /* 获取数据库中配置的底图 */
                var defer = xhr.get("webservice/WebServiceMap.asmx/GetArealist", {
                    handleAs: "json",
                    timeout: 10000
                });
                /* 请求成功事件 */
                defer.then(function (suc) {
                    that.districts = suc;
                    that.toggleDistrict();
                }, function (err) {
                    window.location.href = "Login.aspx?ReturnUrl=" + window.location.href;
                    console.log("区县范围获取失败，请重试!" + err);
                });
            },
            showDistrict: function () {
                var that = this;
                var geo = JZ.district[0];
                JZ.ec = geo.CODE.trim();
                dom.byId("toggle-city").innerHTML = geo.Name;
                on(dom.byId("toggle-city"), "click", function (e) {
                    JZ.mc.setExtent(new Extent(geo.XMin, geo.YMin, geo.XMax, geo.YMax, JZ.mc.spatialReference));
                });
            },
            showDistrictToggle: function () {
                var that = this;
                var htmlStr = "<table class='table table-bordered table-condensed'>";
                arrayUtils.forEach(JZ.district, function (ge) {
                    if (ge.CODE === "110100") {
                        htmlStr += "<h5 style='margin-bottom:10px'>请选择列表中的区县:&nbsp;<span style='color:#FF0000;cursor:pointer;' data-dojo-value='" + ge.ID + "'>" + ge.Name + "</span></h5>";
                    }
                });
                var row = ((JZ.district.length - 1) % 4 === 0) ? ((JZ.district.length - 1) / 4) : (parseInt((JZ.district.length - 1) / 4) + 1);
                var index = 0;
                for (var i = 0; i < row; i++) {
                    var start = i * 4 + 1;
                    var end = (i * 4) + 5;
                    if (end >= JZ.district.length) {
                        end = JZ.district.length;
                    }
                    
                    htmlStr += "<tr>"
                    for (var j = start; j < end; j++) {
                        if (JZ.district[j].CODE !== "110100") {
                            htmlStr += "<td><span data-dojo-value='" + JZ.district[j].ID + "'>" + JZ.district[j].Name + "</span></td>"
                        }
                    }
                    htmlStr += "</tr>";
                }
                htmlStr += "</table>";

                that.tooltipDialog = new TooltipDialog({
                    title: "区域切换",
                    content: htmlStr
                });

                on(dom.byId("toggle-city"), "click", function (e) {
                    stopBubble(e);
                    if (that.tooltipDialog) {
                        Popup.open({
                            popup: that.tooltipDialog,
                            around: this,
                            onCancel: function () {
                                Popup.close(that.tooltipDialog);
                            },
                            style: 'left: 70px;'
                        });
                    }
                });
                on(that.tooltipDialog, "click", function (e) {
                    stopBubble(e);
                    if (e.target.tagName === "SPAN") {
                        var node = e.target;
                        var dojoID = domAttr.get(node, "data-dojo-value");
                        setMapExtent(parseInt(dojoID));
                        dom.byId("toggle-city").innerHTML = domAttr.get(node, "innerHTML");
                        Popup.close(that.tooltipDialog);
                    }
                });

                on(document, "click", function (e) {
                    if (that.tooltipDialog) {
                        Popup.close(that.tooltipDialog);
                    }
                });
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