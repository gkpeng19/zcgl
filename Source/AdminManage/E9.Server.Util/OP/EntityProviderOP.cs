using System;
using NM.Model;
using NM.Util;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using NM.Lib;
using System.Text.RegularExpressions;

namespace NM.OP
{
    public class EntityProviderOP : CertifiedProviderOP
    {
        public EntityProviderOP(LoginInfo user, DataProvider dp)
            : base(user, dp)
        {
            
        }
        /// <summary>
        /// 删除操作(支持多表)
        /// </summary>
        /// <param name="tables">表名集合(字符串形式)</param>
        /// <param name="delkeys">对应主键集合(字符串形式)</param>
        /// <param name="swhere">主键值集合(字符串形式)</param>
        /// <returns>CommandResult对象</returns>
        public CommandResult DeleteObject(string tables, string delkeys, string swhere)
        {
            CommandResult result = new CommandResult();
            try
            {
                var parameters = new[] {
                    new SqlParameter("@tables", tables),
                    new SqlParameter("@Delkeys", delkeys),
                     new SqlParameter("@swhere", swhere)
                };
                List<ProcResult> listResult = DataProvider.LoadData<ProcResult>("usp_DeleteObject", parameters);

                if (listResult.Count > 0)
                {
                    result.IntResult = listResult[0].ResultID;
                    result.Message = listResult[0].ErrorMsg;
                }
                else
                {
                    result.IntResult = 0;
                    result.Message = "操作失败，请重试！";
                }
                result.Result = result.IntResult == 1;
            }
            catch (ApplicationException ex)
            {
                result.Result = false;
                result.Message = ex.Message;
            }
            return result;
        }
    }

    public class EntityProviderOP<T> : CertifiedProviderOP, IEntityProviderOP where T : EntityBase
    {
        public EntityProviderOP(LoginInfo user, DataProvider dp)
            : base(user, dp)
        {

        }

        public IJson Load(SearchCriteria searchCriteria)
        {
            return Search(searchCriteria);
        }

        public SearchResult<T> Search(string sql_Select, string sql_FormWhere, string sql_Orderby, string keyField, int pageSize, int pageIndex)
        {
            return DoSearch(sql_Select, sql_FormWhere, sql_Orderby, keyField, pageSize, pageIndex);
        }

        public SearchResult<T> Search(SearchCriteria searchCriteria)
        {
            return DoSearch(searchCriteria);
        }

        protected virtual bool CanInsert(ref string message)
        {
            return true;
        }

        protected virtual bool CanUpdate(ref string message)
        {
            return true;
        }

        protected virtual bool CanDelete(ref string message)
        {
            return true;
        }


        protected virtual SearchResult<T> DoSearch(SearchCriteria searchCriteria)
        {
            //lxt 141127 增加参数化查询的支持；

          
            string sSelect, sFromWhere, sOrderBy;

            string sSearchID = searchCriteria == null ? typeof(T).Name : searchCriteria.SearchID;
            int pageSize = searchCriteria.PageSize;
            SearchMeta meta = AppCache.GetSearchMeta(sSearchID);

           

            if (meta != null)
            {

                //lxt 141127 增加参数化查询的支持；
                if ((!string.IsNullOrEmpty(meta.Note)) && (meta.Note.ToLower() == "param"))  //使用note字段来表示是否使用参数化查询；
                {

                    return DoSearchwithParam(searchCriteria, meta);
                }


               // if (searchCriteria.PageSize > 0)
               //     meta.PageSize = searchCriteria.PageSize;
               //// else if (searchCriteria.PageSize == -1)
               //else if (searchCriteria.PageSize ==0)
               //     meta.PageSize = 30;
                ///等于0 标示使用数据库配置
                ///-1 标示不限制数量
                if (pageSize < 0)
                    pageSize = 300;
                else if (pageSize == 0)
                    pageSize = meta.PageSize;


                sSelect = meta.ToSelect();
                sFromWhere = meta.ToWhere(searchCriteria.Items);

                if (!string.IsNullOrEmpty(searchCriteria.sort))
                {
                    sOrderBy = " order by  " + searchCriteria.sort + "  " + searchCriteria.order + "  ";
                }
                else
                {
                    sOrderBy = meta.ToOrderBy();
                } 
            }
            else
            {
                throw new ApplicationException("No configuration of Search for " + searchCriteria.SearchID);
            }

            var result = DoSearch(sSelect, sFromWhere, sOrderBy, meta.KeyField, pageSize, searchCriteria.PageIndex);
            if (searchCriteria.IncludeChildren)
                LoadSubEntity(searchCriteria, meta, result);
            return result;
        }

        private SearchResult<T> DoSearchwithParam(SearchCriteria searchCriteria, SearchMeta meta)
        {
            ///等于0 标示使用数据库配置
            ///-1 标示不限制数量
            int _pageSize = searchCriteria.PageSize;            
            if (_pageSize == 0)
            {
                _pageSize = meta.PageSize;
            }
           bool _ispage = (_pageSize>0); //判断是否使用分页；

           string sSelect, sOrderBy, sFromWhere;
           List<SqlParameter> _Parmlist;


            sSelect = meta.ToSelect();            
            sFromWhere = meta.ToWhereWithParam(searchCriteria.Items,out _Parmlist,DataProvider.ParamTag );
            if (!string.IsNullOrEmpty(searchCriteria.sort))
            {
                sOrderBy = " order by  " + searchCriteria.sort + "  " + searchCriteria.order + "  ";
            }
            else
            {
                sOrderBy = meta.ToOrderBy();
            } 

            //获取总记录数；
            string _str = string.Format("select count(*)  {0}", sFromWhere);
            object  o = DataProvider.ExecuteScalar<object>(_str, _Parmlist.ToArray());
            Int32 _totalCount = decimal.ToInt32((decimal)o);



            var result = new SearchResult<T>();
            string _searchsql = DataProvider.BuildParamSearchSql(sSelect, sFromWhere, sOrderBy, meta.KeyField, _pageSize, searchCriteria.PageIndex,_ispage);
            var listResult = DataProvider.LoadData<T>(_searchsql, _Parmlist.ToArray());
            if (searchCriteria.IncludeChildren)
                LoadSubEntity(searchCriteria, meta, result);

            result.AddRange(listResult);
            result.TotalCount = _totalCount;
            result.PageSize = _pageSize;
            return result;
        }


        protected virtual void LoadSubEntity(SearchCriteria searchCriteria, SearchMeta meta, SearchResult<T> result)
        {
            meta.CheckSubProperty(typeof(T));
            foreach (SearchMetaSubProperty sub in meta.SubPropertyMeta)
            {
                var subType = typeof(EntityProviderOP<>).MakeGenericType(new Type[] { sub.PropertyGenericType });
                var criteria = new SearchCriteria() { SearchID = sub.SearchID };
                var searchOP = Activator.CreateInstance(subType, Account, DataProvider);
                var searchMethod = subType.GetMethod("Search", new[] { typeof(SearchCriteria) });
                foreach (T item in result.Items)
                {
                    criteria.Items.Clear();

                    foreach (var fieldMap in sub.FieldMatch)
                    {
                        criteria[fieldMap.Value] = item[fieldMap.Key];
                    }
                    var subResult = searchMethod.Invoke(searchOP, new object[] { criteria }) as ISearchResult;

                    var subPropertyValue = sub.SubProperty.GetValue(item, null);

                    var list = subPropertyValue as IList;
                    if (list != null)
                    {
                        foreach (var subObject in subResult.GetItems())
                        {
                            list.Add(subObject);
                        }
                    }
                }
            }
        }

        protected virtual SearchResult<T> DoSearch(string sql_Select, string sql_FormWhere, string sql_Orderby, string keyField, int pageSize, int pageIndex)
        {
            var sTotalCount = new SqlParameter("@PageCount", 1)
            {
                Direction = ParameterDirection.InputOutput
            };

            var parameters = new[] { 
                  new SqlParameter("@KeyField",keyField),
                  new SqlParameter("@PageIndex",pageIndex),
                  new SqlParameter("@PageSize",pageSize),                 
                  new SqlParameter("@Select",sql_Select),
                  new SqlParameter("@FromWhere",sql_FormWhere),
               //   new SqlParameter("@PageCount", pageIndex==0 ?1:0),
                  new SqlParameter("@OrderBy",sql_Orderby),
                  sTotalCount
            };

            var result = new SearchResult<T>();
            string sOutValue;
            var listResult = DataProvider.LoadData<T>("usp_DataPager", out sOutValue, parameters);

            result.AddRange(listResult);
            result.TotalCount = string.IsNullOrEmpty(sOutValue) ? 0 : int.Parse(sOutValue);
            result.PageSize = pageSize;
            return result;
        }

        bool CheckRegInfo()
        {

            //加入检查当前用户注册是否成功
            bool _isregOK = true ;
            /*
            if ((this.Account != null) && (this.Account.User != null))
            {
                if (!astatic.DicOrgReg.ContainsKey(this.Account.User.OrgId))
                {
                    //可能是超时了，在检查一次
                    var appSetting = new AppSettingOP(this.Account).GetAppSetting();

                }
                _isregOK = astatic.DicOrgReg[this.Account.User.OrgId];
            }
            */
            return _isregOK;
        }
        public int Save(SerializableData data)
        {

            if (!CheckRegInfo())
            {
                 throw new Exception("请联系软件提供商，检查注册信息！");
            }
            //
            TableMeta meta = EntityMetaManager.Default[data.GetType()];
            if (meta == null)
                throw new ApplicationException(string.Format("找不到类型{0}", data.GetType().Name));
            string tableName = meta.TableName;
            string keyField = meta.IdentityFieldName;
            int result = 0;
            switch (data.S)
            {
                case EntityStatus.New:
                    result = DoInsert(data, tableName, true);
                    if (!string.IsNullOrEmpty(keyField))
                        data[keyField] = result;
                    break;
                case EntityStatus.Modified:
                    result = DoUpdate(data, tableName, keyField,null);
                    break;
                case EntityStatus.Delete:
                    result = DoDelete(data, tableName, keyField);
                    break;
                case EntityStatus.Original:
                default:
                    break;
            }
            return result;
        }

        public int Save(SerializableData data, string tableName, string keyField)
        {
            int result = 0;
            switch (data.S)
            {
                case EntityStatus.New:
                    result = DoInsert(data, tableName, true);
                    if (!string.IsNullOrEmpty(keyField))
                        data[keyField] = result;
                    break;
                case EntityStatus.Modified:
                    result = DoUpdate(data, tableName, keyField,null );
                    break;
                case EntityStatus.Delete:
                    result = DoDelete(data, tableName, keyField);
                    break;
                case EntityStatus.Original:
                default:
                    break;
            }
            return result;
        }

        protected virtual int DoInsert(SerializableData data, string tableName, bool needKey)
        {
            int k = 0;
            try
            {
                if (!_blCoustomTran)
                {
                    DataProvider.BeginTran();
                }
                k = DataProvider.DoInsert(data, tableName, needKey);
                if (!_blCoustomTran)
                {
                    DataProvider.CommitTran();
                }
            }
            catch (Exception ee)
            {
                if (!_blCoustomTran)
                {
                    DataProvider.RollBack();
                }
                string _errormsg = AnaysleError(ee,tableName,data);
                if (string.IsNullOrEmpty(_errormsg))
                {
                    throw;
                }
                else
                {
                    throw new Exception(_errormsg);
                }
 
            }
            return k;
            /*
            if (data.Items.Count <= 0)
                return -1;

            var sFields = "";
            var sValues = "";

            data.Items.ForEach(e =>
            {
                if (!string.IsNullOrEmpty(e.K) && !e.K.EndsWith("_G") && !string.IsNullOrEmpty(e.V))
                {
                    sFields += string.Format("[{0}],", e.K);
                    sValues += string.Format("'{0}',", e.V);
                }
            });

            var sSql = string.Format(
                "INSERT INTO {0}({1}) VALUES({2});{3}",
                tableName,
                sFields.Trim().TrimEnd(','),
                sValues.Trim().TrimEnd(','),
                needKey ? "SELECT CAST(scope_identity() AS int);" : "");

            var iPID = DataProvider.ExecuteScalar<int>(sSql);
            return iPID;
             * */
        }
        private string AnaysleError(Exception ee, string tablename)
        {
          return    AnaysleError(ee, tablename, null);
        }
        private string AnaysleError(Exception ee, string tablename, SerializableData data)
        {
            //如果能解析的错误，返回包装后的错误提示，否则返回空字符串；
            if (ee.Message.IndexOf("ix_", StringComparison.OrdinalIgnoreCase) > -1)
            {
                string _errcode = tablename;
                //索引错误，默认用表名查找错误，但当索引错误名格式为ix_*****_xi时，用错误名称*****
                string   _Pattern = @"ix_\S*_xi";
                MatchCollection _Matches = Regex.Matches(ee.Message, _Pattern, RegexOptions.IgnoreCase |RegexOptions.ExplicitCapture);
                if ((_Matches != null) && (_Matches.Count > 0))
                {
                    _errcode = _Matches[0].Value;
                }

                EAP_ErrorMsg _errorMsg = AppCache.GetErrorMsg(_errcode);
                if (_errorMsg != null)
                {
                    string  _fmsg=FormatMsgByEntity( _errorMsg.ErrorMessage,data);
                    return _fmsg;
                }
                else  //默认是编号不能为空
                {
                    return "编号不能重复。";
                }
            }
            return  "" ;
        }

        private string FormatMsgByEntity(string ErrorMessage, SerializableData data)
        {
             //根据实体中的数据，格式化对象；
            string _Pattern = @"\[@[^\[@]*?@\]";
            string _kfield = "";
            MatchCollection _Matches = Regex.Matches(ErrorMessage, _Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            if ((_Matches != null) && (_Matches.Count > 0))
             foreach(Match  m in _Matches )
            {
                _kfield = m.Value.Substring(2, m.Value.Length - 4);
                object _o = data[_kfield];
                if ((_o != null) && (_o.ToString() != "")) //如果不是空和空字符串
                {
                    ErrorMessage = ErrorMessage.Replace(m.Value, _o.ToString());
                }
                else
                {
                    ErrorMessage = ErrorMessage.Replace(m.Value, "");
                }

            }
            return ErrorMessage;
        }

        protected virtual int DoUpdate(SerializableData data, string tableName, string keyField)
        {
           return  DoUpdate(data, tableName, keyField, null);
        }
        protected virtual int DoUpdate(SerializableData data, string tableName, string keyField, List<string> ExcludeFields)
        {
            int k = 0;
            try
            {
                if (!_blCoustomTran)
                {
                    DataProvider.BeginTran();
                }
                 k = DataProvider.DoUpdate(data, tableName, keyField, ExcludeFields);
                if (!_blCoustomTran)
                {
                    DataProvider.CommitTran();
                }
            }
            catch (Exception ee)
            {
                if (!_blCoustomTran)
                {
                    DataProvider.RollBack();
                }
                string _errormsg = AnaysleError(ee,tableName,data);
                if (string.IsNullOrEmpty(_errormsg))
                {
                    throw;
                }
                else
                {
                    throw new Exception(_errormsg);
                }
            }
            finally
            {
               
            }
            return k;
            /*
            var sSql = string.Format("UPDATE {0} SET ", tableName);
            var sql_Update = "";
            data.Items.ForEach(e =>
            {
                if (!string.IsNullOrEmpty(e.K)
                    && !e.K.EndsWith("_G")
                    && !e.K.Equals(keyField, StringComparison.InvariantCultureIgnoreCase)
                    && !string.IsNullOrEmpty(e.V))
                    if ( (e.S== EntityStatus.New || e.S == EntityStatus.Modified) && !string.IsNullOrEmpty(e.V))
                    {
                        sql_Update += string.Format("[{0}]='{1}',", e.K, e.V);
                    }
                    else if (e.S == EntityStatus.Delete)
                    {
                        sql_Update += string.Format("[{0}]=null,", e.K);   
                    } 
            });

            sql_Update = sql_Update.Trim().TrimEnd(',');
            if (!string.IsNullOrEmpty(sql_Update))
            {
                sSql += sql_Update;
                sSql += string.Format(" WHERE {0}='{1}'", keyField, data[keyField]);
                return DataProvider.ExecuteNonQuery(sSql);
            }
            return 0;
             * */
        }

        protected virtual int DoDelete(SerializableData data, string tableName, string keyField)
        {
            int k = 0;
            try
            {
                if (!_blCoustomTran)
                {
                    DataProvider.BeginTran();
                }
                k = DataProvider.DoDelete(data, tableName, keyField);
                if (!_blCoustomTran)
                {
                    DataProvider.CommitTran();
                }
            }
            catch
            {
                if (!_blCoustomTran)
                {
                    DataProvider.RollBack ();
                }

                throw;
            }
            finally
            {

            }
            return k;
 
            /*
            var sSql = string.Format("Delete {0} where {1} ='{2}'", tableName, keyField, data[keyField]);
            return DataProvider.ExecuteNonQuery(sSql);
             */
        }
        public void BeforeSave(SerializableData data)
        {
            DoBeforeSave(data);
        }

        public void AfterSave(SerializableData data)
        {
            DoAfterSave(data);
        }
        protected virtual void DoBeforeSave(SerializableData data)
        {
            //DataProvider.BeginTran();
        }

        protected virtual void DoAfterSave(SerializableData data)
        {
           // DataProvider.CommitTran();
        }

        bool _blCoustomTran =false  ;
        public bool blCoustomTran
        {
            get
            {
                return _blCoustomTran;
            }
            set
            {
                _blCoustomTran=value ;
            }
        }

        public void BeginTran()
        {
            DataProvider.BeginTran ();
        }

        public void CommitTran()
        {
            DataProvider.CommitTran();
        }

        public void RollBackTran()
        {
            DataProvider.RollBack();
        }
    }
}
