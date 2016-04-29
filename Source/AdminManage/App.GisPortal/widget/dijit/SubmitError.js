/**
 * @author Administrator
 */
define(['dojo/_base/declare', 'dojo/topic', 'dojo/_base/lang', 'dijit/_WidgetBase', 'dijit/_TemplatedMixin', 'dojo/request/xhr', 'dojo/on', 'dojo/dom', 'dojo/dom-construct',
'dojo/dom-style', 'dojo/dom-attr', 'dojo/text!./template/SubmitError.html'],
function (declare, topic, lang, _WidgetBase, _TemplatedMixin, xhr, on,
    dom, domConstruct, domStyle,
    domAttr, template) {
    var instance = null, clazz;
    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        map: null,
        pNode: null,
        postCreate: function () {
            this.titleNode.innerHTML = "纠错填报";
        },
        startup: function (layerID, layerCode, layerName, eleID, eleName) {
            this.inherited(arguments);
            if (!this.mixin) {
                domConstruct.place(this.domNode, this.pNode);
                this.mixin = true;
            } else {
                this.onOpen();
            }
            this.layerId.value = layerID;
            this.layerCode.value = layerCode;
            this.layerName.value = layerName;
            this.eleId.value = eleID;
            this.eleName.value = eleName;
        },
        onOpen: function () {
            this.errorDesc.value = "";
            if (domStyle.get(this.domNode, "display") !== "none") {
                return;
            } else {
                domStyle.set(this.domNode, "display", "inline");
            }
        },
        onSubmit: function () {
            domStyle.set(this.domNode, "display", "none");

            xhr.get("webservice/WebServiceMap.asmx/SubmitLayerError", {
                handleAs: "json",
                timeout: 10000,
                query: { layerid: this.layerId.value, layercode: this.layerCode.value, layername: this.layerName.value, eleid: this.eleId.value, elename: this.eleName.value, errordesc: this.errorDesc.value.trim(), ran: Math.random() }
            }).then(function (suc) {
                if (suc != "1") {
                    console.log("保存失败，请重试！错误码：" + suc);
                }
            }, function (err) {
                console.log(err);
            });
        },
        onCancel: function () {
            domStyle.set(this.domNode, "display", "none");
        }
    });

    clazz.getInstance = function () {
        if (instance === null) {
            instance = new clazz({
                map: JZ.mc,
                pNode: "mapDiv"
            });
        }
        return instance;
    };
    return clazz;
});
