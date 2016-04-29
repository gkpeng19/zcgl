using System.Collections.Generic;
using NM.Model;
using NM.OP;
using System.Text;

namespace NM.Util
{
    public class AppCache
    {
        private static readonly object lockHelper = new object();
        static Dictionary<string, EAP_ErrorMsg> _ErrorMsgCache=null ;
        static Dictionary<string, SearchMeta> _SearchMetaCache=null ;
        public static SearchMeta GetSearchMeta(string searchID)
        {
            searchID = searchID.ToUpper();
            if (_SearchMetaCache == null)
            {
                _SearchMetaCache = new Dictionary<string, SearchMeta>();
            }

            if (!_SearchMetaCache.ContainsKey(searchID))
            {
                SearchMetaOP op = new SearchMetaOP(DataProvider.GetEAP_Provider());
                SearchMeta meta = op.GetSearchMeta(searchID);

                if (meta != null)
                {
                    _SearchMetaCache.Add(searchID, meta);
                }
                else
                {
                    return null;
                }
            }

            return _SearchMetaCache[searchID];
        }
        public static EAP_ErrorMsg GetErrorMsg(string ErrorName)
        {
            string  _ErrorName = ErrorName.ToUpper();

            if (_ErrorMsgCache == null)
            {
                lock (lockHelper)
                {
                    if (_ErrorMsgCache == null)
                    {
                        _ErrorMsgCache = new Dictionary<string, EAP_ErrorMsg>();
                        //加载数据
                        SearchMetaOP op = new SearchMetaOP(DataProvider.GetEAP_Provider());
                        List<EAP_ErrorMsg> _ls = op.GetErrorMsgConfig();
                        if (_ls != null)
                        {
                            foreach (EAP_ErrorMsg emsg in _ls)
                            {
                                _ErrorMsgCache.Add(emsg.ErrorName, emsg);

                            }
                        }
                    }
                }

            }

            if (_ErrorMsgCache.ContainsKey(_ErrorName))
            {
                return _ErrorMsgCache[_ErrorName];
            }
            return null;

           
           
        }

        public static void Clear()
        {
            if (_SearchMetaCache != null && _SearchMetaCache.Count > 0)
            {
                _SearchMetaCache.Clear();
            }
        }

        public static string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (_SearchMetaCache != null && _SearchMetaCache.Count > 0)
            {
                sb.AppendLine(" SearchCriteria   " + _SearchMetaCache.Count.ToString());

                foreach (var item in _SearchMetaCache)
                {
                    sb.AppendLine(string.Format("     {0}", item.Key));
                }
            }
            return sb.ToString();
        }
    }
}
