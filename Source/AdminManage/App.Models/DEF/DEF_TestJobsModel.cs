using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using System.ComponentModel;

namespace App.Models.DEF
{
    public class DEF_TestJobsModel
    {
        [DisplayName("版本号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string VerCode { get; set; }
        [DisplayName("名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Name { get; set; }
        [DisplayName("测试通过")]
        public bool? Result { get; set; }
        [DisplayName("说明")]
        public string Description { get; set; }
        [DisplayName("创建人")]
        public string Creator { get; set; }
        [DisplayName("创建人")]
        public string CreatorTitle { get; set; }
        [DisplayName("创建日期")]
        public DateTime? CrtDt { get; set; }
        [DisplayName("关闭状态")]
        public bool? CloseState { get; set; }
        [DisplayName("关闭人")]
        public string Closer { get; set; }
        [DisplayName("关闭人")]
        public string CloserTitle { get; set; }
        [DisplayName("关闭日期")]
        public DateTime? CloseDt { get; set; }
        [DisplayName("默认任务")]
        public bool? Def { get; set; }
        [DisplayName("锁定")]
        public bool? CheckFlag { get; set; }

    }
}
