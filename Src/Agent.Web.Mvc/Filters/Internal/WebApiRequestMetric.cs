#region File Header and License
// /*
//    WebApiRequestMetric.cs
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
