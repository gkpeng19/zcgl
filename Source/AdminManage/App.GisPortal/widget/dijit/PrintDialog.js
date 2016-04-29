/**
 * @author Administrator
 */
define(['dojo/_base/declare', 'dojo/topic', 'dojo/_base/lang', 'dijit/_WidgetBase', 'dijit/_TemplatedMixin', 'dojo/on',
    'dojo/dom', 'dojo/dom-construct', 'dojo/dom-class', 'dojo/_base/array',
'dojo/dom-style', 'dojo/dom-attr', 'esri/tasks/PrintTask', 'esri/tasks/PrintTemplate', 'esri/tasks/PrintParameters', 'dojo/text!./template/PrintDialog.html'],
function (declare, topic, lang, _WidgetBase, _TemplatedMixin, on, dom, domConstruct, domClass, arrayUtils, domStyle, domAttr, PrintTask, PrintTemplate, PrintParameters, template) {
    var instance = null, clazz;
    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        _open: false,
        pNode: null,
        map: null,
        mixin: false,
        printTask: null,
        postCreate: function () {
            this.titleNode.innerHTML = "地图输出";
            //this.printMapWidth.value = this.map.width;
            //this.printMapHeight.value = this.map.height;
            topic.subscribe("topic/MeasureDialogShow", lang.hitch(this, this.onCancel));
        },
        startup: function () {
            this.inherited(arguments);
            if (!this.mixin) {
                domConstruct.place(this.domNode, this.pNode);
                this.mixin = true;
            } else {
                this.onOpen();
            }
            topic.publish("topic/PrintTaskDialog");
            //this.printTask = new PrintTask(JZ.config["PrintUrl"][0].url);
        },
        onOpen: function () {
            //domStyle.set(this.jzPrintMore, "display", "none");
            //domStyle.set(this.jzPrintMoreChoose, "display", "block");
            if (domStyle.get(this.domNode, "display") !== "none") {
                return;
            } else {
                domStyle.set(this.domNode, "display", "inline");
            }
            //this.printMapWidth.value = this.map.width;
            //this.printMapHeight.value = this.map.height;
        },
        onCancel: function () {
            domStyle.set(this.domNode, "display", "none");
        },
        //onMore: function () {
        //    domStyle.set(this.jzPrintMore, "display", "block");
        //    domStyle.set(this.jzPrintMoreChoose, "display", "none");
        //},
        onPrint: function () {
            //mapDiv_container
            //var nodes = dom.byId('mapDiv').childNodes;
            //arrayUtils.forEach(nodes, function (node) {
            //    if (node.nodeType == 1 && node.getAttribute('id') == 'mapDiv_root') {
            //        arrayUtils.forEach(node.childNodes, function (n) {
            //            if (n.nodeType == 1 && n.getAttribute('id') == 'mapDiv_container') {
            //                domClass.add(n, 'print');
            //            } else {
            //                domClass.add(n, 'noprint');
            //            }
            //        });
            //    } else {
            //        domClass.add(node, 'noprint');
            //    }
            //});
            //window.print();
            var height = window.screen.availHeight || window.screen.availHeight;
            var width = window.screen.availWidth || window.screen.width;
            var popWidth = width - 250;
            var popHeight = height - 150;
            var params = "height=" + popHeight + ",width=" + popWidth + ",top=75,left=125,toolbar=no,menubar=yes,scrollbars=no, resizable=no,location=no, status=no";
            var ext = this.map.extent;
            window.open('pages/map.aspx?id=42&xmin=' + ext.xmin + '&xmax=' + ext.xmax + '&ymin=' + ext.ymin + '&ymax=' + ext.ymax, 'newwindow', params)


            //this.printStartBtn.innerHTML = "正在输出中，请稍后...";
            //domAttr.set(this.printStartBtn, "disabled", true);
            //if (!this.printTask) {
            //    this.printTask = new PrintTask(JZ.config["PrintUrl"][0].url);
            //}
            ////地图宽度
            //var mapWidth = this.printMapWidth.value;
            ////地图高度
            //var mapHeight = this.printMapHeight.value;
            ////地图名称
            //var mapTitle = this.printMapTitle.value;
            ////地图作者
            //var mapAuthor = "作者：" + this.printMapAuthor.value ? this.printMapAuthor.value : "北京市园林绿化局";
            ////版权信息
            //var mapCopyright = "版权信息：" + this.printMapCopyRight.value ? this.printMapCopyRight.value : "北京市园林绿化局";
            ////选取的地图单位
            //var mapUnits = this.printMapUnits.options[this.printMapUnits.selectedIndex].value;
            ////选取的地图的格式
            //var mapFormat = this.printMapFormat.options[this.printMapFormat.selectedIndex].value;
            ////选取的地图模板
            //var mapTemplate = this.printMapTemplate.options[this.printMapTemplate.selectedIndex].value;
            //var printTemplate = new PrintTemplate();
            //printTemplate.format = mapFormat;
            //printTemplate.label = mapTitle;
            //printTemplate.layout = mapTemplate;
            //printTemplate.layoutOptions = {
            //    titleText: mapTitle,
            //    authorText: mapAuthor,
            //    copyrightText: mapCopyright,
            //    scalebarUnit: mapUnits,
            //    legendLayers: []
            //    //customTextElements:[]
            //};
            //printTemplate.exportOptions = {
            //    width: mapWidth,
            //    height: mapHeight,
            //    dpi: 96
            //};
            //printTemplate.outScale = this.map.getScale();
            //var printParam = new PrintParameters();
            //printParam.map = this.map;
            //printParam.outSpatialReference = this.map.spatialReference;
            //printParam.template = printTemplate;
            //this.printTask.execute(printParam, lang.hitch(this, this.success), lang.hitch(this, this.error));
        }
        //success: function (suc) {
        //    this.printStartBtn.innerHTML = "输出成功,是否继续？";
        //    domAttr.set(this.printStartBtn, "disabled", false);

        //    //<iframe width="0" height="0" name="saveFrame" frameborder='no' border='0' allowtransparency='yes' style="display:none;"></iframe>
        //    //if (window.frames &&
        //    //    window.frames["saveFrame"] &&
        //    //    window.frames["saveFrame"].document && 
        //    //    window.frames["saveFrame"].document.execCommand) {
        //    //    window.frames["saveFrame"].location.href = suc.url;
        //    //    window.frames["saveFrame"].document.execCommand('SaveAs');
        //    //} else {
        //        window.open(suc.url);
        //    //}
        //},
        //error: function (err) {
        //    console.log(err);
        //    this.printStartBtn.innerHTML = "输出失败,是否重试？";
        //    domAttr.set(this.printStartBtn, "disabled", false);
        //}
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
