using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Provider;
using System.Configuration;
using System.Web.Configuration;
using NM.Util;

namespace NM.Config
{

    public class CacheProviderConfigSection : ConfigurationSection
    {
        [ConfigurationProperty(ConfigurationSectionName.PROVIDERS)]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection)base[ConfigurationSectionName.PROVIDERS]; }
        }

        public static CacheProviderCollection GetServiceProvider()
        {
            CacheProviderConfigSection result = (CacheProviderConfigSection)ConfigurationManager.GetSection(ConfigurationSectionName.E9_CACHE);
            CacheProviderCollection _providers = new CacheProviderCollection();
            ProvidersHelper.InstantiateProviders(result.Providers, _providers, typeof(CacheProvider));
            return _providers;
        }
    }

    public class CacheProviderCollection : ProviderCollection
    {
        public new CacheProvider this[string name]
        {
            get { return (CacheProvider)base[name]; }
        }

        public override void Add(ProviderBase provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (!(provider is CacheProvider))
            {
                throw new ArgumentException("Invalid provider type", "provider");
            }
            base.Add(provider);
        }
    }

    public class CacheProvider : ProviderBase
    {
        public bool Disabled { get; set; }
        public string Category { get; set; }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            if (config["Disabled"] != null)
                Disabled = config["Disabled"] == "True";
            Category = config["Category"];
        }

        public string GetCatheSummary()
        {
            return DoGetCatheSummary();
        }

        protected virtual string DoGetCatheSummary()
        {
            return "没有缓存内容";
        }

        public bool ClearCathe()
        {
            return DoClearCathe();
        }

        protected virtual bool DoClearCathe()
        {
            return false;
        }
    }

    public class SysCacheProvider : CacheProvider
    {
        protected override string DoGetCatheSummary()
        {
            return AppCache.ToString();
        }

        protected override bool DoClearCathe()
        {
            AppCache.Clear();
            return base.DoClearCathe();
        }
    }
}
