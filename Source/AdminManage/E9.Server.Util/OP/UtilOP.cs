using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NM.Model;
using NM.Log;
using System.Data.SqlClient;

namespace NM.OP
{
    public class UtilOP : CertifiedProviderOP
    {
        public UtilOP(LoginInfo user)
            : base(user, DataProvider.GetEAP_Provider())
        {

        }

        #region ** AutoNo Methods

        public CommandResult GetAutoNo(string sKey)
        {
            CommandResult result = new CommandResult();
            result.Message = AutoNo(sKey);
            return result;
        }

        public string AutoNo(string sKey)
        {
            SqlParameter p1;
            SqlParameter pOut;
            if (DataProvider.DataBaseType == DataBaseType.SqlServer)
            {
                  p1 = new SqlParameter("@key", sKey);
                  pOut = new SqlParameter("@out", "aaaaaaaaaaaaaa") { Direction = System.Data.ParameterDirection.InputOutput };
            }
            else
            {
                p1 = new SqlParameter("@PKEY", sKey);
                pOut = new SqlParameter("@POUT", "aaaaaaaaaaaaaa") { Direction = System.Data.ParameterDirection.InputOutput };
            }
         
            DataProvider.ExecuteNonQuery("usp_CreateAutoNo ", new SqlParameter[] { p1, pOut });
            return pOut.Value.ToString();
        }

        #endregion

        #region ** Log Methods

        public void LogError(string module, Exception ex)
        {
            LogIt(module, ex.ToString(), LogLevel.Error);
        }

        public void LogDebug(string module, string message)
        {
            LogIt(module, message, LogLevel.Debug);
        }


        public CommandResult LogIt(string module, string message, LogLevel logLevel)
        {
			return LogIt(Account.SessionID, module, message, logLevel);
		}


		public CommandResult LogIt(int sessionID, string module, string message, LogLevel logLevel)
		{
			CommandResult result = new CommandResult();
			if (message.Length > 1024)
				message = message.Substring(0, 1024);
			string strSQL = " insert into APP_Log(sessionId, message,moduleName,logLevel) values({0},'{1}','{2}',{3}) ";
			strSQL = string.Format(strSQL, sessionID, message, module, (int)logLevel);
			DataProvider.ExecuteNonQuery(strSQL);
			result.Result = true;
			return result;
		}

   
        //        public int SaveMessageInfos(MessageItem data)
        //        {
        //            string strSQL = @"insert into la_message (message_id,message,parameter,response_id,staffer_id,to_staffer_id) 
        //                              values({0},'{1}','{2}',{3},{4},'{5}'); SELECT CAST(scope_identity() AS int);";
        //            data.id = DataProvider.ExecuteScalar<int>(string.Format(strSQL, data.message_id, data.message, data.parameter, data.response_id, data.staffer_id, data.to_staffer_id));
        //            return data.id;
        //        }

        //public List<MessageItem> GetMessageInfoListByStaffID(int staffID)
        //{
        //    string strSQL_1 = "select * from APP_Message";
        //    return DataProvider.LoadData<MessageItem>(strSQL_1);
        //}
        #endregion

    }
}
