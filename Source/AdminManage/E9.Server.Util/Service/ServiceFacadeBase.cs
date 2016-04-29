using NM.Util;

namespace NM.Service
{
    public abstract class ServiceFacadeBase //: ServiceProvider
    {
        protected string ToJson(IJson obj)
        {
            return obj.ToJson();
        }
    }
}
