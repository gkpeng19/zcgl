using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using System.ComponentModel;

namespace App.Models.DEF
{
    public class DEF_TestCaseRelationModel
    {
        public string Id { get; set; }
        [DisplayName("主用例编码")]
        [Required(ErrorMessage = "*")]
        public string PCode { get; set; }
        [DisplayName("子用例编码")]
        [Required(ErrorMessage = "*")]
        public string CCode { get; set; }

        [DisplayName("说明")]
        [Required(ErrorMessage = "*")]
        public string ReMark { get; set; }
        public string Name { get; set; }
        [DisplayName("排序")]
        public int? Sort { get; set; }
    }
}
