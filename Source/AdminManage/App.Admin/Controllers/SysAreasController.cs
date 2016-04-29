
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
 
using App.Common;
using App.Models;
using App.Models.Sys;

using System.Text;
using NM.Model;
using Gis.Models;
using NM.OP;
namespace App.Admin.Controllers
{
    public class SysAreasController : BaseController
    {
        ValidationErrors errors = new ValidationErrors();


        [SupportFilter]
        public ActionResult Index()
        {
            ViewBag.Perm = GetPermission();
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName="Index")]
        public JsonResult GetList(string id)
        {
            if (id == null)
                id = "0";
            EntityProviderOP<EAP_Area2> _op = new EntityProviderOP<EAP_Area2>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("EAP_AREA");
            _search["PCODE"] = id;

            SearchResult<EAP_Area2> _rs = _op.Search(_search);

            foreach (EAP_Area2 _o in _rs.Items)
            {
                _o.state = (_o.HasChild_G > 0) ? "closed" : "open";
            }
            //List<SysAreasModel> list = null;// m_BLL.GetList(id);
            //var json = from r in list
            //           select new SysAreasModel()
            //           {
            //               Id = r.Id,
            //            Name = r.Name,
            //            ParentId = r.ParentId,
            //            Sort = r.Sort,
            //            Enable = r.Enable,
            //            IsMunicipality = r.IsMunicipality,
            //            IsHKMT = r.IsHKMT,
            //            IsOther = r.IsOther,
            //               CreateTime = r.CreateTime,
            //              // state = (m_BLL.GetList(r.Id).Count > 0) ? "closed" : "open"
            //           };


            return Json(_rs.Items.ToArray());
        }

        #region 创建
        [SupportFilter]
        public ActionResult Create(string id)
        {
            ViewBag.Perm = GetPermission();
            SysAreasModel entity = new SysAreasModel()
            {
                ParentId = id,
                Enable = true
            };
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(SysAreasModel model)
        {
            model.Id = ResultHelper.NewId;
            model.CreateTime = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {

                if (true)// (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserName(), "Id" + model.Id + ",Name" + model.Name, "成功", "创建", "SysAreas");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserName(), "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "创建", "SysAreas");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.InsertFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.InsertFail));
            }
        }
        #endregion

        #region 修改
        [SupportFilter]
        public ActionResult Edit(string id)
        {
            ViewBag.Perm = GetPermission();
            SysAreasModel entity = null;// m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(SysAreasModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (true)// (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserName(), "Id" + model.Id + ",Name" + model.Name, "成功", "修改", "SysAreas");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserName(), "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "修改", "SysAreas");
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

        public ActionResult Details(string id)
        {
            //获取父级
            List<SysAreasModel> list = null;// m_BLL.GetList("0");

            foreach (var model in list)
            {
                model.clildren = null;// m_BLL.GetList(model.Id);
                foreach (var m in model.clildren)
                {
                    m.clildren = null;// m_BLL.GetList(m.Id);
                }
            }

            return View(list);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public JsonResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                EAP_Area o = new EAP_Area() { ID = int.Parse(id) };
                CommandResult r = DeleteObj<EAP_Area>(o);
                if (r.IntResult > 0)//  (userBLL.Delete(ref errors, id))
               // if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserName(), "Id:" + id, "成功", "删除", "SysAreas");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserName(), "Id" + id + "," + ErrorCol, "失败", "删除", "SysAreas");
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
