using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Asp.netcore2.Filters
{
    public class CustomLogActionFilter : ActionFilterAttribute
    {
       // private readonly ILogger _logger;

        public CustomLogActionFilter (ILoggerFactory loggerFactory)
        {
           // _logger = loggerFactory.CreateLogger("CustomLogActionFilter");
        }

        public CustomLogActionFilter()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
           // _logger.LogWarning("ClassFilter OnActionExecuting");
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
           // _logger.LogWarning("OnActionExecuted");
            base.OnActionExecuted(context);
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
          //  _logger.LogWarning("OnResultExecuting");
            base.OnResultExecuting(context);
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
          //  _logger.LogWarning("OnResultExecuted");
            base.OnResultExecuted(context);
        }
    }
}
