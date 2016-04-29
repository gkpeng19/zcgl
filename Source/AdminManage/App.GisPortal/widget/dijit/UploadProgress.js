/**
 * @author Administrator
 */
define(['dojo/_base/declare', 'dojo/_base/lang', 'dojo/topic', 'dojo/parser', 'dijit/_WidgetBase',
    'dojo/_base/array', 'widget/swfupload/SWFUpload',
    'dijit/_TemplatedMixin', 'dojo/on', 'dojo/dom', 'dojo/dom-construct', 'dojo/dom-style',
    'esri/dijit/Measurement', 'esri/units', 'dojo/text!./template/UploadProgress.html'],
function (declare, lang, topic, parser, _WidgetBase, arrayUtils, SWFUpload, _TemplatedMixin, on, dom, domConstruct, domStyle, Measurement, Units, template) {
    var instance = null, clazz;
    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        _open: false,
        files: [],
        startup: function (file) {
            this.files.push(file);
            this.inherited(arguments);
            if (!this.mixin) {
                domConstruct.create('link', { type: 'text/css', rel: 'stylesheet', href: 'widget/dijit/css/UploadProgress.css' }, document.getElementsByTagName('head')[0]);
                domConstruct.place(this.domNode, document.body);
                this.mixin = true;
            } else {
                this.onOpen();
            }
            this.createProgressNode(file);
        },
        
        getFileSize: function (fsize) {
            var size = parseInt(fsize);
            var fileSize = "";
            if(size != 0){
                if(size >= 1073741824){
                    fileSize = (size / 1073741824).toFixed(2).toString() + "GB";
                }else if(size >= 1048576){
                    fileSize = (size / 1048576).toFixed(2).toString() + "MB";
                }else if(size >= 1024){
                    fileSize = (size / 1024).toFixed(2).toString() + "KB";
                }else{
                    fileSize = size.toFixed(2).toString() + "bytes";
                }
            }
            return fileSize;
        },
        createProgressNode: function (file) {
            var fileSize = this.getFileSize(file.size);
            var htmlStr = "<div class='jz-progress-item'><div class='jz-progress-col1' style='width:44%'>" + file.name + "</div><div class='jz-progress-col1' style='width:17%'>" + fileSize + "</div><div id='file_item_" + file.index + "' class='jz-progress-col1' style='width:13%'></div><div id='file_oper_" + file.index + "' class='jz-progress-col1' style='color:#F00'>上传中...</div><div id='file_progress_" + file.index + "' class='progressor'></div></div>";
            domConstruct.place(htmlStr, this.containerNode, "last");
        },
        setProgress: function (file, percent) {
            dom.byId("file_progress_" + file.index).style.width = percent + '%';
            dom.byId("file_item_" + file.index).innerHTML = percent + '%';
            if (percent >= 100) {
                dom.byId("file_oper_" + file.index).innerHTML = '上传完成';
            }
        },
        close: function () {
            domConstruct.empty(this.containerNode);
            this.onCancel();
        },
        onOpen: function () {
            domStyle.set(this.domNode, "display", "");
        },
        onCancel: function () {
            domStyle.set(this.domNode, "display", "none");
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
