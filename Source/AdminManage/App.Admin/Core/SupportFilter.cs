﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using App.Models.Sys;
using App.BLL;
using App.DAL;
using NM.OP;
using NM.Model;
using NM.Util;

namespace App.Admin
{
    public class SupportFilterAttribute : ActionFilterAttribute
    {
        public string ActionName { get; set; }
        private string Area;
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

        }
        public LoginInfo getLoginUser(AccountModel account)
        {
            
                LoginInfo _eaplogininfo = null;
             
                AccountModel _login = account;
                if (_login != null)
                {
                    _eaplogininfo = new LoginInfo();
                    _eaplogininfo.ID = _login.UserId;
                    _eaplogininfo.User = new EAP_User() { ID = _login.UserId, UserID = _login.UserId, TrueName = _login.TrueName, OrgId = _login.OrgID };
                }
                return _eaplogininfo;
            
        }
        /// <summary>
        /// Action加上[SupportFilter]在执行actin之前执行以下代码，通过[SupportFilter(ActionName="Index")]指定参数
        /// </summary>
        /// <param name="filterContext">页面传过来的上下文</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //读取请求上下文中的Controller,Action,Id
            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);
            RouteData routeData = routes.GetRouteData(filterContext.HttpContext);
            //取出区域的控制器Action,id
            string ctlName = filterContext.Controller.ToString();
            string[] routeInfo = ctlName.Split('.');
            string controller = null;
            string action = null;
            string id = null;

            int iAreas = Array.IndexOf(routeInfo, "Areas");
            if (iAreas > 0)
            {
                //取区域及控制器
                Area = routeInfo[iAreas + 1];
            }
            int ctlIndex = Array.IndexOf(routeInfo, "Controllers");
            ctlIndex++;
            controller = routeInfo[ctlIndex].Replace("Controller", "").ToLower();

            string url = HttpContext.Current.Request.Url.ToString().ToLower();
            string[] urlArray = url.Split('/');
            int urlCtlIndex = Array.IndexOf(urlArray, controller);
            urlCtlIndex++;
            if (urlArray.Count() > urlCtlIndex)
            {
                action = urlArray[urlCtlIndex];
            }
            urlCtlIndex++;
            if (urlArray.Count() > urlCtlIndex)
            {
                id = urlArray[urlCtlIndex];
            }
            //url
            action = string.IsNullOrEmpty(action) ? "Index" : action;
            int actionIndex = action.IndexOf("?", 0);
            if (actionIndex > 1)
            {
                action = action.Substring(0, actionIndex);
            }
            id = string.IsNullOrEmpty(id) ? "" : id;

            //URL路径
            string filePath = HttpContext.Current.Request.FilePath;
            AccountModel account = filterContext.HttpContext.Session["Account"] as AccountModel;
            if (LoginUserManage.ValidateIsLogined(account) && ValiddatePermission(account, controller, action, filePath) && !LoginUserManage.ValidateRelogin(account))
            {
                //已经登录，有权限，且没有单机登录限制
                return;
            }
            else
            {
                filterContext.Result = new EmptyResult();
                return;
            }
        }
        public bool ValiddatePermission(AccountModel account, string controller, string action, string filePath)
        {
            bool bResult = false;
            string actionName = string.IsNullOrEmpty(ActionName) ? action : ActionName;
            if (account != null)
            {
                List<permModel> permlist = null;
                //测试当前controller是否已赋权限值，如果没有从
                //如果存在区域,Seesion保存（区域+控制器）
                if (!string.IsNullOrEmpty(Area))
                {
                    controller = Area + "/" + controller;
                }
                if  (HttpContext.Current.Session[filePath]==null)
                {
                   //获取当前用户的操作权限列表

                    SourceOP _SourceOP = new SourceOP(getLoginUser(account));
                    TJsonList<EAP_Resource> menus = _SourceOP.GetMenuOperate(controller);
                    permlist=new List<permModel>();
                    foreach (EAP_Resource m in menus)
                    {
                       if (m.Type=="op")
                       {
                           permModel p=new  permModel()  {  KeyCode=m.PageId, IsValid=m.Flag_Delete };
                           permlist.Add(p);
                       }
                    }
                     HttpContext.Current.Session[filePath] = permlist;//获取的权限放入会话由Controller调用
                    
                }
                else
                {
                    permlist = (List<permModel>)HttpContext.Current.Session[filePath];
                }
                
               
 
                //当用户访问index时，只要权限>0就可以访问
                if (actionName.ToLower() == "index")
                {
                    if (permlist.Count > 0)
                    {
                        return true;
                    }
                }
                //查询当前Action 是否有操作权限，大于0表示有，否则没有
                int count = permlist.Where(a => a.KeyCode.ToLower() == actionName.ToLower()).Count();
                if (count > 0)
                {
                    bResult = true;
                }
                else
                {
                    bResult = false;
                    LoginUserManage.WritePage("你没有操作权限，请联系管理员！", "");
                }

            }
            return bResult;
        }
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }
    }


}