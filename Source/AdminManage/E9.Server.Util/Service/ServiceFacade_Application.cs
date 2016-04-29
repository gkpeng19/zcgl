using NM.Service;
using NM.Util;
using NM.Model;
using System;
using System.IO;
using NM.Config;
using System.Text;
using NM.Log;
using System.Collections.Generic;

namespace NM.OP
{
    [ServiceFacadeAttribute("System")]
    public class ServiceFacade_Application : ServiceFacadeBase    
    {  
        [Service("LoadAppSetting")]
        public void LoadAppSetting(ServiceContext context)
        {
            context.E9_Response.Value = ToJson(new ApplicationOP(context.E9_Request.LogIn).LoadAppSetting());
        }

        [Service("GetAutoNo")]
        public void GetAutoNo(ServiceContext context)
        {
            context.E9_Response.Value = ToJson(new ApplicationOP(context.E9_Request.LogIn).GetAutoNo(context.E9_Request[0]));
        }

        [Service("Diagnose")]
        public void Diagnose(ServiceContext context)
        {
            context.E9_Response.Value = ToJson(new ApplicationOP(context.E9_Request.LogIn).Diagnose(context));
        }

        [Service("GetCacheSummary")]
        public void GetCacheSummary(ServiceContext context)
        {
            context.E9_Response.Value = ToJson(new ApplicationOP(context.E9_Request.LogIn).GetCacheSummary());
        }

        [Service("ClearCache")]
        public void ClearCache(ServiceContext context)
        {
            context.E9_Response.Value = ToJson(new ApplicationOP(context.E9_Request.LogIn).ClearCache());
        }

        [Service("ExportLicence")]
        public void ExportLicence(ServiceContext context)
        {
            context.E9_Response.Value = ToJson(new ApplicationOP(context.E9_Request.LogIn).ExportLicence());
        }

        [Service("GetAllService")]
        public void GetAllService(ServiceContext context)
        {
            context.E9_Response.Value = ToJson(new ApplicationOP(context.E9_Request.LogIn).GetAllService());
        }

        [Service("GetAllEntity")]
        public void GetAllEntity(ServiceContext context)
        {
            context.E9_Response.Value = ToJson(new ApplicationOP(context.E9_Request.LogIn).GetAllEntity());
        }

        #region 设置为当前使用帐套
        [Service("SetCurrAccountSet")]
        public void SetCurrAccountSet(DataRequest request, DataResponse result, DataProvider datasource)
        {
            WMS_ACCOUNTSET obj = TJson.Parse<WMS_ACCOUNTSET>(request["P0"]);
            result.Value = ToJson(new ApplicationOP(request.LogIn).SetCurrAccountSet(obj));
        }
        #endregion

        #region 备份帐套
        [Service("ExportExcelAccountSet")]
        public void ExportExcelAccountSet(DataRequest request, DataResponse result, DataProvider datasource)
        {
            string AccountYear = request[0];
            int Orgid = int.Parse(request[1]);
            //int IsOrder = int.Parse(request[2]);
            result.Value = ToJson(new ApplicationOP(request.LogIn).ExportExcelAccountSet(AccountYear, Orgid));
        }
        #endregion

        [Service("DirecExportData")]
        public void DirecExportData(DataRequest request, DataResponse result, DataProvider datasource)
        {
            result.Value = ToJson(new ApplicationOP(request.LogIn).DirecExportData(request[0].ToString(), int.Parse(request[1]), Convert.ToBoolean(request[2])));
        }

        [Service("ImportDataToTempTable")]
        public void ImportDataToTempTable(DataRequest request, DataResponse result, DataProvider datasource)
        {
            result.Value = ToJson(new ApplicationOP(request.LogIn).ImportDataToTempTable(request[0]));
        }

        [Service("RecoverAccountset")]
        public void RecoverAccountset(DataRequest request, DataResponse result, DataProvider datasource)
        {
            result.Value = ToJson(new ApplicationOP(request.LogIn).RecoverAccountset(Convert.ToInt32(request[0])));
        }

        [Service("DelAccountSet")]
        public void DelAccountSet(DataRequest request, DataResponse result, DataProvider datasource)
        {
            result.Value = ToJson(new ApplicationOP(request.LogIn).DelAccountSet(Convert.ToInt32(request[0])));
        }
    }
}
