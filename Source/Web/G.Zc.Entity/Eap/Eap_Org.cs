using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G.Zc.Entity.Eap
{
    public class Eap_Org : EapBaseEntity
    {
        public Eap_Org()
        {
            base.TableName = "eap_org";
        }

        [JsonIgnore]
        public String Name
        {
            get { return GetString("Name"); }
            set
            {
                SetValue("Name", value);
            }
        }

        [JsonIgnore]
        public Int32 ParentID
        {
            get { return GetInt32("ParentID"); }
            set
            {
                SetValue("ParentID", value);
            }
        }

        [JsonIgnore]
        public Int32 Type
        {
            get { return GetInt32("Type"); }
            set
            {
                SetValue("Type", value);
            }
        }
    }
}
