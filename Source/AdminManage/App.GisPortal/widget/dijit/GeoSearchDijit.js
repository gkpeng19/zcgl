/**
* @author wangyafei
* 底图切换
*/
define(['dojo/_base/declare',
        'dijit/_WidgetBase',
        'dijit/_TemplatedMixin',
        'dojo/request/script',
        'dojo/dom',
        'dojo/on',
        'dojo/_base/lang',
        'dojo/dom-construct',
        'dojo/dom-attr',
        'dojo/dom-style',
        'dojo/_base/array',
        'dojo/query',
        'dojo/dom-class',
        'dojo/request/xhr',
        'dojo/_base/lang',
        'esri/geometry/Point',
        'dojo/text!./template/GeoSearchDijit.html'],
function (declare, _WidgetBase, _TemplatedMixin, script, dom, on, lang, domConstruct, domAttr, domStyle,
    arrayUtils, query, domClass, xhr, lang, Point, template) {
    var instance = null, clazz;
    /* 根据用户的权限从数据库中获取用户的功能权限 */
    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        pNode: "",
        map: null,
        results: [],
        deferred: null,
        startup: function () {
            this.inherited(arguments);
            domConstruct.place(this.domNode, this.pNode, "first");
            this.own(on(this.jzSearchPosition, "keyup", lang.hitch(this, this.showNotify)));
            this.own(on(document, "click", lang.hitch(this, this.closeNotify)));
        },
        closeNotify: function () {
            var geoNode = dom.byId("jzGeocoderSearch");
            if (geoNode) {
                //domConstruct.empty(geoNode);
                domStyle.set(geoNode, "display", "none");
            }
        },
        showNotify: function () {
            this.results = [];
            var value = this.jzSearchPosition.value;
            if (value.length >= 3) {
                var that = this;
                var url = "http://172.24.57.47/service/AddrCode/cmd?commandType=0&AddrName=" + value + "&IsAccurate=false&AddrType=AllType&Start=0&Nums=10&username=syllhjxxjx&password=syllhjxxjx123";
                //var url = "http://10.246.0.81:10000/service/AddrCode/cmd?commandType=0&AddrName=" + value + "&IsAccurate=false&AddrType=AllType&Start=0&Nums=10&username=syllhjxxjx&password=syllhjxxjx123";
                if (that.deferred) {
                    that.deferred.cancel();
                }
                //proxy/Handler2.ashx?searchType=placename&pageindex=1&searchname=
                that.deferred = xhr("../webservice/WebServiceMap.asmx/GetRequestXML", {
                    //var defer1 = xhr(url, {
                    handleAs: "xml",
                    timeout: 5000,
                    query: { value: value }
                });
                that.deferred.then(function (suc) {
                    var x = suc.documentElement.getElementsByTagName("row");
                    for (i = 0; i < x.length; i++) {
                        that.results.push({
                            ID: i,
                            NAME: x[i].getElementsByTagName("地址标准名称")[0].textContent,
                            X: x[i].getElementsByTagName("X0")[0].textContent,
                            Y: x[i].getElementsByTagName("Y0")[0].textContent
                        });
                    }
                    that.setSearchResult();
                }, function (err) {
                    console.log("无法获取信息中心的地址编码服务！");
                });
            }
        },
        setSearchResult: function () {
            var that = this;
            var geoNode = dom.byId("jzGeocoderSearch");
            if (geoNode) {
                domConstruct.empty(geoNode);
            }
            if (that.results.length >= 1) {
                if (!geoNode) {
                    var margin = domStyle.getComputedStyle(that.jzSearchPosition);
                    geoNode = domConstruct.create("div", { id: 'jzGeocoderSearch' }, document.body, "last");
                }
                var ulNode = domConstruct.create("ul", {}, geoNode);
                arrayUtils.forEach(that.results, function (res) {
                    domConstruct.create("li", { innerHTML: res.NAME, 'data-dojo-id': res.ID }, ulNode, "last");
                });
                domStyle.set(geoNode, "display", "");

                query("#jzGeocoderSearch ul li").on("click", function (e) {
                    var target = e.currentTarget ? e.currentTarget : e.srcElement;
                    var did = parseInt(domAttr.get(target, "data-dojo-id"), 10);
                    arrayUtils.forEach(that.results, function (ress) {
                        if (did == ress.ID) {
                            that.jzSearchPosition.value = ress.NAME;
                            var point = new Point(ress.X, ress.Y, that.map.spatialReference);
                            that.map.setZoom(6);
                            that.map.centerAt(point);
                        }
                    });
                });
            } else {
                if (geoNode) {
                    domStyle.set(geoNode, "display", "none");
                }
            }
        }
    });
    clazz.getInstance = function (map) {
        if (instance === null) {
            instance = new clazz({
                pNode: "mapTools",
                map: map
            });
        }
        return instance;
    };
    return clazz;
});