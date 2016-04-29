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
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using System.Drawing;
using System.Net;
using System.Drawing.Imaging;

namespace GIS.Portal.webservice
{
    /// <summary>
    /// WebServiceMap 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    [System.Web.Script.Services.ScriptService]
    public class WebServiceMap : System.Web.Services.WebService
    {
        public const string IP = "http://10.246.0.83:6080";
        [WebMethod]
        public void GetRequestXML(string value) 
        {
            //string mm = System.Web.HttpContext.Current.Server.UrlDecode(value);
            //string decodeStr = Encoding.GetEncoding("gb2312").GetString(Encoding.Default.GetBytes(value));
            string url = "http://172.24.254.188/service/AddrCode/cmd?commandType=0&username=syllhjxxjx&password=syllhjxxjx123&AddrName=" +
                value + "&IsAccurate=false&AddrType=AllType&Start=0&Nums=10";
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
            req.Headers.Add("Accept-Language", "zh_CN");
            req.UserAgent = "Mozilla/5.0(compatible;MSIE 10.0;Windows NT 6.2;WOW64;Trident/6.0)";
            //req.ServicePoint.Expect100Continue = false;
            req.Method = "GET";
            using (System.Net.WebResponse wr = req.GetResponse())
            {
                using (StreamReader sr = new StreamReader(wr.GetResponseStream(), Encoding.GetEncoding("gbk")))
                {
                    string str = sr.ReadToEnd();
                    //Context.Response.Write(Encoding.GetEncoding(str));
                    //Context.Response.Write(Encoding.GetEncoding(reStr));
                    Context.Response.Charset = "gbk";
                    Context.Response.ContentEncoding = Encoding.GetEncoding("gbk");
                    Context.Response.Write(str);
                    sr.Close();
                }
            }
        }
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod(EnableSession = true)]
        public void GetLoginUser()
        {
            if (Session["loginuser"] != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                Context.Response.Charset = "utf-8";
                Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
                Context.Response.Write(serializer.Serialize(Session["loginuser"]));
                Context.Response.End();
            }
        }
        [WebMethod(EnableSession = true)]
        public void Logout()
        {
            Session["loginuser"] = null;
            Session.RemoveAll();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            Context.Response.Write(serializer.Serialize(true));
            Context.Response.End();
        }
        [WebMethod(EnableSession = true)]
        public void GetHotWords()
        {
            List<gis_hotword> hotwords = new List<gis_hotword>();
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            EntityProviderOP<gis_hotword> _op = new EntityProviderOP<gis_hotword>(_login, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("GIS_HOTWORD");
            _search["QSUCCESS"] = 1;
            _search.PageSize = -1;
            SearchResult<gis_hotword> _rs = _op.Search(_search);
            string callback = Context.Request["callback"];
            hotwords = _rs.Items.ToList().GetRange(0, 3);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            if (callback != null)
            {
                Context.Response.Write(callback + "(" + serializer.Serialize(hotwords) + ")");
            }
            else
            {
                Context.Response.Write(serializer.Serialize(hotwords));
            }
            Context.Response.End();
        }
        [WebMethod(EnableSession = true)]
        public void SetHotWords(string qword, int qsuccess)
        {
            List<gis_hotword> hotwords = new List<gis_hotword>();
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            EntityProviderOP<gis_hotword> _op = new EntityProviderOP<gis_hotword>(_login, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("GIS_HOTWORD");
            _search["QWORD"] = qword;
            _search["QSUCCESS"] = qsuccess;
            _search.PageSize = 1;

            SearchResult<gis_hotword> _rs = _op.Search(_search);

            gis_hotword obj = null;
            if ((_rs != null) && (_rs.Items != null) && (_rs.Items.Count > 0))
            {
                obj = _rs.Items[0];
                obj.QCOUNT = obj.QCOUNT + 1;
            }
            else
            {
                obj = new gis_hotword() { QWORD = qword, QCOUNT = 1, QSUCCESS = qsuccess };
            }

            _op.Save(obj);
        }
        [WebMethod(EnableSession = true)]
        public void DoLogin(string UserName, string Password)
        {
            LoginInfo login = AccountUtil.LogIn(UserName, Password, null, "GisAddIn", 0);

            if (login.Status == NM.Model.LoginStatus.Successed)
            {
                System.Web.Security.FormsAuthentication.SetAuthCookie(UserName, false);
                Session["loginuser"] = login;
                Session.Add("USERNAME", UserName);
            }

            string callback = Context.Request["callback"];
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            if (callback != null)
            {
                Context.Response.Write(callback + "(" + serializer.Serialize(login.ToJson()) + ")");
            }
            else
            {
                Context.Response.Write(serializer.Serialize(login.ToJson()));
            }
            Context.Response.End();
        }
        [WebMethod(EnableSession = true)]
        public void ChnagePasswd(string oldPwd, string newPwd)
        {
            List<gis_layerclassMoudle> list = new List<gis_layerclassMoudle>();
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            CommandResult cs = new CommandResult();
            
            if ((_login != null) || (_login.User != null))
            {
                AccountOP op = new AccountOP(null);
                cs = op.ChangePassword(_login.User.UserID, oldPwd, newPwd);
            }
            string callback = Context.Request["callback"];
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            if (callback != null)
            {
                Context.Response.Write(callback + "(" + serializer.Serialize(cs.ToJson()) + ")");
            }
            else
            {
                Context.Response.Write(serializer.Serialize(cs.ToJson()));
            }
            Context.Response.End();
        }
        /// <summary>
        /// 取底图的数据
        /// </summary>
        [WebMethod(EnableSession = true)]
        public void GetBaseMapLayer()
        {
            List<GIS_BASEMAP> list = new List<GIS_BASEMAP>();
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            EntityProviderOP<GIS_BASEMAP> _op = new EntityProviderOP<GIS_BASEMAP>(_login, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("GIS_BASEMAP");
            _search.PageSize = -1;
            SearchResult<GIS_BASEMAP> _rs = _op.Search(_search);
            string callback = Context.Request["callback"];
            list = _rs.Items.ToList();
            foreach (GIS_BASEMAP item in list)
            {
                item.SERVICEURL = Regex.Replace(item.SERVICEURL, @"^http://\d+\.\d+\.\d+\.\d+\:\d+", IP);
            }
            list.Sort();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // string _re = serializer.Serialize(_rs.Items.ToList());
            //_re = _rs.Items.ToJson();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            if (callback != null)
            {
                Context.Response.Write(callback + "(" + serializer.Serialize(list) + ")");
            }
            else
            {
                Context.Response.Write(serializer.Serialize(list));
            }

            Context.Response.End();
        }
        public List<servicClassD> pGetMenuLayer(string classid, int layertype)
        {
            List<servicClassD> list = new List<servicClassD>();
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            EntityProviderOP<gis_LayerClassD> _op = new EntityProviderOP<gis_LayerClassD>(_login, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("GIS_LAYERCLASSD");
            _search.PageSize = -1;
            _search["CLASSID"] = classid;

            //-- 0：基础图层1：业务图层2;  专题图层
            if (layertype >= 0)
            {
                _search["LayerTypeint_G"] = layertype;
            }

            SearchResult<gis_LayerClassD> _rs = _op.Search(_search);
            foreach (gis_LayerClassD d in _rs.Items)
            {
                servicClassD m = new servicClassD()
                {
                    id = d.LAYERID,
                    enName = d.LayerCode_G,
                    cnName = d.LayerName_G,
                    shptype = d.SHPTYPEint_G,
                    featureServerUrl = "",
                    gisdatafields = d.GisDataFields_G,
                    mapServerUrl = Regex.Replace(d.serviceurl_G, @"^http://\d+\.\d+\.\d+\.\d+\:\d+", IP),
                    isTimeData = d.ishistory_G,
                    serverindex = d.serviceindex_G,
                    vlevel = d.VLEVEL_G
                };
                m.datayears = "";

                if (!string.IsNullOrEmpty(d.LayerYears_G))
                {
                    m.datayears = d.LayerYears_G;
                    // m.datayears.AddRange(d.LayerYears_G.Split(','));
                }
                list.Add(m);
            }
            return list;
        }
        /// <summary>
        /// 取图层分类
        /// </summary>
        [WebMethod(EnableSession = true)]
        public void GetMenuMapLayer()
        {
            List<gis_layerclassMoudle> list = new List<gis_layerclassMoudle>();

            LoginInfo _login = (LoginInfo)(Session["loginuser"]);

            if ((_login != null) || (_login.User != null))
            {
                int k = 1;
                int uid = _login.User.ID;
                DataProvider dp = DataProvider.GetEAP_Provider();
                List<SqlParameter> _params = new List<SqlParameter>();
                //获取当前登录人有权限的图层分类
                #region
                string ssql = string.Format("select a.*  from   gis_layerclass  a        join  eap_userallowlayerclass    b on a.id=b.classid " +
                            "  where b.roleid in (select c.roleid from eap_userrole  c where c.userid={0})   order by a.odernum", uid);
                // SearchCriteria _search = new SearchCriteria("EAP_RESOURCEOP");
                // _search["parentid"] = moduleId;
                List<gis_layerclass> classlist = dp.LoadData<gis_layerclass>(ssql, _params.ToArray());

                foreach (gis_layerclass o in classlist)
                {
                    gis_layerclassMoudle moudle = new gis_layerclassMoudle();
                    moudle.ID = o.ID;
                    moudle.NAME = o.CLASSNAME; 
                    moudle.ENAME = o.CLASSCODE; 
                    moudle.TYPE = 1; 
                    moudle.IMG = o.CLASSIMG; 
                    moudle.HREF = "";
                    moudle.baseID = 0;
                    moudle.LAYERS = pGetMenuLayer(o.ID.ToString(), 1);
                    list.Add(moudle);
                    //list.Add(new gis_layerclassMoudle() { ID = o.ID, NAME = o.CLASSNAME, ENAME = o.CLASSCODE, TYPE = 1, IMG = o.CLASSIMG, HREF = "", baseID = 0 });
                    if (o.ID > k)
                    {
                        k = o.ID;
                    }

                }
                #endregion


                //获取当前登录人有权限的菜单；
                //#region
                //k = k + 2;
                //string _portid = "124";
                //try
                //{
                //    _portid = WebConfigurationManager.AppSettings["GisPortID"];
                //    if (string.IsNullOrEmpty(_portid))
                //    {
                //        _portid = "124";
                //    }
                //}
                //catch
                //{ }
                //string ssql1 = string.Format("  select r.*   from eap_resource  r  join   eap_rolemodule  b on r.id=b.sourceid " +
                // "  where  r.type='menu' and r.parentid={0}  and  b.roleid in  (select c.roleid from eap_userrole  c where c.userid={1})    order by r.sortby  ", _portid, uid);
                //// SearchCriteria _search = new SearchCriteria("EAP_RESOURCEOP");
                //// _search["parentid"] = moduleId;
                //List<EAP_Resource> layerlist = dp.LoadData<EAP_Resource>(ssql1, _params.ToArray());
                //foreach (EAP_Resource o in layerlist)
                //{
                //    int tempid = k + o.ID;//一个临时id，保证和分类的id不重复；
                //    if (o.Name == "对比分析")
                //    {
                //        list.Add(new gis_layerclassMoudle()
                //        {
                //            ID = o.ID,
                //            NAME = o.Name,
                //            ENAME = o.Description,
                //            TYPE = 0,
                //            IMG = o.Image,
                //            HREF = "",
                //            baseID = k
                //        });
                //    }
                //    else
                //    {
                //        list.Add(new gis_layerclassMoudle() { ID = o.ID, NAME = o.Name, ENAME = o.Description, TYPE = o.VRow, IMG = o.Image, HREF = o.PageId, baseID = k });
                //    }
                //}
                //#endregion

                //强制加上修改密码
                //list.Add(new gis_layerclassMoudle() { ID = k - 1, NAME = "修改密码", ENAME = "ChangePassword", TYPE = 0, IMG = "images/func/2.png", HREF = "", baseID = k - 1 });
            }


            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // string _re = serializer.Serialize(_rs.Items.ToList());
            //_re = _rs.Items.ToJson();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(list);
            Context.Response.Write(mm);
            Context.Response.End();

        }
        /// <summary>
        /// 取图层分类明细，取业务图层明细
        /// </summary>
        [WebMethod(EnableSession = true)]
        public void GetMenuDetailLayer(string classid)
        {
            pGetMenuDetailLayer(classid, 1);
        }
        /// <summary>
        /// 取图层分类明细,取当前分类的底图明细
        /// </summary>
        [WebMethod(EnableSession = true)]
        public void GetMenuBaseDetailLayer(string classid)
        {
            List<classbase> list = new List<classbase>();
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            EntityProviderOP<gis_LayerClassD> _op = new EntityProviderOP<gis_LayerClassD>(_login, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("GIS_LAYERCLASSD");
            _search.PageSize = -1;
            _search["CLASSID"] = classid;
            //-- 0：基础图层1：业务图层2;  专题图层
            _search["LayerTypeint_G"] = 0;

            SearchResult<gis_LayerClassD> _rs = _op.Search(_search);


            int k = 1;
            foreach (gis_LayerClassD d in _rs.Items)
            {
                k = k + 1;
                ReflayerID r = new ReflayerID() { _reference = k.ToString() };
                //string _c = string.Format("{_reference: {0}}",k);
                classbase _cm = list.Find((x) =>
                {
                    return x.mapServerUrl == Regex.Replace(d.serviceurl_G, @"^http://\d+\.\d+\.\d+\.\d+\:\d+", IP);
                });
                if (_cm == null)
                {
                    _cm = new classbase() { classid = k, mapServerUrl = Regex.Replace(d.serviceurl_G, @"^http://\d+\.\d+\.\d+\.\d+\:\d+", IP) };
                    _cm.visibleIndexs.Add(d.serviceindex_G);
                    list.Add(_cm);
                }
                else
                {
                    _cm.visibleIndexs.Add(d.serviceindex_G);
                }
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // string _re = serializer.Serialize(_rs.Items.ToList());
            //_re = _rs.Items.ToJson();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(list);
            Context.Response.Write(mm);
            Context.Response.End();
        }
        [WebMethod(EnableSession = true)]
        public void GetFeatureEditable(string layerid)
        {
            List<gis_layerVersionCol> list = new List<gis_layerVersionCol>();
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            if ((_login != null) || (_login.User != null))
            {
                DataProvider dp = DataProvider.GetEAP_Provider();
                List<SqlParameter> _params = new List<SqlParameter>();

                string ssql = string.Format("select a.*  from  gis_layerVersionCol a where a.layerid={0}  and  a.isvisible=1  order by odernum "
                        , layerid);
                // SearchCriteria _search = new SearchCriteria("EAP_RESOURCEOP");
                // _search["parentid"] = moduleId;
                list = dp.LoadData<gis_layerVersionCol>(ssql, _params.ToArray());
            }

            string callback = Context.Request["callback"];

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // string _re = serializer.Serialize(_rs.Items.ToList());
            //_re = _rs.Items.ToJson();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(list);

            if (callback != null)
            {
                Context.Response.Write(callback + "(" + mm + ")");
            }
            else
            {
                Context.Response.Write(mm);
            }

            Context.Response.End();
        }
        /// <summary>
        /// 取图层分类明细, 的专题图列表
        /// </summary>
        [WebMethod(EnableSession = true)]
        public void GetMenuSpecDetailLayer(string classid)
        {
            // pGetMenuDetailLayer(classid, 2);
            /*
           {
               id: "1", name: "注册公园", type: "root", children: [
                     { _reference: "2" },
                     { _reference: "3" },
                     { _reference: "4" }
               ]
           },
           { id: "2", name: "建设年代", type: "instance" },
           { id: "3", name: "所属区县", type: "instance" },
           { id: "4", name: "公园面积", type: "instance" }，
           { id: "7", name: "建设年代", type: "instance" },
           { id: "8", name: "所属区县", type: "instance" },
           { id: "9", name: "公园面积", type: "instance" }]     
             */

            List<gis_layerclass> classlist = new List<gis_layerclass>();
            List<servicClassD2> list = new List<servicClassD2>();
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            EntityProviderOP<gis_LayerClassD> _op = new EntityProviderOP<gis_LayerClassD>(_login, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("GIS_LAYERCLASSD");
            _search.PageSize = -1;
            if (!string.IsNullOrEmpty(classid))
            {
                _search["CLASSID"] = classid;
            }
            else
            {
                #region //获取当前登录人有权限的图层分类
                int uid = _login.User.ID;
                DataProvider dp = DataProvider.GetEAP_Provider();
                List<SqlParameter> _params = new List<SqlParameter>();


                string ssql = string.Format("select a.*    from   gis_layerclass  a        join  eap_userallowlayerclass    b on a.id=b.classid " +
                            "  where b.roleid in (select c.roleid from eap_userrole  c where c.userid={0})   order by a.odernum", uid);
                // SearchCriteria _search = new SearchCriteria("EAP_RESOURCEOP");
                // _search["parentid"] = moduleId;
                classlist = dp.LoadData<gis_layerclass>(ssql, _params.ToArray());
                #endregion
            }
            int k = 1;
            if (classlist.Count == 0)
            {
                servicClassDM master = new servicClassDM() { id = 1, cnName = "专题分析", type = "root", actclassid = classid };
                master.children = new List<ReflayerID>();
                list.Add(master);
            }
            else
            {
                foreach (gis_layerclass c in classlist)
                {
                    servicClassDM master = new servicClassDM() { id = k, cnName = c.CLASSNAME, type = "root", actclassid = c.ID.ToString() };
                    master.children = new List<ReflayerID>();
                    list.Add(master);
                    k = k + 1;
                }
            }
            //-- 0：基础图层1：业务图层2;  专题图层
            _search["LayerTypeint_G"] = 2;

            SearchResult<gis_LayerClassD> _rs = _op.Search(_search);


            foreach (gis_LayerClassD d in _rs.Items)
            {
                k = k + 1;
                ReflayerID r = new ReflayerID() { _reference = k.ToString() };
                //string _c = string.Format("{_reference: {0}}",k);
                servicClassD2 _b = list.Find((x) => { return x.actclassid == d.CLASSID.ToString(); });
                servicClassDM master = _b as servicClassDM;

                master.children.Add(r);
                list.Add(new servicClassD2()
                {
                    id = k,
                    enName = d.LayerCode_G,
                    cnName = d.LayerName_G,
                    featureServerUrl = Regex.Replace(d.featureserviceurl_G, @"^http://\d+\.\d+\.\d+\.\d+\:\d+", IP),
                    featureServerIndex = d.featureserviceindex_G,
                    mapServerUrl = Regex.Replace(d.serviceurl_G, @"^http://\d+\.\d+\.\d+\.\d+\:\d+", IP),
                    serverindex = d.serviceindex_G,
                    type = "instance",
                    layerid = d.LAYERID.ToString(),
                    layertype = d.LayerTypeint_G.ToString(),
                    shptype = d.SHPTYPEint_G,


                    specshowmode = d.SpecShowMode_G.ToString()

                });
            }


            string callback = Context.Request["callback"];

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // string _re = serializer.Serialize(_rs.Items.ToList());
            //_re = _rs.Items.ToJson();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(list);

            if (callback != null)
            {
                Context.Response.Write(callback + "(" + mm + ")");
            }
            else
            {
                Context.Response.Write(mm);
            }

            Context.Response.End();
        }
        /// <summary>
        /// <summary>
        /// <param name="classid"></param>
        /// <param name="layertype">  0：基础图层1：业务图层2;  专题图层</param>
        [WebMethod(EnableSession = true)]
        public void pGetMenuDetailLayer(string classid, int layertype)
        {
            List<servicClassD> list = new List<servicClassD>();
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            EntityProviderOP<gis_LayerClassD> _op = new EntityProviderOP<gis_LayerClassD>(_login, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("GIS_LAYERCLASSD");
            _search.PageSize = -1;
            _search["CLASSID"] = classid;

            //-- 0：基础图层1：业务图层2;  专题图层
            if (layertype >= 0)
            {
                _search["LayerTypeint_G"] = layertype;
            }

            SearchResult<gis_LayerClassD> _rs = _op.Search(_search);
            foreach (gis_LayerClassD d in _rs.Items)
            {
                servicClassD m = new servicClassD()
                {
                    id = d.LAYERID,
                    enName = d.LayerCode_G,
                    cnName = d.LayerName_G,
                    featureServerUrl = "",
                    gisdatafields = d.GisDataFields_G,
                    mapServerUrl = Regex.Replace(d.serviceurl_G, @"^http://\d+\.\d+\.\d+\.\d+\:\d+", IP),
                    isTimeData = d.ishistory_G,
                    serverindex = d.serviceindex_G,
                    vlevel = d.VLEVEL_G,
                    shptype = d.SHPTYPEint_G
                };
                m.datayears = "";

                if (!string.IsNullOrEmpty(d.LayerYears_G))
                {
                    m.datayears = d.LayerYears_G;
                    // m.datayears.AddRange(d.LayerYears_G.Split(','));
                }
                list.Add(m);
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // string _re = serializer.Serialize(_rs.Items.ToList());
            //_re = _rs.Items.ToJson();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(list);
            Context.Response.Write(mm);
            Context.Response.End();
        }
        /// <summary>
        /// 获得图层信息，把基础图层和业务图层都查询出来
        /// <summary> 
        /// <param name="layertype">  0：基础图层1：业务图层2;  专题图层</param>
        [WebMethod(EnableSession = true)]
        public void pGetMenuDetailLayer_Metadata()
        {
            List<servicClassD> list = new List<servicClassD>();
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            EntityProviderOP<gis_layerDefine> _op = new EntityProviderOP<gis_layerDefine>(_login, DataProvider.GetEAP_Provider());

            SearchCriteria _searchBase = new SearchCriteria("GIS_LAYERDEFINE");
            _searchBase.PageSize = -1;

            SearchResult<gis_layerDefine> _rs = _op.Search(_searchBase);
            servicClassDM masterBase = new servicClassDM() { id = 0, cnName = "基础图层", type = "root" };
            masterBase.children = new List<ReflayerID>();
            list.Add(masterBase);

            servicClassDM masterBussiness = new servicClassDM() { id = 1000, cnName = "业务图层", type = "root" };
            masterBussiness.children = new List<ReflayerID>();
            list.Add(masterBussiness);

            foreach (gis_layerDefine d in _rs.Items)
            {
                if (string.IsNullOrEmpty(d.SERVICEURL))
                {
                    continue;
                }
                if (d.LAYERTYPE == 0)
                {
                    ReflayerID r = new ReflayerID() { _reference = d.ID.ToString() };
                    masterBase.children.Add(r);
                    servicClassD2 m = new servicClassD2()
                    {
                        id = d.ID,
                        enName = d.LAYERCODE,
                        cnName = d.LAYERNAME,
                        featureServerUrl = "",
                        //@"^http://\d+\.\d+\.\d+\.\d+\:\d+"
                        mapServerUrl = Regex.Replace(d.SERVICEURL, @"^http://\d+\.\d+\.\d+\.\d+\:\d+", IP),
                        isTimeData = d.ISHISTORY,
                        serverindex = d.SERVICEINDEX,
                        type = "instance"
                    };
                    m.datayears = "";

                    if (!string.IsNullOrEmpty(d.DataYears))
                    {
                        m.datayears = d.DataYears;
                    }
                    list.Add(m);
                }
                if (d.LAYERTYPE == 1)
                {
                    ReflayerID r = new ReflayerID() { _reference = d.ID.ToString() };
                    masterBussiness.children.Add(r);

                    servicClassD2 m = new servicClassD2()
                    {
                        id = d.ID,
                        enName = d.LAYERCODE,
                        cnName = d.LAYERNAME,
                        featureServerUrl = "",
                        mapServerUrl = Regex.Replace(d.SERVICEURL, @"^http://\d+\.\d+\.\d+\.\d+\:\d+", IP),
                        isTimeData = d.ISHISTORY,
                        serverindex = d.SERVICEINDEX,
                        type = "instance"
                    };

                    m.datayears = "";

                    if (!string.IsNullOrEmpty(d.DataYears))
                    {
                        m.datayears = d.DataYears;
                    }
                    list.Add(m);
                }
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(list);
            Context.Response.Write(mm);
            Context.Response.End();
        }
        [WebMethod(EnableSession = true)]
        /// <summary>
        /// //图层的元数据描述信息
        /// </summary> 
        public void GetDescribe_Metadata(string layerid)
        {
            List<gis_layerdefine_sub> list = new List<gis_layerdefine_sub>();
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            if ((_login != null) || (_login.User != null))
            {
                DataProvider dp = DataProvider.GetEAP_Provider();
                List<SqlParameter> _params = new List<SqlParameter>();

                string ssql = string.Format("select a.*  from  gis_layerdefine_sub a where a.layerid={0} order by id "
                        , "'" + layerid.TrimEnd() + "'");

                list = dp.LoadData<gis_layerdefine_sub>(ssql, _params.ToArray());
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(list);
            Context.Response.Write(mm);
            Context.Response.End();
        }
        //获得资源类型
        public string GetResourceType(string layerCode)
        {
            if (layerCode.StartsWith("LDSC_")) return "生产库图层";
            else if (layerCode.StartsWith("SJ2005_")) return "2005年数据";
            else return "";
        }
        //获得资源类型编码
        public string GetResourceCode(string layerCode)
        {
            if (layerCode.StartsWith("LDSC_")) return "LDSC_";
            else if (layerCode.StartsWith("SJ2005_")) return "SJ2005_";
            else return "";
        }
        //获得数据类型
        public string GetShpType(int shpType)
        {
            switch (shpType)
            {
                case 0:
                    return "矢量";
                case 1:
                    return "栅格";
            }
            return "";
        }
        [WebMethod(EnableSession = true)]
        public void GetArealist()
        {
            List<EAP_Area> list = new List<EAP_Area>();
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            if ((_login != null) || (_login.User != null))
            {
                DataProvider dp = DataProvider.GetEAP_Provider();
                List<SqlParameter> _params = new List<SqlParameter>();

                string ssql = string.Format("select a.*  from  eap_area  a   join  eap_userallowarea  b on a.id=b.areaid  and b.roleid in  (select roleid  from eap_userrole where userid={0}) order by odernum "
                        , _login.User.ID);
                // SearchCriteria _search = new SearchCriteria("EAP_RESOURCEOP");
                // _search["parentid"] = moduleId;
                list = dp.LoadData<EAP_Area>(ssql, _params.ToArray());
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // string _re = serializer.Serialize(_rs.Items.ToList());
            //_re = _rs.Items.ToJson();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(list);
            Context.Response.Write(mm);
            Context.Response.End();
        }
        [WebMethod]
        public void GetInfo()
        {

        }
        /// <summary>
        /// //图层的属性字段列表
        /// </summary>
        /// <param name="layerid"></param>
        [WebMethod(EnableSession = true)]
        public void GetColsBylayer(string layerid)
        {
            List<gis_layerVersionCol> list = new List<gis_layerVersionCol>();
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            if ((_login != null) || (_login.User != null))
            {
                DataProvider dp = DataProvider.GetEAP_Provider();
                List<SqlParameter> _params = new List<SqlParameter>();
                string ssql = string.Format("select a.*  from  gis_layerVersionCol a where a.layerid={0}  and  a.isvisible=1  order by odernum "
                        , layerid);
                // SearchCriteria _search = new SearchCriteria("EAP_RESOURCEOP");
                // _search["parentid"] = moduleId;
                list = dp.LoadData<gis_layerVersionCol>(ssql, _params.ToArray());
            }

            string callback = Context.Request["callback"];

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // string _re = serializer.Serialize(_rs.Items.ToList());
            //_re = _rs.Items.ToJson();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(list);

            if (callback != null)
            {
                Context.Response.Write(callback + "(" + mm + ")");
            }
            else
            {
                Context.Response.Write(mm);
            }

            Context.Response.End();
        }
        /// <summary>
        /// //图层的属性字段列表
        /// </summary>
        /// <param name="layerid"></param>
        [WebMethod(EnableSession = true)]
        public void GetColsBylayer_Metadata(string layerid)
        {
            List<gis_layerVersionCol> list = new List<gis_layerVersionCol>();
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            if ((_login != null) || (_login.User != null))
            {
                DataProvider dp = DataProvider.GetEAP_Provider();
                List<SqlParameter> _params = new List<SqlParameter>();

                string ssql = string.Format("select a.*  from  gis_layerVersionCol a where a.layerid={0} order by odernum "
                        , layerid);
                list = dp.LoadData<gis_layerVersionCol>(ssql, _params.ToArray());
            }

            for (int i = 0; i < list.Count; i++)
            {
                switch (list[i].COLTYPE)
                {
                    case "1":
                        list[i].COLTYPE = "整型";
                        break;
                    case "2":
                        list[i].COLTYPE = "浮点型";
                        break;
                    case "3":
                        list[i].COLTYPE = "字符型";
                        break;
                    case "4":
                        list[i].COLTYPE = "日期型";
                        break;
                }
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(list);
            Context.Response.Write(mm);
            Context.Response.End();
        }
        [WebMethod(EnableSession = true)]
        public void GetStatData(string layercode, string statyear, string k, string v)
        {
            string _toolitp = "面积：";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // List<statparam> ls= serializer.Deserialize<List<statparam>>(statparam);
            string[] karray = k.Split(',');
            string[] varray = v.Split(',');
            List<statuse> list = new List<statuse>();
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            if ((_login != null) || (_login.User != null))
            {
                DataProvider dp = DataProvider.GetProvider("ConnectionString_SDE");
                List<SqlParameter> _params = new List<SqlParameter>();

                string ssql = "";
                string group = "  ";
                string select = "";
                string selecthead = "";
                string where = "";

                //'1'>求和/='2'>百分比(求和)/='3'>计数/'4'>百分比(计数)/'5'>分组/
                for (int m = 0; m < karray.Length; m++)
                {
                    string mode = varray[m];
                    string f = karray[m];
                    if (mode == "5")
                    {
                        selecthead = f;
                        group = f;
                    }
                    else if (mode == "4")
                    {
                        select = "count(*)  ";
                        _toolitp = "数量:";
                    }
                    else if (mode == "3")
                    {
                        select = "count(*) ";
                        _toolitp = "数量:";
                    }
                    else if (mode == "2")
                    {
                        select = "sum(nvl(" + f + ",0)) ";
                    }
                    else if (mode == "1")
                    {
                        select = "sum(nvl(" + f + ",0))  ";
                    }

                }

                if (group.ToUpper() == "OBJYEAR")
                {
                    //当按年分组时，年份不作为where条件
                    statyear = "";
                }
                if (!string.IsNullOrEmpty(statyear))
                {
                    if (statyear != "0")
                    {
                        where = " and  OBJYEAR='" + statyear + "'";
                    }
                }
                ssql = "select " + selecthead + " as groupname," + select + " as total  from " + layercode + " where length(objqx)<5 " + where + " group by " + group;
                // SearchCriteria _search = new SearchCriteria("EAP_RESOURCEOP");
                // _search["parentid"] = moduleId;
                list = dp.LoadData<statuse>(ssql, _params.ToArray());
            }
            var mv = 1;
            // statResult _statResult = new statResult();
            List<statpie> _statResult = new List<statpie>();
            foreach (statuse o in list)
            {
                statpie p = new statpie();
                p.y = (int)(o.total);
                p.text = o.groupname;
                p.stroke = "black";
                p.tooltip = _toolitp + o.total.ToString();
                _statResult.Add(p);
            }
            //[
            //    { y: 4, text: "海淀区", stroke: "black", tooltip: "500平方米" },
            //    { y: 2, text: "朝阳区", stroke: "black", tooltip: "1000平方米" },
            //    { y: 1, text: "东城区", stroke: "black", tooltip: "1500平方米" },
            //    { y: 1, text: "西城区", stroke: "black", tooltip: "800平方米" }
            // ]

            // string _re = serializer.Serialize(_rs.Items.ToList());
            //_re = _rs.Items.ToJson();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(_statResult);
            Context.Response.Write(mm);
            Context.Response.End();
        }
        [WebMethod(EnableSession = true)]
        public void GetSpecData(string layerid, string statyear)
        {
            List<statuse> list = new List<statuse>();
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            if ((_login != null) || (_login.User != null))
            {
                gis_layerDefine layer = null;
                EntityProviderOP<gis_layerDefine> _op = new EntityProviderOP<gis_layerDefine>(_login, DataProvider.GetEAP_Provider());

                SearchCriteria _search = new SearchCriteria("GIS_LAYERDEFINE");
                _search["ID"] = int.Parse(layerid);

                SearchResult<gis_layerDefine> _rs = _op.Search(_search);
                if (_rs.Items.Count > 0)
                {
                    layer = _rs.Items[0];

                }

                DataProvider dp = DataProvider.GetProvider("ConnectionString_SDE");
                List<SqlParameter> _params = new List<SqlParameter>();

                string ssql = "";
                string select = "";
                string where = "";
                if (layer.SpecGroupFN.ToUpper() == "OBJYEAR")
                {
                    //当按年分组时，年份不作为where条件
                    statyear = "";
                }
                if (!string.IsNullOrEmpty(statyear))
                {
                    if (statyear != "0")
                    {
                        where = " and  OBJYEAR='" + statyear + "'";
                    }
                }
                if (layer.SpecStatMode == 0)
                {
                    select = "count(*)  ";

                }
                else
                {
                    select = "sum(nvl(" + layer.SpecStatFN + ",0)) ";
                }



                ssql = "select objqx," + layer.SpecGroupFN + " as groupname," + select + " as total  from " + layer.SpecDS + " where length(objqx)<5 " + where + " group by objqx," + layer.SpecGroupFN;
                // SearchCriteria _search = new SearchCriteria("EAP_RESOURCEOP");
                // _search["parentid"] = moduleId;
                list = dp.LoadData<statuse>(ssql, _params.ToArray());
            }

            // statResult _statResult = new statResult();
            List<specstat> _statResult = new List<specstat>();
            foreach (statuse o in list)
            {
                specstat _p = _statResult.Find((x) => { return x.code.ToUpper() == o.objqx.ToUpper(); });
                if (_p == null)
                {
                    _p = new specstat() { name = o.objqx, code = o.objqx };
                    _statResult.Add(_p);
                }
                statpie p = new statpie();
                p.y = (int)(o.total);
                p.text = o.groupname;
                p.stroke = "black";
                p.tooltip = o.groupname + o.total.ToString();
                _p.children.Add(p);
            }
            //[
            //    { y: 4, text: "海淀区", stroke: "black", tooltip: "500平方米" },
            //    { y: 2, text: "朝阳区", stroke: "black", tooltip: "1000平方米" },
            //    { y: 1, text: "东城区", stroke: "black", tooltip: "1500平方米" },
            //    { y: 1, text: "西城区", stroke: "black", tooltip: "800平方米" }
            // ]

            // string _re = serializer.Serialize(_rs.Items.ToList());
            //_re = _rs.Items.ToJson();
            string callback = Context.Request["callback"];

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // string _re = serializer.Serialize(_rs.Items.ToList());
            //_re = _rs.Items.ToJson();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(_statResult);

            if (callback != null)
            {
                Context.Response.Write(callback + "(" + mm + ")");
            }
            else
            {
                Context.Response.Write(mm);
            }

            Context.Response.End();
        }
        [WebMethod(EnableSession = true)]
        public void SubmitLayerError(string layerid, string layercode, string layername, string eleid, string elename, string errordesc)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string result = "-1";
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            if (_login != null)
            {
                try
                {
                    EntityProviderOP<Gis_Error> eop = new EntityProviderOP<Gis_Error>(_login, DataProvider.GetEAP_Provider());
                    Gis_Error error = new Gis_Error()
                    {
                        LayerID = layerid,
                        LayerCode = layercode,
                        LayerName = layername,
                        EleID = eleid,
                        EleName = elename,
                        ErrorDesc = errordesc,
                        ADDBY = _login.User.TrueName,
                        ADDON = DateTime.Now,
                        Status = 0
                    };
                    eop.Save(error);
                }
                catch
                {
                    result = "0";
                }
                result = "1";
            }

            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(result);
            Context.Response.Write(mm);
            Context.Response.End();
        }
        [WebMethod(EnableSession = true)]
        public void GetErrorList(int status)
        {
            List<Gis_Error> rlist = new List<Gis_Error>();

            try
            {
                LoginInfo _login = (LoginInfo)(Session["loginuser"]);
                EntityProviderOP<Gis_Error> _op = new EntityProviderOP<Gis_Error>(_login, DataProvider.GetEAP_Provider());
                SearchCriteria sc = new SearchCriteria("GIS_ERROR");
                sc["Status"] = status;

                foreach (var error in _op.Search(sc).Items)
                {
                    rlist.Add(error);
                }
            }
            catch { }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(rlist);
            Context.Response.Write(mm);
            Context.Response.End();
        }
        [WebMethod(EnableSession = true)]
        public void ChangeErrorStatus(int id)
        {
            var result = "0";
            try
            {
                LoginInfo _login = (LoginInfo)(Session["loginuser"]);

                EntityProviderOP<Gis_Error> eop = new EntityProviderOP<Gis_Error>(_login, DataProvider.GetEAP_Provider());
                SearchCriteria sc = new SearchCriteria("GIS_ERROR");
                sc["ID"] = id;
                var error = eop.Search(sc).Items[0];
                error.Status = 1;
                eop.Save(error);

                result = "1";
            }
            catch { }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(result);
            Context.Response.Write(mm);
            Context.Response.End();
        }
        [WebMethod(EnableSession = true)]
        public void InsertPics(string layerid, string objid, string picurl, string picdes)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string result = "-1";
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            if (_login != null)
            {
                try
                {
                    EntityProviderOP<EAP_PICS> eop = new EntityProviderOP<EAP_PICS>(_login, DataProvider.GetEAP_Provider());
                    EAP_PICS error = new EAP_PICS()
                    {
                        LAYERID = Convert.ToInt32(layerid),
                        OBJID = Convert.ToInt32(objid),
                        PICURL = picurl,
                        PICDES = picdes,
                        ADDBY = _login.User.TrueName,
                        ADDON = DateTime.Now
                    };
                    eop.Save(error);
                }
                catch
                {
                    result = "0";
                }
                result = "1";
            }
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(result);
            Context.Response.Write(mm);
            Context.Response.End();
        }
        [WebMethod(EnableSession = true)]
        public void InsertVideos(string layerid, string objid, string videourl, string videodes)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string result = "-1";
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);
            if (_login != null)
            {
                try
                {
                    EntityProviderOP<EAP_VIDEOS> eop = new EntityProviderOP<EAP_VIDEOS>(_login, DataProvider.GetEAP_Provider());
                    EAP_VIDEOS error = new EAP_VIDEOS()
                    {
                        LAYERID = Convert.ToInt32(layerid),
                        OBJID = Convert.ToInt32(objid),
                        VIDEOURL = videourl,
                        VIDEODES = videodes,
                        ADDBY = _login.User.TrueName,
                        ADDON = DateTime.Now
                    };
                    eop.Save(error);
                }
                catch
                {
                    result = "0";
                }
                result = "1";
            }
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(result);
            Context.Response.Write(mm);
            Context.Response.End();
        }
        [WebMethod(EnableSession = true)]
        public void GetPics(int id, int objid) 
        {
            List<EAP_PICS> rlist = new List<EAP_PICS>();
            try
            {
                LoginInfo _login = (LoginInfo)(Session["loginuser"]);
                EntityProviderOP<EAP_PICS> eop = new EntityProviderOP<EAP_PICS>(_login, DataProvider.GetEAP_Provider());
                SearchCriteria sc = new SearchCriteria("EAP_PICS");
                sc["LAYERID"] = id;
                sc["OBJID"] = objid;
                foreach (var error in eop.Search(sc).Items)
                {
                    rlist.Add(error);
                }
            }
            catch { }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(rlist);
            Context.Response.Write(mm);
            Context.Response.End();
        }
        [WebMethod(EnableSession = true)]
        public void GetVideos(int id, int objid)
        {
            List<EAP_VIDEOS> rlist = new List<EAP_VIDEOS>();
            try
            {
                LoginInfo _login = (LoginInfo)(Session["loginuser"]);
                EntityProviderOP<EAP_VIDEOS> eop = new EntityProviderOP<EAP_VIDEOS>(_login, DataProvider.GetEAP_Provider());
                SearchCriteria sc = new SearchCriteria("EAP_VIDEOS");
                sc["LAYERID"] = id;
                sc["OBJID"] = objid;
                foreach (var error in eop.Search(sc).Items)
                {
                    rlist.Add(error);
                }
            }
            catch { }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(rlist);
            Context.Response.Write(mm);
            Context.Response.End();
        }
        [WebMethod(EnableSession = true)]
        public void DeleteVideos(int id)
        {
            var result = "-1";
            try
            {
                LoginInfo _login = (LoginInfo)(Session["loginuser"]);
                EntityProviderOP<EAP_VIDEOS> eop = new EntityProviderOP<EAP_VIDEOS>(_login, DataProvider.GetEAP_Provider());
                EAP_VIDEOS error = new EAP_VIDEOS()
                {
                    ID=id,
                    S = EntityStatus.Delete
                };
                eop.Save(error);
                result = "1";
            }
            catch
            {
                result = "0";
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(result);
            Context.Response.Write(mm);
            Context.Response.End();
        }
        [WebMethod(EnableSession = true)]
        public void DeletePics(int id)
        {
            var result = "-1";
            try
            {
                LoginInfo _login = (LoginInfo)(Session["loginuser"]);
                EntityProviderOP<EAP_PICS> eop = new EntityProviderOP<EAP_PICS>(_login, DataProvider.GetEAP_Provider());
                EAP_PICS error = new EAP_PICS()
                {
                    ID = id,
                    S = EntityStatus.Delete
                };
                eop.Save(error);
                result = "1";
            }
            catch
            {
                result = "0";
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(result);
            Context.Response.Write(mm);
            Context.Response.End();
        }
        [WebMethod(EnableSession = true)]
        public void GetUserMenu()
        {
            LoginInfo _login = (LoginInfo)(Session["loginuser"]);

            string _portid = "156";
            try
            {
                var pid = WebConfigurationManager.AppSettings["GisPortID"];
                if (pid != null && pid.Length > 0)
                {
                    _portid = pid;
                }
            }
            catch { }

            var dp = DataProvider.GetEAP_Provider();
            string ssql1 = string.Format("  select r.*   from eap_resource  r  join   eap_rolemodule  b on r.id=b.sourceid " +
             "  where  r.type='menu' and r.parentid={0}  and  b.roleid in  (select c.roleid from eap_userrole  c where c.userid={1})    order by r.sortby  ", _portid, _login.User.ID);
            List<EAP_Resource> layerlist = dp.LoadData<EAP_Resource>(ssql1);
            List<gis_layerclassMoudle> result = new List<gis_layerclassMoudle>();
            foreach (EAP_Resource o in layerlist)
            {
                result.Add(new gis_layerclassMoudle()
                {
                    ID = o.ID,
                    NAME = o.Name,
                    ENAME = o.Description,
                    TYPE = o.VRow,
                    IMG = o.Image,
                    HREF = o.PageId
                });
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Context.Response.Charset = "utf-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            string mm = serializer.Serialize(result);
            Context.Response.Write(mm);
            Context.Response.End();
        }



























        /// <summary>
        /// 获取网络图片
        /// </summary>
        private bool CompareWebImg(string newUrl, string oldUrl) 
        {
            Bitmap img1 = null;
            Bitmap img2 = null;
            HttpWebRequest req1 = null;
            HttpWebRequest req2 = null;
            HttpWebResponse res1 = null;
            HttpWebResponse res2 = null;
            int diffCount = 0;
            bool isSame = false;
            try
            {
                Uri oldUri = new Uri(oldUrl);
                Uri newUri = new Uri(newUrl);
                req1 = (HttpWebRequest)WebRequest.Create(oldUri);
                req2 = (HttpWebRequest)WebRequest.Create(newUri);
                req1.Timeout = 100000;
                req2.Timeout = 100000;
                req1.Method = "GET";
                req2.Method = "GET";
                res1 = (HttpWebResponse)req1.GetResponse();
                res2 = (HttpWebResponse)req2.GetResponse();
                img1 = new Bitmap(res1.GetResponseStream());
                img2 = new Bitmap(res2.GetResponseStream());

                for (int i = 0; i < img1.Width; i++)
                {
                    for (int j = 0; j < img1.Height; j++) 
                    {
                        var img1Ref = img1.GetPixel(i, j).ToString();
                        var img2Ref = img2.GetPixel(i, j).ToString();
                        if (img1Ref != img2Ref)
                        {
                            diffCount++;
                        }
                    }
                }

                if (diffCount > 1000) 
                {
                    isSame = true;
                }
            }
            catch (Exception ex)
            {
            }
            finally 
            {
                res1.Close();
                res2.Close();
            }

            return isSame;
        }

        
        
        
        
        [WebMethod(EnableSession = true)]
        public void GetTileUrl(int level, string minx, string miny, 
            string maxx, string maxy, int type, int year1, int year2, int objid)
        {
           
            for (int i = 0; i < TileModel.lods.Count; i++)
            {
                if (TileModel.lods[i].Level == level)
                {
                    var point = new Point(Convert.ToDouble(minx), Convert.ToDouble(maxy));

                    int tileWidth = 256;
                    int tileHeight = 256;

                    var width = tileWidth * TileModel.lods[i].Resolution;
                    var height = tileHeight * TileModel.lods[i].Resolution;
                    var row = Convert.ToInt32((TileModel.orginY - point.y) / width);
                    var col = Convert.ToInt32((point.x - TileModel.orginX) / height);

                    int r = (int)Math.Floor((point.x - TileModel.orginX) / row);
                    int c = (int)Math.Floor((point.y - TileModel.orginY) / col);

                    var offsetX = Math.Floor(Math.Abs(point.x - (TileModel.orginX + r * row)) * 256 / row);
                    var offsetY = Math.Floor(Math.Abs(point.y - (TileModel.orginY + c * col)) * 256 / col);

                    var lod = TileModel.lods[i];

                    int startRow = TileModel.lods[i].StartTileRow;
                    int startCol = TileModel.lods[i].StartTileCol;
                    int endRow = TileModel.lods[i].EndTileRow;
                    int endCol = TileModel.lods[i].EndTileCol;


                    var offset_X = 0 - offsetX;
                    var offset_Y = 0 - offsetY;


                    var positionX = 256 - offset_X;//t
                    var positionY = 256 - offset_Y;//x


                    positionX = 0 < positionX ? (positionX % 256) : 256 - (Math.Abs(positionX) % 256);
                    positionY = 0 < positionY ? (positionY % 256) : 256 - (Math.Abs(positionY) % 256);

                    int x = (int)Math.Ceiling(0 - (256 - offset_X) / 256);
                    int y = (int)Math.Ceiling(0 - (256 - offset_Y) / 256);
                    //默认设置map空间大小为600和600
                    int g = x + (int)Math.Ceiling((600 - positionX) / 256);
                    int a = y + (int)Math.Ceiling((600 - positionY) / 256);

                    //n = 0 < n ? Math.floor((n + Rect) / d) : y((n - (d - Rect)) / d);
                    //connect = 0 < connect ? Math.floor((connect + G) / b) : y((connect - (b - G)) / b);
                    //G = n + y((a.width - t) / d);
                    //a = connect + y((a.height - x) / b);

                    List<Matrix> matrix = new List<Matrix>();
                    for (var m = x; m < g; m++) 
                    {
                        for (x = y; x <= a; x++)
                        {
                            matrix.Add(new Matrix(x + row, m + col));
                            //row = x + r;
                            //col = m + c;
                            //y = f + n;//row
                            //t = k + x;//col
                        }
                    }



                    List<ImgModel> results = new List<ImgModel>();
                    for (int re = 0; re < matrix.Count; re++) 
                    {
                        var item = matrix[re];
                        if (type == 0) //矢量
                        {
                            string oldUrl = TileModel.VecUrl + year1 + "&TILEMATRIX=" + level + "&TILEROW=" + item.row + "&TILECOL=" + item.col;
                            string newUrl = TileModel.VecUrl + year2 + "&TILEMATRIX=" + level + "&TILEROW=" + item.row + "&TILECOL=" + item.col;
                            if (CompareWebImg(newUrl, oldUrl)) 
                            {
                                results.Add(new ImgModel()
                                {
                                    OldUrl = oldUrl,
                                    NewUrl = newUrl
                                });
                            }
                        }
                        else if (type == 3) //航片
                        {
                            string oldUrl = TileModel.ImgUrl + year1 + "&TILEMATRIX=" + level + "&TILEROW=" + item.row + "&TILECOL=" + item.col;
                            string newUrl = TileModel.ImgUrl + year2 + "&TILEMATRIX=" + level + "&TILEROW=" + item.row + "&TILECOL=" + item.col;
                            if (CompareWebImg(newUrl, oldUrl))
                            {
                                results.Add(new ImgModel()
                                {
                                    OldUrl = oldUrl,
                                    NewUrl = newUrl
                                });
                            }
                        }
                        else //卫片
                        {
                            string oldUrl = TileModel.CUrl + year1 + "&TILEMATRIX=" + level + "&TILEROW=" + item.row + "&TILECOL=" + item.col;
                            string newUrl = TileModel.CUrl + year2 + "&TILEMATRIX=" + level + "&TILEROW=" + item.row + "&TILECOL=" + item.col;
                            if (CompareWebImg(newUrl, oldUrl)) 
                            {
                                results.Add(new ImgModel()
                                {
                                    OldUrl = oldUrl,
                                    NewUrl = newUrl
                                });
                            }
                        }
                    }

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    Context.Response.Charset = "utf-8";
                    Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
                    string mm = serializer.Serialize(results);
                    try
                    {
                        Guid fileName = Guid.NewGuid();
                        string path = Context.Server.MapPath("/logs/" + fileName + ".json");

                        if (!File.Exists(path))
                        {
                            using (StreamWriter sw = File.CreateText(path))
                            {
                                sw.Write(mm);
                            }
                        }
                        Context.Response.Write(fileName);
                        Context.Response.End();
                    }
                    catch (Exception ex)
                    {
                        //Context.Response.Write("无权限：" + ex.ToString());
                        //Context.Response.End();
                    }
                }
            }


            /*
             if (this._ct) {
                var c;
                var d = this._tileW;
                var b = this._tileH;
                var e = this._ct;
                c = e.lod;
                var e = e.tile;
                var g = e.offsets;
                var k = e.coords;
                var f = k.row;
                var k = k.col;
                var domStyle = c.level;//level
                var m = this.opacity;
                var h = this._tileIds;
               var  p = this._loadingList;
                var r = this._addImage;
                var kernel = this._map.id;
                var domConstruct = this.id;
                var n = a.x;
                var connect = a.y;
                var sniff = c.startTileRow;
                var Point = c.endTileRow;
                var declare = c.startTileCol;
                var lang = c.endTileCol;
                var tileUtils = arrayUtils.indexOf;
                var y,
                t;
                var Rect = g.x - this.__coords_dx;
                var G = g.y - this.__coords_dy;
                var t = d - Rect + -a.x;
                var x = b - G + -a.y;
                y = Math.ceil;
                t = 0 < t ? t % d: d - Math.abs(t) % d;
                x = 0 < x ? x % b: b - Math.abs(x) % b;
                n = 0 < n ? Math.floor((n + Rect) / d) : y((n - (d - Rect)) / d);
                connect = 0 < connect ? Math.floor((connect + G) / b) : y((connect - (b - G)) / b);
                G = n + y((a.width - t) / d);
                a = connect + y((a.height - x) / b);
                var E, H, I;
                this._wrap && (E = c._frameInfo, H = E[0], I = E[1], E = E[2]);
                for (x = n; x <= G; x++) {
					for (n = connect; n <= a; n++) {
						y = f + n;//row
						t = k + x;//col
						if(this._wrap){
							(t < I ? (t %= H, t = t < I ? t + H: t) : t > E && (t %= H));
						}
						//domStyle表示level
						if(!this._isExcluded(domStyle, y, t)){
							if((y >= sniff && y <= Point && t >= declare && t <= lang)){ 
								if(c = kernel + "_" + domConstruct + "_tile_" + domStyle + "_" + n + "_" + x, -1 === tileUtils(h, c)){
									p.add(c);
									h.push(c); 
									r(domStyle, n, y, x, t, c, d, b, m, e, g);
								}
							}
							
						}
					}
				}
            }

        }
             */

        //    var point = new Point(Convert.ToDouble(minx), Convert.ToDouble(maxy));
        //    if (objid != 1)
        //    {
        //        List<ImgModel> results = new List<ImgModel>();
        //        List<Matrix> matrix = new List<Matrix>();
        //        for (int i = 0; i < TileModel.lods.Count; i++)
        //        {
        //            if (TileModel.lods[i].Level == level)
        //            {
        //                var width = 256 * TileModel.lods[i].Resolution;
        //                var height = 256 * TileModel.lods[i].Resolution;
        //                var row = (TileModel.orginY - point.y) / width;
        //                var col = (point.x - TileModel.orginX) / height;

        //                var r = Math.Floor((point.x - TileModel.orginX) / row);
        //                var c = Math.Floor((point.y - TileModel.orginY) / col);

        //                var offsetX = Math.Floor(Math.Abs(point.x - (TileModel.orginX + r * row)) * 256 / row);
        //                var offsetY = Math.Floor(Math.Abs(point.y - (TileModel.orginY + c * col)) * 256 / col);

        //                var lod = TileModel.lods[i];

        //                int startRow = TileModel.lods[i].StartTileRow;
        //                int startCol = TileModel.lods[i].StartTileCol;
        //                int endRow = TileModel.lods[i].EndTileRow;
        //                int endCol = TileModel.lods[i].EndTileCol;
        //                for (int j = startRow; j <= endRow; j++)
        //                {
        //                    for (int m = startCol; m <= endCol; m++)
        //                    {
        //                        matrix.Add(new Matrix(j, m));
        //                    }
        //                }
        //            }
        //        }
        //        //矢量图
        //        if (type == 0)
        //        {
        //            if (level == 0 || level == 1 || level == 2)
        //            {
        //                foreach (var item in matrix)
        //                {
        //                    results.Add(new ImgModel()
        //                    {
        //                        OldUrl = TileModel.VecUrl + year1 + "&TILEMATRIX=" + level + "&TILEROW=" + item.row + "&TILECOL=" + item.col,
        //                        NewUrl = TileModel.VecUrl + year2 + "&TILEMATRIX=" + level + "&TILEROW=" + item.row + "&TILECOL=" + item.col,
        //                    });
        //                }
        //            }
        //            else
        //            {
        //                for (int i = 0; i < 5; i++)
        //                {
        //                    var item = matrix[matrix.Count / 2 + i];

        //                    results.Add(new ImgModel()
        //                    {
        //                        OldUrl = TileModel.VecUrl + year1 + "&TILEMATRIX=" + level + "&TILEROW=" + item.row + "&TILECOL=" + item.col,
        //                        NewUrl = TileModel.VecUrl + year2 + "&TILEMATRIX=" + level + "&TILEROW=" + item.row + "&TILECOL=" + item.col,
        //                    });
        //                }
        //            }
        //        }
        //        else if (type == 3)//航片
        //        {
        //            if (level == 1 && level == 2)
        //            {
        //                foreach (var item in matrix)
        //                {
        //                    results.Add(new ImgModel()
        //                    {
        //                        OldUrl = TileModel.ImgUrl + year1 + "&TILEMATRIX=" + level + "&TILEROW=" + item.row + "&TILECOL=" + item.col,
        //                        NewUrl = TileModel.ImgUrl + year2 + "&TILEMATRIX=" + level + "&TILEROW=" + item.row + "&TILECOL=" + item.col,
        //                    });
        //                }
        //            }
        //            else
        //            {
        //                for (int i = 0; i < 5; i++)
        //                {
        //                    var item = matrix[i];
        //                    results.Add(new ImgModel()
        //                    {
        //                        OldUrl = TileModel.ImgUrl + year1 + "&TILEMATRIX=" + level + "&TILEROW=" + item.row + "&TILECOL=" + item.col,
        //                        NewUrl = TileModel.ImgUrl + year2 + "&TILEMATRIX=" + level + "&TILEROW=" + item.row + "&TILECOL=" + item.col,
        //                    });
        //                }
        //            }
        //        }
        //        JavaScriptSerializer serializer = new JavaScriptSerializer();
        //        Context.Response.Charset = "utf-8";
        //        Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
        //        string mm = serializer.Serialize(results);
        //        try
        //        {
        //            Guid fileName = Guid.NewGuid();
        //            string path = Context.Server.MapPath("/logs/" + fileName + ".json");

        //            if (!File.Exists(path))
        //            {
        //                using (StreamWriter sw = File.CreateText(path))
        //                {
        //                    sw.Write(mm);
        //                }
        //            }
        //            Context.Response.Write(fileName);
        //            Context.Response.End();
        //        }
        //        catch (Exception ex)
        //        {
        //            //Context.Response.Write("无权限：" + ex.ToString());
        //            //Context.Response.End();
        //        }
        //    }
        //    else if (objid == 1)
        //    {
        //        JavaScriptSerializer serializer = new JavaScriptSerializer();
        //        Context.Response.Charset = "utf-8";
        //        Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
        //        string fileName = "";
        //        if (type == 0) //矢量
        //        {
        //            fileName = "2f3d3f47-15fa-4392-9cf5-0afa0f0a5b1b";
        //        }
        //        else if (type == 3) //航片
        //        {
        //            fileName = "05e9a84a-9280-432b-bb90-84ead03bafc2";
        //        }
        //        Context.Response.Write(fileName);
        //        Context.Response.End();
        //    }
        }
    }
}