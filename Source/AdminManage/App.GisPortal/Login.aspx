<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="GIS.Portal.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
	<title>北京市园林绿化资源动态监管系统-登录</title>
    <link href="images/favicon.png" type="image/x-icon" rel="Shortcut Icon"/>
    <link rel="stylesheet" href="css/login.css"/>
</head>
<body>
    <div class="wrapper">
        <div id="ie-note" style="display: none;">
            <div class="hint-title">
                发生错误
            </div>
            <div class="hint-img">
                对不起，本程序不支持您的浏览器，请更换IE8以上浏览器后重试！
            </div>
        </div>

        <div id="main-login" class="login">
            <div class="row1">
                <span class="l-title"></span>
                <%--<span class="r-title"></span>--%>
            </div>
            <form id="loginFrm" runat="server">
                <div class="row2">
                    <div class="content">
                        <div class="login-custom">
                            <div>
                                <label for="username">用户名:</label>
                                <input id="username" class="input-large" name="username" type="text" runat="server" />
                            </div>
                            <div>
                                <label for="password">密　码:</label>
                                <input id="password" class="input-large" name="password" type="password" runat="server" />
                            </div>
                            <div style="height:30px;line-height:30px;margin-top:-7px;">
                                <input id="rememberPwd" name="remember" style="vertical-align:middle;" type="checkbox" runat="server" />
                                <label style="vertical-align:middle;" for="rememberPwd">记住密码？</label>
                            </div>
                        </div>
                        <div class="login-credit">
                        </div>
                    </div>
                </div>
                
                <div class="row3">
                    <span id="btnLogin" class="submit-btn" runat="server"></span>
                </div>
            </form>
        </div>
    </div>
    <script type="text/javascript">
        var ie = (function () {
            var undef,
			v = 3,
			div = document.createElement('div'),
			all = div.getElementsByTagName('i');
            div.innerHTML = '<!--[if gt IE ' + (++v) + ']><i></i><![endif]-->';
            while (all[0]) {
                div.innerHTML = '<!--[if gt IE ' + (++v) + ']><i></i><![endif]-->';
            }
            return v > 4 ? v : undef;
        }());

        if (ie < 8) {
            var mainLogin = document.getElementById('main-login');
            var ieNotes = document.getElementById('ie-note');
            mainLogin.style.display = 'none';
            ieNotes.style.display = 'block';
        }
    </script>
    <script type="text/javascript" src="js/login.js"></script>
</body>
</html>

