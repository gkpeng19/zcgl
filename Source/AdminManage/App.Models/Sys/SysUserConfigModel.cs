 

using System;
using System.ComponentModel.DataAnnotations;

namespace App.Models.Sys
{

    public class SysUserConfigModel
    {

        [Display(Name = "ID")]
        public string Id { get; set; }

        [Display(Name = "名称")]
        public string Name { get; set; }

        [Display(Name = "值")]
        public string Value { get; set; }

        [Display(Name = "类型")]
        public string Type { get; set; }

        [Display(Name = "状态")]
        public bool? State { get; set; }

        [Display(Name = "用户")]
        public string UserId { get; set; }



    }
}

