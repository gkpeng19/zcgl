using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NM.Util;
using System.Runtime.Serialization;

namespace NM.Model
{ 
    public partial class CacheMeta : EntityBase//<CacheMeta>
    {
        [IgnoreDataMember]
        public String GroupID
        {
            get { return GetString("GroupID"); }
            set
            {
                SetString("GroupID", value);
            }
        }

        [IgnoreDataMember]
        public String Name
        {
            get { return GetString("Name"); }
            set
            {
                SetString("Name", value);
            }
        }
 
    }
}
