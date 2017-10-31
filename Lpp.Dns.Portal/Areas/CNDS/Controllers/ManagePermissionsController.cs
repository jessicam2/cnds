using Lpp.Dns.ApiClient;
using Lpp.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Lpp.Dns.DTO;
using Lpp.Utilities.WebSites.Models;
using System.Text;

namespace Lpp.Dns.Portal.Areas.CNDS.Controllers
{
    public class ManagePermissionsController : Controller
    {
        // GET: CNDS/ManagePermissions
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            var cookie = JsonConvert.DeserializeObject<LoginResponseModel>(Request.Cookies["Authorization"].Value);
            using (var client = new DnsClient(WebConfigurationManager.AppSettings["ServiceUrl"], cookie.UserName, cookie.Password))
            {
                var response = await client.CNDSSecurity.HasPermissions(new Guid("E3410001-B6F4-4C51-B269-A6BF012EC64D"), cookie.ID.Value);
                if (!response)
                {
                    //return PartialView("~/Views/Errors/AccessDeniedEmbedded.cshtml");
                    return AccessDenied();
                }
            }

            return View();
        }

        public ActionResult AddSecurityGroupDialog()
        {
            return View();
        }
        public ActionResult AddSecurityGroupPermissionDialog()
        {
            return View();
        }
        public ActionResult AccessDenied()
        {
            return View("~/Areas/CNDS/Views/ManagePermissions/AccessDenied.cshtml");
        }
    }
}