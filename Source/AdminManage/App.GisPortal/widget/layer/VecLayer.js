define("widget/layer/VecLayer", ['dojo/_base/declare', 'esri/layers/TiledMapLayer', 'esri/layers/TileInfo', 'esri/geometry/Extent', 'esri/SpatialReference'],
    function (declare, TiledMapServiceLayer, TileInfo, Extent, SpatialReference) {
    "use strict";
    return declare([TiledMapServiceLayer], {
        year: 2013,
        constructor: function (year) {
            this.year = year;
            this.spatialReference = new SpatialReference({
                wkt: 'PROJCS["Beijing_Local",GEOGCS["GCS_Beijing_1954",DATUM["D_Beijing_1954",SPHEROID["Krasovsky_1940",6378245.0,298.3]],PRIMEM["Greenwich",0.0],UNIT["Degree",0.0174532925199433]],PROJECTION["Gauss_Kruger"],PARAMETER["False_Easting",500000.0],PARAMETER["False_Northing",300000.0],PARAMETER["Central_Meridian",116.35025181],PARAMETER["Scale_Factor",1.0],PARAMETER["Latitude_Of_Origin",39.865766],UNIT["Meter",1.0]]'
            });
            this.initialExtent = (this.fullExtent = new Extent(371987.18334, 252920.58593, 624459.12036, 423400.07714, this.spatialReference));
            this.tileInfo = new TileInfo({
                "dpi": 96,
                "format": "image/png",
                "spatialReference": {
                    "wkt": 'PROJCS["Beijing_Local",GEOGCS["GCS_Beijing_1954",DATUM["D_Beijing_1954",SPHEROID["Krasovsky_1940",6378245.0,298.3]],PRIMEM["Greenwich",0.0],UNIT["Degree",0.0174532925199433]],PROJECTION["Gauss_Kruger"],PARAMETER["False_Easting",500000.0],PARAMETER["False_Northing",300000.0],PARAMETER["Central_Meridian",116.35025181],PARAMETER["Scale_Factor",1.0],PARAMETER["Latitude_Of_Origin",39.865766],UNIT["Meter",1.0]]'
                },
                "compressionQuality": 0,
                "rows": 256,
                "cols": 256,
                "origin": {
                    "x": 0,
                    "y": 688194
                },
                "lods": [{
                    "level": 0,
                    "resolution": 896.0859375,
                    "scale": 3386781.496062992
                }, {
                    "level": 1,
                    "resolution": 448.04296875,
                    "scale": 1693390.748031496
                }, {
                    "level": 2,
                    "resolution": 224.021484375,
                    "scale": 846695.374015748
                }, {
                    "level": 3,
                    "resolution": 112.0107421875,
                    "scale": 423347.687007874
                }, {
                    "level": 4,
                    "resolution": 56.00537109375,
                    "scale": 211673.843503937
                }, {
                    "level": 5,
                    "resolution": 28.002685546875,
                    "scale": 105836.9217519685
                }, {
                    "level": 6,
                    "resolution": 14.0013427734375,
                    "scale": 52918.46087598425
                }, {
                    "level": 7,
                    "resolution": 7.00067138671875,
                    "scale": 26459.23043799213
                }, {
                    "level": 8,
                    "resolution": 3.50033569335937,
                    "scale": 13229.61521899604
                }, {
                    "level": 9,
                    "resolution": 1.75016784667968,
                    "scale": 6614.807609498003
                }, {
                    "level": 10,
                    "resolution": 0.875083923339843,
                    "scale": 3307.403804749013
                }, {
                    "level": 11,
                    "resolution": 0.4375419616699215,
                    "scale": 1653.701902374507
                }, {
                    "level": 12,
                    "resolution": 0.2187709808349608,
                    "scale": 826.8509511872533
                }]

            });
            this.loaded = true;
            this.onLoad(this);
        },
        getTileUrl: function (level, row, col) {
            //return "http://10.246.0.81:9000/service/ImageEngine/picdis/abc?request=1&year=2013&type=Shiliang&level=" + level + "&x=" + row + "&y=" + col + "&username=syllhjxxjx&password=syllhjxxjx123";
            return "http://10.246.0.81:9000/service/ImageEngine/picdis/abc?SERVICE=WMTS&REQUEST=GetTile&version=1.0.0&LAYER=Shiliang_" + this.year + "&style=default&format=image/png&TileMatrixSet=nativeTileMatrixSet&TILEMATRIX=" + level + "&TILEROW=" + row + "&TILECOL=" + col + "&username=syllhjxxjx&password=syllhjxxjx123";
            //return "http://172.24.254.188/service/ImageEngine/picdis/abc?SERVICE=WMTS&REQUEST=GetTile&version=1.0.0&LAYER=Shiliang_" + this.year + "&style=default&format=image/png&TileMatrixSet=nativeTileMatrixSet&TILEMATRIX=" + level + "&TILEROW=" + row + "&TILECOL=" + col + "&username=syllhjxxjx&password=syllhjxxjx123";
        }
    });
});