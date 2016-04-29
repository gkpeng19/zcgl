using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Models.Sys
{
    public class AccountModel
    {
        public string Id { get; set; }
        public string TrueName { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }
        public int OrgID { get; set; }

    }
}
