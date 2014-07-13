#region File Header and License
// /*
//    MvcRequestMonitorAttribute.cs
//    Copyright 2013 Gibraltar Software, Inc.
//    
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// */
#endregion
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Gibraltar.Agent.Web.Mvc.Configuration;
using Gibraltar.Agent.Web.Mvc.Filters.Internal;
using Gibraltar.Agent.Web.Mvc.Internal;

namespace Gibraltar.Agent.Web.Mvc.Filters
{
    /// <summary>
    /// An ASP.NET MVC Action Filter that logs diagnostic and performance information for MVC requests
    /// </summary>
    /// <remarks>
    /// 	<para>This filter records log messages for every controller action that is invoked and a performance metric recording the duration of each action.  If an
    /// action throws an exception that is also recorded, however it's advisable to also register the Unhandled Exception filter to ensure all exceptions are
    /// recorded.</para>
    /// </remarks>
    /// <example>
    /// 	<code title="Enabling the Request Monitor" description="The fastest way to enable the request monitor for all of the controllers is to register it into the global filters collection like this example" lang="CS">
    /// using Gibraltar.Agent;
    /// using Gibraltar.Agent.Web.Mvc.Filters;
    ///  
    /// namespace YourSite
    /// {
    ///     public class MvcApplication : System.Web.HttpApplication
    ///     {
    ///         protected void Application_Start()
    ///         {
    ///             Log.StartSession(); //Prompt the Loupe Agent to start immediately
    ///  
    ///             //Register the MVC and Exception filters
    ///             GlobalFilters.Filters.Add(new MvcRequestMonitorAttribute());
    ///             GlobalFilters.Filters.Add(new UnhandledExceptionAttribute());
    ///         }
    ///     }
    /// }</code>
    /// </example>
    public class MvcRequestMonitorAttribute : ActionFilterAttribute
    {
        private const string LogSystem = "Loupe";
        private readonly MvcAgentElement _configuration;

        /// <summary>
        /// Monitor MVC requests using the site-wide configuration settings
        /// </summary>
        public MvcRequestMonitorAttribute()
            : this(MvcAgentElement.SafeLoad())
        {
        }

        /// <summary>
        /// Monitor MVC requests using the provided configuration settings
        /// </summary>
        /// <param name="configuration"></param>
        public MvcRequestMonitorAttribute(MvcAgentElement configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            _configuration = configuration;
            Order = 0; //so we run first and last in general
        }

        /// <summary>
        /// The category for log messages
        /// </summary>
        public string Category { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //create our request tracking object
            var tracker = new MvcRequestMetric(filterContext);

            // Store this on the request
            HttpContext.Current.Store(tracker);

            //And log the request
            if (_configuration.LogRequests == false)
                return;

            var caption = string.Format("Web Site {0} {1} Requested", tracker.ControllerName, tracker.ActionName);

            var descriptionBuilder = new StringBuilder(1024);
            descriptionBuilder.AppendFormat("Controller: {0}\r\n", filterContext.ActionDescriptor.ControllerDescriptor.ControllerType);
            descriptionBuilder.AppendFormat("Action: {0}\r\n", filterContext.ActionDescriptor.ActionName);
            if (_configuration.LogRequestParameters)
            {
                descriptionBuilder.AppendLine("Parameters:");
                foreach (var param in filterContext.ActionDescriptor.GetParameters())
                {
                    object value = filterContext.ActionParameters[param.ParameterName];
                    descriptionBuilder.AppendFormat("- {0}: {1}\r\n", param.ParameterName, 
                        Extensions.ObjectToString(value, _configuration.LogRequestParameterDetails));
                }
            }

            descriptionBuilder.AppendFormat("\r\nURL: {0}\r\n", HttpContext.Current.Request.Url);
   
            Log.Write(_configuration.RequestMessageSeverity, LogSystem, tracker, tracker.UserName, null,
                LogWriteMode.Queued, null, Category, caption, descriptionBuilder.ToString());
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //find our request tracking object.  Be wary because it might not be there any more if someone is messing with us
            var tracker = HttpContext.Current.Retrieve<MvcRequestMetric>(filterContext.ActionDescriptor.UniqueId);
            if (tracker != null)
            {
                tracker.Exception = filterContext.Exception;
                tracker.Record();
            }

            //we log exceptions in our unhandled exception attribute
        }
    }
}
