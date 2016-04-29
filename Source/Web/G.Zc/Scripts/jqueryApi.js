var $api = {
    //api_address: "http://api.hollysmart.com.cn/",
    api_address: "http://localhost:63655/",
    bearer_token: null,
    get: function (url, data, callback, async) {
        this.ajax("GET", url, data, callback, async);
    },
    post: function (url, data, callback, async) {
        this.ajax("POST", url, data, callback,async);
    },
    put: function (url, data, callback, async) {
        this.ajax("PUT", url, data, callback,async);
    },
    delete: function (url, data, callback, async) {
        this.ajax("DELETE", url, data, callback,async);
    },
    ajax: function (method, url, data, callback, async) {
        if (!this.bearer_token) {
            this.bearer_token = $.cookie('Bearer_Token');
            if (!this.bearer_token || this.bearer_token == '[object Object]') {
                location.href = "Login.html";
                return;
            }
        }

        $.ajax({
            type: method,
            url: this.api_address + url,
            data: data,
            async: (async == false ? false : true),
            dataType: "json",
            headers: {
                "Authorization": "Bearer " + this.bearer_token
            },
            success: function (result) {
                callback(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(XMLHttpRequest.responseJSON.Message);
            }
        });
    },
    loginIn: function (username, password, authcode, isRemember, callback) {
        $.ajax({
            type: "POST",
            url: this.api_address + "token?authcode=" + authcode,
            data: { grant_type: "password", username: username, password: password, ran: Math.random() },
            dataType: "json",
            success: function (result) {
                if (result.access_token && result.access_token.length > 0) {
                    var config = { path: '/' };
                    if (isRemember) {
                        config.expires = 10;
                    }

                    $.cookie('Bearer_Token', result.access_token, config);

                    callback(1, "登录成功。");
                }
                else {
                    callback(0, "未知错误！");
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                callback(0, XMLHttpRequest.responseJSON.error);
            }
        });
    },
    loginOut: function () {
        $.cookie('Bearer_Token', { path: '/' }, null);
        location.href = "Login.html";
    }
}

var loginInfo = null;
var uAuths = {};

if (!$api.bearer_token) {
    $api.bearer_token = $.cookie('Bearer_Token');
    //if (!$api.bearer_token || $api.bearer_token == '[object Object]') {
    //    location.href = "Login.html";
    //}
}

if ($api.bearer_token && $api.bearer_token != '[object Object]') {
    $.ajax({
        type: "GET",
        url: $api.api_address + "api/Common/GetLoginInfo",
        data: {},
        async: false,
        dataType: "json",
        headers: {
            "Authorization": "Bearer " + $api.bearer_token
        },
        success: function (result) {
            loginInfo = $.toJsResult(result);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        }
    });

    var pageId = window.parent.getCurrPageId ? window.parent.getCurrPageId() : -1;
    if (pageId > 0) {
        $.ajax({
            type: "GET",
            url: $api.api_address + "api/Common/GetUserOpBtns",
            data: { pageid: pageId },
            async: false,
            dataType: "json",
            headers: {
                "Authorization": "Bearer " + $api.bearer_token
            },
            success: function (result) {
                var userAuthorityBtns = $.toJsResult(result);
                $(userAuthorityBtns).each(function () {
                    uAuths[this.code] = true;
                });
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(XMLHttpRequest.responseJSON.Message);
            }
        });
    }
}