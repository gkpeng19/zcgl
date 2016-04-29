define([
    "dojo/_base/declare",
    "dojo/query",
    "dojo/_base/lang",
    'dojo/_base/window',
    "dojo/_base/array",
    'dojo/on',
    'dojo/dom-class',
    'dojo/dom-construct',
    "dojo/dom-attr",
    "esri/request",
    "esri/graphic",
    "esri/Color",
    "esri/symbols/PictureMarkerSymbol",
    "esri/symbols/SimpleMarkerSymbol",
    "esri/symbols/SimpleFillSymbol",
    "esri/symbols/SimpleLineSymbol",
    "esri/symbols/TextSymbol",
    "esri/layers/GraphicsLayer",
    "esri/geometry/Point",
    "esri/geometry/Polygon",
    "esri/geometry/Extent"
], function (declare, query, lang, win, Array, on, domClass, domConstruct, domAttr, esriRequest, Graphic, Color,
    PictureMarkerSymbol, SimpleMarkerSymbol, SimpleFillSymbol, SimpleLineSymbol, TextSymbol, GraphicsLayer, Point,
    Polygon, Extent) {
    "use strict";

    var graphicLayer = new GraphicsLayer();
    var sms = new SimpleMarkerSymbol().setSize(2);
    var blue = new PictureMarkerSymbol("images/marker/blue.png", 32, 32).setOffset(0, 15);
    var green = new PictureMarkerSymbol("images/marker/green.png", 64, 64).setOffset(0, 15);
    var red = new PictureMarkerSymbol("images/marker/red.png", 72, 72).setOffset(0, 15);
    var extentEvent = null;
    var sfs = new SimpleFillSymbol(SimpleFillSymbol.STYLE_SOLID,
        new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([216, 196, 118]), 1),
        new Color([255, 255, 0, 0.5]));

    var ClusteringPoint = declare([], {
        constructor: function () {
            //on(graphicLayer, "mouse-over", lang.hitch(this, this.mouseOverHandler));
        },
        /*
        mouseOverHandler: function (suc) {
            var that = this;
            var xmin = parseFloat(suc.graphic.attributes.L);
            var xmax = parseFloat(suc.graphic.attributes.R);
            var ymin = parseFloat(suc.graphic.attributes.T);
            var ymax = parseFloat(suc.graphic.attributes.B);

            var extent = new Extent(xmin, ymin, xmax, ymax, JZ.mc.spatialReference);
            var gra = new Graphic(extent, sfs);
            JZ.mc.graphics.clear();
            JZ.mc.graphics.add(gra);
        },
        */
        getRequest: function () {
            var self = this;
            var extent = JZ.mc.extent;
            var polygon = {
                "rings": [[[extent.xmin, extent.ymin], [extent.xmin, extent.ymax], [extent.xmax, extent.ymin], [extent.xmax, extent.ymax]]],
                "spatialReference": { "wkt": JZ.mc.spatialReference }
            };

            var json = "{'rings':[[[" + extent.xmin + ", " + extent.ymin + "],[" + extent.xmin + ", " + extent.ymax + "],[" + extent.xmax + ", " + extent.ymin + "],[" + extent.xmax + ", " + extent.ymax + "]]],'spatialReference':{'wkt':\"PROJCS['Beijing_Local',GEOGCS['GCS_Beijing_1954',DATUM['D_Beijing_1954',SPHEROID['Krasovsky_1940',6378245.0,298.3]],PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]],PROJECTION['Gauss_Kruger'],PARAMETER['False_Easting',500000.0],PARAMETER['False_Northing',300000.0],PARAMETER['Central_Meridian',116.35025181],PARAMETER['Scale_Factor',1.0],PARAMETER['Latitude_Of_Origin',39.865766],UNIT['Meter',1.0]]\"}}";

            var extentPolygon = new Polygon(polygon);
            var req = esriRequest({
                url: JZ.config["ClusterPointSOE"][0].url,
                content: {
                    LayerID: self.LayerID, Envelope: json, Level: JZ.mc.getLevel(), DistrictLayerID: 22, f: 'json'
                },
                handleAs: "json",
                callbackParamName: "callback"
            });
            req.then(lang.hitch(self, self.requestSuccess), lang.hitch(self, self.requestError));
        },

        addGraphicLayer: function (layerID) {
            this.LayerID = layerID;
            this.getRequest();
        },
        requestSuccess: function (suc) {
            var that = this;
            if (graphicLayer) {
                graphicLayer.clear();
            }
            
            Array.forEach(suc.results, function (result) {
                var point = new Point(result.X, result.Y, JZ.mc.spatialReference);
                var graphic = null;
                var graphic1 = null;
                var textSymbol = null;
                /* 返回属性数据解释
                    X，Y分别表示X, Y坐标
                    I 表示聚类点个数
                    N 表示一个聚类点的要素名称
                    C 表示一个聚类点的要素编码
                    L, R, T, B 分别表示XMIN, XMAX, YMIN, YMAX
                    D 表示聚类点的ID集合
                */
                if (result.I === -1){
                    graphic = new Graphic(point, red, result);
                    textSymbol = new TextSymbol("过多").setColor(new Color("#FFF")).setOffset(0, 8);
                } else if (result.I <= 9) {
                    graphic = new Graphic(point, blue);
                    textSymbol = new TextSymbol(result.I).setColor(new Color("#FFF")).setOffset(0, 10);
                } else if (result.I <= 100) {
                    graphic = new Graphic(point, green);
                    textSymbol = new TextSymbol(result.I).setColor(new Color("#FFF")).setOffset(0, 10);
                } else if (result.I <= 999) {
                    graphic = new Graphic(point, red);
                    textSymbol = new TextSymbol(result.I).setColor(new Color("#FFF")).setOffset(0, 8);
                } else {
                    graphic = new Graphic(point, red);
                    textSymbol = new TextSymbol("过多").setColor(new Color("#FFF")).setOffset(0, 8);
                }
                graphic1 = new Graphic(point, textSymbol, result);
                graphicLayer.add(graphic);
                graphicLayer.add(graphic1);
            });

            JZ.mc.addLayer(graphicLayer);
            if (!extentEvent) {
                extentEvent = on(JZ.mc, "extent-change", lang.hitch(that, that.updateCluster));
            }
        },
        updateCluster: function () {
            this.getRequest();
        },
        requestError: function (err) {
            console.log(err);
        },
        removeGraphicLayer: function () {
            if (graphicLayer) {
                JZ.mc.removeLayer(graphicLayer);
            }
        }
    });

    return new ClusteringPoint();
});