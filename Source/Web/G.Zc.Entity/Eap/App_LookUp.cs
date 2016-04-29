using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G.Zc.Entity.Eap
{
    public class App_LookUp : EapBaseEntity
    {
        public App_LookUp()
        {
            base.TableName = "App_LookUp";
        }

        [JsonIgnore]
        public string Code
        {
            get { return GetString("Code"); }
            set { SetValue("Code", value); }
        }
        [JsonIgnore]
        public string Value
        {
            get { return GetString("Value"); }
            set { SetValue("Value", value); }
        }
    }
}
