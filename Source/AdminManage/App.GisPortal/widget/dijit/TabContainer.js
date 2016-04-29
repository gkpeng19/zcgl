/**
 * @author Administrator
 */
define(['dojo/_base/declare',
    'dojo/dom-class',
    'dojo/_base/array',
    'dojo/_base/lang',
    'dojo/query',
    'dijit/_WidgetBase',
    'dijit/_TemplatedMixin',
	'dstore/Memory',
    'dojo/request/xhr',
    'dojo/dom-construct',
	'dstore/Trackable',
    'dojo/dom-style',
    'dojo/dom-attr',
    'dojo/dom',
    'dojo/on',
    'dojo/topic',
	'dgrid/OnDemandGrid',
    'dgrid/Selection',
    'widget/dijit/EditFeature',
    'dojo/DeferredList',
    'dojo/text!./template/TabContainer.html'],
function (declare, domClass, arrayUtils, lang, query, _WidgetBase, _TemplatedMixin,
    Memory, xhr, domConstruct, Trackable, domStyle, domAttr, dom, on, topic, OnDemandGrid, Selection, EditFeatureDialog,
    DeferredList, template) {
    var instance = null, clazz, uploadedFiles = [];
    clazz = declare([_WidgetBase, _TemplatedMixin], {
        templateString: template,
        map: null,
        feature: null,
        layerID: -1,
        objID: -1,
        layerIndex: -1,
        type: -1,
        editDialog: null,
        isShown: false,
        postCreate: function () {
            instance = this;
            if (this.feature) {
                this.jzObjCode.innerHTML = this.feature.attributes["OBJCODE"] ? this.feature.attributes["OBJCODE"] : "";
                this.jzObjName.innerHTML = this.feature.attributes["OBJNAME"] ? this.feature.attributes["OBJNAME"] : "";
                //this.jzObjLength.innerHTML = this.feature.attributes["SHAPE.AREA"] ? this.feature.attributes["SHAPE.AREA"] : "属性信息为空";
                //this.jzObjArea.innerHTML = this.feature.attributes["OBJAREA"] ? this.feature.attributes["OBJAREA"] : "属性信息为空";
            }
        },
        startup: function () {
            this.inherited(arguments);
            this.getParams();
            topic.subscribe('topic/UploadFileSuccess', lang.hitch(this, this.getAttachments));
        },
        getParams: function () {
            this.objID = this.feature.attributes["OBJECTID"] || this.feature.attributes["FID"];
            var layerIndex = -1;
            var that = this;
            arrayUtils.forEach(JZ.privilegeLayers, function (clsInfo) {
                var pLayers = clsInfo.LAYERS;
                arrayUtils.forEach(pLayers, function (layer) {
                    if (layer.id == that.layerID) {
                        that.layerIndex = layer.serverindex;
                    }
                })
            });
        },
        createStore: function (cols) {
            var data = [];
            var attributes = this.feature.attributes;
            var i = 0;
            for (var key in attributes) {
                if (!(key === "OBJECTID" || key === "FID") && key != 'SHAPE') {
                    if (attributes[key] &&
                        attributes[key].toString().replace(/(^\s*)|(\s*$)/g, '') != "" && 
                        attributes[key].toString() != 'null' && 
                        attributes[key].toString() != 'Null' && 
                        attributes[key].toString() != 'undefined') {
                        data.push({});
                        data[i]["id"] = i;
                        data[i]["PROPERTYENAME"] = key;//英文名
                        if (key == 'SHAPE.AREA') {
                            data[i]["PROPERTYNAME"] = '面积(平方米)';//中文名
                        } else if (key == 'SHAPE.LEN') {
                            data[i]["PROPERTYNAME"] = '周长(米)';//中文名
                        } else {
                            data[i]["PROPERTYNAME"] = key;
                        }
                        data[i]["PROPERTYVALUE"] = attributes[key];
                        i = i + 1;
                    }
                }
            }
            if (cols) {
                for (var j = 0; j < cols.length; j++) {
                    var col = cols[j];
                    arrayUtils.forEach(data, function (info) {
                        if (info.PROPERTYENAME === col.COLCODE) {
                            info.PROPERTYNAME = col.COLNAME;
                        }
                    });
                }
            }
            var pattern = new RegExp("^[a-zA-Z_0-9]*$");
            var flag = 0;
            for (var m = 0, length = data.length; m < length; m++) {
                if (pattern.test(data[flag].PROPERTYNAME)) {
                    data.splice(flag, 1);
                } else {
                    flag = flag + 1;
                }
            }
            return data;
        },
        attachData: function () {
            var that = this;
            var _lcols = window.AllLayerCols.get(that.layerID);
            if (_lcols) {
                //if (_lcols.length <= 0) {
                //    _lcols = (window.AllLayerCols && window.AllLayerCols.data.length > 0) ? window.AllLayerCols.data[0].lcos : _lcols;
                //}
                that.createDataGrid(that.createStore(_lcols));
            } else {
                xhr.get("webservice/WebServiceMap.asmx/GetColsBylayer", {
                    handleAs: "json",
                    timeout: 10000,
                    query: { layerid: that.layerID }
                }).then(function (suc) {
                    var o = new layercols(that.layerID, suc);
                    window.AllLayerCols.add(o);
                    that.createDataGrid(that.createStore(suc));
                }, function (err) {
                    console.log(err);
                });
            }
        },
        createDataGrid: function (data) {
            var that = this;
            var store = new (declare([Memory, Trackable]))({
                data: data
            });
            var myGrid = new (declare([OnDemandGrid, Selection]))({
                collection: store,
                columns: {
                    PROPERTYNAME: {
                        label: '属性名'
                    },
                    PROPERTYVALUE: {
                        label: '属性值'
                    }
                }
            }, that.jzFeatureGrid);
            myGrid.startup();
        },

        onBasicShow: function () {
            domClass.add(this.jzBasic, "active");
            domClass.remove(this.jzDetail, "active");
            domClass.remove(this.jzFeature, "active");
            domClass.add(this.jzTabBasic, "jz-tab-show");
            domClass.remove(this.jzTabDetail, "jz-tab-show");
            domClass.remove(this.jzTabFeature, "jz-tab-show");
        },
        onDetailShow: function () {
            domClass.remove(this.jzBasic, "active");
            domClass.add(this.jzDetail, "active");
            domClass.remove(this.jzFeature, "active");
            domClass.remove(this.jzTabBasic, "jz-tab-show");
            domClass.add(this.jzTabDetail, "jz-tab-show");
            domClass.remove(this.jzTabFeature, "jz-tab-show");
            if (!this.isShown) {
                this.attachData();
                this.isShown = true;
            }
        },
        mulShown:false,
        onFeatureShow: function () {
            domClass.remove(this.jzBasic, "active");
            domClass.remove(this.jzDetail, "active");
            domClass.add(this.jzFeature, "active");
            domClass.remove(this.jzTabBasic, "jz-tab-show");
            domClass.remove(this.jzTabDetail, "jz-tab-show");
            domClass.add(this.jzTabFeature, "jz-tab-show");
            if (!this.mulShown) {
                this.getAttachments();
                this.mulShown = true;
                this.initBtn();
            }
        },
        getAttachments: function () {
            JZ.loading.show();
            var that = this;
            var d1 = xhr.get("webservice/WebServiceMap.asmx/GetPics", {
                query: { id: that.layerIndex, objid: that.objID },
                handleAs: "json",
                timeout: 10000
            });
            var d2 = xhr.get("webservice/WebServiceMap.asmx/GetVideos", {
                query: { id: that.layerIndex, objid: that.objID },
                handleAs: "json",
                timeout: 10000
            });
            var deferredList = new DeferredList([d1, d2]);
            deferredList.then(function (result) {
                that.results = result;
                that.createNode(result);
                JZ.loading.hide();
            });
        },
        initBtn: function () {
            var that = this;
            if (window.File && window.FileReader && window.FileList && window.Blob) {
                return (function () {
                    require(['widget/dijit/HtmlUploader'], function (uploader) {
                        new uploader({
                            layerIndex: that.layerIndex,
                            objID: that.objID
                        }).startup();
                    });
                    //require(['widget/dijit/UploadProgressHtml', 'dojox/form/Uploader'], function (UploadProgress) {
                    //    var uploader = new dojox.form.Uploader({
                    //        label: "选择文件",
                    //        multiple: true,
                    //        uploadOnSelect: true,
                    //        force: 'html5',
                    //        url: "webservice/UploadHandler.ashx"
                    //    });
                    //    domConstruct.place(uploader.domNode, that.jzAttachDialogTools, 'only');
                    //    uploader.startup();
                    //    //UploadProgress.getInstance().startup(file);
                    //    on(uploader, 'begin', function (fis) {
                    //        for (var i = 0; i < fis.length; i++) {
                    //            UploadProgress.getInstance().startup(fis[i]);
                    //        }
                    //    });
                    //    on(uploader, 'complete', function () {
                    //        that.reflash();
                    //    });
                    //    on(uploader, 'progress', function (e) {
                    //        try {
                    //            var percent = Math.ceil((e.bytesLoaded / e.bytesTotal) * 100);
                    //            UploadProgress.getInstance().setProgress(percent > 100 ? 100 : percent);
                    //        } catch (ex) {
                    //            console.log(ex);
                    //        }
                    //    });
                    //});
                })();
            } else {
                return (function () {
                    require(['widget/dijit/FlashUploader'], function (uploader) {
                        new uploader(that.layerIndex, that.objID);
                    });
                })();
            }
        },
        //reflash: function () {
        //    JZ.loading.show();
        //    var list = [];
        //    var that = instance;
        //    arrayUtils.forEach(uploadedFiles, function (upFile) {
        //        var defer = null;
        //        var exts = /\.[^\.]+/.exec(upFile.NEW);
        //        if (exts.length > 0 && (exts[0].toLowerCase() == '.flv' || exts[0].toLowerCase() == '.mp4')) {
        //            defer = xhr.get("webservice/WebServiceMap.asmx/InsertVideos", {
        //                query: { layerid: that.layerIndex, objid: that.objID, videourl: upFile.NEW, videodes: upFile.OLD },
        //                handleAs: "json",
        //                timeout: 10000
        //            });
        //            list.push(defer);
        //        } else {
        //            defer = xhr.get("webservice/WebServiceMap.asmx/InsertPics", {
        //                query: { layerid: that.layerIndex, objid: that.objID, picurl: upFile.NEW, picdes: upFile.OLD },
        //                handleAs: "json",
        //                timeout: 10000
        //            });
        //            list.push(defer);
        //        }
        //    });
        //    var deferredList = new DeferredList(list);
        //    deferredList.then(function (result) {
        //        alert("全部文件上传成功！");
        //        UploadProgress.getInstance().close();
        //        topic.publish("topic/UploadFileSuccess");
        //    });
        //},
        //initUploadBtn: function () {
        //    var settings_object = {
        //        upload_url: "webservice/Handler.ashx",
        //        flash_url: "widget/swfupload/swfupload.swf",
        //        file_post_name: "Filedata",
        //        post_params: {
        //            "post_param_name_1": "post_param_value_1",
        //            "post_param_name_2": "post_param_value_2",
        //            "post_param_name_n": "post_param_value_n"
        //        },
        //        use_query_string: false,
        //        requeue_on_error: false,
        //        http_success: [201, 202],
        //        assume_success_timeout: 0,
        //        file_types: "*.jpg;*.gif;*.png;*.flv;*.mp4",
        //        file_types_description: "图片及视频文件",
        //        file_size_limit: "40980",//最大40M
        //        file_upload_limit: 10,
        //        file_queue_limit: 0,
        //        debug: false,
        //        prevent_swf_caching: false,
        //        preserve_relative_urls: false,
        //        button_placeholder_id: 'jzUploadFiles',
        //        button_image_url: "widget/swfupload/images/placeholder.png",
        //        button_width: 82,
        //        button_height: 34,
        //        button_text: "上传附件",
        //        button_text_style: "color:#FFF;height:50px;width:112px;",
        //        button_text_left_padding: 3,
        //        button_text_top_padding: 2,
        //        button_action: SWFUpload.BUTTON_ACTION.SELECT_FILES,
        //        button_disabled: false,
        //        button_cursor: SWFUpload.CURSOR.HAND,
        //        button_window_mode: SWFUpload.WINDOW_MODE.TRANSPARENT,
        //        swfupload_loaded_handler: this.swfupload_loaded_function,
        //        file_dialog_start_handler: this.file_dialog_start_function,
        //        file_queued_handler: this.file_queued_function,
        //        file_queue_error_handler: this.file_queue_error_function,
        //        file_dialog_complete_handler: this.file_dialog_complete_function,
        //        upload_start_handler: this.upload_start_function,
        //        upload_progress_handler: this.upload_progress_function,
        //        upload_error_handler: this.upload_error_function,
        //        upload_success_handler: this.upload_success_function,
        //        upload_complete_handler: this.upload_complete_function
        //    };
        //    var swf = new SWFUpload(settings_object);
        //},
        //swfupload_loaded_function: function () {
        //    console.log("swf加载成功！");
        //},
        //file_dialog_start_function: function () {
        //    uploadedFiles = [];
        //    console.log("即将打开选择文件窗口！");
        //},
        //file_queued_function: function (file) {
        //    UploadProgress.getInstance().startup(file);
        //    console.log("即将打开选择文件窗口！");
        //},
        //file_queue_error_function: function (file, errorCode, message) {
        //    try {
        //        if (errorCode === SWFUpload.errorCode_QUEUE_LIMIT_EXCEEDED) {
        //            alert("您选择的图片过多，请重新选择！");
        //        }
        //        switch (errorCode) {
        //            case SWFUpload.QUEUE_ERROR.ZERO_BYTE_FILE:
        //                alert("不能上传空文件!");
        //                break;
        //            case SWFUpload.QUEUE_ERROR.FILE_EXCEEDS_SIZE_LIMIT:
        //                alert("文件不能大于40M!");
        //                break;
        //            case SWFUpload.QUEUE_ERROR.INVALID_FILETYPE:
        //                alert("不支持的文件类型！");
        //            default:
        //                alert("未知错误，请重试！");
        //                break;
        //        }
        //    } catch (ex) {
        //        this.debug(ex);
        //    }
        //},
        //file_dialog_complete_function: function (numFilesSelected, numFilesQueued) {
        //    try {
        //        if (numFilesQueued > 0) {
        //            this.startUpload();
        //        }
        //        if (numFilesSelected > numFilesQueued) {
        //            alert("有" + (numFilesSelected - numFilesQueued) + "个文件大于40M，已被忽略!");
        //        }
        //    } catch (ex) {
        //        this.debug(ex);
        //    }
        //},
        //upload_start_function: function (file) {
        //    console.log("开始上传！");
        //},
        //upload_progress_function: function (file, bytesLoaded) {
        //    try {
        //        var percent = Math.ceil((bytesLoaded / file.size) * 100);
        //        UploadProgress.getInstance().setProgress(file, percent > 100 ? 100 : percent);
        //    } catch (ex) {
        //        this.debug(ex);
        //    }
        //},
        //upload_error_function: function (file, errorCode, message) {
        //    console.log("文件 " + file.name + " 发生错误，错误码：" + errorCode + "，错误信息：" + message);
        //},
        //upload_success_function: function (file, serverData) {
        //    try {
        //        //将原文件名file.name和新文件名serverData保存到数据库
        //        //先保存下来，等全部上传成功后再提交到数据库
        //        uploadedFiles.push({ OLD: file.name, NEW: serverData });
        //    } catch (ex) {
        //        this.debug(ex);
        //    }
        //},
        //upload_complete_function: function (file) {
        //    try {
        //        if (this.getStats().files_queued > 0) {
        //            this.startUpload();
        //        } else {
        //            JZ.loading.show();
        //            var list = [];
        //            var that = instance;
        //            arrayUtils.forEach(uploadedFiles, function (upFile) {
        //                var defer = null;
        //                var exts = /\.[^\.]+/.exec(upFile.NEW);
        //                if (exts.length > 0 && (exts[0].toLowerCase() == '.flv' || exts[0].toLowerCase() == '.mp4')) {
        //                    defer = xhr.get("webservice/WebServiceMap.asmx/InsertVideos", {
        //                        query: { layerid: that.layerIndex, objid: that.objID, videourl: upFile.NEW, videodes: upFile.OLD },
        //                        handleAs: "json",
        //                        timeout: 10000
        //                    });
        //                    list.push(defer);
        //                } else {
        //                    defer = xhr.get("webservice/WebServiceMap.asmx/InsertPics", {
        //                        query: { layerid: that.layerIndex, objid: that.objID, picurl: upFile.NEW, picdes: upFile.OLD },
        //                        handleAs: "json",
        //                        timeout: 10000
        //                    });
        //                    list.push(defer);
        //                }
        //            });
        //            var deferredList = new DeferredList(list);
        //            deferredList.then(function (result) {
        //                alert("全部文件上传成功！");
        //                UploadProgress.getInstance().close();
        //                that.getAttachments();
        //            });
        //        }
        //    } catch (ex) {
        //        this.debug(ex);
        //    }
        //},

        createNode: function (results) {
            var that = this;
            if (that.jzAttachList) {
                var htmlStr = "";
                if (results[0][0]) {
                    arrayUtils.forEach(results[0][1], function (pic) {
                        //var milliseconds = parseInt(pic.ADDON.replace(/\D/igm, ""));
                        //实例化一个新的日期格式，使用1970 年 1 月 1 日至今的毫秒数为参数
                        //var addDate = new Date(milliseconds).Format("yyyy-MM-dd hh:mm:ss");
                        htmlStr += "<div id='jzPic_" + pic.ID + "' class='item clearfix'><div class='col c1' style='width: 40%;'><div class='name'><span class='name-text-wrapper'><span class='name-text enabled'>" + pic.PICDES + "</span></span></div></div><div class='col' style='width: 29%'>图片</div><div class='col' style='width: 29%'><a data-id='" + pic.ID + "' class='jzDeleteUploadPics' href='javascript:void(0)'>删除</a></div></div>";
                    });
                }
                that.jzAttachList.innerHTML = htmlStr;
                var videoStr = "";
                if (results[1][0]) {
                    arrayUtils.forEach(results[1][1], function (pic) {
                        //var milliseconds = parseInt(pic.ADDON.replace(/\D/igm, ""));
                        //实例化一个新的日期格式，使用1970 年 1 月 1 日至今的毫秒数为参数
                        //var addDate = new Date(milliseconds).Format("yyyy-MM-dd hh:mm:ss");
                        videoStr += "<div id='jzVideo_" + pic.ID + "' class='item clearfix'><div class='col c1' style='width: 40%;'><div class='name'><span class='name-text-wrapper'><span class='name-text enabled'>" + pic.VIDEODES + "</span></span></div></div><div class='col' style='width: 29%'>视频</div><div class='col' style='width: 29%'><a data-id='" + pic.ID + "' class='jzDeleteUploadVideos' href='javascript:void(0)'>删除</a></div></div>";
                    });
                }
                if (videoStr) {
                    domConstruct.place(videoStr, that.jzAttachList, 'first');
                }
                query('.jzDeleteUploadPics').on('click', function (evt) {
                    stopEvent(evt);
                    var id = domAttr.get(this, 'data-id');
                    xhr.get("webservice/WebServiceMap.asmx/DeletePics", {
                        query: { id: id },
                        handleAs: "json",
                        timeout: 10000
                    }).then(function (suc) {
                        if (suc == '1') {
                            domConstruct.destroy(dom.byId('jzPic_' + id));
                        } else {
                            alert("删除失败，请重试！");
                        }
                    }, function (err) {
                        alert("删除失败，请重试！");
                    });
                });
                query('.jzDeleteUploadVideos').on('click', function (evt) {
                    stopEvent(evt);
                    var id = domAttr.get(this, 'data-id');
                    xhr.get("webservice/WebServiceMap.asmx/DeleteVideos", {
                        query: { id: id },
                        handleAs: "json",
                        timeout: 10000
                    }).then(function (suc) {
                        if (suc == '1') {
                            domConstruct.destroy(dom.byId('jzVideo_' + id));
                        } else {
                            alert("删除失败，请重试！");
                        }
                    }, function (err) {
                        alert("删除失败，请重试！");
                    });
                });
            }
        },

        onEditFeature: function () {
            EditFeatureDialog.getInstance().startup(this.layerIndex, this.objID, this.layerID);
        },
        //onManageAttach: function () {
        //    var that = this;
        //    require(['widget/dijit/Attachments'], function (clazz) {
        //        var layerID = -1;
        //        var layerCode = -1;
        //        arrayUtils.forEach(JZ.privilegeLayers, function (clsInfo) {
        //            var pLayers = clsInfo.LAYERS;
        //            arrayUtils.forEach(pLayers, function (layer) {
        //                if (layer.id == that.layerID) {
        //                    layerID = layer.id;
        //                    layerCode = layer.serverindex;
        //                }
        //            })
        //        });
        //        var eleID = that.feature.attributes["OBJECTID"];
        //        clazz.getInstance().startup(layerCode, eleID);
        //    });
        //},
        onShowSlider: function () {
            var that = this;
            xhr.get("webservice/WebServiceMap.asmx/GetPics", {
                query: { id: that.layerIndex, objid: that.objID },
                handleAs: "json",
                timeout: 10000
            }).then(function (suc) {
                if (suc.length > 0) {
                    //显示幻灯片，根据图层ID和要素ID去数据库中取出幻灯片信息
                    require(['widget/dijit/SliderShow'], function (clazz) {
                        clazz.getInstance().startup(suc);
                    });
                } else {
                    alert("您尚未上传图片，请上传后查看！");
                }
            });
        },
        onShowVideo: function () {
            var that = this;
            xhr.get("webservice/WebServiceMap.asmx/GetVideos", {
                query: { id: that.layerIndex, objid: that.objID },
                handleAs: "json",
                timeout: 10000
            }).then(function (suc) {
                if (suc.length > 0) {
                    //显示幻灯片，根据图层ID和要素ID去数据库中取出幻灯片信息
                    require(['widget/dijit/VideoHTML'], function (clazz) {
                        clazz.getInstance().startup(suc[0].VIDEOURL);
                    });
                } else {
                    alert("您尚未上传视频，请上传后查看！");
                }
            });
        },
        onSubmitError: function () {
            var that = this;
            require(['widget/dijit/SubmitError'], function (clazz) {
                //图层ID， 图层编码， 图层名， 元素ID， 元素名称
                var layerName = "";
                arrayUtils.forEach(JZ.privilegeLayers, function (clsInfo) {
                    var pLayers = clsInfo.LAYERS;
                    arrayUtils.forEach(pLayers, function (layer) {
                        if (layer.id == that.layerID) {
                            layerName = layer.cnName;
                        }
                    })
                });
                var eleName = that.feature.attributes["MC"] || that.feature.attributes["OBJNAME"] || that.feature.attributes["名称"];
                clazz.getInstance().startup(that.layerID, that.layerIndex, layerName, that.objID, eleName);
            });
        }
    });
    return clazz;
});
