 

using System;
using System.ComponentModel.DataAnnotations;
using NM.Util;
namespace App.Models.Sys
{

    public class SysStructModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        [NotNullExpression]
        [IsCharExpression]
        [Display(Name = "机构编码")]
        public string Code { get; set; }

        [NotNullExpression]
        [Display(Name = "名称")]
        public string Name { get; set; }
        [NotNullExpression]
        [Display(Name = "上级机构")]
        public string ParentId { get; set; }

        [Required(ErrorMessage = "{0}必须是一个数字")]
        [IsNumberExpression]//如果填写判断是否是数字
        [Display(Name = "排序号")]
        public int Sort { get; set; }
        [Display(Name = "领导")]
        public string Higher { get; set; }
        [Display(Name = "状态")]
        public bool Enable { get; set; }
        [Display(Name = "说明")]
        public string Remark { get; set; }

       // [DateExpression]//如果填写判断是否是日期
        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }

        public string  state { get; set; }

        public int objstate { get; set; }
    }
}

