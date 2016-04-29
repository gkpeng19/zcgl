using System.Web;
using NM.OP;
using NM.Util;

namespace NM.Service
{
    public class ServiceContext
    {
        private ServiceContext()
        {

        }

        public ServiceContext(DataRequest request, DataResponse response)
        {
            E9_Request = request;
            E9_Response = response;
        }

        public ServiceContext(DataRequest request, DataResponse response, DataProvider datasource)
            : this(request, response)
        {
            DataSource = datasource;
        }

        public ServiceContext(DataRequest request, DataResponse response, DataProvider datasource, HttpContext context)
            : this(request, response, datasource)
        {
            HttpContext = context;
        }

        public DataRequest E9_Request { get; set; }
        public DataResponse E9_Response { get; set; }
        public HttpContext HttpContext { get; set; }
        public DataProvider DataSource { get; set; }
        public bool IsCustomerResponse { get; set; }
    }
}
