using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Provider;
using System.Web.Configuration;
using System.Configuration;

namespace NM.Config
{
    public class ServiceProviderConfigSection : ConfigurationSection
    {
        [ConfigurationProperty(ConfigurationSectionName.PROVIDERS)]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection)base[ConfigurationSectionName.PROVIDERS]; }
        }

        [ConfigurationProperty(ConfigurationPropertyName.ENABLE_TRACE, DefaultValue = "false")]
        public string EnableTrace
        {
            get { return (string)base[ConfigurationPropertyName.ENABLE_TRACE]; }
            set { base[ConfigurationPropertyName.ENABLE_TRACE] = value; }
        }

        public static ServiceProviderCollection GetServiceProvider()
        {
            ServiceProviderConfigSection result = (ServiceProviderConfigSection)ConfigurationManager.GetSection(ConfigurationSectionName.OP_SERVICE);
            ServiceProviderCollection _providers = new ServiceProviderCollection();
            ProvidersHelper.InstantiateProviders(result.Providers, _providers, typeof(ServiceProvider));
            return _providers;
        }

        public static ServiceProviderCollection GetEntityProvider()
        {
            ServiceProviderConfigSection result = (ServiceProviderConfigSection)ConfigurationManager.GetSection(ConfigurationSectionName.OP_Entity);
            ServiceProviderCollection _providers = new ServiceProviderCollection();
            ProvidersHelper.InstantiateProviders(result.Providers, _providers, typeof(ServiceProvider));
            return _providers;
        }
    }

    public class ServiceProviderCollection : ProviderCollection
    {
        public new ServiceProvider this[string name]
        {
            get { return (ServiceProvider)base[name]; }
        }

        public override void Add(ProviderBase provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (!(provider is ServiceProvider))
            {
                throw new ArgumentException("Invalid provider type", "provider");
            }
            base.Add(provider);
        }
    }

    public class ServiceProvider : ProviderBase
    {
        public bool Disabled { get; set; }
        public string AssemblyName { get; set; }
        public string Category { get; set; }

        public string ServiceName { get; set; }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            if (config["Disabled"] != null)
                Disabled = config["Disabled"] == "True";
            AssemblyName = config["AssemblyName"];

            Category = config["Category"];
        }

        private static void ApplyConfig(System.Collections.Specialized.NameValueCollection config, ref string parameterValue, string configName)
        {
            if (config[configName] != null)
            {
                parameterValue = config[configName];
            }
        }
    }
}
