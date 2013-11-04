using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Gibraltar.Agent.Web.Mvc.Filters.Internal
{
    /// <summary>
    /// The tracking information for an MVC engine request
    /// </summary>
    internal class MvcRequestMetric : RequestMetric, IMessageSourceProvider
    {
        private readonly string _className;
        private readonly string _methodName;

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
            _className = controllerDescriptor.ControllerType.FullName;
            _methodName = actionContext.ActionDescriptor.ActionName; //BUGBUG: This is wrong; we want the real method
        }

        string IMessageSourceProvider.MethodName { get { return _methodName; } }

        string IMessageSourceProvider.ClassName { get { return _className; } }

        string IMessageSourceProvider.FileName { get { return null; } }

        int IMessageSourceProvider.LineNumber { get { return 0; } }
    }
}
