/**
* @author wangyafei
* 功能菜单
*/
define("widget/menu/MenuBar",
[
"dojo/_base/declare",
"dojo/_base/lang",
"dojo/request/xhr",
"dojo/_base/array",
"dojo/dom-construct",
"dojo/dom",
"dojo/on",
"dojo/dom-class",
"dojo/dom-attr",
"dojo/topic",
"dojo/query",
"dojo/mouse",
"dojo/dom-style",
"dojo/_base/fx"
],
function (declare, lang, xhr, arrayUtils, domConstruct, dom, on, domClass,
    domAttr, topic, query, mouse, domStyle, fx) {
    "use strict";
    var instance = null, clazz;
    /* 根据用户的权限从数据库中获取用户的功能权限 */
    clazz = declare(null, {
        pNode: null,
        menuLength: 0,

        constructor: function () {
            //类别切换事件
            topic.subscribe("topic/CategoryToggle", lang.hitch(this, this.toggle));
        },
        show: function (node) {
            this.pNode = node;
            this.getUserPrivilege();
        },
        toggle: function () {
            /* 先绑定事件 */
            query("#user-privs ul li img").on("click", lang.hitch(this, this.setStatus));
            var that = this;
            on(dom.byId("up-func"), "click", function (e) {
                var node = dom.byId("ld-category");
                var value = domStyle.get(node, "margin-top");
                value = value ? value : 0;
                that.rollUp(node, value);
            });
            on(dom.byId("down-func"), "click", function (e) {
                var node = dom.byId("ld-category");
                var value = domStyle.get(node, "margin-top");
                value = value ? value : 0;
                that.rollDown(node, value);
            });
            that.getSelectedNode();
        },
        setStatus: function (e) {
            var node = e.currentTarget;
            if (!domClass.contains(node.parentNode, 'selected')) {
                var nodes = query("#user-privs ul li.selected img");
                arrayUtils.forEach(nodes, function (n) {
                    var newSrc = n.src.substr(0, n.src.indexOf('_1.png')) + '.png';
                    n.src = newSrc;
                });
                query("#user-privs ul li.selected").removeClass('selected');
                domClass.add(node.parentNode, 'selected');
                dom.byId("layerOperPanel_className").innerHTML = domAttr.get(node.parentNode, "data-title");
                this.getLayersList(node.parentNode);
            }
        },
        //获取当前选中的类别
        getSelectedNode: function () {
            var node = query("#user-privs ul li.selected")[0];
            this.getLayersList(node);
        },
        getLayersList: function (node) {
            //获取大分类下面的所有有权限的图层
            require(['widget/category/LayerCls'], function (LayerCls) {
                var pid = domAttr.get(node, "data-id");
                var layerCls = LayerCls.getInstance();
                layerCls.ajaxGet(pid);
            });
        },
        getUserPrivilege: function () {
            var that = this;
            /* 获取用户权限 */
            var defer = xhr.get("webservice/WebServiceMap.asmx/GetMenuMapLayer", {
                handleAs: "json",
                timeout: 50000
            });
            /* 请求成功事件,获取到用户拥有的菜单功能 */
            defer.then(function (suc) {
                JZ.privilegeLayers = suc;
                that.menuLength = suc.length;
                that.ResizeMenuBar();
            }, function (err) {
                window.location.href = "Login.aspx?ReturnUrl=" + window.location.href;
            });
        },
        /* 浏览器窗口大小改变事件 */
        ResizeMenuBar: function () {
            var objArray = JZ.privilegeLayers;
            var parentNode = this.pNode;
            var htmlStr = "";
            arrayUtils.forEach(objArray, function (obj, i) {
                if (i === 0) {
                    var src = obj.IMG.substr(0, obj.IMG.indexOf('.png')) + '_1.png';
                    htmlStr += "<li class='selected' data-title='" + obj["NAME"] + "' data-id='" + obj["ID"] + "'><img src='" + src + "'/></li>";
                } else {
                    htmlStr += "<li data-title='" + obj["NAME"] + "' data-id='" + obj["ID"] + "'><img src='" + obj.IMG + "'/></li>";
                }
            });
            domConstruct.place(htmlStr, parentNode);
            //this.getSelectedNode();
            this.attachEvent();
            //菜单项及菜单项的事件添加成功
            topic.publish("topic/AddMenuComplete");
        },
        attachEvent: function () {
            var nodeList = query("#user-privs img");
            nodeList.on(mouse.enter, function (e) {
                var node = e.currentTarget;
                if (!domClass.contains(node.parentNode, 'selected')) {
                    var newSrc = node.src.substr(0, node.src.indexOf('.png')) + '_1.png';
                    node.src = newSrc;
                }
            });
            nodeList.on(mouse.leave, function (e) {
                var node = e.currentTarget;
                if (!domClass.contains(node.parentNode, 'selected')) {
                    var newSrc = node.src.substr(0, node.src.indexOf('_1.png')) + '.png';
                    node.src = newSrc;
                }
            });
            var pNode = dom.byId("user-privs");
            var that = this;
            dojo.connect(pNode, (!dojo.isMozilla ? "onmousewheel" : "DOMMouseScroll"), function (e) {
                var node = dom.byId("ld-category");
                var scroll = e[(!dojo.isMozilla ? "wheelDelta" : "detail")] * (!dojo.isMozilla ? 1 : -1);
                var value = domStyle.get(node, "margin-top");
                value = value ? value : 0;
                if (scroll > 0) {
                    that.rollDown(node, value);
                } else {
                    that.rollUp(node, value);
                }
            });
        },
        rollUp: function (node, value) {
            if (value + 324 >= 0) {
                value = 0;
            } else {
                value = value + 324;
            }
            fx.animateProperty({
                node: node, duration: 500,
                properties: {
                    marginTop: value
                }
            }).play();
        },
        rollDown: function (node, value) {
            value = value - 324;
            var result = value + this.menuLength * 81;
            var pStyle = domStyle.getComputedStyle(dom.byId("ld-menu-category"));
            var height = parseInt(pStyle.height, 10);
            if (result < height) {
                value = height - this.menuLength * 81;
            }
            fx.animateProperty({
                node: node, duration: 500,
                properties: {
                    marginTop: value
                }
            }).play();
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