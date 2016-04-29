using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using App.Common;
using App.Models;
using App.Models.Sys;
using NM.OP;
using NM.Model;
using System;

namespace App.Admin.Controllers
{
    public class SysModuleController : BaseController
    {
        ValidationErrors errors = new ValidationErrors();

        /// <summary>
        /// 主页
        /// </summary>
        /// <returns>视图</returns>
        [SupportFilter]
        public ActionResult Index()
        {
            ViewBag.Perm = GetPermission();
            return View();

        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="pager">分页</param>
        /// <param name="queryStr">查询条件</param>
        /// <returns></returns>
        [SupportFilter(ActionName = "Index")]
        [HttpPost]
        public JsonResult GetList(string id)
        {

            if (id == null)
                id = "0";


            EntityProviderOP<EAP_Resource> _op = new EntityProviderOP<EAP_Resource>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("EAP_MOUDLE");
            _search["parentid"] = id;

            SearchResult<EAP_Resource> _rs = _op.Search(_search);

            // var  list = m_BLL.GetListByParentId(id);             

            var json = new List<SysModuleModel>();
            foreach (EAP_Resource r in _rs.Items)
            {
                SysModuleModel _m = new SysModuleModel()
                {
                    Id = r.ID.ToString(),
                    Url = r.PageId                    ,
                    Name = r.Name,
                    ParentId = r.ParentId.ToString(),
                    Sort = r.SortBy,
                    Enable = !r.Flag_Delete,
                    Remark=r.Description,
                    Iconic=r.Image ,
                    CreateTime = r.AddOn,
                    IsLast=(r.HasChild_G == 0),
                    state = (r.HasChild_G == 1) ? "closed" : "open"
                };
                json.Add(_m);
            }

            return Json(json);
            /*
            if (id == null)
                id = "0";
            List<SysModuleModel> list = m_BLL.GetList(id);
            var json = from r in list
                       select new SysModuleModel()
                       {
                           Id = r.Id,
                           Name = r.Name,
                           EnglishName = r.EnglishName,
                           ParentId = r.ParentId,
                           Url = r.Url,
                           Iconic = r.Iconic,
                           Sort = r.Sort,
                           Remark = r.Remark,
                           Enable = r.Enable,
                           CreatePerson = r.CreatePerson,
                           CreateTime = r.CreateTime,
                           IsLast = r.IsLast,
                           state = (m_BLL.GetList(r.Id).Count > 0) ? "closed" : "open"
                       };


            return Json(json);
            */
        }

        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetOptListByModule(GridPager pager, string mid)
        {
            if (string.IsNullOrEmpty(mid))
                mid = "0";


            EntityProviderOP<EAP_Resource> _op = new EntityProviderOP<EAP_Resource>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("EAP_RESOURCEOP");
            _search["parentid"] = mid;

            SearchResult<EAP_Resource> _rs = _op.Search(_search);

            List<SysModuleOperateModel> ls = new List<SysModuleOperateModel>();
            foreach (EAP_Resource r in _rs.Items)
            {
                SysModuleOperateModel _m = new SysModuleOperateModel()
                {
                    Id = r.ID.ToString(),
                    KeyCode = r.PageId,
                    Name = r.Name,
                    ModuleId = r.ParentId.ToString(),
                    Sort = r.SortBy,
                    IsValid = !r.Flag_Delete 
                     
                };
                ls.Add(_m);
            }
          
           
            var json = new
            {
                total = ls.Count ,
                rows = ls.ToArray()

            };

            return Json(json);
        }
            

        #region 创建模块
        [SupportFilter]
        public ActionResult Create(string id)
        {
            ViewBag.Perm = GetPermission();
            SysModuleModel entity = new SysModuleModel()
            {
                ParentId = id,
                Enable = true,
                Sort = 0
            };
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(SysModuleModel model)
        {
            model.Id = ResultHelper.NewId;
            model.CreateTime = ResultHelper.NowTime;
            model.CreatePerson = "aa"; // GetUserId();
            if (model != null && ModelState.IsValid)
            {
                EAP_Resource _m =  new EAP_Resource();              
                LoginInfo _login = LoginUser;
                _m.ParentId = int.Parse(model.ParentId);

                _m.Name = model.Name;
                _m.PageId = model.Url;
                _m.AddBy = _login.User.TrueName;
                _m.Flag_Delete = !model.Enable  ;
                _m.Type = "menu";
                _m.Description = model.Remark;
                _m.VRow = model.isnewWin ? 0 : 1; 
                _m.SortBy = model.Sort == null ? 0 : (int)(model.Sort);
                _m.Image = model.Iconic;
                CommandResult r = SaveObj<EAP_Resource>(_m);

                if (r.IntResult > 0)
                // if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(_login.User.UserName, "Id" + model.Id + ",Name" + model.Name, "成功", "创建", "系统菜单");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.InsertSucceed));
                }
                else
                {
                    string ErrorCol = r.Message ;
                    LogHandler.WriteServiceLog(_login.User.UserName, "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "创建", "系统菜单");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.InsertFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.InsertFail));
            }
        }
        #endregion


        #region 创建
        [SupportFilter(ActionName = "Create")]
        public ActionResult CreateOpt(string moduleId)
        {
            ViewBag.Perm = GetPermission();
            SysModuleOperateModel sysModuleOptModel = new SysModuleOperateModel();
            sysModuleOptModel.ModuleId = moduleId;
            sysModuleOptModel.IsValid = true;
            return View(sysModuleOptModel);
        }


        [HttpPost]
        [SupportFilter(ActionName = "Create")]
        public JsonResult CreateOpt(SysModuleOperateModel info)
        {
            if (info != null && ModelState.IsValid)
            {
                EAP_Resource _m =null;
                if (!string.IsNullOrEmpty(info.Id))
                {
                    _m= GetObjByID<EAP_Resource>(info.Id, "EAP_RESOURCEOP");// operateBLL.GetById(info.Id);
                }
               
                if (_m != null)
                    return Json(JsonHandler.CreateMessage(0, Suggestion.PrimaryRepeat), JsonRequestBehavior.AllowGet);

                _m = new EAP_Resource();

                LoginInfo _login = LoginUser;
                _m.ParentId = int.Parse(info.ModuleId);

                _m.Name = info.Name;
                _m.PageId = info.KeyCode;
                _m.AddBy = _login.User.TrueName;
                _m.SortBy = info.Sort;
                _m.Type = "op";
               

                CommandResult r = SaveObj<EAP_Resource>(_m);

                if (r.IntResult > 0)            

               // if (operateBLL.Create(ref errors, entity))
                {
                    LogHandler.WriteServiceLog(_login.User.UserName, "Id:" + info.Id + ",Name:" + info.Name, "成功", "创建", "操作设置");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.InsertSucceed), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(_login.User.UserName, "Id:" + info.Id + ",Name:" + info.Name + "," + ErrorCol, "失败", "创建", "操作设置");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.InsertFail + ErrorCol), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.InsertFail), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 修改模块
        [SupportFilter]
        public ActionResult Edit(string id)
        {
            ViewBag.Perm = GetPermission();
            SysModuleModel _m = null;
            EAP_Resource r = GetObjByID<EAP_Resource>(id, "EAP_MOUDLE");// operateBLL.GetById(info.Id);
            if (r != null)
            {
                _m = new SysModuleModel()
                {
                    Id = r.ID.ToString(),
                    Url = r.PageId,
                    Name = r.Name,
                    ParentId = r.ParentId.ToString(),
                    Sort = r.SortBy,
                    Enable = !r.Flag_Delete,
                    CreateTime = r.AddOn,
                    isnewWin=(r.VRow==0) ,
                    Iconic=r.Image ,
                    Remark=r.Description ,
                    IsLast= (r.HasChild_G == 0),
                    state = (r.HasChild_G == 1) ? "closed" : "open"
                };
            }

            return View(_m);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(SysModuleModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                EAP_Resource _m = GetObjByID<EAP_Resource>(model.Id, "EAP_MOUDLE");


                LoginInfo _login = LoginUser;
                _m.ParentId = int.Parse(model.ParentId);

                _m.Name = model.Name;
                _m.PageId = model.Url;
                _m.AddBy = _login.User.TrueName;
                _m.Flag_Delete = !model.Enable;
                _m.Description = model.Remark;
                _m.EditBy = LoginUser.User.UserName;
                _m.EditOn = DateTime.Now;
                _m.VRow = model.isnewWin ? 0 : 1; 
                _m.SortBy = model.Sort==null?0:(int)(model.Sort);
                _m.Description = model.Remark;
                _m.Image = model.Iconic;
                CommandResult r = SaveObj<EAP_Resource>(_m);

                if (r.IntResult > 0)
               // if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(_login.User.UserName, "Id" + model.Id + ",Name" + model.Name, "成功", "修改", "系统菜单");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.EditSucceed));
                }
                else
                {
                    string ErrorCol = r.Message ;
                    LogHandler.WriteServiceLog(_login.User.UserName, "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "修改", "系统菜单");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail));
            }
        }
        #endregion

     

        #region 删除
        [HttpPost]
        [SupportFilter]
        public JsonResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                LoginInfo _login = LoginUser;
                EAP_Resource m = new EAP_Resource() { ID = int.Parse(id) };
                CommandResult r = DeleteObj<EAP_Resource>(m);
                if (r.IntResult>0)
                //if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(_login.User.UserName, "Ids:" + id, "成功", "删除", "系统菜单");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.DeleteSucceed), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(_login.User.UserName, "Id:" + id + "," + ErrorCol, "失败", "删除", "系统菜单");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail + ErrorCol), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail), JsonRequestBehavior.AllowGet);
            }


        }


        [HttpPost]
        [SupportFilter(ActionName = "Delete")]
        public JsonResult DeleteOpt(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                LoginInfo _login = LoginUser;
                EAP_Resource m = new EAP_Resource() { ID = int.Parse(id) };
                CommandResult r = DeleteObj<EAP_Resource>(m);
                if (r.IntResult > 0)
                //if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(_login.User.UserName, "Ids:" + id, "成功", "删除", "操作设置");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.DeleteSucceed), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(_login.User.UserName, "Id:" + id + "," + ErrorCol, "失败", "删除", "操作设置");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail + ErrorCol), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail), JsonRequestBehavior.AllowGet);
            }


        }

        #endregion
    }
}

