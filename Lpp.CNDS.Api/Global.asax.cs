using Lpp.CNDS.Data;
using Lpp.Utilities.WebSites;
using Lpp.Utilities.WebSites.Attributes;
using Lpp.Utilities.WebSites.Filters;
using Lpp.Utilities.WebSites.Handlers;
using Lpp.Utilities.WebSites.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.OData.Extensions;
using System.Web.Routing;

namespace Lpp.CNDS.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //force load all assemblies to ensure availability
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            loadedAssemblies
                .SelectMany(x => x.GetReferencedAssemblies())
                .Distinct()
                .Where(y => loadedAssemblies.Any((a) => a.FullName == y.FullName) == false)
                .ToList()
                .ForEach(x => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(x)));


            //This initializes the data context and force loads all of the DLLs that are being lazy loaded.
            using (var db = new DataContext())
            {
                db.Users.Where(u => u.ID == Guid.Empty).FirstOrDefault();
            }

            RouteTable.Routes.Clear();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            WebApiConfig.Register(GlobalConfiguration.Configuration);

            GlobalConfiguration.Configuration.AddODataQueryFilter();
            GlobalConfiguration.Configuration.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            //Add basic auth. All functions are secure by default unless otherwise attributed with allowanonymous
            //GlobalConfiguration.Configuration.Filters.Add(new BasicAuthentication());


            //Validation so that intelligent errors are returned.            
            //GlobalConfiguration.Configuration.Filters.Add(new ValidationActionFilter());
            //GlobalConfiguration.Configuration.Filters.Add(new UnwrapExceptionFilterAttribute());

            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));

            //SSL Requirement
            GlobalConfiguration.Configuration.MessageHandlers.Add(new RequireHttpsMessageHandler());

            //Fix for jquery returning error on OK/Accept if no json content included even if just {}
            GlobalConfiguration.Configuration.MessageHandlers.Add(new HttpResponseMessageHandler());

            //Remove the default json formatter that is broken.
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.JsonFormatter);

            //Add our Json formatter that works properly including with Dates
            //Insert makes it the first choice for serialization, however the end user can request xml as well.
            GlobalConfiguration.Configuration.Formatters.Insert(0, new JsonNetFormatter());

            RouteTable.Routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            GlobalConfiguration.Configuration.EnsureInitialized();
        }
    }
}
