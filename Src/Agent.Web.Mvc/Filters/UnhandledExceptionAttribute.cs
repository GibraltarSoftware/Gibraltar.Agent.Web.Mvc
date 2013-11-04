using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace Gibraltar.Agent.Web.Mvc.Filters
{
    /// <summary>
    /// Manages unhandles exceptions
    /// </summary>
    public class UnhandledExceptionAttribute : HandleErrorAttribute 
    {
        public override void OnException(ExceptionContext filterContext)
        {
            //here we want to interrogate the filter context for route information, 
            //view/controller/model information and create a better message than
            //our default record exception would.
            Log.RecordException(filterContext.Exception, "Web Site.Exceptions", true);
            base.OnException(filterContext);
        }
    }
}