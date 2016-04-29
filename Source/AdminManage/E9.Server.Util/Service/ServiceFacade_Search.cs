using NM.Service;
using NM.Util;
using NM.OP;
using NM.Model;
using System;

namespace NM.Track.Service
{
    [ServiceFacadeAttribute("Entity")]
    public class ServiceFacade_Entity : ServiceFacadeBase
    { 
        [Service("Search")]
        public void Search(DataRequest request, DataResponse response, DataProvider datasource)
        {
            string entityName = request[0];
            SearchCriteria criteria = TJson.Parse<SearchCriteria>(request[1]);
            var op = EntityMetaManager.Default.GetMetaByTypeName(entityName).GetOP(request.LogIn,datasource);
            var searchResult = op.Load(criteria);
            //Type entityType = EntityMetaManager.Default.GetMetaByTypeName(entityName).EntityType; 

            //var searchType = typeof(SearchOP<>).MakeGenericType(new Type[] { entityType });

            //var searchOP = Activator.CreateInstance(searchType, request.LogIn, datasource);
            //var searchMethod = searchType.GetMethod("Search", new[] { typeof(SearchCriteria) });

            //var searchResult = searchMethod.Invoke(searchOP, new object[] { criteria }) as TJson;

            response.Value = searchResult.ToJson();
        }

        [Service("Save")]
        public void Save(DataRequest request, DataResponse response, DataProvider datasource)
        {
            string entityName = request[0];
            Type entityType = EntityMetaManager.Default.GetMetaByTypeName(entityName).EntityType;
            EntityBase entity = TJson.Parse(entityType, request[1]) as EntityBase;
            entity.Save(null, request.LogIn, datasource);
            response.Value = entity.ToJson();
        }


        [Service("DeleteObject")]
        public void DeleteObject(DataRequest request, DataResponse result, DataProvider datasource)
        {
            var tables = request[0];
            var delkeys = request[1];
            var swhere = request[2];
            result.Value = ToJson(new EntityProviderOP(request.LogIn, datasource).DeleteObject(tables, delkeys, swhere));
        }
    }
}