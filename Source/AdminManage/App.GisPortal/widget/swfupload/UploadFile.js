
define([], function () {
    var UploadFile;

    function EventUtils(node, event, cbk) {
        if (node.addEventListener) {
            node.addEventListener(event, cbk);
        } else if (node.attachEvent) {
            node.attachEvent(event, cbk);
        } else {
            node['on' + event] = cbk;
        }
    }

    if (UploadFile == undefined) {
        UploadFile = function (settings) {
            this.initUploadFile(settings);
        };
    }
    UploadFile.prototype.initUploadFile = function (settings) {
        this.customSettings = {};
        this.settings = settings;
        this.eventQueue = [];
        this.initSettings();
        this.loadFileUpload();
    };
    UploadFile.instances = {};
    UploadFile.movieCount = 0;
    UploadFile.version = "1.0.0 2015-09-11";
    UploadFile.QUEUE_ERROR = {
        QUEUE_LIMIT_EXCEEDED: -100,
        FILE_EXCEEDS_SIZE_LIMIT: -110,
        ZERO_BYTE_FILE: -120,
        INVALID_FILETYPE: -130
    };
    UploadFile.UPLOAD_ERROR = {
        HTTP_ERROR: -200,
        MISSING_UPLOAD_URL: -210,
        IO_ERROR: -220,
        SECURITY_ERROR: -230,
        UPLOAD_LIMIT_EXCEEDED: -240,
        UPLOAD_FAILED: -250,
        SPECIFIED_FILE_ID_NOT_FOUND: -260,
        FILE_VALIDATION_FAILED: -270,
        FILE_CANCELLED: -280,
        UPLOAD_STOPPED: -290
    };
    UploadFile.FILE_STATUS = {
        QUEUED: -1,
        IN_PROGRESS: -2,
        ERROR: -3,
        COMPLETE: -4,
        CANCELLED: -5
    };
    UploadFile.BUTTON_ACTION = {
        SELECT_FILE: -100,
        SELECT_FILES: -110,
        START_UPLOAD: -120
    };
    UploadFile.CURSOR = {
        ARROW: -1,
        HAND: -2
    };
    UploadFile.WINDOW_MODE = {
        WINDOW: "window",
        TRANSPARENT: "transparent",
        OPAQUE: "opaque"
    };
    // 将相对路径转换成绝对路径
    UploadFile.completeURL = function (url) {
        if (typeof (url) !== "string" || url.match(/^https?:\/\//i) || url.match(/^\//)) {
            return url;
        }
        var currentURL = window.location.protocol + "//" + window.location.hostname + (window.location.port ? ":" + window.location.port : "");
        var indexSlash = window.location.pathname.lastIndexOf("/");
        if (indexSlash <= 0) {
            path = "/";
        } else {
            path = window.location.pathname.substr(0, indexSlash) + "/";
        }
        return path + url;
    };
    UploadFile.prototype.initSettings = function () {
        this.ensureDefault = function (settingName, defaultValue) {
            this.settings[settingName] = (this.settings[settingName] == undefined) ? defaultValue : this.settings[settingName];
        };
        this.ensureDefault("upload_url", "");
        this.ensureDefault("preserve_relative_urls", false);
        this.ensureDefault("file_post_name", "Filedata");
        this.ensureDefault("post_params", {});
        this.ensureDefault("use_query_string", false);
        this.ensureDefault("requeue_on_error", false);
        this.ensureDefault("http_success", []);
        this.ensureDefault("assume_success_timeout", 0);
        this.ensureDefault("file_types", "*.*");
        this.ensureDefault("file_types_description", "All Files");
        this.ensureDefault("file_size_limit", 0);
        this.ensureDefault("file_upload_limit", 0);
        this.ensureDefault("file_queue_limit", 0);
        this.ensureDefault("button_width", 60);
        this.ensureDefault("button_height", 30);
        this.ensureDefault("button_text", "选择文件");
        this.ensureDefault("button_action", UploadFile.BUTTON_ACTION.SELECT_FILES);
        this.ensureDefault("button_disabled", false);
        this.ensureDefault("button_placeholder_id", "");
        this.ensureDefault("button_placeholder", null);
        this.ensureDefault("button_cursor", UploadFile.CURSOR.ARROW);
        this.ensureDefault("button_window_mode", UploadFile.WINDOW_MODE.WINDOW);
        this.ensureDefault("debug", false);
        this.settings.debug_enabled = this.settings.debug;
        this.settings.return_upload_start_handler = this.returnUploadStart;
        this.ensureDefault("file_queued_handler", null);
        this.ensureDefault("file_queue_error_handler", null);
        this.ensureDefault("file_dialog_complete_handler", null);
        this.ensureDefault("upload_start_handler", null);
        this.ensureDefault("upload_progress_handler", null);
        this.ensureDefault("upload_error_handler", null);
        this.ensureDefault("upload_success_handler", null);
        this.ensureDefault("upload_complete_handler", null);
        this.ensureDefault("debug_handler", this.debugMessage);
        this.ensureDefault("custom_settings", {});
        this.customSettings = this.settings.custom_settings;
        if (!this.settings.preserve_relative_urls) {
            this.settings.upload_url = UploadFile.completeURL(this.settings.upload_url);
            this.settings.button_image_url = UploadFile.completeURL(this.settings.button_image_url);
        }
        delete this.ensureDefault;
    };
    UploadFile.prototype.loadFileUpload = function () {
        var targetElement, tempParent;
        if (document.getElementById(this.movieName) !== null) {
            throw "ID " + this.movieName + "已经存在，请提供其它ID名称！";
        }
        targetElement = document.getElementById(this.settings.button_placeholder_id) || this.settings.button_placeholder;
        if (targetElement == undefined) {
            throw "请提供PlaceHolder元素的ID：" + this.settings.button_placeholder_id;
        }
        tempParent = document.createElement("span");
        tempParent.className = 'btn btn-default';
        tempParent.setAttribute('style', 'position: relative;overflow: hidden;cursor: pointer;width:' + this.settings.button_width + ';height:' + this.settings.button_height);
        tempParent.innerHTML = this.getInnerHTML();
        targetElement.parentNode.replaceChild(tempParent.firstChild, targetElement);
        EventUtils(document.getElementById(this.settings.movieName), 'change', this.settings.file_dialog_complete_handler);
    };
    UploadFile.prototype.file_dialog_complete_handler = function () {
        var selectFiles = this.files;
        var htmlStr = "<ul>";
        for (var i = 0; i < selectFiles.length; i++) {
            var file = selectFiles[i];
            queuedFiles.push(file);
            htmlStr += "<li>" + file.name + "</li>";
        }
    },
    UploadFile.prototype.getInnerHTML = function () {
        var str = this.settings.multiple ? "multiple='multiple'" : "";
        var defaultFilter = this.settings.file_types ? this.settings.file_types : "image/jpeg,.jpg,image/gif,.gif,image/png,.png,video/x-mpeg2,.mp4";
        return this.settings.button_text + '<input id="' + this.movieName + '" name="' + this.movieName + '" type="file"' + str + ' accept="' + defaultFilter + ' style="border: none;opacity: 0;filter: alpha(opacity=0);position: absolute;top: 0;right: 0;width: 100%;bottom: 0;height: 100%;cursor: pointer;"">';
    };
    UploadFile.prototype.destroy = function () {
        try {
            this.cancelUpload(null, false);
            var movieElement = null;
            movieElement = this.getMovieElement();
            if (movieElement && typeof (movieElement.CallFunction) === "unknown") { 
                for (var i in movieElement) {
                    try {
                        if (typeof (movieElement[i]) === "function") {
                            movieElement[i] = null;
                        }
                    } catch (ex1) { }
                }
                try {
                    movieElement.parentNode.removeChild(movieElement);
                } catch (ex) { }
            }
            window[this.movieName] = null;
            UploadFile.instances[this.movieName] = null;
            delete UploadFile.instances[this.movieName];
            this.movieElement = null;
            this.settings = null;
            this.customSettings = null;
            this.eventQueue = null;
            this.movieName = null;
            return true;
        } catch (ex2) {
            return false;
        }
    };
    UploadFile.prototype.displayDebugInfo = function () {
        this.debug(
		    [
			    "---UploadFile Instance Info---\n",
			    "Version: ", UploadFile.version, "\n",
			    "Movie Name: ", this.movieName, "\n",
			    "Settings:\n",
			    "\t", "upload_url:               ", this.settings.upload_url, "\n",
			    "\t", "flash_url:                ", this.settings.flash_url, "\n",
			    "\t", "use_query_string:         ", this.settings.use_query_string.toString(), "\n",
			    "\t", "requeue_on_error:         ", this.settings.requeue_on_error.toString(), "\n",
			    "\t", "http_success:             ", this.settings.http_success.join(", "), "\n",
			    "\t", "assume_success_timeout:   ", this.settings.assume_success_timeout, "\n",
			    "\t", "file_post_name:           ", this.settings.file_post_name, "\n",
			    "\t", "post_params:              ", this.settings.post_params.toString(), "\n",
			    "\t", "file_types:               ", this.settings.file_types, "\n",
			    "\t", "file_types_description:   ", this.settings.file_types_description, "\n",
			    "\t", "file_size_limit:          ", this.settings.file_size_limit, "\n",
			    "\t", "file_upload_limit:        ", this.settings.file_upload_limit, "\n",
			    "\t", "file_queue_limit:         ", this.settings.file_queue_limit, "\n",
			    "\t", "debug:                    ", this.settings.debug.toString(), "\n",
			    "\t", "prevent_swf_caching:      ", this.settings.prevent_swf_caching.toString(), "\n",
			    "\t", "button_placeholder_id:    ", this.settings.button_placeholder_id.toString(), "\n",
			    "\t", "button_placeholder:       ", (this.settings.button_placeholder ? "Set" : "Not Set"), "\n",
			    "\t", "button_image_url:         ", this.settings.button_image_url.toString(), "\n",
			    "\t", "button_width:             ", this.settings.button_width.toString(), "\n",
			    "\t", "button_height:            ", this.settings.button_height.toString(), "\n",
			    "\t", "button_text:              ", this.settings.button_text.toString(), "\n",
			    "\t", "button_text_style:        ", this.settings.button_text_style.toString(), "\n",
			    "\t", "button_text_top_padding:  ", this.settings.button_text_top_padding.toString(), "\n",
			    "\t", "button_text_left_padding: ", this.settings.button_text_left_padding.toString(), "\n",
			    "\t", "button_action:            ", this.settings.button_action.toString(), "\n",
			    "\t", "button_disabled:          ", this.settings.button_disabled.toString(), "\n",
			    "\t", "custom_settings:          ", this.settings.custom_settings.toString(), "\n",
			    "Event Handlers:\n",
			    "\t", "UploadFile_loaded_handler assigned:  ", (typeof this.settings.UploadFile_loaded_handler === "function").toString(), "\n",
			    "\t", "file_dialog_start_handler assigned: ", (typeof this.settings.file_dialog_start_handler === "function").toString(), "\n",
			    "\t", "file_queued_handler assigned:       ", (typeof this.settings.file_queued_handler === "function").toString(), "\n",
			    "\t", "file_queue_error_handler assigned:  ", (typeof this.settings.file_queue_error_handler === "function").toString(), "\n",
			    "\t", "upload_start_handler assigned:      ", (typeof this.settings.upload_start_handler === "function").toString(), "\n",
			    "\t", "upload_progress_handler assigned:   ", (typeof this.settings.upload_progress_handler === "function").toString(), "\n",
			    "\t", "upload_error_handler assigned:      ", (typeof this.settings.upload_error_handler === "function").toString(), "\n",
			    "\t", "upload_success_handler assigned:    ", (typeof this.settings.upload_success_handler === "function").toString(), "\n",
			    "\t", "upload_complete_handler assigned:   ", (typeof this.settings.upload_complete_handler === "function").toString(), "\n",
			    "\t", "debug_handler assigned:             ", (typeof this.settings.debug_handler === "function").toString(), "\n"
		    ].join("")
	    );
    };
    /*
    var settings_object = {
    upload_url: "webservice/Handler.ashx",flash_url : "widget/swfupload/swfupload.swf",file_post_name : "Filedata",
    post_params : {"post_param_name_1" : "post_param_value_1","post_param_name_2" : "post_param_value_2","post_param_name_n" : "post_param_value_n"},
    use_query_string : false,requeue_on_error : false, http_success : [201, 202],assume_success_timeout : 0,
    file_types : "*.jpg;*.gif;*.png;*.flv;*.mp4",file_types_description: "图片及视频文件",file_size_limit : "40980",file_upload_limit : 10,file_queue_limit : 0,debug : false,
    prevent_swf_caching : false,preserve_relative_urls : false,button_placeholder_id: 'jzUploadFiles',
    button_image_url: "widget/swfupload/images/placeholder.png",button_width : 82,button_height : 34,button_text : "上传附件",
    button_text_style: "color:#FFF;height:50px;width:112px;",button_text_left_padding : 3,button_text_top_padding : 2,
    button_action : SWFUpload.BUTTON_ACTION.SELECT_FILES,button_disabled : false,button_cursor : SWFUpload.CURSOR.HAND,
    button_window_mode : SWFUpload.WINDOW_MODE.TRANSPARENT,swfupload_loaded_handler : this.swfupload_loaded_function,file_dialog_start_handler : this.file_dialog_start_function,
    file_queued_handler : this.file_queued_function,file_queue_error_handler : this.file_queue_error_function,file_dialog_complete_handler : this.file_dialog_complete_function,
    upload_start_handler : this.upload_start_function,upload_progress_handler : this.upload_progress_function,upload_error_handler: this.upload_error_function,
    upload_success_handler : this.upload_success_function,upload_complete_handler : this.upload_complete_function
    };var swf = new SWFUpload(settings_object);
    */
    UploadFile.prototype.selectFile = function () {
        this.callFlash("SelectFile");
    };
    UploadFile.prototype.selectFiles = function () {
        this.callFlash("SelectFiles");
    };
    UploadFile.prototype.startUpload = function (fileID) {
        this.callFlash("StartUpload", [fileID]);
    };
    UploadFile.prototype.cancelUpload = function (fileID, triggerErrorEvent) {
        if (triggerErrorEvent !== false) {
            triggerErrorEvent = true;
        }
        this.callFlash("CancelUpload", [fileID, triggerErrorEvent]);
    };
    UploadFile.prototype.stopUpload = function () {
        this.callFlash("StopUpload");
    };
    UploadFile.prototype.getStats = function () {
        return this.callFlash("GetStats");
    };
    UploadFile.prototype.setStats = function (statsObject) {
        this.callFlash("SetStats", [statsObject]);
    };
    UploadFile.prototype.getFile = function (fileID) {
        if (typeof (fileID) === "number") {
            return this.callFlash("GetFileByIndex", [fileID]);
        } else {
            return this.callFlash("GetFile", [fileID]);
        }
    };
    UploadFile.prototype.addFileParam = function (fileID, name, value) {
        return this.callFlash("AddFileParam", [fileID, name, value]);
    };
    UploadFile.prototype.removeFileParam = function (fileID, name) {
        this.callFlash("RemoveFileParam", [fileID, name]);
    };
    UploadFile.prototype.setUploadURL = function (url) {
        this.settings.upload_url = url.toString();
        this.callFlash("SetUploadURL", [url]);
    };
    UploadFile.prototype.setPostParams = function (paramsObject) {
        this.settings.post_params = paramsObject;
        this.callFlash("SetPostParams", [paramsObject]);
    };
    UploadFile.prototype.addPostParam = function (name, value) {
        this.settings.post_params[name] = value;
        this.callFlash("SetPostParams", [this.settings.post_params]);
    };
    UploadFile.prototype.removePostParam = function (name) {
        delete this.settings.post_params[name];
        this.callFlash("SetPostParams", [this.settings.post_params]);
    };
    UploadFile.prototype.setFileTypes = function (types, description) {
        this.settings.file_types = types;
        this.settings.file_types_description = description;
        this.callFlash("SetFileTypes", [types, description]);
    };
    UploadFile.prototype.setFileSizeLimit = function (fileSizeLimit) {
        this.settings.file_size_limit = fileSizeLimit;
        this.callFlash("SetFileSizeLimit", [fileSizeLimit]);
    };
    UploadFile.prototype.setFileUploadLimit = function (fileUploadLimit) {
        this.settings.file_upload_limit = fileUploadLimit;
        this.callFlash("SetFileUploadLimit", [fileUploadLimit]);
    };
    UploadFile.prototype.setFileQueueLimit = function (fileQueueLimit) {
        this.settings.file_queue_limit = fileQueueLimit;
        this.callFlash("SetFileQueueLimit", [fileQueueLimit]);
    };
    UploadFile.prototype.setFilePostName = function (filePostName) {
        this.settings.file_post_name = filePostName;
        this.callFlash("SetFilePostName", [filePostName]);
    };
    UploadFile.prototype.setUseQueryString = function (useQueryString) {
        this.settings.use_query_string = useQueryString;
        this.callFlash("SetUseQueryString", [useQueryString]);
    };
    UploadFile.prototype.setRequeueOnError = function (requeueOnError) {
        this.settings.requeue_on_error = requeueOnError;
        this.callFlash("SetRequeueOnError", [requeueOnError]);
    };
    UploadFile.prototype.setHTTPSuccess = function (http_status_codes) {
        if (typeof http_status_codes === "string") {
            http_status_codes = http_status_codes.replace(" ", "").split(",");
        }
        this.settings.http_success = http_status_codes;
        this.callFlash("SetHTTPSuccess", [http_status_codes]);
    };
    UploadFile.prototype.setAssumeSuccessTimeout = function (timeout_seconds) {
        this.settings.assume_success_timeout = timeout_seconds;
        this.callFlash("SetAssumeSuccessTimeout", [timeout_seconds]);
    };
    UploadFile.prototype.setDebugEnabled = function (debugEnabled) {
        this.settings.debug_enabled = debugEnabled;
        this.callFlash("SetDebugEnabled", [debugEnabled]);
    };
    UploadFile.prototype.setButtonImageURL = function (buttonImageURL) {
        if (buttonImageURL == undefined) {
            buttonImageURL = "";
        }

        this.settings.button_image_url = buttonImageURL;
        this.callFlash("SetButtonImageURL", [buttonImageURL]);
    };
    UploadFile.prototype.setButtonDimensions = function (width, height) {
        this.settings.button_width = width;
        this.settings.button_height = height;

        var movie = this.getMovieElement();
        if (movie != undefined) {
            movie.style.width = width + "px";
            movie.style.height = height + "px";
        }
        this.callFlash("SetButtonDimensions", [width, height]);
    };
    UploadFile.prototype.setButtonText = function (html) {
        this.settings.button_text = html;
        this.callFlash("SetButtonText", [html]);
    };
    UploadFile.prototype.setButtonTextPadding = function (left, top) {
        this.settings.button_text_top_padding = top;
        this.settings.button_text_left_padding = left;
        this.callFlash("SetButtonTextPadding", [left, top]);
    };
    UploadFile.prototype.setButtonTextStyle = function (css) {
        this.settings.button_text_style = css;
        this.callFlash("SetButtonTextStyle", [css]);
    };
    UploadFile.prototype.setButtonDisabled = function (isDisabled) {
        this.settings.button_disabled = isDisabled;
        this.callFlash("SetButtonDisabled", [isDisabled]);
    };
    UploadFile.prototype.setButtonAction = function (buttonAction) {
        this.settings.button_action = buttonAction;
        this.callFlash("SetButtonAction", [buttonAction]);
    };
    UploadFile.prototype.setButtonCursor = function (cursor) {
        this.settings.button_cursor = cursor;
        this.callFlash("SetButtonCursor", [cursor]);
    };
    UploadFile.prototype.queueEvent = function (handlerName, argumentArray) {
        if (argumentArray == undefined) {
            argumentArray = [];
        } else if (!(argumentArray instanceof Array)) {
            argumentArray = [argumentArray];
        }
        var self = this;
        if (typeof this.settings[handlerName] === "function") {
            this.eventQueue.push(function () {
                this.settings[handlerName].apply(this, argumentArray);
            });
            setTimeout(function () {
                self.executeNextEvent();
            }, 0);
        } else if (this.settings[handlerName] !== null) {
            throw "Event handler " + handlerName + " is unknown or is not a function";
        }
    };
    UploadFile.prototype.executeNextEvent = function () {
        var f = this.eventQueue ? this.eventQueue.shift() : null;
        if (typeof (f) === "function") {
            f.apply(this);
        }
    };
    UploadFile.prototype.unescapeFilePostParams = function (file) {
        var reg = /[$]([0-9a-f]{4})/i;
        var unescapedPost = {};
        var uk;

        if (file != undefined) {
            for (var k in file.post) {
                if (file.post.hasOwnProperty(k)) {
                    uk = k;
                    var match;
                    while ((match = reg.exec(uk)) !== null) {
                        uk = uk.replace(match[0], String.fromCharCode(parseInt("0x" + match[1], 16)));
                    }
                    unescapedPost[uk] = file.post[k];
                }
            }
            file.post = unescapedPost;
        }
        return file;
    };
    UploadFile.prototype.testExternalInterface = function () {
        try {
            return this.callFlash("TestExternalInterface");
        } catch (ex) {
            return false;
        }
    };
    UploadFile.prototype.flashReady = function () {
        var movieElement = this.getMovieElement();
        if (!movieElement) {
            this.debug("Flash called back ready but the flash movie can't be found.");
            return;
        }
        this.cleanUp(movieElement);
        this.queueEvent("UploadFile_loaded_handler");
    };
    UploadFile.prototype.cleanUp = function (movieElement) {
        try {
            if (this.movieElement && typeof (movieElement.CallFunction) === "unknown") { // We only want to do this in IE
                this.debug("Removing Flash functions hooks (this should only run in IE and should prevent memory leaks)");
                for (var key in movieElement) {
                    try {
                        if (typeof (movieElement[key]) === "function") {
                            movieElement[key] = null;
                        }
                    } catch (ex) {
                    }
                }
            }
        } catch (ex1) {
        }
        window["__flash__removeCallback"] = function (instance, name) {
            try {
                if (instance) {
                    instance[name] = null;
                }
            } catch (flashEx) {

            }
        };
    };
    UploadFile.prototype.fileDialogStart = function () {
        this.queueEvent("file_dialog_start_handler");
    };
    UploadFile.prototype.fileQueued = function (file) {
        file = this.unescapeFilePostParams(file);
        this.queueEvent("file_queued_handler", file);
    };
    UploadFile.prototype.fileQueueError = function (file, errorCode, message) {
        file = this.unescapeFilePostParams(file);
        this.queueEvent("file_queue_error_handler", [file, errorCode, message]);
    };
    UploadFile.prototype.fileDialogComplete = function (numFilesSelected, numFilesQueued, numFilesInQueue) {
        this.queueEvent("file_dialog_complete_handler", [numFilesSelected, numFilesQueued, numFilesInQueue]);
    };
    UploadFile.prototype.uploadStart = function (file) {
        file = this.unescapeFilePostParams(file);
        this.queueEvent("return_upload_start_handler", file);
    };
    UploadFile.prototype.returnUploadStart = function (file) {
        var returnValue;
        if (typeof this.settings.upload_start_handler === "function") {
            file = this.unescapeFilePostParams(file);
            returnValue = this.settings.upload_start_handler.call(this, file);
        } else if (this.settings.upload_start_handler != undefined) {
            throw "upload_start_handler must be a function";
        }
        if (returnValue === undefined) {
            returnValue = true;
        }
        returnValue = !!returnValue;
        this.callFlash("ReturnUploadStart", [returnValue]);
    };
    UploadFile.prototype.uploadProgress = function (file, bytesComplete, bytesTotal) {
        file = this.unescapeFilePostParams(file);
        this.queueEvent("upload_progress_handler", [file, bytesComplete, bytesTotal]);
    };
    UploadFile.prototype.uploadError = function (file, errorCode, message) {
        file = this.unescapeFilePostParams(file);
        this.queueEvent("upload_error_handler", [file, errorCode, message]);
    };
    UploadFile.prototype.uploadSuccess = function (file, serverData, responseReceived) {
        file = this.unescapeFilePostParams(file);
        this.queueEvent("upload_success_handler", [file, serverData, responseReceived]);
    };
    UploadFile.prototype.uploadComplete = function (file) {
        file = this.unescapeFilePostParams(file);
        this.queueEvent("upload_complete_handler", file);
    };
    UploadFile.prototype.debug = function (message) {
        this.queueEvent("debug_handler", message);
    };
    UploadFile.prototype.debugMessage = function (message) {
        if (this.settings.debug) {
            var exceptionMessage, exceptionValues = [];
            if (typeof message === "object" && typeof message.name === "string" && typeof message.message === "string") {
                for (var key in message) {
                    if (message.hasOwnProperty(key)) {
                        exceptionValues.push(key + ": " + message[key]);
                    }
                }
                exceptionMessage = exceptionValues.join("\n") || "";
                exceptionValues = exceptionMessage.split("\n");
                exceptionMessage = "EXCEPTION: " + exceptionValues.join("\nEXCEPTION: ");
                UploadFile.Console.writeLine(exceptionMessage);
            } else {
                UploadFile.Console.writeLine(message);
            }
        }
    };
    UploadFile.Console = {};
    UploadFile.Console.writeLine = function (message) {
        var console, documentForm;
        try {
            console = document.getElementById("UploadFile_Console");
            if (!console) {
                documentForm = document.createElement("form");
                document.getElementsByTagName("body")[0].appendChild(documentForm);

                console = document.createElement("textarea");
                console.id = "UploadFile_Console";
                console.style.fontFamily = "monospace";
                console.setAttribute("wrap", "off");
                console.wrap = "off";
                console.style.overflow = "auto";
                console.style.width = "700px";
                console.style.height = "350px";
                console.style.margin = "5px";
                documentForm.appendChild(console);
            }
            console.value += message + "\n";
            console.scrollTop = console.scrollHeight - console.clientHeight;
        } catch (ex) {
            alert("发生错误: " + ex.name + " 错误信息: " + ex.message);
        }
    };
    return UploadFile;
});