using System.Configuration;
using System.Web.Configuration;

namespace NM.Config
{

    public class ConfigurationSectionName
    {
        public const string PROVIDERS = "providers";
        public const string OP_SERVICE = "E9_OPService";
        public const string OP_Entity = "E9_Entity";
        public const string E9_DIAGNOSIS = "E9_Diagnosis";
        public const string E9_CACHE = "E9_Cache";
    }

    public class ConfigurationPropertyName
    {
        public const string ENABLE_TRACE = "enableTrace";
    }
}
