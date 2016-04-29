using System.Collections.Generic;
using NM.Util;

namespace  NM.Model
{
    public class LookupDataItem
    {
        public LookupDataItem() { }

        public int Id { get; set; }
        //key
        public string K { get; set; }
        //value
        public string V { get; set; }
        public string P1 { get; set; }
        public string P2 { get; set; }
    }

    public class LookupData  :TJson
    {
        public LookupData()
        {
            Items = new List<LookupDataItem>();
        }

        public string this[string k]
        {
            get
            {
                LookupDataItem item = GetItem(k);

                if (null != item)
                {
                    return item.V;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                LookupDataItem item = GetItem(k);
                if (null != item)
                {
                    item.V = value;
                }
                else
                {
                    LookupDataItem l = new LookupDataItem() { K = k, V = value };
                    Items.Add(l);
                }
            }
        }

        public void Add(LookupDataItem item)
        {
            Items.Add(item);
        }

        public LookupDataItem GetItem(string sK)
        {
            LookupDataItem item = null;
            this.Items.ForEach(e =>
            {
                if (e.K == sK)
                {
                    item = e;
                    return;
                }
            });
            return item;
        }

        public List<LookupDataItem> Items { get; set; }


        public string LookupID { get; set; }
    }

    public class LookupCriteria : TJson
    {
        public LookupCriteria()
            : base()
        { }

        public LookupCriteria(string lookup_id)
            : this()
        {
            LookupID = lookup_id;
        }

        public string LookupID { get; set; }
        public string SQL { get; set; }
    }
}
