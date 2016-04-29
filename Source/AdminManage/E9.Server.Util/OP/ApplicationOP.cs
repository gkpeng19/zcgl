using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NM.Model;
using System.Data.SqlClient;
using System.Net;
using NM.Util;
using System.Web;
using NM.Config;
using NM.Service;
using System.IO;
using System.Web.Hosting;
using System.Data;
using System.Configuration;

namespace NM.OP
{
    public class ApplicationOP : CertifiedProviderOP
    {
        LoginInfo login = null;
        public ApplicationOP(LoginInfo user)
            : base(user, DataProvider.GetEAP_Provider())
        {
            login = user;
        }

        public AppSetting LoadAppSetting()
        {
            return new AppSettingOP(this.Account).GetAppSetting();
        }

        public CommandResult GetAutoNo(string key)
        {
            return new UtilOP(Account).GetAutoNo(key);
        }

        public TJsonList<DiagnoseResult> Diagnose(ServiceContext context)
        {
            TJsonList<DiagnoseResult> result = new TJsonList<DiagnoseResult>();

            foreach (DiagnosticProvider item in DiagnosticProviderConfigSection.GetServiceProvider())
            {
                item.ServiceContext = context;
                item.Diagnose();
                result.AddRange(item.Reuslt);
            }
            int index = 0;
            foreach (var item in result)
            {
                item.ID = ++index;
            }
            return result;
        }

        public CommandResult GetCacheSummary()
        {
            CommandResult result = new CommandResult();
            StringBuilder sb = new StringBuilder();
            foreach (CacheProvider item in CacheProviderConfigSection.GetServiceProvider())
            {
                sb.AppendLine(item.GetCatheSummary());
            }
            result.Message = sb.ToString();
            result.Result = true;
            return result;
        }

        public CommandResult ClearCache()
        {
            CommandResult result = new CommandResult();
            StringBuilder sb = new StringBuilder();
            foreach (CacheProvider item in CacheProviderConfigSection.GetServiceProvider())
            {
                item.ClearCathe();
            }
            result.Message = "清除成功";
            result.Result = true;
            return result;
        }


        public CommandResult ExportLicence()
        {
            CommandResult result = new CommandResult();
            result.Message = Guid.NewGuid().ToString();
            result.Result = true;
            return result;
        }

        public CommandResult GetAllService()
        {
            CommandResult result = new CommandResult();
            result.Message = ServiceManager.Default.ToString();
            result.Result = true;
            return result;
        }

        public CommandResult GetAllEntity()
        {
            CommandResult result = new CommandResult();
            result.Message = EntityMetaManager.Default.ToString();
            result.Result = true;
            return result;
        }

        #region 设为当前使用帐套
        public WMS_ACCOUNTSET SetCurrAccountSet(WMS_ACCOUNTSET item)
        {
            var parameters = new[]{
                 new SqlParameter("@ID",item.ID),
                 new SqlParameter("@ORGID",item.ORGID),
                 new SqlParameter("@OpUser",login.User.UserName)
            };

            List<WMS_ACCOUNTSET> listResult = DataProvider.LoadData<WMS_ACCOUNTSET>("usp_SetCurrAccountSet", parameters);
            if (listResult.Count > 0)
            {
                return listResult[0];
            }
            item.ResultID = -1;
            return item;
        }
        #endregion

        #region  备份帐套,,暂无用，为导出到Excle
        public CommandResult ExportExcelAccountSet(string AccountYear, int Orgid)
        {
            // int CostSourceType = -1; // 0：订单1：退货单2：费用3:收货调整4、专柜销售
            CommandResult _result = new CommandResult();

            string sf = "AccountYear";

            string sql = string.Format("select * from uv_kct_accountest  where {0}={1}  ", sf, AccountYear);

            List<KCT_ACCOUNTSET> _contents = DataProvider.LoadData<KCT_ACCOUNTSET>(sql);
            if (_contents.Count == 0)
            {
                _result.Result = false;
                _result.Message = "无数据";
                return _result;
            }
            StringBuilder title = new StringBuilder();
            string fileName = AccountYear + "_" + Guid.NewGuid().ToString();
            string _localPath = string.Format(@HostingEnvironment.ApplicationPhysicalPath + "data/{0}.xls", fileName);
            FileStream fs = new FileStream(_localPath, FileMode.OpenOrCreate);
            //FileStream fs1 = File.Open(file, FileMode.Open, FileAccess.Read);
            StreamWriter sw = new StreamWriter(new BufferedStream(fs), System.Text.Encoding.Default);
            try
            {
                title.AppendLine("帐套时间" + "\t" + "是否使用" + "\t" +
                    "备注"
                    );
                foreach (KCT_ACCOUNTSET item in _contents)
                {
                    title.AppendLine(item.ACCOUNTYEAR + "\t" + " " + item.ISCURRACCOUNT_G + "\t" + " " + item.REMARK);
                }
                sw.Write(title.ToString());
            }
            finally
            {
                sw.Close();
                fs.Close();
                fs = null;
            }
            _result.IntResult = 1;
            _result.Result = true;
            _result.Message = fileName;
            return _result;

        }
        #endregion

        #region 备份帐套

        public CommandResult DirecExportData(string AccountsetYear,int AccountSetID,bool IsDelData)
        {
            CommandResult result = new CommandResult();
            string ToDirectoryName = AccountsetYear + "_" + login.User.Org_Name_G + "_"
                + DateTime.Now.ToShortDateString() + "-"
                + DateTime.Now.ToLongTimeString().Replace(':', '-');
            string ExportDataTo = HostingEnvironment.MapPath("~/data/") + ToDirectoryName;
            if (!Directory.Exists(ExportDataTo))
            {
                Directory.CreateDirectory(ExportDataTo);
            }
            result.Result=DataProvider.DirecExporeData(AccountSetID,ExportDataTo);
            if (result.Result&&IsDelData)
            {
                result.Result = DelAccountSetData(AccountSetID);
            }
            result.Message = ToDirectoryName + ".kct";
            return result;
        }

        #endregion

        #region 恢复帐套

        public CommandResult<KCT_ACCOUNTSET> ImportDataToTempTable(string fileName)
        {
            CommandResult<KCT_ACCOUNTSET> result = new CommandResult<KCT_ACCOUNTSET>();
            result.Result = false;

            string filePath=HttpContext.Current.Server.MapPath("~/data/"+fileName);
            string directoryPath = filePath.Substring(0, filePath.LastIndexOf('.'));
            try
            {
                FileCompress.Decompress(directoryPath, filePath);
            }
            catch (Exception e)
            {
                result.Message = e.Message;
                return result;
            }
            #region 校验文件，获得帐套ID

            //key是TableName  value是保存到的文件名
            Dictionary<string,string> tableList = LoadBCPConfigTables();
            int accountSetID = 0;
            string accountSetName = string.Empty;
            ValidateUploadData(directoryPath, tableList,ref accountSetID,ref accountSetName);
            if (accountSetID == 0)
            {
                result.Message = "错误的数据文件!";
                try
                {
                    File.Delete(filePath);
                }
                catch { }
                return result;
            }

            #endregion

            Dictionary<string,string> dicData=new Dictionary<string,string>();
            string[] strs = null;
            foreach (var tableInfo in tableList)
            {
                strs=tableInfo.Key.Split('.');
                dicData.Add(strs[0] + "..Temp_" + strs[strs.Length - 1], directoryPath + "\\" + tableInfo.Value + ".txt");
            }

            #region 删除临时表中数据，不成功直接返回错误

            if (!DelTempTableData(accountSetID))
            {
                return result;
            }

            #endregion
            
            //执行BCP导入数据到临时表中
            if (DataProvider.DirecImportData(dicData))
            {
                int r = ValidateAccountsetDataIsDel(accountSetID);
                if (r != 0)//等于0标识存储过程执行发生错误
                {
                    result.Result = true;
                    result.IntResult = r;
                    result.Entity.ID = accountSetID;
                    result.Entity.ACCOUNTYEAR = accountSetName;
                }
            }
            return result;
        }

        Dictionary<string,string> LoadBCPConfigTables()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            using (StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BCPConfigPath"])))
            {
                string[] strs=null;
                while (!sr.EndOfStream)
                {
                    strs=sr.ReadLine().Split(':');
                    list.Add(strs[0], strs[2]);
                }
            }
            return list;
        }

        void ValidateUploadData(string directoryPath, Dictionary<string,string> tableList,ref int accountsetID,ref string accountsetName)
        {
            DirectoryInfo di = new DirectoryInfo(directoryPath);
            string str = string.Empty;
            foreach (var item in di.GetFiles())
            {
                str = item.Name.Substring(0, item.Name.LastIndexOf('.'));
                if (!tableList.Values.Contains(str))
                {
                    accountsetID = 0;
                    return;
                }

                #region 读取帐套文件，获得要恢复的帐套ID

                if (str.Equals(tableList["nm_eap..kct_accountset"], StringComparison.CurrentCultureIgnoreCase))
                {
                    using (StreamReader sr = new StreamReader(item.OpenRead()))
                    {
                        while (!sr.EndOfStream)
                        {
                            try
                            {
                                string[] strs=sr.ReadLine().Split(',');
                                if (login.User.OrgId != Convert.ToInt32(strs[1]))
                                {
                                    accountsetID = 0;
                                    return;
                                }
                                accountsetID = Convert.ToInt32(strs[0]);
                                accountsetName = strs[2];
                                break;
                            }
                            catch { accountsetID = 0; return; }
                        }
                    }
                }

	            #endregion
            }
        }

        #endregion

        #region 删除临时表中的数据

        bool DelTempTableData(int accountSetID)
        {
            try
            {
                int resultID = (int)DataProvider.LoadTable("usp_DelTempTableData", new SqlParameter("@accountsetid",accountSetID)).Rows[0][0];
                return resultID == 0 ? false : true;
            }
            catch { }
            return false;
        }

        #endregion

        #region 删除帐套下的数据

        bool DelAccountSetData(int AccountSetID)
        {
            int result=0;
            try
            {
                DataTable dt = DataProvider.LoadTable("usp_DelAccountSetData", new SqlParameter("@accountsetid", AccountSetID));
                result = Convert.ToInt32(dt.Rows[0][0]);
            }
            catch
            {
                return false;
            }
            return result == 0 ? false : true;
        }

        #endregion

        #region 调用存储过程检查实际表中是否存在要恢复帐套的数据

        int ValidateAccountsetDataIsDel(int AccountsetID)
        {
            try
            {
                return (int)DataProvider.LoadTable("usp_validateAccountSetDataIsDel", new SqlParameter("@accountsetid", AccountsetID)).Rows[0][0];
            }
            catch { return 0; }
        }

        #endregion

        #region 调用存储过程，恢复临时表中的数据到实际表中

        public CommandResult RecoverAccountset(int AccountsetID)
        {
            CommandResult result = new CommandResult();
            try
            {
                //调用存储过程，恢复临时表中的数据到实际表中
                DataRow dr = DataProvider.LoadTable(
                    "usp_RecoverAccountset", 
                    new SqlParameter("@accountsetid", AccountsetID),
                    new SqlParameter("@opuser", login.User.UserName)).Rows[0];
                result.Result = Convert.ToBoolean(dr[0]);
                result.Message = dr[1].ToString();
            }
            catch (Exception e)
            {
                result.Result = false;
                result.Message = e.Message;
            }
            //if (result.Message.Contains("PK_"))
            //{
            //    result.Message = "系统中已存在该文件的数据,无需恢复!";
            //}
            return result;
        }

        #endregion

        #region 删除帐套

        public CommandResult DelAccountSet(int AccountSetID)
        {
            CommandResult result = new CommandResult();
            try
            {
                DataTable dt = DataProvider.LoadTable("usp_DelAccountSet", new SqlParameter("@AccountSetID", AccountSetID));
                result.IntResult = Convert.ToInt32(dt.Rows[0][0]);
                result.Message = dt.Rows[0][1].ToString();
            }
            catch(Exception e)
            {
                result.IntResult = 0;
                result.Message = e.Message;
            }
            return result;
        }

        #endregion


    }
}
