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
        public IdentityResult Create(UsersModel model, string password)
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
                    //model.SecurityStamp = Guid.NewGuid().ToString();
                    model.DateCreated = DateTime.Now;
                    model.DateModified = DateTime.Now;
                    model.EmailConfirmed = false;
                    model.UserStatus = 1;
                    db.users.Add(model);
                    db.SaveChanges();
                    result.Succeeded = true;
                }
                result.ErrorMessage = "User Already Exist";  // it works when result.Succeeded is false (default)
            }
            return result;
        }

        public IdentityResult ConfirmEmail (int ID,string code)
        {
            IdentityResult result = new IdentityResult();
            using (var db = new AuthContext())
            {
                var chkUser = (from x in db.users where x.ID == ID && x.SecurityStamp == code select x).FirstOrDefault();
                if (chkUser != null)
                {
                    chkUser.DateModified = DateTime.Now;
                    chkUser.EmailConfirmed = true;
                    db.SaveChanges();
                    result.Succeeded = true;
                }
            }
            return result;
        }
        public string GenerateEmailConfirmationTokenAsync(int ID)
        {
            using (var db = new AuthContext())
            {
                var code = Guid.NewGuid().ToString();
                var user = (from x in db.users where x.ID == ID select x).FirstOrDefault();
                if (user != null)
                {
                    user.SecurityStamp = code;
                    db.SaveChanges();
                }
                return code;
            }
        }
        public UsersModel FindByID(int ID)
        {
            using (var db = new AuthContext())
            {
                return (from x in db.users where x.ID == ID select x).FirstOrDefault();
            }
        }
        public UsersModel FindByUserID(string UserID)
        {
            using (var db = new AuthContext())
            {
                return (from x in db.users where x.UserID == UserID select x).FirstOrDefault();
            }
        }
        public Boolean IsEmailConfirmed(int ID)
        {
            using (var db = new AuthContext())
            {
                var user =  (from x in db.users where x.ID == ID select x).FirstOrDefault();
                if (user != null)
                    return user.EmailConfirmed;
            }
            return false;
            
        }
        public SignInStatus PasswordSignIn(string UserID, string Password)
        {
            using (var db = new AuthContext())
            {
                var PasswordHash = Helper.Hash(Password);
                var chkUser = (from x in db.users where x.UserID == UserID select x).FirstOrDefault();
                if (chkUser != null)
                {
                    if (PasswordHash == chkUser.PasswordHash)
                        return SignInStatus.Success;
                    else if (chkUser.Try >= 2)
                        return SignInStatus.Success;
                    else if (!chkUser.EmailConfirmed)
                        return SignInStatus.RequiresVerification;

                }
            }
            return SignInStatus.Failure; ;
        }
    }
}