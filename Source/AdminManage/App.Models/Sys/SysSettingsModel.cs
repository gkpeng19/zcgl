

using System;
using System.ComponentModel.DataAnnotations;

namespace App.Models.Sys
{

    public class SysSettingsModel
    {
        [Display(Name = "ID")]
        public string Id { get; set; }
        [Display(Name = "名称")]
        public string Name { get; set; }
        [Display(Name = "类型")]
        public string Type { get; set; }
        [Display(Name = "参数")]
        public string Parameter { get; set; }
        [Display(Name = "说明")]
        public string ReMark { get; set; }


    }
}

