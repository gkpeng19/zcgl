/**
 * @author Administrator
 */
define(['dojo/_base/declare',
    'dojo/_base/fx',
    'dojo/_base/lang', 'dojo/topic',
    'dojo/parser', 'dijit/_WidgetBase', 'dijit/_TemplatedMixin', 'dojo/on',
    'dojo/dom', 'dojo/dom-construct',
    'dojo/dom-attr',
    'dojo/request/xhr',
    'dojo/json',
'dojo/dom-style', 'esri/dijit/Measurement', 'esri/units',
'dojo/text!./template/PasswordDijit.html'],
function (declare, baseFx, lang, topic, parser, _WidgetBase, _TemplatedMixin, on, dom, domConstruct, domAttr, xhr, JSON, domStyle, Measurement, Units, template) {
    var instance = null, clazz;
    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        baseClass: "jz-pwd-dialog",
        mixin: false,
        isValid: false,
        postCreate: function () {
            this.titleNode.innerHTML = "密码修改";
        },
        startup: function () {
            this.inherited(arguments);
            if (!this.mixin) {
                domConstruct.place(this.domNode, document.body);
                this.showDelay();
                this.mixin = true;
                this.own(on(this.jzOldPwd, "change", lang.hitch(this, this.onOldPwdChange)));
                this.own(on(this.jzNewPwd, "change", lang.hitch(this, this.onNewPwdChange)));
                this.own(on(this.jzReNewPwd, "change", lang.hitch(this, this.onReNewPwdChange)));
            } else {
                this.onOpen();
            }
            domStyle.set(this.pwdErrorSum, "display", "none");
        },
        onOldPwdChange: function () {
            //xhr.get();
        },
        onNewPwdChange: function () {
            if (this.jzOldPwd.value == "" || this.jzNewPwd.value == this.jzOldPwd.value) {
                this.pwdErrorSum.innerHTML = "发生错误：旧密码不能为空，且不能和新密码相同！";
                domStyle.set(this.pwdErrorSum, "display", "");
                domAttr.set(this.jzBtnCommit, "disabled", true);
            } else if (this.jzNewPwd.value.length < 6) {
                this.pwdErrorSum.innerHTML = "发生错误：新密码至少6位字符！";
                domStyle.set(this.pwdErrorSum, "display", "");
                domAttr.set(this.jzBtnCommit, "disabled", true);
            } else if (this.jzReNewPwd.value != this.jzNewPwd.value) {
                this.pwdErrorSum.innerHTML = "发生错误：两次输入的新密码不一样！";
                domStyle.set(this.pwdErrorSum, "display", "");
                domAttr.set(this.jzBtnCommit, "disabled", true);
            } else {
                domStyle.set(this.pwdErrorSum, "display", "none");
                domAttr.set(this.jzBtnCommit, "disabled", false);
            }
        },
        onReNewPwdChange: function () {
            //this.jzOldPwd.value.replace(/(^s*)|(s*$)/g, '')
            if (this.jzOldPwd.value == "" || this.jzNewPwd.value == this.jzOldPwd.value) {
                this.pwdErrorSum.innerHTML = "发生错误：旧密码不能为空，且不能和新密码相同！";
                domStyle.set(this.pwdErrorSum, "display", "");
                domAttr.set(this.jzBtnCommit, "disabled", true);
            } else if (this.jzReNewPwd.value != this.jzNewPwd.value) {
                this.pwdErrorSum.innerHTML = "发生错误：两次输入的新密码不一样！";
                domStyle.set(this.pwdErrorSum, "display", "");
                domAttr.set(this.jzBtnCommit, "disabled", true);
            } else {
                domStyle.set(this.pwdErrorSum, "display", "none");
                domAttr.set(this.jzBtnCommit, "disabled", false);
            }
        },
        showDelay: function () {
            var that = this;
            baseFx.animateProperty({
                duration: 500,
                node: that.jzPwdDialogPane,
                properties: {
                    opacity: { start: 0, end: 1 }
                }
            }).play();
        },
        hideDelay: function () {
            var that = this;
            baseFx.animateProperty({
                duration: 500,
                node: that.jzPwdDialogPane,
                properties: {
                    opacity: { start: 1, end: 0 }
                },
                onEnd: function () {
                    domStyle.set(that.domNode, "display", "none");
                }
            }).play();
        },
        onOpen: function () {
            if (domStyle.get(this.domNode, "display") !== "none") {
                return;
            } else {
                domStyle.set(this.domNode, "display", "inline");
                this.showDelay();
            }
            this.jzOldPwd.value = "";
            this.jzNewPwd.value = "";
            this.jzReNewPwd.value = "";
        },
        onCancel: function () {
            this.hideDelay();
        },
        onChangePwd: function () {
            var that = this;
            xhr.get("webservice/WebServiceMap.asmx/ChnagePasswd", {
                handleAs: "json",
                timeout: 10000,
                query: { oldPwd:this.jzOldPwd.value, newPwd:this.jzNewPwd.value }
            }).then(function (res) {
                var suc = JSON.parse(res);
                if (!suc.Result) {
                    that.pwdErrorSum.innerHTML = "发生错误：" + suc.Message + "!";
                    domStyle.set(that.pwdErrorSum, "display", "");
                } else {
                    alert("密码修改成功！");
                    that.hideDelay();
                }
            }, function (err) {
                console.log(err);
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
