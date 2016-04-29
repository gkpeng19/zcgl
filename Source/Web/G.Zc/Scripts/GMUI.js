var staticContent = null;//全局Json变量

$.fn.extend({
    initSelect: function (dsource) {
        var target = this;
        target.find("option").remove();
        var defaultOption = target.attr("data-default-option");
        if (defaultOption) {
            defaultOption = eval('(' + defaultOption + ')');
            target.append("<option value='" + defaultOption.value + "'>" + defaultOption.text + "</option>");
        }

        if (dsource.length == 0) {
            return;
        }

        for (var i = 0; i < dsource.length; ++i) {
            var json = dsource[i];
            target.append("<option value='" + json.k + "'" + (json.c ? "data-c='" + json.c + "'" : "") + ">" + json.v + "</option>");
        }

        var defaultValue = target.attr("data-value");
        if (defaultValue) {
            if (target.attr("multiple")) {
                target.val(defaultValue.split(','))
            }
            else {
                target.val(defaultValue);
            }
        }
    },
    gmSelect: function (option) {
        var target = this;

        if (option.url) {//从url加载数据
            $api.get(option.url, option.search, function (r) {
                target.initSelect(r);
                target.trigger("change");
            });
        }
        else if (option.sourcekey) {//从staticContent加载数据
            var dsource = staticContent[option.sourcekey];
            if (dsource) {
                if (!option.pselector) {
                    target.initSelect(dsource);
                }
                else {
                    target.attr("data-ischild", 1);
                    $(option.pselector).bind("change", function () {
                        var fvalue = this.value;
                        var tsource = [];
                        for (var i = 0; i < dsource.length; ++i) {
                            if (dsource[i][option.fkey] == fvalue) {
                                tsource.push(dsource[i]);
                            }
                        }
                        target.initSelect(tsource);
                        target.trigger("change");
                    });
                }
            }
        }
    },
    gmTable: function (option) {
        if (!option.url) {
            return;
        }

        var target = this;

        if (!option.psize) {
            option.psize = 10;
        }
        if (!option.search || option.search == '#') {
            option.search = {};
        }

        if (!option.pselector) {
            target.datagrid({ url: option.url, search: option.search, psize: option.psize, multiple: option.multiple });
        }
        else {
            target.datagrid({ psize: option.psize, multiple: option.multiple });
            var parentEles = $(option.pselector);
            if (parentEles.hasClass("gm-table")) {
                //注册主表查询选择变化事件
                $(option.pselector).datagrid("onSelectChanged", function (json) {
                    if (typeof (option.search) != "object") {
                        option.search = { searchid: option.search };
                    }
                    option.search[option.fkey] = json.id;
                    target.datagrid({ url: option.url, search: option.search });
                });
            }
            else if (parentEles.hasClass("gm-tree")) {
                //注册树控件选择变化事件
                parentEles.gtree("selectChanged", function (json) {
                    var nids = '-1';
                    $(json.children_d).each(function () {
                        nids += (',' + this);
                    });
                    if (typeof (option.search) != "object") {
                        option.search = { searchid: option.search };
                    }
                    option.search[option.fkey] = nids;
                    target.datagrid({ url: option.url, search: option.search });
                });
            }
        }

        //新增事件
        target.parent().prev().find(".btn-table-add").bind('click', function () {
            if (option.pselector) {
                var sels = $(option.pselector).datagrid("getSelecteds");
                if (sels.length == 0 || sels.length > 1) {
                    $.error("请选择一条主表数据！");
                    throw new Error();
                }
            }
            var editDiv = $("#" + option.editdivid);
            var entity = target.triggleEvent("beforeAdd");
            if (!entity) {
                entity = {};
            }
            editDiv.bindEntity(entity);
            editDiv.open("新增", function (win) {
                if (!tt.validateForm(option.editdivid)) {
                    return false;
                }

                var update = editDiv.getContext();
                if (option.pselector) {
                    update[option.fkey] = $(option.pselector).datagrid("getSelecteds")[0].id;
                }
                //触发beforeSave事件，可以在保存前维护必要的内容
                var rup = target.triggleEvent("beforeSave", update);
                if (rup) {
                    update = rup;
                }
                $api.post(option.url, update, function (r) {
                    r = $.toJsResult(r);
                    if (r.id > 0) {
                        r = $.combineJson(update, r);
                        target.datagrid("addRow", r);
                        layer.close(win);

                        target.triggleEvent("afterEditWinClosed");
                    }
                    else {
                        $.error("保存失败，请重试！");
                    }
                });

                return false;
            });
            target.triggleEvent("afterEditWinOpened");
        });

        //编辑事件
        target.datagrid("onEdit", function () {
            var json = target.datagrid("getCurrData");
            target.triggleEvent("beforeEdit", json);

            var editDiv = $("#" + option.editdivid);
            editDiv.bindEntity(json);
            editDiv.open("编辑", function (win) {
                if (!tt.validateForm(option.editdivid)) {
                    return false;
                }

                var update = editDiv.getContext();
                //触发beforeSave事件，可以在保存前维护必要的内容
                var rup = target.triggleEvent("beforeSave", update);
                if (rup) {
                    update = rup;
                }
                $api.post(option.url, update, function (r) {
                    r = $.toJsResult(r);
                    if (r.id > 0) {
                        //var newjson = $.combineJson(json, update, r);
                        //target.datagrid("updateRow", json, newjson);
                        target.datagrid("refresh");
                        layer.close(win);

                        target.triggleEvent("afterEditWinClosed");
                    }
                    else {
                        $.error("保存失败，请重试！");
                    }
                });

                return false;
            });
            target.triggleEvent("afterEditWinOpened");
        });

        //删除事件
        target.datagrid("onDelete", function () {
            $.confirm("确定要删除吗？", function () {
                var json = target.datagrid("getCurrData");
                $api.delete(option.url, { id: json["id"] }, function (r) {
                    if (r > 0) {
                        target.datagrid("removeCurrRow");
                    }
                    else {
                        $.error("删除失败，请重试！");
                    }
                });
            });
        });
    },
    gmOutInput: function (option, onSelFunc) {
        if (!option.oselector) {
            return;
        }

        var target = this;

        if (option.isnewpage != true) {
            option.isnewpage = false;
        }

        if (option.showkey) {
            option.showkey = option.showkey.toLowerCase();
        }

        var codeName = target.attr("data-excode");
        var nameName = target.attr("data-exname");

        target.css({ border: "none", padding: '0px', width: '0px' });
        var id = target.attr("id");
        target.removeAttr("id");
        var fCode = $('<input type="text" style="display:none;" ' + (codeName ? 'name="' + codeName + '"' : '') + ' />');
        var fName = $('<input type="text" readonly="readonly" class="span11" id="' + id + '" ' + (nameName ? 'name="' + nameName + '"' : '') + ' />');
        var fOutBtn = $('<label class="btn btn-primary btn-small" style="float:right;margin-top:-37px;right:28px;">&nbsp;选&nbsp;择&nbsp;</label>');
        var fContainer = $("<span></span>");
        fContainer.append(fCode).append(fName).append(fOutBtn).insertAfter(target);

        var submitFun = function (selectRes) {
            if (selectRes.length == 0) {
                target.val('');
                fCode.val('');
                fName.val('');
            }
            else {
                var valueStr = "";
                var codeStr = "";
                var showStr = "";
                $(selectRes).each(function (idx) {
                    if (idx == 0) {
                        valueStr += this.id;
                        codeStr += this.code;
                        showStr += this.name;
                    }
                    else {
                        valueStr += ("," + this.id);
                        codeStr += ("," + this.code);
                        showStr += ("," + this.name);
                    }
                });
                target.val(valueStr);
                fCode.val(codeStr);
                fName.val(showStr);
            }
        };

        fOutBtn.bind("click", function () {
            if (option.isnewpage) {
                $("#" + option.oselector).find("iframe").open("请选择", function () {
                    var table = $(window.frames[0].document.body).find("table:first");
                    if (table.length > 0) {
                        var grid = window.frames[0].gExtends.datagrid[table[0].id];
                        if (grid) {
                            if (onSelFunc) {
                                var rs = onSelFunc(grid["getSelecteds"]());
                                submitFun(rs);
                            }
                            else {
                                submitFun(grid["getSelecteds"]());
                            }
                        }
                    }
                });
            }
            else {
                $("#" + option.oselector).parent().parent().open("请选择", function () {
                    var table = $("#" + option.oselector);
                    if (table.length > 0) {
                        if (onSelFunc) {
                            var rs = onSelFunc(table.datagrid("getSelecteds"));
                            submitFun(rs);
                        }
                        else {
                            submitFun(table.datagrid("getSelecteds"));
                        }
                    }
                });
            }
        });
    }
});

function setStaticContent(json) {
    staticContent = json;
    $(".gm-select").each(function () {
        var option = $(this).attr("data-option");
        if (!option) {
            return;
        }
        option = eval('(' + option + ')');//{sourcekey:数据源key,pselector:父selector,fkey:fkey}
        $(this).gmSelect(option);
    });
    $(".gm-select").each(function () {
        $(this).trigger("change");
    });

    $(".gm-form").each(function () {
        var option = $(this).attr("data-option");
        if (!option) {
            return;
        }
        option = eval('(' + option + ')');//{sourcekey:数据源key}
        if (!option.sourcekey) {
            return;
        }
        if (option.sourcekey.toLowerCase() == 'staticcontent') {
            $(this).bindEntity($.toJsResult(staticContent));
        }
        else {
            var dsource = staticContent[option.sourcekey];
            if (dsource) {
                $(this).bindEntity($.toJsResult(dsource));
            }
        }
    });
}

/*--------------界面初始化-Start---------------------------*/

/*tree class='gm-tree'*/
$(".gm-tree").each(function () {
    var target = $(this);

    var option = target.attr("data-option");
    if (!option) {
        return;
    }
    option = eval('(' + option + ')');//{url:url,checkbox:true,editdivid:编辑框id,reftableid:与树关联的表格插件id,parentidfield:子节点与父节点的关联字段,textField:textField}
    if (!option.url) {
        return;
    }
    target.gtree({ url: option.url, checkbox: option.checkbox });

    if (option.reftableid && option.reftableid.length > 0) {
        var refTable = $("#" + option.reftableid);
        if (refTable.length > 0) {
            target.gtree("beforeDeleteNode", function () {
                if (refTable.datagrid("hasChildren")) {
                    $.error("请先删除该节点下的内容！");
                    return false;
                }
                refTable.datagrid("refresh");
                if (refTable.datagrid("hasChildren")) {
                    $.error("请先删除该节点下的内容！");
                    return false;
                }
                return true;
            });
        }
    }

    //新增根节点
    target.prev().find(".btn-add-treeroot").bind("click", function () {
        var editDiv = $("#" + option.editdivid);
        editDiv.bindEntity({});
        editDiv.open("新增根节点", function () {
            if (!tt.validateForm(option.editdivid)) {
                return false;
            }
            var update = editDiv.getContext();
            $api.post(option.url, update, function (r) {
                r = $.toJsResult(r);
                if (r.id > 0) {
                    target.gtree("addRootNode", { id: r.id, text: r[option.textField] });//初始化树节点数据并添加节点
                    layer.close(win);
                }
                else {
                    $.error("保存失败，请重试！");
                }
            });

            return false;
        });
    });

    //新增节点
    target.prev().find(".btn-add-treechild").bind("click", function () {
        var snode = target.gtree("getSelected");
        if (snode) {
            var editDiv = $("#" + option.editdivid);
            editDiv.bindEntity({});
            editDiv.open("新增节点", function () {
                if (!tt.validateForm(option.editdivid)) {
                    return false;
                }
                var update = editDiv.getContext();
                update[option.parentidfield] = snode.id;
                $api.post(option.url, update, function (r) {
                    r = $.toJsResult(r);
                    if (r.id > 0) {
                        target.gtree("addChildNode", { id: r.id, parentid: snode.id, text: r[option.textField] });
                        layer.close(win);
                    }
                    else {
                        $.error("保存失败，请重试！");
                    }
                });

                return false;
            });
        }
    });

    //编辑节点
    target.prev().find(".btn-edit-treenode").bind('click', function () {
        var snode = target.gtree("getSelected");
        if (snode) {
            var editDiv = $("#" + option.editdivid);
            editDiv.bindEntity(snode);
            editDiv.open("编辑节点", function () {
                if (!tt.validateForm(option.editdivid)) {
                    return false;
                }
                var update = editDiv.getContext();
                update.id = snode.id;
                $api.post(option.url, update, function (r) {
                    r = $.toJsResult(r);
                    if (r.id > 0) {
                        target.gtree("updateSelectNode", { id: r.id, parentid: snode.parentid, text: r[option.textField] });//初始化树节点数据并修改节点数据
                        layer.close(win);
                    }
                    else {
                        $.error("保存失败，请重试！");
                    }
                });

                return false;
            });
        }
    });

    //删除节点
    target.prev().find(".btn-del-treenode").bind('click', function () {
        var snode = target.gtree("getSelected");
        if (snode) {
            $.confirm("确定要删除该节点吗？", function () {
                if (target.gtree("removeSelectNode")) {
                    $api.delete(option.url, { ID: snode.id }, function (r) { });
                }
            });
        }
    });
});

/*table class='gm-table'*/
$(".gm-table").each(function () {
    var target = $(this);

    var option = target.attr("data-option");
    if (!option) {
        return;
    }
    option = eval('(' + option + ')');//{url:url,search:selector/{...,searchid:selector},psize:psize,editdivid:editdivid,multiple:true,pselector:pselector,fkey:fkey}
    target.gmTable(option);
});

/*validate class='gm-valider'*/
$(".gm-valid").each(function () {
    var target = $(this);

    var option = $(this).attr("data-valid");
    if (!option) {
        return;
    }
    if (!this.id || this.id.length == 0) {
        return;
    }
    option = eval('(' + option + ')');//{valids:[{vtype:验证类型,minlen:minlen,maxlen:maxlen,regstr:regstr}],islayerout:true}
    var fer = new tt.Field(target.prev().text(), "", this.id);
    if (option.islayerout != false) {
        fer.setMsgId("layer-error-msg");
    }
    if (option.valids.length > 0) {
        $(option.valids).each(function () {
            switch (this.type) {
                case "required":
                    tt.vf.req.add(fer);
                    break;
                case "number":
                    tt.vf.num.add(fer);
                    break;
                case "numint":
                    tt.vf.int.add(fer);
                    break;
                case "email":
                    tt.vf.email.add(fer);
                    break;
                case "ipaddress":
                    tt.vf.ip.add(fer);
                    break;
                case "postcode":
                    tt.vf.postcode.add(fer);
                    break;
                case "telnum":
                    tt.vf.tel.add(fer);
                    break;
                case "phone":
                    new tt.RV().set(new RegExp("^(0|86|17951)?(13[0-9]|15[012356789]|17[678]|18[0-9]|14[57])[0-9]{8}$"), "数据格式输入错误").add(fer);
                    break;
                case "idcard":
                    tt.vf.idcard.add(fer);
                    break;
                case "lenvalidate":
                    new tt.LV().set(this.minlen, this.maxlen).add(fer);
                    break;
                case "regvalidate":
                    new tt.RV().set(new RegExp(this.regstr), "数据格式输入错误").add(fer);
                    break;
            }
        });
    }
});

/*outInput class='gm-outinput'*/
$(".gm-outinput").each(function () {
    var target = $(this);

    var option = $(this).attr("data-option");
    if (!option) {
        return;
    }
    option = eval('(' + option + ')');//{oselector:弹出层的id,showkey:showkey,isnewpage:isnewpage}
    if (!option.oselector) {
        return;
    }

    if (option.isnewpage != true) {
        option.isnewpage = false;
    }

    if (option.showkey) {
        option.showkey = option.showkey.toLowerCase();
    }

    target.css({ border: "none", padding: '0px', width: '0px' });
    var id = target.attr("id");
    target.removeAttr("id");
    var fInput = $('<input type="text" readonly="readonly" class="span11" id="' + id + '" />');
    var fOutBtn = $('<label class="btn btn-primary btn-small" style="float:right;margin-top:-37px;right:28px;">&nbsp;选&nbsp;择&nbsp;</label>');
    var fContainer = $("<span></span>");
    fContainer.append(fInput).append(fOutBtn).insertAfter(target);

    var submitFun = function (selectRes) {
        if (selectRes.length == 0) {
            target.val('');
            fInput.val('');
        }
        else {
            var valueStr = "";
            var showStr = "";
            $(selectRes).each(function (idx) {
                if (idx == 0) {
                    valueStr += this.id;
                    showStr += this[option.showkey];
                }
                else {
                    valueStr += ("," + this.id);
                    showStr += ("," + this[option.showkey]);
                }
            });
            target.val(valueStr);
            fInput.val(showStr);
        }
    };

    fOutBtn.bind("click", function () {
        if (option.isnewpage) {
            $("#" + option.oselector).find("iframe").open("请选择", function () {
                var table = $(window.frames[0].document.body).find("table:first");
                if (table.length > 0) {
                    var grid = window.frames[0].gExtends.datagrid[table[0].id];
                    if (grid) {
                        submitFun(grid["getSelecteds"]());
                    }
                }
            });
        }
        else {
            $("#" + option.oselector).parent().parent().open("请选择", function () {
                var table = $("#" + option.oselector);
                if (table.length > 0) {
                    submitFun(table.datagrid("getSelecteds"));
                }
            });
        }
    });
});

/*--------------界面初始化-End---------------------------*/