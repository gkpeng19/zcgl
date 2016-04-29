(function () {
    loginAuto();

    function loginAuto() {
        var strAllCookie = document.cookie;
        var cookies = {};
        if (strAllCookie) {
            var strCookies = strAllCookie.split(';');
            for (var i = 0; i < strCookies.length; i++) {
                var splits = strCookies[i].split('=');
                if (splits && splits.length > 1) {
                    cookies[splits[0].replace(/^\s+|\s+$/gm, '')] = splits[1];
                }
            }
            document.getElementById("username").value = cookies["username"];
            document.getElementById("password").value = cookies["password"];
            document.getElementById("rememberPwd").checked = true;
        }
    }
    function addCookie(objName, objValue, day) {
        var str = objName + "=" + escape(objValue);
        if (day > 0) {
            var date = new Date();
            var ms = day * 3600 * 1000;
            date.setTime(date.getTime() + ms);
            str += "; expires=" + date.toGMTString();
        }
        document.cookie = str;
    }

    var btn = document.getElementById("btnLogin");
    var AttachEvent = function (node, type, callback) {
        if (node.addEventListener) {
            node.addEventListener(type, callback);
        } else if (node.attachEvent) {
            node.attachEvent("on" + type, callback);
        } else {
            node["on" + type] = callback;
        }
    }
    //停止冒泡
    function stopBubble(e) {
        if (e && e.stopPropagation) {
            e.stopPropagation();
        } else {
            window.event.cancelBubble = true;
        }
    };

    function submit() {
        var username = document.getElementById("username").value;
        var password = document.getElementById("password").value;

        if (username === "" || password === "") {
            alert("用户名或密码不能为空，请重试!");
        } else {
            if (document.getElementById("rememberPwd").checked) {
                addCookie("username", username, 10);
                addCookie("password", password, 10);
            }
            document.getElementById("loginFrm").submit();
        }
    }

    function monitorSubmit(e) {
        stopBubble(e);
        e = e || window.event;
        if (e.keyCode === 13 || e.which === 13) {
            btn.click();
        }
    }

    AttachEvent(btn, "click", submit);
    AttachEvent(document, "keydown", monitorSubmit);
})();



