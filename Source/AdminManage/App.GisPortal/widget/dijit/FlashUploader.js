/**
 * @author Administrator
 */
define(['dojo/_base/declare',
    'dojo/_base/array',
    'dojo/request/xhr',
    'dojo/dom-construct',
    'dojo/dom-style',
    'dojo/dom-attr',
    'dojo/dom',
    'dojo/topic',
    'dojo/DeferredList',
    'widget/swfupload/SWFUpload',
    'widget/dijit/UploadProgress'],
function (declare, arrayUtils, xhr, domConstruct, domStyle, domAttr, dom, topic,
    DeferredList, SWFUpload, UploadProgress) {
    var instance = null, clazz, uploadedFiles = [];
    clazz = declare(null, {
        constructor: function (layerIndex, objID) {
            this.layerIndex = layerIndex;
            this.objID = objID;
            this.initUploadBtn();
            instance = this;
        },
        initUploadBtn: function () {
            var settings_object = {
                upload_url: "webservice/Handler.ashx",
                flash_url: "widget/swfupload/swfupload.swf",
                file_post_name: "Filedata",
                post_params: {
                    "post_param_name_1": "post_param_value_1",
                    "post_param_name_2": "post_param_value_2",
                    "post_param_name_n": "post_param_value_n"
                },
                use_query_string: false,
                requeue_on_error: false,
                http_success: [201, 202],
                assume_success_timeout: 0,
                file_types: "*.jpg;*.gif;*.png;*.flv;*.mp4",
                file_types_description: "图片及视频文件",
                file_size_limit: "40980",//最大40M
                file_upload_limit: 10,
                file_queue_limit: 0,
                debug: false,
                prevent_swf_caching: false,
                preserve_relative_urls: false,
                button_placeholder_id: 'jzUploadFiles',
                button_image_url: "widget/swfupload/images/placeholder.png",
                button_width: 82,
                button_height: 34,
                button_text: "上传附件",
                button_text_style: "color:#FFF;height:50px;width:112px;",
                button_text_left_padding: 3,
                button_text_top_padding: 2,
                button_action: SWFUpload.BUTTON_ACTION.SELECT_FILES,
                button_disabled: false,
                button_cursor: SWFUpload.CURSOR.HAND,
                button_window_mode: SWFUpload.WINDOW_MODE.TRANSPARENT,
                swfupload_loaded_handler: this.swfupload_loaded_function,
                file_dialog_start_handler: this.file_dialog_start_function,
                file_queued_handler: this.file_queued_function,
                file_queue_error_handler: this.file_queue_error_function,
                file_dialog_complete_handler: this.file_dialog_complete_function,
                upload_start_handler: this.upload_start_function,
                upload_progress_handler: this.upload_progress_function,
                upload_error_handler: this.upload_error_function,
                upload_success_handler: this.upload_success_function,
                upload_complete_handler: this.upload_complete_function
            };
            var swf = new SWFUpload(settings_object);
        },
        swfupload_loaded_function: function () {
            console.log("swf加载成功！");
        },
        file_dialog_start_function: function () {
            uploadedFiles = [];
            console.log("即将打开选择文件窗口！");
        },
        file_queued_function: function (file) {
            UploadProgress.getInstance().startup(file);
            console.log("即将打开选择文件窗口！");
        },
        file_queue_error_function: function (file, errorCode, message) {
            try {
                if (errorCode === SWFUpload.errorCode_QUEUE_LIMIT_EXCEEDED) {
                    alert("您选择的图片过多，请重新选择！");
                }
                switch (errorCode) {
                    case SWFUpload.QUEUE_ERROR.ZERO_BYTE_FILE:
                        alert("不能上传空文件!");
                        break;
                    case SWFUpload.QUEUE_ERROR.FILE_EXCEEDS_SIZE_LIMIT:
                        alert("文件不能大于40M!");
                        break;
                    case SWFUpload.QUEUE_ERROR.INVALID_FILETYPE:
                        alert("不支持的文件类型！");
                    default:
                        alert("未知错误，请重试！");
                        break;
                }
            } catch (ex) {
                this.debug(ex);
            }
        },
        file_dialog_complete_function: function (numFilesSelected, numFilesQueued) {
            try {
                if (numFilesQueued > 0) {
                    this.startUpload();
                }
                if (numFilesSelected > numFilesQueued) {
                    alert("有" + (numFilesSelected - numFilesQueued) + "个文件大于40M，已被忽略!");
                }
            } catch (ex) {
                this.debug(ex);
            }
        },
        upload_start_function: function (file) {
            console.log("开始上传！");
        },
        upload_progress_function: function (file, bytesLoaded) {
            try {
                var percent = Math.ceil((bytesLoaded / file.size) * 100);
                UploadProgress.getInstance().setProgress(file, percent > 100 ? 100 : percent);
            } catch (ex) {
                this.debug(ex);
            }
        },
        upload_error_function: function (file, errorCode, message) {
            console.log("文件 " + file.name + " 发生错误，错误码：" + errorCode + "，错误信息：" + message);
        },
        upload_success_function: function (file, serverData) {
            try {
                //将原文件名file.name和新文件名serverData保存到数据库
                //先保存下来，等全部上传成功后再提交到数据库
                uploadedFiles.push({ OLD: file.name, NEW: serverData });
            } catch (ex) {
                this.debug(ex);
            }
        },
        upload_complete_function: function (file) {
            try {
                if (this.getStats().files_queued > 0) {
                    this.startUpload();
                } else {
                    JZ.loading.show();
                    var list = [];
                    var that = instance;
                    arrayUtils.forEach(uploadedFiles, function (upFile) {
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
                        alert("全部文件上传成功！");
                        UploadProgress.getInstance().close();
                        topic.publish("topic/UploadFileSuccess");
                    });
                }
            } catch (ex) {
                this.debug(ex);
            }
        }
    });
    return clazz;
});
