using System.Collections.Generic;
using NM.Model;
using NM.Util;
using System.Data;
using System.Data.SqlClient;


namespace NM.OP
{
    public class SourceOP : EntityProviderOP<EAP_Resource>
    {
        public SourceOP(LoginInfo user)
            : base(user, DataProvider.GetEAP_Provider())
        {
        }

        public TJsonList<EAP_Resource> GetAllResource(int parent_id,int IsUser,int loginUserID)
        {
            //string sql = string.Format(" exec usp_GetAllResource  {0}", parent_id);
            //var resource = new TJsonList<EAP_Resource>(DataProvider.LoadData<EAP_Resource>(sql));
            var parameters = new[] { 
                  new SqlParameter("@AppID",parent_id) ,
                   new SqlParameter("@isuser",IsUser) ,
                    new SqlParameter("@LoginUserID",loginUserID) ,

            };

            //var listResult = DataProvider.LoadData<T>("usp_GetResourceByUser",  parameters);
            var resource = new TJsonList<EAP_Resource>(DataProvider.LoadData<EAP_Resource>("usp_GetAllResource", parameters));
            return resource;
 
        }

        public TJsonList<EAP_Resource> GetMyMenu(int appid=0 ,int PResID=0)
        {
            //string sql = string.Format(" exec [usp_GetResourceByUser] {0},{1}  ", 0, Account.User.UserID);
            //lxt 20101102 改为传参数的方式 AppID int,UserID 
           // object o = 0;
            var parameters = new[] { 
                  new SqlParameter("@AppID", appid),
                  new SqlParameter("@UserID",Account.User.UserID) ,
                  new SqlParameter("@ResID",PResID) ,
            };             
             
            //var listResult = DataProvider.LoadData<T>("usp_GetResourceByUser",  parameters);
            var resource = new TJsonList<EAP_Resource>(DataProvider.LoadData<EAP_Resource>("usp_GetResourceByUser2", parameters));
            return resource;
        }

        /// <summary>
        /// 获取控制的按钮权限
        /// </summary>
        /// <param name="Controller"></param>
        /// <returns></returns>
        public TJsonList<EAP_Resource> GetMenuOperate(string Controller)
        {
            //string sql = string.Format(" exec [usp_GetResourceByUser] {0},{1}  ", 0, Account.User.UserID);
            //lxt 20101102 改为传参数的方式 AppID int,UserID 
            // object o = 0;
            var parameters = new[] { 
                  new SqlParameter("@UserID",Account.User.UserID) ,
                  new SqlParameter("@Pageid",Controller) ,
            };

            //var listResult = DataProvider.LoadData<T>("usp_GetResourceByUser",  parameters);
            var resource = new TJsonList<EAP_Resource>(DataProvider.LoadData<EAP_Resource>("usp_GetOpResourceByUser2", parameters));
            return resource;
        }

        public NavigatorMeta GetNavigator(int appId)
        {
            var nm = new NavigatorMeta();
            string sql = string.Format("exec [usp_GetNavigateMeta] {0}, {1}", appId, Account.User.UserID);
            DataSet ds = DataProvider.ExecuteDataSet(sql);
            nm.Nodes.AddRange(DataProvider.LoadData<EAP_Navigator_Node>(ds.Tables[0]));
            nm.Asso.AddRange(DataProvider.LoadData<EAP_Navigator_Asso>(ds.Tables[1]));            
            return nm;
        }
    }
}
