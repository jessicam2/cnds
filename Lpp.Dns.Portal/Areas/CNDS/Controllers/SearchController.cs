using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lpp.Utilities.WebSites.Models;
using Newtonsoft.Json;
using System.Data.Entity;

namespace Lpp.Dns.Portal.Areas.CNDS.Controllers
{
    public class SearchController : Controller
    {
        static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(SearchController));
        static readonly string QlikHost = ConfigurationManager.AppSettings["CNDS.Qlik.Host"] ?? string.Empty;
        static readonly string QlikPrefix = ConfigurationManager.AppSettings["CNDS.Qlik.Prefix"] ?? "/";
        static readonly string QlikPort = ConfigurationManager.AppSettings["CNDS.Qlik.Port"] ?? "443";
        static readonly string QlikIsSecure = ConfigurationManager.AppSettings["CNDS.Qlik.IsSecure"] ?? "True";
        static readonly string QlikDataSourceSearchAppID = ConfigurationManager.AppSettings["CNDS.Qlik.DataSourceSearchAppID"] ?? string.Empty;
        static readonly string QlikDataSourceSearchObjectID = ConfigurationManager.AppSettings["CNDS.Qlik.DataSourceSearchObjectID"] ?? string.Empty;
        static readonly string QlikQPSPort = ConfigurationManager.AppSettings["CNDS.Qlik.QPS.Port"] ?? "4243";

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

        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> NewRequest()
        {
            return View();
        }
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> DataSources()
        {
            return View();
        }
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> Organizations()
        {
            return View();
        }

        //[HttpGet,ActionName("datasources-qlik")]
        //public async System.Threading.Tasks.Task<ActionResult> DataSourcesQlik()
        //{

        //    string networkName = string.Empty;
        //    using (var db = new Lpp.Dns.Data.DataContext())
        //    {
        //        networkName = await db.Networks.Where(n => n.Name != "Aqueduct").Select(n => n.Name).FirstOrDefaultAsync();

        //        var permission = await db.HasGrantedPermissions(ApiIdentity, DTO.Security.PermissionIdentifiers.Portal.SearchCNDS);
        //        if (permission == null || !permission.Any())
        //        {
        //            return View("~/Areas/CNDS/Views/ManagePermissions/AccessDenied.cshtml");
        //        }
        //    }

        //    //If a ticket does not exist in the user's session object, create a new one from Qlik
        //    //TODO: modify the log action to also remove the user from Qlik server.

        //    //Note: The Ticket by the Qlik Server is only valid for 1 minute.
        //    //So if the session was not previously authenticated and no session exists, we must get a new ticket as using the old ticket will fail.
        //    //Note below that the code that sets the session variable has been commented out. 
        //    //Dean - please modify this code as appropriate after confirming/reviewing.
        //    string ticket = Utilities.ObjectEx.ToStringEx(Session["X-Qlik-Session-WebTicket"]);

        //    if (string.IsNullOrEmpty(ticket))
        //    {

        //        var urlBuilder = new UriBuilder((Convert.ToBoolean(QlikIsSecure) ? "https" : "http"), QlikHost, Convert.ToInt32(QlikQPSPort));
        //        urlBuilder.Path = "qps/" + QlikPrefix;

        //        var req = new QlikAuthNet.Ticket
        //        {
        //            UserDirectory = networkName.Replace(" ", "-"),
        //            //UserId = ApiIdentity.UserName,
        //            UserId = ((Guid)Session["CNDS.UserID"]).ToString("D"),
        //            ProxyRestUri = urlBuilder.ToString(),
        //            TargetId = string.Empty,
        //            CertificateLocation = System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine                    
        //        };
        //        req.AddAttributes("UserName", new List<string>() { ApiIdentity.UserName });

        //        Logger.Debug(string.Format("Requesting ticket:{0}. User: {1}/{2}/{3}", urlBuilder.ToString(), req.UserDirectory, req.UserId, ApiIdentity.UserName));


        //        try
        //        {
        //            ticket = req.TicketRequest();
        //            Logger.Debug(string.Format("Successfully requested ticket from \"{2}\" for \"{0}/{1}\": {3}", req.UserDirectory, req.UserId, urlBuilder.ToString(), ticket));
        //            //Session.Add("X-Qlik-Session-WebTicket", ticket);
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.Error(string.Format("Error requesting ticket for \"{0}/{1}\" from \"{2}\"", req.UserDirectory, req.UserId, urlBuilder.ToString()), ex);
        //            throw;
        //        }

        //        ViewBag.QlikTicket = ticket;
        //    }
        //    else
        //    {
        //        ViewBag.QlikTicket = ticket;
        //        Logger.Debug(string.Format("Ticket already in session for \"{0}/{1}\": {2}", networkName.Replace(" ", "-"), ApiIdentity.UserName, ticket));
        //    }

        //    string qlikBaseUrl = Utilities.ObjectEx.ToStringEx(Session["X-Qlik-BaseUrl"]);
        //    if (string.IsNullOrEmpty(qlikBaseUrl))
        //    {
        //        var urlBuilder = new UriBuilder((Convert.ToBoolean(QlikIsSecure) ? "https" : "http"), QlikHost, Convert.ToInt32(QlikPort));
        //        qlikBaseUrl = urlBuilder.ToString() + (string.IsNullOrEmpty(QlikPrefix) ? "" : QlikPrefix + "/");
        //        Session.Add("X-Qlik-BaseUrl", qlikBaseUrl);
        //    }

        //    ViewBag.QlikBaseUrl = qlikBaseUrl;

        //    #region "To be deleted"
        //    //string qlikCookieName = "X-Qlik-Session-WebTicket";
        //    //string qlikSessionID = Utilities.ObjectEx.ToStringEx(Session["X-Qlik-SessionID"]);
        //    //if (string.IsNullOrEmpty(qlikSessionID))
        //    //{
        //    //    string urlAddress = string.Format("{0}resources/assets/external/requirejs/require.js{1}", qlikBaseUrl, (string.IsNullOrEmpty(ticket) ? string.Empty : "?" + ticket));

        //    //    System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(urlAddress);
        //    //    System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();

        //    //    if (response.Headers.AllKeys.Contains("Set-Cookie"))
        //    //    {
        //    //        string[] cookies = response.Headers.GetValues("Set-Cookie");

        //    //        foreach (var singleCookie in cookies)
        //    //        {
        //    //            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(singleCookie, "(.+?)=(.+?);");
        //    //            if (match.Captures.Count == 0)
        //    //                continue;

        //    //            if (match.Groups[1].ToString() == qlikCookieName)
        //    //            {
        //    //                qlikSessionID = match.Groups[2].ToString();

        //    //                response.Cookies.Add(
        //    //                new System.Net.Cookie(
        //    //                    match.Groups[1].ToString(),
        //    //                    match.Groups[2].ToString(),
        //    //                    "/",
        //    //                    request.Host.Split(':')[0]));
        //    //            }
        //    //        }
        //    //    }

        //    //    Session.Add("X-Qlik-SessionID", qlikSessionID);
        //    //}

        //    //ViewBag.QlikSessionID = qlikSessionID;

        //    //string requireJSURL = string.Format("{0}resources/assets/external/requirejs/require.js{1}", qlikBaseUrl, (string.IsNullOrEmpty(ticket) ? string.Empty : "?" + ticket));

        //    //System.Net.HttpWebRequest requestResource_RequireJS = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(requireJSURL);
        //    //requestResource_RequireJS.Headers.Add(qlikCookieName, qlikSessionID);
        //    //requestResource_RequireJS.CookieContainer = new System.Net.CookieContainer(1);
        //    //requestResource_RequireJS.CookieContainer.Add(new System.Net.Cookie(qlikCookieName, qlikSessionID) { Domain = (new System.Uri(qlikBaseUrl)).Host });
        //    //requestResource_RequireJS.Host = (new System.Uri(qlikBaseUrl)).Host;

        //    //System.Net.HttpWebResponse responseResource_RequireJS = (System.Net.HttpWebResponse)requestResource_RequireJS.GetResponse();

        //    //var jsStream = responseResource_RequireJS.GetResponseStream();
        //    //System.IO.StreamReader reader = null;

        //    //if (responseResource_RequireJS.CharacterSet == null)
        //    //{
        //    //    reader = new System.IO.StreamReader(jsStream);
        //    //}
        //    //else
        //    //{
        //    //    reader = new System.IO.StreamReader(jsStream, System.Text.Encoding.GetEncoding(responseResource_RequireJS.CharacterSet));
        //    //}

        //    //ViewBag.RequireJsScript = reader.ReadToEnd();

        //    //Response.AddHeader("Cookie", qlikCookieName + "=" + qlikSessionID);
        //    //Response.AppendCookie(new HttpCookie(qlikCookieName, qlikSessionID) { Domain = (new System.Uri(qlikBaseUrl)).Host });
        //    #endregion

        //    return View("~/areas/cnds/views/search/datasources-qlik.cshtml");
        //}

        [HttpGet]
        public ActionResult SelectRequestType()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateRequest()
        {
            return View();
        }

        [HttpGet]
        public JsonResult QlikConfiguration()
        {
            var config = new {
                    host = QlikHost,
                    prefix = string.Format("/{0}/", QlikPrefix),
                    port = Convert.ToInt32(QlikPort),
                    isSecure = Convert.ToBoolean(QlikIsSecure),
                    dataSourceSearchAppID = QlikDataSourceSearchAppID,
                    dataSourceSearchObjectID = QlikDataSourceSearchObjectID
                };

            return Json(config, JsonRequestBehavior.AllowGet);
        }
    }
}