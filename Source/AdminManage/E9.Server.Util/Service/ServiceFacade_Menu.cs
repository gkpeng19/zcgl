using NM.Service;
using NM.Util;
using NM.Model;

namespace NM.OP
{
    [ServiceFacadeAttribute]
    public class ServiceFacade_Menu : ServiceFacadeBase
    {
        [Service("GetAllResource")]
        public void GetAllResource(DataRequest request, DataResponse result, DataProvider datasource)
        {
            string parent_id = request["P0"];
            if (string.IsNullOrEmpty(parent_id))
                parent_id = "0";
            int _isu = 1;
            int _uid = 0;
            string _isuser =  request["P1"];
            string _userid =  request["P2"];
            if (!string.IsNullOrEmpty(_isuser))
            {
                _isu = int.Parse(_isuser);
            }
            if (!string.IsNullOrEmpty(_userid))
            {
                _uid = int.Parse(_userid);
            }

            result.Value = ToJson(new SourceOP(request.LogIn).GetAllResource(int.Parse(parent_id), _isu, _uid));
        }

        [Service("GetMyMenu")]
        public void GetMyMenu(DataRequest request, DataResponse result, DataProvider datasource)
        {
            result.Value = ToJson(new SourceOP(request.LogIn).GetMyMenu());
        }

        [Service("GetNavigator")]
        public void GetNavigator(DataRequest request, DataResponse result, DataProvider datasource)
        {
            int appID = int.Parse(request["P0"]);
            result.Value = ToJson(new SourceOP(request.LogIn).GetNavigator(appID));
        }

        [Service("GetLookUp")]
        public void GetLookUp(DataRequest request, DataResponse result, DataProvider datasource)
        {
            LookupCriteria criteria = TJson.Parse<LookupCriteria>(request["P0"]);
            result.Value = ToJson(new LookupOP(request.LogIn, datasource).GetLookup(criteria));
        }

       [Service("GetAllookupID")]
        public void GetAllookupID(DataRequest request, DataResponse result, DataProvider datasource)
        {     
            result.Value = ToJson(new LookupOP(request.LogIn, datasource).GetAllookupID());
        }  
    }
}
