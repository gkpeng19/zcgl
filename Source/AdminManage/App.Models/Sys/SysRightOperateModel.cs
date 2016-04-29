using System;
using System.ComponentModel.DataAnnotations;
using App.Models;
namespace App.Models.Sys
{
    public class SysRightOperateModel
    {
        public string Id { get; set; }
        public string RoleID { get; set; }
        public string KeyCode { get; set; }
        public bool IsValid { get; set; }

    }
}
