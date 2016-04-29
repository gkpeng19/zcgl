using System;
using NM.Util;

namespace NM.Diagram.Render
{
    public class SearchCriteria : SerializableData
    {
		public SearchCriteria() { }
    
        public ObjectType DataType { get; set; }
        protected override Type GetJasonDataType()
        {
            return typeof(SearchCriteria);
        }
    }
}