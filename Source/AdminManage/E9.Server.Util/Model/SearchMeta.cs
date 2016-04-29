using System;
using System.Linq;
using System.Collections.Generic;
using NM.Util;
using System.Text;
using System.Reflection;
using NM.OP;
using System.Data.SqlClient;

namespace NM.Model
{
    public class SearchField
    {
        public SearchField()
        {

        }
        public string ID { get; set; }
        public string SearchId { get; set; }
        public string FieldName { get; set; }
        public string DataMember { get; set; }
        public string DataType { get; set; }
        public string Operation { get; set; }
        public string SqlFormat { get; set; }
        public string Sort { get; set; }
        public string Leading { get; set; }
        public string Trailing { get; set; }
        public string Note { get; set; }
        public string AddBy { get; set; }
        public string AddOn { get; set; }
        public string FlagDelete { get; set; }

        public void FreshFormate()
        {
            if (string.IsNullOrEmpty(SqlFormat))
            {
                string op = Operation.ToLower();
                switch (DataType.Trim().ToLower())
                {
                    case "number":
                        if (op == "in")//todo
                            SqlFormat = "{0} in({1}) ";
                        else if (op == "between")
                            SqlFormat = "{0} between  {1} and {2}) ";
                        else
                            SqlFormat = "{0}" + op + " {1} ";
                        break;
                    case "datetime":
                        if (op == "in")//todo
                            SqlFormat = "{0} in('{1}') ";
                        else if (op == "between")
                            SqlFormat = "{0} between  '{1}' and '{2}') ";
                        else
                            SqlFormat = "{0}" + op + " '{1}' ";
                        break;
                    case "string":
                    default:
                        if (Operation == "=")
                            SqlFormat = "{0} = '{1}' ";
                        else if (op == "like")
                            SqlFormat = "{0} like '%{1}%' ";
                        else if (op == "leftlike")
                            SqlFormat = "{0}  like '%{1}' ";
                        else if (op == "rightlike")
                            SqlFormat = "{0} like '{1}%' ";
                        else if (op == "in")//todo
                            SqlFormat = "{0} in('{1}') ";
                        else if (op == "between")
                            SqlFormat = "{0} between  '{1}' and '{2}') ";
                        else
                            SqlFormat = "{0}" + op + " '{1}' ";
                        break;
                }

            }
        }
    }

    public class SearchMetaSubProperty
    {
        public SearchMetaSubProperty()
        {
            FieldMatch = new List<KeyValuePair<string, string>>();
        }

        public SearchMetaSubProperty(string express)
            : this()
        {
            Parse(express);
        }

        public string SearchID { get; set; }
        public string PropertyName { get; set; }
        public PropertyInfo SubProperty { get; set; }
        public Type PropertyType { get; set; }
        public Type PropertyGenericType { get; set; }

        public List<KeyValuePair<string, string>> FieldMatch { get; set; }

        public void Parse(string express)
        {
            //searchID[p<=(f1=f2,f3=f4)]
            string p = express.Replace("[", ",").Replace(")]", "").Replace("<=(", ",").Replace("=", ",");
            string[] parties = p.Split(",".ToCharArray());
            if (parties.Length >= 4 && parties.Length % 2 == 0)
            {
                SearchID = parties[0];
                PropertyName = parties[1];
                for (int index = 2; index < parties.Length; index += 2)
                {
                    FieldMatch.Add(new KeyValuePair<string, string>(parties[index], parties[index + 1]));
                }
            }
        }
    }

    public class EAP_ErrorMsg
    {

        public int id { get; set; } 
        public string ErrorName { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorType { get; set; }
    }

    public class SearchMeta
    {
        public SearchMeta()
        {
            Fields = new Dictionary<string, SearchField>();
            SubPropertyMeta = new List<SearchMetaSubProperty>();
        }

        public string SearchId { get; set; }
        public string ID { get; set; }
        public string Sql_Select { get; set; }
        public string Sql_From { get; set; }
        public string Sql_OrderBy { get; set; }
        public string SQL_Pro { get; set; }
        public string KeyField { get; set; }
        public string ClassOfOP { get; set; }
        private string _SubSearch;
        public string SubSearch
        {
            get { return _SubSearch; }
            set
            {
                _SubSearch = value;
                ParseSubSearch(value);
            }
        }

        public int PageSize { get; set; }
        public string Note { get; set; }
        public string AddBy { get; set; }
        public string AddOn { get; set; }
        public string FlagDelete { get; set; }
        public List<SearchMetaSubProperty> SubPropertyMeta { get; set; }

        Dictionary<string, SearchField> Fields;

        private void ParseSubSearch(string subSearch)
        {
            if (!string.IsNullOrEmpty(subSearch))
            {
                foreach (string s in subSearch.Split(";".ToCharArray()))
                {
                    SearchMetaSubProperty subProperty = new SearchMetaSubProperty(s);
                    if (subProperty.FieldMatch.Count > 0)
                    {
                        SubPropertyMeta.Add(subProperty);
                    }
                }
            }
        }

        public void CheckSubProperty(Type mainType)
        {
            for (int index = SubPropertyMeta.Count - 1; index >= 0; index--)
            {
                SearchMetaSubProperty sub = SubPropertyMeta[index];
                bool keep = false;

                //PropertyInfo pi = mainType.GetProperty(sub.PropertyName);
                PropertyInfo pi = SerializableData.GetObjPro(sub.PropertyName, mainType);
                if (pi != null && pi.PropertyType.IsGenericType)
                {
                    sub.SubProperty = pi;
                    sub.PropertyType = pi.PropertyType;

                    foreach (Type tParam in pi.PropertyType.GetGenericArguments())
                    {
                        if (tParam.IsSubclassOf(typeof(SerializableData)))
                        {
                            sub.PropertyGenericType = tParam;
                            keep = true;
                            break;
                        }
                    }
                }

                if (!keep)
                {
                    SubPropertyMeta.RemoveAt(index);
                }
            }
        }

        public void Add(IEnumerable<SearchField> fields)
        {
            var sourtFiels = from c in fields
                             orderby c.Sort
                             select c;

            foreach (var item in sourtFiels)
            {
                string fieldName = item.DataMember.ToUpper();

                if (!Fields.ContainsKey(fieldName))
                {
                    Fields.Add(fieldName, item);
                }
            }
        }

        public SearchField this[string dataMember]
        {
            get
            {
                string id = dataMember.ToUpper();
                if (Fields.ContainsKey(id))
                {
                    return Fields[id];
                }
                else
                    return null;
            }
        }

        public string ToSelect()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(SQL_Pro))
            {
                sb.AppendFormat(" exec {0} ", SQL_Pro);
            }
            else
            {
                sb.AppendFormat("  {0}  ", Sql_Select);
            }
            return sb.ToString();
        }

        public string ToWhereWithParam(List<DataItem> datas, out List<SqlParameter> sqlParams,string preParam)
        {
            sqlParams = new List<SqlParameter>();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" from {0} where 1=1 ", Sql_From);

            foreach (var item in datas)
            {
                if (string.IsNullOrEmpty(item.V)) continue;

                SearchField fieldMeta = this[item.K];

                if (fieldMeta == null) continue;

                sb.AppendFormat(" {0} ", fieldMeta.Leading); //前缀

                //fieldMeta.FreshFormate();

                if (!string.IsNullOrEmpty(fieldMeta.SqlFormat))
                {
                    #region
                    //lxt  141128 加入参数化支持的功能；主要是通过拼接字符构造动态的参数化sql语句；
                    StringBuilder _sbparams = new StringBuilder();
                    string op = fieldMeta.Operation.ToLower();
                    string _ftype = fieldMeta.DataType.Trim().ToLower();
                    
                    switch (op)
                    {
                        case "in":
                            string[] _arrayv = item.V.Split(',');
                            int k = 0;
                            #region
                            foreach (string _av in _arrayv)
                            {
                                if (!string.IsNullOrEmpty(_av))
                                {
                                    k++;
                                    string _pname = fieldMeta.DataMember + "_tp_" + k.ToString();
                                    _sbparams.Append(preParam + _pname + ",");
                                    object _pv = null;
                                    if (_ftype == "int")
                                    {
                                        _pv = int.Parse(_av);
                                    }
                                    else if (_ftype == "string")
                                    {
                                        _pv = _av;
                                    };
                                    sqlParams.Add(new SqlParameter(_pname, _av));

                                }

                            }
                            string _newsql = _sbparams.ToString();
                            if (_newsql.Length >= 1)
                            {
                                _newsql = _newsql.Substring(0, _newsql.Length - 1);
                            }
                            #endregion
                            //采用参数化数据替换原来直接用值的方式
                            sb.AppendFormat(fieldMeta.SqlFormat, fieldMeta.FieldName, _newsql);
                            break;
                        default:                          
                            
                            string _newsqlformat = fieldMeta.SqlFormat;
                            string _newv = item.V;
                            #region  处理字符中的 '%{1}%'  '{1}'  '%{1}'   '{1}%'//分项解析 '%{     }%'
                            int _t = fieldMeta.SqlFormat.IndexOf("'%{");
                            if (_t >= 0)
                            {
                                _newsqlformat = _newsqlformat.Replace("'%{", "{");
                                _newv = "%" + _newv;
                            }
                            _t = fieldMeta.SqlFormat.IndexOf("'{");
                            if (_t >= 0)
                            {
                                _newsqlformat = _newsqlformat.Replace("'{", "{");
                            }

                            _t = fieldMeta.SqlFormat.IndexOf("}%'");
                            if (_t >= 0)
                            {
                                _newsqlformat = _newsqlformat.Replace("}%'", "}");
                                _newv = _newv + "%";
                            }
                            _t = fieldMeta.SqlFormat.IndexOf("}'");
                            if (_t >= 0)
                            {
                                _newsqlformat = _newsqlformat.Replace("}'", "}");
                            }
                            #endregion
                            string _pname1 = preParam + fieldMeta.DataMember;
                             object _pv1 = null;
                                    if (_ftype == "int")
                                    {
                                        _pv1 = int.Parse(_newv);
                                    }
                                    else if (_ftype == "string")
                                    {
                                        _pv1 = _newv;
                                    };

                            sb.AppendFormat(_newsqlformat, fieldMeta.FieldName, _pname1);
                            sqlParams.Add(new SqlParameter(fieldMeta.DataMember, _pv1));
                            break;

                    }
                    #endregion 
                }
                else
                {
                    sb.Append(fieldMeta.Operation);
                }

                sb.AppendFormat(" {0} ", fieldMeta.Trailing); //后缀
            }

            return sb.ToString();

        }

        public string ToWhere(List<DataItem> datas)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" from {0} where 1=1 ", Sql_From);

            foreach (var item in datas)
            {
                if (string.IsNullOrEmpty(item.V)) continue;

                SearchField fieldMeta = this[item.K];

                if (fieldMeta == null) continue;

                sb.AppendFormat(" {0} ", fieldMeta.Leading); //前缀

                fieldMeta.FreshFormate();

                if (!string.IsNullOrEmpty(fieldMeta.SqlFormat))
                {
                      
                   //liangxt 20110916 sb.AppendFormat(fieldMeta.SqlFormat, "[" + fieldMeta.FieldName + "]", item.V);
                    sb.AppendFormat(fieldMeta.SqlFormat, fieldMeta.FieldName , item.V);
                }
                else
                {
                    sb.Append(fieldMeta.Operation);
                }

                sb.AppendFormat(" {0} ", fieldMeta.Trailing); //后缀
            }

            //if (!string.IsNullOrEmpty(Sql_OderBy))
            //{
            //    sb.AppendFormat("order by {0}", Sql_OderBy);
            //}
            return sb.ToString();
        }

        public string ToOrderBy()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(this.Sql_OrderBy))
            {
                sb.AppendFormat(" ORDER BY {0} ", Sql_OrderBy);
            }

            return sb.ToString();
        }

        public string ToSQL(List<DataItem> datas)
        {
            return string.Format("select {0} {1}", ToSelect(), ToWhere(datas));
        }
    }
}
