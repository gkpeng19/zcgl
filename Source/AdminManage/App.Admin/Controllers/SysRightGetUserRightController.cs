﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Common;
using Microsoft.Practices.Unity;
 
using App.Models.Sys;
using App.Models;


namespace App.Admin.Controllers
{
    public class SysRightGetUserRightController : Controller
    {

        [SupportFilter]
        public ActionResult Index()
        {
            return View();
        }

        //获取用户列表
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetUserList(GridPager pager,string queryStr)
        {
            List<SysUserModel> list = sysUserBLL.GetList(ref pager, queryStr);
            var json = new
            {
                total = pager.totalRows,
                rows = (from r in list
                        select new SysUserModel()
                        {

                            Id = r.Id,
                            UserName = r.UserName,
                            TrueName = r.TrueName,
                            MobileNumber = r.MobileNumber
                        }).ToArray()

            };

            return Json(json);
        }


        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetUserRight(GridPager pager, string userId)
        {
         
            if (userId == null)
                return Json(0);

            var userRightList = sysRightGetUserRightBLL.GetList(userId);
            List<P_Sys_GetRightByUser_Result> list = userRightList.Skip((pager.page - 1) * pager.rows).Take(pager.rows).ToList();
            int totalRecords = userRightList.Count();
            var json = new
            {
                total = totalRecords,
                rows = (from r in list
                        select new SysRightUserRight()
                        {

                            ModuleId = r.moduleId,
                            ModuleName = r.moduleName,
                            KeyCode = r.keyCode,
 

                        }).ToArray()

            };
            return Json(json);
        }

    }
}
