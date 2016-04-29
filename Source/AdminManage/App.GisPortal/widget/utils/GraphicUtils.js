/**
* @author wangyafei
* 绿色产业类别
*/
define('widget/utils/GraphicUtils',
    ['dojo/_base/declare',
     'esri/symbols/SimpleFillSymbol',
     'esri/symbols/SimpleLineSymbol',
     'esri/symbols/PictureMarkerSymbol',
     'esri/Color',
     'esri/graphic'
    ], function (declare, SimpleFillSymbol, SimpleLineSymbol, PictureMarkerSymbol, Color, Graphic) {
        "use strict"
        var instance = null, clazz;
        var pms = new PictureMarkerSymbol("images/marker/locate_1.png", 17, 24);
        var sls = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 0, 0, 0.5]), 2);
        var sfs = new SimpleFillSymbol(SimpleFillSymbol.STYLE_SOLID, sls, new Color([255, 0, 0, 0.3]));

        clazz = declare(null, {
            constructor: function () {
            },
            addGraphic: function (graphicLayer, geometry, attr) {
                JZ.mc.graphics.clear();
                //五种要素 point | multipoint | polyline | polygon | extent
                switch (geometry.type) {
                    case "point":
                        var gra = new Graphic(geometry, pms, attr);
                        graphicLayer.add(gra);
                        break;
                    case "polyline":
                        var gra = new Graphic(geometry, sls, attr);
                        graphicLayer.add(gra);
                        break;
                    case "polygon":
                    case "extent":
                        var gra = new Graphic(geometry, sfs, attr);
                        graphicLayer.add(gra);
                        break;
                }
            }
        });
        clazz.getInstance = function () {
            if (instance === null) {
                instance = new clazz();
            }
            return instance;
        };
        return clazz;
    });