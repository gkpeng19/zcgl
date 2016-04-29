

using System;
using System.ComponentModel.DataAnnotations;
using NM.Util;
namespace App.Models.Sys
{

    public class SysModuleModel
    {
        [NotNullExpression]
        [Display(Name = "ID")]
        public string Id { get; set; }
        [NotNullExpression]
        [Display(Name = "名称")]
        public string Name { get; set; }

        [Display(Name = "别名")]
        public string EnglishName { get; set; }
        [NotNullExpression]
        [Display(Name = "上级ID")]
        public string ParentId { get; set; }
        [Display(Name = "链接")]
        public string Url { get; set; }
        [Display(Name = "图标")]
        public string Iconic { get; set; }
        [NotNullExpression]
        [IsNumberExpression]//如果填写判断是否是数字
        [Display(Name = "排序号")]
        public int? Sort { get; set; }
        [Display(Name = "说明")]
        public string Remark { get; set; }
        [Display(Name = "状态")]
        public bool Enable { get; set; }
        [Display(Name = "创建人")]
        public string CreatePerson { get; set; }
        [DateExpression]//如果填写判断是否是日期
        [Display(Name = "创建时间")]
        public DateTime? CreateTime { get; set; }
        [Display(Name = "是否最后一项")]
        public bool IsLast { get; set; }

   
        [Display(Name = "是否弹窗")]
        public bool isnewWin{ get; set; }
        public string state { get; set; }//treegrid

    }
}

