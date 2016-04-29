using NM.Service;
using NM.Util;
using NM.Model;
using NM.Log;

namespace NM.OP
{
    [ServiceFacadeAttribute]
    public class ServiceFacade_Account : ServiceFacadeBase
    {
        #region Config保存

        [Service("SaveSysConfig")]
        public void SaveSysConfig(DataRequest request, DataResponse result, DataProvider datasource)
        {
            EntityList<EAP_SYSConfig> configs = TJson.Parse<EntityList<EAP_SYSConfig>>(request[0]);
            EntityList<XX_Column> columnConfigs = TJson.Parse<EntityList<XX_Column>>(request[1]);
            result.Value = ToJson(new ConfigsOP(request.LogIn).SaveSysConfig(configs,columnConfigs));
        }

        #endregion

        #region ** User

        [Service("AddUser")]
        public void AddUser(DataRequest request, DataResponse result, DataProvider datasource)
        {
            EAP_User user = TJson.Parse<EAP_User>(request["P0"]);
            result.Value = ToJson(new AccountOP(request.LogIn).AddUser(user));
        }

        [Service("UpdateUser")]
        public void UpdateUser(DataRequest request, DataResponse result, DataProvider datasource)
        {
            EAP_User user = TJson.Parse<EAP_User>(request["P0"]);
            result.Value = ToJson(new AccountOP(request.LogIn).UpdateUser(user));
        }

        [Service("DeleteUser")]
        public void DeleteUser(DataRequest request, DataResponse result, DataProvider datasource)
        {
            string sIDs = request["P0"];
            result.Value = ToJson(new AccountOP(request.LogIn).DeleteUser(sIDs));
        }

        [Service("AddRoleToUser")]
        public void AddRoleToUser(DataRequest request, DataResponse result, DataProvider datasource)
        {
            EAP_User user = TJson.Parse<EAP_User>(request["P0"]);
            string sRoleIDs = request["P1"];
            result.Value = ToJson(new AccountOP(request.LogIn).AddRoleToUser(user, sRoleIDs));
        }

        [Service("GetOrgList")]
        public void GetOrgList(DataRequest request, DataResponse result, DataProvider datasource)
        {          
            result.Value = ToJson(new AccountOP(request.LogIn).GetOrgList());
        }

        [Service("DeleteRoleFromUser")]
        public void DeleteRoleFromUser(DataRequest request, DataResponse result, DataProvider datasource)
        {
            EAP_User user = TJson.Parse<EAP_User>(request["P0"]);
            string sRoleIDs = request["P1"];
            result.Value = ToJson(new AccountOP(request.LogIn).DeleteRoleFromUser(user, sRoleIDs));
        }

        [Service("ResetUserPwd")]
        public void ResetUserPwd(DataRequest request, DataResponse result, DataProvider datasource)
        {
            string sUserIDs = request["P0"];
            string sNewPass = request["P1"];

            result.Value = ToJson(new AccountOP(request.LogIn).ResetUserPwd(sUserIDs, sNewPass));
        }
        [Service("GetUserByName")]
        public void GetUserByName(DataRequest request, DataResponse result, DataProvider datasource)
        {
            string name = request["P0"];
            result.Value = ToJson(new AccountOP(request.LogIn).GetUserByName(name));
        }

        [Service("UserRegister")]
        public void UserRegister(DataRequest request, DataResponse result, DataProvider datasource)
        {
            LoginUser user = TJson.Parse<LoginUser>(request["P0"]);
            result.Value = ToJson(new AccountOP(request.LogIn).UserRegister(user));
        }
        #endregion

        #region ** Role

        [Service("AddRole")]
        public void AddRole(DataRequest request, DataResponse result, DataProvider datasource)
        {
            EAP_Role role = TJson.Parse<EAP_Role>(request["P0"]);
            result.Value = ToJson(new AccountOP(request.LogIn).AddRole(role));
        }

        [Service("UpdateRole")]
        public void UpdateRole(DataRequest request, DataResponse result, DataProvider datasource)
        {
            EAP_Role role = TJson.Parse<EAP_Role>(request["P0"]);
            result.Value = ToJson(new AccountOP(request.LogIn).UpdateRole(role));
        }

        [Service("DeleteRole")]
        public void DeleteRole(DataRequest request, DataResponse result, DataProvider datasource)
        {
            string sIDs = request["P0"];
            result.Value = ToJson(new AccountOP(request.LogIn).DeleteRole(sIDs));
        }

        #endregion
      
        [Service("UpdateRoleModules")]
        public void UpdateRoleModules(DataRequest request, DataResponse result, DataProvider datasource)
        {
            string rolID = request["P0"];
            string sSourceIDs = request["P1"];
            result.Value = ToJson(new AccountOP(request.LogIn).UpdateRoleMedules(rolID, sSourceIDs));
        }

        [Service("GetAllUsers")]
        public void GetAllUsers(DataRequest request, DataResponse result, DataProvider datasource)
        {
            result.Value = ToJson(new AccountOP(request.LogIn).GetAllUsers());
        }

        [Service("GetAllRoles")]
        public void GetAllRoles(DataRequest request, DataResponse result, DataProvider datasource)
        {
            result.Value = ToJson(new AccountOP(request.LogIn).GetAllRoles());
        }

        [Service("ChangePassword")]
        public void ChangePassword(DataRequest request, DataResponse result, DataProvider datasource)
        {
            string oldPass = request[0];
            string newPass = request[1];
            result.Value = ToJson(new AccountOP(request.LogIn).ChangePassword(request.LogIn.User.ID, oldPass, newPass));
        }

        [Service("LogIt")]
        public void LogIt(ServiceContext context)
        {
            UtilOP logOP = new UtilOP(context.E9_Request.LogIn);
            string module = context.E9_Request[0];
            LogLevel level = (LogLevel)int.Parse(context.E9_Request[1]);
            string message = context.E9_Request[2];
            context.E9_Response.Value = ToJson(logOP.LogIt(module, message, level));
        }


        [Service("LogIn")]
        public void LogIn(ServiceContext context)
        {
            string username = context.E9_Request[0];
            string pass = context.E9_Request[1];
            string ukeyid = context.E9_Request[2];//;lxt 增加了ukey的验证
            context.E9_Response.Value = ToJson(new AccountOP(context.E9_Request.LogIn).LogIn(username,pass,context.HttpContext,ukeyid));
        }
        [Service("GetUrl")]
        public void GetUrl(ServiceContext context)
        {          
            context.E9_Response.Value = ToJson(new AccountOP(context.E9_Request.LogIn).GetUrl(context.HttpContext));
        }
    }
}
