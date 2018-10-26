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
                    var keyNew = Helper.GeneratePassword(10);
                    var passwordEncoded = Helper.EncodePassword(password, keyNew);
                    model.PasswordHash = passwordEncoded;
                    model.SecurityStamp = keyNew;
                    model.DateCreated = DateTime.Now;
                    model.DateModified = DateTime.Now;
                    db.users.Add(model);
                    db.SaveChanges();
                    result.Succeeded = true;
                }
                result.ErrorMessage = "User Already Exist";
            }
            return result;
        }
    }
}