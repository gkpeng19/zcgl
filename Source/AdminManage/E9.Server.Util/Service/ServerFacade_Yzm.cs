using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NM.OP;

namespace NM.Service
{
    [ServiceFacadeAttribute("YZM")]
    public class ServerFacade_Yzm : ServiceFacadeBase
    {
        [Service("GenerateYZM")]
        public void GenerateYZM(ServiceContext context)
        {
            context.E9_Response.Value = ToJson(new YzmOP().GenerateYZM());
        }
    }
}
