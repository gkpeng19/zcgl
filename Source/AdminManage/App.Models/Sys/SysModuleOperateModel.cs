 

using System;
using System.ComponentModel.DataAnnotations;
using NM.Util;
namespace App.Models.Sys
{

    public class SysModuleOperateModel
    {
        [Display(Name = "ID")]
        public string Id { get; set; }
        [Display(Name = "操作名称")]
        public string Name { get; set; }
        [Display(Name = "操作码")]
        public string KeyCode { get; set; }
        [Display(Name = "所属模块")]
        public string ModuleId { get; set; }
        [Display(Name = "是否验证")]
        public bool IsValid { get; set; }
        [Required(ErrorMessage = "{0}必须填写")]
        [IsNumberExpression]//如果填写判断是否是数字
        [Display(Name = "排序号")]
        public int Sort { get; set; }


    }
}

