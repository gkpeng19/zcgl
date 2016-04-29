using System;
using System.Collections.Generic;
using NM.Util;
using NM.Log;
using NM.Model;
using System.Net;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using NM.Service;

namespace NM.OP
{
    [CustomEntityOPAttribute(typeof(App_Lookup),"Lookup")]
    public class LookupOP : EntityProviderOP<App_Lookup>
    {
        public LookupOP(LoginInfo user, DataProvider dp)
            : base(user, dp)
        {

        }

        public LookupData GetLookup(LookupCriteria criteria)
        {
            string sSQL = ""; //= criteria.SQL;

            if (!string.IsNullOrEmpty(criteria.LookupID))
            {
                sSQL = string.Format("SELECT Code AS K,Value AS V,Par1 as P1,Par2 as P2 FROM APP_LookUp WHERE LookupID='{0}' and flagDelete=0", criteria.LookupID);
                sSQL += criteria.SQL;
                sSQL += " order by  [SORTBY]";
            }

            LookupData result = new LookupData() { LookupID = string.IsNullOrEmpty(criteria.LookupID) ? string.Empty : criteria.LookupID };
            List<LookupDataItem> _Items = DataProvider.LoadData<LookupDataItem>(sSQL);
            result.Items.AddRange(_Items);
            return result;
        }

        public LookupData GetAllookupID()
        {
            string sSQL = @"select distinct lookupid as K from app_lookup
where flagdelete=0
order by lookupid";

            LookupData result = new LookupData() { LookupID = "AllLookupID" };
            var _Items = DataProvider.LoadData<LookupDataItem>(sSQL);
            result.Items.AddRange(_Items);
            return result;
        }

    }
}
