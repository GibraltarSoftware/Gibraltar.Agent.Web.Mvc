using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gibraltar.Agent.Web.Mvc.Filters.Internal
{
    /// <summary>
    /// The tracking information for an MVC engine request
    /// </summary>
    internal class MvcRequestMetric : RequestMetric
    {
        public MvcRequestMetric(ActionExecutingContext actionContext)
        {
            var parameters = actionContext.ActionDescriptor.GetParameters().Select(p => p.ParameterName);
            Parameters = String.Join(", ", parameters);

            HttpMethod = HttpContext.Current.Request.HttpMethod;
            var controllerDescriptor = actionContext.ActionDescriptor.ControllerDescriptor;
            ControllerName = controllerDescriptor.ControllerName;
            ActionName = actionContext.ActionDescriptor.ActionName;
            UniqueId = actionContext.ActionDescriptor.UniqueId;

            SubCategory = "MVC";

            //resolve the exact controller target.
            ClassName = controllerDescriptor.ControllerType.FullName;
            MethodName = actionContext.ActionDescriptor.ActionName; //BUGBUG: This is wrong; we want the real method
            FileName = null;
            LineNumber = 0;
        }
    }
}
