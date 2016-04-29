/**
* @author wangyafei
* 绿色产业类别
*/
define('widget/utils/SearchingUtil',
    ['dojo/_base/declare', 'dojo/_base/lang', 'dojo/_base/array', 'esri/tasks/FindTask', 'esri/tasks/FindParameters'],
    function (declare, lang, arrayUtils, FindTask, FindParameters) {
        "use strict"
        var instance = null, clazz;
        clazz = declare(null, {
            results: [],
            constructor: function () {
            },
            search: function (text) {
                var that = this;
                that.results = [];
                //此处可以查询图层信息，图层里面的要素信息
                arrayUtils.forEach(JZ.privilegeLayers, function (clsInfo) {
                    var layerCls = clsInfo.LAYERS;
                    arrayUtils.forEach(layerCls, function (layer) {
                        if (layer.cnName.indexOf(text) != -1 ||
                           layer.enName.indexOf(text) != -1) {
                            layer['PENAME'] = clsInfo.ENAME ? "" : clsInfo.ENAME;
                            layer['PID'] = clsInfo.ID;
                            layer['PNAME'] = clsInfo.NAME;
                            that.results.push(layer);
                        }
                    });
                });
                //继续查询所有图层中的要素信息
                that.searchFeatures(text);
            },
            searchFeatures: function (text) {
                var ids = [];
                var url = "";
                var added = false;
                arrayUtils.forEach(JZ.privilegeLayers, function (clsInfo) {
                    var layerCls = clsInfo.LAYERS;
                    arrayUtils.forEach(layerCls, function (layer) {
                        if (layer.serverindex == -1) {
                            console.log(layer);
                        }
                        ids.push(layer.serverindex);
                        if (!added) {
                            url = layer.mapServerUrl ? layer.mapServerUrl : JZ.config["SearchingUrl"][0].url;
                            added = true;
                        }
                    });
                });

                var findTask = new esri.tasks.FindTask(url);
                var findParams = new esri.tasks.FindParameters();
                findParams.returnGeometry = true;
                findParams.layerIds = ids;
                findParams.searchFields = ["MC", "OBJCODE", "OBJNAME"];
                findParams.searchText = text;
                JZ.loading.show();
                findTask.execute(findParams, lang.hitch(this, this.findResults), lang.hitch(this, this.findError));
            },
            findResults: function (featureSet) {
                var that = this;
                require(['widget/utils/PagingSearchResult'], function (clazz) {
                    //将查询结果合并
                    that.results = that.results.concat(featureSet);
                    clazz.getInstance().paging(that.results);
                    JZ.loading.hide();
                });
            },
            findError: function () {
                JZ.loading.hide();
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