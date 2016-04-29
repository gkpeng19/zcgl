using System;
using NM.Util;

namespace NM.Model
{
    [Serializable]
    public class MetaBase : TJson
    {
        public int ID { get; set; }
        public DateTime AddOn { get; set; }
        public string AddBy { get; set; }
        public string Permission { get; set; }
        public int Version { get; set; }
    }
}
