using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GIS.Portal.webservice
{
    public class Point 
    {
        public double x;
        public double y;

        public Point(double x, double y) 
        {
            this.x = x;
            this.y = y;
        }
    }

    public class ImgModel 
    {
        public string OldUrl;

        public string NewUrl;
    }

    public class Matrix
    {
        public int row;
        public int col;
        public Matrix(int row, int col) 
        {
            this.row = row;
            this.col = col;
        }
    }
    public static class TileModel
    {
        public const int dpi = 96;
        public const int rows = 256;
        public const int cols = 256;
        public const int orginX = 0;
        public const int orginY = 688194;
        public const int compressionQuality = 0;
        public const double minx = 371987.18334;
        public const double miny = 252920.58593;
        public const double maxx = 624459.12036;
        public const double maxy = 423400.07714;
        public const string spatialReference = "PROJCS['Beijing_Local',GEOGCS['GCS_Beijing_1954',DATUM['D_Beijing_1954',SPHEROID['Krasovsky_1940',6378245.0,298.3]],PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]],PROJECTION['Gauss_Kruger'],PARAMETER['False_Easting',500000.0],PARAMETER['False_Northing',300000.0],PARAMETER['Central_Meridian',116.35025181],PARAMETER['Scale_Factor',1.0],PARAMETER['Latitude_Of_Origin',39.865766],UNIT['Meter',1.0]]";
        public const string format = "image/png";
        public static List<LodModel> lods = new List<LodModel>(){
            new LodModel(0, 896.0859375, 3386781.496062992, 1, 1, 1, 2),
            new LodModel(1, 448.04296875, 1693390.748031496, 2, 3, 3, 5),
            new LodModel(2, 224.021484375, 846695.374015748, 4, 7, 7, 10),
            new LodModel(3, 112.0107421875, 423347.687007874, 8, 14, 15, 21),
            new LodModel(4, 56.00537109375, 211673.843503937, 17, 28, 31, 42),
            new LodModel(5, 28.002685546875, 105836.9217519685, 34, 57, 62, 84),
            new LodModel(6, 14.0013427734375, 52918.46087598425, 68, 114, 125, 167),
            new LodModel(7, 7.00067138671875, 26459.23043799213, 137, 232, 247, 335),
            new LodModel(8, 3.50033569335937, 13229.61521899604, 280, 464, 487, 671),
            new LodModel(9, 1.75016784667968, 6614.807609498003, 560, 936, 975, 1343),
            new LodModel(10, 0.875083923339843, 3307.403804749013, 1120, 1872, 1951, 2679),
            new LodModel(11, 0.4375419616699215, 1653.701902374507, 2248, 3744, 3895, 5359),
            new LodModel(12, 0.2187709808349608, 826.8509511872533, 4496, 7496, 7791, 10719)
        };
        //public const string VecUrl = "http://172.24.254.188/service/ImageEngine/picdis/abc?SERVICE=WMTS&REQUEST=GetTile&version=1.0.0&style=default&format=image/png&TileMatrixSet=nativeTileMatrixSet&username=syllhjxxjx&password=syllhjxxjx123&LAYER=Shiliang_";
        //public const string ImgUrl = "http://172.24.254.188/service/ImageEngine/picdis/abc?SERVICE=WMTS&REQUEST=GetTile&version=1.0.0&style=default&format=image/png&TileMatrixSet=nativeTileMatrixSet&username=syllhjxxjx&password=syllhjxxjx123&LAYER=Sate_Rv_";
        
        public const string VecUrl = "http://10.246.0.81:9000/service/ImageEngine/picdis/abc?SERVICE=WMTS&REQUEST=GetTile&version=1.0.0&style=default&format=image/png&TileMatrixSet=nativeTileMatrixSet&username=syllhjxxjx&password=syllhjxxjx123&LAYER=Shiliang_";
        public const string ImgUrl = "http://10.246.0.81:9000/service/ImageEngine/picdis/abc?SERVICE=WMTS&REQUEST=GetTile&version=1.0.0&style=default&format=image/png&TileMatrixSet=nativeTileMatrixSet&username=syllhjxxjx&password=syllhjxxjx123&LAYER=Sate_Rv_";
        public const string CUrl = "http://10.246.0.83:6080/arcgis/rest/services/basemap/img/MapServer/tile/";
    }
    public class LodModel
    {
        private int level;
        private double resolution;
        private double scale;
        private int startTileCol;
        private int startTileRow;
        private int endTileCol;
        private int endTileRow;

        public int Level
        {
            get
            {
                return this.level;
            }
        }
        public double Resolution
        {
            get
            {
                return this.resolution;
            }
        }
        public double Scale
        {
            get
            {
                return this.scale;
            }
        }
        public int StartTileCol
        {
            get
            {
                return this.startTileCol;
            }
        }
        public int StartTileRow
        {
            get
            {
                return this.startTileRow;
            }
        }
        public int EndTileCol
        {
            get
            {
                return this.endTileCol;
            }
        }
        public int EndTileRow
        {
            get 
            {
                return this.endTileRow;
            }
        }
        public LodModel(int level, double resolution, double scale, int startRow, int startCol, int endRow, int endCol)
        {
            this.level = level;
            this.resolution = resolution;
            this.scale = scale;
            this.startTileRow = startRow;
            this.endTileRow = endRow;
            this.startTileCol = startCol;
            this.endTileCol = endCol;
        }
    }
}