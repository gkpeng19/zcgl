/**
 * @author Administrator
 */
define(['dojo/_base/declare',
    'dojo/dom-class',
    'dojo/_base/array',
    'dojo/_base/lang',
    'dijit/_WidgetBase',
    'dijit/_TemplatedMixin',
    'dojo/request/xhr',
    'dojo/dom-construct',
    'dojo/dom-style',
    'dojo/dom-attr',
    'dojo/dom',
    'dojo/on',
    'dojo/topic',
    'dojo/DeferredList',
    'widget/dijit/UploadProgressHtml',
    'widget/dijit/FileModel',
    'dojo/text!./template/HtmlUploader.html'],
function (declare, domClass, arrayUtils, lang, _WidgetBase, _TemplatedMixin,
    xhr, domConstruct, domStyle, domAttr, dom, on, topic, DeferredList, UploadProgress, FileModel, template) {
    var instance = null, clazz, queuedFiles = [], index = 0;
    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        layerIndex: -1,
        objID: -1,
        uploadedFiles: [],
        fileMode: null,
        startup: function () {
            this.inherited(arguments);
            domConstruct.place(this.domNode, dom.byId("jzUploadFiles").parentNode, 'only');
            this.own(on(this.jzHTMLUploadFile, "change", this.fileHandler));
            //topic.subscribe("topic/HtmlUploadComplete", lang.hitch(this, this.uploadAnother))
            instance = this;
            this.fileMode = new FileModel();
        },
        uploadAnother: function (newFile, flag) {
            if (flag) {
                if (newFile) {
                    this.uploadedFiles.push({ OLD: queuedFiles[index].file.name, NEW: newFile });
                }
            }
            if ((index + 1) < queuedFiles.length) {
                index = index + 1;
                this.fileMode.startup(queuedFiles[index], UploadProgress, this);
            } else {
                this.insertDB();
            }
        },
        fileHandler: function (e) {
            domClass.add(this.parentNode, 'disabled')
            domAttr.set(this, 'disabled', 'disabled');
            queuedFiles = [];
            index = 0;
            instance.uploadedFiles = [];
            var selectFiles = this.files;
            for (var i = 0; i < selectFiles.length; i++) {
                var file = selectFiles[i];
                queuedFiles.push({index:i, file:file});
                UploadProgress.getInstance().startup({ index: i, file: file });
            }
            instance.fileMode.startup(queuedFiles[index], UploadProgress, instance);
        },
        insertDB: function () {
            var list = [];
            var that = this;
            if (that.uploadedFiles.length > 0) {
                JZ.loading.show();
                arrayUtils.forEach(that.uploadedFiles, function (upFile) {
                    var defer = null;
                    var exts = /\.[^\.]+/.exec(upFile.NEW);
                    var fileDes = upFile.OLD.substring(0, upFile.OLD.lastIndexOf('.'));
                    if (exts.length > 0 && (exts[0].toLowerCase() == '.flv' || exts[0].toLowerCase() == '.mp4')) {
                        defer = xhr.get("webservice/WebServiceMap.asmx/InsertVideos", {
                            query: { layerid: that.layerIndex, objid: that.objID, videourl: upFile.NEW, videodes: fileDes },
                            handleAs: "json",
                            timeout: 10000
                        });
                        list.push(defer);
                    } else {
                        defer = xhr.get("webservice/WebServiceMap.asmx/InsertPics", {
                            query: { layerid: that.layerIndex, objid: that.objID, picurl: upFile.NEW, picdes: fileDes },
                            handleAs: "json",
                            timeout: 10000
                        });
                        list.push(defer);
                    }
                });
                var deferredList = new DeferredList(list);
                deferredList.then(function (result) {
                    if (that.uploadedFiles.length < queuedFiles.length) {
                        alert("部分文件没有上传成功，请确保文件大小符合要求后重试！");
                    } else {
                        alert("全部文件上传成功！");
                    }
                    UploadProgress.getInstance().close();
                    topic.publish("topic/UploadFileSuccess");
                });
            } else {
                alert("文件没有上传成功，请确保文件大小符合要求后重试！");
                UploadProgress.getInstance().close();
            }
            if (domAttr.has(that.jzHTMLUploadFile, 'disabled')) {
                domAttr.remove(that.jzHTMLUploadFile, 'disabled');
                domClass.remove(that.jzHTMLUploadFile.parentNode, 'disabled')
            }
        }
    });
    return clazz;
});
