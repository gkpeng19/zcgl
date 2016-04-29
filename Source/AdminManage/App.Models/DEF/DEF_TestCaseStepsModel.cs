using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using System.ComponentModel;
namespace App.Models.DEF
{
   public  class DEF_TestCaseStepsModel
   {
       [DisplayName("ID")]
       public string ItemID { get; set; }
       [DisplayName("用例编码")]
       [Required(ErrorMessage = "*")]
       public string Code { get; set; }
       [DisplayName("标题")]
       [Required(ErrorMessage = "*")]
       public string Title { get; set; }
       [DisplayName("测试内容")]
       public string TestContent { get; set; }
       [DisplayName("状态")]
       public bool? state { get; set; }
       [DisplayName("排序")]
       public int? sort { get; set; }


    }
}
