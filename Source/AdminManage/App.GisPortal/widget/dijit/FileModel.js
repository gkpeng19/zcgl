/**
 * @author Administrator
 */
define(['dojo/_base/declare',
    'dojo/dom-class',
    'dojo/_base/array',
    'dojo/_base/lang',
    'dojo/request/xhr',
    'dojo/dom-construct',
    'dojo/dom-style',
    'dojo/dom-attr',
    'dojo/dom',
    'dojo/on',
    'dojo/topic',
    'dojo/DeferredList'],
function (declare, domClass, arrayUtils, lang, 
    xhr, domConstruct, domStyle, domAttr, dom, on, topic, DeferredList) {
    var clazz;
    clazz = declare(null, {
        file: null,
        ps: null,
        context: null,

        startup: function (file, ps, instance) {
            this.file = file;
            this.ps = ps;
            this.context = instance;
            this.startUpload();
        },
        startUpload: function () {
            var fd = new FormData();
            fd.append("Filedata", this.file.file);
            var xhr = this.createXHR();
            xhr.send(fd);
        },
        createXHR: function () {
            var xhr = new XMLHttpRequest();
            var timer;
            xhr.upload.addEventListener("progress", lang.hitch(this, "_xhrProgress"), false);
            xhr.addEventListener("load", lang.hitch(this, "_xhrProgress"), false);
            xhr.addEventListener("error", lang.hitch(this, function (evt) {
                //this.onError(evt);
                //提示用户发生错误，开始下一次上传
                clearInterval(timer);
            }), false);
            xhr.addEventListener("abort", lang.hitch(this, function (evt) {
                //this.onAbort(evt);
                clearInterval(timer);
            }), false);
            xhr.onreadystatechange = lang.hitch(this, function () {
                if (xhr.readyState === 4) {
                    clearInterval(timer);
                    try {
                        if (xhr.status == 200) {
                            this.context.uploadAnother(xhr.responseText, true);
                        } else {
                            this.ps.getInstance().setError(this.file.index);
                            this.context.uploadAnother(xhr.responseText, false);
                        }
                    } catch (e) {
                        var msg = "错误信息如下：";
                        console.error(msg, e);
                        console.error(xhr.responseText);
                    }
                }
            });
            xhr.open("POST", "webservice/Handler.ashx");
            xhr.setRequestHeader("Accept", "application/json");

            return xhr;
        },

        _xhrProgress: function (evt) {
            if (evt.lengthComputable) {
                var o = {
                    bytesLoaded: evt.loaded,
                    bytesTotal: evt.total,
                    type: evt.type,
                    timeStamp: evt.timeStamp
                };
                if (evt.type == "load") {
                    o.percent = "100%";
                    o.decimal = 1;
                } else {
                    o.decimal = evt.loaded / evt.total;
                    o.percent = Math.ceil((evt.loaded / evt.total) * 100);
                }
                this.ps.getInstance().setProgress(this.file.index, o.percent > 100 ? 100 : o.percent);
            }
        }
    });
    return clazz;
});
