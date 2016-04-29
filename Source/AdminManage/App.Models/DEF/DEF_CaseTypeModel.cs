using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using System.ComponentModel;

namespace App.Models.DEF
{
    public class DEF_CaseTypeModel
    {
        [DisplayName("编码")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Id { get; set; }
        [DisplayName("名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Name { get; set; }
        [DisplayName("上级编码")]
        public string ParentId { get; set; }

        [DisplayName("子项标识")]
        [Required(ErrorMessage = "{0}不能为空")]
        public bool IsLast { get; set; }
        public string state { get; set; }
        public string text { get; set; }
        public string id { get; set; }
        public object clildren { get; set; }
    }
}
