using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SessionAuthMVC.Models;
using SessionAuthMVC.Common.Helper;

namespace SessionAuthMVC.Services
{
    public class UserManagerJohnService:IUserManagerJohnService
    {
        //private AuthContext db = new AuthContext();
        public IdentityResult Create(RegisterModel model, string password)
        {
            IdentityResult result = new IdentityResult();
            using (var db = new AuthContext())
            {
                var chkUser = (from x in db.users where x.UserID == model.UserID select x).FirstOrDefault();
                if (chkUser == null)
                {
                    //var keyNew = Helper.GeneratePassword(10);
                    //var passwordEncoded = Helper.EncodePassword(password, keyNew);
                    //model.SecurityStamp = keyNew;

                    model.PasswordHash = Helper.Hash(password);
                    model.SecurityStamp = Guid.NewGuid().ToString();
                    model.DateCreated = DateTime.Now;
                    model.DateModified = DateTime.Now;
                    model.EmailConfirmed = false;
                    db.users.Add(model);
                    db.SaveChanges();
                    result.Succeeded = true;
                }
                result.ErrorMessage = "User Already Exist";  // it works when result.Succeeded is false (default)
            }
            return result;
        }

        public IdentityResult ConfirmEmail (int UserID,string code)
        {
            IdentityResult result = new IdentityResult();
            using (var db = new AuthContext())
            {
                var chkUser = (from x in db.users where x.ID == UserID && x.SecurityStamp == code select x).FirstOrDefault();
                if (chkUser == null)
                {
                    chkUser.DateModified = DateTime.Now;
                    chkUser.EmailConfirmed = true;
                    db.SaveChanges();
                    result.Succeeded = true;
                }
            }
            return result;
        }
    }
}