using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using System.ComponentModel;

namespace App.Models.DEF
{
    public class DEF_TestJobsDetailItemModel
    {

        [DisplayName("版本号")]
        [Required(ErrorMessage = "*")]
        public string VerCode { get; set; }
        [DisplayName("用例编码")]
        [Required(ErrorMessage = "*")]
        public string Code { get; set; }
        [DisplayName("名称")]
        [Required(ErrorMessage = "*")]
        public string Name { get; set; }
        [DisplayName("说明")]
        public string Description { get; set; }
        [DisplayName("测试结果")]
        public bool? Result { get; set; }
        [DisplayName("排序")]
        public int? Sort { get; set; }
        [DisplayName("执行序号")]
        public int? ExSort { get; set; }
        [DisplayName("锁定状态")]
        public bool Lock { get; set; }

        [DisplayName("开发者")]
        public string Developer { get; set; }
        [DisplayName("测试者")]
        public string Tester { get; set; }
        [DisplayName("实际完成时间")]
        public DateTime? FinDt { get; set; }
        [DisplayName("开发完成标志")]
        public bool? DevFinFlag { get; set; }
        [DisplayName("请求测试标志")]
        public bool? TestRequestFlag { get; set; }

    }
}
