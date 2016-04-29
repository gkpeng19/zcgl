using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NM.Model;
using System.Data.SqlClient;
using System.Net;
using NM.Util;
using System.Web;
using System.Configuration;
using NM.Lib;
using System.Data;

namespace NM.OP
{
    public class AccountOP : CertifiedProviderOP
    {
        LoginInfo loginUser;
        public AccountOP(LoginInfo user)
            : base(user, DataProvider.GetEAP_Provider())
        {
            loginUser = user;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sUserName"></param>
        /// <param name="nametype">0:用户名登录 1：caid登录</param>
        /// <returns></returns>
        public EAP_User GetUserByUserName(string sUserName,int nametype=0)
        {
            SqlParameter[] parameters = new SqlParameter[] { 
                    new SqlParameter("@UserName", sUserName),
                    new SqlParameter("@nametype", nametype)
            };

            EAP_User user = DataProvider.GetEntity<EAP_User>("usp_GetUserByUserName2", parameters);

            if (user != null)
            {
                string sSql = string.Format("SELECT * FROM uv_UserRole WHERE UserID='{0}'", user.ID);
                List<EAP_Role> _List = DataProvider.LoadData<EAP_Role>(sSql);
                user.Roles.Clear();
                user.Roles.AddRange(_List);
            }

            return user;
        }
       

        #region ** Login Methods

        private void SaveLoginInfo(LoginInfo lgi)
        {
            SqlParameter[] parameters = new SqlParameter[] { 
                    new SqlParameter("@UserID",lgi.User == null ? "-1" : lgi.User.UserName),
                    new SqlParameter("@ServerIP",lgi.ServerIP),
                    new SqlParameter("@ServerName",lgi.ServerName),
                    new SqlParameter("@ClientIP",lgi.ClientIP),
                    new SqlParameter("@ClientName",lgi.ClientName),                    
                    new SqlParameter("@LoginPort",lgi.LoginPort),     
                    new SqlParameter("@LogMessage",lgi.Message),
                    new SqlParameter("@Status",lgi.Status.GetHashCode())
            };

            int iLogID = (int)DataProvider.ExecuteScalar<decimal>("usp_Login", parameters);
            if (iLogID > 0)
            {
                lgi.SessionID = iLogID;
                lgi.ID = iLogID;
            }
            else
                throw new ApplicationException("数据库操作失败。");
        }

        public CommandResult LogIn(string sUserName, string sPassword, string clientIP, string clientName, int loginPort,string ukeyid)
        {
            CommandResult result = new CommandResult();
            try
            {
                var login = Login(sUserName, sPassword, clientIP, clientName, loginPort,ukeyid);
                if (login.Status == LoginStatus.Successed)
                {                 
                     bool  _bl=  VaildateLoginIP(login);
                     if (_bl)
                     {
                         result.Result = true;
                         result.Message = login.ToJson();
                     }
                     else
                     {
                         result.Result = false;
                         result.Message = "登录失败，该用户禁止远程外网登录。";
                     }
                }
                else
                {
                    result.Message = login.Message;
                    result.Result =false;
                }
                
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        private bool  VaildateLoginIP(LoginInfo login)
        {
            string _isValidate = ConfigurationManager.AppSettings["VaildateIP"].ToString();
            if (string.IsNullOrEmpty(_isValidate))
            {
                _isValidate = "1";
            }

            if (_isValidate == "1")
            {
                if ((login.User.ORGTYPE_G == 0) || (login.User.ORGTYPE_G == 1)) //总部和分店帐号
                {
                    if (!login.ClientIP.Equals("127.0.0.1"))
                    {
                        if (login.ClientIP.Substring(0, 3) != "172")
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public CommandResult LogIn(string sUserName, string sPassword, HttpContext context,string  ukeyid)
        {
            #region
            //加入注册校验


            #endregion

            string clientIP = context.Request.UserHostAddress;
            //string clientName = string.Format("{0},{1},{2}",
            //     HttpContext.Current.Request.ServerVariables["REMOTE_HOST"],
            //     HttpContext.Current.Request.ServerVariables["LOGON_USER"],
            //     HttpContext.Current.Request.ServerVariables["SERVER_NAME"]
            //     );// HttpContext.Current.Request.UserHostName;
            string clientName = context.Request.ServerVariables["LOGON_USER"];
            int loginPort = context.Request.Url.Port;

            if (sUserName.Equals("admin", StringComparison.InvariantCultureIgnoreCase)
                && !clientIP.Equals("127.0.0.1"))
                return new CommandResult() {Result=false,Message="登录失败，该用户禁止远程登录" };
            return LogIn(sUserName, sPassword, clientIP, clientName, loginPort,ukeyid);
        }

        public LoginInfo LoginBykey(string ukeyid)
        {

            return Login("", "", "", "", 80, ukeyid, 1);
            
        }

        public LoginInfo Login(string sUserName, string sPassword, string clientIP, string clientName,
            int port, string ukeyid = "000000",int nametype=0)
        {
            string sLoginMessage = "";
            LoginStatus ls = LoginStatus.Failed;

            if (nametype == 0)
            {
                if (string.IsNullOrEmpty(sUserName))
                {
                    sLoginMessage = "登录失败，用户名输入为空。";
                }

                if (string.IsNullOrEmpty(sPassword))
                {
                    sLoginMessage = "登录失败，密码输入为空。";
                }
            }
            else if (nametype == 1)
            {
                if (string.IsNullOrEmpty(ukeyid))
                {
                    sLoginMessage = "登录失败，用户iD为空。";
                }
            }


            EAP_User user = null;
            if (nametype == 1)
            {
                user = GetUserByUserName(ukeyid, 1); 
            }
            else
            {
                user = GetUserByUserName(sUserName);
            }
            

            if (null == user)
            {
                sLoginMessage = "登录失败，用户不存在。";
            }
            else if ((nametype==0) && (sPassword != DESEncrypt.Decrypt(user.Password)))
            {
                sLoginMessage = "登录失败，密码不正确。";
            }
            else if (user.IsLock)
            {
                sLoginMessage = "该用户名已经已经停止使用。";
            }
            else
            {
                //加上检验是否进行ukey验证的代码
                bool blok=true  ;
                if (user.isUseKey_G >=1)
                {
                    if (string.IsNullOrEmpty(ukeyid))
                    {
                        sLoginMessage = "没有检测到UKey，请重试！";
                        blok = false;
                    }
                    else if (ukeyid != "000000")//当传入000000时，表示是后台调用，不需要校验
                    {
                        if (user.isUseKey_G == 1) //一个组织可以使用多个ukey，不分用户；
                        {
                            string sSql = string.Format("select ID from EAP_OrgUKey where orgid={0} and ukeyid='{1}'", user.OrgId, ukeyid);
                            List<int> _ukeyls = DataProvider.LoadData<int>(sSql);
                            if ((_ukeyls == null) || (_ukeyls.Count == 0))
                            {
                                sLoginMessage = "UKey认证失败！";
                                blok = false;
                            }
                        }
                        else if (user.isUseKey_G == 2) //一个用户一个ukey
                        {
                            if (string.Compare(user.BarCode, ukeyid, true) != 0)
                            {
                                sLoginMessage = "UKey认证失败！";
                                blok = false;
                            }
                        }

                    }

                }
                if (blok )
                {
                    sLoginMessage = "登录成功。";
                    ls = LoginStatus.Successed;
                }
            }

            #region get client pc info

            string serverIP, serverName;
            DateTime myNow = DateTime.Now;
            serverName = Dns.GetHostEntry("localhost").HostName;
            //System.Net.Dns.GetHostName();

            System.Net.IPAddress[] addressList = Dns.GetHostEntry(serverName).AddressList;
            if (addressList.Length > 0)
            {
                int _k = addressList.Length - 1;
                serverIP = addressList[_k].ToString();
            }

            else
                serverIP = addressList[0].ToString();
            serverIP = "1";

            #endregion

            LoginInfo login = new LoginInfo()
            {
                //ClientIP = clientIP,
                ClientName = string.IsNullOrEmpty(clientName) ? "No get." : clientName,
                //ServerIP = string.IsNullOrEmpty(serverIP) ? "No get." : serverIP,
                ServerName = string.IsNullOrEmpty(serverName) ? "No get." : serverName,
                LoginPort = port,
                Status = ls,
                Message = string.IsNullOrEmpty(sLoginMessage) ? "No Message." : sLoginMessage,
                User = user,
            };

            //user.Roles.ForEach(e =>
            //{
            //    List<String> _List = this.GetPermissionByRole(e.ID);
            //    _List.ForEach(c =>
            //    {
            //        if (!login.Permission.Contains(c))
            //        {
            //            login.Permission.Add(c);
            //        }
            //    });
            //});

            SaveLoginInfo(login);
            return login;
        }

        private List<string> GetPermissionByRole(int roleID)
        {
            string sSql = string.Format("SELECT * FROM uv_RoleModule WHERE RoleID='{0}'", roleID);
            return DataProvider.LoadData<string>(sSql);
        }

        public void Logout(LoginInfo lgi)
        {
            if (lgi.User == null)
                return;

            DateTime logoutTime = DateTime.Now;
            SqlParameter[] parameters = new SqlParameter[] { 
                    new SqlParameter("@ID",lgi.ID)
            };

            int iReturn = DataProvider.ExecuteNonQuery("usp_Logout", parameters);

            if (iReturn > 0)
            {
                lgi.Status = LoginStatus.Exited;
                lgi.LogoutTime = logoutTime;
            }
        }

        #endregion

        #region ** User Methods

        public CommandResult AddUser(EAP_User user)
        {
            if (string.IsNullOrEmpty(user.Password))
                user.Password = DESEncrypt.Encrypt("123456");
            CommandResult result = new CommandResult();
            result.Result = false;

            /*
            string sFields = "";
            string sValues = "";

            if (user.Items.Count <= 0)
            {
                return result;
            }

            user.Items.ForEach(e =>
            {
                if (e.K != "ID" && e.K != "Org_Name")
                {
                    if (!string.IsNullOrEmpty(e.K) && e.S == EntityStatus.New && !e.K.EndsWith("_G"))
                    {
                        sFields += string.Format("[{0}],", e.K);
                        sValues += string.Format("'{0}',", e.V);
                    }
                }
            });

            string sSql = string.Format(
                "INSERT INTO EAP_User({0}) VALUES({1});SELECT CAST(scope_identity() AS int);",
                sFields.Trim().TrimEnd(','),
                sValues.Trim().TrimEnd(','));

            int iPID = DataProvider.ExecuteScalar<int>(sSql);
            */
            List<string>  _fns=new List<string> ();
            _fns.Add("Org_Name");
            int iPID = DataProvider.DoInsert(user, "EAP_USER", true, _fns);
            if (iPID > 0)
            {
                result.Result = true;
                result.ReturnValue.Add(new LookupDataItem() { K = "ID", V = iPID.ToString() });
                result.Message = "新增成功。";
            }
            else
            {
                result.Result = false;
                result.Message = "新增失败。";
            }

            return result;
        }

        public CommandResult UpdateUser(EAP_User user)
        {
            CommandResult result = new CommandResult();


            List<string> _fns = new List<string>();
            _fns.Add("Org_Name");
            _fns.Add("ID");
            int iRows= DataProvider.DoUpdate(user, "EAP_USER","ID", _fns);

            /*
            string sSql = "UPDATE EAP_User SET ";
            user.Items.ForEach(e =>
            {
                if (e.K != "ID" && e.K != "Org_Name")
                {
                        if (e.S == EntityStatus.Modified && !string.IsNullOrEmpty(e.K))
                        {
                            sSql += string.Format("[{0}]='{1}',", e.K, e.V);
                        }                  
                }
            });

            sSql = sSql.Trim().TrimEnd(',');
            sSql += " WHERE ID='" + user.ID.ToString() + "'";

            int iRows = DataProvider.ExecuteNonQuery(sSql);
            */

            result.Result = iRows > 0 ? true : false;
            result.Message = iRows > 0 ? "更新成功。" : "更新失败。";

            return result;
        }


        //查询所有EAP_Org
        public EAP_User GetOrgList()
        {    
            EAP_User user = new EAP_User();
            string sSql = string.Format("SELECT * FROM EAP_Org");
            List<EAP_Org> _List = DataProvider.LoadData<EAP_Org>(sSql);
            if (_List != null)
            {
                user.OrgList = _List;             
            }
            return user;
        }

        public List<WMS_DEPARTMENT> GetDepSecList()
        {           
            string sSql = string.Format("SELECT * FROM WMS_DEPARTMENT");
            List<WMS_DEPARTMENT> lst = DataProvider.LoadData<WMS_DEPARTMENT>(sSql);
            if (lst != null)
            {
                return lst;
            }
            return null;
        }

        public CommandResult DeleteUser(string sIDs)
        {
            string sSql = string.Format(@"DELETE FROM EAP_User WHERE ID IN({0})", sIDs);

            CommandResult result = new CommandResult();
            int iRows = DataProvider.ExecuteNonQuery(sSql);
            result.Result = iRows > 0 ? true : false;
            result.Message = iRows > 0 ? "删除成功。" : "删除失败。";

            return result;
        }

        public CommandResult AddRoleToUser(EAP_User user, string sRoleIDs)
        {
            CommandResult result = new CommandResult();

            string[] arrayRoleID = sRoleIDs.Split(',');

            string sSql = "";

            try
            {
                for (int i = 0; i < arrayRoleID.Length; i++)
                {
                    /* 
                    sSql = string.Format("INSERT INTO EAP_UserRole([UserID],[RoleID],[AddBy],[AddOn]) values('{0}','{1}','{2}','{3}');SELECT CAST(scope_identity() AS int);",
                    user.ID, arrayRoleID[i], Account.User.UserName, DateTime.Now);

                    DataProvider.ExecuteScalar<int>(sSql);
                    */
                    SerializableData _sd = new SerializableData();
                    _sd.SetInt32("USERID", user.ID);
                    _sd.SetString("ROLEID", arrayRoleID[i]);
                    _sd.SetString("ADDBY", Account.User.UserName);
                    int iPID = DataProvider.DoInsert(_sd, "EAP_UserRole", true, null );
                    if (iPID <= 0)
                    {
                        throw new Exception("保存失败。"+_sd .ToJson ());
                    }
                    
                }

                result.Result = true;
                result.Message = "添加成功。";

                return result;
            }
            catch
            {

                result.Result = false;
                result.Message = "数据库操作失败，参考Sql: " + sSql;

                return result;
            }
        }

        public CommandResult DeleteRoleFromUser(EAP_User user, string sRoleIDs)
        {
            CommandResult result = new CommandResult();

            string sSql = string.Format(@"DELETE FROM EAP_UserRole WHERE [USERID]='{0}' and [ROLEID] IN({1})", user.ID, sRoleIDs);

            int iRows = DataProvider.ExecuteNonQuery(sSql);
            result.Result = iRows > 0 ? true : false;
            result.Message = iRows > 0 ? "删除成功。" : "删除失败。";

            return result;
        }

        public CommandResult ResetUserPwd(string sUserIDs, string sNewPass)
        {
            CommandResult result = new CommandResult();
            string sSql = string.Format(@"UPDATE EAP_User SET Password='{0}' WHERE ID IN ({1})", sNewPass, sUserIDs);

            int iRow = DataProvider.ExecuteNonQuery(sSql);
            result.Result = iRow > 0 ? true : false;
            result.Message = iRow > 0 ? "密码重置成功。" : "密码重置失败。";

            return result;
        }

        public CommandResult ChangePassword(int userID, string oldPassword, string newPass)
        {

            //var t = DESEncrypt.Encrypt("test");
            //var t1 = DESEncrypt.Decrypt(t);

            CommandResult result = new CommandResult();
            //SqlParameter outputPar = new SqlParameter("@Output", "") { Direction = System.Data.ParameterDirection.InputOutput,Size=32 };
            //SqlParameter resultPar = new SqlParameter("@Result", 1) { Direction = System.Data.ParameterDirection.InputOutput };

            //SqlParameter[] parameters = new SqlParameter[] { 
            //        new SqlParameter("@UserID",userID),
            //        new SqlParameter("@OldPass",DESEncrypt.Encrypt(oldPassword)),
            //        new SqlParameter("@NewPass",DESEncrypt.Encrypt(newPass)),
            //        resultPar,outputPar                   
            //};

            //DataProvider.ExecuteNonQuery("usp_ChangePassword", parameters);
            //result.Result = (int)resultPar.Value == 1;
            //result.Message = outputPar.Value.ToString();
            //return result;
            
            var sql = string.Format("select [PASSWORD] from EAP_User where ID={0} ",userID);
            var pass = DataProvider.ExecuteScalar<string>(sql);
            if (DESEncrypt.Decrypt(pass) == oldPassword)
            {
                result.Result = true;
                sql = string.Format("update EAP_User set [PASSWORD]='{1}' where ID={0}", userID, DESEncrypt.Encrypt(newPass));
                DataProvider.ExecuteNonQuery(sql);
                result.Message = "密码修改成功";               
            }
            else
            {
                result.Result = false;
                result.Message = "原密码不正确";
            }
            return result;
             
        }

        public TJsonList<EAP_User> GetAllUsers()
        {
            string strSQL = "select * from EAP_User";
            return new TJsonList<EAP_User>(DataProvider.LoadData<EAP_User>(strSQL));
        }

        public TJsonList<EAP_Role> GetAllRoles()
        {
            string strSQL = "select * from EAP_Role";
            return new TJsonList<EAP_Role>(DataProvider.LoadData<EAP_Role>(strSQL));
        }
        public CommandResult GetUserByName(string name)
        {
            CommandResult res = new CommandResult();
            try
            {
                string str = string.Format("select count(*) from EAP_User where UserName='{0}'", name);
                res.IntResult= DataProvider.ExecuteScalar<int>(str);
            }catch(Exception e)
            {
                res.IntResult = 0;
                res.Message = e.Message;
            }
            return res;
        }

        public CommandResult UserRegister(LoginUser user)
        {
            var parameters = new[]{
                 new SqlParameter("@username",user.UserName),
                 new SqlParameter("@psw",DESEncrypt.Encrypt(user.Password)),
                 new SqlParameter("@cname",user.CName),
                 new SqlParameter("@caddr",user.CAddr),
                 new SqlParameter("@industorytype",user.IndustoryType),
                 new SqlParameter("@csellphone",user.SellPhone),
                new SqlParameter("@cLicense","")
                // new SqlParameter("@cLicense",astatic.GenFile(user.CName, DateTime.Now.AddMonths(1).ToShortDateString()))
            };

            CommandResult res = new CommandResult();
            res.IntResult = 0;
            DataSet ds = null;
            try
            {
                ds = DataProvider.ExecuteDataSet("usp_UserRegister", parameters);
                DataRow dr = ds.Tables[0].Rows[0];
                res.IntResult = Convert.ToInt32(dr[0]);
                res.Message = dr[1].ToString();
            }
            finally
            {
                ds.Dispose();
            }

            return res;
        }
        #endregion

        #region ** Role

        public CommandResult AddRole(EAP_Role role)
        {
            CommandResult result = new CommandResult();
            result.Result = false;
           
            string sFields = "";
            string sValues = "";

            if (role.Items.Count <= 0)
            {
                return result;
            }
            /*
            role.Items.ForEach(e =>
            {
                if (!string.IsNullOrEmpty(e.K) && e.S == EntityStatus.New)
                {
                    sFields += string.Format("[{0}],", e.K);
                    sValues += string.Format("'{0}',", e.V);
                }
            });
             * */
            string sql = string.Format("select count(*) from EAP_Role where RoleName='{0}'",role.RoleName);
            /*
            string sSql = string.Format(
                "INSERT INTO EAP_Role({0}) VALUES({1});SELECT CAST(scope_identity() AS int);",
                sFields.Trim().TrimEnd(','),
                sValues.Trim().TrimEnd(','));
             * int count = DataProvider.ExecuteScalar<int>(sql);
            */
            int count =(int) DataProvider.ExecuteScalar<decimal>(sql);
            if (count <= 0)
            {
               // int iPID = DataProvider.ExecuteScalar<int>(sSql);
                int iPID = DataProvider.DoInsert(role, "EAP_Role", true, null);
                if (iPID > 0)
                {
                    result.Result = true;
                    result.ReturnValue.Add(new LookupDataItem() { K = "ID", V = iPID.ToString() });
                    result.Message = "新增成功。";
                }
                else
                {
                    result.Result = false;
                    result.Message = "新增失败。";
                }
            }
            else {
                result.Result = false;
                result.IntResult = 1;
                result.Message = "该角色已经存在。";
            }
            return result;
        }

        public CommandResult UpdateRole(EAP_Role role)
        {
            CommandResult result = new CommandResult();
            /*
            string sSql = "UPDATE EAP_Role SET ";
            role.Items.ForEach(e =>
            {
                if (e.S == EntityStatus.Modified && !string.IsNullOrEmpty(e.K))
                {
                    sSql += string.Format("[{0}]='{1}',", e.K, e.V);
                }
            });

            sSql = sSql.Trim().TrimEnd(',');
            sSql += " WHERE ID='" + role.ID.ToString() + "'";

            int iRows = DataProvider.ExecuteNonQuery(sSql);
            */
            int iRows = DataProvider.DoUpdate(role, "EAP_Role", "ID", null);
            result.Result = iRows > 0 ? true : false;
            result.Message = iRows > 0 ? "更新成功。" : "更新失败。";

            return result;
        }

        public CommandResult DeleteRole(string sIDs)
        {
            string sSql = string.Format(@"DELETE FROM EAP_Role WHERE ID IN({0})", sIDs);

            CommandResult result = new CommandResult();
            int iRows = DataProvider.ExecuteNonQuery(sSql);
            result.Result = iRows > 0 ? true : false;
            result.Message = iRows > 0 ? "删除成功。" : "删除失败。";

            return result;
        }

        #endregion
     
        public CommandResult UpdateRoleMedules(string roleID, string sSourceIDs)
        {
            CommandResult result = new CommandResult() { Result = false, Message = "更新失败。" };

            SqlParameter[] parameters = new SqlParameter[] { 
                    new SqlParameter("@RoleId",roleID),
                    new SqlParameter("@Reources",sSourceIDs),
                    new SqlParameter("@AddBy",Account.User.UserName)
            };
            List<ProcResult> listResult = DataProvider.LoadData<ProcResult>("usp_AddPermissionToRole", parameters);
           // int iRow = DataProvider.ExecuteNonQuery("usp_AddPermissionToRole", parameters);
            if ((listResult != null) && (listResult.Count > 0))
            {
                if (listResult[0].ResultID == 1)
                {
                    result.Result =  true  ;
                    result.Message =   "更新成功。"  ;
                    return result;
                }
            }

            

            return result;
        }
        /// <summary>
        /// 获取页面的url地址
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public CommandResult GetUrl(HttpContext context)
        {
            CommandResult result = new CommandResult();
            try
            {
                result.Message = context.Request.Url.ToString();
            }
            catch (Exception)
            {

            }
            return result;
        }
    }
}
