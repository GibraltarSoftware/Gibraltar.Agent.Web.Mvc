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
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Gibraltar.Agent.Web.Mvc.Configuration;
using Gibraltar.Agent.Web.Mvc.Filters.Internal;
using Gibraltar.Agent.Web.Mvc.Internal;
using Extensions = Gibraltar.Agent.Web.Mvc.Internal.Extensions;

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
        private readonly MvcAgentElement _configuration;
        private readonly bool _enableDebugMode;
        private readonly UrlCheck _urlCheck;

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
                throw new ArgumentNullException(nameof(configuration));

            _configuration = configuration;

            _urlCheck = new UrlCheck();

            Order = 0; //so we run first and last in general
        }

        /// <summary>
        /// The category for log messages
        /// </summary>
        public string Category { get; set; }

        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //create our request tracking object
            var tracker = new MvcRequestMetric(filterContext);

            //If this is a Loupe client request then we don't want to log that (unless we're in debug mode)
            tracker.Suppress = (_enableDebugMode == false) && (_urlCheck.IsLoupeUrl(filterContext));

            // Store this on the request
            HttpContext.Current.Store(tracker);

            //And log the request
            if (_configuration.LogRequests && 
                (tracker.Suppress == false))
            {
                var caption = string.Format("Web Site {0} {1} Requested", tracker.ControllerName, tracker.ActionName);

                var requestLogging = new MonitorRequestLogging(filterContext, _configuration);
                requestLogging.Log(Category,caption,tracker.UserName, tracker);

            }

            base.OnActionExecuting(filterContext);
        }

        /// <inheritdoc />
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //find our request tracking object.  Be wary because it might not be there any more if someone is messing with us
            var tracker = HttpContext.Current.Retrieve<MvcRequestMetric>(filterContext.ActionDescriptor.UniqueId);
            if (tracker != null)
            {
                tracker.Exception = filterContext.Exception;
                if ((Thread.CurrentPrincipal != null) && (Thread.CurrentPrincipal.Identity != null))
                {
                    tracker.UserName = Thread.CurrentPrincipal.Identity.Name;
                }

                tracker.Record();
            }

            //we log exceptions in our unhandled exception attribute

            base.OnActionExecuted(filterContext);
        }
    }
}
