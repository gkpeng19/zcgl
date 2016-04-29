using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Serialization;
using Gis.Models;

/// <summary>
/// Summary description for WebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class WebService : System.Web.Services.WebService
{

    public WebService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    /// <summary>
    /// 取底图的数据
    /// </summary>
    [WebMethod]
    public void GetMapLayer()
    {
        List<GIS_BASEMAP> list = new List<GIS_BASEMAP>();
        GIS_BASEMAP baseMap = new GIS_BASEMAP();

        //矢量底图
        baseMap.SERVICEURL = "http://10.246.63.6:6080/arcgis/rest/services/basemap/vecbj/MapServer";
        baseMap.SERVICEINDEX = 0;
        baseMap.XMIN = "389483.39363082737";
        baseMap.XMAX = "632535.9126890795";
        baseMap.YMIN = "280734.27527673624";
        baseMap.YMAX = "372096.91515229165";
        baseMap.COORSYS = "PROJCS[\"Beijing_Local\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Gauss_Kruger\"],PARAMETER[\"False_Easting\",500000.0],PARAMETER[\"False_Northing\",300000.0],PARAMETER[\"Central_Meridian\",116.35025181],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",39.865766],UNIT[\"Meter\",1.0]]";

        list.Add(baseMap);
        //影像底图
        GIS_BASEMAP baseMap1 = new GIS_BASEMAP();
        baseMap1.SERVICEURL = "http://10.246.63.6:6080/arcgis/rest/services/basemap/img/MapServer";
        baseMap1.SERVICEINDEX = 1;
        baseMap1.XMIN = "389483.39363082737";
        baseMap1.XMAX = "632535.9126890795";
        baseMap1.YMIN = "280734.27527673624";
        baseMap1.YMAX = "372096.91515229165";
        baseMap1.COORSYS = "PROJCS[\"Beijing_Local\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Gauss_Kruger\"],PARAMETER[\"False_Easting\",500000.0],PARAMETER[\"False_Northing\",300000.0],PARAMETER[\"Central_Meridian\",116.35025181],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",39.865766],UNIT[\"Meter\",1.0]]";


        list.Add(baseMap1);

        JavaScriptSerializer serializer = new JavaScriptSerializer();

        Context.Response.Charset = "GB2312";
        Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
        Context.Response.Write(serializer.Serialize(list));
        Context.Response.End();
    }
    /// <summary>
    /// 取图层分类
    /// </summary>
    [WebMethod]
    public void GetMenuMapLayer()
    {

    }

    /// <summary>
    /// 取图层分类明细
    /// </summary>
    [WebMethod]
    public void GetMenuDetailLayer()
    {

    }

    [WebMethod]
    public void GetInfo()
    {

    }
//    public partial class metadata_MetaData : System.Web.UI.Page
//{
//    protected void Page_Load(object sender, EventArgs e)
//    {
//        ////有一个元数据的获取的功能 liang

//        //分类
//        //分类的图层
//        //图层的属性

}
