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
namespace App.Admin.Controllers
{
    public class GisGroupController : BaseController
    {
        //

        ValidationErrors errors = new ValidationErrors();

        [SupportFilter]
        public ActionResult Index()
        {
            ViewBag.Perm = GetPermission();
            return View();
        }

        [SupportFilter(ActionName="Index")]
        public JsonResult GetList(GridPager pager,string queryStr)
        {
            EntityProviderOP<gis_layerclass> _op = new EntityProviderOP<gis_layerclass>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("GIS_LAYERCLASS");
            ToSearchCriteria(pager, ref _search);
            _search["CLASSNAME"] = queryStr;

            SearchResult<gis_layerclass> _rs = _op.Search(_search);


            var json = new
            {
                total = _rs.TotalCount,
                rows = _rs.Items.ToArray()

            };

            return Json(json);
       

        }
         [SupportFilter(ActionName = "Index")]
        public JsonResult GetDetailList(GridPager pager, string classID)
        {
            EntityProviderOP<gis_LayerClassD> _op = new EntityProviderOP<gis_LayerClassD>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("GIS_LAYERCLASSD");
            ToSearchCriteria(pager, ref _search);
            _search["CLASSID"] = classID;

            SearchResult<gis_LayerClassD> _rs = _op.Search(_search);


            var json = new
            {
                total = _rs.TotalCount,
                rows = _rs.Items.ToArray()

            };

            return Json(json);


        }


        /// <summary>
        /// 添加图层
        /// </summary>
        /// <returns></returns>
       [SupportFilter(ActionName = "Index")]
        public ActionResult AddLayer(string ClassId)
        {
            ViewBag.Perm = GetPermission();
               ViewBag.ClassId=ClassId;
            return View();
        }

        [SupportFilter(ActionName = "Index")]
           public JsonResult GetLayerList(GridPager pager, string ClassId, string queryStr)
        {
            EntityProviderOP<gis_layerDefine> _op = new EntityProviderOP<gis_layerDefine>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("GIS_LAYERDEFINE");
            ToSearchCriteria(pager, ref _search);
            _search["LAYERNAME"] = queryStr;

            SearchResult<gis_layerDefine> _rs = _op.Search(_search);



            EntityProviderOP<gis_LayerClassD> _detailop = new EntityProviderOP<gis_LayerClassD>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria _Detailsearch = new SearchCriteria("GIS_LAYERCLASSD");
            _Detailsearch.PageSize = -1;
            _Detailsearch["CLASSID"] = ClassId;

            SearchResult<gis_LayerClassD> _Deatilrs = _detailop.Search(_Detailsearch);

            //设置图层的选中状态；
            if (_Deatilrs.Items.Count > 0)
            {
                foreach (gis_LayerClassD o in _Deatilrs.Items)
                {
                    gis_layerDefine obj = _rs.Items.Find((m) => { return m.ID == o.LAYERID; });
                    
                    if (obj!=null)
                    {
                        obj.IsCheck_G = (obj.ID == o.LAYERID);
                    }
                    
                
                    
                }
            
            }

            var json = new
            {
                total = _rs.TotalCount,
                rows = _rs.Items.ToArray()

            };

            return Json(json);


        }
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult DeleteLayer(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                string usid = LoginUser.User.UserName;
                gis_LayerClassD m = new gis_LayerClassD() { ID = int.Parse(id) };
                CommandResult r = DeleteObj<gis_LayerClassD>(m);
                // if (m_BLL.Delete(ref errors, id))
                if (r.IntResult > 0)
                {
                    LogHandler.WriteServiceLog(usid, "Id:" + id, "成功", "删除", "图层分类明细");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = r.Message;
                    LogHandler.WriteServiceLog(usid, "Id" + id + "," + ErrorCol, "失败", "删除", "图层分类明细");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail));
            }


        }    
        

        //保存
        [HttpPost]
        [SupportFilter(ActionName = "Save")]
        public ActionResult Updatelayer(string idschecks, string idsunchecks, string ClassID)
        {

            int _ClassID = 0;


            if (!string.IsNullOrEmpty(ClassID))
            {
                int.TryParse(ClassID, out _ClassID);
            }

           
            LoginInfo lu = LoginUser;
            List<SqlParameter> _params=new  List<SqlParameter>();
            {
                SqlParameter p = new SqlParameter() { ParameterName = "opuser", Value = lu.User.UserName };
                _params.Add(p);
            }
            {
               SqlParameter  p=new SqlParameter(){ParameterName="idschecks",Value=idschecks};
                _params.Add(p);
            }
             {
               SqlParameter  p=new SqlParameter(){ParameterName="idsunchecks",Value=idsunchecks};
                _params.Add(p);
            }
             {
                 SqlParameter p = new SqlParameter() { ParameterName = "cid", Value = _ClassID };
                _params.Add(p);
            }
            
            DataProvider dp = DataProvider.GetEAP_Provider();
             List<CommandResult> cr=  dp.LoadData<CommandResult>("usp_SaverlayerClassDetail", _params.ToArray());
            //  if  （!((string.IsNullOrEmpty(model.RoleID)  || string.IsNullOrEmpty(model.Id))）
             CommandResult m = new CommandResult() { IntResult = 0 };
            
             if (cr != null && cr.Count > 0)
             {
                return  Json(cr[0]);
             }
             else
             {
                 return Json(m);
             }
            
           // return sysRightBLL.UpdateRight(model);
        }


        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
            ViewBag.Perm = GetPermission();

            gis_layerclass m = new gis_layerclass();
            return View(m);
        }


        [HttpPost]
        [SupportFilter]
        public JsonResult Create(gis_layerclass model)
        {
            if (model != null && ModelState.IsValid)
            {
                LoginInfo _login = LoginUser;
                model.ADDBY = _login.User.UserName;
                CommandResult r = SaveObj<gis_layerclass>(model);
                if (r.IntResult > 0)
                {
                    LogHandler.WriteServiceLog(_login.User.UserID.ToString(), "Id:" + model.ID + ",Name:" + model.CLASSNAME, "成功", "保存", "图层分类");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.Save), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = r.Message;
                    LogHandler.WriteServiceLog(_login.User.UserID.ToString(), "Id:" + model.ID + ",Name:" + model.CLASSNAME + "," + ErrorCol, "失败", "保存", "图层分类");
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



       [SupportFilter(ActionName = "Index")]
        public ActionResult EditLayerOrder(string id)
        {
            ViewBag.Perm = GetPermission();
            gis_LayerClassD entity = GetObjByID<gis_LayerClassD>(id);
          
            //SysRoleModel entity = m_BLL.GetById(id);
            return View(entity);
        }
       [HttpPost]
       [SupportFilter(ActionName = "Index")]
       public JsonResult EditLayerOrder(gis_LayerClassD model)
       {
           if (model != null && ModelState.IsValid)
           {

               LoginInfo _login = LoginUser;

               gis_LayerClassD o = GetObjByID<gis_LayerClassD>(model.ID.ToString());
            
               o.ODERNUM = model.ODERNUM;            
               o.EDITBY = _login.User.UserName;
               o.EDITON = DateTime.Now;


               CommandResult r = SaveObj<gis_LayerClassD>(o);
               if (r.IntResult > 0)
               {
                   LogHandler.WriteServiceLog(GetUserName(), "Id" + model.ID + ",Name" + model.LayerName_G, "成功", "修改", "图层顺序");
                   return Json(JsonHandler.CreateMessage(1, Suggestion.EditSucceed));
               }
               else
               {
                   string ErrorCol = r.Message;
                   LogHandler.WriteServiceLog(GetUserName(), "Id" + model.ID + ",Name" + model.LayerName_G + "," + ErrorCol, "失败", "修改", "图层顺序");
                   return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail + ErrorCol));
               }
           }
           else
           {
               return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail));
           }
       }



        [SupportFilter]
        public ActionResult Edit(string id)
        {
            ViewBag.Perm = GetPermission();
            gis_layerclass entity = GetObjByID<gis_layerclass>(id);
          
            //SysRoleModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(gis_layerclass model)
        {
            if (model != null && ModelState.IsValid)
            {

                LoginInfo _login = LoginUser;

                gis_layerclass o = GetObjByID<gis_layerclass>(model.ID.ToString());
                o.CLASSNAME = model.CLASSNAME;
                o.CLASSCODE = model.CLASSCODE;
                o.CLASSIMG = model.CLASSIMG;
                o.ODERNUM = model.ODERNUM;
                o.FLAGDELETE = model.FLAGDELETE;
                
                o.EDITBY = _login.User.UserName;
                o.EDITON = DateTime.Now;


                CommandResult r = SaveObj<gis_layerclass>(o);
                if (r.IntResult > 0)
                {
                    LogHandler.WriteServiceLog(GetUserName(), "Id" + model.ID + ",Name" + model.CLASSNAME, "成功", "修改", "图层设置");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.EditSucceed));
                }
                else
                {
                    string ErrorCol = r.Message ;
                    LogHandler.WriteServiceLog(GetUserName(), "Id" + model.ID + ",Name" + model.CLASSNAME + "," + ErrorCol, "失败", "修改", "图层设置");
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
                gis_layerclass m = new gis_layerclass() { ID = int.Parse(id) };
                CommandResult r = DeleteObj<gis_layerclass>(m);
                // if (m_BLL.Delete(ref errors, id))
                if (r.IntResult >0)
                {
                    LogHandler.WriteServiceLog(usid, "Id:" + id, "成功", "删除", "图层分类");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = r.Message;
                    LogHandler.WriteServiceLog(usid, "Id" + id + "," + ErrorCol, "失败", "删除", "图层分类");
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
