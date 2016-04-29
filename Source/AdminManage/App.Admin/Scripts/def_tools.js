//根据测试结果设置表格的颜色
function SetColorByTestResult(gridName) {
    var ids = jQuery(gridName).jqGrid('getDataIDs');
    for (var i = 0; i < ids.length; i++) {

        //设置测试状态,未通过：红色,通过：蓝色，未测试：黑色
        var r = ids[i].replace(".", "\\.");
        var rowData = jQuery(gridName).jqGrid('getRowData', ids[i]);
        if (rowData.Result.toLowerCase() == "true") {
            $("#" + r + " td").css("color", "#008800");
        }
        if (rowData.Result.toLowerCase() == "false") {
            $("#" + r + " td").css("color", "#ff0000");
        }
    }
};
////////////////////////////////////////////////////////////////////
//jgGrid辅肋类
function jgGridAssistant() {
    //property
    this.curId = null; //当前行
    this.MAXLENGTH = 8000;
    //function
    if (typeof jgGridAssistant._initialized == "undefined") {
        jgGridAssistant._initialized = true;
        //////////////////////////////////////
        //关于
        jgGridAssistant.prototype.About = function () {
            alert("jgGridAssistant 1.0");
        };
        //新增
        jgGridAssistant.prototype.Create = function () {
            alert("未实现 1.0");
        };
        //删除
        jgGridAssistant.prototype.Delete = function () {

            alert("未实现 1.0");
        };
        //保存
        jgGridAssistant.prototype.Save = function () {

            alert("未实现 1.0");
        };
        //显示jqGrid信息窗口
        jgGridAssistant.prototype.ShowInfoDialog=function() {
            $("#info_dialog").css("top", 150).css("left", 300);
        };
    }
}

//创建jgGrid辅肋类
var jgAst=new jgGridAssistant();
////////////////////////////////////////////////////////////////////////



//导航表格
function RowNav(gridName) {

    //页面导航
    this.grid = jQuery(gridName);
    //function
    if (typeof RowNav._initialized == "undefined") {
        RowNav._initialized = true;

        RowNav.prototype.About = function () {
            alert("RowNav 1.0");
        };
        //////////////////////////////////////
        RowNav.prototype.FirstRow = function () {

            var ids = this.grid.jqGrid('getDataIDs');
            if (ids.length > 0) {
                this.grid.jqGrid('resetSelection');
                this.grid.jqGrid('setSelection', ids[0]);
            }

        }
        RowNav.prototype.PriorRow = function () {
            var ids = this.grid.jqGrid('getDataIDs');
            if (ids.length < 1) {
                return;
            }
            var curId = this.grid.jqGrid('getGridParam', 'selrow');
            if (curId == null) {
                this.grid.jqGrid('setSelection', ids[0]);
                return;
            }
            for (var i = 1; i < ids.length; i++) {
                var id = ids[i];
                if (curId == id) {
                    this.grid.jqGrid('resetSelection');
                    this.grid.jqGrid('setSelection', ids[i - 1]);
                    return;
                }

            }
        }
        RowNav.prototype.NextRow = function () {
            var ids = this.grid.jqGrid('getDataIDs');
            if (ids.length < 1) {
                return;
            }
            var curId = this.grid.jqGrid('getGridParam', 'selrow');
            if (curId == null) {
                this.grid.jqGrid('setSelection', ids[0]);
                return;
            }
            for (var i = 0; i < ids.length - 1; i++) {
                var id = ids[i];
                if (curId == id) {
                    this.grid.jqGrid('resetSelection');
                    this.grid.jqGrid('setSelection', ids[i + 1]);
                    return;
                }

            }
        }
        RowNav.prototype.LastRow = function () {

            var ids = this.grid.jqGrid('getDataIDs');
            if (ids.length > 0) {
                this.grid.jqGrid('resetSelection');
                this.grid.jqGrid('setSelection', ids[ids.length - 1]);
            }

        }

    }

};
