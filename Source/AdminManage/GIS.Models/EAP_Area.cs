
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using NM.Model;
using System.Runtime.Serialization;
namespace Gis.Models
{
    /*EAP_Area
    *
    * exec ESP_ClassCodeGenerator  EAP_Area,1
    */
    public partial class EAP_Area : EntityBase//<EAP_Area>
    {
        [IgnoreDataMember]
        public Int32 ID
        {
            get { return GetInt32("ID"); }
            set { SetInt32("ID", value); }
        }
        [IgnoreDataMember]
        public String AREACODE
        {
            get { return GetString("AREACODE"); }
            set { SetString("AREACODE", value); }
        }
        [IgnoreDataMember]
        public String AREANAME
        {
            get { return GetString("AREANAME"); }
            set { SetString("AREANAME", value); }
        }
        [IgnoreDataMember]
        public String PCODE
        {
            get { return GetString("PCODE"); }
            set { SetString("PCODE", value); }
        }
        [IgnoreDataMember]
        public String ADDBY
        {
            get { return GetString("ADDBY"); }
            set { SetString("ADDBY", value); }
        }
        [IgnoreDataMember]
        public DateTime ADDON
        {
            get { return GetDateTime("ADDON"); }
            set { SetDateTime("ADDON", value); }
        }
        [IgnoreDataMember]
        public String EDITBY
        {
            get { return GetString("EDITBY"); }
            set { SetString("EDITBY", value); }
        }
        [IgnoreDataMember]
        public DateTime EDITON
        {
            get { return GetDateTime("EDITON"); }
            set { SetDateTime("EDITON", value); }
        }
        [IgnoreDataMember]
        public String DELETEBY
        {
            get { return GetString("DELETEBY"); }
            set { SetString("DELETEBY", value); }
        }
        [IgnoreDataMember]
        public DateTime DELETEON
        {
            get { return GetDateTime("DELETEON"); }
            set { SetDateTime("DELETEON", value); }
        }
        [IgnoreDataMember]
        public Int32 FLAGDELETE
        {
            get { return GetInt32("FLAGDELETE"); }
            set { SetInt32("FLAGDELETE", value); }
        }
        [IgnoreDataMember]
        public Int32 ODERNUM
        {
            get { return GetInt32("ODERNUM"); }
            set { SetInt32("ODERNUM", value); }
        }

        [IgnoreDataMember]
        public Int32 HasChild_G
        {
            get { return GetInt32("HasChild_G"); }
            set
            {
                SetInt32("HasChild_G", value);
            }
        }
         [IgnoreDataMember]
        public Boolean IsAreaVaild_G
        {
            get { return GetBoolean("IsAreaVaild_G"); }
            set
            {
                SetBoolean("IsAreaVaild_G", value);
            }
        }
    }


    public class EAP_Area2 : EAP_Area
    {
        [IgnoreDataMember]
        public String state
        {
            get { return GetString("state"); }
            set { SetString("state", value); }
        }
    }
}
