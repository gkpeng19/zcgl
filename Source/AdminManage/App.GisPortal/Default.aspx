<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GIS.Portal.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>北京市园林绿化资源动态监管系统-主页</title>
    <link href="images/favicon.png" type="image/x-icon" rel="Shortcut Icon" />
    <style type="text/css">
        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }
        a {
            text-decoration: none;
        }
        body, html {
            width: 100%;
            height: 100%;
            margin: 0;
            padding: 0;
            overflow: hidden;
            background-color: #DDD;
            font: normal normal normal 12px '宋体' Arial sans-serif;
        }
        @media print { 
            .noprint { 
                display: none; 
            }
            .print {
                position:fixed;
                top:0;
                bottom:0;
                left:0;
                right:0;
                z-index:9999;
            }
        } 
        #main-loading {
            width: 100%;
            height: 100%;
            background-color: #344357;
            text-align: center;
            overflow: hidden;
        }
        #main-loading #app-loading, #main-loading #ie-note {
            position: absolute;
            top: 0;
            bottom: 0;
            left: 0;
            right: 0;
            margin: auto;
        }
        #main-loading #app-loading {
            width: 100%;
            height: 300px;
        }
        #main-loading .app-name {
            font: 20px arial;
            position: absolute;
            z-index: 2;
            left: 0;
            right: 0;
            margin: auto;
        }
        #main-loading img {
            position: relative;
            display: block;
            margin: auto;
        }
        #main-loading .loading-info {
            font: 14px 'arial';
            margin-top: 30px;
            overflow: hidden;
            position: relative;
        }
        #main-loading .loading-info .loading {
            width: 260px;
            height: 4px;
            border-radius: 2px;
            background-color: #31659b;
            margin: auto;
        }
        #main-loading .loading-info .loading-progress {
            height: 4px;
            border-radius: 2px;
            background-color: white;
        }
        #main-loading #ie-note {
            width: 586px;
            height: 253px;
            background-image: url('images/notes.png');
            padding: 0 30px 40px 30px;
            font-size: 14px;
            color: #596679;
            background-repeat: no-repeat;
        }
        #ie-note .hint-title {
            height: 40px;
            line-height: 48px;
            text-align: left;
            font-weight: bold;
        }
        #ie-note .hint-img {
            background-image: url('images/hint.png');
            background-position: left;
            padding-left: 40px;
            margin-top: 20px;
            background-repeat: no-repeat;
            height: 30px;
            text-align: left;
            line-height: 30px;
            font-weight: bold;
        }
        #ie-note span {
            display: block;
            line-height: 14px;
        }
        #main-page {
            display: none;
            width: 100%;
            height: 100%;
            position: relative;
        }
        .hidden {
            visibility: hidden;
            position: absolute;
            top: 30px;
            right: 20px;
            z-index: 10;
        }
        .in {
            visibility: visible;
            position: absolute;
            top: 30px;
            right: 20px;
            z-index: 10;
        }
        #main-page .header {
            position: absolute;
            height: 70px;
            width: 100%;
            background: url(images/bg.png) repeat-x;
            z-index: 99;
        }
        #main-page .header .sys-info {
            position: absolute;
            left: 0;
            width: 370px;
        }
        #main-page .header .logo {
            height: 70px;
            width: 70px;
            background: url(images/logo.png) no-repeat;
            float: left;
        }
        #main-page .header .ld-geocoder {
            position: absolute;
            right: 0;
            width: 280px;
            height: 70px;
        }
        #main-page .header .ld-geocoder .geocoder-table {
            float: right;
            margin-top: 5px;
            margin-right: 20px;
        }
        #main-page .header .ld-geocoder .geocoder-table td {
            padding: 2px 5px;
        }
        #main-page .header .ld-geocoder .geocoder-table td p {
            color: #FFF;
        }
        #main-page .header .ld-geocoder .geocoder-table td p a {
            padding: 0px 2px;
            cursor: pointer;
        }
        #hotwords a {
            color: #F00;
        }
        /*@media screen and (max-width: 640px) {
            #main-page .header .menu {
                left: 400px;
            }
        }*/
        #main-page .header .title {
            height: 70px;
            width: 300px;
            float: left;
            color: #FFF;
        }
        #main-page .header .title span {
            cursor: pointer;
            max-width: 75px;
            overflow: hidden;
        }
        #main-page .header .title span:hover {
            color: #f00;
        }
        #main-page .header .menu {
            position: absolute;
            right: 20px;
            left:550px;
            height: 70px;
            overflow:hidden;
        }
        #main-page .header .menu .menu-list {
            line-height: 70px;
            margin: 0;
            /*position: absolute;*/
            white-space: nowrap;
            float:right;
        }
        #main-page .header .menu .menu-list li {
            display: inline-block;
        }
        #main-page .header .menu .menu-list li a {
            color: #FFF;
            padding: 7px;
            border-radius: 5px;
        }
        #main-page .header .menu .menu-list li i {
            padding: 4px 2px;
            background: url(images/blank.png) no-repeat center;
        }
        #main-page .header .menu .menu-list li input {
            height: 30px;
            width: 210px;
            border-radius: 5px;
            margin-left: 20px;
        }
        #main-page .header .menu .menu-list li a:hover {
            background-color: #31659b;
        }
        #main-page .leftNav {
            position: absolute;
            top: 70px;
            bottom: 0;
            left: 0;
            width: 70px;
            background: url(images/left.png) repeat-y;
            /*overflow: hidden;*/
        }
        ul, ul li {
            list-style: none;
        }
        /*#main-page .leftNav ul li {
            font-size: 0;
            width: 70px;
            height: 100%;
        }*/
        #main-page .middleNav {
            position: absolute;
            top: 70px;
            bottom: 0;
            left: 70px;
            width: 315px;
            background: #EBEDEF;
            z-index: 9;
            border-right: 1px solid #A19F9C;
        }
        #main-page .middleNav .middle-header {
            height: 30px;
            width: 100%;
            background: url(images/nav.png) repeat-x;
            -moz-box-shadow: 0 1px 10px #7B7F84;
            -webkit-box-shadow: 0 1px 10px #7B7F84;
            box-shadow: 0 1px 10px #7B7F84;
            z-index: 9;
        }
        #main-page .middleNav .collpse {
            height: 58px;
            width: 14px;
            right: -15px;
            background: url(images/col-left.png) no-repeat;
            position: absolute;
            margin-top: -29px;
            top: 50%;
            cursor: pointer;
        }
        #main-page .leftNav img {
            cursor: pointer;
        }
        #main-page .rightNav {
            position: absolute;
            top: 70px;
            bottom: 0;
            left: 385px;
            right: 0px;
        }
        #main-page .rightNav .navHeader {
            position: relative;
            top: 0;
            right: 0;
            left: 0;
            height: 30px;
            background: url(images/nav.png) repeat-x;
            -moz-box-shadow: 0 1px 10px #7B7F84;
            -webkit-box-shadow: 0 1px 10px #7B7F84;
            box-shadow: 0 1px 10px #7B7F84;
            z-index: 19;
        }
        #main-page .rightNav .navContent {
            position: absolute;
            top: 30px;
            right: 0;
            left: 0;
            bottom: 0;
            background-color: #DDD;
        }
        .clearfix:before,
        .clearfix:after {
            display: table;
            content: " ";
        }
        .clearfix:after {
            clear: both;
        }
        img {
            border: 0;
        }
        #toggle-city {
        }
        #toggleDistrict {
            float: left;
            line-height: 30px;
            margin-left: 10px;
            z-index: 19;
        }
    </style>
    <%--<link rel="stylesheet" href="css/print.css" media="print" />--%>
    <script>
        var progress;
        function loadingCallback(url, i, count) {
            var loading = document.getElementById('main-loading-bar');
            loading.setAttribute('title', url);
            if (!progress) {
                progress = document.createElement('div');
                progress.setAttribute('class', 'loading-progress');
                loading.appendChild(progress);
            }
            progress.style.width = (((i - 1) / count) * 100) + '%';
        }
    </script>
</head>
<body class="claro" oncontextmenu='return false;' ondragstart='return false;' onselectstart='return false;'>
    <!-- 资源中心登录 -->
    <div id="main-loading" class="noprint">
        <div id="app-loading">
            <div class="app-name">
                <h3 style="color: white">欢迎进入北京市园林绿化动态监管平台</h3>
            </div>
            <img src="images/loading.gif" />
            <div class="loading-info">
                <div id="main-loading-bar" class="loading">
                </div>
            </div>
        </div>
        <div id="ie-note" style="display: none;">
            <div class="hint-title">
                浏览器版本不兼容
            </div>
            <div class="hint-img">
                对不起，本程序不支持您的浏览器，请更换浏览器后重试！
            </div>
            <p class="hint-text">
                <span>提示：本程序不支持IE9以下浏览器，请将您的浏览器升级到IE9以上版本！</span>
                <br />
                <span>或者使用非IE浏览器来查看！</span>
            </p>
        </div>
    </div>
    <div id="main-page">
        <div class="header clearfix noprint">
            <div class="sys-info clearfix">
                <div class="logo"></div>
                <div class="title">
                    <img src="images/title.png" />
                    <span id="currentUserName"></span>
                </div>
            </div>
            <div class="menu">
                <ul class="menu-list clearfix" id="ul_menu"></ul>
            </div>
        </div>
        <div id="user-privs" class="leftNav noprint" style="z-index: 99">
            <img id="up-func" src="images/func/up.png" />
            <div id="ld-menu-category" style="position: absolute; bottom: 45px; top: 45px; overflow: hidden;">
                <ul id="ld-category" class="clearfix">
                </ul>
            </div>
            <img id="down-func" src="images/func/down.png" style="bottom: 0; position: absolute; left: 0;" />
        </div>
        <div id="leftNav" class="leftNav middleNav noprint">
            <div class="middle-header">
                <span style="line-height: 30px; margin-left: 10px;">共有<span id="sel-layers-num" style="color: #F00;">5</span>个图层</span>
            </div>
            <div id="layerOperPanel" class="leftContent">
                <div class="searchContent">
                    <input id="searchText" class="form-control" type="text" />
                    <button id="btnSearch" class="btn btn-primary">
                        <img src="images/search.png" />
                    </button>
                </div>
                <hr />
                <div class="searchFactor" style="overflow-y: auto; height: 120px;">
                    <div class="clearfix">
                        <div class="searchTitle clearfix">
                            <img src="images/layer-title.png" />
                            <span id="layerOperPanel_className" class="title">公园风景区</span>
                            <button id="btnChooseAll" class="btn btn-default titlebtn">全选</button>
                            <button id="btnChooseNone" class="btn btn-default titlebtn">全不选</button>
                        </div>
                        <ul id="ulLayersList">
                        </ul>
                    </div>
                    <div class="clearfix">
                        <div id="yearOrCategory" class="searchTitle">
                            <img src="images/layer-title.png" />
                            <span class="title">分类</span>
                        </div>
                        <ul id="ulYearsList">
                        </ul>
                    </div>
                </div>
                <div id="search-results" class="searchfuncs">
                    <ul class="navigation clearfix" role="tablist" id="UlEnterpriseInfo">
                        <li class="active"><a data-toggle="Datalist">数据列表</a></li>
                        <li><a data-toggle="BufferAnalyst">缓冲区分析</a></li>
                        <li><a data-toggle="HistoryAnalyst">历史分析</a></li>
                        <li><a data-toggle="StatAnalyst">数据统计</a></li>
                    </ul>
                    <div class="tab-content">
                        <div class="tab-pane show-pane" id="Datalist">
                            <div id="featureResultsLength" style="text-align: center;"></div>
                            <ul id="search-result-list" class="search-result-list">
                            </ul>
                            <div class="pagination-container">
                                <div id="pagination-list">
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="BufferAnalyst">
                        </div>
                        <div class="tab-pane" id="HistoryAnalyst">
                        </div>
                        <div class="tab-pane" id="StatAnalyst">
                        </div>
                    </div>
                </div>
            </div>
            <div id="collpse-left" class="collpse"></div>
        </div>
        <div id="rightNav" class="rightNav">
            <div class="navHeader noprint">
                <!-- 区域切换 -->
                <div id="toggleDistrict" style="vertical-align: middle">
                    <img src="images/location.png" />
                    <span>当前区域：</span>
                    <a id="toggle-city">北京市</a>
                    <button id="editMap" class="btn btn-default">地图标注</button>
                </div>
                <!-- 底图切换 -->
                <div id="toggleBaseLayer">
                </div>
            </div>
            <div id="navContent" class="navContent">
                <div id="cNavigation" class="noprint">
                </div>
                <div id="mapDiv" style="position: absolute; top: 0; bottom: 0; right: 0; left: 0;background:#FDFDFD">
                </div>
                <div id="real-coordinate" class="noprint">
                </div>
            </div>
        </div>
    </div>
    <%--<iframe width="0" height="0" name="saveFrame" frameborder='no' border='0' allowtransparency='yes' style="display:none;"></iframe>--%>  
    <script src="js/index.js"></script>
    <div id='jz-street' class='noprint'>
        <iframe id='returnToMapFrame' src='street/street-return.html' width='90' height='30' style='border-radius: 5px; position: absolute; z-index: 999999; margin-top: 30px; margin-left: 30px;'
            frameborder='no' border='0' marginwidth='0' marginheight='0' scrolling='no'
            allowtransparency='yes'></iframe>
        <div id='jz-street-object'>
        </div>
        <div id='jz-street-map' class='jz-street-map'>
            <iframe id='streetMapFrame' src='street/street-foot.html' width='300' height='300'
                style='position: absolute; z-index: 999999; bottom: 0px; left: 0px; -webkit-border-radius: 5px; -moz-border-radius: 5px; -ms-border-radius: 5px; -o-border-radius: 5px;'
                frameborder='no' border='0' marginwidth='0' marginheight='0' scrolling='no'
                allowtransparency='yes'></iframe>
        </div>
    </div>
    <script type='text/javascript' for='PPVision' event='onInit()'>
        allviewinit();
    </script>
    <script type='text/javascript' for='PPVision' event='onPosition(lon,lat,alt,dirx,diry,dirz,fovx)'>
        //响应视点移动
        var s = '(' + lon + ',' + lat + ',' + alt + '), (' + dirx + ',' + diry + ',' + dirz + '), (' + fovx + ')';
        allViewMove(s, lon, lat);
    </script>
    <script type='text/javascript'>
        function allviewinit() {
            dojo.require('dojo.topic');
            var PPVision = document.getElementById('PPVision');
            if (PPVision) {
                PPVision.setServer(JZ.config['PPVisionServer'][0].url);
                var m = document.getElementById('jz-street-object');
                var spx = m.getAttribute('px');
                var spy = m.getAttribute('py');
                px = parseFloat(spx);
                py = parseFloat(spy);
                PPVision.locate(4, px, py, 0.0);
            }
        }
        function allViewLocate(px, py) {
            var PPVision = document.getElementById('PPVision');
            if (PPVision) {
                PPVision.locate(4, px, py, 0.0);
                dojo.topic.publish('topic/StreetViewMoveComplete', px, py);
            }
        }
        function allViewMove(s, px, py) {
            //var s1 = s;
            //var px1 = px;
            //var py1 = py;
            dojo.topic.publish('topic/StreetViewMoveComplete', px, py);
        }
        function editComplete(result) {
            if (result === 1) {
                alert('要素更新成功！');
                JZ.editDialog.hide();
                JZ.mc.infoWindow.hide();
            } else if (result === 3) {
                alert('要素更新失败，请重试!');
            } else if (result === 2) {
                alert('要素删除成功！');
                JZ.editDialog.hide();
                JZ.mc.infoWindow.hide();
            } else if (result === 4) {
                alert('要素删除失败，请重试!');
            }
        }
    </script>
</body>
</html>
