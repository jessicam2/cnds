using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Lpp.Dns.DTO.Security;

namespace Lpp.Dns.Portal.Controllers
{
    public class UsersController : BaseController
    {
        Lpp.Utilities.Security.ApiIdentity ApiIdentity
        {
            get
            {

                var apiIdentity = HttpContext.User.Identity as Lpp.Utilities.Security.ApiIdentity;
                if (apiIdentity == null)
                    throw new System.Security.SecurityException("User is not logged in.");

                return apiIdentity;
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Details()
        {
            Guid userID;
            if (Guid.TryParse((Request.QueryString["ID"] ?? ApiIdentity.ID.ToString()), out userID))
            {
                List<Guid> grantedPermissions = new List<Guid>();
                if (ApiIdentity.ID == userID)
                {
                    //user can always change own password
                    grantedPermissions.Add(PermissionIdentifiers.User.ChangePassword.ID);
                }
                else
                {
                    Lpp.Dns.Data.ExtendedQuery query = new Lpp.Dns.Data.ExtendedQuery() { Users = a => a.UserID == userID };

                    var permissionsList = new List<PermissionDefinition>();
                    permissionsList.Add(DTO.Security.PermissionIdentifiers.User.ChangePassword);

                    grantedPermissions = (await DataContext.HasGrantedPermissions(ApiIdentity, query, permissionsList.ToArray())).Select(p => p.ID).ToList();
                }

                ViewBag.ScreenPermissions = grantedPermissions;
            }

            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        public ActionResult RejectRegistration()
        {
            return View();
        }

        public ActionResult Deactivate()
        {
            return View();
        }

        public ActionResult SecurityGroups()
        {
            return View();
        }
    }
}