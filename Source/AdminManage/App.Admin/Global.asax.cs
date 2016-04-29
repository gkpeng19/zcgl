using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using App.BLL.Core;
using App.Common;
using App.Core;
using Microsoft.Practices.Unity;
using Quartz;
using Quartz.Impl;

namespace App.Admin
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {           
            AreaRegistration.RegisterAllAreas();

           // WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleTable.EnableOptimizations = true;//开启文件压缩功能
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
      

            //注入 Ioc
          //  var container = new UnityContainer();
            //lxt  定义的业务操作和数据库操作的接口和实现，
            //用于反转依赖，对数据库的操作，需要考虑引入eap，支持oracle
            //DependencyRegisterType.Container_Sys(ref container);
            //DependencyResolver.SetResolver(new UnityDependencyResolver(container));

        }
   
        /// <summary>
        /// 全局的异常处理
        /// </summary>
        public void ExceptionHandlerStarter()
        {
            App.Models.Sys.siteconfig siteConfig = new App.BLL.SysConfigBLL().loadConfig(Utils.GetXmlMapPath("Configpath"));
            if (siteConfig.globalexceptionstatus == 1)
            {
                string s = HttpContext.Current.Request.Url.ToString();
                HttpServerUtility server = HttpContext.Current.Server;
                if (server.GetLastError() != null)
                {
                    Exception lastError = server.GetLastError();
                    // 此处进行异常记录，可以记录到数据库或文本，也可以使用其他日志记录组件。
                    ExceptionHander.WriteException(lastError);
                    Application["LastError"] = lastError;
                    int statusCode = HttpContext.Current.Response.StatusCode;
                    string exceptionOperator = siteConfig.globalexceptionurl;
                    try
                    {
                        if (!String.IsNullOrEmpty(exceptionOperator))
                        {
                            exceptionOperator = new System.Web.UI.Control().ResolveUrl(exceptionOperator);
                            string url = string.Format("{0}?ErrorUrl={1}", exceptionOperator, server.UrlEncode(s));
                            string script = String.Format("<script language='javascript' type='text/javascript'>window.top.location='{0}';</script>", url);
                            Response.Write(script);
                            Response.End();
                        }
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// 全局的异常处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            #if DEBUG
                        //调试状态不进行异常跟踪
            #else
                   ExceptionHandlerStarter();
            #endif
        }

    }
}