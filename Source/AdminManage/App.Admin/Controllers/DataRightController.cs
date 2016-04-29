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
using Gis.Models;

namespace App.Admin.Controllers
{
    public class DataRightController : BaseController
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

        }

        //根据角色得到可查看区域
        [SupportFilter(ActionName = "Index")]
        [HttpPost]
        public JsonResult GetClassListByRole(GridPager pager, string roleId)
        {
            List<gis_layerclass> rs = new List<gis_layerclass>();
            if (string.IsNullOrEmpty(roleId))
            {
                var pp = new
                {
                    total = 0,
                    rows = rs.ToArray()

                };
                return Json(pp);

            }
            DataProvider dp = DataProvider.GetEAP_Provider();
            List<SqlParameter> _params = new List<SqlParameter>();

            string ssql = string.Format("select a.* ,case nvl(b.id,0) when 0 then 0 else 1 end as IsClassVaild_G  from   gis_layerclass  a       left join  eap_userallowlayerclass b on a.id=b.classid  and b.roleid={0}"
                    , roleId);
            // SearchCriteria _search = new SearchCriteria("EAP_RESOURCEOP");
            // _search["parentid"] = moduleId;
            rs = dp.LoadData<gis_layerclass>(ssql, _params.ToArray());


            //SearchResult<EAP_Resource> _rs = _op.Search(_search);


            //  pager.rows = 100000;
            // var right = sysRightBLL.GetRightByRoleAndModule(roleId,moduleId);
            var json = new
            {
                total = rs.Count,
                rows = rs.ToArray()

            };

            return Json(json);
        }


        //根据角色得到可查看区域
        [SupportFilter(ActionName = "Index")]
        [HttpPost]
        public JsonResult GetLayerListByClass(GridPager pager, string clsid)
        {
            EntityProviderOP<gis_LayerClassD> _op = new EntityProviderOP<gis_LayerClassD>(LoginUser, DataProvider.GetEAP_Provider());
            SearchCriteria _search = new SearchCriteria("GIS_LAYERCLASSD");
            ToSearchCriteria(pager, ref _search);
            _search["CLASSID"] = clsid;
            SearchResult<gis_LayerClassD> _rs = _op.Search(_search);

            if (string.IsNullOrEmpty(clsid)) 
            {
                return null;
            }
            var json = new
            {
                total = _rs.TotalCount,
                rows = _rs.Items.ToArray()

            };
            return Json(json);
        }


        //根据角色得到可查看区域
        [SupportFilter(ActionName = "Index")]
        [HttpPost]
        public JsonResult GetAreaListByRole(GridPager pager, string roleId)
        {

            List<EAP_Area> rs = new List<EAP_Area>();
            if (string.IsNullOrEmpty(roleId))
            {
                var pp = new
                {
                    total = 0,
                    rows = rs.ToArray()

                };
                return Json(pp);

            }
            DataProvider dp = DataProvider.GetEAP_Provider();
            List<SqlParameter> _params = new List<SqlParameter>();

            string ssql = string.Format("select a.* ,case nvl(b.id,0) when 0 then 0 else 1 end as IsAreaVaild_G  from   eap_area  a       left join  eap_userallowarea    b on a.id=b.areaid  and b.roleid={0}"
                    , roleId);
            // SearchCriteria _search = new SearchCriteria("EAP_RESOURCEOP");
            // _search["parentid"] = moduleId;
            rs = dp.LoadData<EAP_Area>(ssql, _params.ToArray());


            //SearchResult<EAP_Resource> _rs = _op.Search(_search);


            //  pager.rows = 100000;
            // var right = sysRightBLL.GetRightByRoleAndModule(roleId,moduleId);
            var json = new
            {
                total = rs.Count,
                rows = rs.ToArray()

            };

            return Json(json);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idschecks">有权限分类</param>
        /// <param name="idsunchecks">无权限分类</param>
        /// <param name="idsArea"></param>
        /// <param name="idsunArea"></param>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        [HttpPost]
        [SupportFilter(ActionName = "Save")]
        public ActionResult UpdateDataRight(string idschecks, string idsunchecks, string idsArea, string idsunArea, string idsLayerchecks, string idsLayerunchecks, string RoleId)
        {
            int _roleid = 0;

            if (!string.IsNullOrEmpty(RoleId))
            {
                int.TryParse(RoleId, out _roleid);
            }
            LoginInfo lu = LoginUser;
            List<SqlParameter> _params = new List<SqlParameter>();
            {
                SqlParameter p = new SqlParameter() { ParameterName = "opuser", Value = lu.User.UserName };
                _params.Add(p);
            }
            {
                SqlParameter p = new SqlParameter() { ParameterName = "idsclass", Value = idschecks };
                _params.Add(p);
            }
            {
                SqlParameter p = new SqlParameter() { ParameterName = "idsunClass", Value = idsunchecks };
                _params.Add(p);
            }
            {
                SqlParameter p = new SqlParameter() { ParameterName = "idsArea", Value = idsArea };
                _params.Add(p);
            }
            {
                SqlParameter p = new SqlParameter() { ParameterName = "idsunArea", Value = idsunArea };
                _params.Add(p);
            }


            {
                SqlParameter p = new SqlParameter() { ParameterName = "idsLayerchecks", Value = idsLayerchecks };
                _params.Add(p);
            }
            {
                SqlParameter p = new SqlParameter() { ParameterName = "idsLayerunchecks", Value = idsLayerunchecks };
                _params.Add(p);
            }



            {
                SqlParameter p = new SqlParameter() { ParameterName = "proleid", Value = _roleid };
                _params.Add(p);
            }
            DataProvider dp = DataProvider.GetEAP_Provider();
            List<CommandResult> cr = dp.LoadData<CommandResult>("usp_SaveroleData", _params.ToArray());
            //  if  （!((string.IsNullOrEmpty(model.RoleID)  || string.IsNullOrEmpty(model.Id))）
            CommandResult m = new CommandResult() { IntResult = 0 };

            if (cr != null && cr.Count > 0)
            {
                return Json(cr[0]);
            }
            else
            {
                return Json(m);
            }

            // return sysRightBLL.UpdateRight(model);
        }


    }
}
