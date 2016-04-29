using System.Web.Services;
using NM.Service;
using NM.Util;
using NM.OP;
using System.Web;

namespace NM.Handler
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class OPHandler : DataHandler
    {
        protected override void DoProcess(ServiceContext context)
        {
            ServiceManager.Default.CallService(context);
        }

        protected override string ServiceCategoryName
        {
            get { return ServiceFacadeAttribute.DefalutCategory; }
        }
    }
}
