#region File Header and License
// /*
//    UnhandledExceptionAttribute.cs
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
using System.Web.Mvc;
using Gibraltar.Agent.Web.Mvc.Configuration;

namespace Gibraltar.Agent.Web.Mvc.Filters
{
    /// <summary>
    /// Manages unhandled exceptions
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
                throw new ArgumentNullException(nameof(configuration));

            _configuration = configuration;
        }

        /// <inheritdoc />
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