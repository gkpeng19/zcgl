using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Serialization;
using Gis.Models;
using NM.Model;
using NM.OP;
using System.Data.SqlClient;
using System.Web.Configuration;
using NM.Util;
using DataService;

namespace GIS.Portal.webservice
{
    /// <summary>
    /// WebServiceAddin 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
     [System.Web.Script.Services.ScriptService]
    public class WebServiceAddin : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

              #region   //为插件工具提供的服务
        [WebMethod(EnableSession = true)]
        public string DoLogin(string UserName, string Password)
        {

            LoginInfo login = AccountUtil.LogIn(UserName, Password, null, "GisAddIn", 0);


            if (login.Status == NM.Model.LoginStatus.Successed)
            {
                System.Web.Security.FormsAuthentication.SetAuthCookie(UserName, false);


                Session["loginuser"] = login;
                Session.Add("USERNAME", UserName);

                return login.ToJson();

            }
            return "";

        }

         [WebMethod(EnableSession = true)]
        public List<Glayer> GetLayerList(string logininfo, string layername = "")
        {
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            if (_login == null)
            {
                _login = LoginInfo.Parse<LoginInfo>(logininfo);
            }
            if (_login != null)
            {
                EntityProviderOP<gis_layerDefine> _op = new EntityProviderOP<gis_layerDefine>(_login, DataProvider.GetEAP_Provider());
                SearchCriteria _search = new SearchCriteria("GIS_LAYERDEFINE");
                if (string.IsNullOrEmpty(layername))
                {
                    //不指定条件，只返回50条记录
                    _search.PageSize =50;
                    _search.PageIndex = 0;
                }
                else
                {
                        _search.PageSize = -1;
                        //LAYERCODE是模糊查询
                        _search["LAYERCODE"] = layername;
                }
               


                SearchResult<gis_layerDefine> _rs = _op.Search(_search);

                List<Glayer> mm = new List<Glayer>();
                foreach (gis_layerDefine d in _rs.Items)
                {
                    mm.Add(new Glayer() { ename = d.LAYERCODE, cname = d.LAYERNAME, datatype = d.DATATYPE.ToString(), });
                }
                return mm;
            
            }
            return null;
        }

         [WebMethod(EnableSession = true)]
         public string inserintolayer(string logininfo, List<Glayer> layers)
         {

             LoginInfo _login = (LoginInfo)(Session["loginuser"]);
             if (_login == null)
             {
                 _login = LoginInfo.Parse<LoginInfo>(logininfo);
             }
             if (_login != null)
             {

                 EntityProviderOP<gis_layerDefine> _op = new EntityProviderOP<gis_layerDefine>(_login, DataProvider.GetEAP_Provider());
                 try
                 {
                     foreach (Glayer g in layers)
                     {
                         gis_layerDefine l = null;
                         SearchCriteria _search = new SearchCriteria("GIS_LAYERDEFINE");
                         //LAYERCODENEW是精确查询
                         _search["LAYERCODENEW"] = g.ename;
                         SearchResult<gis_layerDefine> _rs = _op.Search(_search);
                         //有则修改，没有新增
                         if (_rs.Items.Count > 0)
                         {
                             l = _rs.Items[0];
                             l.EDITBY = _login.User.UserName;

                         }
                         else
                         {
                             l = new gis_layerDefine();
                             l.ADDBY = _login.User.UserName;

                         }
                         l.LAYERCODE = g.ename;
                         l.LAYERNAME = g.cname;
                         int _dt = 0;
                         int.TryParse(g.datatype, out _dt);
                         l.DATATYPE = _dt;
                         int _st = 0;
                         int.TryParse(g.shptype, out _st);
                         l.SHPTYPE = _st;
                         int k = _op.Save(l);
                     }

                     return "1";
                 }
                 catch (Exception ee)
                 {
                     return "0";
                 }

             }
             return "0";
         }

         [WebMethod(EnableSession = true)]
         public string inserintometa(string logininfo, List<Gmeta> gmeta)
         {
             List<Gmeta> gmetanew= GetLayerID(gmeta);

             LoginInfo _login = (LoginInfo)(Session["loginuser"]);
             if (_login == null)
             {
                 _login = LoginInfo.Parse<LoginInfo>(logininfo);
             }
             if (_login != null)
             {

                 EntityProviderOP<gis_layerdefine_sub> _op = new EntityProviderOP<gis_layerdefine_sub>(_login, DataProvider.GetEAP_Provider());
                 try
                 {
                     foreach (Gmeta g in gmetanew)
                     {
                         gis_layerdefine_sub l = new gis_layerdefine_sub();
                         SearchCriteria _search = new SearchCriteria("GIS_LAYERDEFINE_SUB");
                         ////LAYERCODENEW是精确查询
                         _search["LAYERID"] = g.layername;// 说明：此处layername 实际存储了layerid
                         _search["PROPNAME"] = g.propname;
                         SearchResult<gis_layerdefine_sub> _rs = _op.Search(_search);
                         ////有则修改，没有新增
                         if (_rs.Items.Count > 0)
                         {
                             l = _rs.Items[0];
                            // l.EDITBY = _login.User.UserName;

                         }
                         else
                         {
                             l = new gis_layerdefine_sub();
                            // l.ADDBY = _login.User.UserName;

                         }
                         l.PROPNAME = g.propname;
                         l.PROPVALUE = g.propvalue; 
                         l.LAYERID = g.layername;

                         int k = _op.Save(l);
                     }

                     return "1";
                 }
                 catch (Exception ee)
                 {
                     return "0";
                 }

             }
             return "0";
         }

         public List<Gmeta> GetLayerID(List<Gmeta> gmeta) 
         {
             List<servicClassD> list = new List<servicClassD>();
             LoginInfo _login = (LoginInfo)(Session["loginuser"]);
             EntityProviderOP<gis_layerDefine> _op = new EntityProviderOP<gis_layerDefine>(_login, DataProvider.GetEAP_Provider());

             SearchCriteria _searchBase = new SearchCriteria("GIS_LAYERDEFINE");
             _searchBase.PageSize = -1;

             SearchResult<gis_layerDefine> _rs = _op.Search(_searchBase);

             for (int i = 0; i < gmeta.Count; i++)
             { 
                 foreach (gis_layerDefine d in _rs.Items)
                 {
                     if (gmeta[i].layername == d.LAYERCODE)
                     {
                         gmeta[i].layername = d.ID.ToString();
                         break;
                     }
                 }
             }
             return gmeta;
         }


         [WebMethod(EnableSession = true)]
         public string inserintolayerCol(string logininfo,string layername, List<Glayercol> layerCols)
         {
             LoginInfo _login = (LoginInfo)(Session["loginuser"]);
             if (_login == null)
             {
                 _login = LoginInfo.Parse<LoginInfo>(logininfo);
             }
             if (_login != null)
             {               
                 try
                 {
                     EntityProviderOP<gis_layerDefine> _oplayer = new EntityProviderOP<gis_layerDefine>(_login, DataProvider.GetEAP_Provider());

                     gis_layerDefine l = null;
                     SearchCriteria _searchalyer = new SearchCriteria("GIS_LAYERDEFINE");
                     //LAYERCODENEW是精确查询
                     _searchalyer["LAYERCODENEW"] = layername;
                     SearchResult<gis_layerDefine> _rslayer = _oplayer.Search(_searchalyer);
                     if (_rslayer.Items.Count > 0)
                     {
                         l = _rslayer.Items[0];
                     }
                     else
                     {
                         return "01";
                     }


                     EntityProviderOP<gis_layerVersionCol> _op = new EntityProviderOP<gis_layerVersionCol>(_login, DataProvider.GetEAP_Provider());
                     int _ordernum = 0;
                     foreach (Glayercol g in layerCols)
                     {
                         _ordernum = _ordernum + 1;
                         gis_layerVersionCol col = null;

                         SearchCriteria _search = new SearchCriteria("GIS_LAYERVERSIONCOL");
                         _search["LAYERID"] = l.ID;
                         _search["COLCODE"] = g.colename;
                         SearchResult<gis_layerVersionCol> _rs = _op.Search(_search);
                         //有则修改，没有新增
                         if (_rs.Items.Count > 0)
                         {
                             col = _rs.Items[0];
                             col.EDITBY = _login.User.UserName;

                         }
                         else
                         {
                             col = new gis_layerVersionCol();
                             col.ADDBY = _login.User.UserName;
                             col.LayerID = l.ID;

                         }
                         col.COLCODE = g.colename;
                         col.COLNAME = g.colcname;
                         
                         col.COLTYPE = g.coltype.ToString();
                         col.ColLen = g.colLen;
                         col.ColXSLen = g.colXSLen;
                         col.ODERNUM = _ordernum;
                        
                         int k = _op.Save(col);
                     }

                     return "1";
                 }
                 catch (Exception ee)
                 {
                     return "02";
                 }

             }
             return "03";

         }
        #endregion
    }
}
