<%@ page language="C#" autoeventwireup="true" codebehind="Thematic.aspx.cs" inherits="GIS.Portal.Thematic" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>北京市园林绿化资源动态监管系统-主页</title>
    <link href="../images/favicon.png" type="image/x-icon" rel="Shortcut Icon" />
    <link rel="stylesheet" type="text/css" href="../esri/dijit/themes/claro/claro.css"/>
    <link rel="stylesheet" type="text/css" href="../esri/esri/css/esri.css" />
    <link rel="stylesheet" type="text/css" href="../css/thematic.css" />
    <link rel="stylesheet" type="text/css" href="../css/bootstrap.css" />
    <script>
        var JZ = {
            mc: null,
            ie: null,
            en: null
        };
    </script>
</head>
<body class="claro" oncontextmenu='return false;' ondragstart='return false;' onselectstart='return false;'>
    <div id="main-page">
        <div class="header clearfix">
            <div class="sys-info clearfix">
                <div class="logo"></div>
                <div class="title">
                    <img src="../images/title-th.png" />
                    <%--<span id="currentUserName"></span>--%>
                </div>
            </div>
            <div class="menu">
                <ul class="menu-list clearfix">
                    <li><a href="../Default.aspx">返回首页</a></li>
                    <li><a id="thematicLegend" style="cursor:pointer">图例信息</a></li>
                    <li><a href="/pages/API.aspx" target="_blank">地图API</a></li>
                    <li><a href="/pages/MetaData.aspx" target="_blank">地图元数据</a></li>
                    <li><a href="/pages/ResourceService.aspx" target="_blank">服务资源</a></li>
                </ul>
            </div>
            <div class="menu1">
                <span style="color:#FFF;cursor:pointer;padding:0 0 0 7px;">
                    当前用户：
                    <a id="currentUserName" style="color:#F00;"></a>
                </span>
            </div>
        </div>
        <div id="leftNav" class="leftNav middleNav">
            <div class="middle-header">
                <span style="line-height: 30px; margin-left: 10px;">专题图</span>
            </div>
            <div id="layerOperPanel" class="leftContent">
                <div id="layerTree" class="tree"></div>
            </div>
            <div id="collpse-left" class="collpse"></div>
        </div>
        <div id="rightNav" class="rightNav">
            <div id="navContent" class="navContent">
                <div id="cNavigation">
                </div>
                <div id="chart"></div>
                <div id="mapDiv" style="position: absolute; top: 0; bottom: 0; right: 0; left: 0;">
                </div>
            </div>
        </div>
    </div>
    <script src="js/config.js"></script>
    <script src="../esri/init.js"></script>
    <script src="js/thematic.js"></script>
</body>
</html>
