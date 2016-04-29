using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Common;
using App.Models;
using Microsoft.Practices.Unity;
 

using App.Models.Sys;
using NM.OP;
using NM.Model;
using System.Data.SqlClient;
using System;
namespace App.Admin.Controllers
{
    public class SysRoleController : BaseController
    {
        //

        ValidationErrors errors = new ValidationErrors();

        [SupportFilter]
        public ActionResult Index()
        {
            ViewBag.Perm = GetPermission();
            return View();
        }
        [SupportFilter(ActionName="Index")]
        public JsonResult GetList(GridPager pager,string queryStr)
        {
            EntityProviderOP<EAP_Role> _op = new EntityProviderOP<EAP_Role>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("EAP_ROLE");
            ToSearchCriteria(pager, ref _search);
            _search["RoleName"] = queryStr;

            SearchResult<EAP_Role> _rs = _op.Search(_search);


            var json = new
            {
                total = _rs.TotalCount,
                rows = _rs.Items.ToArray()

            };

            return Json(json);
           // ToGrigPagerCount(_rs.TotalCount ,ref pager);


            /*
            List<SysRoleModel> _list = new List<SysRoleModel>();// m_BLL.GetList(ref pager, queryStr);

            foreach (EAP_Role r in _rs.Items)
            {
                SysRoleModel _o = new SysRoleModel()
                        {
                            Id = r.ID.ToString(),
                            Name = r.RoleName,
                            Description = r.Description,
                            CreateTime = r.AddOn,
                            CreatePerson = r.AddBy 
                        };
                foreach (EAP_User u in  r.Users)
                {
                    _o.UserName = _o.UserName + "【" + u.UserName + "】";
                }
                _list.Add(_o);
                    
            }
             */

        }


        #region 设置角色用户
        [SupportFilter(ActionName = "Allot")]
        public ActionResult GetUserByRole(string roleId)
        {
            ViewBag.RoleId = roleId;
            ViewBag.Perm = GetPermission();
            return View();
        }

        [SupportFilter(ActionName="Allot")]
        public JsonResult GetUserListByRole(GridPager pager,string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
                return Json(0);
            List<SqlParameter> parms = new List<SqlParameter>();
            DataProvider  _dp=  DataProvider.GetEAP_Provider();
            string _str = "select count(*)  from eap_user ";
            object o = _dp.ExecuteScalar<object>(_str, parms.ToArray());

            Int32 _totalCount = decimal.ToInt32((decimal)o);

            string _select=string.Format(" u.ID,u.UserName,u.TrueName,case nvl(ur.roleid,0) when 0 then '0' else '1' end  as State_G "+
                          "from EAP_User u left join EAP_UserRole ur on u.ID = ur.UserID  and ur.roleid={0}",roleId);
            string sOrderBy = "";
            if (!string.IsNullOrEmpty(pager.sort))
            {
                sOrderBy = " order by " + pager.sort + "  " + pager.order;
            }
            
           string _ssql=  DataProvider.GetEAP_Provider().BuildParamSearchSql(_select, "", sOrderBy, "ID", pager.rows, pager.page-1, true);
            
            List<EAP_User> _ulist = DataProvider.GetEAP_Provider().LoadData<EAP_User>(_ssql, parms.ToArray());
           //var userList = m_BLL.GetUserByRoleId(ref pager, roleId);
            //List<SysUserModel> _rlist = new List<SysUserModel>();
            //foreach (EAP_User u in _ulist)
            //{
            //    _rlist.Add(new SysUserModel() { Id = u.ID.ToString(), UserName = u.UserName, TrueName = u.TrueName, Flag = u.State_G });
           // }
            var jsonData = new
            {
                total = _totalCount,
                rows = _ulist.ToArray()
            };
            return Json(jsonData);
        }
        #endregion

        [SupportFilter(ActionName = "Save")]
        public JsonResult UpdateUserRoleByRoleId(string roleId, string userIds, string allIDs)
        {
            string[] arr = userIds.Split(',');
            string _uname = LoginUser.User.UserName;
            List<SqlParameter> parms = new List<SqlParameter>();
            DataProvider dp = DataProvider.GetEAP_Provider();
            string _serror = "";
            try
            {
                if (!string.IsNullOrEmpty(allIDs))
                {
                    string deletesql = string.Format("delete  from eap_userrole where roleid={0} and userid in ({1})", roleId, allIDs);
                    dp.ExecuteNonQuery(deletesql, parms.ToArray());

                }
                if (!string.IsNullOrEmpty(userIds))
                {
                    string inssql = string.Format("insert into  eap_userrole (ID,userid,roleid,addby) select SEQ_EAP_USERROLE.nextval  as id,u.id as userid,{0} as roleid , "+
                       " '{1}' as addby from eap_user u  where u.id in ({2})", roleId, _uname, userIds);
                    dp.ExecuteNonQuery(inssql, parms.ToArray());

                }

            }
            catch (Exception ee)
            {
                _serror = ee.Message;
            }


            if (string.IsNullOrEmpty (_serror))
            {
                LogHandler.WriteServiceLog(_uname, "Ids:" + arr, "成功", "分配用户", "角色设置");
                return Json(JsonHandler.CreateMessage(1, Suggestion.SetSucceed), JsonRequestBehavior.AllowGet);
            }
            else
            {
                string ErrorCol = _serror;
                LogHandler.WriteServiceLog(_uname, "Ids:" + arr, "失败", "分配用户", "角色设置");
                return Json(JsonHandler.CreateMessage(0, Suggestion.SetFail), JsonRequestBehavior.AllowGet);
            }



        }


        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
            ViewBag.Perm = GetPermission();
            EAP_Role m = new EAP_Role();
            return View(m);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(EAP_Role model)
        {
            if (model != null && ModelState.IsValid)
            {
                LoginInfo _login = LoginUser;
                CommandResult r = SaveObj<EAP_Role>(model);
                if (r.IntResult > 0)
                {
                    LogHandler.WriteServiceLog(_login.User.UserID.ToString(), "Id:" + model.ID + ",Name:" + model.RoleName, "成功", "保存", "组织结构");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.Save), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = r.Message;
                    LogHandler.WriteServiceLog(_login.User.UserID.ToString(), "Id:" + model.ID + ",Name:" + model.RoleName + "," + ErrorCol, "失败", "保存", "组织结构");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.SaveFail + ErrorCol), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.SaveFail), JsonRequestBehavior.AllowGet);
            }

            /*
            model.Id = ResultHelper.NewId;
            model.CreateTime = ResultHelper.NowTime;
            model.CreatePerson = GetUserId();
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name, "成功", "创建", "SysRole");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "创建", "SysRole");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.InsertFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.InsertFail));
            }
             */
        }
        #endregion

        #region 修改
        [SupportFilter]
        public ActionResult Edit(string id)
        {
            ViewBag.Perm = GetPermission();
            EAP_Role entity = GetObjByID<EAP_Role>(id);
            //SysRoleModel entity = m_BLL.GetById(id);
            return View("Create", entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(SysRoleModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (true)//m_BLL.Edit(ref errors, model)
                {
                    LogHandler.WriteServiceLog(GetUserName(), "Id" + model.Id + ",Name" + model.Name, "成功", "修改", "SysRole");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserName(), "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "修改", "SysRole");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail));
            }
        }
        #endregion

        #region 详细
        [SupportFilter]
        public ActionResult Details(string id)
        {
            ViewBag.Perm = GetPermission();
            EAP_Role entity = GetObjByID<EAP_Role>(id);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public JsonResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                string usid = LoginUser.User.UserName;
                EAP_Role m=new EAP_Role (){ID=int.Parse(id)};
                CommandResult r=DeleteObj<EAP_Role>(m);
               // if (m_BLL.Delete(ref errors, id))
                if (r.IntResult >0)
                {
                    LogHandler.WriteServiceLog(usid, "Id:" + id, "成功", "删除", "SysRole");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = r.Message;
                    LogHandler.WriteServiceLog(usid, "Id" + id + "," + ErrorCol, "失败", "删除", "SysRole");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail));
            }


        }
        #endregion

    }
}
