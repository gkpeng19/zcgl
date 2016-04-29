using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using NM.Util;
namespace App.Models.Sys
{
    public class SysRoleModel
    {

        public string Id { get; set; }
        [NotNullExpression]
        [Display(Name = "角色名称")]
        public string Name { get; set; }
        
        [Display(Name = "说明")]
        public string Description { get; set; }
        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }
        [Display(Name = "创建人")]
        public string CreatePerson { get; set; }
        [Display(Name = "拥有的用户")]
        public string UserName { get; set; }//拥有的用户

        public string Flag { get; set; }//用户分配角色
    }
}
