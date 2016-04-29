using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using NM.Util;

namespace NM.Server
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDataChannel" in both code and config file together.
    [ServiceContract]
    public interface IDataChannel
    {
        [OperationContract]
        string ProcessRequest(string request);
    }
}
