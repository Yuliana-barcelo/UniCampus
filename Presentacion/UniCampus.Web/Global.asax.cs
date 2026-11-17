using System;
using System.Diagnostics;
using System.Web;

namespace UniCampus.Web
{
    public class Global : HttpApplication
    {
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception error = Server.GetLastError();
            if (error != null)
                Trace.TraceError("Error no controlado en la aplicación UniCampus: {0}", error);
        }
    }
}
