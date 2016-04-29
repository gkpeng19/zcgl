using System;
using System.Collections.Generic;
using System.Text;

namespace NM.Service
{
    public class ServiceFacadeAttribute : Attribute
    {
        public static string DefalutCategory = "Defalut";
        
        public ServiceFacadeAttribute( )
        {
            Category = DefalutCategory;
        }

        public ServiceFacadeAttribute(string category)
        {
            if (!string.IsNullOrEmpty(category))
                Category = category;
            else
                Category = DefalutCategory;
        }


        public string Category { get; set; }
    }
}
