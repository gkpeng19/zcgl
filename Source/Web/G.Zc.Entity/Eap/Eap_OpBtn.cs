using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G.Zc.Entity.Eap
{
    public class Eap_OpBtn : EapBaseEntity
    {
        [JsonIgnore]
        public string Code
        {
            get { return GetString("Code"); }
            set { SetValue("Code", value); }
        }
        [JsonIgnore]
        public string Name
        {
            get { return GetString("Name"); }
            set { SetValue("Name", value); }
        }
    }
}
