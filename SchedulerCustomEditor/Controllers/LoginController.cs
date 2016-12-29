namespace SchedulerCustomEditor.Controllers
{
    using Facebook;
    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;
    using SchedulerCustomEditor.Models;
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    public class LoginController : Controller
    {
        private SchedulerLoginService loginService;
        public string Facebook_AppID = System.Configuration.ConfigurationManager.AppSettings["client_id"].ToString();
        public string Facebook_AppSecret = System.Configuration.ConfigurationManager.AppSettings["client_secret"].ToString();

        public LoginController()
        {
            this.loginService = new SchedulerLoginService();
        }

        
        public ActionResult Index()
        {

            ViewBag.Message = "Welcome to My test app";
            return View(new UserViewModel());
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index");
        }

        [HttpPost] 
        public JsonResult ValidateUser(UserViewModel user)
        {
            if (ModelState.IsValid)
            {
                var data = loginService.GetUser_ByEmailId_Password(user.EmailId, Models.EncryptDecrypt.EncryptString(user.Password), null);
                if (data != null)
                {
                    FormsAuthentication.SetAuthCookie(user.EmailId, true);
                    Session["UserID"] = data.UserID;
                    Session["UserName"] = data.UserName;
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, Valid = false }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Success = false, Valid = false }, JsonRequestBehavior.AllowGet);
            }
        }

        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        public ActionResult Facebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = Facebook_AppID,
                client_secret = Facebook_AppSecret,
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email" // Add other permissions as needed
            });

            return Redirect(loginUrl.AbsoluteUri);
        }

        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = Facebook_AppID,
                client_secret = Facebook_AppSecret,
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });

            var accessToken = result.access_token;

            // Store the access token in the session
            Session["AccessToken"] = accessToken;

            // update the facebook client with the access token so 
            // we can make requests on behalf of the user
            fb.AccessToken = accessToken;

            // Get the user's information
            dynamic me = fb.Get("me?fields=first_name,last_name,id,email");
            string fid = me.id;

            var data = loginService.GetUser_ByEmailId_Password(null, null, fid);
            if (data != null)
            {
                Session["UserID"] = data.UserID;
                Session["UserName"] = data.UserName;
            }
            else
            { 
                //create new user with the fb email 
                SampleEntities db = new SampleEntities();
                SchedulerCustomEditor.Models.User user = new User
                {
                    EmailId = me.email,
                    FacebookID = fid,
                    FirstName = me.first_name,
                    LastName = me.last_name,
                    Password = EncryptDecrypt.EncryptString("Temp@pwd123") // create a temp pwd for fb login
                };
                db.Users.Add(user);
                db.SaveChanges();

                Session["UserID"] = user.UserID;
                Session["UserName"] = user.FirstName + " " + user.LastName;
            }
            // Set the auth cookie
            FormsAuthentication.SetAuthCookie(fid, false);

            return RedirectToAction("Index", "Home");
        }

    }
}
