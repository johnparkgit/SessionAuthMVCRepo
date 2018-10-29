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
        //[UserAuthenticationFilter]
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
                var user = new RegisterModel { UserID = model.UserID, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
                UserManagerJohnService UserManager = new UserManagerJohnService();
                var result = UserManager.Create(user, model.PasswordHash);
                //    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, NewRow = model.NewRow, Name = model.Name };
                //    var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var activationCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(model.Email, activationCode);

                    //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    //        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    //        // Send an email with this link
                    //        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("",result.ErrorMessage);
                //    AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public void SendVerificationLinkEmail(string emailID, string activationCode)
        {
            var verifyUrl = "/User/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("junsoft2000@gmail.com", "John");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "Myemailpassword0914"; // Replace with actual password
            string subject = "Your account is successfully created!";
            var link1 = "here";
            string body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                " successfully created. Please click on the below link to verify your account" +
                " <br/><br/><a href='" + link + "'>" + link1 + "</a> ";

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
        public ActionResult VerifyAccount(int UserID,string code)
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