using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Common;
using App.Models;
using Microsoft.Practices.Unity;

using App.Models.Sys;
using System;
using NM.OP;
using NM.Model;
namespace App.Admin.Controllers
{
    public class SysUserController : BaseController
    {
        //
        // GET: /SysUser/
        ValidationErrors errors = new ValidationErrors();
        [SupportFilter]
        public ActionResult Index()
        {
            ViewBag.Perm = GetPermission();
            return View();
        }
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(GridPager pager, string queryStr)
        {
            EntityProviderOP<EAP_User> _op = new EntityProviderOP<EAP_User>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("EAP_USER");
            ToSearchCriteria(pager, ref _search);
            _search["TrueName"] = queryStr;

            SearchResult<EAP_User> _rs = _op.Search(_search);


            var json = new
            {
                total = _rs.TotalCount,
                rows = _rs.Items.ToArray()

            };

            return Json(json, JsonRequestBehavior.AllowGet);

            /*
            List<SysUserModel> list = userBLL.GetList(ref pager, queryStr);
            var json = new
            {
                total = pager.totalRows,
                rows = (from r in list
                        select new SysUserModel()
                        {
                   
                            Id=r.Id,
                            UserName=r.UserName,
                            TrueName=r.TrueName,
                            MobileNumber=r.MobileNumber,
                            PhoneNumber=r.PhoneNumber,
                            QQ=r.QQ,
                            EmailAddress=r.EmailAddress,
                            OtherContact=r.OtherContact,
                            Province=r.Province,
                            City=r.City,
                            Village=r.Village,
                            Address=r.Address,
                            State=r.State,
                            CreateTime=r.CreateTime,
                            CreatePerson=r.CreatePerson.ToString(),
                            RoleName=r.RoleName
                
                        }).ToArray()

            };
            return Json(json, JsonRequestBehavior.AllowGet);
             * */
        }

        public ActionResult LookUp(string owner)
        {
            if (string.IsNullOrEmpty(owner))
            {
                ViewBag.owner = "1";
            }
            else
            {
                ViewBag.owner = owner;
            }
            return View();
        }

        #region 设置用户角色
        [SupportFilter(ActionName = "Allot")]
        public ActionResult GetRoleByUser(string userId)
        {
            ViewBag.UserId = userId;
            ViewBag.Perm = GetPermission();
            return View();
        }

        [SupportFilter(ActionName = "Allot")]
        public JsonResult GetRoleListByUser(GridPager pager, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Json(0);
            var userList = new List<SysRoleModel>();// userBLL.GetRoleByUserId(ref pager, userId);
            var jsonData = new
            {
                total = pager.totalRows,
                rows = (
                    from r in userList
                    select new SysRoleModel()
                    {
                      Id= r.Id,
                      Name= r.Name,
                      Description = r.Description,
                     // Flag = r.flag=="0"?"0":"1",
                    }
                ).ToArray()
            };
            return Json(jsonData);
        }
        #endregion
        [SupportFilter(ActionName = "Save")]
        public JsonResult UpdateUserRoleByUserId(string userId, string roleIds)
        {
            string[] arr = roleIds.Split(',');


            if (true)// (userBLL.UpdateSysRoleSysUser(userId, arr))
            {
                LogHandler.WriteServiceLog(GetUserName(), "Ids:" + roleIds, "成功", "分配角色", "用户设置");
                return Json(JsonHandler.CreateMessage(1, Suggestion.SetSucceed), JsonRequestBehavior.AllowGet);
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserName(), "Ids:" + roleIds, "失败", "分配角色", "用户设置");
                return Json(JsonHandler.CreateMessage(0, Suggestion.SetFail), JsonRequestBehavior.AllowGet);
            }
            
        }

        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
            ViewBag.Perm = GetPermission();
            ViewBag.Struct = null;// new SelectList(structBLL.GetQueryableByParentId("0"), "Value", "Text");
            //ViewBag.Areas = new SelectList(areasBLL.GetList("0"), "Id", "Name");
            EAP_User m = new EAP_User();
            return View(m);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(EAP_User model)
        {

            if (model != null && ModelState.IsValid)
            {
                LoginInfo _login = LoginUser;
                //默认密码123456
                model.Password = NM.Util.DESEncrypt.Encrypt("123456");
                
                CommandResult r = SaveObj<EAP_User>(model);
                if (r.IntResult > 0)
                {
                    LogHandler.WriteServiceLog(_login.User.UserID.ToString(), "Id:" + model.ID + ",Name:" + model.TrueName, "成功", "保存", "用户");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.Save), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = r.Message;
                    LogHandler.WriteServiceLog(_login.User.UserID.ToString(), "Id:" + model.ID + ",Name:" + model.TrueName + "," + ErrorCol, "失败", "保存", "用户");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.SaveFail + ErrorCol), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.SaveFail), JsonRequestBehavior.AllowGet);
            }
            /*
            if (model != null && ModelState.IsValid)
            {

                model.Id = ResultHelper.NewId;
                model.CreateTime = ResultHelper.NowTime;
                model.Password = ValueConvert.MD5(model.Password);
                model.CreatePerson = GetUserTrueName();
                model.State = true;
                if (userBLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + model.Id + ",Name:" + model.UserName, "成功", "创建", "用户设置");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.InsertSucceed), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + model.Id + ",Name:" + model.UserName + "," + ErrorCol, "失败", "创建", "用户设置");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.InsertFail + ErrorCol), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.InsertFail), JsonRequestBehavior.AllowGet);
            }
             */
        }
        //判断是否用户重复
        [HttpPost]
        public JsonResult JudgeUserName(string userName)
        {
            return Json("用户名已经存在！",JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 修改
        [SupportFilter]
        public ActionResult Edit(string id)
        {
            ViewBag.Perm = GetPermission();

            EAP_User o = GetObjByID<EAP_User>(id);
            ViewBag.SelectedOrgId = o.OrgId;
            return View(o);
            /*
            SysUser entity = userBLL.GetById(id);
            ViewBag.Struct = new SelectList(structBLL.GetQueryableByParentId("0"), "Value", "Text");
            ViewBag.Areas = new SelectList(areasBLL.GetList("0"), "Id", "Name");
            SysUserEditModel info = new SysUserEditModel()
            {
                Id = entity.Id,
                UserName = entity.UserName,
                TrueName = entity.TrueName,
                Card = entity.Card,
                MobileNumber = entity.MobileNumber,
                PhoneNumber = entity.PhoneNumber,
                QQ = entity.QQ,
                EmailAddress = entity.EmailAddress,
                OtherContact = entity.OtherContact,
                Province = entity.Province,
                City = entity.City,
                Village = entity.Village,
                Address = entity.Address,
                State = entity.State,
                CreateTime = entity.CreateTime,
                CreatePerson = entity.CreatePerson,
                Sex = entity.Sex,
                Birthday = ResultHelper.DateTimeConvertString(entity.Birthday),
                JoinDate = ResultHelper.DateTimeConvertString(entity.JoinDate),
                Marital = entity.Marital,
                Political = entity.Political,
                Nationality = entity.Nationality,
                Native = entity.Native,
                School = entity.School,
                Professional = entity.Professional,
                Degree = entity.Degree,
                DepId = entity.DepId,
                PosId = entity.PosId,
                Expertise = entity.Expertise,
                JobState = entity.JobState,
                Photo = entity.Photo,
                Attach = entity.Attach
            };
            return View(info);
            */
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(EAP_User model)
        {
            if (model != null && ModelState.IsValid)
            {
                LoginInfo _login = LoginUser;

                EAP_User o = GetObjByID<EAP_User>(model.ID.ToString());
                o.UserName = model.UserName;
                o.TrueName = model.TrueName;
                o.OrgId = model.OrgId;

                ///need code 
                ///

                CommandResult r = SaveObj<EAP_User>(o);
                if (r.IntResult > 0)
                {
                    LogHandler.WriteServiceLog(_login.User.UserID.ToString(), "Id:" + model.ID + ",Name:" + model.TrueName, "成功", "保存", "用户");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.Save), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = r.Message;
                    LogHandler.WriteServiceLog(_login.User.UserID.ToString(), "Id:" + model.ID + ",Name:" + model.TrueName + "," + ErrorCol, "失败", "保存", "用户");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.SaveFail + ErrorCol), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.SaveFail), JsonRequestBehavior.AllowGet);
            }
            /*
            if (info != null && ModelState.IsValid)
            {

                EAP_User o = GetObjByID<EAP_User>(info.ID.ToString());
                o.UserName = info.UserName;
                o.TrueName = info.TrueName;
                o.OrgId = info.OrgId;

                ///need code 
                ///

            
                if (userBLL.Edit(ref errors, info))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + info.Id + ",Name:" + info.UserName, "成功", "修改", "用户设置");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.EditSucceed), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + info.Id + ",Name:" + info.UserName + "," + ErrorCol, "失败", "修改", "用户设置");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail + ErrorCol), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail), JsonRequestBehavior.AllowGet);
            }
             * */
        }

        [HttpPost]
        [SupportFilter(ActionName="Edit")]
        public JsonResult ReSet(string Id,string Pwd)
        {

            if (!string.IsNullOrEmpty(Id))
            {
                LoginInfo _login = LoginUser;

                EAP_User o = GetObjByID<EAP_User>(Id);
                o.Password = NM.Util.DESEncrypt.Encrypt(Pwd);

                CommandResult r = SaveObj<EAP_User>(o);
                if (r.IntResult > 0)
                {
                    LogHandler.WriteServiceLog(_login.User.UserName.ToString(), "Id:" + Id + ",Name:" + o.TrueName, "成功", "初始化密码", "用户");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.EditSucceed), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = r.Message;
                    LogHandler.WriteServiceLog(_login.User.UserName.ToString(), "Id:" + Id + ",Name:" + o.TrueName + "," + ErrorCol, "失败", "初始化密码", "用户");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail + ErrorCol), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail), JsonRequestBehavior.AllowGet);
            }
            /*
            SysUserEditModel editModel = new SysUserEditModel();
            editModel.Id = Id;
            editModel.Password = ValueConvert.MD5(Pwd);
            if (userBLL.EditPwd(ref errors, editModel))
            {
                LogHandler.WriteServiceLog(GetUserId(), "Id:" + Id + ",密码:********", "成功", "初始化密码", "用户设置");
                return Json(JsonHandler.CreateMessage(1, Suggestion.EditSucceed), JsonRequestBehavior.AllowGet);
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserId(), "Id:" + Id + ",,密码:********" + ErrorCol, "失败", "初始化密码", "用户设置");
                return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail + ErrorCol), JsonRequestBehavior.AllowGet);
            }
             * */
        }
        #endregion

        #region 详细
        [SupportFilter]
        public ActionResult Details(string id)
        {
            ViewBag.Perm = GetPermission();

            SysUser entity = new SysUser ();// userBLL.GetById(id);
            //防止读取错误
            string CityName, ProvinceName, VillageName, DepName, PosName;
            
                CityName = "";
                ProvinceName = "";
                VillageName = "";
                DepName = "";
                PosName = "";
            
            SysUserEditModel info = new SysUserEditModel()
            {
                Id = entity.Id,
                UserName = entity.UserName,
                TrueName = entity.TrueName,
                Card = entity.Card,
                MobileNumber = entity.MobileNumber,
                PhoneNumber = entity.PhoneNumber,
                QQ = entity.QQ,
                EmailAddress = entity.EmailAddress,
                OtherContact = entity.OtherContact,
                Province = entity.Province,
                City = entity.City,
                Village = entity.Village,
                Address = entity.Address,
                State = entity.State,
                CreateTime = entity.CreateTime,
                CreatePerson = entity.CreatePerson,
                Sex = entity.Sex,
                Birthday =ResultHelper.DateTimeConvertString(entity.Birthday),
                JoinDate = ResultHelper.DateTimeConvertString(entity.JoinDate),
                Marital = entity.Marital,
                Political = entity.Political,
                Nationality = entity.Nationality,
                Native = entity.Native,
                School = entity.School,
                Professional = entity.Professional,
                Degree = entity.Degree,
                DepId = entity.DepId,
                PosId = entity.PosId,
                Expertise = entity.Expertise,
                JobState = entity.JobState,
                Photo = entity.Photo,
                Attach = entity.Attach,
                RoleName ="",// userBLL.GetRefSysRole(id),
                CityName = CityName,
                ProvinceName = ProvinceName,
                VillageName = VillageName,
                DepName = DepName,
                PosName = PosName
            };
            
            return View(info);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public JsonResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                //保护管理员不能被删除
                if (id == "admin")
                {
                    LogHandler.WriteServiceLog(GetUserName(), "尝试删除管理员", "失败", "删除", "用户设置");
                    return Json(JsonHandler.CreateMessage(0, "管理员不能被删除！"), JsonRequestBehavior.AllowGet);
                }
                EAP_User o = new EAP_User() { TrueName = id };
                CommandResult r = DeleteObj<EAP_User>(o);
                if (r.IntResult>0)//  (userBLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserName(), "Id:" + id, "成功", "删除", "用户设置");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.DeleteSucceed), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = r.Message;
                    LogHandler.WriteServiceLog(GetUserName(), "Id:" + id + "," + ErrorCol, "失败", "删除", "用户设置");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail + ErrorCol), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail), JsonRequestBehavior.AllowGet);
            }


        }
        #endregion



    }
}
