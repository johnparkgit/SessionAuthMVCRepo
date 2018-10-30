using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using SessionAuthMVC.Models;
namespace SessionAuthMVC.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {

        }
        public UsersModel CurrentUser
        {
            get
            {
                var serializer = new JavaScriptSerializer();
                FormsIdentity identity = (FormsIdentity)HttpContext.User.Identity;
                var user = identity.Ticket.UserData;

                UsersModel userModel = serializer.Deserialize<UsersModel>(user);
                return userModel;
            }
        }
    }
}