using System.Linq;
using System;
using System.Collections.Generic;
using NM.Util;
namespace NM.Model
{

    public interface ISearchResult
    {
        #if SILVERLIGHT
#else
        List<object> GetItems();
#endif
    }


    public class SearchResult<T> : TJson, ISearchResult where T : EntityBase
    {
        public SearchResult()
        {
            Items = new EntityList<T>();
        }
        public EntityList<T> Items { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }

        public void AddRange(IEnumerable<T> datas)
        {
            Items.AddRange(datas);
        }
        //protected override void AfterDeserialize(TJson obj)
        //{
        //    base.AfterDeserialize(obj);
        //    Items.AfterDeserialize();
        //}

        //protected override void BeforeSerialize(TJson obj)
        //{
        //    base.BeforeSerialize(obj);
        //    Items.BeforeSerialize();
        //}
#if SILVERLIGHT
#else

        public List<object> GetItems()
        {
            return Items.ConvertAll<object>(s => { return s; });
        }
#endif
        //  public SearcuCriteria Criteria { get; set; }
    }

}
