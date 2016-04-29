using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data.Common;
using System.Configuration;
using NM.Util;
using Oracle.DataAccess.Client;
using System.Diagnostics;
using System.IO;
using NM.Log;
using NM.Lib;
using System.Web;
using System.Text;
using System.Web.Hosting;
using System.Threading;

namespace NM.OP
{
    public enum DataBaseType
    {
        SqlServer = 0,
        Oracle = 1
    }
    public static class DataHelper
    {
        public static string ReadString(this DataRow row, string fieldName)
        {
            object value = row[fieldName];
            return (value != null && value != DBNull.Value) ? value.ToString() : "";
        }

        public static string ReadString(this DataRow row, int colIndex)
        {
            object value = row[colIndex];
            return (value != null && value != DBNull.Value) ? value.ToString() : "";
        }

        public static int ReadInt(this DataRow row, string fieldName)
        {
            int result = 0;
            object value = row[fieldName];
            if (value != null && value != DBNull.Value)
            {
                int.TryParse(value.ToString(), out result);
            }
            return result;
        }

        public static int ReadInt(this DataRow row, int colIndex)
        {
            int result = 0;
            object value = row[colIndex];
            if (value != null && value != DBNull.Value)
            {
                int.TryParse(value.ToString(), out result);
            }
            return result;
        }

        public static string ReadString(this IDataReader row, string fieldName)
        {
            object value = row[fieldName];
            return (value != null && value != DBNull.Value) ? value.ToString() : "";
        }

        public static string ReadString(this IDataReader row, int colIndex)
        {
            object value = row[colIndex];
            return (value != null && value != DBNull.Value) ? value.ToString() : "";
        }

        public static int ReadInt(this IDataReader row, string fieldName)
        {
            int result = 0;
            object value = row[fieldName];
            if (value != null && value != DBNull.Value)
            {
                int.TryParse(value.ToString(), out result);
            }
            return result;
        }

        public static int ReadInt(this IDataReader row, int colIndex)
        {
            int result = 0;
            object value = row[colIndex];
            if (value != null && value != DBNull.Value)
            {
                int.TryParse(value.ToString(), out result);
            }
            return result;
        }
    }

    /// <summary>
    /// 一个操作数据库使用的接口
    /// </summary>
    public interface IDBOperator
    {
        //DataBaseType DataBaseType { get; }
        bool IsTransactionInProgress { get; set; }

        bool BeginTran();

        void CommitTran();

        void RollBack();

        void Open();

        void Close();

        void Dispose();

        bool Test();

        CommandType GetCommandType(string sSqlText);

        void ExecuteNonQuery_Async(string query, Action<object, bool, int> callback, object AsyncState, params DbParameter[] parameters);

        int EndExecuteNonQuery_Async(IAsyncResult result);

        DataSet ExecuteTranDataSet(string query, params DbParameter[] parameters);

        DataTable ExecuteTranDataTable(string query, out string sOutValue, params DbParameter[] parameters);


        DataTable ExecuteTranDataTable(string query, params DbParameter[] parameters);

        int ExecuteTranNonQuery(string query, params DbParameter[] parameters);

        DbDataReader ExecuteTranReader(string query, params DbParameter[] parameters);

        object ExecuteTranScalar(string query, params DbParameter[] parameters);

        object ExecuteScalar(string query, params DbParameter[] parameters);

        #region geng

        object ExecuteScalar(string query, List<DbParameter> parameters);

        #endregion

        DataTable ExecuteDataTable(string query, params DbParameter[] parameters);

        DataTable ExecuteDataTable(string query, out string sOutValue, params DbParameter[] parameters);

        DataSet ExecuteDataSet(string query, params DbParameter[] parameters);

        DbDataReader ExecuteReader(string query, params DbParameter[] parameters);
        //SqlDataReader ExecuteReader(string query, params SqlParameter[] parameters)

        int ExecuteNonQuery(string query, params DbParameter[] parameters);

        #region geng

        int ExecuteNonQuery(string query, List<DbParameter> parameters);

        #endregion


        int DoInsert(SerializableData data, string tableName, bool needKey, List<string> ExcludeFields);
        int DoUpdate(SerializableData data, string tableName, string keyField, List<string> ExcludeFields);
        int DoDelete(SerializableData data, string tableName, string keyField);
        string GetVersion();

        bool DirectImportData(string DataFileName, string DtName, string DtFields);
        bool DirecExprotData(int AccountSetID, string ToDirectoryPath);
        bool DirecImportData(Dictionary<string, string> dicData);
    }

    public class DBOperate : IDBOperator
    {

        public static int COMMANDTIMEOUT = 6000;

        private bool _isTransactionInProgress;
        public bool IsTransactionInProgress
        {
            get
            {
                return _isTransactionInProgress;
            }
            set
            {
                _isTransactionInProgress = value;
            }

        }

        #region

        #region 数据库备份、恢复

        public virtual bool DirectImportData(string DataFileName, string DtName, string DtFields)
        {
            return true;
        }

        public virtual bool DirecImportData(Dictionary<string, string> dicData)
        {
            return true;
        }

        public virtual bool DirecExprotData(int AccountSetID, string ToDirectoryPath)
        {
            return true;
        }

        #endregion

        public virtual string GetVersion()
        {
            return "";
        }
        public virtual bool BeginTran()
        {
            return true;
        }

        public virtual void CommitTran()
        {

        }

        public virtual void RollBack()
        {

        }

        public virtual void Open()
        {

        }

        public virtual void Close()
        {

        }

        public virtual void Dispose()
        {

        }

        public virtual bool Test()
        {
            return true;
        }

        public virtual CommandType GetCommandType(string sSqlText)
        {
            return CommandType.Text;
        }

        public virtual int ExecuteNonQuery(string query, params DbParameter[] parameters)
        {
            return 0;
        }

        public virtual void ExecuteNonQuery_Async(string query, Action<object, bool, int> callback, object AsyncState, params DbParameter[] parameters)
        {

        }

        public virtual int EndExecuteNonQuery_Async(IAsyncResult result)
        {
            return 0;
        }

        public virtual DataSet ExecuteTranDataSet(string query, params DbParameter[] parameters)
        {
            return null;
        }

        public virtual DataTable ExecuteTranDataTable(string query, out string sOutValue, params DbParameter[] parameters)
        {
            sOutValue = "";
            return null;
        }

        public virtual DataTable ExecuteTranDataTable(string query, params DbParameter[] parameters)
        {
            return null;
        }

        public virtual int ExecuteTranNonQuery(string query, params DbParameter[] parameters)
        {
            return 0;
        }

        public virtual DbDataReader ExecuteTranReader(string query, params DbParameter[] parameters)
        {
            return null;
        }

        public virtual object ExecuteTranScalar(string query, params DbParameter[] parameters)
        {
            return null;
        }

        public virtual object ExecuteScalar(string query, params DbParameter[] parameters)
        {
            return null;
        }

        #region geng

        public virtual object ExecuteScalar(string query, List<DbParameter> parameters)
        {
            return null;
        }

        public virtual int ExecuteNonQuery(string query, List<DbParameter> parameters)
        {
            return 0;
        }

        #endregion

        public virtual DataTable ExecuteDataTable(string query, params DbParameter[] parameters)
        {
            return null;
        }

        public virtual DataTable ExecuteDataTable(string query, out string sOutValue, params DbParameter[] parameters)
        {
            sOutValue = "";
            return null;
        }

        public virtual DbDataReader ExecuteReader(string query, params DbParameter[] parameters)
        {
            return null;
        }
        #endregion


        public virtual DataSet ExecuteDataSet(string query, params DbParameter[] parameters)
        {
            return null;
        }



        public virtual int DoInsert(SerializableData data, string tableName, bool needKey, List<string> ExcludeFields)
        {

            return 0;
        }

        public virtual int DoUpdate(SerializableData data, string tableName, string keyField, List<string> ExcludeFields)
        {
            return 0;
        }

        public virtual int DoDelete(SerializableData data, string tableName, string keyField)
        {
            return 0;
        }

        public virtual void beforedosomthing()
        {

            string reginfo = ""; ConfigurationManager.AppSettings["RegInfo"].ToString();
            string Customer = ""; ConfigurationManager.AppSettings["Customer"].ToString();
            GetCurrGetInfo(ref reginfo, ref Customer);
            CheckResult cr = astatic.checkreg(reginfo, Customer);
            if (!cr.isok)
            {
                throw new Exception(cr.Message);
            }
        }

        public virtual void GetCurrGetInfo(ref string reginfo, ref string Customer)
        {

        }
        //public virtual  DataBaseType DataBaseType
        //{
        //    get 
        //    {
        //        return DataBaseType.SqlServer;
        //    }
        //}

        protected void DebugOutput_SQL(string sSql, params DbParameter[] parameters)
        {
            var outSQL = sSql;
            if (parameters != null)
            {
                foreach (var p in parameters)
                    outSQL += string.Format(" {0}='{1}',", p.ParameterName, p.Value);
            }
            if (outSQL.EndsWith(","))
                outSQL = outSQL.Remove(outSQL.Length - 1);
            LogManager.DebugIt(outSQL);
        }

        #region geng

        protected void DebugOutput_SQL(string sSql, List<DbParameter> parameters)
        {
            var outSQL = sSql;
            if (parameters != null)
            {
                foreach (var p in parameters)
                    outSQL += string.Format(" {0}='{1}',", p.ParameterName, p.Value);
            }
            if (outSQL.EndsWith(","))
                outSQL = outSQL.Remove(outSQL.Length - 1);

            LogManager.DebugIt(outSQL);
        }

        #endregion




    }

    public class SqlServerDBOperate : DBOperate
    {


        private SqlTransaction _tran;


        string _ConnectionString;

        SqlConnection _DBConnection;
        public SqlConnection DBConnection
        {
            get
            {
                if (_DBConnection == null)
                {
                    _DBConnection = new SqlConnection(_ConnectionString);
                }
                return _DBConnection;
            }
            private set { }
        }



        public SqlServerDBOperate(string sConn)
        {
            _ConnectionString = sConn;
        }

        public SqlServerDBOperate(string sConn, bool async)
        {
            _ConnectionString = sConn.Trim();
        }

        public override void GetCurrGetInfo(ref string reginfo, ref string Customer)
        {

        }

        #region 数据库备份、恢复  sql版本

        public override bool DirectImportData(string DataFileName, string DtName, string DtFields)
        {
            try
            {
                //string bcpFileName = "Tools/BCP2008/bcp.exe";
                Process p = new Process();
                p.StartInfo.FileName = @"C:\Program Files\Microsoft SQL Server\90\Tools\Binn\bcp.exe";
                p.StartInfo.UseShellExecute = false;

                //@必须加上，不然特殊字符会被自动过滤掉
                SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(_ConnectionString);
                p.StartInfo.Arguments = string.Format(@"{0}..{5} in ""{4}"" -t, -U{1} -P{2} -S{3} -c", b.InitialCatalog, b.UserID, b.Password, b.DataSource, DataFileName, DtName);
                p.Start();
                p.WaitForExit();
                p.Close();
                return true;
            }
            catch (Exception ee)
            {
                return false;
            }
        }

        public override bool DirecImportData(Dictionary<string, string> dicData)
        {
            try
            {
                #region 使用BCP，插入数据到临时表中

                string path = ConfigurationManager.AppSettings["BCPPath"].ToString();
                Process p = new Process();
                p.StartInfo.FileName = path;
                p.StartInfo.UseShellExecute = false;        //关闭Shell的使用
                p.StartInfo.CreateNoWindow = true;      //设定不显示窗口

                //@必须加上，不然特殊字符会被自动过滤掉
                SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(_ConnectionString);
                foreach (var data in dicData)
                {
                    p.StartInfo.Arguments = string.Format(@"""{0}"" in ""{4}"" -t, -U{1} -P{2} -S{3} -c", data.Key, b.UserID, b.Password, b.DataSource, data.Value);
                    p.Start();
                }
                p.WaitForExit();
                p.Close();

                #endregion
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public override bool DirecExprotData(int AccountSetID, string ToDirectoryPath)
        {
            try
            {
                string path = ConfigurationManager.AppSettings["BCPPath"].ToString();
                Process p = new Process();
                p.StartInfo.FileName = path;
                p.StartInfo.UseShellExecute = false;        //关闭Shell的使用
                p.StartInfo.CreateNoWindow = true;          //设定不显示窗口

                //@必须加上，不然特殊字符会被自动过滤掉
                SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(_ConnectionString);
                int fileLength = 0;
                using (StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BCPConfigPath"])))
                {
                    string[] strs = null;
                    string savePath = string.Empty;
                    while (!sr.EndOfStream)
                    {
                        fileLength++;
                        strs = sr.ReadLine().Split(':');
                        savePath = ToDirectoryPath + "\\" + strs[2] + ".txt";
                        p.StartInfo.Arguments = string.Format(@"""{0}"" queryout ""{4}"" -t, -U{1} -P{2} -S{3} -c", string.Format(strs[1], AccountSetID.ToString()), b.UserID, b.Password, b.DataSource, savePath);
                        p.Start();
                    }
                }
                p.WaitForExit();
                p.Close();
                if (Directory.GetFiles(ToDirectoryPath).Length < fileLength)
                {
                    Thread.Sleep(1000);
                    if (Directory.GetFiles(ToDirectoryPath).Length < fileLength)
                    {
                        return false;
                    }
                }
                FileCompress.Compress(ToDirectoryPath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion


        public override string GetVersion()
        {
            object obj = ExecuteScalar(" Select  @@Version ");
            string version = obj.ToString();
            return version.Length > 28 ? version.Substring(0, 28) : "Unknow";
        }
        public override bool BeginTran()
        {
            if (IsTransactionInProgress)
                return true;
            try
            {
                this.DBConnection.Open();
                _tran = this.DBConnection.BeginTransaction();
                IsTransactionInProgress = true;
            }
            catch
            {
                IsTransactionInProgress = false;
            }
            return IsTransactionInProgress;
        }

        public override void CommitTran()
        {
            _tran.Commit();
            IsTransactionInProgress = false;
            this.DBConnection.Close();
            _tran.Dispose();
        }

        public override void RollBack()
        {
            _tran.Rollback();
            IsTransactionInProgress = false;
            this.DBConnection.Close();
            _tran.Dispose();
        }





        public override void Open()
        {

            if (DBConnection.State == ConnectionState.Closed)
            {
                try
                {
                    DBConnection.Open();
                }
                catch (SqlException ex)
                {
                    throw new ApplicationException("数据库连接失败,详细信息如下：" + ex.Message, ex);
                }
            }
        }

        public override void Close()
        {
            if (IsTransactionInProgress)
            {
                return;
            }
            if (DBConnection != null)
            {
                if (DBConnection.State == ConnectionState.Open)
                {
                    DBConnection.Close();
                }
            }
        }

        public override void Dispose()
        {
            if (DBConnection != null)
            {
                DBConnection.Dispose();
                DBConnection = null;
            }
        }

        public override bool Test()
        {
            try
            {
                Open();
                Close();
                Dispose();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public override CommandType GetCommandType(string sSqlText)
        {
            sSqlText = sSqlText.TrimStart();
            if (sSqlText.StartsWith("INSERT", StringComparison.InvariantCultureIgnoreCase)
                || sSqlText.StartsWith("UPDATE", StringComparison.InvariantCultureIgnoreCase)
                || sSqlText.StartsWith("DROP", StringComparison.InvariantCultureIgnoreCase)
                || sSqlText.StartsWith("DELETE", StringComparison.InvariantCultureIgnoreCase)
                || sSqlText.StartsWith("EXEC ", StringComparison.InvariantCultureIgnoreCase)
                || sSqlText.StartsWith("ALTER ", StringComparison.InvariantCultureIgnoreCase)
                || sSqlText.StartsWith("CREATE ", StringComparison.InvariantCultureIgnoreCase)
                || sSqlText.StartsWith("SELECT", StringComparison.InvariantCultureIgnoreCase))
            {
                return CommandType.Text;
            }
            else
            {
                return CommandType.StoredProcedure;
            }
        }

        public override void ExecuteNonQuery_Async(string query, Action<object, bool, int> callback, object AsyncState, params DbParameter[] parameters)
        {
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query.Trim());
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }

            try
            {
                Open();
                cmd.BeginExecuteNonQuery((IAsyncResult result) =>
                {
                    var sqlResult = result.IsCompleted;
                    var actionRecorderCount = EndExecuteNonQuery_Async(result);

                    if (callback != null)
                        callback(((AsyncCallArgs)result.AsyncState).State, sqlResult, actionRecorderCount);
                }, new AsyncCallArgs(AsyncState, cmd));
            }
            catch (SqlException ex)
            {
                Close();
                throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
            }
            //finally
            //{
            //    Close();
            //}
        }

        class AsyncCallArgs
        {
            public AsyncCallArgs(object userState, SqlCommand command)
            {
                State = userState;
                Command = command;
            }
            public SqlCommand Command { get; set; }
            public object State { get; set; }
        }


        public override int EndExecuteNonQuery_Async(IAsyncResult result)
        {
            int r = 0;
            SqlCommand command = null;
            try
            {
                if (result.IsCompleted)
                {
                    command = ((AsyncCallArgs)result.AsyncState).Command;
                    r = command.EndExecuteNonQuery(result);
                }
            }
            catch (Exception ex)
            {
                //add exception log
            }
            finally
            {
                if (command != null && command.Connection != null)
                {
                    command.Connection.Close();
                    command.Dispose();
                }
            }
            return r;
        }

        public override int ExecuteNonQuery(string query, params DbParameter[] parameters)
        {
            DebugOutput_SQL(query, parameters);
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query.Trim());
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }
            if (IsTransactionInProgress)
            {
                cmd.Transaction = _tran;
                return cmd.ExecuteNonQuery();
            }
            else
            {
                int retval;
                try
                {
                    Open();
                    SqlTransaction tran = this.DBConnection.BeginTransaction();
                    try
                    {
                        cmd.Transaction = tran;
                        retval = cmd.ExecuteNonQuery();
                        tran.Commit();
                    }
                    catch (SqlException ex1)
                    {
                        tran.Rollback();
                        throw new Exception("数据库操作执行失败，详细信息：" + ex1.Message);
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                finally
                {
                    Close();
                }
                return retval;
            }
        }

        #region geng

        public override int ExecuteNonQuery(string query, List<DbParameter> parameters)
        {
            DebugOutput_SQL(query, parameters);
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query.Trim());
            if (parameters != null)
                for (int i = 0; i <= parameters.Count - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }
            if (IsTransactionInProgress)
            {
                cmd.Transaction = _tran;
                return cmd.ExecuteNonQuery();
            }
            else
            {
                int retval;
                try
                {
                    Open();
                    SqlTransaction tran = this.DBConnection.BeginTransaction();
                    try
                    {
                        cmd.Transaction = tran;
                        retval = cmd.ExecuteNonQuery();
                        tran.Commit();
                    }
                    catch (SqlException ex1)
                    {
                        tran.Rollback();
                        throw new Exception("数据库操作执行失败，详细信息：" + ex1.Message);
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                finally
                {
                    Close();
                }
                return retval;
            }
        }

        #endregion

        public override DataSet ExecuteTranDataSet(string query, params DbParameter[] parameters)
        {
            query = query.Trim();
            DebugOutput_SQL(query, parameters);
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }
            }

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd); ;
            if (IsTransactionInProgress)
            {
                cmd.Transaction = _tran;
                da.Fill(ds);
            }
            else
            {
                try
                {
                    Open();
                    da.Fill(ds);
                }
                catch (SqlException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                finally
                {
                    Close();
                }
            }
            return ds;
        }

        public override DataTable ExecuteTranDataTable(string query, out string sOutValue, params DbParameter[] parameters)
        {
            query = query.Trim();
            DebugOutput_SQL(query, parameters);
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            SqlParameter outParameter = null;
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    if (parameters[i].Direction == ParameterDirection.InputOutput)
                        outParameter = cmd.Parameters.Add(parameters[i] as SqlParameter);
                    else
                        cmd.Parameters.Add(parameters[i]);
                }

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd); ;
            if (IsTransactionInProgress)
            {
                cmd.Transaction = _tran;
                da.Fill(ds);
                if (outParameter != null)
                    sOutValue = outParameter.Value.ToString();
                else
                    sOutValue = string.Empty;
            }
            else
            {
                try
                {
                    Open();
                    da.Fill(ds);
                    if (outParameter != null)
                        sOutValue = outParameter.Value.ToString();
                    else
                        sOutValue = string.Empty;
                }
                catch (SqlException ex)
                {
                    throw new Exception(string.Format("数据库操作执行失败，详细信息：{0}{1}", ex.Message, _ConnectionString));
                }
                finally
                {
                    Close();
                }
            }
            return ds.Tables[0];
        }

        public override DataTable ExecuteTranDataTable(string query, params DbParameter[] parameters)
        {
            query = query.Trim();
            DebugOutput_SQL(query, parameters);
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            if (IsTransactionInProgress)
            {
                cmd.Transaction = _tran;
                da.Fill(ds);
            }
            else
            {
                try
                {
                    Open();
                    da.Fill(ds);
                }
                catch (SqlException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                finally
                {
                    Close();
                }
            }
            return ds.Tables[0];
        }


        public override int ExecuteTranNonQuery(string query, params DbParameter[] parameters)
        {
            DebugOutput_SQL(query, parameters);
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query.Trim());
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }

            if (IsTransactionInProgress)
            {
                cmd.Transaction = _tran;
                return cmd.ExecuteNonQuery();
            }
            else
            {
                int retval;
                try
                {
                    Open();
                    SqlTransaction tran = this.DBConnection.BeginTransaction();
                    try
                    {
                        cmd.Transaction = tran;
                        retval = cmd.ExecuteNonQuery();
                        tran.Commit();
                    }
                    catch (SqlException ex1)
                    {
                        tran.Rollback();
                        throw new Exception("数据库操作执行失败，详细信息：" + ex1.Message);
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                finally
                {
                    Close();
                }
                return retval;
            }

        }

        public override DbDataReader ExecuteTranReader(string query, params DbParameter[] parameters)
        {
            DebugOutput_SQL(query, parameters);
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);

            for (int i = 0; i <= parameters.Length - 1; i++)
            {
                cmd.Parameters.Add(parameters[i]);
            }

            SqlDataReader dr;
            if (IsTransactionInProgress)
            {
                cmd.Transaction = _tran;
                dr = cmd.ExecuteReader();
                return dr;
            }
            else
            {
                try
                {
                    Open();
                    dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    return dr;
                }
                catch (SqlException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                //finally
                //{
                //    Close();
                //}
            }
        }

        public override object ExecuteTranScalar(string query, params DbParameter[] parameters)
        {
            DebugOutput_SQL(query, parameters);
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }

            object retval;
            if (IsTransactionInProgress)
            {
                cmd.Transaction = _tran;
                retval = cmd.ExecuteScalar();
            }
            else
            {
                try
                {
                    Open();
                    retval = cmd.ExecuteScalar();
                }
                catch (SqlException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                finally
                {
                    Close();
                }
            }
            return retval;
        }

        public override object ExecuteScalar(string query, params DbParameter[] parameters)
        {
            DebugOutput_SQL(query, parameters);
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }

            object retval;
            if (IsTransactionInProgress)
            {
                cmd.Transaction = _tran;
                retval = cmd.ExecuteScalar();
            }
            else
            {
                try
                {
                    Open();
                    retval = cmd.ExecuteScalar();
                }
                catch (SqlException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                finally
                {
                    Close();
                }
            }
            return retval;


        }

        #region geng

        public override object ExecuteScalar(string query, List<DbParameter> parameters)
        {
            DebugOutput_SQL(query, parameters);
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
                for (int i = 0; i <= parameters.Count - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }

            object retval;
            if (IsTransactionInProgress)
            {
                cmd.Transaction = _tran;
                retval = cmd.ExecuteScalar();
            }
            else
            {
                try
                {
                    Open();
                    retval = cmd.ExecuteScalar();
                }
                catch (SqlException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                finally
                {
                    Close();
                }
            }
            return retval;
        }

        #endregion


        public override DataTable ExecuteDataTable(string query, params DbParameter[] parameters)
        {
            query = query.Trim();
            DebugOutput_SQL(query, parameters);
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }


            DataSet ds;

            if (IsTransactionInProgress)
            {
                cmd.Transaction = _tran;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
            }
            else
            {
                try
                {
                    Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);
                }
                catch (SqlException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                finally
                {
                    Close();
                }
            }

            return ds.Tables[0];
        }

        public override DataSet ExecuteDataSet(string query, params DbParameter[] parameters)
        {
            query = query.Trim();
            DebugOutput_SQL(query, parameters);
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }
            }

            DataSet ds;
            try
            {
                Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
            }
            catch (SqlException ex)
            {
                throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
            }
            finally
            {
                Close();
            }

            return ds;
        }

        public override DataTable ExecuteDataTable(string query, out string sOutValue, params DbParameter[] parameters)
        {
            query = query.Trim();
            DebugOutput_SQL(query, parameters);
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            SqlParameter outParameter = null;
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    if (parameters[i].Direction == ParameterDirection.InputOutput)
                        outParameter = cmd.Parameters.Add(parameters[i] as SqlParameter);
                    else
                        cmd.Parameters.Add(parameters[i]);
                }

            DataSet ds;
            try
            {
                Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                if (outParameter != null)
                    sOutValue = outParameter.Value.ToString();
                else
                    sOutValue = string.Empty;
            }
            catch (SqlException ex)
            {
                throw new Exception(string.Format("数据库操作执行失败，详细信息：{0}{1}", ex.Message, _ConnectionString));
            }
            finally
            {
                Close();
            }

            return ds.Tables[0];
        }


        #region ** SqlDataReader

        public override DbDataReader ExecuteReader(string query, params DbParameter[] parameters)
        {
            DebugOutput_SQL(query, parameters);
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);

            for (int i = 0; i <= parameters.Length - 1; i++)
            {
                cmd.Parameters.Add(parameters[i]);
            }

            SqlDataReader dr;
            try
            {
                Open();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return dr;
            }
            catch (SqlException ex)
            {
                throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
            }
            //finally
            //{
            //    Close();
            //}
        }

        #endregion


        public override int DoInsert(SerializableData data, string tableName, bool needKey, List<string> ExcludeFields)
        {
            if (data.Items.Count <= 0)
                return -1;

            var sFields = "";
            var sValues = "";
            List<DbParameter> list = new List<DbParameter>();
            data.Items.ForEach(e =>
            {
                if (!string.IsNullOrEmpty(e.K) && !e.K.EndsWith("_G") && !string.IsNullOrEmpty(e.V))
                {
                    string fn = e.K.ToUpper();

                    if ((ExcludeFields == null) || (!ExcludeFields.Exists((x) => { return x.ToUpper() == fn; })))
                    {
                        sFields += string.Format("[{0}],", e.K);
                        sValues += string.Format("@{0},", e.K);//------------------
                        if (e.V == DateTime.MinValue.ToString(DataItem.DATETIMEFORMAT))
                        {
                            list.Add(new SqlParameter(e.K, null) as DbParameter);
                        }
                        else
                        {
                            list.Add(new SqlParameter(e.K, e.V) as DbParameter);
                        }
                    }
                }
            });


            var sSql = string.Format(
                "INSERT INTO {0}({1}) VALUES({2});{3}",
                tableName,
                sFields.Trim().TrimEnd(','),
                sValues.Trim().TrimEnd(','),
                needKey ? "SELECT CAST(scope_identity() AS int);" : "");

            var iPID = ExecuteScalar(sSql, list);
            return (int)iPID;
        }

        public override int DoUpdate(SerializableData data, string tableName, string keyField, List<string> ExcludeFields)
        {
            var sSql = string.Format("UPDATE {0} SET ", tableName);
            var sql_Update = "";
            List<DbParameter> list = new List<DbParameter>();
            data.Items.ForEach(e =>
            {
                //if (!string.IsNullOrEmpty(e.K)
                //    && !e.K.EndsWith("_G")
                //    && !e.K.Equals(keyField, StringComparison.InvariantCultureIgnoreCase)
                //    && !string.IsNullOrEmpty(e.V))
                if (!string.IsNullOrEmpty(e.K)
                    && !e.K.EndsWith("_G")
                    && !e.K.Equals(keyField, StringComparison.InvariantCultureIgnoreCase))
                {
                    string fn = e.K.ToUpper();

                    if ((ExcludeFields == null) || (!ExcludeFields.Exists((x) => { return x.ToUpper() == fn; })))
                    {
                        //if ((e.S == EntityStatus.New || e.S == EntityStatus.Modified) && !string.IsNullOrEmpty(e.V))
                        if ((e.S == EntityStatus.New || e.S == EntityStatus.Modified))
                        {
                            sql_Update += string.Format("[{0}]=@{1},", e.K, e.K);
                            if ((e.T == TypeCode.DateTime) && e.V == DateTime.MinValue.ToString(DataItem.DATETIMEFORMAT))
                            {
                                list.Add(new SqlParameter(e.K, DBNull.Value) as DbParameter);
                            }
                            else if (e.T == TypeCode.String && string.IsNullOrEmpty(e.V))
                            {
                                list.Add(new SqlParameter(e.K, DBNull.Value) as DbParameter);
                            }
                            else
                            {
                                list.Add(new SqlParameter(e.K, e.V) as DbParameter);
                            }
                        }
                        else if (e.S == EntityStatus.Delete)
                        {
                            sql_Update += string.Format("[{0}]=null,", e.K);
                        }
                    }
                }

            });

            sql_Update = sql_Update.Trim().TrimEnd(',');
            if (!string.IsNullOrEmpty(sql_Update))
            {
                sSql += sql_Update;
                sSql += string.Format(" WHERE {0}=@{1}", keyField, keyField);
                list.Add(new SqlParameter(keyField, data[keyField]) as DbParameter);
                return ExecuteNonQuery(sSql, list);
            }
            return 0;
        }

        public override int DoDelete(SerializableData data, string tableName, string keyField)
        {
            CheckForeign(tableName, keyField, data[keyField].ToString());
            var sSql = string.Format("Delete {0} where {1} =@{1}", tableName, keyField);
            return ExecuteNonQuery(sSql, new SqlParameter(keyField, data[keyField]) as DbParameter);
        }

        private void CheckForeign(string tableName, string keyField, string KeyValue)
        {
            SqlParameter[] dbpars = new SqlParameter[] {                  
                          new SqlParameter("@TableName",tableName ),   
                          new SqlParameter("@keyField",keyField),
                          new SqlParameter("@KeyValue",KeyValue) 
                           };
            DataTable dt = ExecuteDataTable("usp_CheckForeign", dbpars);
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                string r = dt.Rows[0]["ResultID"].ToString();
                if (r == "0")
                {
                    throw new Exception(dt.Rows[0]["ErrorMsg"].ToString());
                }

            }
        }
    }

    public class OracleDBOperate : DBOperate
    {


        private OracleTransaction _tran;


        string _ConnectionString;

        OracleConnection _DBConnection;
        public OracleConnection DBConnection
        {
            get
            {
                if (_DBConnection == null)
                {
                    _DBConnection = new OracleConnection(_ConnectionString);
                    //_DBConnection.ConnectionTimeout = 200;
                }
                return _DBConnection;
            }
            private set { }
        }

        //sql版本


        public OracleDBOperate(string sConn)
        {
            _ConnectionString = sConn;
        }

        public OracleDBOperate(string sConn, bool async)
        {
            _ConnectionString = sConn.Trim();
        }

        public override string GetVersion()
        {
            object obj = ExecuteScalar("Select  version   FROM   Product_component_version where   SUBSTR(PRODUCT,1,6)= 'Oracle'");
            string version = obj.ToString();
            return version;//  .Length > 28 ? version.Substring(0, 28) : "Unknow";
        }
        private string GetControlFile(string DataFileName, string DtName, string DtFields)
        {
            string spath = Path.GetDirectoryName(DataFileName);
            string _sf = spath + @"\" + Guid.NewGuid().ToString() + ".ctl";
            // string _sf = DataFileName + ".ctl";

            using (FileStream stream = new FileStream(_sf, FileMode.Create))
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine("load data");
                writer.WriteLine("append into table  " + DtName);
                writer.WriteLine("fields terminated by \",\" optionally enclosed by '\"' ");
                writer.WriteLine("(" + DtFields + ")");
                writer.Flush();
                stream.Flush();
                writer.Close();
                stream.Close();
            }


            return _sf;

        }

        public override bool DirectImportData(string DataFileName, string DtName, string DtFields)
        {
            try
            {
                string _controlfile = GetControlFile(DataFileName, DtName, DtFields);


                string lf = Path.GetDirectoryName(_controlfile) + "\\" + Path.GetFileNameWithoutExtension(_controlfile);
                File.Copy(DataFileName, lf + ".dat");
                SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(_ConnectionString);
                //p.StartInfo.Arguments = string.Format(@"{0}..{5} in ""{4}"" -t, -U{1} -P{2} -S{3} -c", b.InitialCatalog, b.UserID, b.Password, b.DataSource, DataFileName, DtName);
                ProcessStartInfo p = new ProcessStartInfo();
                string ppath = "";// @"F:\oracle\product\10.2.0\client_1\BIN\\";
                p.FileName = ppath + @"sqlldr.exe"; //D:\oracle\ora92\bin\
                //动态创建一个文件?暂时将DtName用作控制文件名
                //p.Arguments = @"control=D:\loader.ctl DATA=D:\datafile.txt userid=eap@orcl/Jawas0ft";
                p.Arguments = string.Format(@"control={0} DATA={1} userid={2}/{3}@{4} log={5}", _controlfile, lf + ".dat", b.UserID, b.Password, b.DataSource, lf + ".log");
                //p.UseShellExecute = false;
                //p.RedirectStandardOutput = true;
                //p.WindowStyle = ProcessWindowStyle.Hidden;
                //p.CreateNoWindow = true;
                Process pro = Process.Start(p);

                pro.WaitForExit();
                //string output = pro.StandardOutput.ReadToEnd();

                return true;

            }
            catch (Exception ex)
            {

                return false;
            }


        }

        public override bool BeginTran()
        {
            if (IsTransactionInProgress)
            {
                if ((_DBConnection != null) && (_DBConnection.State != ConnectionState.Closed) && (_tran != null) && (_tran.Connection != null))
                {
                    return true;
                }
            }
            try
            {
                DBConnection.Open();
                _tran = this.DBConnection.BeginTransaction();
                IsTransactionInProgress = true;
            }
            catch
            {
                IsTransactionInProgress = false;
            }
            return IsTransactionInProgress;
        }

        public override void CommitTran()
        {
            try
            {

                _tran.Commit();
            }
            finally
            {
                IsTransactionInProgress = false;
                this.DBConnection.Close();
                //_tran.Dispose();               
            }
        }

        public override void RollBack()
        {
            try
            {
                _tran.Rollback();
            }
            finally
            {
                IsTransactionInProgress = false;
                this.DBConnection.Close();
                //_tran.Dispose();
            }
        }





        public override void Open()
        {
            base.Open();
            if (DBConnection.State == ConnectionState.Closed)
            {
                try
                {
                    DBConnection.Open();
                }
                catch (SqlException ex)
                {
                    throw new ApplicationException("数据库连接失败,详细信息如下：" + ex.Message, ex);
                }
            }
        }

        public override void Close()
        {
            if (IsTransactionInProgress)
            {
                return;
            }
            if (DBConnection != null)
            {
                if (DBConnection.State == ConnectionState.Open)
                {
                    DBConnection.Close();
                }
            }
        }

        public override void Dispose()
        {
            if (DBConnection != null)
            {
                DBConnection.Dispose();
                DBConnection = null;
            }
        }

        public override bool Test()
        {
            try
            {
                Open();
                Close();
                Dispose();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public override CommandType GetCommandType(string sSqlText)
        {
            sSqlText = sSqlText.TrimStart();
            if (sSqlText.StartsWith("INSERT", StringComparison.InvariantCultureIgnoreCase)
                || sSqlText.StartsWith("UPDATE", StringComparison.InvariantCultureIgnoreCase)
                || sSqlText.StartsWith("DROP", StringComparison.InvariantCultureIgnoreCase)
                || sSqlText.StartsWith("DELETE", StringComparison.InvariantCultureIgnoreCase)
                || sSqlText.StartsWith("EXEC ", StringComparison.InvariantCultureIgnoreCase)
                || sSqlText.StartsWith("ALTER ", StringComparison.InvariantCultureIgnoreCase)
                || sSqlText.StartsWith("CREATE ", StringComparison.InvariantCultureIgnoreCase)
                || sSqlText.StartsWith("SELECT", StringComparison.InvariantCultureIgnoreCase))
            {
                return CommandType.Text;
            }
            else
            {
                return CommandType.StoredProcedure;
            }
        }

        public override void ExecuteNonQuery_Async(string query, Action<object, bool, int> callback, object AsyncState, params DbParameter[] parameters)
        {
            query = OralceKeyReplace(query.Trim());
            OracleCommand cmd = new OracleCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query.Trim());
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }

            try
            {
                Open();
                throw new Exception("EndExecuteNonQuery_Async未完成");
                /* lxt 20101101 need code  error code
                cmd.BeginExecuteNonQuery((IAsyncResult result) =>
                {
                    var sqlResult = result.IsCompleted;
                    var actionRecorderCount = EndExecuteNonQuery_Async(result);

                    if (callback != null)
                        callback(((AsyncCallArgs)result.AsyncState).State, sqlResult, actionRecorderCount);
                }, new AsyncCallArgs(AsyncState, cmd));
                */
            }
            catch (SqlException ex)
            {
                Close();
                throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
            }
            finally
            {
                Close();
            }
        }
        /* lxt 20101101 need code  error code
       class AsyncCallArgs
       {
           public AsyncCallArgs(object userState, SqlCommand command)
           {
               State = userState;
               Command = command;
           }
           public SqlCommand Command { get; set; }
           public object State { get; set; }
       }
        */


        public override int EndExecuteNonQuery_Async(IAsyncResult result)
        {
            int r = 0;
            SqlCommand command = null;
            try
            {
                if (result.IsCompleted)
                {
                    throw new Exception("EndExecuteNonQuery_Async未完成");
                    /* lxt 20101101 need code  error code
                   command = ((AsyncCallArgs)result.AsyncState).Command;
                   r = command.EndExecuteNonQuery(result);
                     * */
                }
            }
            catch (Exception ex)
            {
                //add exception log
            }
            finally
            {
                if (command != null && command.Connection != null)
                {
                    command.Connection.Close();
                    command.Dispose();
                }
            }
            return r;
        }

        public override int ExecuteNonQuery(string query, params DbParameter[] parameters)
        {
            query = OralceKeyReplace(query.Trim());
            OracleCommand cmd = new OracleCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query.Trim());
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    OracleParameter mm = GetOralceCommandParam(parameters[i]);
                    cmd.Parameters.Add(mm);
                }
            // AutoAddCommandParams(cmd);
            int retval;
            try
            {
                Open();
                if (IsProc(cmd))
                {
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    if ((ds != null) && (ds.Tables != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
                    {
                        retval = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                    }
                    else
                    {
                        retval = 1;
                    }
                }
                else
                {
                    // retval = cmd.ExecuteNonQuery();
                    cmd.ExecuteNonQuery();
                    retval = 1;
                }
                //处理参数
                DealWithOutParams(parameters, cmd.Parameters);

            }
            catch (OracleException ex)
            {
                throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
            }
            finally
            {
                Close();
            }
            return retval;
        }

        #region geng

        public override object ExecuteScalar(string query, List<DbParameter> parameters)
        {
            DebugOutput_SQL(query, parameters);

            query = OralceKeyReplace(query.Trim());
            OracleCommand cmd = new OracleCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
            {
                for (int i = 0; i <= parameters.Count - 1; i++)
                {
                    OracleParameter mm = GetOralceCommandParam(parameters[i]);
                    cmd.Parameters.Add(mm);
                }
            }
            OracleParameter p_cursor = AutoAddCommandParams(cmd);

            object retval;
            try
            {
                Open();
                //if (IsProc(cmd))
                //{
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if ((ds != null) && (ds.Tables != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
                {
                    retval = ds.Tables[0].Rows[0][0];
                }
                else
                {
                    retval = null;
                }
                //}
                //else  // lxt 20101108这段代码执行后无效
                //{
                // retval = cmd.ExecuteScalar();
                //}
            }
            catch (OracleException ex)
            {
                throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
            }
            finally
            {
                Close();
            }

            return retval;
        }

        public override int ExecuteNonQuery(string query, List<DbParameter> parameters)
        {
            query = OralceKeyReplace(query.Trim());
            OracleCommand cmd = new OracleCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query.Trim());
            if (parameters != null)
                for (int i = 0; i <= parameters.Count - 1; i++)
                {
                    OracleParameter mm = GetOralceCommandParam(parameters[i]);

                    cmd.Parameters.Add(mm);
                }
            // AutoAddCommandParams(cmd);
            int retval;
            try
            {
                Open();
                if (IsProc(cmd))
                {
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    if ((ds != null) && (ds.Tables != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
                    {
                        retval = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                    }
                    else
                    {
                        retval = 1;
                    }
                }
                else
                {
                    // retval = cmd.ExecuteNonQuery();
                    cmd.ExecuteNonQuery();
                    retval = 1;
                }
                //处理参数
                DealWithOutParams(parameters, cmd.Parameters);

            }
            catch (OracleException ex)
            {
                throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
            }
            finally
            {
                Close();
            }
            return retval;
        }

        #endregion

        void DealWithOutParams(DbParameter[] parameters, OracleParameterCollection outParams)
        {
            if (parameters != null)
            {
                foreach (DbParameter p in parameters)
                {
                    if ((p.Direction == ParameterDirection.InputOutput) || (p.Direction == ParameterDirection.Output))
                    {
                        string pn = OracleParamNewName(OralceKeyReplace(p.ParameterName));

                        p.Value = outParams[pn].Value;

                    }
                }
            }
        }

        void DealWithOutParams(List<DbParameter> parameters, OracleParameterCollection outParams)
        {
            if (parameters != null)
            {
                foreach (DbParameter p in parameters)
                {
                    if ((p.Direction == ParameterDirection.InputOutput) || (p.Direction == ParameterDirection.Output))
                    {
                        string pn = OracleParamNewName(OralceKeyReplace(p.ParameterName));

                        p.Value = outParams[pn].Value;

                    }
                }
            }
        }

        public override DataSet ExecuteTranDataSet(string query, params DbParameter[] parameters)
        {
            query = OralceKeyReplace(query.Trim());
            OracleCommand cmd = new OracleCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    OracleParameter mm = GetOralceCommandParam(parameters[i]);
                    cmd.Parameters.Add(mm);
                    //cmd.Parameters.Add(parameters[i]);
                }
            }
            OracleParameter p_cursor = AutoAddCommandParams(cmd);
            DataSet ds = new DataSet();
            OracleDataAdapter da = new OracleDataAdapter(cmd); ;
            if (IsTransactionInProgress)
            {
                cmd.Transaction = _tran;
                da.Fill(ds);
            }
            else
            {
                try
                {
                    Open();
                    da.Fill(ds);
                }
                catch (SqlException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                finally
                {
                    Close();
                }
            }
            return ds;
        }

        public override DataTable ExecuteTranDataTable(string query, out string sOutValue, params DbParameter[] parameters)
        {
            query = OralceKeyReplace(query.Trim());
            OracleCommand cmd = new OracleCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            OracleParameter outParameter = null;
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {

                    OracleParameter mm = GetOralceCommandParam(parameters[i]);
                    cmd.Parameters.Add(mm);
                    if ((mm.Direction == ParameterDirection.InputOutput) || (mm.Direction == ParameterDirection.Output))
                        outParameter = mm;
                    // cmd.Parameters.Add(parameters[i] as OracleParameter);
                    //else
                    //cmd.Parameters.Add(parameters[i]);
                }
            OracleParameter p_cursor = AutoAddCommandParams(cmd);
            DataSet ds = new DataSet();
            OracleDataAdapter da = new OracleDataAdapter(cmd); ;
            if (IsTransactionInProgress)
            {
                cmd.Transaction = _tran;
                da.Fill(ds);
                if (outParameter != null)
                    sOutValue = outParameter.Value.ToString();
                else
                    sOutValue = string.Empty;
            }
            else
            {
                try
                {
                    Open();
                    da.Fill(ds);
                    if (outParameter != null)
                        sOutValue = outParameter.Value.ToString();
                    else
                        sOutValue = string.Empty;
                }
                catch (SqlException ex)
                {
                    throw new Exception(string.Format("数据库操作执行失败，详细信息：{0}{1}", ex.Message, _ConnectionString));
                }
                finally
                {
                    Close();
                }
            }
            return ds.Tables[0];
        }

        public override DataTable ExecuteTranDataTable(string query, params DbParameter[] parameters)
        {
            query = OralceKeyReplace(query.Trim());

            OracleCommand cmd = new OracleCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    OracleParameter mm = GetOralceCommandParam(parameters[i]);
                    cmd.Parameters.Add(mm);
                }
            }
            OracleParameter p_cursor = AutoAddCommandParams(cmd);

            DataSet ds = new DataSet();
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            if (IsTransactionInProgress)
            {
                cmd.Transaction = _tran;
                da.Fill(ds);
            }
            else
            {
                try
                {
                    Open();
                    da.Fill(ds);
                }
                catch (SqlException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                finally
                {
                    Close();
                }
            }
            return ds.Tables[0];
        }


        public override int ExecuteTranNonQuery(string query, params DbParameter[] parameters)
        {
            query = OralceKeyReplace(query.Trim());
            OracleCommand cmd = new OracleCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query.Trim());
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    OracleParameter mm = GetOralceCommandParam(parameters[i]);
                    cmd.Parameters.Add(mm);
                    // cmd.Parameters.Add(parameters[i]);
                }
            // OracleParameter p_cursor = AutoAddCommandParams(cmd);

            if (IsTransactionInProgress)
            {
                cmd.Transaction = _tran;
                return cmd.ExecuteNonQuery();
            }
            else
            {
                int retval;
                try
                {
                    Open();

                    OracleTransaction tran = this.DBConnection.BeginTransaction();
                    try
                    {
                        cmd.Transaction = tran;
                        retval = cmd.ExecuteNonQuery();
                        tran.Commit();
                    }
                    catch (OracleException ex1)
                    {
                        tran.Rollback();
                        throw new Exception("数据库操作执行失败，详细信息：" + ex1.Message);
                    }
                }
                catch (OracleException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                finally
                {
                    Close();
                }
                return retval;
            }

        }

        public override DbDataReader ExecuteTranReader(string query, params DbParameter[] parameters)
        {
            query = OralceKeyReplace(query.Trim());
            OracleCommand cmd = new OracleCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);

            for (int i = 0; i <= parameters.Length - 1; i++)
            {
                OracleParameter mm = GetOralceCommandParam(parameters[i]);
                cmd.Parameters.Add(mm);
                // cmd.Parameters.Add(parameters[i]);
            }
            OracleParameter p_cursor = AutoAddCommandParams(cmd);

            OracleDataReader dr;
            if (IsTransactionInProgress)
            {
                cmd.Transaction = _tran;

                dr = cmd.ExecuteReader();
                return dr;
            }
            else
            {
                try
                {
                    Open();
                    dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    return dr;
                }
                catch (OracleException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                //finally
                //{
                //    Close();
                //}
            }
        }

        public override object ExecuteTranScalar(string query, params DbParameter[] parameters)
        {
            query = OralceKeyReplace(query.Trim());
            OracleCommand cmd = new OracleCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    OracleParameter mm = GetOralceCommandParam(parameters[i]);
                    cmd.Parameters.Add(mm);
                }
            }
            OracleParameter p_cursor = AutoAddCommandParams(cmd);

            object retval;
            if (IsTransactionInProgress)
            {
                cmd.Transaction = _tran;
                retval = cmd.ExecuteScalar();
            }
            else
            {
                try
                {
                    Open();
                    if (IsProc(cmd))
                    {
                        OracleDataAdapter da = new OracleDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        if ((ds != null) && (ds.Tables != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
                        {
                            retval = ds.Tables[0].Rows[0][0];
                        }
                        else
                        {
                            retval = null;
                        }
                    }
                    else
                    {
                        retval = cmd.ExecuteScalar();
                    }
                    // retval = cmd.ExecuteScalar();
                }
                catch (OracleException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                finally
                {
                    Close();
                }
            }
            return retval;
        }

        public override object ExecuteScalar(string query, params DbParameter[] parameters)
        {
            DebugOutput_SQL(query, parameters);

            query = OralceKeyReplace(query.Trim());
            OracleCommand cmd = new OracleCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    OracleParameter mm = GetOralceCommandParam(parameters[i]);
                    cmd.Parameters.Add(mm);
                }
            }
            OracleParameter p_cursor = AutoAddCommandParams(cmd);

            object retval;
            try
            {
                Open();
                //if (IsProc(cmd))
                //{
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if ((ds != null) && (ds.Tables != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
                {
                    retval = ds.Tables[0].Rows[0][0];
                }
                else
                {
                    retval = null;
                }
                //}
                //else  // lxt 20101108这段代码执行后无效
                //{
                // retval = cmd.ExecuteScalar();
                //}
            }
            catch (OracleException ex)
            {
                throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
            }
            finally
            {
                Close();
            }

            return retval;
        }
        OracleParameter AutoAddCommandParams(OracleCommand cmd)
        {
            #region
            //lxt 20101101  当调用存储过程时，自动增加一个输出参数，用作获取数据使用
            if (cmd != null)
            {
                if (IsProc(cmd))
                {
                    OracleParameter p_cursor = new OracleParameter("p_cursor", OracleDbType.RefCursor,
                                      DBNull.Value, ParameterDirection.Output);
                    cmd.Parameters.Add(p_cursor);
                    return p_cursor;
                }
            }
            return null;
            #endregion
        }
        bool IsProc(OracleCommand cmd)
        {
            return (cmd.CommandType == CommandType.StoredProcedure);
        }


        OracleDbType ConvertDataDBType(DbType sqlType)
        {

            switch (sqlType)
            {
                case DbType.String:
                case DbType.AnsiString:
                    return OracleDbType.Varchar2;
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                    return OracleDbType.Int64;
                default:
                    return OracleDbType.Varchar2;
            }



        }

        OracleDbType ConvertSqlDBType(SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case SqlDbType.Char:
                case SqlDbType.VarChar:
                case SqlDbType.NChar:
                case SqlDbType.NVarChar:
                    return OracleDbType.Varchar2;
                case SqlDbType.BigInt:
                case SqlDbType.Bit:
                case SqlDbType.Int:
                case SqlDbType.SmallInt:
                    return OracleDbType.Int64;
                default:
                    return OracleDbType.Varchar2;
            }
        }
        string OralceKeyReplace(string QueryString)
        {
            //此处考虑正则表达式来处理
            if (GetCommandType(QueryString) == CommandType.Text)
            {
                //如果是
                QueryString = QueryString.Replace("[", "\"");
                QueryString = QueryString.Replace("]", "\"");

                QueryString = QueryString.Replace("isnull", "nvl");
            }
            // QueryString = QueryString.Replace("@", "");
            QueryString = QueryString.Replace("[", "");
            QueryString = QueryString.Replace("]", "");
            return QueryString;
        }
        string OracleParamNewName(string ParamName)
        {
            ParamName = ParamName.Replace("@", "");
            if (ParamName.ToLower() == "select")
            {
                return "PSELECT";
            }
            else if (ParamName.ToLower() == "level")
            {
                return "PLEVEL";
            }

            return ParamName;
        }
        object OracleReplaceValue(object obj)
        {
            if (obj is string)
            {
                return obj.ToString().Replace("[", "").Replace("]", "");
            }
            return obj;
        }
        object OracleReplaceValue(object obj, string newstr)
        {
            if (obj is string)
            {
                return obj.ToString().Replace("[", newstr).Replace("]", newstr);
            }
            return obj;
        }

        OracleParameter GetOralceCommandParam(DbParameter Param)
        {
            if (Param is OracleParameter)
            {
                return Param as OracleParameter;
            }
            else
            {

                OracleParameter _p = new OracleParameter();

                _p.ParameterName = OracleParamNewName(OralceKeyReplace(Param.ParameterName));


                // Param.ParameterName.Replace("@", "");
                if (_p.ParameterName.ToLower() == "orderby")
                {
                    _p.Value = OracleReplaceValue(Param.Value, "\"");
                }
                else
                {
                    _p.Value = OracleReplaceValue(Param.Value);
                }
                _p.DbType = Param.DbType;
                _p.Direction = Param.Direction;

                return _p;
            }

        }

        public override DataTable ExecuteDataTable(string query, params DbParameter[] parameters)
        {
            query = OralceKeyReplace(query.Trim());
            DebugOutput_SQL(query, parameters);
            OracleCommand cmd = new OracleCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    OracleParameter mm = GetOralceCommandParam(parameters[i]);
                    cmd.Parameters.Add(mm);
                }
            }
            OracleParameter p_cursor = AutoAddCommandParams(cmd);

            DataSet ds;
            try
            {
                Open();
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
            }
            catch (OracleException ex)
            {
                throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
            }
            finally
            {
                Close();
            }

            return ds.Tables[0];
        }

        public override DataSet ExecuteDataSet(string query, params DbParameter[] parameters)
        {
            query = OralceKeyReplace(query.Trim());
            DebugOutput_SQL(query, parameters);
            OracleCommand cmd = new OracleCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    OracleParameter mm = GetOralceCommandParam(parameters[i]);
                    cmd.Parameters.Add(mm);
                }
            }
            OracleParameter p_cursor = AutoAddCommandParams(cmd);

            DataSet ds;
            try
            {
                Open();
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
            }
            catch (OracleException ex)
            {
                throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
            }
            finally
            {
                Close();
            }

            return ds;
        }

        public override DataTable ExecuteDataTable(string query, out string sOutValue, params DbParameter[] parameters)
        {
            query = OralceKeyReplace(query.Trim());
            DebugOutput_SQL(query, parameters);
            OracleCommand cmd = new OracleCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            OracleParameter outParameter = null;
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    OracleParameter mm = GetOralceCommandParam(parameters[i]);
                    cmd.Parameters.Add(mm);
                    if ((mm.Direction == ParameterDirection.InputOutput) || (mm.Direction == ParameterDirection.Output))
                    {
                        outParameter = mm;
                    }
                    //cmd.Parameters.Add(parameters[i] as OracleParameter);
                    //else
                    //    cmd.Parameters.Add(parameters[i] as OracleParameter);
                }
            OracleParameter p_cursor = AutoAddCommandParams(cmd);
            DataSet ds;
            try
            {
                Open();
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                if (outParameter != null)
                    sOutValue = outParameter.Value.ToString();
                else
                    sOutValue = string.Empty;
            }
            catch (OracleException ex)
            {
                throw new Exception(string.Format("数据库操作执行失败，详细信息：{0}{1}", ex.Message, _ConnectionString));
            }
            finally
            {
                Close();
            }

            return ds.Tables[0];
        }


        #region ** SqlDataReader

        public override DbDataReader ExecuteReader(string query, params DbParameter[] parameters)
        {
            query = OralceKeyReplace(query.Trim());
            DebugOutput_SQL(query, parameters);
            OracleCommand cmd = new OracleCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);

            for (int i = 0; i <= parameters.Length - 1; i++)
            {
                OracleParameter mm = GetOralceCommandParam(parameters[i]);
                cmd.Parameters.Add(mm);
                // cmd.Parameters.Add(parameters[i]);
            }
            OracleParameter p_cursor = AutoAddCommandParams(cmd);
            OracleDataReader dr;
            try
            {
                Open();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return dr;
            }
            catch (OracleException ex)
            {
                throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
            }
            //finally
            //{
            //    Close();
            //}
        }

        #endregion
        int GetNewID(string TableName)
        {
            string querystr = "select SEQ_" + TableName + ".nextval  from dual;";
            var iPID = ExecuteScalar(querystr);
            return int.Parse(iPID.ToString());
        }
        bool IsDataTime(string val)
        {
            if ((!string.IsNullOrEmpty(val)) && (val.Length >= 6))
            {

                DateTime dt;
                return DateTime.TryParse(val, out  dt);
            }
            else
            {
                return false;
            }

        }
        public override int DoInsert(SerializableData data, string tableName, bool needKey, List<string> ExcludeFields)
        {
            if (data.Items.Count <= 0)
                return -1;

            var sFields = "";
            var sValues = "";
            int iPID = 0;
            int tid = 0;
            if (data["ID"] != null)
            {


                bool bl = int.TryParse(data["ID"].ToString(), out tid);
                if (bl)
                {
                    //此处需要增加一个自动从配置中读取的功能
                    if (((tid > 0) && (string.Compare(tableName.ToUpper(), "WMS_STORE") == 0)))
                    {
                        needKey = false;
                    }
                    else if (((tid > 0) && (string.Compare(tableName.ToUpper(), "WMS_SEND") == 0)))
                    {
                        needKey = false;
                    }
                    else if (((tid > 0) && (string.Compare(tableName.ToUpper(), "SMS_MEMBER") == 0)))
                    {
                        needKey = false;
                    }
                }
            }

            if (needKey)
            {
                sFields = "[ID],";
                iPID = GetNewID(tableName);
                sValues = iPID.ToString() + ",";
            }
            List<DbParameter> list = new List<DbParameter>();
            data.Items.ForEach(e =>
            {
                if (!string.IsNullOrEmpty(e.K) && !e.K.EndsWith("_G") && !string.IsNullOrEmpty(e.V) && (!((e.K.ToLower() == "id") && (tid == 0))))
                {
                    string fn = e.K.ToUpper();

                    if ((ExcludeFields == null) || (!ExcludeFields.Exists((x) => { return x.ToUpper() == fn; })))
                    {
                        sFields += string.Format("[{0}],", fn);
                        sValues += string.Format(":{0},", fn);

                        // if (e.T ==TypeCode .DateTime )
                        //// if (IsDataTime (e.V )) //temp code  lxt 20101108
                        // {
                        //     sValues += string.Format("to_date('{0}','yyyy-mm-dd hh24:mi:ss'),", e.V);
                        // }
                        // else  
                        // {
                        //     list.Add(new SqlParameter(fn, e.V) as DbParameter);
                        // }
                        //        geng------
                        if (e.T == TypeCode.DateTime && e.V == DateTime.MinValue.ToString(DataItem.DATETIMEFORMAT))
                        {
                            list.Add(new OracleParameter(e.K, null) as DbParameter);
                        }
                        else
                        {
                            if (IsDataTime(e.V)) //temp code  lxt 20101108
                            {
                                string st = string.Format("to_date('{0}','yyyy-mm-dd hh24:mi:ss')", e.V);
                                sValues = sValues.Replace(":" + fn, st);
                                //sValues += string.Format("to_date('{0}','yyyy-mm-dd hh24:mi:ss'),", e.V);
                            }
                            else
                            {
                                list.Add(new OracleParameter(fn, e.V) as DbParameter);
                            }

                        }
                    }
                }
            });

            sFields = sFields.Replace('[', '"').Replace(']', '"');


            var sSql = string.Format(
                "INSERT INTO {0}({1}) VALUES({2})",
                tableName,
                sFields.Trim().TrimEnd(','),
                sValues.Trim().TrimEnd(','));

            //if (needKey)
            //{
            //    sSql = string.Format(
            //    "INSERT INTO {0}([ID],{1}) VALUES({3},{2});",
            //    tableName,
            //    sFields.Trim().TrimEnd(','),
            //    sValues.Trim().TrimEnd(','), iPID.ToString());
            //}


            //var sSql = string.Format(
            //"INSERT INTO {0}(\"{1}\") VALUES({2});",
            //tableName,
            //sFields.Trim().TrimEnd(','),
            //sValues.Trim().TrimEnd(','),
            //needKey ? "SELECT CAST(scope_identity() AS int);" : "");
            var obj = ExecuteNonQuery(sSql, list);
            // var obj = ExecuteScalar(sSql);
            return iPID;
        }

        public override int DoUpdate(SerializableData data, string tableName, string keyField, List<string> ExcludeFields)
        {
            var sSql = string.Format("UPDATE {0} SET ", tableName);
            var sql_Update = "";

            List<DbParameter> list = new List<DbParameter>();
            data.Items.ForEach(e =>
            {

                //if (!string.IsNullOrEmpty(e.K)
                //    && !e.K.EndsWith("_G")
                //    && !e.K.Equals(keyField, StringComparison.InvariantCultureIgnoreCase)
                //    && !string.IsNullOrEmpty(e.V))
                if (!string.IsNullOrEmpty(e.K)
                   && !e.K.EndsWith("_G")
                   && !e.K.Equals(keyField, StringComparison.InvariantCultureIgnoreCase)
                  )
                {
                    //如果值不为空，或者和旧值不一样，则认为需要更新；
                    //if (
                    //    (!(string.IsNullOrEmpty(e.V))) || 
                    //    (e.V.CompareTo(e.O) != 0) 
                    //    )
                    {


                        string fn = e.K.ToUpper();

                        if ((ExcludeFields == null) || (!ExcludeFields.Exists((x) => { return x.ToUpper() == fn; })))
                        {
                            //if ((e.S == EntityStatus.New || e.S == EntityStatus.Modified) && !string.IsNullOrEmpty(e.V))
                            if ((e.S == EntityStatus.New || e.S == EntityStatus.Modified))
                            {
                                //if (e.T == TypeCode.DateTime)
                                //// if (IsDataTime (e.V )) //temp code  lxt 20101108 
                                //{
                                //    sql_Update += string.Format("\"{0}\"=to_date('{1}','yyyy-mm-dd hh24:mi:ss'),", fn, e.V);
                                //}
                                //else
                                //{
                                //    sql_Update += string.Format("\"{0}\"='{1}',", fn, e.V);
                                //}
                                //geng=========================
                                sql_Update += string.Format("\"{0}\"=:{0} , ", fn);
                                if (e.T == TypeCode.DateTime && e.V == DateTime.MinValue.ToString(DataItem.DATETIMEFORMAT))
                                {
                                    list.Add(new OracleParameter(fn, null) as DbParameter);
                                }
                                else if (e.T == TypeCode.String && string.IsNullOrEmpty(e.V))
                                {
                                    list.Add(new OracleParameter(fn, null) as DbParameter);
                                }
                                else
                                {
                                    if (IsDataTime(e.V)) //temp code  lxt 20101108
                                    {
                                        string st = string.Format("to_date('{0}','yyyy-mm-dd hh24:mi:ss')", e.V);
                                        sql_Update = sql_Update.Replace(":" + fn, st);
                                    }
                                    else
                                    {
                                        list.Add(new OracleParameter(fn, e.V) as DbParameter);
                                    }

                                }
                            }
                            else if (e.S == EntityStatus.Delete)
                            {
                                sql_Update += string.Format("\"{0}\"=null,", fn);
                            }
                        }
                    }
                }
            });

            sql_Update = sql_Update.Trim().TrimEnd(',');
            if (!string.IsNullOrEmpty(sql_Update))
            {
                sSql += sql_Update;
                sSql += string.Format(" WHERE \"{0}\"=:{0}", keyField);
                list.Add(new OracleParameter(keyField, data[keyField]));
                return ExecuteNonQuery(sSql, list);
            }
            return 1;
        }

        public override int DoDelete(SerializableData data, string tableName, string keyField)
        {
            CheckForeign(tableName, keyField, data[keyField].ToString());

            var sSql = string.Format("Delete {0} where {1} =:{1}", tableName, keyField);
            return ExecuteNonQuery(sSql, new OracleParameter(keyField, data[keyField]) as DbParameter);
        }

        private void CheckForeign(string tableName, string keyField, string KeyValue)
        {
            OracleParameter[] dbpars = new OracleParameter[] {                  
                          new OracleParameter("TableName",tableName ),   
                          new OracleParameter("keyField",keyField),
                          new OracleParameter("KeyValue",KeyValue) 
                           };
            DataTable dt = ExecuteDataTable("usp_CheckForeign", dbpars);
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                string r = dt.Rows[0]["ResultID"].ToString();
                if (r == "0")
                {
                    throw new Exception(dt.Rows[0]["ErrorMsg"].ToString());
                }

            }
        }
    }



    /// <summary>
    /// 改成一个工厂，同时组合了接口
    /// 将原来的DataAccess实际变成了SqlServerDBOperate， 由此可以扩展支持OracleDBOperate，其他数据库等等；
    /// </summary>
    public class DataAccess
    {
        private IDBOperator _DBOperator = null;
        #region
        //public static int COMMANDTIMEOUT = 600;

        //private bool _isTransactionInProgress;
        //public bool IsTransactionInProgress
        //{
        //    get
        //    {
        //        return _isTransactionInProgress;
        //    }
        //}
        DataBaseType _DataBaseType = DataBaseType.SqlServer;
        public DataBaseType DataBaseType
        {
            get
            {
                return _DataBaseType;
            }
        }

        IDBOperator CreateIDBOperateBase(ConnectionStringSettings conn, bool async)
        {
            //need code 根据数据库类型创建不同的数据库操作类。
            string _strProvider = conn.ProviderName.ToUpper();
            #region  //解密连接串
            string _dconnstr = conn.ConnectionString;
            // string _dconnstr = newTemp.DecryptString(conn.ConnectionString);
            #endregion
            IDBOperator _oper = null;
            if (_strProvider.ToUpper().CompareTo("SQLCLIENT") == 0)  //Sqlserver
            {
                _oper = new SqlServerDBOperate(_dconnstr, async);
                _DataBaseType = DataBaseType.SqlServer;
            }
            else if (_strProvider.ToUpper().CompareTo("SYSTEM.DATA.ORACLECLIENT") == 0)  //oracle
            {
                _oper = new OracleDBOperate(_dconnstr, async);
                _DataBaseType = DataBaseType.Oracle;

            }
            else if (_strProvider.ToUpper().CompareTo("ORACLE.DATAACCESS.CLIENT") == 0)
            {
                _oper = new OracleDBOperate(_dconnstr, async);
                _DataBaseType = DataBaseType.Oracle;
            }
            else//默认Sqlserver
            {
                _oper = new SqlServerDBOperate(_dconnstr, async);
                _DataBaseType = DataBaseType.SqlServer;
            }
            return _oper;
        }


        public DataAccess(ConnectionStringSettings conn)
        {
            _DBOperator = CreateIDBOperateBase(conn, false);
        }



        public DataAccess(ConnectionStringSettings conn, bool async)
        {
            _DBOperator = CreateIDBOperateBase(conn, async);
        }
        public bool DirectImportData(string DataFileName, string DtName, string DtFields)
        {
            if (_DBOperator != null)
            {
                return _DBOperator.DirectImportData(DataFileName, DtName, DtFields);
            }
            return false;
        }

        public bool DirecImportData(Dictionary<string, string> dicData)
        {
            if (_DBOperator != null)
            {
                return _DBOperator.DirecImportData(dicData);
            }
            return false;
        }

        public bool DirecExportData(int AccountSetID, string ToDirectoryPath)
        {
            if (_DBOperator != null)
            {
                return _DBOperator.DirecExprotData(AccountSetID, ToDirectoryPath);
            }
            return false;
        }

        public string GetVersion()
        {
            if (_DBOperator != null)
            {
                return _DBOperator.GetVersion();
            }
            return "";
        }
        public bool BeginTran()
        {
            if (_DBOperator != null)
            {
                return _DBOperator.BeginTran();
            }
            return false;
            //if (_isTransactionInProgress)
            //    return true;
            //try
            //{
            //    this.DBConnection.Open();
            //    _tran = this.DBConnection.BeginTransaction();
            //    _isTransactionInProgress = true;
            //}
            //catch
            //{
            //    _isTransactionInProgress = false;
            //}
            //return _isTransactionInProgress;
        }

        public void CommitTran()
        {
            if (_DBOperator != null)
            {
                _DBOperator.CommitTran();
            }
        }

        public void RollBack()
        {
            if (_DBOperator != null)
            {
                _DBOperator.RollBack();
            }
            //_tran.Rollback();
            //_isTransactionInProgress = false;
            //this.DBConnection.Close();
            //_tran.Dispose();
        }



        //SqlConnection _DBConnection;
        //public SqlConnection DBConnection
        //{
        //    get
        //    {
        //        if (_DBConnection == null)
        //        {
        //            _DBConnection = new SqlConnection(_ConnectionString);
        //        }
        //        return _DBConnection;
        //    }
        //    private set { }
        //}

        public void Open()
        {
            if (_DBOperator != null)
            {
                _DBOperator.Open();
            }
            //if (DBConnection.State == ConnectionState.Closed)
            //{
            //    try
            //    {
            //        DBConnection.Open();
            //    }
            //    catch (SqlException ex)
            //    {
            //        throw new ApplicationException("数据库连接失败,详细信息如下：" + ex.Message, ex);
            //    }
            //}
        }

        public void Close()
        {
            if (_DBOperator != null)
            {
                _DBOperator.Close();
            }
            //if (DBConnection != null)
            //{
            //    if (DBConnection.State == ConnectionState.Open)
            //    {
            //        DBConnection.Close();
            //    }
            //}
        }

        public void Dispose()
        {
            if (_DBOperator != null)
            {
                _DBOperator.Dispose();
            }
            //if (DBConnection != null)
            //{
            //    DBConnection.Dispose();
            //    DBConnection = null;
            //}
        }

        public bool Test()
        {
            if (_DBOperator != null)
            {
                return _DBOperator.Test();
            }
            return false;
            //try
            //{
            //    Open();
            //    Close();
            //    Dispose();
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
        }

        public CommandType GetCommandType(string sSqlText)
        {
            if (_DBOperator != null)
            {
                return _DBOperator.GetCommandType(sSqlText);
            }
            return CommandType.StoredProcedure;
            //sSqlText = sSqlText.TrimStart();
            //if (sSqlText.StartsWith("INSERT", StringComparison.InvariantCultureIgnoreCase)
            //    || sSqlText.StartsWith("UPDATE", StringComparison.InvariantCultureIgnoreCase)
            //    || sSqlText.StartsWith("DROP", StringComparison.InvariantCultureIgnoreCase)
            //    || sSqlText.StartsWith("DELETE", StringComparison.InvariantCultureIgnoreCase)
            //    || sSqlText.StartsWith("EXEC ", StringComparison.InvariantCultureIgnoreCase)
            //    || sSqlText.StartsWith("ALTER ", StringComparison.InvariantCultureIgnoreCase)
            //    || sSqlText.StartsWith("CREATE ", StringComparison.InvariantCultureIgnoreCase)
            //    || sSqlText.StartsWith("SELECT", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    return CommandType.Text;
            //}
            //else
            //{
            //    return CommandType.StoredProcedure;
            //}
        }

        public void ExecuteNonQuery_Async(string query, Action<object, bool, int> callback, object AsyncState, params SqlParameter[] parameters)
        {
            if (_DBOperator != null)
            {
                _DBOperator.ExecuteNonQuery_Async(query, callback, AsyncState, parameters);
            }
            #region
            /*
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query.Trim());
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }

            try
            {
                Open();
                cmd.BeginExecuteNonQuery((IAsyncResult result) =>
                {
                    var sqlResult = result.IsCompleted;
                    var actionRecorderCount = EndExecuteNonQuery_Async(result);

                    if (callback != null)
                        callback(((AsyncCallArgs)result.AsyncState).State, sqlResult, actionRecorderCount);
                }, new AsyncCallArgs(AsyncState, cmd));
            }
            catch (SqlException ex)
            {
                Close();
                throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
            }
            //finally
            //{
            //    Close();
            //}
            */
            #endregion
        }
        /* lxt
        class AsyncCallArgs
        {
            public AsyncCallArgs(object userState, SqlCommand command)
            {
                State = userState;
                Command = command;
            }
            public SqlCommand Command { get; set; }
            public object State { get; set; }
        }
        )
         * */

        private int EndExecuteNonQuery_Async(IAsyncResult result)
        {
            if (_DBOperator != null)
            {
                _DBOperator.EndExecuteNonQuery_Async(result);
            }
            return 0;
            /*
            int r = 0;
            SqlCommand command = null;
            try
            {
                if (result.IsCompleted)
                {
                    command = ((AsyncCallArgs)result.AsyncState).Command;
                    r = command.EndExecuteNonQuery(result);
                }
            }
            catch (Exception ex)
            {
                //add exception log
            }
            finally
            {
                if (command != null && command.Connection != null)
                {
                    command.Connection.Close();
                    command.Dispose();
                }
            }
            return r;
             * */
        }

        public int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            if (_DBOperator != null)
            {
                return _DBOperator.ExecuteNonQuery(query, parameters);
            }
            return 0;

            /*
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query.Trim());
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }

            int retval;
            try
            {
                Open();
                retval = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
            }
            finally
            {
                Close();
            }
            return retval;
             * */
        }

        public DataSet ExecuteTranDataSet(string query, params SqlParameter[] parameters)
        {

            if (_DBOperator != null)
            {
                return _DBOperator.ExecuteTranDataSet(query, parameters);
            }
            return null;
            /*
            query = query.Trim();

            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }
            }

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd); ;
            if (_isTransactionInProgress)
            {
                cmd.Transaction = _tran;
                da.Fill(ds);
            }
            else
            {
                try
                {
                    Open();
                    da.Fill(ds);
                }
                catch (SqlException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                finally
                {
                    Close();
                }
            }
            return ds;
             * */
        }

        public DataTable ExecuteTranDataTable(string query, out string sOutValue, params SqlParameter[] parameters)
        {

            if (_DBOperator != null)
            {
                return _DBOperator.ExecuteTranDataTable(query, out   sOutValue, parameters);
            }
            sOutValue = "";
            return null;
            /*
            query = query.Trim();

            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            SqlParameter outParameter = null;
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    if (parameters[i].Direction == ParameterDirection.InputOutput)
                        outParameter = cmd.Parameters.Add(parameters[i]);
                    else
                        cmd.Parameters.Add(parameters[i]);
                }

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd); ;
            if (_isTransactionInProgress)
            {
                cmd.Transaction = _tran;
                da.Fill(ds);
                if (outParameter != null)
                    sOutValue = outParameter.Value.ToString();
                else
                    sOutValue = string.Empty;
            }
            else
            {
                try
                {
                    Open();
                    da.Fill(ds);
                    if (outParameter != null)
                        sOutValue = outParameter.Value.ToString();
                    else
                        sOutValue = string.Empty;
                }
                catch (SqlException ex)
                {
                    throw new Exception(string.Format("数据库操作执行失败，详细信息：{0}{1}", ex.Message, _ConnectionString));
                }
                finally
                {
                    Close();
                }
            }
            return ds.Tables[0];
             * */
        }

        public DataTable ExecuteTranDataTable(string query, params SqlParameter[] parameters)
        {
            if (_DBOperator != null)
            {
                return _DBOperator.ExecuteTranDataTable(query, parameters);
            }
            return null;
            /*
            query = query.Trim();

            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            if (_isTransactionInProgress)
            {
                cmd.Transaction = _tran;
                da.Fill(ds);
            }
            else
            {
                try
                {
                    Open();
                    da.Fill(ds);
                }
                catch (SqlException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                finally
                {
                    Close();
                }
            }
            return ds.Tables[0];
             * */
        }


        public int ExecuteTranNonQuery(string query, params SqlParameter[] parameters)
        {
            if (_DBOperator != null)
            {
                return _DBOperator.ExecuteTranNonQuery(query, parameters);
            }
            return 0;
            /*
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query.Trim());
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }

            if (_isTransactionInProgress)
            {
                cmd.Transaction = _tran;
                return cmd.ExecuteNonQuery();
            }
            else
            {
                int retval;
                try
                {
                    Open();
                    SqlTransaction tran = this.DBConnection.BeginTransaction();
                    try
                    {
                        cmd.Transaction = tran;
                        retval = cmd.ExecuteNonQuery();
                        tran.Commit();
                    }
                    catch (SqlException ex1)
                    {
                        tran.Rollback();
                        throw new Exception("数据库操作执行失败，详细信息：" + ex1.Message);
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                finally
                {
                    Close();
                }
                return retval;
            }
            */
        }

        public DbDataReader ExecuteTranReader(string query, params SqlParameter[] parameters)
        {
            if (_DBOperator != null)
            {
                return _DBOperator.ExecuteTranReader(query, parameters);
            }
            return null;
            /*
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);

            for (int i = 0; i <= parameters.Length - 1; i++)
            {
                cmd.Parameters.Add(parameters[i]);
            }

            SqlDataReader dr;
            if (_isTransactionInProgress)
            {
                cmd.Transaction = _tran;
                dr = cmd.ExecuteReader();
                return dr;
            }
            else
            {
                try
                {
                    Open();
                    dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    return dr;
                }
                catch (SqlException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                //finally
                //{
                //    Close();
                //}
            }
             * */
        }

        public object ExecuteTranScalar(string query, params SqlParameter[] parameters)
        {
            if (_DBOperator != null)
            {
                return _DBOperator.ExecuteTranScalar(query, parameters);
            }
            return null;
            /*
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }

            object retval;
            if (_isTransactionInProgress)
            {
                cmd.Transaction = _tran;
                retval = cmd.ExecuteScalar();
            }
            else
            {
                try
                {
                    Open();
                    retval = cmd.ExecuteScalar();
                }
                catch (SqlException ex)
                {
                    throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
                }
                finally
                {
                    Close();
                }
            }
            return retval;
             * */
        }

        public object ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            if (_DBOperator != null)
            {
                return _DBOperator.ExecuteScalar(query, parameters);
            }
            return null;
            /*
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }

            object retval;
            try
            {
                Open();
                retval = cmd.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
            }
            finally
            {
                Close();
            }

            return retval;
             * */
        }

        public DataTable ExecuteDataTable(string query, params SqlParameter[] parameters)
        {
            if (_DBOperator != null)
            {
                return _DBOperator.ExecuteDataTable(query, parameters);
            }
            return null;
            /*
            query = query.Trim();

            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }

            DataSet ds;
            try
            {
                Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
            }
            catch (SqlException ex)
            {
                throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
            }
            finally
            {
                Close();
            }

            return ds.Tables[0];
             * */
        }

        public DataSet ExecuteDataSet(string query, params SqlParameter[] parameters)
        {
            if (_DBOperator != null)
            {
                return _DBOperator.ExecuteDataSet(query, parameters);
            }
            return null;
            /*
            query = query.Trim();

            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            if (parameters != null)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }
            }

            DataSet ds;
            try
            {
                Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
            }
            catch (SqlException ex)
            {
                throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
            }
            finally
            {
                Close();
            }

            return ds;
             * */
        }

        public DataTable ExecuteDataTable(string query, out string sOutValue, params SqlParameter[] parameters)
        {

            if (_DBOperator != null)
            {
                return _DBOperator.ExecuteDataTable(query, out sOutValue, parameters);
            }
            sOutValue = "";
            return null;
            /*
            query = query.Trim();

            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);
            SqlParameter outParameter = null;
            if (parameters != null)
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    if (parameters[i].Direction == ParameterDirection.InputOutput)
                        outParameter = cmd.Parameters.Add(parameters[i]);
                    else
                        cmd.Parameters.Add(parameters[i]);
                }

            DataSet ds;
            try
            {
                Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                if (outParameter != null)
                    sOutValue = outParameter.Value.ToString();
                else
                    sOutValue = string.Empty;
            }
            catch (SqlException ex)
            {
                throw new Exception(string.Format("数据库操作执行失败，详细信息：{0}{1}", ex.Message, _ConnectionString));
            }
            finally
            {
                Close();
            }

            return ds.Tables[0];
             * */
        }

        #endregion
        #region ** SqlDataReader

        public DbDataReader ExecuteReader(string query, params SqlParameter[] parameters)
        {
            if (_DBOperator != null)
            {
                return _DBOperator.ExecuteReader(query, parameters);
            }
            return null;
            /*
            SqlCommand cmd = new SqlCommand(query, DBConnection);
            cmd.CommandTimeout = COMMANDTIMEOUT;
            cmd.CommandType = GetCommandType(query);

            for (int i = 0; i <= parameters.Length - 1; i++)
            {
                cmd.Parameters.Add(parameters[i]);
            }

            SqlDataReader dr;
            try
            {
                Open();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return dr;
            }
            catch (SqlException ex)
            {
                throw new Exception("数据库操作执行失败，详细信息：" + ex.Message);
            }
            //finally
            //{
            //    Close();
            //}
             * */
        }


        public int DoInsert(SerializableData data, string tableName, bool needKey, List<string> ExcludeFields)
        {
            if (_DBOperator != null)
            {
                return _DBOperator.DoInsert(data, tableName, needKey, ExcludeFields);
            }
            return 0;
        }

        public int DoUpdate(SerializableData data, string tableName, string keyField, List<string> ExcludeFields)
        {
            if (_DBOperator != null)
            {
                return _DBOperator.DoUpdate(data, tableName, keyField, ExcludeFields);
            }
            return 0;
        }

        public int DoDelete(SerializableData data, string tableName, string keyField)
        {
            if (_DBOperator != null)
            {
                return _DBOperator.DoDelete(data, tableName, keyField);
            }
            return 0;
        }

        #endregion
    }
}