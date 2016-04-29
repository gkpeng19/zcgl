using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G.Zc.Entity.Eap
{
    public class Eap_Role : EapBaseEntity
    {
        [JsonIgnore]
        public String RoleName
        {
            get { return GetString("RoleName"); }
            set
            {
                SetValue("RoleName", value);
            }
        }

        [JsonIgnore]
        public String Description
        {
            get { return GetString("Description"); }
            set
            {
                SetValue("Description", value);
            }
        }
    }
}
