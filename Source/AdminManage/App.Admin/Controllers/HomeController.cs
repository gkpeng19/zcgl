using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Models.Sys;
using App.Models;
using Microsoft.Practices.Unity;
 
using App.Common;
using App.Admin;
using System.Globalization;
using System.Threading;
using System.Text;
using System;
using NM.OP;
using NM.Util;
using NM.Model;
namespace App.Admin.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/
        #region UI框架

        private App.Models.Sys.siteconfig siteConfig = new App.BLL.SysConfigBLL().loadConfig(Utils.GetXmlMapPath("Configpath"));

        public ActionResult Index()
        {
            if (Session["Account"] != null)
            {
                //获取是否开启WEBIM
                ViewBag.IsEnable = siteConfig.webimstatus;
                //获取信息间隔时间
                ViewBag.NewMesTime = siteConfig.refreshnewmessage;
                //系统名称
                ViewBag.WebName = siteConfig.webname;
                //公司名称
                ViewBag.ComName = siteConfig.webcompany;
                AccountModel account = new AccountModel();
                account = (AccountModel)Session["Account"];
                return View(account);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }


        }
        /// <summary>
        /// 获取导航菜单
        /// </summary>
        /// <param name="id">所属</param>
        /// <returns>树</returns>
        public JsonResult GetTree(string id)
        {
            string appid=id;
            var _resid = 0;
            if (!string.IsNullOrEmpty(id))
            {
                int.TryParse(id, out _resid);  
            }
            var _appid = 0;
            if (!string.IsNullOrEmpty(appid))
            {
                int.TryParse(appid, out _appid);
            }

            if (Session["Account"] != null)
            {
                //AccountModel account = (AccountModel)Session["Account"];
                TJsonList<EAP_Resource> menus = null;
                if (Session["myMenu"] != null)
                {
                    menus = (Session["myMenu"]) as TJsonList<EAP_Resource>;
                }
                else
                {
                     SourceOP _SourceOP = new SourceOP(LoginUser);
                     menus = _SourceOP.GetMyMenu(_appid, 0);
                     Session["myMenu"] = menus;
                }
              
                //下级
                 // List<SysModule> _menus = homeBLL.GetMenuByPersonId(account.Id, id);
                var jsonData = (
                        from m in menus where m.ParentId==_resid  
                        select new
                        {
                            id = m.ID.ToString(),
                            text = m.Name,
                            value =m.PageId,
                            showcheck = false,
                            complete = false,
                            isexpand = false,
                            checkstate = 0,
                            hasChildren =  menus.Exists((x)=>{return x.ParentId==m.ID;}),  //需要处理是否有下级的问题；
                            Icon = m.Image
                        }
                    ).ToArray();
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public void SetThemes(string value)
        {
            //SysUserConfig entity = userConfigBLL.GetById("themes", GetUserId());
            //if (entity != null)
            //{
            //    entity.Value = value;
            //    userConfigBLL.Edit(ref errors, entity);
            //}
            //else
            //{
            //    userConfigBLL.Create(ref errors, "themes", "用户自定义主题", value, "themes", true, GetUserId());
            //}
            Session["themes"] = value;
        }

        #endregion
 
        ValidationErrors errors = new ValidationErrors();
        #region 我的资料
        public ActionResult Info()
        {
            EAP_User _currlogin=LoginUser.User;
            EAP_User info = GetObjByID<EAP_User>(_currlogin.UserID.ToString());
            
            return View(info);
        }


        [HttpPost]
        public JsonResult EditPwd(string oldPwd, string newPwd)
        {
            
               LoginInfo _login = LoginUser;

               EAP_User user = GetObjByID<EAP_User>(_login.User.ID.ToString());
            // if (user != null)
            //{
                
            //    return Json(JsonHandler.CreateMessage(0, "旧密码不匹配！"), JsonRequestBehavior.AllowGet);
            //}
                user.Password = NM.Util.DESEncrypt.Encrypt(newPwd);

                CommandResult r = SaveObj<EAP_User>(user);
           if (r.IntResult > 0) 
            {
                LogHandler.WriteServiceLog(GetUserName(), "Id:" + LoginUser.User.UserName + ",密码:********", "成功", "初始化密码", "用户设置");
                return Json(JsonHandler.CreateMessage(1, Suggestion.EditSucceed), JsonRequestBehavior.AllowGet);
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserName(), "Id:" + GetUserName() + ",,密码:********" + ErrorCol, "失败", "初始化密码", "用户设置");
                return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail + ErrorCol), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        #region webpart


        public ActionResult Doc()
        {
            SysUserConfig ss = null;// webPartBLL.GetByIdAndUserId("webpart", GetUserId());
            if (ss != null)
            {
                ViewBag.Value = ss.Value;
            }
            else
            {
                ViewBag.Value = "";
            }
            return View();
        }
        [HttpPost]
        public JsonResult GetPartData1()
        {

            return Json("待办事项", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPartData2()
        {
            return Json("站内信箱", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPartData3()
        {
            return Json("获取条数", JsonRequestBehavior.AllowGet);
            ////获取条数
            //int rows = 5;
            //SysSettings set = settingBLL.GetById("WP0001");
            //if (set != null)
            //{
            //    try
            //    {
            //        rows =int.Parse(set.Parameter);
            //    }
            //    catch {
            //        rows = 5;
            //    }
            //}
            //List<P_MIS_GetInfo_Result> list = webPartBLL.GetPartData3(rows, GetUserId());
            //StringBuilder sb = new StringBuilder("");
            //sb.Append("<table style=\"width:100%\">");
            //foreach (var i in list)
            //{
            //    sb.AppendFormat("<tr><td class=\"infolist-icon\"><a class=\"a-default\" href=\"javascript:ShowInfo('{0}','{1}')\">{2}</a></td><td style=\"width:75px\">[{3}]</td></tr>", i.Title, i.Id, i.Title, i.CreateTime.ToShortDateString());
            //}
            //sb.Append("</table>");
            //return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPartData4()
        {
            return Json("我的申请", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPartData5()
        {
            return Json("我的批准", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPartData6()
        {
            return Json("我的项目", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPartData7()
        {
            return Json("会议邀请", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPartData8()
        {
            //List<P_Mis_FileGetMyReadFile_Result> list = webPartBLL.GetPartData8(5, GetCurrentId());
            StringBuilder sb = new StringBuilder("");
            //sb.Append("<table style=\"width:100%\">");
            //foreach (var i in list)
            //{
            //    sb.AppendFormat("<tr><td class=\"sharelist-icon\"><a href=\"javascript:ShowFile('{0}','{1}')\">{2}</a></td><td style=\"width:75px\">[{3}]</td><td style='width:30px;'>{4}</td></tr>", "文件查看", "/Mis/File/File?id=" + i.Id, i.Name, Convert.ToDateTime(i.CreateTime).ToShortDateString(), " <a  class='a-default'  href=\"/Mis/File/DownFile/" + i.Id + "\">下载</a>");
            //}
            //sb.Append("</table>");
            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPartData9()
        {
            return Json("备忘录", JsonRequestBehavior.AllowGet);
        }
        ValidationErrors validationErrors = new ValidationErrors();
        [ValidateInput(false)]
        public JsonResult SaveHtml(string html)
        {
           // webPartBLL.SaveHtml(ref validationErrors, GetUserId(), html);
            return Json("保存成功!", JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
