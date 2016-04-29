/**
 * @author Administrator
 */
define(['dojo/_base/declare', 'dojo/topic', 'dojo/_base/lang',
    'dijit/_WidgetBase', 'dijit/_TemplatedMixin', 'dojo/on', 'dojo/dom',
    'dojo/dom-geometry',
    'dojo/dom-construct',
    'dojo/request/xhr',
'dojo/dom-style', 'dojo/dom-attr', 'dojo/text!./template/UserInfo.html'],
function (declare, topic, lang, _WidgetBase, _TemplatedMixin, on, dom,
    domGeometry,
    domConstruct, xhr, domStyle, domAttr, template) {
    var instance = null, clazz;
    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        baseClass: "jz-userinfo",
        pNode: '',
        exists: false,
        constructor: function () {
            this.inherited(arguments);
            this.own(on(document, "click", lang.hitch(this, this.hideDialog)));
            this.own(on(document, "mousemove", lang.hitch(this, this.checkHideDialog)));
        },
        hideDialog: function () {
            domStyle.set(this.domNode, "display", "none");
        },
        checkHideDialog: function (e) {
            var margin = domGeometry.getMarginBox(this.domNode);
            if (e.clientX > (margin.l + 105) || e.clientY > (margin.t + 80) || e.clientX < (margin.l - 5) || e.clientY < (margin.t - 20)) {
                this.hideDialog();
            }
        },
        startup: function () {
            if (!this.exists) {
                if (this.pNode) {
                    var pMargin = domGeometry.getMarginBox(this.pNode);
                    var center = pMargin.l + pMargin.w / 2;
                    domStyle.set(this.domNode, "left", center - 45 + "px");
                    domStyle.set(this.domNode, "top", pMargin.t + 15 + "px");
                    domConstruct.place(this.domNode, document.body);
                    this.exists = true;
                }
            } else {
                domStyle.set(this.domNode, "display", "");
            }
        },
        onChangePwd: function () {
            var that = this;
            require(['widget/dijit/PasswordDijit'], function (clazz) {
                clazz.getInstance().startup();
                domStyle.set(that.domNode, "display", "none");
            });
        },
        onLogout: function () {
            var defer = xhr.get("webservice/WebServiceMap.asmx/Logout", {
                handleAs: "json",
                timeout: 10000
            });
            defer.then(function (suc) {
                window.location.href = "Login.aspx?ReturnUrl=" + window.location.href;
            }, function (err) {
                window.location.href = "Login.aspx?ReturnUrl=" + window.location.href;
            });
        }
    });

    clazz.getInstance = function () {
        if (instance === null) {
            instance = new clazz({
                pNode: 'currentUserName'
            });
        }
        return instance;
    };
    return clazz;

});
