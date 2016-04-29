using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NM.Model;
using NM.Util;

namespace NM.OP
{
    class ConfigsOP:EntityProviderOP<EAP_SYSConfig>
    {
        public ConfigsOP(LoginInfo user)
            : base(user, DataProvider.GetEAP_Provider())
        {}

        internal GCommandResult<EAP_SYSConfig, XX_Column> SaveSysConfig(EntityList<EAP_SYSConfig> configs, EntityList<XX_Column> columnConfigs)
        {
            GCommandResult<EAP_SYSConfig, XX_Column> result = new GCommandResult<EAP_SYSConfig, XX_Column>();
            try
            {
                configs.ForEach(e =>
                {
                    Save(e);
                    if (e.S == EntityStatus.Delete)//不需要重设状态   应为到了客服端状态又变成New了
                    {
                        configs.Remove(e);
                    }
                 });
                columnConfigs.ForEach(e =>
                {
                    Save(e);
                    if (e.S == EntityStatus.Delete)
                    {
                        columnConfigs.Remove(e);
                    }
                });
                result.Result = true;
                result.DatasOne = configs;
                result.DatasTwo = columnConfigs;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Message = "保存失败：" + ex.Message;
            }
            return result;
        }
    }
}
