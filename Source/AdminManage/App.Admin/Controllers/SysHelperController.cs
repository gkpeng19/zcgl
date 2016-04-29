using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using App.Common;

namespace App.Admin.Controllers
{
    public class SysHelperController : Controller
    {
        //
        // GET: /SysHelper/

        public ActionResult Index()
        {
            return View();
        }
        #region 上传图片
        //上传图片
        public ActionResult UpLoadImg(string id="1")
        {
            ViewBag.Dif = id;
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Upload(HttpPostedFileBase fileData)
        {
            if (fileData != null)
            {
                try
                {
                    // 文件上传后的保存路径
                    string filePath = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    string fileName = Path.GetFileName(fileData.FileName);// 原始文件名称
                    string fileExtension = Path.GetExtension(fileName); // 文件扩展名
                    string saveName = ResultHelper.NewId + fileExtension; // 保存文件名称

                    fileData.SaveAs(filePath + saveName);

                    return Json(new { Success = true, FileName = fileName, SaveName = saveName, FilePath = "/Uploads/"+saveName }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {

                return Json(new { Success = false, Message = "请选择要上传的文件！" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        //导出时候读取报表
        public ActionResult ReportControl()
        {
            return View();
        }
        //万能查询
        public ActionResult Query()
        {
            return View();
        }
    }
}
