using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SessionAuthMVC.Models;
using SessionAuthMVC.Services;
using SessionAuthMVC.Filters;
using System.Net.Mail;
using System.Net;

namespace SessionAuthMVC.Controllers
{
    public class AccountJohnFormController : Controller
    {
        // GET: AccountJohn
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Require the user to have a confirmed email before they can log on.
            //var user = await UserManager.FindByNameAsync(model.Email);
            UserManagerJohnService UserManager = new UserManagerJohnService();
            var user = UserManager.FindByUserID(model.UserID);
            if (user != null)
            {
                if (user.UserStatus != 1)
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
                }
                /* added by john: first time existing user login */
                if (string.IsNullOrEmpty(user.PasswordHash))
                {
                    return View("ForgotPassword");
                    //ViewBag.Message = "Your password need to reset";
                    //return View("Error");
                }
                if (!UserManager.IsEmailConfirmed(user.ID))
                {
                    var code = UserManager.GenerateEmailConfirmationTokenAsync(user.ID);
                    var callbackUrl = Url.Action("ConfirmEmail", "AccountJohnForm", new { userId = user.ID, code = code }, protocol: Request.Url.Scheme);

                    SendVerificationLinkEmail(user.Email, callbackUrl);

                   ViewBag.Message = "Check your email and confirm your account, you must be confirmed "
                         + "before you can log in.";
                    return View("Info");
                }
            }
            //return View();

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            //returnUrl = ConfigurationManager.AppSettings["baseurl"];
            //returnUrl = "localhost";
            //            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: true);

            
           // var result = UserManager.PasswordSignIn(model.UserName, model.Password, false, shouldLockout: true);
            var result = UserManager.PasswordSignIn(model.UserID, model.Password);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                //RememberMe(model.RememberMe, model.UserName); ******

                /*
                if (user.Title.ToUpper() == "ADMIN")
                    //return Redirect(returnUrl);
                    return RedirectToAction("Index", "Home");
                else
                    return RedirectToCMIS(user.UserName);
                */

                //return Redirect("http://localhost");
                //private ActionResult RedirectToLocal(string returnUrl)
                //{
                //    if (Url.IsLocalUrl(returnUrl))
                //    {
                //        return Redirect(returnUrl);
                //    }
                //    return RedirectToAction("about", "Home");
                //}
                case SignInStatus.LockedOut:
                    return View("Lockout");
                //case SignInStatus.RequiresVerification:   // phone verifiation
                //    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
            
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }


        //[UserAuthenticationFilter]
        [Authorize]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        //[UserAuthenticationFilter]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        public ActionResult Register(RegisterViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                var user = new UsersModel { UserID = model.UserID, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
                UserManagerJohnService UserManager = new UserManagerJohnService();
                var result = UserManager.Create(user, model.PasswordHash);
                //    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, NewRow = model.NewRow, Name = model.Name };
                //    var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var code = UserManager.GenerateEmailConfirmationTokenAsync(user.ID);
                    var callbackUrl = Url.Action("ConfirmEmail", "AccountJohnForm", new { userId = user.ID, code = code }, protocol: Request.Url.Scheme);

                    SendVerificationLinkEmail(user.Email, callbackUrl);

                    //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    //        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    //        // Send an email with this link
                    //        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    ViewBag.Message = "Check your email and confirm your account, you must be confirmed "
                         + "before you can log in.";
                    return View("Info");
                }
                ModelState.AddModelError("",result.ErrorMessage);
                //    AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public void SendVerificationLinkEmail(string emailID, string callbackUrl)
        {
            //var verifyUrl = "/User/VerifyAccount/" + activationCode;
            //var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("junsoft2000@gmail.com", "John");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "Myemailpassword0914"; // Replace with actual password
            string subject = "Your account is successfully created!";
            var link1 = "here";
            string body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                " successfully created. Please click on the below link to verify your account" +
                " <br/><br/><a href='" + callbackUrl + "'>" + link1 + "</a> ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        [HttpGet]
        public ActionResult ConfirmEmail(int UserID,string code)
        {
            if (UserID == default(int) || code == null)
            {
                return View("Error");
            }
            UserManagerJohnService UserManager = new UserManagerJohnService();

            //var result = await UserManager.ConfirmEmailAsync(userId, code);
            var result = UserManager.ConfirmEmail(UserID, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");

            
        }
    }

    
}