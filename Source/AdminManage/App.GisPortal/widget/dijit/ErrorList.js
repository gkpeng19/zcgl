/**
 * @author Administrator
 */
define(['dojo/_base/declare', 'dojo/topic', 'dojo/_base/lang', 'dijit/_WidgetBase', 'dijit/_TemplatedMixin',
    'dojo/request/xhr', 'dojo/on', 'dojo/dom', 'dojo/dom-construct', 'dojo/_base/array', 'esri/tasks/query', 'esri/tasks/QueryTask',
'dojo/dom-style', 'dojo/dom-attr', 'dojo/query', 'widget/utils/GraphicUtils', 'dojo/text!./template/ErrorList.html'],
function (declare, topic, lang, _WidgetBase, _TemplatedMixin, xhr, on,
    dom, domConstruct, arrayUtils, Query, QueryTask, domStyle,
    domAttr, query, GraphicUtils, template) {
    var instance = null, clazz;
    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        map: null,
        pNode: null,
        postCreate: function () {
            this.titleNode.innerHTML = "错误列表";
        },
        startup: function () {
            this.initErrorListTable(0);
			
			this.cbk_no.checked=true;
			
            this.inherited(arguments);
            if (!this.mixin) {
                domConstruct.place(this.domNode, this.pNode);
                this.mixin = true;
            } else {
                this.onOpen();
            }
        },
        onOpen: function () {
            if (domStyle.get(this.domNode, "display") !== "none") {
                return;
            } else {
                domStyle.set(this.domNode, "display", "inline");
            }
        },
        onCancel: function () {
            domStyle.set(this.domNode, "display", "none");
        },
        onSelectedYes: function () {
            this.initErrorListTable(1);
        },
        onSelectedNo: function () {
            this.initErrorListTable(0);
        },
        initErrorListTable: function (status) {
            var target = this;

            //移除旧的tbody
            var tbodys = target.tbl_errorlist.getElementsByTagName("tbody");
            if (tbodys.length > 0) {
                target.tbl_errorlist.removeChild(tbodys[0]);
            }

            //处理表头
            //var op = document.getElementById("operator");
            //if (!op && status == 0) {
                //op = document.createElement("th");
                //op.id = "operator";
                //op.innerText = "操作";
                //this.tbl_errorlist.getElementsByTagName("thead")[0].getElementsByTagName("tr")[0].appendChild(op);
            //}
            //else if (op && status == 1) {
               // this.tbl_errorlist.getElementsByTagName("thead")[0].getElementsByTagName("tr")[0].removeChild(op);
            //}


            //初始化表格数据
            xhr.get("webservice/WebServiceMap.asmx/GetErrorList", {
                handleAs: "json",
                timeout: 20000,
                query: { status: status, ran: Math.random() }
            }).then(function (suc) {
                var html = "";
                for (var i in suc) {
                    html += "<tr>";
					html += "<td>" + suc[i].LayerName + "</td><td>" + suc[i].EleName + "</td><td>" + suc[i].ErrorDesc + "</td><td>" + suc[i].ADDBY + "</td><td>" + suc[i].AddOn_G + "</td>" +
                        (status == 1 ? ("<td><a href='javascript:void(0);' data-pid='" + suc[i].LayerID + "' data-oid='" + suc[i].EleID + "' class='one'>定位</a></td>") : ("<td><a href='javascript:void(0);' data-pid='" + suc[i].LayerID + "' data-oid='" + suc[i].EleID + "' class='one'>定位</a></br><a href='javascript:void(0);' class='two' data-id='" + suc[i].ID + "'>确认处理</a></td>"));
                    html += "</tr>";
                }
                if (suc.length > 0) {
                    var div = document.createElement('div');
                    div.innerHTML = "<table><tbody>" + html + "</tbody></table>";
                    var btnTwos = div.getElementsByClassName("two");
					for(var i in btnTwos)
					{
                        var btnTwo = btnTwos[i];
                        btnTwo.onclick = function () {
                            target.onTwo(this.attributes["data-id"].nodeValue);
                        };
					}

                    target.tbl_errorlist.appendChild(div.firstChild.firstChild);

                    query(".jz-errorlist-dialog-content .one").on("click", function (e) {
                        var node = e.target ? e.target : e.srcElement;
                        var pid = domAttr.get(node, "data-pid");
                        var oid = domAttr.get(node, "data-oid");

                        target.showGraphic(pid, oid);
                    });
                }
            }, function (err) {
                console.log(err);
            });
        },
        showGraphic: function (pid, oid) {
            arrayUtils.forEach(JZ.privilegeLayers, function (pLayer) {
                var oLayers = pLayer.LAYERS;
                arrayUtils.forEach(oLayers, function (layer) {
                    if (layer.id == parseInt(pid, 10)) {
                        //开始查询,并定位
                        var queryTask = new QueryTask(layer.mapServerUrl + "/" + layer.serverindex);
                        var query = new Query();
                        query.outSpatialReference = JZ.mc.spatialReference;
                        query.returnGeometry = true;
                        query.outFields = ["*"];
                        query.where = "OBJECTID=" + parseInt(oid, 10);
                        queryTask.execute(query).then(function (suc) {
                            //alert(suc);
                            //开始定位
                            
                            require(['widget/utils/GraphicUtils', 'widget/prop/PropInfo'], function (clazz, PropertyInfo) {
                                clazz.getInstance().addGraphic(JZ.mc.graphics, suc.features[0].geometry, suc.features[0].attributes);
                                var infoWin = new PropertyInfo(JZ.mc, suc.features[0], parseInt(pid, 10));
                                infoWin.show();
                            });
                        }, function (err) {
                            //alert();
                        });
                    }
                });
            });
        },
        onTwo: function (id) {
            var target = this;
            //改变状态为已处理
            xhr.get("webservice/WebServiceMap.asmx/ChangeErrorStatus", {
                handleAs: "json",
                timeout: 20000,
                query: { id: id, ran: Math.random() }
            }).then(function (suc) {
                if (suc == "1") {
                    target.initErrorListTable(0);
                }
            }, function (err) {
                console.log(err);
            });
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
