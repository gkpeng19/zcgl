using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using System.Web.Mvc;
using NM.Util;
namespace App.Models.Sys
{
    public class SysUserModel
    {
        [Display(Name = "ID")]
        
        public string Id { get; set; }
        [NotNullExpression]
        [IsCharExpression]
        [Display(Name = "用户名")]
        public string UserName { get; set; }
        [NotNullExpression]
        [StringLength(50,MinimumLength=5)]
        [System.Web.Mvc.Compare("ComparePassword", ErrorMessage = "两次密码不一致")]
        [Display(Name = "密码")]
        public string Password { get; set; }
        [System.Web.Mvc.Compare("Password", ErrorMessage = "两次密码不一致")]
        [Display(Name = "确认密码")]
        public string ComparePassword { get; set; }
        [NotNullExpression]
        [Display(Name = "真实名称")]
        public string TrueName { get; set; }
        [Display(Name = "身份证")]
        public string Card { get; set; }
        [Display(Name = "手机号码")]
        public string MobileNumber { get; set; }
        [Display(Name = "电话号码")]
        public string PhoneNumber { get; set; }
        [Display(Name = "QQ")]
        public string QQ { get; set; }
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }
        [Display(Name = "其他联系方式")]
        public string OtherContact { get; set; }
        [Display(Name = "省份")]
        public string Province { get; set; }
        [Display(Name = "城市")]
        public string City { get; set; }
        [Display(Name = "地区")]
        public string Village { get; set; }
        [Display(Name = "详细地址")]
        public string Address { get; set; }
        [Display(Name = "状态")]
        public bool? State { get; set; }
        [DateExpression]//如果填写判断是否是日期
        [Display(Name = "创建时间")]
        public DateTime? CreateTime { get; set; }
        [Display(Name = "创建人")]
        public string CreatePerson { get; set; }
        [Display(Name = "性别")]
        public string Sex { get; set; }
        [DateExpression]//如果填写判断是否是日期
        [Display(Name = "生日")]
        public DateTime? Birthday { get; set; }

        public DateTime? JoinDate { get; set; }
        [Display(Name = "婚姻状况")]
        public string Marital { get; set; }
        [Display(Name = "党派")]
        public string Political { get; set; }
        [Display(Name = "籍贯")]
        public string Nationality { get; set; }
        [Display(Name = "所在地")]
        public string Native { get; set; }
        [Display(Name = "学校")]
        public string School { get; set; }
        [Display(Name = "专业")]
        public string Professional { get; set; }
        [Display(Name = "学历")]
        public string Degree { get; set; }
        [NotNullExpression]
        [Display(Name = "部门")]
        public string DepId { get; set; }
        [NotNullExpression]
        [Display(Name = "职位")]
        public string PosId { get; set; }
        [Display(Name = "Expertise")]
        public string Expertise { get; set; }
        [Display(Name = "是否在职")]
        public string JobState { get; set; }
        [Display(Name = "照片")]
        public string Photo { get; set; }
        [Display(Name = "附件")]
        public string Attach { get; set; }
        [Display(Name = "角色")]
        public string RoleName { get; set; }//拥有的用户

        public string Flag { get; set; }//用户分配角色
    }

    public class SysUserEditModel
    {
        [Display(Name = "ID")]
        public string Id { get; set; }

        [Display(Name = "用户名")]
        public string UserName { get; set; }
     
        [Display(Name = "密码")]
        public string Password { get; set; }
     
        [Display(Name = "确认密码")]
        public string ComparePassword { get; set; }
        [NotNullExpression]
        [Display(Name = "真实名称")]
        public string TrueName { get; set; }
        [Display(Name = "身份证")]
        public string Card { get; set; }
        [Display(Name = "手机号码")]
        public string MobileNumber { get; set; }
        [Display(Name = "电话号码")]
        public string PhoneNumber { get; set; }
        [Display(Name = "QQ")]
        public string QQ { get; set; }
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }
        [Display(Name = "其他联系方式")]
        public string OtherContact { get; set; }
        [Display(Name = "省份")]
        public string Province { get; set; }

        [Display(Name = "城市")]
        public string City { get; set; }

        [Display(Name = "地区")]
        public string Village { get; set; }
        
        [Display(Name = "详细地址")]
        public string Address { get; set; }
        [Display(Name = "状态")]
        public bool? State { get; set; }
        [DateExpression]//如果填写判断是否是日期
        [Display(Name = "创建时间")]
        public DateTime? CreateTime { get; set; }
        [Display(Name = "创建人")]
        public string CreatePerson { get; set; }
        [Display(Name = "性别")]
        public string Sex { get; set; }
        [DateExpression]//如果填写判断是否是日期
        [Display(Name = "生日")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public string Birthday { get; set; }
        [DateExpression]//如果填写判断是否是日期
        [Display(Name = "加入日期")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public string JoinDate { get; set; }
        [Display(Name = "婚姻状况")]
        public string Marital { get; set; }
        [Display(Name = "党派")]
        public string Political { get; set; }
        [Display(Name = "籍贯")]
        public string Nationality { get; set; }
        [Display(Name = "所在地")]
        public string Native { get; set; }
        [Display(Name = "学校")]
        public string School { get; set; }
        [Display(Name = "专业")]
        public string Professional { get; set; }
        [Display(Name = "学历")]
        public string Degree { get; set; }
        [NotNullExpression]
        [Display(Name = "部门")]
        public string DepId { get; set; }
        [NotNullExpression]
        [Display(Name = "职位")]
        public string PosId { get; set; }
        [Display(Name = "Expertise")]
        public string Expertise { get; set; }
        [Display(Name = "是否在职")]
        public string JobState { get; set; }
        [Display(Name = "照片")]
        public string Photo { get; set; }
        [Display(Name = "附件")]
        public string Attach { get; set; }
        [Display(Name = "角色")]
        public string RoleName { get; set; }//拥有的用户
        public string CityName { get; set; }
        public string ProvinceName { get; set; }
        public string VillageName { get; set; }
        public string DepName { get; set; }
        public string PosName { get; set; }
    }
}
