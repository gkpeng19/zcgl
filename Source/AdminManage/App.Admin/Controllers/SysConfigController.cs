using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
 
using App.Common;
using App.Models.Sys;
using App.BLL;


namespace App.Admin.Controllers
{
    public class SysConfigController : BaseController
    {
        //
        // GET: /SysConfig/

        ValidationErrors errors = new ValidationErrors();

        [SupportFilter]
        public ActionResult Index()
        {
            ViewBag.Perm = GetPermission();
            BLL.SysConfigBLL bll = new BLL.SysConfigBLL();
            App.Models.Sys.siteconfig model = bll.loadConfig(Utils.GetXmlMapPath(ContextKeys.FILE_SITE_XML_CONFING));
            return View(model);
        }
        [HttpPost]
        [SupportFilter]
        [ValidateInput(false)]
        public JsonResult Edit(App.Models.Sys.siteconfig model)
        {
            BLL.SysConfigBLL bll = new BLL.SysConfigBLL();
            try
            {
                bll.saveConifg(model, Utils.GetXmlMapPath(ContextKeys.FILE_SITE_XML_CONFING));
                return Json(JsonHandler.CreateMessage(1, Suggestion.EditSucceed));
            }
            catch
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail));
            }
        }
    }
}
