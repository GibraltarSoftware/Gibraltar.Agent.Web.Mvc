using System;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Gibraltar.Agent.Web.Mvc.Configuration;
using Gibraltar.Agent.Web.Mvc.Filters.Internal;
using Gibraltar.Agent.Web.Mvc.Internal;

namespace Gibraltar.Agent.Web.Mvc.Filters
{
    public class WebApiRequestMonitorAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        private const string LogSystem = "Loupe";
        private readonly MvcAgentElement _configuration;

        /// <summary>
        /// Monitor MVC requests using the site-wide configuration settings
        /// </summary>
        public WebApiRequestMonitorAttribute()
            : this(MvcAgentElement.SafeLoad())
        {
        }

        /// <summary>
        /// Monitor MVC requests using the provided configuration settings
        /// </summary>
        /// <param name="configuration"></param>
        public WebApiRequestMonitorAttribute(MvcAgentElement configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            _configuration = configuration;
        }

        /// <summary>
        /// The category for log messages
        /// </summary>
        public string Category { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            //create our request tracking object
            var tracker = new WebApiRequestMetric(actionContext);

            // Store this on the request
            actionContext.Request.Store(tracker);

            //And log the request
            if (_configuration.LogRequests == false)
                return;

            var caption = string.Format("Web Api {0} {1} Requested", tracker.ControllerName, tracker.ActionName);

            var descriptionBuilder = new StringBuilder(1024);
            descriptionBuilder.AppendFormat("Controller: {0}\r\n", actionContext.ActionDescriptor.ControllerDescriptor.ControllerType);
            descriptionBuilder.AppendFormat("Action: {0}\r\n", actionContext.ActionDescriptor.ActionName);
            if (_configuration.LogRequestParameters)
            {
                descriptionBuilder.AppendLine("Parameters:");
                foreach (var param in actionContext.ActionDescriptor.GetParameters())
                {
                    object value = actionContext.ActionArguments[param.ParameterName];
                    descriptionBuilder.AppendFormat("- {0} = {1}\r\n", param.ParameterName,
                        Extensions.ObjectToString(value, _configuration.LogRequestParameterDetails));
                }
            }

            descriptionBuilder.AppendFormat("URL: {0}\r\n", HttpContext.Current.Request.Url);
            descriptionBuilder.AppendFormat("Request Id: {0}\r\n", tracker.UniqueId);

            Log.Write(_configuration.RequestMessageSeverity, LogSystem, tracker, tracker.UserName, null,
                LogWriteMode.Queued, null, Category, caption, descriptionBuilder.ToString());
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionContext)
        {
            //find our request tracking object.  Be wary because it might not be there any more if someone is messing with us
            var tracker = actionContext.Request.Retrieve<WebApiRequestMetric>();
            if (tracker != null)
            {
                tracker.Exception = actionContext.Exception;
                tracker.Record();
            }

            //we log exceptions in our unhandled exception attribute
        }
    }
}
