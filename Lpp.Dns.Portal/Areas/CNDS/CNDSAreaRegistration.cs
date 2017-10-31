using System.Web.Mvc;

namespace Lpp.Dns.Portal.Areas.CNDS
{
    public class CNDSAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CNDS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CNDS_default",
                "CNDS/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}