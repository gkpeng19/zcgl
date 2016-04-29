using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Admin.Controllers
{
    public class SysSampleByUpLoadController : BaseController
    {
        //
        // GET: /SysSampleByUpLoad/

        public ActionResult Index(string id)
        {
            ViewBag.Dif = id;
            return View();
        }



    }
}
