using System;
using System.Collections.Generic;
using System.Text;

namespace NM.Service
{
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute(string serviceName)
        {
            ServieName = serviceName;
        }

        public string ServieName { get; set; }
    }
}
