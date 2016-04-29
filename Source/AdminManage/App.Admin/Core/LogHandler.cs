using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.Common;
 
using App.Models;
using Microsoft.Practices.Unity;
using NM.OP;
using NM.Model;

namespace App.Admin
{
    public static class LogHandler
    {
        //[Dependency]
        //public static ISysLogBLL logBLL { get; set; }
 /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="oper">操作人</param>
        /// <param name="mes">操作信息</param>
        /// <param name="result">结果</param>
        /// <param name="type">类型</param>
        /// <param name="module">操作模块</param>
        public static void WriteServiceLog(string oper, string mes, string result, string type, string module)
        {
            App.Models.Sys.siteconfig siteConfig =  new App.BLL.SysConfigBLL().loadConfig(Utils.GetXmlMapPath("Configpath"));
            //后台管理日志开启
            if (siteConfig.logstatus == 1)
            {
                APP_Log o = new APP_Log();
                o.Message = mes;
                o.LOGTYPE = type;
                o.ModuleName = module;
                o.OPERATOR = oper;
                o.LOGRESULT = result;
                EntityProviderOP<APP_Log> _op = new EntityProviderOP<APP_Log>(null, DataProvider.GetEAP_Provider());
                int k = _op.Save(o);
               // UtilOP op=new UtilOP()
                //ValidationErrors errors = new ValidationErrors();
                //SysLog entity = new SysLog();
                //entity.Id = ResultHelper.NewId;
                //entity.Operator = oper;
                //entity.Message = mes;
                //entity.Result = result;
                //entity.Type = type;
                //entity.Module = module;
                //entity.CreateTime = ResultHelper.NowTime;
                //using (SysLogRepository logRepository = new SysLogRepository())
                //{
                //    logRepository.Create(entity);
                //}
            }
            else
            {
                return;
            }
        }
    }
}