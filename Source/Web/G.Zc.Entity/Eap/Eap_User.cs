using GOMFrameWork.DataEntity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G.Zc.Entity.Eap
{
    public partial class EAP_User : EapBaseEntity
    {
        public EAP_User() { }

        [JsonIgnore]
        public int UserID
        {
            get { return GetInt32("UserID"); }
            set
            {
                SetValue("UserID", value);
            }
        }

        [JsonIgnore]
        public Int32 AccountSetID
        {
            get { return GetInt32("AccountSetID"); }
            set
            {
                SetValue("AccountSetID", value);
            }
        }

        [JsonIgnore]
        public Int32 OrgId
        {
            get { return GetInt32("OrgId"); }
            set
            {
                SetValue("OrgId", value);
            }
        }

        public String Org_Name_G
        {
            get { return GetString("Org_Name_G"); }
            set
            {
                SetValue("Org_Name_G", value);
            }
        }

        /// <summary>
        /// 组织类型 0：分店 1：总公司 2:供应商
        /// </summary>
        [JsonIgnore]
        public Int32 ORGTYPE_G
        {
            get { return GetInt32("ORGTYPE_G"); }
            set
            {
                SetValue("ORGTYPE_G", value);
            }
        }

        /// <summary>
        /// 当前组织是否启用了需要使用ukey才能登录，默认为0，不使用；
        /// </summary>
        [JsonIgnore]
        public Int32 isUseKey_G
        {
            get { return GetInt32("isUseKey_G"); }
            set
            {
                SetValue("isUseKey_G", value);
            }
        }



        [JsonIgnore]
        public String UserName
        {
            get { return GetString("UserName"); }
            set
            {
                SetValue("UserName", value);
            }
        }

        [JsonIgnore]
        public String TrueName
        {
            get { return GetString("TrueName"); }
            set
            {
                SetValue("TrueName", value);
            }
        }

        [JsonIgnore]
        public String Password
        {
            get { return GetString("Password"); }
            set
            {
                SetValue("Password", value);
            }
        }

        [JsonIgnore]
        public String ComparePassword_G
        {
            get { return GetString("ComparePassword_G"); }
            set
            {
                SetValue("ComparePassword_G", value);
            }
        }

        [JsonIgnore]
        public String BarCode
        {
            get { return GetString("BarCode"); }
            set
            {
                SetValue("BarCode", value);
            }
        }

        /// <summary>
        /// 买价是否可见
        /// </summary>
        [JsonIgnore]
        public Int32 IsBuyPrice
        {
            get { return GetInt32("IsBuyPrice"); }
            set
            {
                SetValue("IsBuyPrice", value);
            }
        }
        /// <summary>
        /// 卖价是否可见
        /// </summary>
        [JsonIgnore]
        public Int32 IsSellPrice
        {
            get { return GetInt32("IsSellPrice"); }
            set
            {
                SetValue("IsSellPrice", value);
            }
        }


        [JsonIgnore]
        public String Phone1
        {
            get { return GetString("Phone1"); }
            set
            {
                SetValue("Phone1", value);
            }
        }

        [JsonIgnore]
        public String Phone2
        {
            get { return GetString("Phone2"); }
            set
            {
                SetValue("Phone2", value);
            }
        }

        [JsonIgnore]
        public String Phone3
        {
            get { return GetString("Phone3"); }
            set
            {
                SetValue("Phone3", value);
            }
        }

        [JsonIgnore]
        public String Email
        {
            get { return GetString("Email"); }
            set
            {
                SetValue("Email", value);
            }
        }

        [JsonIgnore]
        public Boolean IsLock
        {
            get { return GetBoolean("IsLock"); }
            set
            {
                SetValue("IsLock", value);
            }
        }

        [JsonIgnore]
        public String AddBy
        {
            get { return GetString("AddBy"); }
            set
            {
                SetValue("AddBy", value);
            }
        }

        [JsonIgnore]
        public DateTime AddOn
        {
            get { return GetDateTime("AddOn"); }
            set
            {
                SetValue("AddOn", value);
            }
        }

        [JsonIgnore]
        public String EditBy
        {
            get { return GetString("EditBy"); }
            set
            {
                SetValue("EditBy", value);
            }
        }

        [JsonIgnore]
        public DateTime EditOn
        {
            get { return GetDateTime("EditOn"); }
            set
            {
                SetValue("EditOn", value);
            }
        }

        [JsonIgnore]
        public String DeleteBy
        {
            get { return GetString("DeleteBy"); }
            set
            {
                SetValue("DeleteBy", value);
            }
        }

        [JsonIgnore]
        public DateTime DeleteOn
        {
            get { return GetDateTime("DeleteOn"); }
            set
            {
                SetValue("DeleteOn", value);
            }
        }

        [JsonIgnore]
        public Boolean Flag_Delete
        {
            get { return GetBoolean("Flag_Delete"); }
            set
            {
                SetValue("Flag_Delete", value);
            }
        }

        [JsonIgnore]
        public String Card
        {
            get { return GetString("Card"); }
            set { SetValue("Card", value); }
        }

        [JsonIgnore]
        public String MobileNumber
        {
            get { return GetString("MobileNumber"); }
            set { SetValue("MobileNumber", value); }
        }

        [JsonIgnore]
        public String PhoneNumber
        {
            get { return GetString("PhoneNumber"); }
            set { SetValue("PhoneNumber", value); }
        }

        [JsonIgnore]
        public String QQ
        {
            get { return GetString("QQ"); }
            set { SetValue("QQ", value); }
        }

        [JsonIgnore]
        public String EmailAddress
        {
            get { return GetString("EmailAddress"); }
            set { SetValue("EmailAddress", value); }
        }

        [JsonIgnore]
        public String OtherContact
        {
            get { return GetString("OtherContact"); }
            set { SetValue("OtherContact", value); }
        }

        [JsonIgnore]
        public String Province
        {
            get { return GetString("Province"); }
            set { SetValue("Province", value); }
        }

        [JsonIgnore]
        public String City
        {
            get { return GetString("City"); }
            set { SetValue("City", value); }
        }

        [JsonIgnore]
        public String Village
        {
            get { return GetString("Village"); }
            set { SetValue("Village", value); }
        }

        [JsonIgnore]
        public String Address
        {
            get { return GetString("Address"); }
            set { SetValue("Address", value); }
        }

        [JsonIgnore]
        public Int32 State
        {
            get { return GetInt32("State"); }
            set { SetValue("State", value); }
        }

        public String State_G
        {
            get { return GetString("State_G"); }
            set { SetValue("State_G", value); }
        }

        [JsonIgnore]
        public String Sex
        {
            get { return GetString("Sex"); }
            set { SetValue("Sex", value); }
        }

        [JsonIgnore]
        public String Birthday
        {
            get { return GetString("Birthday"); }
            set { SetValue("Birthday", value); }
        }

        [JsonIgnore]
        public String JoinDate
        {
            get { return GetString("JoinDate"); }
            set { SetValue("JoinDate", value); }
        }

        [JsonIgnore]
        public String Marital
        {
            get { return GetString("Marital"); }
            set { SetValue("Marital", value); }
        }
        [JsonIgnore]
        public String Political
        {
            get { return GetString("Political"); }
            set { SetValue("Political", value); }
        }
        [JsonIgnore]
        public String Nationality
        {
            get { return GetString("Nationality"); }
            set { SetValue("Nationality", value); }
        }
        [JsonIgnore]
        public String Native
        {
            get { return GetString("Native"); }
            set { SetValue("Native", value); }
        }
        [JsonIgnore]
        public String SCHOOL
        {
            get { return GetString("SCHOOL"); }
            set { SetValue("SCHOOL", value); }
        }
        [JsonIgnore]
        public String Professional
        {
            get { return GetString("Professional"); }
            set { SetValue("Professional", value); }
        }
        [JsonIgnore]
        public String Degree
        {
            get { return GetString("Degree"); }
            set { SetValue("Degree", value); }
        }
        [JsonIgnore]
        public String PosId
        {
            get { return GetString("PosId"); }
            set { SetValue("PosId", value); }
        }
        [JsonIgnore]
        public String Expertise
        {
            get { return GetString("Expertise"); }
            set { SetValue("Expertise", value); }
        }

        [JsonIgnore]
        public String Photo
        {
            get { return GetString("Photo"); }
            set { SetValue("Photo", value); }
        }
        [JsonIgnore]
        public String Attach
        {
            get { return GetString("Attach"); }
            set { SetValue("Attach", value); }
        }

        [JsonIgnore]
        public String CAID
        {
            get { return GetString("CAID"); }
            set { SetValue("CAID", value); }
        }

        [JsonIgnore]
        public Boolean IsSave_G
        {
            get { return GetBoolean("IsSave_G"); }
            set
            {
                SetValue("IsSave_G", value);
            }
        }


        [JsonIgnore]
        public String RoleName_G
        {
            get { return GetString("RoleName_G"); }
            set { SetValue("RoleName_G", value); }
        }

        [JsonIgnore]
        public Int32 ALLITEMSRIGHT
        {
            get { return GetInt32("ALLITEMSRIGHT"); }
            set
            {
                SetValue("ALLITEMSRIGHT", value);
            }
        }

        [JsonIgnore]
        public Int32 DEPARTMENTID
        {
            get { return GetInt32("DEPARTMENTID"); }
            set
            {
                SetValue("DEPARTMENTID", value);
            }
        }
        [JsonIgnore]
        public Int32 SECTIONID
        {
            get { return GetInt32("SECTIONID"); }
            set
            {
                SetValue("SECTIONID", value);
            }
        }

        [JsonIgnore]
        public String DEPARTMENTName_G
        {
            get { return GetString("DEPARTMENTName_G"); }
            set
            {
                SetValue("DEPARTMENTName_G", value);
            }
        }
        [JsonIgnore]
        public String SECTIONName_G
        {
            get { return GetString("SECTIONName_G"); }
            set
            {
                SetValue("SECTIONName_G", value);
            }
        }

        [JsonIgnore]
        public string RoleIDs_G
        {
            get { return GetString("RoleIDs_G"); }
            set { SetValue("RoleIDs_G", value); }
        }
        [JsonIgnore]
        public int POrgID_G
        {
            get { return GetInt32("POrgID_G"); }
            set
            {
                SetValue("POrgID_G", value);
            }
        }
    }
}
