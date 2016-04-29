using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
 
using App.Models;
using App.Common;
using App.Models.Sys;
using NM.OP;
using NM.Model;
using System.Data.SqlClient;
using App.Admin.Core;

namespace App.Admin.Controllers
{
    public class SysRightController : BaseController
    {

        [SupportFilter]
        public ActionResult Index()
        {
            ViewBag.Perm = GetPermission();
            return View();
        }
        //获取角色列表
        [SupportFilter(ActionName = "Index")]
        [HttpPost]
        public JsonResult GetRoleList(GridPager pager)
        {

            EntityProviderOP<EAP_Role> _op = new EntityProviderOP<EAP_Role>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("EAP_ROLE");
            ToSearchCriteria(pager, ref _search);
           // _search["RoleName"] = queryStr;

            SearchResult<EAP_Role> _rs = _op.Search(_search);


            var json = new
            {
                total = _rs.TotalCount,
                rows = _rs.Items.ToArray()

            };

            return Json(json);
            
            /*
            List<SysRoleModel> list = sysRoleBLL.GetList(ref pager, "");
            var json = new
            {
                total = pager.totalRows,
                rows = (from r in list
                        select new SysRoleModel()
                        {

                            Id = r.Id,
                            Name = r.Name,
                            Description = r.Description,
                            CreateTime = r.CreateTime,
                            CreatePerson = r.CreatePerson

                        }).ToArray()

            };

            return Json(json);
             */
        }

        //获取模组列表
        [SupportFilter(ActionName = "Index")]
        public string  GetModelList2(string ParentId, string RoleID)
        {
            if (ParentId == null)
                ParentId = "0";
            EntityProviderOP<EAP_Resource> _op = new EntityProviderOP<EAP_Resource>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("EAP_MOUDLE");
            _search["parentid"] = ParentId;

            SearchResult<EAP_Resource> _rs = _op.Search(_search);

            // var  list = m_BLL.GetListByParentId(id);             

            List<TreeBase> _ls = new List<TreeBase>();
            foreach (EAP_Resource r in _rs.Items)
            {
                
                    TreeBase c = new TreeBase();
                    c.id = r.ID;
                    c.text = r.Name ;
                    c.ischeck = r.Checked;
                    c.state = (r.HasChild_G == 1) ? "closed" : "open";
                    _ls.Add(c);
                


            }
            string mm = Newtonsoft.Json.JsonConvert.SerializeObject(_ls);
           // string  Newtonsoft.Json.JsonConvert 
           // JsonResult  jr=  Json(_ls, JsonRequestBehavior.AllowGet);
          
            return mm  ;
        }
         //获取模组列表
        [SupportFilter(ActionName = "Index")]
        [HttpPost]
        public JsonResult GetModelList(string id)
         {
             if (id == null)
                 id = "0";
             EntityProviderOP<EAP_Resource> _op = new EntityProviderOP<EAP_Resource>(LoginUser, DataProvider.GetEAP_Provider());
             SearchCriteria _search = new SearchCriteria("EAP_MOUDLE");
             _search["parentid"] = id;

             SearchResult<EAP_Resource> _rs = _op.Search(_search);

             // var  list = m_BLL.GetListByParentId(id);             

             var json = new List<SysModuleModel>();
             foreach (EAP_Resource r in _rs.Items)
             {
                 SysModuleModel _m = new SysModuleModel()
                 {
                     Id = r.ID.ToString(),
                     Url = r.PageId,
                     Name = r.Name,
                     ParentId = r.ParentId.ToString(),
                     Sort = r.SortBy,
                     Enable = !r.Flag_Delete,
                     CreateTime = r.AddOn,
                     IsLast = (r.HasChild_G == 0),
                     state = (r.HasChild_G == 1) ? "closed" : "open"
                 };
                 json.Add(_m);
             }

             return Json(json);
         }

         //根据角色与模块得出权限
        [SupportFilter(ActionName = "Index")]
        [HttpPost]
        public JsonResult GetRightByRoleAndModule(GridPager pager, string roleId, string moduleId)
         {
             if (string.IsNullOrEmpty(moduleId))
                 moduleId = "0";

             DataProvider dp = DataProvider.GetEAP_Provider();
             List<SqlParameter> _params=new  List<SqlParameter>();

             string ssql = string.Format("select r.*,case nvl(a.aid,0) when 0 then 0 else 1 end as IsVaild_G  from eap_resource  r " +
                     "left join (select sourceid as aid  from eap_rolemodule where roleid={0}) a on r.id=a.aid where r.parentid={1}"
                     , roleId, moduleId);
            // SearchCriteria _search = new SearchCriteria("EAP_RESOURCEOP");
            // _search["parentid"] = moduleId;
             List<EAP_Resource> rs = dp.LoadData<EAP_Resource>(ssql, _params.ToArray());


             //SearchResult<EAP_Resource> _rs = _op.Search(_search);


           //  pager.rows = 100000;
            // var right = sysRightBLL.GetRightByRoleAndModule(roleId,moduleId);
             var json = new
             {
                 total = rs.Count ,
                 rows = rs.ToArray()

             };

             return Json(json);
         }
        //保存
        [HttpPost]
        [SupportFilter(ActionName = "Save")]
        public ActionResult UpdateRight(string idschecks, string idsunchecks, string PMID, string RoleId)
        {

            int  _mid=0;
            int  _roleid=0;

            if (!string.IsNullOrEmpty (PMID))
            {
                   int.TryParse(PMID,out _mid);
            }

            if (!string.IsNullOrEmpty (RoleId))
            {
                   int.TryParse(RoleId,out _roleid);
            }
            LoginInfo lu = LoginUser;
            List<SqlParameter> _params=new  List<SqlParameter>();
            {
                SqlParameter p = new SqlParameter() { ParameterName = "opuser", Value = lu.User.UserName };
                _params.Add(p);
            }
            {
               SqlParameter  p=new SqlParameter(){ParameterName="idschecks",Value=idschecks};
                _params.Add(p);
            }
             {
               SqlParameter  p=new SqlParameter(){ParameterName="idsunchecks",Value=idsunchecks};
                _params.Add(p);
            }
             {
               SqlParameter  p=new SqlParameter(){ParameterName="pmid",Value=_mid};
                _params.Add(p);
            }
             {
               SqlParameter  p=new SqlParameter(){ParameterName="proleid",Value=_roleid};
                _params.Add(p);
            }
            DataProvider dp = DataProvider.GetEAP_Provider();
             List<CommandResult> cr=  dp.LoadData<CommandResult>("usp_Saveroleop", _params.ToArray());
            //  if  （!((string.IsNullOrEmpty(model.RoleID)  || string.IsNullOrEmpty(model.Id))）
             CommandResult m = new CommandResult() { IntResult = 0 };
            
             if (cr != null && cr.Count > 0)
             {
                return  Json(cr[0]);
             }
             else
             {
                 return Json(m);
             }
            
           // return sysRightBLL.UpdateRight(model);
        }


    }
}
