using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Provider;
using System.Configuration;
using System.Web.Configuration;
using NM.Model;
using NM.Util;
using NM.OP;
using NM.Service;

namespace NM.Config
{

    public class DiagnosticProviderConfigSection : ConfigurationSection
    {
        [ConfigurationProperty(ConfigurationSectionName.PROVIDERS)]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection)base[ConfigurationSectionName.PROVIDERS]; }
        }

        public static DiagnosticProviderCollection GetServiceProvider()
        {
            DiagnosticProviderConfigSection result = (DiagnosticProviderConfigSection)ConfigurationManager.GetSection(ConfigurationSectionName.E9_DIAGNOSIS);
            DiagnosticProviderCollection _providers = new DiagnosticProviderCollection();
            ProvidersHelper.InstantiateProviders(result.Providers, _providers, typeof(DiagnosticProvider));
            return _providers;
        }
    }

    public class DiagnosticProviderCollection : ProviderCollection
    {
        public new DiagnosticProvider this[string name]
        {
            get { return (DiagnosticProvider)base[name]; }
        }

        public override void Add(ProviderBase provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (!(provider is DiagnosticProvider))
            {
                throw new ArgumentException("Invalid provider type", "provider");
            }
            base.Add(provider);
        }
    }

    public class DiagnosticProvider : ProviderBase
    {
        public bool Disabled { get; set; }
        public string Category { get; set; }
        public TJsonList<DiagnoseResult> Reuslt { get; set; }
        public ServiceContext ServiceContext { get; set; }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            if (config["Disabled"] != null)
                Disabled = config["Disabled"] == "True";
            Category = config["Category"];
        }

        public void Diagnose()
        {
            Reuslt = new TJsonList<DiagnoseResult>();
            DoDiagnose();
        }

        protected virtual void DoDiagnose()
        {

        }
        protected void AddDiagnosic(string name, bool result, string message, DateTime begin, DateTime end)
        {
            AddDiagnosic(new DiagnoseResult()
            {
                Message = message,
                Result = result ? 1 : 0,
                Name = name,
                BeginTime = begin,
                EndTime = end
            });
        }

        protected void AddDiagnosic(DiagnoseResult r)
        {
            Reuslt.Add(r);
        }
    }

    public class SystemDiagnosticProvider : DiagnosticProvider
    {
        protected override void DoDiagnose()
        {
            AddDiagnosic("网络连接", true, "通过", DateTime.Now, DateTime.Now);
            for (int index = 1; index < ConfigurationManager.ConnectionStrings.Count; index++)
            {
                DateTime begin = DateTime.Now;
                ConnectionStringSettings conn = ConfigurationManager.ConnectionStrings[index];
                DataAccess da = new DataAccess(conn);
                bool connected = da.Test();
                AddDiagnosic("数据库连接", connected, conn.Name + (connected ? "--连接成功" : "--连接失败"), begin, DateTime.Now);

                begin = DateTime.Now;

                try
                {
                    DataProvider dp = DataProvider.GetProvider(conn.Name);
                    base.Reuslt.AddRange(dp.LoadData<DiagnoseResult>("usp_Diagnose"));
                }
                catch (Exception ex)
                {
                    AddDiagnosic("数据库诊断", false, ex.Message, begin, DateTime.Now);
                }
            }
        }
    }
}
