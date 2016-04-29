<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConstrastAnalyst.aspx.cs" Inherits="GIS.Portal.ConstrastAnalyst" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="images/favicon.png" type="image/x-icon" rel="Shortcut Icon" />
    <link rel="stylesheet" href="../esri/dijit/themes/claro/claro.css" />
    <link rel="stylesheet" href="../css/bootstrap.css" />
    <link rel="stylesheet" href="../css/parent.css" />
    <link rel="stylesheet" href="../esri/esri/css/esri.css" />
    <title>对比分析</title>
    <style type="text/css">
        .main-content {
            width:100%;
            padding-left:0; 
            padding-right:0;
            margin-top:10px;
            margin-bottom:10px;
        }
        @media screen and (max-width: 1300px){
            .main-content {
                width:1295px;
                padding-left:0; 
                padding-right:0;
                margin-top:10px;
                margin-bottom:10px;
            }
            .header {
                width:1295px;
            }
            .footer {
                width:1295px;
            }
        }
        .maptools {
            width: 19.33333333%;
            border:1px solid #ddd;
            border-bottom:2px solid #DDD;
            height:800px;
            float:left;
            min-width:250px;
            position:relative;
        }
        .mapgroup {
             width: 80.66666666%;
             float:left;
        }
        .mapcontent {
            position:relative;
            width: 50%;
            float:left;
            height:800px;
            border:1px solid #DDD;
            border-bottom:2px solid #DDD;
        }
        .mapdiv {
            position:absolute;
            top:30px;bottom:0;left:0;right:0;
        }
        .map-tools {
            background: #F5F5F5; 
            position: absolute;
            top: 0px;
            height: 30px;
            right: 0px;
            left: 0px;
            box-shadow: 0 1px 10px #7B7F84;
            -webkit-box-shadow: 0 1px 10px #7B7F84;
            -moz-box-shadow: 0 1px 10px #7B7F84;
            -o-box-shadow: 0 1px 10px #7B7F84;
            z-index: 99;
        }
        .header .h-row, .banner, .content {
            width:100%;
        }
.map-switcher{
	position:absolute;
    right:0px;
	top:0px;
	height:30px;
	width:245px;
    margin-left:-122px;
	z-index:1;
	border-radius:5px;
}
.map-switcher ul {
	margin: 0px;
	padding: 0px;
    font-size:12px;
}
.map-switcher a.selected{
	background-color:#D5D1CD;
}
.map-switcher ul li {
	float: left;
	list-style: none;
	height: 30px;
	line-height: 30px;
	display: inline;
	text-align: center;
	cursor:pointer;
}
.map-switcher ul li a {
	color: #000;
	line-height: 30px;
	width: 80px;
	margin: 0px;
	padding: 0px;
	display: block;
	text-decoration: none;
}
.map-switcher ul li a span {
	padding: 0px 8px 0px 8px;
}
.map-switcher ul li a span.jz-map-fullscreen {
	background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAMHSURBVDhPfZNJTFNRFIYbYohxZVi5MKxcGImJC3XpEBMTNg4IBHEoPsZESKkMBWx5hYZUbKBMqVTCIIUWy1ApECCMjQUpgwVeKx0YhQJtKQ0iIQ3v9vgePGMao19yNvee7/7n3uSyfoPjEBRfPRWbqTCq+Mr5L68avrY/K9OnJMmnzzAt/4ZdbziX0TQ3VTuwDKVai0/cZnJIOkxI3muD3PpJ++OivotM699EqU3B/BazsazHgkq6bPIl1yHm2vOnrzh9cUKlUfxGbTzkNxjW2LjmLKMEklAzk1Q7SCV32yrdP5HU9QPZt7zIveFB9lUPkhYoZ7LLNPOAFfeKGSWQXBXRU6xZ2F/f8T2n5U0vQg6vH1Z3SLTsJu2LLjLmdd2YPbtaZ2WUQArU5gVRm3mOHptK9tDymgfBshuB3Ul6LNtkenrVSGdejf6QUQLhq+Y/F7URW9t75FPHLlpao5JXdhDYnCRa2CLt3zbImAyZbor3bsTBKIEkyydE7/ttoJ1cL949QP2ufdL9fZfcszjJJdMmkkpaZyPEzQbEFmkVjBJIfMl4SIV2wZPV+/YI00UBXcLxQiP1BqnSdtM9fq1+pbrLBBFZKunDbMV5RvsDAIQarC5+Wl/mUZWVD7JFAaT2p7k/jljnKjtmj4qVBp9MS4BUQ0Bm1bD7EU91i1FPSBnCFKlDicRt9VUvbmSDiIiDcM1N4I3kwcv2/IPOUUs0t3zQIW4lQKicBU7FkO8+pzGO0VkseuQqqwBo+cnQJcDGwkCyhEHjthCSxqLB7/fHcCTdN7gVw858xTTk1E0Ap3wAYnOaxDiOBx0fILMLoHA+Dl7owyBl+jJIV+Oh2S2E5PGTA6hrno7MabyQIOo059TogSvTAU8+CtHc+pKTKwwnEnfarnklixiUrmJw99N1L0eXSNB7lHyFqlP0tJHcmpCY7KYBXvUoCOrHABO0mI8fkUqJSBtNJD5s49DkEgIt02v0HlUBvzEKVwdTyRJMoDKGs6UPjhfpRjqNvjM9NpMcerz5X1isXyzyBFY8V2I/AAAAAElFTkSuQmCC)
}
.map-switcher ul li ul {
	display:none;
}
.map-switcher ul li ul li {
	float:none;
	height: 30px;
}
.map-switcher ul li ul li a {
	background: #F2F1EF;
	line-height: 30px;
}
.map-switcher ul li a:hover {
	background: #D5D1CD;
}
.map-switcher ul li:hover ul {
	display: block;
}
.map-switcher ul li ul li a:hover {
	background: #D5D1CD;
}
.map-switcher ul li ul li.arrow {
	height: 15px;
	line-height: 15px;
}
.map-switcher ul li ul li.arrow span {
	display: block;
	width: 0;
	height: 0;
	border-width: 0 10px 15px;
	border-style: solid;
	border-color: transparent transparent #006400;
	margin: 0 auto;
}
.map-switcher .basemap{
	border-right:1px #B0B0AE solid;
    background:#F2F1EF;
}
.jz-tab .jz-tab-nav {
    list-style: none;
    padding: 0;
}

.jz-tab .jz-tab-nav li {
    list-style: none;
    float: left;
    padding: 5px 0;
    border-right: 1px solid #ccc;
    border-bottom: 1px solid #C4C1BB;
    cursor: pointer;
    text-align: center;
    margin-right:0;
}

.jz-tab .jz-tab-nav li:last-child {
    border-right: none;
}

.jz-tab .jz-tab-nav li.active {
    border-top: 1px solid #008CBD;
    border-bottom: none;
    margin-top: -1px;
}

.jz-tab .jz-tab-nav li a {
    text-decoration: none;
    color: #ACA8A4;
    outline: none;
}

.jz-tab .jz-tab-nav li:hover a {
    background: #FFF;
    color: #0C88CB;
}

.jz-tab .jz-tab-nav li.active a {
    background: #FFF;
    color: #0C88CB;
}

.jz-tab .jz-tab-nav li:hover {
    border-top: 1px solid #008CBD;
    margin-top: -1px;
}

.jz-tab .jz-tab-content {
    background: #FFF;
}

.jz-tab .jz-tab-content .jz-tab-pane {
    display: none;
}

.jz-tab .jz-tab-content .jz-tab-pane .jz-grid {
    max-height: 190px;
    overflow-y: auto;
    margin: 0 5px;
}
.custom-note {
    margin:0 auto;
    padding:10px 20px;
}
.tab-content .tab-pane {
    background:#FFF;
}
.loading-indicator{
  position: fixed;
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
  background:#ccc;
  opacity:0.5;
  z-index:99;
  display:none;
}
.widget-loading{
  position: absolute;
  top: 50%;
  margin-top: -20px;
  left: 50%;
  margin-left: -68px;
  text-align:center;
}
    </style>
    <script type="text/javascript">
        var apiPath = window.location.protocol + "//" + window.location.host + "/";
        //需要预先加载的资源
        var dojoConfig = {
            parseOnLoad: false,
            async: true,
            tlmSiblingOfDojo: false,
            has: {
                'extend-esri': 1
            },
            packages: [{
                name: "widget",
                location: apiPath + "widget"
            }]
        };
    </script>
    <script type="text/javascript" src="../esri/init.js"></script>
</head>
<body class="claro">
    <div class="header">
        <div class="h-row clearfix">
            <div class="h-left">
                <div class="h-top">
                    <ul class="clearfix">
                        <li><a href="../Default.aspx" rel="tooltip" title="主页" data-toggle="tooltip">主页</a></li>
                        <li><span>|</span></li>
                        <li><a href="../pages/MetaData.aspx" rel="tooltip" title="元数据管理" data-toggle="tooltip">元数据管理</a></li>
                        <li><span>|</span></li>
                        <li><a href="../pages/API.aspx" rel="tooltip" title="地图调用接口" data-toggle="tooltip">地图API</a></li>
                        <li><span>|</span></li>
                        <li><a class="h-a-clear" href="../pages/ResourceService.aspx" rel="tooltip" title="服务资源" data-toggle="tooltip">服务资源</a></li>
                    </ul>
                </div>
                <div class="logo"></div>
            </div>
        </div>
    </div>
    <div class="container-fluid main-content">
        <div class="maptools">
            <div class="map-tools" style="line-height:30px;padding-left:10px;">
                分析方法
            </div>
            <div style="position:absolute;top:30px;bottom:0;left:0;right:0;background:#F2F2F2;">
                <div>
                    <ul class="nav nav-tabs" role="tablist">
                        <li id="liAreaContrast" class="active"><a id="aAreaContrast" href="javascript:void(0);">按区域对比</a></li>
                        <li id="liCustomContrast"><a id="aCustomContrast" href="javascript:void(0);">自定义对比</a></li>
                    </ul>
                    <div class="tab-content">
                        <div class="tab-pane active" id="tabAreaContrast">
                            <div class="custom-note">
                                <div class="jz-info" id="tabAreaFeatures">
                                    <label>选择风景区：</label>
                                </div>
                            </div>
                            <div class="custom-note">
                                <p>描述：点击下方"开始比对"按钮，系统将自动对比所选风景区的差异情况，下方的表格中显示的是系统实时对比状态。</p>
                                <button class="btn btn-primary" id="btnAreaStartAnalyst">开始比对</button>
                            </div>
                            <%--<div class="custom-note" style="margin-top:10px;">
                                <p>描述：在系统进行对比的过程中，点击下方的"停止"按钮，停止当前比对过程。</p>
                                <button class="btn btn-primary" style="z-index:999;" id="btnAreaEndAnalyst">停止</button>
                            </div>--%>
                            <%--<div class="custom-note" style="margin-top:10px;">
                                <p>描述：在弹出的"对比结果"页面确定差异后，可利用右侧的"标记按钮在地图中将差异位置与范围标示出来"。</p>
                                <button class="btn btn-primary" id="btnAreaMarker">标记</button>
                                <button class="btn btn-primary" id="btnAreaClear">清除</button>
                            </div>--%>
                            <%--<div class="custom-note" style="margin-top:10px;">
                                <p>描述：显示对比实时信息。</p>
                                <div class="jz-info">
                                    <label>总图块数：</label>
                                    <input type="text" class="form-control" id="areaAllLength"/>
                                </div>
                                <div class="jz-info">
                                    <label>当前图块：</label>
                                    <input type="text" class="form-control" id="areaCurrentLength"/>
                                </div>
                                <div class="jz-info">
                                    <label>对比状态：</label>
                                    <input type="text" class="form-control" id="areaCurrentStatus"/>
                                </div>
                                <div class="jz-info">
                                    <label>当前信息：</label>
                                    <input type="text" class="form-control" id="areaCurrentInfo"/>
                                </div>
                            </div>--%>
                        </div>
                        <div class="tab-pane" id="tabCustomContrast">
                            <div class="custom-note">
                                <p>描述：点击下方"添加区域"按钮，然后在地图上框出需要对比的区域。</p>
                                <button class="btn btn-primary" id="btnCustomSelect">选择区域</button>
                            </div>
                            <div class="custom-note" style="margin-top:10px;">
                                <p>描述：在确定了需要对比的区域后，点击下方的"开始对比"按钮进行对比，对比结果将以弹出页面的形式展示。</p>
                                <button class="btn btn-primary" id="btnCustomStart">开始对比</button>
                            </div>
                            <%--<div class="custom-note" style="margin-top:10px;">
                                <p>描述：在弹出的"对比结果"页面确定差异后，可利用右侧的"标记按钮在地图中将差异位置与范围标示出来"。</p>
                                <button class="btn btn-primary" id="btnCustomMarker">标记</button>
                            </div>
                            <div class="custom-note" style="margin-top:10px;">
                                <p>描述：对比结束后，点击下方的"清除对比结果"按钮，将地图上的对比结果清空。</p>
                                <button class="btn btn-primary" id="btnCustomClear">清除对比结果</button>
                            </div>--%>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="mapgroup">
            <div class="mapcontent">
                <div class="map-tools">
                    <div id="mapTools1" class="map-switcher">
                    
                    </div>
                </div>
                <div id="mapDiv1" class="mapdiv"></div>
            </div>
            <div class="mapcontent">
                <div class="map-tools">
                    <div id="mapTools2" class="map-switcher">
                    
                    </div>
                </div>
                <div id="mapDiv2" class="mapdiv"></div>
            </div>
        </div>
    </div>
    <div class="footer">
        <p>&copy;2014&nbsp;版权所有&nbsp;<a href="#" style="color:#F00;">北京市园林绿化局</a></p>
    </div>
    <script src="js/constrast.js"></script>
</body>
</html>
