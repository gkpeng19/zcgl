using System.Collections.Generic;
using System.Data; 
using NM.OP;
using NM.Util;
using NM.Model;

namespace NM.OP
{
    public class SearchMetaOP : NamedProviderOP
    {
        public SearchMetaOP(DataProvider dp)
            : base(dp)
        {

        }

        public SearchMeta GetSearchMeta(string searchID)
        {
            string pp = searchID.ToUpper();
            string strsql = string.Format(" select * from EAP_Search where upper(searchId)='{0}' and FlagDelete=0", pp);
            SearchMeta result = DataProvider.GetEntity<SearchMeta>(strsql);
            if (result != null)
            {
                strsql = string.Format(" select * from EAP_SearchField where upper(searchId)='{0}' and FlagDelete=0", pp);
                result.Add(DataProvider.LoadData<SearchField>(strsql));
            }
            return result;
        }

        public List<EAP_ErrorMsg> GetErrorMsgConfig()
        {
           // string pp = searchID.ToUpper();
            string strsql = string.Format(" select * from EAP_ErrorMsg ");
            List<EAP_ErrorMsg> _errormsgls = DataProvider.LoadData<EAP_ErrorMsg>(strsql);

            return _errormsgls;
        }
    }
}
