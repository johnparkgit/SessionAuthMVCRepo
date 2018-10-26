using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SessionAuthMVC.Models;
using SessionAuthMVC.Services;

namespace SessionAuthMVC.Controllers
{
    public class AccountJohnController : Controller
    {
        // GET: AccountJohn
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
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
    }
}