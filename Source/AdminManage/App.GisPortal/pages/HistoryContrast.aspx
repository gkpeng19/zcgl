<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HistoryContrast.aspx.cs" Inherits="GIS.Portal.history.HistroyContrast" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>历史对比分析</title>
    <link rel="stylesheet" href="../esri/dijit/themes/claro/claro.css" />
    <link rel="stylesheet" href="../esri/esri/css/esri.css" />
    <link rel="stylesheet" href="../esri/dojox/widget/Dialog/Dialog.css" />
    <link rel="stylesheet" href="../css/bootstrap.css" />
    <link rel="stylesheet" href="../css/history.css" />
    <!--[if IE 8]>
    <script type="text/javascript" src="js/ie8.js"></script>
    <![endif]-->
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
    <script type="text/javascript" src="js/history.js"></script>
</head>
<body class="claro">
    <div class="hc-wrapper">
        <div class="hc-toolbar">
            <label for="sel-view">查看方式:</label>
            <select id="sel-view" class="form-control">
                <option value="1" selected="selected">两图对比</option>
                <option value="2">多图对比</option>
            </select>
            <label for="latCoor">经度：</label>
            <input id="latCoor" type="text" value="116.3" class="form-control small" />
            <label for="lonCoor">纬度：</label>
            <input id="lonCoor" type="text" value="39.9" class="form-control small" />
            <button id="btnStartLocate" class="btn btn-primary">定位</button>
        </div>
        <div id="mapBox" class="mapbox show">
            <div id="map1C" class="hc-map-size">
                <div id="mapDiv1">
                    <div class="hc-switch-case">
                        <div id="basemapSwitcher1" class="map-switcher">
                        </div>
                        <div class="map-fullscreen">
                            <button id="oneMapScreen" class="btn btn-primary">全屏</button>
                        </div>
                    </div>
                </div>
            </div>
            <div id="map2C" class="hc-map-size" style="float: right;">
                <div id="mapDiv2">
                    <div class="hc-switch-case">
                        <div id="basemapSwitcher2" class="map-switcher">
                        </div>
                        <div class="map-fullscreen">
                            <button id="twoMapScreen" class="btn btn-primary">全屏</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="mapBox1" class="mapbox">
            <div id="mapBox2" style="height:49%">
                <div id="map3C" class="hc-map-size">
                    <div id="mapDiv3">
                        <div class="hc-switch-case">
                            <div id="basemapSwitcher3" class="map-switcher">
                            </div>
                            <div class="map-fullscreen">
                                <button id="threeMapScreen" class="btn btn-primary">全屏</button>
                            </div>
                        </div>
                    </div>
                </div>
                <div id="map4C" class="hc-map-size" style="float:right;">
                    <div id="mapDiv4">
                        <div class="hc-switch-case">
                            <div id="basemapSwitcher4" class="map-switcher">
                            </div>
                            <div class="map-fullscreen">
                                <button id="fourMapScreen" class="btn btn-primary">全屏</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="mapBox3" style="height:49%">
                <div id="map5C" class="hc-map-size">
                    <div id="mapDiv5">
                        <div class="hc-switch-case">
                            <div id="basemapSwitcher5" class="map-switcher">
                            </div>
                            <div class="map-fullscreen">
                                <button id="fiveMapScreen" class="btn btn-primary">全屏</button>
                            </div>
                        </div>
                    </div>
                </div>
                <div id="map6C" class="hc-map-size" style="float:right;">
                    <div id="mapDiv6">
                        <div class="hc-switch-case">
                            <div id="basemapSwitcher6" class="map-switcher">
                            </div>
                            <div class="map-fullscreen">
                                <button id="sixMapScreen" class="btn btn-primary">全屏</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</body>
</html>
