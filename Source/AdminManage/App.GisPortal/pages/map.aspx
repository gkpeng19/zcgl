<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="map.aspx.cs" Inherits="GIS.Portal.pages.map" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>地图输出</title>
    <style type="text/css">
        * {
            margin:0;padding:0;
        }
        html, body, form, #mapDiv{
            width:100%;
            height:100%;
        }
    </style>
    <link rel="stylesheet" href="../esri/dijit/themes/claro/claro.css" />
    <link rel="stylesheet" href="../esri/esri/css/esri.css" />
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
    <form id="form1" runat="server">
        <div id="mapDiv"></div>
    </form>

    <script type="text/javascript">
        require(['esri/map', 'dojo/topic',
            'widget/layer/PrintMap',
            'esri/geometry/Extent', 'esri/SpatialReference',
            'esri/layers/ArcGISTiledMapServiceLayer',
            'widget/layer/VecLayer', 'widget/layer/ImgLayer',
            'dojo/domReady!'], function (Map, topic, PrintMap, Extent, SpatialReference, ArcGISTiledMapServiceLayer, VecLayer, ImgLayer) {
                //获取所有的底图
            var map = new Map("mapDiv", {
                logo: false,
                slider: false,
                fadeOnZoom: true
            });
            var queryObject = (function () {
                var query = window.location.search;
                if (query.indexOf('?') > -1) {
                    query = query.substr(1);
                }
                var pairs = query.split('&');
                var queryObject = {};
                for (var i = 0; i < pairs.length; i++) {
                    var splits = decodeURIComponent(pairs[i]).split('=');
                    queryObject[splits[0]] = splits[1];
                }
                return queryObject;
            })();
            topic.subscribe("topic/basemap-complete", function (baseLayers) {
                for (var i = 0; i < baseLayers.length; i++) {
                    var layer = baseLayers[i];
                    if (layer.ID == parseInt(queryObject['id'], 10)) {
                        var sr = new SpatialReference(layer.COORSYS);
                        var extent = new Extent(parseFloat(queryObject.xmin), parseFloat(queryObject.ymin), parseFloat(queryObject.xmax), parseFloat(queryObject.ymax), sr);
                        map.setExtent(extent);
                        switch (layer.DATATYPE) {
                            case 0:
                                var vc = new VecLayer(layer.YEAR);
                                map.addLayer(vc);
                                break;
                            case 1:
                                var tileLayer = new ArcGISTiledMapServiceLayer(layer.SERVICEURL);
                                map.addLayer(tileLayer);
                                break;
                            case 2:
                                break;
                            case 3:
                                var rc = new ImgLayer(layer.YEAR);
                                map.addLayer(rc);
                                break;
                        }
                    }
                }
            });
            var printMap = new PrintMap();
            printMap.getBaseLayer();
        });
    </script>
</body>
</html>
