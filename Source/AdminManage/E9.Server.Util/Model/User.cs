using System;
using System.Collections.Generic;
using NM.Util;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations; 
#if SILVERLIGHT
#else
using System.Data.SqlClient;
using System.Data;
#endif

namespace  NM.Model
{
    //public class User : TJson//,IPrincipal,IIdentity
    //{
    //    public User()
    //    {
    //        Roles = new List<string>();
    //    }

    //    public int ID { get; set; }
    //    public string UserName { get; set; }
    //    public string TrueName { get; set; }
    //    public string Password { get; set; }
    //    public bool IsLock { get; set; }
    //    public string Email { get; set; }
    //    public string Phone1 { get; set; }
    //    public string Phone2 { get; set; }
    //    public string BarCode { get; set; }
    //    public List<string> Roles { get; set; }
    //}

    public class LoginUser : EntityBase
    {
        [IgnoreDataMember]
        public String UserName
        {
            get { return GetString("UserName"); }
            set { SetString("UserName", value); }
        }
        [IgnoreDataMember]
        public String Password
        {
            get { return GetString("Password"); }
            set { SetString("Password", value); }
        }
        [IgnoreDataMember]
        public String Password_G
        {
            get { return GetString("Password_G"); }
            set { SetString("Password_G", value); }
        }
        [IgnoreDataMember]
        public String CName
        {
            get { return GetString("CName"); }
            set { SetString("CName", value); }
        }
        [IgnoreDataMember]
        public String CAddr
        {
            get { return GetString("CAddr"); }
            set { SetString("CAddr", value); }
        }
        [IgnoreDataMember]
        public String SellPhone
        {
            get { return GetString("SellPhone"); }
            set { SetString("SellPhone", value); }
        }
        [IgnoreDataMember]
        public Int32 IndustoryType
        {
            get { return GetInt32("IndustoryType"); }
            set { SetInt32("IndustoryType", value); }
        }
        //[IgnoreDataMember]
        //public String CLicense
        //{
        //    get { return GetString("CLicense"); }
        //    set { SetString("CLicense", value); }
        //}
    }
}
