using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.Sys
{
    public class SysRightUserRight
    {
        public string ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string KeyCode { get; set; }
    }
    public class SysRightRoleRight
    {

    }
    public class SysRightModuleRight
    {

    }

    public class UserRight
    {
        public string Ids { get; set; }
        public string UserName { get; set; }
        public string RightList { get; set; }
    }
    public class RoleRight
    {
        public string Ids { get; set; }
        public string RoleName { get; set; }
        public string RightList { get; set; }
    }
}
