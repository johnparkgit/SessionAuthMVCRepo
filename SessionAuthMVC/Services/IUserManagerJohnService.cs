using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SessionAuthMVC.Models;
namespace SessionAuthMVC.Services
{
    public interface IUserManagerJohnService
    {
        IdentityResult Create(RegisterModel model, string password);
    }
}