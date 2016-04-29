using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace App.Models.MIS
{
    public class MIS_WebIM_RecentContactModel
    {
        [DisplayName("ID")]
        [Required(ErrorMessage = "*")]
        public int Id { get; set; }
        [DisplayName("联系人")]
        [Required(ErrorMessage = "*")]
        public string ContactPersons { get; set; }
        [DisplayName("用户")]
        [Required(ErrorMessage = "*")]
        public string UserId { get; set; }
        [DisplayName("最后通信时间")]
        [Required(ErrorMessage = "*")]
        public DateTime InfoTime { get; set; }
        [DisplayName("联系人")]
        public string ContactPersonsTitle { get; set; }
    }
}
