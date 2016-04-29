using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Configuration;
using NM.Log;
using NM.Util;
using System.Data.SqlClient;
using NM.Model;
using System.Data.Common;
using System.Configuration;
using NM.Lib;
using System.Text;

namespace NM.OP
{
    public class FieldMapping
    {
        public FieldMapping()
        {
            Index = -1;
        }

        public string FieldName { get; set; }
        public string PropertyName { get; set; }
        public bool IgnoreCase { get; set; }
        public object DefaultValue { get; set; }
        public int Index { get; set; }
        internal PropertyInfo PropertyInfo { get; set; }
    }

    public class FieldMappings
    {
        public FieldMappings(Type type)
        {
            EntityType = type;
            _Properties = new Dictionary<string, FieldMapping>();
        }

        public Type EntityType { get; private set; }

        private Dictionary<string, FieldMapping> _Properties;

        public Dictionary<string, FieldMapping> Properties
        {
            get
            {
                return _Properties;
            }
        }

        public MappingOption Options { get; set; }

        public void FillProperty(DataTable dbTable)
        {
            List<string> toRemove = new List<string>();
            foreach (var item in _Properties)
            {
                if (item.Value.Index >= 0 && item.Value.Index < dbTable.Columns.Count)
                    item.Value.FieldName = dbTable.Columns[item.Value.Index].ColumnName;
                else
                {
                    if (!string.IsNullOrEmpty(item.Value.FieldName) && dbTable.Columns.IndexOf(item.Value.FieldName) < 0)
                        toRemove.Add(item.Key);
                }
            }

            toRemove.ForEach(e => { _Properties.Remove(e); });

            if (Options == MappingOption.AllowMegerAll)
                foreach (DataColumn itme in dbTable.Columns)
                {
                    AddMapp(itme.ColumnName, itme.ColumnName, false, itme.DefaultValue);
                }
        }

        public bool AddMapp(FieldMapping map)
        {
            bool result = false;
            if (!_Properties.ContainsKey(map.PropertyName))
            {
               // map.PropertyInfo = EntityType.GetProperty(map.PropertyName);
                map.PropertyInfo = SerializableData.GetObjPro(map.PropertyName, EntityType);
                if (null != map.PropertyInfo)
                {
                    _Properties.Add(map.PropertyName, map);
                    result = true;
                }
            }
            return result;
        }

        public bool AddMapp(string propertyName, int index, object defaultValue)
        {
            return AddMapp(new FieldMapping() { PropertyName = propertyName, Index = index, DefaultValue = defaultValue });
        }

        public bool AddMapp(string propertyName, int index)
        {
            return AddMapp(propertyName, index, null);
        }

        public bool AddMapp(string propertyName, string fieldName, bool ignorCase, object defaultValue)
        {
            return AddMapp(new FieldMapping() { PropertyName = propertyName, FieldName = fieldName, IgnoreCase = ignorCase, DefaultValue = defaultValue });
        }

        public bool AddMapp(string propertyName, string fieldName, bool ignorCase)
        {
            return AddMapp(propertyName, fieldName, ignorCase, null);
        }
    }

    public enum MappingOption
    {
        AllowMegerAll = 0,
        ThisIsAll = 1,
    }

    public class DataProvider
    {
        // Static instace of this class
        //public static readonly DataProvider Default = GetProvider("");

        static Dictionary<string, DataProvider> Providers;
        public string ConnectionString { get; private set; }

        public ConnectionStringSettings _Conn { get; set; }
        static object lockobject = new object();
        private DataAccess _tranDal;
        public DataBaseType DataBaseType
        {
            get
            {
                if (_tranDal != null)
                {
                    return _tranDal.DataBaseType;
                }
                return DataBaseType.SqlServer;
            }
        }

        public string ParamTag
        {
            get
            {
                if (DataBaseType == DataBaseType.Oracle)
                {
                    return ":";
                }
                return "@";
            }
        }
        /// <summary>
        /// 利用数据库特定的工具直接将文件文件导入到数据库的表中
        /// </summary>
        /// <param name="DataFileName">数据文件全路径文件名</param>
        /// <param name="DtName">表名</param>
        /// <param name="DtFields">字段名，中间用，分割</param>
        /// <returns></returns>
        public bool DirectImportData(string DataFileName, string DtName,string DtFields)
        {

            return _tranDal.DirectImportData(DataFileName, DtName,DtFields);
        }

        public bool DirecImportData(Dictionary<string,string> dicData)
        {
            return _tranDal.DirecImportData(dicData);
        }

        public bool DirecExporeData(int AccountSetID,string ToDirectotyName)
        {
            return _tranDal.DirecExportData(AccountSetID, ToDirectotyName);
        }

        public bool BeginTran()
        {
            return _tranDal.BeginTran();
        }

        public void CommitTran()
        {
            _tranDal.CommitTran();
        }

        public void RollBack()
        {
            _tranDal.RollBack();
        }


        public int DoInsert(SerializableData data, string tableName, bool needKey)
        {
            if (_tranDal != null)
            {
                return _tranDal.DoInsert(data, tableName, needKey,null);
            }
            return 0;


        }
        public int DoInsert(SerializableData data, string tableName, bool needKey, List<string> ExcludeFields)
        {
            if (_tranDal != null)
            {
                return _tranDal.DoInsert(data, tableName, needKey, ExcludeFields);
            }
            return 0;
        }

        public int DoUpdate(SerializableData data, string tableName, string keyField, List<string> ExcludeFields)
        {
            if (_tranDal != null)
            {
                return _tranDal.DoUpdate(data, tableName, keyField,ExcludeFields);
            }
            return 0;
        }

        public int DoDelete(SerializableData data, string tableName, string keyField)
        {
            if (_tranDal != null)
            {
                return _tranDal.DoDelete(data, tableName, keyField);
            }
            return 0;
        }

        public static DataProvider GetProvider(string sConnKey)
        {
            lock (lockobject)
            {
                var sConn = "";

                if (string.IsNullOrEmpty(sConnKey) || sConnKey == "Default")
                {
                    sConnKey = "Default";

                }

                ConnectionStringSettings conn = WebConfigurationManager.ConnectionStrings[sConnKey];
                sConn = conn.ConnectionString;

                if (Providers == null)
                {
                    Providers = new Dictionary<string, DataProvider>();
                }
                /*
                if (!Providers.ContainsKey(sConnKey))
                {
                    Providers.Add(sConnKey, new DataProvider(conn));
                }
                return Providers[sConnKey];
                 */
                DataProvider d = new DataProvider(conn);
                return d;
            }
        }

        public static DataProvider GetEAP_Provider()
        {
            lock (lockobject)
            {
                var sConnKey = "ConnectionString_EAP";
                ConnectionStringSettings conn = WebConfigurationManager.ConnectionStrings[sConnKey];
                var sConn = conn.ConnectionString;

                if (string.IsNullOrEmpty(sConn))
                {
                    throw new ApplicationException("EAP Database Connection is not correct ");
                }

                if (Providers == null)
                {
                    Providers = new Dictionary<string, DataProvider>();
                }

                /*
                if (!Providers.ContainsKey(sConnKey))
                {
                    Providers.Add(sConnKey, new DataProvider(conn));
                }
                return Providers[sConnKey];
                 */
                DataProvider d = new DataProvider(conn);
                return d;
            }
        }

        //private DataProvider(string conn)
        private DataProvider(ConnectionStringSettings conn)
        {
            _Conn = conn;

            ConnectionString = conn.ConnectionString;

            _tranDal = new DataAccess(conn);
        }
        //private DataProvider(string conn)
        //{
        //    ConnectionString = conn;
        //    _tranDal = new DataAccess(ConnectionString);
        //}

        public string GetVersion()
        {
            var dal = new DataAccess(_Conn);
            return dal.GetVersion();
           
        }

        #region ** Operate DataBase

        public DataTable LoadTable(string sSql, params SqlParameter[] parameters)
        {
            DataTable result = null;
            if (!string.IsNullOrEmpty(sSql))
            {
                var dal = new DataAccess(_Conn);
                DebugOutput_SQL(sSql, parameters);
                result = dal.ExecuteDataTable(sSql, parameters);
            }
            return result;
        }

        public string BuildParamSearchSql(string sSelect, string FromWhere, string OrderBy, string keyField, int PageSize, int PageIndex,bool ispage)
        {
             
            int  varStartNum=PageIndex*PageSize+1;
  
            int varEndNum  = (PageIndex+1)*PageSize;

            string varsql ="select " +sSelect + "   " + FromWhere + "    " + OrderBy;
            if (ispage)
            {
                if (DataBaseType == DataBaseType.SqlServer)
                {
                    varsql = "Select sp.* FROM (Select ROW_NUMBER() OVER ("+OrderBy+") AS row_pos_id,"
                               +sSelect+ "    " + FromWhere+") AS sp Where sp.row_pos_id  BETWEEN   "+varStartNum.ToString()
                             + "  and " + varEndNum.ToString();
                }
                else if (DataBaseType == DataBaseType.Oracle)
                {
                 
                  varsql=  "SELECT * FROM ( select newtable.*,rownum as rn  from  (select " + sSelect + "  " + FromWhere + "    " + OrderBy
                      + "  )  newtable )　WHERE rn >= " + varStartNum.ToString() + " and rn<=" + varEndNum.ToString();
                }
            }        
           
            return varsql;
        
        }


        public IDataReader LoadDataReader(string sSql, params SqlParameter[] parameters)
        {
            IDataReader result = null;
            if (!string.IsNullOrEmpty(sSql))
            {
                var dal = new DataAccess(_Conn);
                DebugOutput_SQL(sSql, parameters);
                result = dal.ExecuteReader(sSql, parameters);
            }

            return result;
        }

        public DataTable LoadTable(string sSql, out string sOutValue, params SqlParameter[] parameters)
        {
            DataTable result = null;
            if (!string.IsNullOrEmpty(sSql))
            {
                var dal = new DataAccess(_Conn);
                DebugOutput_SQL(sSql, parameters);
                result = dal.ExecuteDataTable(sSql, out sOutValue, parameters);
            }
            else
            {
                sOutValue = string.Empty;
            }

            return result;
        }

        public T ExecuteScalar<T>(string sSql, params SqlParameter[] parameters)
        {
            if (!string.IsNullOrEmpty(sSql))
            {

                var dal = new DataAccess(_Conn);
                DebugOutput_SQL(sSql, parameters);
                object value = null;
                value = dal.ExecuteScalar(sSql, parameters);

                if (value != null && value != DBNull.Value)
                {
                    
                    return (T)value;
                }
                   
            }
            return default(T);
        }

        public DataSet ExecuteDataSet(string strSQL, params SqlParameter[] parameters)
        {
            DebugOutput_SQL(strSQL, parameters);
            DataSet result = null;
            if (!string.IsNullOrEmpty(strSQL))
            {
                var dal = new DataAccess(_Conn);
                result = dal.ExecuteDataSet(strSQL, parameters);
            }
            return result;
        }

        public void ExecuteNonQuery_Async(string strSQL, object AsyncState, Action<object, bool, int> callback, params SqlParameter[] parameters)
        {
            DebugOutput_SQL(strSQL, parameters);
            if (!string.IsNullOrEmpty(strSQL))
            {
                var dal = new DataAccess(_Conn, true);
                dal.ExecuteNonQuery_Async(strSQL, callback, AsyncState, parameters);
            }
        }

        public int ExecuteNonQuery(string strSQL, params SqlParameter[] parameters)
        {
            DebugOutput_SQL(strSQL, parameters);
            int result = 0;
            if (!string.IsNullOrEmpty(strSQL))
            {
                var dal = new DataAccess(_Conn);
                result = dal.ExecuteNonQuery(strSQL, parameters);
            }
            return result;
        }
        public DataSet ExecuteTranDataSet(string strSQL, params SqlParameter[] parameters)
        {
            DebugOutput_SQL(strSQL, parameters);
            DataSet result = null;
            if (!string.IsNullOrEmpty(strSQL))
            {
                result = _tranDal.ExecuteTranDataSet(strSQL, parameters);
            }
            return result;
        }
        /// <summary>
        /// 带有事务处理的方法
        /// </summary>
        /// <param name="strSQL"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteTranNonQuery(string strSQL, params SqlParameter[] parameters)
        {
            DebugOutput_SQL(strSQL, parameters);
            int result = 0;
            if (!string.IsNullOrEmpty(strSQL))
            {
                result = _tranDal.ExecuteTranNonQuery(strSQL, parameters);
            }
            return result;
        }

        public T ExecuteTranScalar<T>(string sSql, params SqlParameter[] parameters)
        {
            if (!string.IsNullOrEmpty(sSql))
            {
                DebugOutput_SQL(sSql, parameters);
                object value = null;
                value = _tranDal.ExecuteTranScalar(sSql, parameters);

                if (value != null && value != DBNull.Value)
                    return (T)value;
            }
            return default(T);
        }

        void DebugOutput_SQL(string sSql, params SqlParameter[] parameters)
        {
            return;
            var outSQL = sSql;
            foreach (var p in parameters)
                outSQL += string.Format(" {0}='{1}',", p.ParameterName, p.Value);
            if (outSQL.EndsWith(","))
                outSQL = outSQL.Remove(outSQL.Length - 1);
            LogManager.DebugIt(outSQL);
        }
        //public T ExecSQL(string sSql, params SqlParameter[] parameters)
        //{
        //    LogManager.DebugIt(sSql);
        //}


        #endregion

        #region ** Base OP

        public int MaxCount = 60;

        public List<T> LoadData<T>(DataTable tab, FieldMappings maps)
        {
            var result = new List<T>();
            if (tab != null)
            {
                maps.FillProperty(tab);

                foreach (DataRow row in tab.Rows)
                {
                    T newObject = (T)Activator.CreateInstance(maps.EntityType);
                    foreach (var item in maps.Properties)
                    {
                        object objValue = null;
                        if (item.Value.Index >= 0 && item.Value.Index < tab.Columns.Count)
                            objValue = row[item.Value.Index];
                        else if (!string.IsNullOrEmpty(item.Value.FieldName))
                            objValue = row[item.Value.FieldName];

                        SetPropertyValue(newObject, item.Value.PropertyInfo, objValue);
                    }
                    result.Add(newObject);
                }
            }
            return result;
        }

        public static List<T> LoadData<T>(DataTable tab)
        {
            var result = new List<T>();
            if (tab != null)
            {
                foreach (DataRow item in tab.Rows)
                {
                    result.Add(LoadRecord<T>(item));
                    //   if (result.Count >= MaxCount) break;
                }
            }
            return result;
        }

        public T GetEntity<T>(string sql, params SqlParameter[] parameters)
        {
            T result = default(T);
            var t = LoadData<T>(sql, parameters);
            if (t.Count > 0)
                result = t[0];
            return result;
        }

        public T GetEntity<T>(string sql, FieldMappings maps, params SqlParameter[] parameters)
        {
            T result = default(T);
            List<T> entities = LoadData<T>(sql, maps, parameters);
            if (entities.Count > 0)
                result = entities[0];
            return result;
        }

        public List<T> LoadData<T>(string sql, out string sOutValue, params SqlParameter[] parameters)
        {
            var _dt = LoadTable(sql, out sOutValue, parameters);
            return LoadData<T>(_dt);
        }

        public List<T> LoadData<T>(string sql, params SqlParameter[] parameters)
        {
            var _dt = LoadTable(sql, parameters);
            
            return LoadData<T>(_dt);
        }

        public List<T> LoadData<T>(string sql, FieldMappings maps, params SqlParameter[] parameters)
        {
            return LoadData<T>(LoadTable(sql), maps);
        }


        private static T LoadRecord<T>(DataRow row)
        {
            Type EntityType = typeof(T);
            T newObject = (T)Activator.CreateInstance(EntityType);
            foreach (DataColumn itme in row.Table.Columns)
            {
                var objValue = row[itme.ColumnName];
                if (objValue != null && objValue != DBNull.Value)
                {
                    /*lxt 20140925 原来的代码可能有问题，先让程序运行
                    //var Property = EntityType.GetProperty(itme.ColumnName,BindingFlags.IgnoreCase );
                    //PropertyInfo Property = SerializableData.GetObjPro(itme.ColumnName, EntityType);
                    //if (Property != null)
                    //{
                    //    SetPropertyValue(newObject, Property, objValue);
                    //}
                    //else
                    //{
                    //    if (newObject is EntityBase)
                    //    {
                    //        (newObject as EntityBase)[itme.ColumnName] = objValue;
                    //    }
                    //}
                     */
                    if (EntityType == typeof(EntityBase))
                    {
                        (newObject as EntityBase)[itme.ColumnName] = objValue;
                    }
                    else
                    {
                        PropertyInfo Property = SerializableData.GetObjPro(itme.ColumnName, EntityType);
                        DataProvider.SetPropertyValue(newObject, Property, objValue);
                    }
                }
            }
            if (newObject is IEntity)
            {
                (newObject as IEntity).ResetStatus(EntityStatus.Original);
            }
            return newObject;
        }

        static void SetPropertyValue(object obj, PropertyInfo Property, object objValue)
        {
            if (objValue != null && objValue != DBNull.Value && null != Property && Property.CanWrite)
            {
                if (!Property.PropertyType.Equals(objValue.GetType()))
                    objValue = Convert.ChangeType(objValue, Property.PropertyType);
                SerializableData.IsDataLoading = true;
                Property.SetValue(obj, objValue, null);
                SerializableData.IsDataLoading = false;
            }
        }

        public SerializableData LoadSerializableData(string strSQL, params SqlParameter[] paramters)
        {
            var result = new SerializableData();
            var table = LoadTable(strSQL, paramters);
            if (table.Rows.Count > 0)
                result = LoadSerializableData(table.Rows[0]);
            return result;
        }

        public SerializableData LoadSerializableData(DataRow row)
        {
            var result = new SerializableData();
            foreach (DataColumn item in row.Table.Columns)
            {
                object objValue = row[item.ColumnName];
                if (objValue != null && objValue != DBNull.Value)
                    result[item.ColumnName] = objValue;
            }

            return result;
        }

        public List<SerializableData> LoadSerializableDataCollection(string strSQL, params SqlParameter[] paramters)
        {
            DataTable table = LoadTable(strSQL, paramters);
            return (from DataRow row in table.Rows select LoadSerializableData(row)).ToList();
        }


        #endregion
    }
}
