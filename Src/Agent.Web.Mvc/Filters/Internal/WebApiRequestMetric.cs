using System;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;

namespace Gibraltar.Agent.Web.Mvc.Filters.Internal
{
    /// <summary>
    /// The tracking metric for a Web API request
    /// </summary>
    internal class WebApiRequestMetric : RequestMetric
    {
        public WebApiRequestMetric(HttpActionContext actionContext)
        {
            var parameters = actionContext.ActionDescriptor.GetParameters().Select(p => p.ParameterName);
            Parameters = String.Join(", ", parameters);

            HttpMethod = HttpContext.Current.Request.HttpMethod;
            var controllerDescriptor = actionContext.ActionDescriptor.ControllerDescriptor;
            ControllerName = controllerDescriptor.ControllerName;
            ActionName = actionContext.ActionDescriptor.ActionName;

            SubCategory = "Web API";

            //resolve the exact controller target.
            ClassName = controllerDescriptor.ControllerType.FullName;
            MethodName = actionContext.ActionDescriptor.ActionName; //BUGBUG: This is wrong; we want the real method
            FileName = null;
            LineNumber = 0;
        }
    }
}
