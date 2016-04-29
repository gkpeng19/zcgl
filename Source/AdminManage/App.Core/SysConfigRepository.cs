using System;
using System.Collections.Generic;
using System.Text;
using App.Common;

namespace App.DAL
{
    /// <summary>
    /// 数据访问类:站点配置
    /// </summary>
    public partial class SysConfigRepository
    {
        private static object lockHelper = new object();

        /// <summary>
        ///  读取站点配置文件
        /// </summary>
        public App.Models.Sys.siteconfig loadConfig(string configFilePath)
        {
            return (App.Models.Sys.siteconfig)SerializationHelper.Load(typeof(App.Models.Sys.siteconfig), configFilePath);
        }

        /// <summary>
        /// 写入站点配置文件
        /// </summary>
        public App.Models.Sys.siteconfig saveConifg(App.Models.Sys.siteconfig model, string configFilePath)
        {
            lock (lockHelper)
            {
                SerializationHelper.Save(model, configFilePath);
            }
            return model;
        }

    }
}
