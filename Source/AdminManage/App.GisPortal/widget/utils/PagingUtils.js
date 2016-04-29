/**
* @author wangyafei
* 绿色产业类别
*/
define('widget/utils/PagingUtils',
    ['dojo/_base/declare',
     'dojo/_base/array',
     'dojo/_base/lang',
     'dojo/dom',
     'dojo/dom-class',
     'dojo/dom-attr',
     'dojo/dom-construct',
     'dojo/query',
     'esri/tasks/QueryTask',
     'esri/tasks/query',
    ], function (declare, arrayUtils, lang, dom, domClass, domAttr, domConstruct, query, QueryTask, EsriQuery) {
        "use strict"
        var instance = null, clazz;
        clazz = declare(null, {
            _map: null,
            featureIDS: null,
            constructor: function (map) {
                this._map = map;
            },
            pagingSmall: function () {
                if (query("#pagination-list p.paging").length > 0) {
                    domConstruct.destroy(query("#pagination-list p.paging")[0]);
                }
                var index = this.pageIndex;
                var htmlStr = "<p class='paging'>";
                for (var i = 1; i <= this.pagesCount; i++) {
                    if (i === index) {
                        htmlStr += "<span class='single selected' data-dojo-value='" + i + "'>" + i + "</span>"
                    } else {
                        htmlStr += "<span class='single' data-dojo-value='" + i + "'>" + i + "</span>"
                    }
                }
                htmlStr += "</p>";
                domConstruct.place(htmlStr, dom.byId("pagination-list"), "last");
                query("#pagination-list p span").on("click", lang.hitch(this, this.recalcSmall));
            },
            recalcSmall: function (e) {
                var node = e.target;
                var dataValue = parseInt(domAttr.get(node, "data-dojo-value"));
                if (this.pageIndex !== dataValue) {
                    query("#pagination-list p span").removeClass("selected");
                    domClass.add(node, "selected");
                    this.pageIndex = dataValue;
                    //计算当前页需要加载的内容
                    this.loadGeometry();
                }
            },
            loadGeometry: function () {
                var nodes = query("#search-result-list li");
                if (nodes.length > 0) {
                    arrayUtils.forEach(nodes, function (node) {
                        domConstruct.destroy(node);
                    });
                }
                var index = this.pageIndex;
                var start = 7 * (index - 1);
                var end = start + 7;
                this.getGeometries(start, end);
            },
            // 分页
            pagingBig: function () {
                var that = this;
                // 当前页索引
                var index = that.pageIndex;
                var htmlStr = "<p class='paging'><span data-dojo-value='first' class='colored'>首页</span><span data-dojo-value='prev' class='colored'>上一页</span>";
                for (var i = that.pageStart; i <= that.pageEnd; i++) {
                    if (i < 9) {
                        if (i === that.pageIndex) {
                            htmlStr += "<span class='single selected' data-dojo-value='" + i + "'>" + i + "</span>";
                        } else {
                            htmlStr += "<span class='single' data-dojo-value='" + i + "'>" + i + "</span>";
                        }
                    } else if (i < 99) {
                        if (i === that.pageIndex) {
                            htmlStr += "<span class='double selected' data-dojo-value='" + i + "'>" + i + "</span>";
                        } else {
                            htmlStr += "<span class='double' data-dojo-value='" + i + "'>" + i + "</span>";
                        }
                    } else{
                        if (i === that.pageIndex) {
                            htmlStr += "<span class='selected' data-dojo-value='" + i + "'>" + i + "</span>";
                        } else {
                            htmlStr += "<span class='' data-dojo-value='" + i + "'>" + i + "</span>";
                        }
                    }
                }
                if (query("#pagination-list p").length > 0) {
                    domConstruct.empty(dom.byId("pagination-list"));
                }
                htmlStr += "<span data-dojo-value='next' class='colored'>下一页</span><span data-dojo-value='last' class='colored'>尾页</span></p>";
                domConstruct.place(htmlStr, dom.byId("pagination-list"), "last");
                query("#pagination-list p span").on("click", lang.hitch(that, that.recalc));
            },
            recalc: function (e) {
                var node = e.target;
                var dataValue = domAttr.get(node, "data-dojo-value");
                if (dataValue === "first") {
                    this.firstPage();
                } else if (dataValue === "prev") {
                    this.prevPage();
                } else if (dataValue === "next") {
                    this.nextPage();
                } else if (dataValue === "last") {
                    this.lastPage();
                } else {
                    this.pageIndex = parseInt(dataValue);
                    this.pagingBig();
                }
                this.loadGeometry();
            },
            nextPage: function () {
                if (this.pageIndex !== this.pagesCount) {
                    this.pageIndex += 1;
                    if (this.pageIndex > 998 && this.pageIndex <= 9997) {
                        this.pageStart = this.pageIndex;
                        this.pageEnd = (this.pageIndex + 3 < this.pagesCount) ? (this.pageIndex + 3) : this.pagesCount;
                    } else if (this.pageIndex > 9997) {
                        this.pageStart = this.pageIndex;
                        this.pageEnd = (this.pageIndex + 2 < this.pagesCount) ? (this.pageIndex + 2) : this.pagesCount;
                    } else {
                        this.pageStart = (this.pageIndex - 2 >= 1) ? (this.pageIndex - 2) : 1;
                        this.pageEnd = (this.pageIndex + 2 < this.pagesCount) ? (this.pageIndex + 2) : this.pagesCount;
                        this.setValue();
                    }
                    this.pagingBig();
                }
            },
            prevPage: function () {
                if (this.pageIndex > 1) {
                    this.pageIndex -= 1;
                    if (this.pageIndex > 998 && this.pageIndex <= 9997) {
                        this.pageStart = this.pageIndex;
                        this.pageEnd = (this.pageIndex + 3 < this.pagesCount) ? (this.pageIndex + 3) : this.pagesCount;
                    } else if (this.pageIndex > 9997) {
                        this.pageStart = this.pageIndex;
                        this.pageEnd = (this.pageIndex + 2 < this.pagesCount) ? (this.pageIndex + 2) : this.pagesCount;
                    } else {
                        this.pageStart = (this.pageIndex - 2 >= 1) ? (this.pageIndex - 2) : 1;
                        this.pageEnd = (this.pageIndex + 2 < this.pagesCount) ? (this.pageIndex + 2) : this.pagesCount;
                        this.setValue();
                    }
                    this.pagingBig();
                }
            },
            setValue: function () {
                if (this.pageStart === 1) {
                    this.pageEnd = 5;
                } else if (this.pageStart === 2) {
                    this.pageEnd = 6;
                }
                if (this.pageIndex === this.pagesCount) {
                    this.pageStart = this.pagesCount - 4;
                } else if (this.pageIndex === this.pagesCount - 1) {
                    this.pageStart = this.pagesCount - 4;
                }
            },
            firstPage: function () {
                this.pageIndex = 1;
                this.pageStart = 1;
                this.pageEnd = 5;
                this.pagingBig();
            },
            lastPage: function () {
                this.pageIndex = this.pagesCount;

                if (this.pageIndex > 998 && this.pageIndex <= 9997) {
                    this.pageStart = this.pagesCount - 3;
                    this.pageEnd = this.pagesCount;
                } else if (this.pageIndex > 9997) {
                    this.pageStart = this.pagesCount - 2;
                    this.pageEnd = this.pagesCount;
                } else {
                    this.pageStart = this.pagesCount - 4;
                    this.pageEnd = this.pagesCount;
                }
                this.pagingBig();
            },
            getVisibleFields: function () {
                var vFields = [];
                var fields = [];
                var layerInfo = null;
                arrayUtils.forEach(JZ.layersList, function (layer) {
                    if (layer.id === JZ.currentLayerID) {
                        layerInfo = layer;
                    }
                });
                if (layerInfo.gisdatafields && typeof layerInfo.gisdatafields == 'string') {
                    fields = layerInfo.gisdatafields.split(",");
                    if (fields.length <= 1) {
                        fields = layerInfo.gisdatafields.split("，");
                    }
                    arrayUtils.forEach(fields, function (f) {
                        var d = f.split(":");
                        if (d.length <= 1) {
                            d = f.split("：");
                        }
                        vFields.push({"ENAME":d[0], "CNAME":d[1]});
                    });
                }
                return vFields;
            },
            getGeometries: function (start, end) {
                var that = this;
                arrayUtils.forEach(JZ.layersList, function (layer) {
                    if (layer.id === JZ.currentLayerID) {
                        var featureUrl = layer.mapServerUrl + "/" + layer.serverindex;
                        var queryTask = new QueryTask(featureUrl);
                        var esriQuery = new EsriQuery();
                        var objids = [];
                        for (var i = start; i < end; i++) {
                            if (that.featureIDS[i]) {
                                objids.push(that.featureIDS[i]);
                            }
                        }
                        if (objids.length > 0) {
                            var whereStr = (objids.length > 1) ? objids.join(",") : objids[0];
                            var qwhere = "OBJECTID in (" + whereStr + ")";
                            esriQuery.where = qwhere;
                            esriQuery.returnGeometry = true;
                            esriQuery.outFields = ["*"];
                            queryTask.execute(esriQuery, function (featureSets) {
                                that.addFeatureInfo(featureSets);
                            }, function (err) {
                                console.log("发生错误，请检查网络连接是否正常！");
                            });
                        }
                    }
                });
            },
            addFeatureInfo: function (featureSet) {
                //此处开始查询
                var divHtml = "";
                var vFields = this.getVisibleFields();
                var fieldsAlias = featureSet.fieldAliases;
                var length = featureSet.features.length;
                for (var i = 0; i < length; i++) {
                    divHtml += "<li data-index='" + i + "'>";
                    divHtml += "<div style='float:left;position: absolute;'><img src='images/marker/locate.png'/></div>";
                    divHtml += "<div style='margin-left:50px;'>";
                    var colName = (!featureSet.features[i].attributes["OBJNAME"] || 
                                    featureSet.features[i].attributes["OBJNAME"] == 'null' ||
                                    featureSet.features[i].attributes["OBJNAME"] == 'undefined') ? "" : featureSet.features[i].attributes["OBJNAME"];
                    divHtml += "<p>" + colName + "</p>";
                    arrayUtils.forEach(vFields, function (field) {
                        var ename = field.ENAME;
                        for (var key in fieldsAlias) {
                            if (fieldsAlias[key] == field.ENAME) {
                                ename = key;
                            }
                        }
                        var colValue = (!featureSet.features[i].attributes[ename] || 
                                         featureSet.features[i].attributes[ename] == 'null' || 
                                         featureSet.features[i].attributes[ename] == 'undefined') ? "" : featureSet.features[i].attributes[ename];
                        divHtml += "<p>" + field.CNAME + "：" + colValue + "</p>";
                    });
                    divHtml += "</div>"
                    divHtml = divHtml + "</li>"
                }
                domConstruct.place(divHtml, dom.byId("search-result-list"), "first");
                query("#search-result-list li").on("click", function (e) {
                    if (e.currentTarget) {
                        var index = parseInt(domAttr.get(e.currentTarget, "data-index"), 10);
                        require(['widget/utils/GraphicUtils', 'widget/prop/PropInfo'], function (clazz, PropertyInfo) {
                            if (JZ.currentLayerID == 482 || JZ.currentLayerID == 262) {
                                var url = featureSet.features[index].attributes['OBJPIC'];
                                require(['esri/layers/MapImageLayer', 'esri/layers/MapImage'], function (MapImageLayer, MapImage) {
                                    if (!JZ.imageLayer) {
                                        JZ.imageLayer = new MapImageLayer();
                                        JZ.mc.addLayer(JZ.imageLayer, 10);
                                    }
                                    JZ.imageLayer.removeAllImages();
                                    var img = new MapImage({
                                        'extent': featureSet.features[index].geometry.getExtent(),
                                        'href': url
                                    });
                                    JZ.imageLayer.addImage(img);
                                });
                            } else {
                                clazz.getInstance().addGraphic(JZ.mc.graphics, featureSet.features[index].geometry, featureSet.features[index].attributes);
                                var infoWin = new PropertyInfo(JZ.mc, featureSet.features[index], JZ.currentLayerID);
                                infoWin.show();
                            }
                        });
                    }
                });
            },
            paging: function (suc) {
                var that = this;
                that.featureIDS = suc;
                domConstruct.empty(dom.byId("search-result-list"));
                if (JZ.mc && JZ.mc.graphics) {
                    JZ.mc.graphics.clear();
                }
                if (that.featureIDS && that.featureIDS.length > 0) {
                    var length = that.featureIDS.length > 7 ? 7 : that.featureIDS.length;
                    that.getGeometries(0, length);
                    /* 开始制作分页 */
                    that.pagesCount = (that.featureIDS.length % 7 !== 0) ? (parseInt(that.featureIDS.length / 7 + 1)) : (that.featureIDS.length / 7);
                    dom.byId("featureResultsLength").innerHTML = "共查询到" + that.featureIDS.length + "条记录(共" + that.pagesCount + "页)";
                    if (that.pagesCount <= 8) {
                        that.pageIndex = 1;
                        that.pagingSmall();
                    } else {
                        that.pageIndex = 1;
                        that.pageStart = 1;
                        that.pageEnd = (that.pagesCount > 5) ? 5 : that.pagesCount;
                        that.pagingBig();
                    }
                } else {
                    domConstruct.empty(dom.byId("pagination-list"));
                    dom.byId("featureResultsLength").innerHTML = "";
                    domConstruct.place("<li><p style='text-align:center;vertical-align:center;line-height:45px;color:#F00;'>查询结果为空，请重新输入条件后查询</p></li>", dom.byId("search-result-list"), "last");
                }
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