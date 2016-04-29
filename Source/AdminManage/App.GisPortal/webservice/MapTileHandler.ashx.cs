using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GIS.Portal.webservice
{
    /// <summary>
    /// Summary description for MapTileHandler
    /// </summary>
    public class MapTileHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}