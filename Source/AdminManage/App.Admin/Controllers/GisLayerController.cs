using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Common;
using App.Models;
using Microsoft.Practices.Unity;

using Gis.Models;
using App.Models.Sys;
using NM.OP;
using NM.Model;
using System.Data.SqlClient;
using System;
using Newtonsoft.Json;
namespace App.Admin.Controllers
{
    public class GisLayerController : BaseController
    {
        //

        ValidationErrors errors = new ValidationErrors();

        [SupportFilter]
        public ActionResult Index()
        {
            ViewBag.Perm = GetPermission();
            return View();
        }


        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(GridPager pager, string queryStr)
        {
            EntityProviderOP<gis_layerDefine> _op = new EntityProviderOP<gis_layerDefine>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("GIS_LAYERDEFINE");
            ToSearchCriteria(pager, ref _search);
            _search["LAYERNAME"] = queryStr;

            SearchResult<gis_layerDefine> _rs = _op.Search(_search);


            var json = new
            {
                total = _rs.TotalCount,
                rows = _rs.Items.ToArray()

            };

            return Json(json);


        }

        [SupportFilter(ActionName = "Index")]
        public ActionResult CreateXY(string id)
        {
            ViewBag.Perm = GetPermission();
            GIS_BASEMAP entity = GetObjByID<GIS_BASEMAP>(id, null, "LayerID");
            if (entity == null)
            {
                entity = new GIS_BASEMAP() { LayerID = int.Parse(id) };
            }
            //SysRoleModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [SupportFilter(ActionName = "Index")]
        public ActionResult SetlayerCols(string id)
        {
            ViewBag.Perm = GetPermission();
            ViewBag.LayerID = id;
            //SysRoleModel entity = m_BLL.GetById(id);
            return View();
        }


       [SupportFilter(ActionName = "Index")]
        public ActionResult SetLayersub(string id)
        {
            ViewBag.Perm = GetPermission();
            ViewBag.LayerID = id;
            //SysRoleModel entity = m_BLL.GetById(id);
            return View();
        }
        
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult getColsList(GridPager pager, string id)
        {
            ViewBag.Perm = GetPermission();
            EntityProviderOP<gis_layerVersionCol> _op = new EntityProviderOP<gis_layerVersionCol>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("GIS_LAYERVERSIONCOL");
            ToSearchCriteria(pager, ref _search);
            _search["LayerID"] = id;

            SearchResult<gis_layerVersionCol> _rs = _op.Search(_search);


            var json = new
            {
                total = _rs.TotalCount,
                rows = _rs.Items.ToArray()

            };

            return Json(json);
            //SysRoleModel entity = m_BLL.GetById(id);

        }

        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult SavecolsList()
        {
            string result = Request.Form[0];

            var changelist = JsonConvert.DeserializeObject<List<gis_layerVersionColpo>>(result);              
                

            //后台拿到字符串时直接反序列化。根据需要自己处理

            var layercollist = JsonConvert.DeserializeObject<List<gis_layerVersionCol>>(result);

            string ErrorCol="";
            int rid = 1;
            foreach (var o in layercollist)
            {
                var co = changelist.Find(x => { return x.ID == o.ID; });
                if (co != null)
                {
                    o.COLNAME = co.COLNAME;
                    o.ISVISIBLE = co.ISVISIBLE;
                    o.ODERNUM = co.ODERNUM;
                    o.ISGROUP = co.ISGROUP;
                }
                LoginInfo _login = LoginUser;
                CommandResult r = SaveObj<gis_layerVersionCol>(o);
                if (r.IntResult > 0)
                {
                    //LogHandler.WriteServiceLog(_login.User.UserID.ToString(), "Id:" + o.ID + ",Name:" + o.COLNAME, "成功", "保存", "字段定义");
                    //return Json(JsonHandler.CreateMessage(1, Suggestion.Save), JsonRequestBehavior.AllowGet);
                }
                else
                {
                      ErrorCol = r.Message;
                    LogHandler.WriteServiceLog(_login.User.UserID.ToString(), "Id:" + o.ID + ",Name:" + o.COLNAME + "," + ErrorCol, "失败", "保存", "字段定义");
                    //return Json(JsonHandler.CreateMessage(0, Suggestion.SaveFail + ErrorCol), JsonRequestBehavior.AllowGet);
                    rid = 0;
                }

            }
            if (rid == 0)
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.SaveFail + ErrorCol), JsonRequestBehavior.AllowGet);
            }
            else
            { 
                return Json(JsonHandler.CreateMessage(1, Suggestion.SetSucceed), JsonRequestBehavior.AllowGet);
            }
            
        }


        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult getlayerSubList(GridPager pager, string id)
        {
            ViewBag.Perm = GetPermission();
            EntityProviderOP<gis_layerdefine_sub> _op = new EntityProviderOP<gis_layerdefine_sub>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("GIS_LAYERDEFINE_SUB");
            ToSearchCriteria(pager, ref _search);
            _search["LAYERID"] = id;

            SearchResult<gis_layerdefine_sub> _rs = _op.Search(_search);
            var json = new
            {
                total = _rs.TotalCount,
                rows = _rs.Items.ToArray()

            };
            return Json(json);
        }

        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult SavelayerSubList()
        {
             string result = Request.Form[0];

             var changelist = JsonConvert.DeserializeObject<List<gis_layerdefine_subpoco>>(result);              
                

            //后台拿到字符串时直接反序列化。根据需要自己处理

             var layersublist = JsonConvert.DeserializeObject<List<gis_layerdefine_sub>>(result);

            string ErrorCol="";
            int rid = 1;
            foreach (var o in layersublist)
            {
                var co = changelist.Find(x => { return x.ID == o.ID; });
                if (co != null)
                {
                    o.PROPVALUE = co.PROPVALUE;
                    o.ISVISIBLE = co.ISVISIBLE;
                    o.ODERNUM = co.ODERNUM;
                }
                LoginInfo _login = LoginUser;
                CommandResult r = SaveObj<gis_layerdefine_sub>(o);
                if (r.IntResult > 0)
                {
                    //LogHandler.WriteServiceLog(_login.User.UserID.ToString(), "Id:" + o.ID + ",Name:" + o.COLNAME, "成功", "保存", "字段定义");
                    //return Json(JsonHandler.CreateMessage(1, Suggestion.Save), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ErrorCol = r.Message;
                    LogHandler.WriteServiceLog(_login.User.UserID.ToString(), "Id:" + o.ID + ",Name:" + o.PROPNAME + "," + ErrorCol, "失败", "保存", "图层元数据定义");
                    //return Json(JsonHandler.CreateMessage(0, Suggestion.SaveFail + ErrorCol), JsonRequestBehavior.AllowGet);
                    rid = 0;
                }
            }
            if (rid == 0)
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.SaveFail + ErrorCol), JsonRequestBehavior.AllowGet);
            }
            else
            { 
                return Json(JsonHandler.CreateMessage(1, Suggestion.SetSucceed), JsonRequestBehavior.AllowGet);
            }

 
        }

        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult CreateXY(GIS_BASEMAP model)
        {
            if (model != null && ModelState.IsValid)
            {

                LoginInfo _login = LoginUser;

                GIS_BASEMAP obj = null;


                obj = GetObjByID<GIS_BASEMAP>(model.LayerID.ToString(), null, "LayerID");
                if (obj != null)
                {
                    obj.LayerCode = model.LayerCode;
                    obj.LAYERNAME = model.LAYERNAME;
                    obj.DSOURCE = model.DSOURCE;
                    obj.SERVICEURL = model.SERVICEURL;
                    obj.SERVICEINDEX = model.SERVICEINDEX;
                    obj.EDITBY = _login.User.UserName;
                    obj.EDITON = DateTime.Now;

                }
                else
                {
                    obj = new GIS_BASEMAP();
                    gis_layerDefine _layer = GetObjByID<gis_layerDefine>(model.LayerID.ToString());
                    obj.LayerCode = _layer.LAYERCODE;
                    obj.LAYERNAME = _layer.LAYERNAME;

                    obj.DSOURCE = _layer.DSOURCE;
                    obj.SERVICEURL = _layer.SERVICEURL;
                    obj.SERVICEINDEX = _layer.SERVICEINDEX;
                    obj.ADDBY = _login.User.UserName;
                }

                obj.LayerID = model.LayerID;
                obj.XMAX = model.XMAX;
                obj.YMAX = model.YMAX;
                obj.XMIN = model.XMIN;
                obj.YMIN = model.YMIN;
                obj.COORSYS = model.COORSYS;
                obj.YEAR = model.YEAR;


                CommandResult r = SaveObj<GIS_BASEMAP>(obj);
                if (r.IntResult > 0)
                {
                    LogHandler.WriteServiceLog(GetUserName(), "Id" + model.ID + ",Name" + model.LAYERNAME, "成功", "修改", "图层范围设置");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.EditSucceed));
                }
                else
                {
                    string ErrorCol = r.Message;
                    LogHandler.WriteServiceLog(GetUserName(), "Id" + model.ID + ",Name" + model.LAYERNAME + "," + ErrorCol, "失败", "修改", "图层范围设置");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail));
            }
        }

        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
            ViewBag.Perm = GetPermission();
            ViewBag.shptypelist = initlist(1);
            ViewBag.datatypelist = initlist(2);
            ViewBag.layertypelist = initlist(3);

            //ViewBag.specShowModelist= initlist(10);
            //ViewBag.specGroupFNlist = initlist(11);
            //ViewBag.specStatModeList = initlist(12);
            //ViewBag.SpecStatFNlist = initlist(13);

            gis_layerDefine m = new gis_layerDefine();
            return View(m);
        }

        List<SelectListItem> initlist(int seltype, string selectValue = "0",string layercode="")
        {
            List<SelectListItem> m = new List<SelectListItem>();
            if (seltype == 3)
            {
                #region
                //0：基础图层1：业务图层2;  专题图层
                {
                    SelectListItem o = new SelectListItem();
                    o.Value = "0";
                    o.Text = "基础图层";
                    if ("0" == selectValue)
                    {
                        o.Selected = true;
                    }
                    m.Add(o);
                }
                //0：基础图层1：业务图层2;  专题图层
                {
                    SelectListItem o = new SelectListItem();
                    o.Value = "1";
                    o.Text = "业务图层";
                    if ("1" == selectValue)
                    {
                        o.Selected = true;
                    }
                    m.Add(o);
                }
                {
                    SelectListItem o = new SelectListItem();
                    o.Value = "2";
                    o.Text = "专题图层";
                    if ("2" == selectValue)
                    {
                        o.Selected = true;
                    }
                    m.Add(o);
                }
                #endregion

            }
            else if (seltype == 2)
            {
                #region
                ///  0:矢量1：影像
                {
                    SelectListItem o = new SelectListItem();
                    o.Value = "0";
                    o.Text = "矢量";
                    if ("0" == selectValue)
                    {
                        o.Selected = true;
                    }
                    m.Add(o);
                }

                {
                    SelectListItem o = new SelectListItem();
                    o.Value = "1";
                    o.Text = "影像";
                    if ("1" == selectValue)
                    {
                        o.Selected = true;
                    }
                    m.Add(o);
                }

                #endregion

            }
            else if (seltype == 1)
            {
                #region
                ///  0:面1：线2：点
                {
                    SelectListItem o = new SelectListItem();
                    o.Value = "0";
                    o.Text = "面";
                    if ("0" == selectValue)
                    {
                        o.Selected = true;
                    }
                    m.Add(o);
                }
                ///  0:面1：线2：点
                {
                    SelectListItem o = new SelectListItem();
                    o.Value = "1";
                    o.Text = "线";
                    if ("1" == selectValue)
                    {
                        o.Selected = true;
                    }
                    m.Add(o);
                }
                {
                    SelectListItem o = new SelectListItem();
                    o.Value = "2";
                    o.Text = "点";
                    if ("2" == selectValue)
                    {
                        o.Selected = true;
                    }
                    m.Add(o);
                }
                #endregion

            }
            else if (seltype == 10)
            {
                //设置显示模式
                #region
                ///0:直接显示1：柱状图2：饼图3：聚类图
                {
                    SelectListItem o = new SelectListItem();
                    o.Value = "0";
                    o.Text = "直接显示";
                    if ("0" == selectValue)
                    {
                        o.Selected = true;
                    }
                    m.Add(o);
                }
                {
                    SelectListItem o = new SelectListItem();
                    o.Value = "1";
                    o.Text = "柱状图";
                    if ("1" == selectValue)
                    {
                        o.Selected = true;
                    }
                    m.Add(o);
                }
                {
                    SelectListItem o = new SelectListItem();
                    o.Value = "2";
                    o.Text = "饼图";
                    if ("2" == selectValue)
                    {
                        o.Selected = true;
                    }
                    m.Add(o);
                }
                {
                    SelectListItem o = new SelectListItem();
                    o.Value = "3";
                    o.Text = "聚类图";
                    if ("3" == selectValue)
                    {
                        o.Selected = true;
                    }
                    m.Add(o);
                }
                #endregion
            }
            else if (seltype == 11)
            {
                //设置分组字段，从可用的分组字段中选择；
                #region
                if (!string.IsNullOrEmpty(layercode))
                {
                    DataProvider dp = DataProvider.GetEAP_Provider();
                    List<SqlParameter> _params = new List<SqlParameter>();

                    string ssql = string.Format("select a.*  from  uv_gis_layerVersionColspec a where a.layercode='{0}' and a.isgroup=1 and isvisible=1  order by odernum "
                            , layercode);
                    List<gis_layerVersionCol> _list = dp.LoadData<gis_layerVersionCol>(ssql, _params.ToArray());

                    foreach (gis_layerVersionCol col in _list)
                    {
                        SelectListItem o = new SelectListItem();
                        o.Value = col.COLCODE;
                        o.Text = col.COLNAME;
                        if (!string.IsNullOrEmpty(selectValue))
                        {
                            if (string.Compare(col.COLCODE.ToLower(), selectValue.ToLower()) == 0)
                            {
                                o.Selected = true;
                            }
                        }
                        m.Add(o);

                    }
                }
             
                #endregion
            }
            else if (seltype == 12)
            {
                //设置统计模式模式
                #region
                ///0：计数1：求和
                {
                    SelectListItem o = new SelectListItem();
                    o.Value = "0";
                    o.Text = "计数";
                    if ("0" == selectValue)
                    {
                        o.Selected = true;
                    }
                    m.Add(o);
                }
                {
                    SelectListItem o = new SelectListItem();
                    o.Value = "1";
                    o.Text = "求和";
                    if ("1" == selectValue)
                    {
                        o.Selected = true;
                    }
                    m.Add(o);
                }
              
                #endregion
            }
            else if (seltype == 13)
            {
                //设置求合计字段，从数值型字段中选择；；
                if (!string.IsNullOrEmpty(layercode))
                {
                    DataProvider dp = DataProvider.GetEAP_Provider();
                    List<SqlParameter> _params = new List<SqlParameter>();

                    string ssql = string.Format("select a.*  from  uv_gis_layerVersionColspec a where a.layercode='{0}' and a.coltype=2 and isvisible=1  order by odernum "
                            , layercode);
                    List<gis_layerVersionCol> _list = dp.LoadData<gis_layerVersionCol>(ssql, _params.ToArray());
                    #region
                    foreach (gis_layerVersionCol col in _list)
                    {
                        SelectListItem o = new SelectListItem();
                        o.Value = col.COLCODE;
                        o.Text = col.COLNAME;
                        if (!string.IsNullOrEmpty(selectValue))
                        {
                            if (string.Compare(col.COLCODE.ToLower(), selectValue.ToLower()) == 0)
                            {
                                o.Selected = true;
                            }
                        }
                        m.Add(o);

                    }
                }

                #endregion
            }
            return m;

        }
        [HttpPost]
        [SupportFilter]
        public JsonResult Create(gis_layerDefine model)
        {
            if (model != null && ModelState.IsValid)
            {
                LoginInfo _login = LoginUser;
                model.ADDBY = _login.User.UserName;
                CommandResult r = SaveObj<gis_layerDefine>(model);
                if (r.IntResult > 0)
                {
                    LogHandler.WriteServiceLog(_login.User.UserID.ToString(), "Id:" + model.ID + ",Name:" + model.LAYERNAME, "成功", "保存", "图层定义");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.Save), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = r.Message;
                    LogHandler.WriteServiceLog(_login.User.UserID.ToString(), "Id:" + model.ID + ",Name:" + model.LAYERNAME + "," + ErrorCol, "失败", "保存", "图层定义");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.SaveFail + ErrorCol), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.SaveFail), JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region 修改
        [SupportFilter]
        public ActionResult Edit(string id)
        {
            ViewBag.Perm = GetPermission();
            gis_layerDefine entity = GetObjByID<gis_layerDefine>(id);
            ViewBag.shptypelist = initlist(1, entity.SHPTYPE.ToString());
            ViewBag.datatypelist = initlist(2, entity.DATATYPE.ToString());
            ViewBag.layertypelist = initlist(3, entity.LAYERTYPE.ToString());
            //SysRoleModel entity = m_BLL.GetById(id);

            ViewBag.specShowModelist = initlist(10,entity.SpecShowMode.ToString(),entity.SpecDS);
            ViewBag.specGroupFNlist = initlist(11, entity.SpecGroupFN, entity.SpecDS);
            ViewBag.specStatModeList = initlist(12, entity.SpecStatMode.ToString(), entity.SpecDS);
            ViewBag.SpecStatFNlist = initlist(13, entity.SpecStatFN, entity.SpecDS);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(gis_layerDefine model)
        {
            if (model != null && ModelState.IsValid)
            {

                LoginInfo _login = LoginUser;

                gis_layerDefine o = GetObjByID<gis_layerDefine>(model.ID.ToString());
                o.LAYERCODE = model.LAYERCODE;
                o.LAYERNAME = model.LAYERNAME;
                o.LAYERTYPE = model.LAYERTYPE;
                o.DATATYPE = model.DATATYPE;
                o.SHPTYPE = model.SHPTYPE;
                o.ISHISTORY = model.ISHISTORY;
                o.DSOURCE = model.DSOURCE;
                o.SERVICEURL = model.SERVICEURL;
                o.SERVICEINDEX = model.SERVICEINDEX;
                o.EDITBY = _login.User.UserName;
                o.EDITON = DateTime.Now;
                o.DataYears = model.DataYears;
                o.GisDataFields = model.GisDataFields;
                o.VLEVEL = model.VLEVEL;
                o.SpecDS = model.SpecDS;
                o.SpecGroupFN = model.SpecGroupFN;
                o.SpecShowMode = model.SpecShowMode;
                o.SpecStatFN = model.SpecStatFN;
                o.SpecStatMode = model.SpecStatMode;
                o.FEATURESERVICEURL = model.FEATURESERVICEURL;
                o.FEATURESERVICEINDEX = model.FEATURESERVICEINDEX;
                CommandResult r = SaveObj<gis_layerDefine>(o);
                if (r.IntResult > 0)
                {
                    LogHandler.WriteServiceLog(GetUserName(), "Id" + model.ID + ",Name" + model.LAYERNAME, "成功", "修改", "图层设置");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.EditSucceed));
                }
                else
                {
                    string ErrorCol = r.Message;
                    LogHandler.WriteServiceLog(GetUserName(), "Id" + model.ID + ",Name" + model.LAYERNAME + "," + ErrorCol, "失败", "修改", "图层设置");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail));
            }
        }
        #endregion

        #region 详细
        [SupportFilter]
        public ActionResult Details(string id)
        {
            ViewBag.Perm = GetPermission();
            gis_layerDefine entity = GetObjByID<gis_layerDefine>(id);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public JsonResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                string usid = LoginUser.User.UserName;
                gis_layerDefine m = new gis_layerDefine() { ID = int.Parse(id) };
                CommandResult r = DeleteObj<gis_layerDefine>(m);
                // if (m_BLL.Delete(ref errors, id))
                if (r.IntResult > 0)
                {
                    LogHandler.WriteServiceLog(usid, "Id:" + id, "成功", "删除", "图层定义");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = r.Message;
                    LogHandler.WriteServiceLog(usid, "Id" + id + "," + ErrorCol, "失败", "删除", "图层定义");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail));
            }


        }
        #endregion


        #region 删除
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult DeleteXY(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                string usid = LoginUser.User.UserName;
                GIS_BASEMAP m = new GIS_BASEMAP() { LayerID = int.Parse(id) };
                CommandResult r = DeleteObj<GIS_BASEMAP>(m, "LayerID");
                // if (m_BLL.Delete(ref errors, id))
                if (r.IntResult > 0)
                {
                    LogHandler.WriteServiceLog(usid, "Id:" + id, "成功", "删除", "图层范围");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = r.Message;
                    LogHandler.WriteServiceLog(usid, "Id" + id + "," + ErrorCol, "失败", "删除", "图层范围");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail));
            }


        }
        #endregion

    }
}
