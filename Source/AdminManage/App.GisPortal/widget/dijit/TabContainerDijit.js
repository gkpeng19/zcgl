/**
 * @author Administrator
 */
define(['dojo/_base/declare',
    'dojo/dom-class',
    'dojo/dom-style',
    'dojo/_base/array',
    'dojo/_base/lang',
    'dojo/on',
    'dijit/_WidgetBase',
    'dijit/_TemplatedMixin',
	'dstore/Memory',
    'dojo/request/xhr',
	'dstore/Trackable',
	'dgrid/OnDemandGrid',
    'dgrid/Selection',
    'dojox/widget/Dialog',
    'esri/layers/ArcGISDynamicMapServiceLayer',
    'esri/layers/MapImageLayer',
    'esri/layers/MapImage',
    'dojo/text!./template/TabContainerDijit.html'],
function (declare, domClass, domStyle, arrayUtils, lang, on, _WidgetBase, _TemplatedMixin,
    Memory, xhr, Trackable, OnDemandGrid, Selection, Dialog, ArcGISDynamicMapServiceLayer,
    MapImageLayer, MapImage,
    template) {
    var instance = null, clazz;
    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        map: null,
        feature: null,
        index: -1,
        editDialog: null,
        startup: function () {
            this.inherited(arguments);
            this.attachData();
            if (this.feature.geometry.type === "polyline" ||
                this.feature.geometry.type === "point" ||
                this.feature.geometry.type === "multipoint" || 
                !this.feature.attributes["OBJPIC"]) {
                domStyle.set(this.jzFeatureImage, "display", "none");
            }
        },
        createStore: function (cols) {

            var data = [];
            var attributes = this.feature.attributes;
            var i = 0;
            for (var key in attributes) {
                if (!(key === "OBJECTID" || key === "FID") && key != 'SHAPE') {
                    if (attributes[key] &&
                        attributes[key].toString().replace(/(^\s*)|(\s*$)/g, '') != "" &&
                        attributes[key].toString() != 'null' &&
                        attributes[key].toString() != 'Null' &&
                        attributes[key].toString() != 'undefined') {
                        data.push({});
                        data[i]["id"] = i;
                        data[i]["PROPERTYENAME"] = key;//英文名
                        if (key == 'SHAPE.AREA') {
                            data[i]["PROPERTYNAME"] = '面积';//中文名
                        } else if (key == 'SHAPE.LEN') {
                            data[i]["PROPERTYNAME"] = '周长';//中文名
                        } else {
                            data[i]["PROPERTYNAME"] = key;
                        }
                        data[i]["PROPERTYVALUE"] = attributes[key];
                        i = i + 1;
                    }
                }
            }
            if (cols) {
                for (var j = 0; j < cols.length; j++) {
                    var col = cols[j];
                    arrayUtils.forEach(data, function (info) {
                        if (info.PROPERTYENAME === col.COLCODE) {
                            info.PROPERTYNAME = col.COLNAME;
                        }
                    });
                }
            }
            var pattern = new RegExp("^[a-zA-Z_0-9]*$");
            var flag = 0;
            for (var m = 0, length = data.length; m < length; m++) {
                if (pattern.test(data[flag].PROPERTYNAME)) {
                    data.splice(flag, 1);
                } else {
                    flag = flag + 1;
                }
            }
            return data;


        },
        attachData: function () {
            var that = this;
            var layerID = 1;
            arrayUtils.forEach(JZ.typeObj, function (o) {
                if (o.INDEX === that.index) {
                    layerID = o.LAYERID;
                }
            });
            var _lcols = JZ.AllLayerCols.get(that.index);

            if (_lcols) {
                that.createDataGrid(that.createStore(_lcols));
            } else {
                xhr.get("../webservice/WebServiceMap.asmx/GetColsBylayer", {
                    handleAs: "json",
                    timeout: 10000,
                    query: { layerid: layerID }
                }).then(function (suc) {
                    var o = new layercols(that.index, suc);
                    JZ.AllLayerCols.add(o);
                    that.createDataGrid(that.createStore(suc));
                }, function (err) {
                    console.log(err);
                });
            }
        },
        cadLayer: null,
        onShowImage: function () {
            var that = this;
            if (that.feature.attributes && that.feature.attributes["OBJPIC"]) {
                var regex = /^\S+\.(?:png|jpg|bmp|gif)$/i;
                var url = that.feature.attributes["OBJPIC"];
                var flag = regex.test(url);
                if (!flag) {
                    /* 调用GP服务查看dwg */
                    if (that.cadLayer) {
                        that.map.removeLayer(that.cadLayer);
                        that.cadLayer = null;
                    }
                    that.cadLayer = new ArcGISDynamicMapServiceLayer(url, {
                        opacity: 0.8
                    });
                    on(that.cadLayer, "load", function (e) {
                        var layerInfos = e.target.createDynamicLayerInfosFromLayerInfos();
                        var dynamicLayerInfos = [];
                        var index = 0;
                        //把包含子图层的去掉
                        arrayUtils.forEach(layerInfos, function (layerInfo) {
                            if (!layerInfo.subLayerIds) {
                                dynamicLayerInfos[index] = layerInfo;
                                index = index + 1;
                            }
                        });
                        //将面图层放到最下面
                        var count = dynamicLayerInfos.length - 1;
                        for (var i = 0; i <= count; i++) {
                            if (i < count && (dynamicLayerInfos[i].name === 'Polygon' || dynamicLayerInfos[i].name === 'Polygon1')) {
                                var temp = dynamicLayerInfos[i];
                                dynamicLayerInfos[i] = dynamicLayerInfos[count];
                                dynamicLayerInfos[count] = temp;
                            }
                            else if (i < count - 1 && (dynamicLayerInfos[i].name === 'MultiPatch' || dynamicLayerInfos[i].name === 'MultiPatch1')) {
                                var temp1 = dynamicLayerInfos[i];
                                dynamicLayerInfos[i] = dynamicLayerInfos[count - 1];
                                dynamicLayerInfos[count - 1] = temp1;
                            }
                        }
                        e.target.setDynamicLayerInfos(dynamicLayerInfos, true);
                        that.map.setLevel(8);
                    });
                    that.map.addLayers([that.cadLayer]);
                    //that.gp = new Geoprocessor(that.feature.attributes["OBJPIC"]);
                    //that.gp.submitJob({}, lang.hitch(that, that.gpJobComplete), lang.hitch(that, that.gpJobStatus), lang.hitch(that, that.gpJobFailed));
                } else {
                    if (!that.imageLayer) {
                        that.imageLayer = new MapImageLayer({
                            opacity: 0.8
                        });
                        that.map.addLayer(that.imageLayer, 5);
                    }
                    that.imageLayer.removeAllImages();
                    that.map.setLevel(8);
                    var img = new MapImage({
                        'extent': that.feature.geometry.getExtent(),
                        'href': url
                    });
                    that.imageLayer.addImage(img);
                }
            }
        },
        //gpJobComplete: function (jobinfo) {
        //    that.gp.getResultImageLayer(jobinfo.jobId, null, null, function (layer) {
        //        layer.setOpacity(0.7);
        //        on(layer, "load", function (e) {
        //            var layerInfos = e.target.createDynamicLayerInfosFromLayerInfos();
        //            var dynamicLayerInfos = [];
        //            var index = 0;
        //            //把包含子图层的去掉
        //            arrayUtils.forEach(layerInfos, function (layerInfo) {
        //                if (!layerInfo.subLayerIds) {
        //                    dynamicLayerInfos[index] = layerInfo;
        //                    index = index + 1;
        //                }
        //            });
        //            //将面图层放到最下面
        //            var count = dynamicLayerInfos.length - 1;
        //            for (var i = 0; i <= count; i++) {
        //                if (i < count && dynamicLayerInfos[i].name === 'Polygon') {
        //                    var temp = dynamicLayerInfos[i];
        //                    dynamicLayerInfos[i] = dynamicLayerInfos[count];
        //                    dynamicLayerInfos[count] = temp;
        //                }
        //                else if (i < count - 1 && dynamicLayerInfos[i].name === 'MultiPatch') {
        //                    var temp1 = dynamicLayerInfos[i];
        //                    dynamicLayerInfos[i] = dynamicLayerInfos[count - 1];
        //                    dynamicLayerInfos[count - 1] = temp1;
        //                }
        //            }
        //            e.target.setDynamicLayerInfos(dynamicLayerInfos, true);
        //        });
        //        that.map.addLayers([layer]);
        //    });
        //},
        //gpJobStatus: function (jobinfo) {
        //    var jobstatus = '';
        //    switch (jobinfo.jobStatus) {
        //        case 'esriJobSubmitted':
        //            console.log(jobinfo.jobId + ":开始调用GP服务...");
        //            break;
        //        case 'esriJobExecuting':
        //            console.log(jobinfo.jobId + ":GP服务正在运行...");
        //            break;
        //        case 'esriJobSucceeded':
        //            console.log(jobinfo.jobId + ":GP服务运行成功...");
        //            break;
        //    }
        //},
        //gpJobFailed: function (err) {
        //    Dialog.getInstance().startup(null, "发生错误", "调用CAD服务出现错误，请重试！", 2);
        //},
        createDataGrid: function (data) {
            var that = this;
            var store = new (declare([Memory, Trackable]))({
                data: data
            });
            var myGrid = new (declare([OnDemandGrid, Selection]))({
                collection: store,
                columns: {
                    PROPERTYNAME: {
                        label: '属性名'
                    },
                    PROPERTYVALUE: {
                        label: '属性值'
                    }
                }
            }, that.jzFeatureGrid);
            myGrid.startup();
        }
    });
    return clazz;
});
