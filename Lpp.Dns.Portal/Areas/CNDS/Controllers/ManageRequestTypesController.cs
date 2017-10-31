using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Lpp.Dns.ApiClient;
using Lpp.Utilities.WebSites.Models;
using Newtonsoft.Json;

namespace Lpp.Dns.Portal.Areas.CNDS.Controllers
{
    public class ManageRequestTypesController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var cookie = JsonConvert.DeserializeObject<LoginResponseModel>(Request.Cookies["Authorization"].Value);
            using (var client = new DnsClient(WebConfigurationManager.AppSettings["ServiceUrl"], cookie.UserName, cookie.Password))
            {
                var response = await client.CNDSSecurity.HasPermissions(new Guid("9AFF0001-1E18-4AEA-8C2E-A6DB01656A4B"), cookie.ID.Value);
                if (!response)
                {
                    return View("~/areas/cnds/views/managepermissions/accessdenied.cshtml");
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult EditMapping()
        {
            return View("~/areas/cnds/views/managerequesttypes/editmapping-dialog.cshtml");
        }

        [HttpGet]
        public ActionResult EditDefinition()
        {
            return View("~/areas/cnds/views/managerequesttypes/editdefinition-dialog.cshtml");
        }
    }
}