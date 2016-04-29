using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using System.ComponentModel;

namespace App.Models.DEF
{
    public class DEF_TestJobsDetailRelationModel
    {
        [DisplayName("版本号")]
        [Required(ErrorMessage = "*")]
        public string VerCode { get; set; }
        [DisplayName("主用例编码")]
        [Required(ErrorMessage = "*")]
        public string PCode { get; set; }
        [DisplayName("子用例编码")]
        [Required(ErrorMessage = "*")]
        public string CCode { get; set; }
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
    }
}
