using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Asp.netcore2.Filters
{
    public class AuthenticationFilter : Attribute  
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            //Your Code
        }
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            //Your Code
        }
    }

    public class AuthenticationContext
    {
    }

    public class AuthenticationChallengeContext
    {
    }



}
