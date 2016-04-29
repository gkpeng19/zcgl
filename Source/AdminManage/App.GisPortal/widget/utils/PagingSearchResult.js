/**
* @author wangyafei
* 绿色产业类别
*/
define('widget/utils/PagingSearchResult',
    ['dojo/_base/declare',
     'dojo/_base/array',
     'dojo/_base/lang',
     'dojo/dom',
     'dojo/dom-class',
     'dojo/dom-attr',
     'dojo/dom-construct',
     'dojo/query'
    ], function (declare, arrayUtils, lang, dom, domClass, domAttr, domConstruct, query) {
        "use strict"
        var instance = null, clazz;
        clazz = declare(null, {
            _map: null,
            features: null,
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
                var end = start + 6;
                if (end > this.features.length - 1) {
                    end = this.features.length - 1;
                }
                var divHtml = "";
                var that = this;
                var vFields = [];
                for (var i = start; i <= end; i++) {
                    //判断是图层还是要素，如果是图层，则执行图层的操作
                    if (that.features[i] && that.features[i].PNAME && that.features[i].PID) {
                        divHtml += "<li data-index='" + i + "'>";
                        divHtml += "<div style='float:left;'><img width='48px' height='48px' src='images/marker/layer.png'/></div>";
                        divHtml += "<div>";
                        divHtml += "<p>" + that.features[i]["cnName"] + "</p>";
                        divHtml += "<p>英文名称：" + that.features[i]["enName"] + "</p>";
                        divHtml += "<p>所属类别：" + that.features[i]["PNAME"] + "</p>";
                        divHtml += "</div>";
                        divHtml = divHtml + "</li>";
                    } else {
                        var currentLayer = null;
                        var layerId = 0;
                        arrayUtils.forEach(JZ.privilegeLayers, function (clsInfo) {
                            var layersInfo = clsInfo.LAYERS;
                            arrayUtils.forEach(layersInfo, function (layer) {
                                if (layer.serverindex == that.features[i].layerId) {
                                    layerId = layer.id;
                                    vFields = that.getVisibleFields(layer);
                                    currentLayer = layer;
                                }
                            });
                        });
                        divHtml += "<li data-layerid='" + layerId + "' data-index='" + i + "'>";
                        divHtml += "<div style='float:left;padding: 15px 0 5px 0;'><img width='48px' height='48px' src='images/marker/locate.png'/></div>";
                        divHtml += "<div>";
                        var name = that.features[i].feature.attributes["OBJNAME"] ? that.features[i].feature.attributes["OBJNAME"] : that.features[i].feature.attributes["MC"];
                        if (!name) {
                            name = that.features[i].feature.attributes["名称"];
                        }
                        divHtml += "<p>" + name + "</p>";
                        
                        arrayUtils.forEach(vFields, function (field) {
                            var nvalue = that.features[i].feature.attributes[field.ENAME] ? that.features[i].feature.attributes[field.ENAME] : that.features[i].feature.attributes[field.CNAME];
                            divHtml += "<p>" + field.CNAME + "：" + nvalue + "</p>";
                        });
                        divHtml += "<p>图层：" + currentLayer.cnName + "</p>";
                        divHtml += "</div>";
                        divHtml = divHtml + "</li>";
                    }
                }
                domConstruct.place(divHtml, dom.byId("search-result-list"), "first");
                query("#search-result-list li").on("click", lang.hitch(this, this.showInfo));
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
                    } else {
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
                    this.pageStart = (this.pageIndex - 2 >= 1) ? (this.pageIndex - 2) : 1;
                    this.pageEnd = (this.pageIndex + 2 < this.pagesCount) ? (this.pageIndex + 2) : this.pagesCount;
                    this.setValue();
                    this.pagingBig();
                }
            },
            prevPage: function () {
                if (this.pageIndex > 1) {
                    this.pageIndex -= 1;
                    this.pageStart = (this.pageIndex - 2 >= 1) ? (this.pageIndex - 2) : 1;
                    this.pageEnd = (this.pageIndex + 2 < this.pagesCount) ? (this.pageIndex + 2) : this.pagesCount;
                    this.setValue();
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
                this.pageStart = this.pagesCount - 4;
                this.pageEnd = this.pagesCount;
                this.pagingBig();
            },
            getVisibleFields: function (layer) {
                var vFields = [];
                if (layer.gisdatafields && typeof layer.gisdatafields == 'string') {
                    var fields = layer.gisdatafields.split(",");
                    if (fields.length <= 1) {
                        fields = layer.gisdatafields.split("，");
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
            paging: function (featureSet) {
                var that = this;
                that.features = featureSet;

                if (JZ.mc && JZ.mc.graphics) {
                    JZ.mc.graphics.clear();
                }
                domConstruct.empty(dom.byId("search-result-list"));
                if (that.features.length > 0) {
                    var divHtml = "";
                    var length = that.features.length > 7 ? 7 : that.features.length;
                    //计算显示字段
                    var vFields = null;
                    for (var i = 0; i < length; i++) {
                        //判断是图层还是要素，如果是图层，则执行图层的操作
                        if (that.features[i] && that.features[i].PNAME && that.features[i].PID) {
                            divHtml += "<li data-cls='1' data-index='" + i + "'>";
                            divHtml += "<div style='float:left;'><img width='48px' height='48px' src='images/marker/layer.png'/></div>";
                            divHtml += "<div>";
                            divHtml += "<p>" + that.features[i]["cnName"] + "</p>";
                            divHtml += "<p>英文名称：" + that.features[i]["enName"] + "</p>";
                            divHtml += "<p>所属类别：" + that.features[i]["PNAME"] + "</p>";
                            divHtml += "</div>"
                            divHtml = divHtml + "</li>"
                        } else {
                            var currentLayer = null;
                            var layerId = -1;
                            arrayUtils.forEach(JZ.privilegeLayers, function (clsInfo) {
                                var layersInfo = clsInfo.LAYERS;
                                arrayUtils.forEach(layersInfo, function (layer) {
                                    if (layer.serverindex == that.features[i].layerId) {
                                        layerId = layer.id;
                                        vFields = that.getVisibleFields(layer);
                                        currentLayer = layer;
                                    }
                                });
                            });
                            divHtml += "<li data-layerid='" + layerId + "' data-cls='0' data-index='" + i + "'>";
                            divHtml += "<div style='float:left;padding: 15px 0 5px 0;'><img width='48px' height='48px' src='images/marker/locate.png'/></div>";
                            divHtml += "<div>";
                            var name = that.features[i].feature.attributes["OBJNAME"] ? that.features[i].feature.attributes["OBJNAME"] : that.features[i].feature.attributes["MC"];
                            if (!name) {
                                name = that.features[i].feature.attributes["名称"];
                            }
                            divHtml += "<p>" + name + "</p>";
                            
                            arrayUtils.forEach(vFields, function (field) {
                                var nvalue = that.features[i].feature.attributes[field.ENAME] ? that.features[i].feature.attributes[field.ENAME] : that.features[i].feature.attributes[field.CNAME];
                                divHtml += "<p>" + field.CNAME + "：" + nvalue + "</p>";
                            });
                            divHtml += "<p>图层：" + currentLayer.cnName + "</p>";
                            divHtml += "</div>"
                            divHtml = divHtml + "</li>"
                        }
                    }
                    domConstruct.place(divHtml, dom.byId("search-result-list"), "first");
                    query("#search-result-list li").on("click", lang.hitch(this, this.showInfo));
                    /* 开始制作分页 */
                    that.pagesCount = (that.features.length % 7 !== 0) ? (parseInt(that.features.length / 7 + 1)) : (that.features.length / 7);
                    dom.byId("featureResultsLength").innerHTML = "共查询到" + that.features.length + "条记录(共" + that.pagesCount + "页)";
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
            },
            showInfo: function (e) {
                if (e.currentTarget) {
                    var that = this;
                    var index = parseInt(domAttr.get(e.currentTarget, "data-index"), 10);
                    var cls = parseInt(domAttr.get(e.currentTarget, "data-cls"), 10);
                    var layerid = parseInt(domAttr.get(e.currentTarget, "data-layerid"), 10);
                    if (cls === 1) {
                        //表示图层

                    } else {
                        //表示要素
                        require(['widget/utils/GraphicUtils', 'widget/prop/PropInfo'], function (clazz, PropertyInfo) {
                            clazz.getInstance().addGraphic(JZ.mc.graphics, that.features[index].feature.geometry, that.features[index].attributes);
                            var infoWin = new PropertyInfo(JZ.mc, that.features[index].feature, layerid);
                            infoWin.show();
                        });
                    }
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