using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
using System.Web.Caching;
using App.Common;

namespace App.BLL
{
    public partial class SysConfigBLL
    {
        private readonly App.DAL.SysConfigRepository dal = new App.DAL.SysConfigRepository();

        /// <summary>
        ///  读取配置文件
        /// </summary>
        public App.Models.Sys.siteconfig loadConfig(string configFilePath)
        {
            App.Models.Sys.siteconfig model = CacheHelper.Get<App.Models.Sys.siteconfig>(ContextKeys.CACHE_SITE_CONFIG);
            if (model == null)
            {
                CacheHelper.Insert(ContextKeys.CACHE_SITE_CONFIG, dal.loadConfig(configFilePath), configFilePath);
                model = CacheHelper.Get<App.Models.Sys.siteconfig>(ContextKeys.CACHE_SITE_CONFIG);
            }
            return model;
        }
        /// <summary>
        /// 读取客户端站点配置信息
        /// </summary>
        public App.Models.Sys.siteconfig loadConfig(string configFilePath, bool isClient)
        {
            App.Models.Sys.siteconfig model = CacheHelper.Get<App.Models.Sys.siteconfig>(ContextKeys.CACHE_SITE_CONFIG_CLIENT);
            if (model == null)
            {
                model = dal.loadConfig(configFilePath);
                model.templateskin = model.webpath + "templates/" + model.templateskin;
                CacheHelper.Insert(ContextKeys.CACHE_SITE_CONFIG_CLIENT, model, configFilePath);
            }
            return model;
        }

        /// <summary>
        ///  保存配置文件
        /// </summary>
        public App.Models.Sys.siteconfig saveConifg(App.Models.Sys.siteconfig model, string configFilePath)
        {
            return dal.saveConifg(model, configFilePath);
        }

    }
}
