/**
 * @author Administrator
 */
define(['dojo/_base/declare', 'dojo/_base/lang', 'dojo/topic', 'dojo/parser', 'dijit/_WidgetBase',
    'dojo/_base/array', 'widget/swfupload/SWFUpload', 'dojo/DeferredList', 'dojo/request/xhr',
    'dijit/_TemplatedMixin', 'dojo/on', 'dojo/dom', 'dojo/dom-construct', 'dojo/dom-style',
    'widget/dijit/UploadProgress', 'dojo/text!./template/Attachments.html'],
function (declare, lang, topic, parser, _WidgetBase, arrayUtils, SWFUpload, DeferredList, xhr,
    _TemplatedMixin, on, dom, domConstruct, domStyle, UploadProgress, template) {
    var instance = null, clazz, uploadedFiles = [];

    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        _open: false,
        layerID: -1,
        objID: -1,
        results: [],
        postCreate: function () {
            this.titleNode.innerHTML = "附件管理";
        },
        startup: function (layerID, objID) {
            this.layerID = layerID;
            this.objID = objID;
            this.inherited(arguments);
            if (!this.mixin) {
                domConstruct.place(this.domNode, 'mapDiv');
                this.mixin = true;
                this.initUploadBtn();
            } else {
                this.onOpen();
            }
            uploadedFiles = [];
            this.getAttachments();
        },
        getAttachments: function () {
            JZ.loading.show();
            var that = this;
            var d1 = xhr.get("webservice/WebServiceMap.asmx/GetPics", {
                query: { id: that.layerID, objid: that.objID },
                handleAs: "json",
                timeout: 10000
            });
            var d2 = xhr.get("webservice/WebServiceMap.asmx/GetVideos", {
                query: { id: that.layerID, objid: that.objID },
                handleAs: "json",
                timeout: 10000
            });
            var deferredList = new DeferredList([d1, d2]);
            deferredList.then(function (result) {
                that.results = result;
                that.createNode();
                JZ.loading.hide();
            });
        },
        //创建
        createNode: function () {
            //this.jzAttachList
            var that = this;
            var htmlStr = "";
            //var htmlStr = "<div class='item clearfix'><div class='col c1' style='width: 45%;'><span class='chk'><span class='chk-ico'></span></span><div class='name'><span class='name-text-wrapper'><span class='name-text enabled'>第三季 </span></span></div></div><div class='col' style='width: 35%'></div><div class='col' style='width: 19%'></div></div>";
            if (that.results[0][0]) {
                arrayUtils.forEach(that.results[0][1], function (pic) {
                    var milliseconds = parseInt(pic.ADDON.replace(/\D/igm, ""));
                    //实例化一个新的日期格式，使用1970 年 1 月 1 日至今的毫秒数为参数
                    var addDate = new Date(milliseconds).Format("yyyy-MM-dd hh:mm:ss");
                    htmlStr += "<div class='item clearfix'><div class='col c1' style='width: 45%;'><span class='chk'><span class='chk-ico'></span></span><div class='name'><span class='name-text-wrapper'><span class='name-text enabled'>" + pic.PICDES + "</span></span></div></div><div class='col' style='width: 35%'>" + addDate + "</div><div class='col' style='width: 19%'>图片</div></div>";
                });
            }
            that.jzAttachList.innerHTML = htmlStr;
            var videoStr = "";
            if (that.results[1][0]) {
                arrayUtils.forEach(that.results[1][1], function (pic) {
                    var milliseconds = parseInt(pic.ADDON.replace(/\D/igm, ""));
                    //实例化一个新的日期格式，使用1970 年 1 月 1 日至今的毫秒数为参数
                    var addDate = new Date(milliseconds).Format("yyyy-MM-dd hh:mm:ss");
                    videoStr += "<div class='item clearfix'><div class='col c1' style='width: 45%;'><span class='chk'><span class='chk-ico'></span></span><div class='name'><span class='name-text-wrapper'><span class='name-text enabled'>" + pic.VIDEODES + "</span></span></div></div><div class='col' style='width: 35%'>" + addDate + "</div><div class='col' style='width: 19%'>视频</div></div>";
                });
            }
            if (videoStr) {
                domConstruct.place(videoStr, that.jzAttachList, 'first');
            }
        },
        initUploadBtn: function () {
            var settings_object = {
                upload_url: "webservice/Handler.ashx",
                flash_url : "widget/swfupload/swfupload.swf",
                file_post_name : "Filedata",
                post_params : {
                    "post_param_name_1" : "post_param_value_1",
                    "post_param_name_2" : "post_param_value_2",
                    "post_param_name_n" : "post_param_value_n"
                },
                use_query_string : false,
                requeue_on_error : false,
                http_success : [201, 202],
                assume_success_timeout : 0,
                file_types : "*.jpg;*.gif;*.png;*.flv;*.mp4",
                file_types_description: "图片及视频文件",
                file_size_limit : "40980",//最大40M
                file_upload_limit : 10,
                file_queue_limit : 0,
                debug : false,
                prevent_swf_caching : false,
                preserve_relative_urls : false,
                button_placeholder_id: 'jzUploadFiles',
                button_image_url: "widget/swfupload/images/placeholder.png",
                button_width : 82,
                button_height : 34,
                button_text : "上传附件",
                button_text_style: "color:#FFF;height:50px;width:112px;",
                button_text_left_padding : 3,
                button_text_top_padding : 2,
                button_action : SWFUpload.BUTTON_ACTION.SELECT_FILES,
                button_disabled : false,
                button_cursor : SWFUpload.CURSOR.HAND,
                button_window_mode : SWFUpload.WINDOW_MODE.TRANSPARENT,
                swfupload_loaded_handler : this.swfupload_loaded_function,
                file_dialog_start_handler : this.file_dialog_start_function,
                file_queued_handler : this.file_queued_function,
                file_queue_error_handler : this.file_queue_error_function,
                file_dialog_complete_handler : this.file_dialog_complete_function,
                upload_start_handler : this.upload_start_function,
                upload_progress_handler : this.upload_progress_function,
                upload_error_handler: this.upload_error_function,
                upload_success_handler : this.upload_success_function,
                upload_complete_handler : this.upload_complete_function
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
            //var imageName = "error.gif";
            //var progress;
            //try {
            //    switch (errorCode) {
            //        case SWFUpload.UPLOAD_ERROR.FILE_CANCELLED:
            //            try {
            //                progress = new FileProgress(file, this.customSettings.upload_target);
            //                progress.setCancelled();
            //                progress.setStatus("Cancelled");
            //                progress.toggleCancel(false);
            //            }
            //            catch (ex1) {
            //                this.debug(ex1);
            //            }
            //            break;
            //        case SWFUpload.UPLOAD_ERROR.UPLOAD_STOPPED:
            //            try {
            //                progress = new FileProgress(file, this.customSettings.upload_target);
            //                progress.setCancelled();
            //                progress.setStatus("Stopped");
            //                progress.toggleCancel(true);
            //            }
            //            catch (ex2) {
            //                this.debug(ex2);
            //            }
            //        case SWFUpload.UPLOAD_ERROR.UPLOAD_LIMIT_EXCEEDED:
            //            imageName = "uploadlimit.gif";
            //            break;
            //        default:
            //            alert(message);
            //            break;
            //    }
            //    addImage("images/" + imageName);
            //} catch (ex3) {
            //    this.debug(ex3);
            //}
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
                    var that = clazz.getInstance();
                    arrayUtils.forEach(uploadedFiles, function (upFile) {
                        var defer = null;
                        var exts = /\.[^\.]+/.exec(upFile.NEW);
                        if (exts.length > 0 && (exts[0].toLowerCase() == '.flv' || exts[0].toLowerCase() == '.mp4')) {
                            defer = xhr.get("webservice/WebServiceMap.asmx/InsertVideos", {
                                query: { layerid: that.layerID, objid: that.objID, videourl: upFile.NEW, videodes: upFile.OLD },
                                handleAs: "json",
                                timeout: 10000
                            });
                            list.push(defer);
                        } else {
                            defer = xhr.get("webservice/WebServiceMap.asmx/InsertPics", {
                                query: { layerid: that.layerID, objid: that.objID, picurl: upFile.NEW, picdes: upFile.OLD },
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
                        clazz.getInstance().getAttachments();
                    });
                }
            } catch (ex) {
                this.debug(ex);
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
