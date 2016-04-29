define([
    "dojo/_base/declare",
    "dojo/on",
    "dojo/dom",
    "dojo/query",
    "dojo/dom-class",
    "dojo/_base/window",
    "dojo/_base/array",
    "dijit/layout/TabContainer",
    "dijit/layout/ContentPane",
    "dojo/data/ItemFileWriteStore",
    "dojox/grid/DataGrid",
    "dojo/domReady!"
], function (declare, on, dom, query, domClass, win, Array, TabContainer, ContentPane, ItemFileWriteStore, DataGrid) {
    "use strict";
    var PropertyInfo = declare([], {
        defaultFields: {
            "OBJNAME": "OBJNAME",
            "OBJCODE": "OBJCODE"
        },
        //构造函数
        constructor: function (map, graphic, geometryService) {
            this.map = map;
            this.graphic = graphic;
            this.geometryService = geometryService;
        },
        show: function () {
            var centerPoint = null;
            //计算中心点坐标
            if (this.graphic && this.graphic.geometry) {
                if (this.graphic.geometry.type === "polygon") {
                    centerPoint = this.graphic.geometry.getCentroid();
                } else if (this.graphic.geometry.type === "point") {
                    centerPoint = this.graphic.geometry;
                } else if (this.graphic.geometry.type === "polyline") {
                    centerPoint = this.graphic.geometry.getPoint(0, 0);
                } else if (this.graphic.geometry.type === "multipoint") {
                    centerPoint = this.graphic.geometry.getPoint(0);
                } else if (this.graphic.geometry.type === "extent") {
                    centerPoint = this.graphic.geometry.getCenter();
                }

                if (this.graphic.attributes) {
                    //获取graphic中的属性
                    this.map.infoWindow.setTitle(this.graphic.attributes[this.defaultFields["OBJNAME"]]);
                    this.map.infoWindow.setContent("<div style='width:100%;height:260px'><div id='propertyWindow'></div><div class='infoWindowOper' style='margin-top:5px;text-align:right;'><button class='btn btn-primary' id='btnInfoHistory'>历史</button><button class='btn btn-primary' id='btnInfoEdit'>编辑</button><button class='btn btn-primary hide' id='btnInfoSave'>保存</button><button class='btn btn-primary hide' id='btnInfoCancel'>取消</button></div></div>");
                    this.map.infoWindow.resize(300, 300);
                    this.map.infoWindow.show(centerPoint);
                    //添加Tab页
                    this.addTabContent();
                    //添加DataGrid
                    this.addDataGrid();
                    this.map.centerAt(centerPoint);
                    //添加事件
                    this.attachEvent();
                }
            }
        },

        addTabContent: function () {
            var mc = this.graphic.attributes[this.defaultFields["OBJNAME"]];
            var code = this.graphic.attributes[this.defaultFields["OBJCODE"]];

            var htmlStr = "<div style='margin:5px;padding:5px;border:1px solid #E9E9E9;background-color:#F5F5F5;border-radius:4px;-webkit-border-radius:4px;-moz-border-radius:4px;'><p>编码:" + code + "</p><p>名称:" + mc + "</p></div>";
            var infoStr = "<div id='propertyInfoDiv' style='width:100%;height:100%'></div>";

            var tc = new TabContainer({
                style: "width:100%;height:220px"
            }, "propertyWindow");

            //基本信息Tab页
            var cp1 = new ContentPane({
                title: "基本信息",
                content: htmlStr
            });
            tc.addChild(cp1);

            //详细信息Tab页
            var cp2 = new ContentPane({
                title: "详细信息",
                content: infoStr
            });
            tc.addChild(cp2);

            //坐标信息Tab页
            var cp3 = new ContentPane({
                title: "面积信息(m³)",
                content: "<p>面积：" + this.graphic.attributes["SHAPE.AREA"] + "</p><p>实际面积：" + 1000 + "</p>"
            });
            tc.addChild(cp3);

            var picturesArray = [
                "images/upload/1.jpg",
                "images/upload/2.jpg",
                "images/upload/3.jpg",
                "images/upload/4.jpg",
                "images/upload/5.jpg",
                "images/upload/6.jpg",
                "images/upload/7.jpg",
                "images/upload/8.jpg"
            ];
            var imgData = {
                items: []
            };

            var imgWriteStore = new dojo.data.ItemFileWriteStore({ data: imgData });

            Array.forEach(picturesArray, function (pic) {
                var additem = { "image": pic }
                imgWriteStore.newItem(additem);
            });
            //if(!self.flagEvent){
            //self.addEvents(picturesArray);
            //self.flagEvent = true;
            //}
            //添加幻灯片
            var cp4 = new dijit.layout.ContentPane({
                title: "实景图",
                content: "<div id='slideShow' style='width:100%;height:220px' data-dojo-type='dojox/image/SlideShow' data-dojo-props='noLink:true, autoLoad:true, autoStart:true, loop:true,slideshowInterval:3, fixedHeight:true'></div>"
            });
            tc.addChild(cp4);

            var slideShow = registry.byId("slideShow");
            slideShow.setDataStore(imgWriteStore, { query: {}, count: 20 }, {
                imageLargeAttr: "image"
            });

            domStyle.set(dom.byId("slideShow"), "width", "350px");
            on(slideShow, "click", function (e) {
                var currentImage = e.target;
                if (currentImage.src) {
                    //停止幻灯片放映
                    slideShow._stop();
                    //为该img标签赋值
                    domAttr.set(dom.byId("slidshowCurrentImage"), "src", currentImage.src);
                    //修改图片的大小
                    self.resizeImage(currentImage);
                    //计算当前点击图片的索引位置
                    self.flagI = 0;
                    var imageName = currentImage.src.substr(currentImage.src.lastIndexOf("/") + 1);
                    for (var i = 0; i < picturesArray.length; i++) {
                        var item = picturesArray[i];
                        var imgName = item.substr(item.lastIndexOf("/") + 1);
                        if (imgName == imageName) {
                            break;
                        }
                        self.flagI = self.flagI + 1;
                    }

                    window.onresize = function () {
                        if (domStyle.get(dom.byId("slidshowImagecontainer"), "display") == "none" ||
						   domStyle.get(dom.byId("Imagecontainer"), "display") == "none") {
                            return;
                        } else {
                            self.resizeImage();
                        }
                    };
                }
            });
            on(dom.byId("BtnSlidShowClose"), "click", function () {
                domStyle.set(dom.byId("Imagecontainer"), "display", "none");
                domStyle.set(dom.byId("slidshowImagecontainer"), "display", "none");
            });
            tc.startup();
        },

        addDataGrid: function () {
            var layout = [
	                    { name: '属性名', field: 'PROPERTYNAME', width: 6 },
	                    { name: '属性值', field: 'PROPERTYVALUE', width: 9}];

            //定义可写的数据源
            var store = new ItemFileWriteStore({
                data: {
                    identifier: "PROPERTYNAME",
                    items: []
                }
            });

            //创建DataGrid对象并启动DataGrid
            var grid = new DataGrid({
                store: store,
                structure: layout
            }, dom.byId("propertyInfoDiv"));
            grid.startup();

            for (var attr in this.graphic.attributes) {
                var item = {
                    "PROPERTYNAME": attr,
                    "PROPERTYVALUE": this.graphic.attributes[attr]
                };
                store.newItem(item);
            }
        },
        attachEvent: function () {
            var btns = query(".infoWindowOper button");
            on(win.body(), on.selector(".infoWindowOper .btn", 'click'), function (e) {
                if (e && e.preventDefault) {
                    e.preventDefault();
                } else {
                    window.event.returnValue = false;
                }

                var btn = e.target;
                switch (e.target.id) {
                    case 'btnInfoHistory':
                        break;
                    case 'btnInfoEdit':
                    case 'btnInfoSave':
                    case 'btnInfoCancel':
                        Array.forEach(btns, function (but) {
                            if (!domClass.contains(but, 'hide')) {
                                domClass.add(but, 'hide');
                            } else if (domClass.contains(but, 'hide')) {
                                domClass.remove(but, 'hide');
                            }
                        });
                        break;
                }
                
            });
        },
        detachEvent: function () {

        }
    });

    return PropertyInfo;
});

