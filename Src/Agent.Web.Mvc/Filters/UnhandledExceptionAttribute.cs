using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Gibraltar.Agent.Web.Mvc.Configuration;
using Gibraltar.Agent.Web.Mvc.Filters.Internal;
using Gibraltar.Agent.Web.Mvc.Internal;

namespace Gibraltar.Agent.Web.Mvc.Filters
{
    /// <summary>
    /// Manages unhandles exceptions
    /// </summary>
    public class UnhandledExceptionAttribute : HandleErrorAttribute 
    {
        private readonly MvcAgentElement _configuration;

        /// <summary>
        /// Monitor MVC requests using the site-wide configuration settings
        /// </summary>
        public UnhandledExceptionAttribute()
            : this(MvcAgentElement.SafeLoad())
        {
        }

        /// <summary>
        /// Monitor MVC requests using the provided configuration settings
        /// </summary>
        /// <param name="configuration"></param>
        public UnhandledExceptionAttribute(MvcAgentElement configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            _configuration = configuration;
        }

        public override void OnException(ExceptionContext filterContext)
        {
            //here we want to interrogate the filter context for route information, 
            //view/controller/model information and create a better message than
            //our default record exception would.
            Log.RecordException(filterContext.Exception, MvcAgentElement.LogCategory + ".Exceptions", true);
            base.OnException(filterContext);
        }
    }
}