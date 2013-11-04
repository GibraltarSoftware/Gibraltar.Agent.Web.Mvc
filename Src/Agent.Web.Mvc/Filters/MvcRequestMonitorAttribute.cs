using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Gibraltar.Agent.Web.Mvc.Configuration;
using Gibraltar.Agent.Web.Mvc.Filters.Internal;
using Gibraltar.Agent.Web.Mvc.Internal;

namespace Gibraltar.Agent.Web.Mvc.Filters
{
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
            Order = -100; //so we run first and last in general
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
                    descriptionBuilder.AppendFormat("- {0} = {1}\r\n", param.ParameterName, 
                        ObjectToString(value, _configuration.LogRequestParameterDetails));
                }
            }

            descriptionBuilder.AppendFormat("URL: {0}\r\n", HttpContext.Current.Request.Url);
            descriptionBuilder.AppendFormat("Request Id: {0}\r\n", tracker.UniqueId);
   
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

            //we log exceptions in our unhnadled exception attribute
        }


        /// <summary>
        /// Process an arbitrary object instance into a string representation.
        /// </summary>
        /// <param name="value">An object to represent as a string, such as a return value.</param>
        /// <param name="getObjectDetails">True to get object details by evaluating ToString() even on class types.
        /// False to avoid calling ToString() on class types (still used for struct and enum types).</param>
        /// <returns>A string representing the provided object.</returns>
        internal static string ObjectToString(object value, bool getObjectDetails)
        {
            string valueString = null;
            if (value == null)
            {
                valueString = "(null)";
            }
            else
            {
                Type parameterType = value.GetType();
                if (parameterType.IsClass == false)
                {
                    // Structs and enums should always have efficient ToString implementations, we assume.
                    if (parameterType.IsEnum)
                    {
                        //we want to pop the enum class name in front of the value to make it clear.
                        valueString = parameterType.Name + "." + value.ToString();
                    }
                    else
                    {
                        valueString = value.ToString();
                    }
                }
                else if (parameterType == typeof(string))
                {
                    valueString = (string)value; // Use the value itself if it's already a string.
                }
                else
                {
                    if (getObjectDetails) // Before we call ToString() on a class instance...
                    {
                        valueString = value.ToString(); // Only evaluate ToString() of a class instance if logging details.

                        if (string.IsNullOrEmpty(valueString))
                        {
                            valueString = null; //and we don't need to do the next check...
                        }
                        else if (valueString.StartsWith(parameterType.Namespace + "." + parameterType.Name)) //if it's a generic type then it will have MORE than the full name.
                        {
                            valueString = null; // It's just the base object ToString implementation; we can improve on this.
                        }
                        // Otherwise, we'll keep the result of ToString to describe this object.
                    }

                    if (valueString == null) // If not logging details or if ToString was simply the type...
                    {
                        // Replace it with the type name and hash code to distinguish polymorphism and instance.
                        valueString = string.Format("{{{0}[0x{1:X8}]}}", parameterType.Namespace + "." + parameterType.Name, value.GetHashCode());
                    }
                }
            }

            return valueString;
        }


    }
}
