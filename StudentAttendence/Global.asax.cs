using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;
using StudentAttendence.Models;

namespace StudentAttendence
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Register the safe schema initializer before any DB access
            Database.SetInitializer(new AttendanceDbInitializer());

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
