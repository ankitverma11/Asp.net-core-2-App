using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Asp.netcore2.Filters;
using Microsoft.Extensions.Logging;

namespace Asp.netcore2.Controllers
{
    //The [Authorize] attribute signifies that this controller will be accessed only after successful authentication.
    //When an authentication request is made then, the middleware will check for the existence of an authentication cookie which was set during login. If the cookie is found then the login is successful and the user will be redirected to UserHome view. But if the cookie is not present then the user will be redirected to the Login page (which we will set up using options.LoginPath in Startup.cs in the next section). The user cannot access the UserHome view in this case.
     //We have also added a Logout method which will clear the authentication cookie and sign out the user from the application by redirecting them to our login page.
    
     [Authorize]
     [ServiceFilter(typeof(CustomLogActionFilter))]
    public class UserController : Controller
    {
        private readonly ILogger _logger;

        public UserController (ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("UserController");
        }

        public IActionResult UserHome()
        {
            //_logger.LogInformation("Executing user");
            return View("StudentDetails");
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("UserLogin", "Login");
        }
    }
}