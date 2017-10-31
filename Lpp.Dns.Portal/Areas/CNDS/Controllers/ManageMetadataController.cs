using Lpp.Dns.ApiClient;
using Lpp.Utilities.WebSites.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Lpp.Dns.Portal.Areas.CNDS.Controllers
{
    public class ManageMetadataController : Controller
    {
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            var cookie = JsonConvert.DeserializeObject<LoginResponseModel>(Request.Cookies["Authorization"].Value);
            using (var client = new DnsClient(WebConfigurationManager.AppSettings["ServiceUrl"], cookie.UserName, cookie.Password))
            {
                var response = await client.CNDSSecurity.HasPermissions(new Guid("4EB90001-6F08-46E3-911D-A6BF012EBFB8"), cookie.ID.Value);
                if (!response)
                {
                    return View("~/Areas/CNDS/Views/ManagePermissions/AccessDenied.cshtml");
                }
            }

            return View();
        }

        public ActionResult NewDomainDefinitionDialog()
        {
            return View("~/areas/cnds/views/managemetadata/newdomaindefinition-dialog.cshtml");
        }
    }
}