using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NM.Util;
using NM.Model;
using System.Runtime.Serialization;

namespace NM.Model
{
    public class AppSetting : EntityBase//<AppSetting>
    {
        public AppSetting()
        {
             
        } 

        [IgnoreDataMember]
        public String App_Title
        {
            get { return GetString("App_Title"); }
            set
            {
                SetString("App_Title", value);
            }
        }

        [IgnoreDataMember]
        public String App_Version
        {
            get { return GetString("App_Version"); }
            set
            {
                SetString("App_Version", value);
            }
        }

        [IgnoreDataMember]
        public String Customer
        {
            get { return GetString("Customer"); }
            set
            {
                SetString("Customer", value);
            }
        }

        [IgnoreDataMember]
        public String Provider
        {
            get { return GetString("Provider"); }
            set
            {
                SetString("Provider", value);
            }
        }

        [IgnoreDataMember]
        public String Surport_Mail
        {
            get { return GetString("Surport_Mail"); }
            set
            {
                SetString("Surport_Mail", value);
            }
        }

        [IgnoreDataMember]
        public String Surport_Tel
        {
            get { return GetString("Surport_Tel"); }
            set
            {
                SetString("Surport_Tel", value);
            }
        }
        [IgnoreDataMember]
        public String Surport_Web
        {
            get { return GetString("Surport_Web"); }
            set
            {
                SetString("Surport_Web", value);
            }
        } 
    }
}
