using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using App.Common;
using App.Models;
using Microsoft.Practices.Unity;
 
using App.Models.Sys;
using NM.OP;
using NM.Model;

namespace App.Admin.Controllers
{
    public class SysLogController : BaseController
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

            EntityProviderOP<APP_Log> _op = new EntityProviderOP<APP_Log>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("APP_LOG");
            ToSearchCriteria(pager, ref _search);
            _search["ModuleName"] = queryStr;

            SearchResult<APP_Log> _rs = _op.Search(_search);


            var json = new
            {
                total = _rs.TotalCount,
                rows = _rs.Items.ToArray()

            }; 
          
            return Json(json);
        }

         [SupportFilter(ActionName = "Index")]
        public ActionResult LoginIndex()
        {
            ViewBag.Perm = GetPermission();
            return View();

        }
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetLoginList(GridPager pager, string queryStr)
        {

            EntityProviderOP<APP_Login> _op = new EntityProviderOP<APP_Login>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("APP_LOGIN");
            ToSearchCriteria(pager, ref _search);
            _search["USERNAME"] = queryStr;

            SearchResult<APP_Login> _rs = _op.Search(_search);


            var json = new
            {
                total = _rs.TotalCount,
                rows = _rs.Items.ToArray()

            }; 
          
            return Json(json);
        }

        #region 详细
        [SupportFilter]
        public ActionResult Details(string id)
        {
            ViewBag.Perm = GetPermission();
            SysLog entity = null;// logBLL.GetById(id);
            SysLogModel info = new SysLogModel()
            {
                Id = entity.Id,
                Operator = entity.Operator,
                Message = entity.Message,
                Result = entity.Result,
                Type = entity.Type,
                Module = entity.Module,
                CreateTime = entity.CreateTime,
            };
            return View(info);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public JsonResult Delete(string ids)
        {
            if (!string.IsNullOrWhiteSpace(ids))
            {
                APP_Log o = new APP_Log();
                o.ID = int.Parse(ids);
                CommandResult r = DeleteObj<APP_Log>(o);
              //  string[] deleteIds = ids.Split(',');
                if(r.IntResult>0)
                  
                //if (logBLL.Delete(ref errors, deleteIds))
                {
                    return Json(JsonHandler.CreateMessage(1, Suggestion.DeleteSucceed), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol =r.Message;
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
