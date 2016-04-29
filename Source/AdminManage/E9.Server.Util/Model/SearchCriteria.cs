using System;
using System.Collections.Generic;
using NM.Util;

namespace NM.Model
{
    public class SearchCriteria : SerializableData//EntityBase//<SearchCriteria>
    {
        public SearchCriteria()
        {
            IncludeChildren = false;
        }

        public SearchCriteria(string searchID)
            : this()
        {
            SearchID = searchID;
        }

        public string SearchID { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public SearchCriteria SearchItem { get; set; }
        public bool IncludeChildren { get; set; }
        public string ClassOfOP { get; set; }
        public string order { get; set; }//排序方式
        public string sort { get; set; }//排序列
        protected override void SetValue(string key, object value, TypeCode tc)
        {
            //对查询内容进行sql注入校验；先检查 ' ''
            if ((value != null) && (tc == TypeCode.String))
            {
                value = value.ToString().Replace("'", "").Replace(" ", "");//将‘ 空格 清除，避免sql注入问题
            }
            base.SetValue(key, value, tc);
        }
    }
}
