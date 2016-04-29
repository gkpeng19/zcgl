
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using App.Common;
using App.Models;
using App.Models.Sys;

using System.Text;
using NM.OP;
using NM.Model;
using NM.Util;
using Gis.Models;
using System;
using App.Admin.Core;

namespace App.Admin.Controllers
{
    public class SysStructController : BaseController
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
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetGridList(string  id)
        {
            if (id == null)
                id = "0";

            EntityProviderOP<EAP_Org> _op = new EntityProviderOP<EAP_Org>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria  _search=new SearchCriteria ("EAP_ORG");
            _search["parentid"]=id ;

            SearchResult<EAP_Org> _rs = _op.Search(_search);                
             
           // var  list = m_BLL.GetListByParentId(id);             
          
              var json =new List<SysStructModel>();
               foreach (EAP_Org r in _rs.Items)   
               {
                   SysStructModel _m=new SysStructModel()
                       {
                           Id = r.ID.ToString(),
                           Code=r.Code ,
                           Name = r.Name,
                           ParentId = r.ParentID.ToString(),
                           Sort = r.SortBy,
                           Enable = (r.Status == 1),
                           Remark = r.Remark,
                           CreateTime = r.AddOn,
                           state = (r.HasChild_G==1) ? "closed" : "open"
                       };
                   json.Add(_m);
               }            

            return Json(json);
        
        }
        public JsonResult GetTreeStruct()
        {
            EntityProviderOP<EAP_Org> _op = new EntityProviderOP<EAP_Org>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("EAP_ORG");          

            SearchResult<EAP_Org> _rs = _op.Search(_search);


            List<TreeBase> _ls = BuildTree<EAP_Org>(_rs.Items);
  
            return  Json(_ls, JsonRequestBehavior.AllowGet);;
        }

        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(GridPager pager, string nodeid, int? n_level)
        {
            object o = null;// m_BLL.GetParentStruct(pager, nodeid, n_level);
            return Json(o, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取下拉列表的集合
        /// </summary>
        /// <param name="id">父ID</param>
        /// <returns></returns>
        public JsonResult GetListByParentId(string id)
        {
            var list = new List<EAP_Org>();// m_BLL.GetListByParentId(id);
            StringBuilder sb = new StringBuilder();
            foreach (var l in list)
            {
                sb.AppendFormat("<option value='{0}'>{1}</option>", l.ID, l.Name);
            }
            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }
        #region 创建
        [SupportFilter]
        public ActionResult Create(string ParentId)
        {
            ViewBag.Title = "创建";
            ViewBag.Perm = GetPermission();
            //EAP_Org _m = new EAP_Org();
            //_m.S = EntityStatus.New;
            //_m.ParentID = int.Parse(ParentId);
            //_m.SortBy = 0;

            SysStructModel model = new SysStructModel()
            {
                Code = ResultHelper.NewId,
                ParentId = ParentId,
                Sort = 0,
                Higher = ParentId,
                objstate = 0

            };
            return View(model);
        }

 

        [SupportFilter]
        public ActionResult Edit(string Id)
        {
            ViewBag.Title = "修改";
            ViewBag.Perm = GetPermission();
            SysStructModel _m = null;
            EAP_Org r = GetObjByID<EAP_Org>(Id, "EAP_Org");           

            if (r!=null)
            {
               

                _m = new SysStructModel()
                {
                    Id = r.ID.ToString(),
                    Code = r.Code,
                    Name = r.Name,
                    ParentId = r.ParentID.ToString(),
                    Sort = r.SortBy,
                    Enable=( r.Status==1),
                    //= r.State,
                    Remark = r.Remark,
                    CreateTime = r.AddOn,
                    state = (r.HasChild_G == 1) ? "closed" : "open"
                };
              }

            return View("Create",_m);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(SysStructModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                EAP_Org _m = null;
                if (!string.IsNullOrEmpty(model.Id))
                {
                    _m = GetObjByID<EAP_Org>(model.Id, "EAP_Org");   
                }
                else
                {
                    _m = new EAP_Org();
                }

                LoginInfo _login = LoginUser;
                _m.ParentID = int.Parse(model.ParentId);

                 _m.Name = model.Name ;
                _m.Code = model.Code;
                _m.AddBy = _login.User.TrueName;
                _m.Status = model.Enable ? 1 : 0;

                _m.Remark = model.Remark;

                CommandResult r = SaveObj<EAP_Org>(_m);             

                if (r.IntResult > 0)
                {
                    LogHandler.WriteServiceLog(_login.User.UserID.ToString(), "Id:" + model.Id + ",Name:" + model.Name, "成功", "保存", "组织结构");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.Save), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = r.Message;
                    LogHandler.WriteServiceLog(_login.User.UserID.ToString(), "Id:" + model.Id + ",Name:" + model.Name + "," + ErrorCol, "失败", "保存", "组织结构");
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
        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(SysStructModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                return null;
                //if (m_BLL.Edit(ref errors, model))
                //{
                //    LogHandler.WriteServiceLog(GetUserId(), "Id:" + model.Id + ",Name:" + model.Name, "成功", "修改", "组织结构");
                //    return Json(JsonHandler.CreateMessage(1, Suggestion.EditSucceed), JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    string ErrorCol = errors.Error;
                //    LogHandler.WriteServiceLog(GetUserId(), "Id:" + model.Id + ",Name:" + model.Name + "," + ErrorCol, "失败", "修改", "组织结构");
                //    return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail + ErrorCol), JsonRequestBehavior.AllowGet);
                //}
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 详细
        //[SupportFilter]
        //public ActionResult Details(string id)
        //{
        //    ViewBag.Perm = GetPermission();
        //    SysStruct entity = m_BLL.GetById(id);
        //    SysStructModel info = new SysStructModel()
        //    {
        //        Id = entity.Id,
        //        Name = entity.Name,
        //        ParentId = entity.ParentId,
        //        Sort = entity.Sort,
        //        Enable = entity.State,
        //        CreateTime = entity.CreateTime,

        //    };
        //    return View(info);
        //}

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public JsonResult Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {             
                LoginInfo _login = LoginUser;

                EAP_Org m=new EAP_Org (){ID=int.Parse(id),S=EntityStatus.Delete };
                CommandResult  r=    DeleteObj<EAP_Org>(m);

                if (r.IntResult>0)
                {
                    LogHandler.WriteServiceLog(_login.User.UserID.ToString(), "Ids:" + id, "成功", "删除", "体系结构");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.DeleteSucceed), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = r.Message;
                    LogHandler.WriteServiceLog(_login.User.UserID.ToString(), "Id:" + id + "," + ErrorCol, "失败", "删除", "体系结构");
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

