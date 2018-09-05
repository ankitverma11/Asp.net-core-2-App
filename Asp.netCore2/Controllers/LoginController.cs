using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Asp.netcore2.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Asp.netcore2.Filters;

namespace Asp.netcore2.Controllers
{
    //SL (Secure Sockets Layer) is a standard security protocol for establishing encrypted links between a web server and a browser during online communication.
    //The use of SSL technology ensures that all data transmitted between the web server and browser remains encrypted.
    
    // [RequireHttps]  -- on Controler level

    public class LoginController : Controller
    {
        UserDataAccessLayer _dbcontext = new UserDataAccessLayer();

        [HttpGet]
        public IActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterUser([Bind] UserDetails user)
        {
            if (ModelState.IsValid)
            {
                string Registeruser = _dbcontext.RegisterUser(user);
                if (Registeruser == "Success")
                {
                    ModelState.Clear();
                    TempData["Success"] = "Registration Successful!";
                    return View();
                }
                else
                {
                    TempData["Fail"] = "This User ID already exists. Registration Failed.";
                    return View();
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult UserLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomActionFilter]
        public async Task<IActionResult> UserLogin([Bind] UserDetails user)
        {
            ModelState.Remove("FirstName");
            ModelState.Remove("LastName");
            if (ModelState.IsValid)
            {
                string LoginStatus = _dbcontext.validatelogin(user);
                if (LoginStatus == "Success")
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserID)
                };
                    //If the validation is successful from the database end, then we will create a Claims list which will store the UserID of the user 
                    //into the Name claim inside the cookie. This cookie data will be encrypted by default. Then we will build an identity and 
                    //principal and then set the cookie using the SignInAsync method.
                    ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);

                    await HttpContext.SignInAsync(principal);
                    return RedirectToAction("UserHome", "User");
                }
                else
                {
                    TempData["UserLoginFailed"] = "Login Failed.Please enter correct credentials";
                    return View();
                }
            }
            else
                return View();
        }

    }
}