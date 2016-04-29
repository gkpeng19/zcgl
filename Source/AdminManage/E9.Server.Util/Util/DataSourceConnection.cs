using System;
using System.Net;
using NM.Util;
using System.Collections.Generic;

namespace NM.Util
{
    public class DataSourceConnection : TJson
    {
        public DataSourceConnection()
        {
        }

        public string Server { get; set; }

        public string DataBase { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public bool Integrated_SecurityPassword { get; set; }

        public string KeyName { get { return Server + DataBase; } }

        string _ConnectionString;

        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_ConnectionString))
                    _ConnectionString = string.Format(" Data Source={0}; Initial Catalog={1}; User ID={2}; Password={3};", Server.ToLower(), DataBase.ToLower(), User.ToLower(), Password);
                return _ConnectionString;
            }
            set
            {
                _ConnectionString = value.ToLower();
            }
        }

        public string Text
        {
            get
            {
                return string.Format("{0}.{1}[{2}]", Server, DataBase, User);
            }
        }         
    }


    public class DataBaseConnections : TJsonList<DataSourceConnection>
    {
        public static string FileName = "MyConnections.UserSetting";
        public DataBaseConnections()
        {

        }
    }
}
