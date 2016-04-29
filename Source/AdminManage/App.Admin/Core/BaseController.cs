using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using App.Common;
using App.Models.Sys;
using NM.Model;
using NM.OP;
using NM.Util;
using App.Admin.Core;

namespace App.Admin
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 获取当前用户Id
        /// </summary>
        /// <returns></returns>
        public string GetUserName()
        {
            if (Session["Account"] != null)
            {
                AccountModel info = (AccountModel)Session["Account"];
                return info.UserName;
            }
            else
            {

                return "";
            }
        }
        /// <summary>
        /// 获取当前用户Name
        /// </summary>
        /// <returns></returns>
        public string GetUserTrueName()
        {
            if (Session["Account"] != null)
            {
                AccountModel info = (AccountModel)Session["Account"];
                return info.TrueName;
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns>用户信息</returns>
        public AccountModel GetAccount()
        {
            if (Session["Account"] != null)
            {
                return (AccountModel)Session["Account"];
            }
            return null;
        }

        public LoginInfo LoginUser
        {
            get
            {
                LoginInfo _eaplogininfo =null ;
                AccountModel _login = GetAccount();
                if (_login != null)
                {
                    _eaplogininfo = new LoginInfo();
                    _eaplogininfo.ID = _login.UserId;
                    _eaplogininfo.User = new EAP_User() {ID=_login.UserId,UserID=_login.UserId,TrueName=_login.TrueName,UserName=_login.UserName,OrgId=_login.OrgID };
                }
                return _eaplogininfo;
            }
        }

        /// <summary>
        /// 获取当前页或操作访问权限
        /// </summary>
        /// <returns>权限列表</returns>
        public List<permModel> GetPermission()
        {
            string filePath = HttpContext.Request.FilePath;
            List<permModel> perm = (List<permModel>)Session[filePath];
            return perm;
        }
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new ToJsonResult
            {
                Data = data,
                ContentEncoding = contentEncoding,
                ContentType = contentType,
                JsonRequestBehavior = behavior,
                FormateStr = "yyyy-MM-dd HH:mm:ss"
            };
        }
        /// <summary>
        /// 返回JsonResult.24         /// </summary>
        /// <param name="data">数据</param>
        /// <param name="behavior">行为</param>
        /// <param name="format">json中dateTime类型的格式</param>
        /// <returns>Json</returns>
        protected JsonResult MyJson(object data, JsonRequestBehavior behavior, string format)
        {
            return new ToJsonResult
            {
                Data = data,
                JsonRequestBehavior = behavior,
                FormateStr = format
            };
        }
        /// <summary>
        /// 返回JsonResult42         /// </summary>
        /// <param name="data">数据</param>
        /// <param name="format">数据格式</param>
        /// <returns>Json</returns>
        protected JsonResult MyJson(object data, string format)
        {
            return new ToJsonResult
            {
                Data = data,
                FormateStr = format
            };
        }

        public void ToSearchCriteria(GridPager pager, ref SearchCriteria search)
        {
            search.sort = pager.sort;
            search.order = pager.order;
            search.PageIndex = pager.page-1;
            search.PageSize = pager.rows;
        }

        public void ToGrigPagerCount(int TotalCount, ref GridPager pager)
        {
            pager.totalRows = TotalCount;
        }
        /// <summary>
        /// 检查SQL语句合法性
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool ValidateSQL(string sql, ref string msg)
        {
            if (sql.ToLower().IndexOf("delete") > 0)
            {
                msg = "查询参数中含有非法语句DELETE";
                return false;
            }
            if (sql.ToLower().IndexOf("update") > 0)
            {
                msg = "查询参数中含有非法语句UPDATE";
                return false;
            }

            if (sql.ToLower().IndexOf("insert") > 0)
            {
                msg = "查询参数中含有非法语句INSERT";
                return false;
            }

            if (sql.ToLower().IndexOf("drop") > 0)
            {
                msg = "查询参数中含有非法语句drop";
                return false;
            }
            return true;
        }

        public T GetObjByID<T>(string Id, string SearchID="",string KeyField="ID") where T : EntityBase
        {
            if (string.IsNullOrEmpty(SearchID))
            {
                
                Type t = typeof(T);
                SearchID = t.Name.ToUpper();
            }

            EntityProviderOP<T> _op = new EntityProviderOP<T>(LoginUser, DataProvider.GetEAP_Provider());
          
            SearchCriteria _search = new SearchCriteria(SearchID);
            _search[KeyField] = int.Parse(Id);

            SearchResult<T> _rs = _op.Search(_search);
            if (_rs.Items.Count > 0)
            {
                return _rs.Items[0];
            }
            return null ;
        }

        public CommandResult SaveObj<T>(T obj) where T : EntityBase
        {
            CommandResult r = new CommandResult();
            try
            {
                if (obj.ID > 0)
                {
                    obj._S = obj.S = EntityStatus.Modified;
                }
                else
                {
                    obj._S = obj.S = EntityStatus.New ;
                }
                EntityProviderOP<T> _op = new EntityProviderOP<T>(LoginUser, DataProvider.GetEAP_Provider());
                r.IntResult = _op.Save(obj);

            }
            catch (Exception ee)
            {
                r.Message = ee.Message;
                
            }
            return r;
        
        }
        public CommandResult DeleteObj<T>(T obj,string KeyField="ID") where T : EntityBase
        {
            CommandResult r = new CommandResult();
            Type t = obj.GetType();
            string tableName = t.Name;
            
            //   typeof(T).GetGenericArguments();
            try
            {
                r.IntResult = DataProvider.GetEAP_Provider().DoDelete(obj, tableName, KeyField);

            }
            catch (Exception ee)
            {

                r.Message = ee.Message;

            }
            return r;
        }
        void rebuild<T>(EntityList<T> Items, TreeBase currTree, int ParentID) where T : EntityBase 
          {
              foreach (T _c in Items)
              {
                  object o = _c["ParentID"];
                  if ((o != null) && (o.ToString() == ParentID.ToString()))
                  {
                      TreeBase c = new TreeBase();
                      c.id = _c.ID;
                      c.text = _c["Name"].ToString();
                      rebuild(Items, c, c.id);
                      currTree.children .Add(c);
                  }


              }


           }
        public List<TreeBase>   BuildTree<T>(EntityList<T> Items)   where T:EntityBase
        {
            List<TreeBase> _ls = new List<TreeBase>();
            foreach (T _c in Items)
            {
                object o=_c["ParentID"];
                if ((o != null) && (o.ToString() == "0"))
                { 
                    TreeBase c = new TreeBase();
                    c.id = _c.ID;
                    c.text = _c["Name"].ToString();
                    c.ischeck = _c.Checked;
                    
                    _ls.Add(c);
                    rebuild<T>(Items, c, c.id);
                   
                }            
                
                
            }

            return _ls;
        }

       

    }
}