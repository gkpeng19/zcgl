using System;
using System.Collections.Generic;
using NM.Util;
using NM.Log;
using NM.Model;
using System.Net;
using System.Data.SqlClient;
using System.Data;
using NM.Config;
using System.Configuration;
using NM.Lib;

namespace NM.OP
{
    public class AppSettingOP : NamedProviderOP
    {
        LoginInfo _LoginInfo;
        public AppSettingOP()
            : base(DataProvider.GetEAP_Provider())
        {

        }
        public AppSettingOP(LoginInfo LoginInfo)
            : base(DataProvider.GetEAP_Provider())
        {
            _LoginInfo = LoginInfo;
        }
        public AppSetting GetAppSetting()
        {
            int _orgid = 0;
            AppSetting result = new AppSetting();
            string strSQL = "select * from App_Config where LoadOnStart=1 and orgid=0";
            if (_LoginInfo != null) 
            {
                strSQL = "select * from App_Config where LoadOnStart=1 and ( orgid=0 or orgid="+_LoginInfo.User.OrgId+" )";
                _orgid = _LoginInfo.User.OrgId;
            }
            string _customer = "";
            string _reginfo = "";
            
            foreach (var config in base.DataProvider.LoadData<App_Config>(strSQL))
            {

                if (config.Name.ToUpper() == "CUSTOMER") 
                {
                    if ((_LoginInfo!=null) && (_LoginInfo.User!=null) &&  (config.OrgID == _LoginInfo.User.OrgId))
                    {
                        _customer = config.Value;
                        
                    }
                }
                else if (config.Name.ToUpper() == "LICENSE")
                {
                    if ((_LoginInfo != null) && (_LoginInfo.User != null) && (config.OrgID == _LoginInfo.User.OrgId))
                    {
                        _reginfo = config.Value;
                    }
                }
                else
                {
                    result.SetString(config.Name, config.Value);
                }
            }
            string checkinfo = CheckReg(_customer, _reginfo, _orgid);
            result.Customer = checkinfo;
             
            return result;
        }

        private string CheckReg(string Customer, string reginfo,int _orgid)
        {
         // string reginfo=  ConfigurationManager.AppSettings["RegInfo"].ToString();
            CheckResult _r = astatic.checkreg(reginfo, Customer);
            if (astatic.DicOrgReg.ContainsKey(_orgid))
            {
                astatic.DicOrgReg[_orgid] = _r.isok;
            }
            else
            {
                astatic.DicOrgReg.Add(_orgid, _r.isok);
            }
            string addinfo = _r.Message;

          return Customer + "["+addinfo+"]";


        }
    }
}
