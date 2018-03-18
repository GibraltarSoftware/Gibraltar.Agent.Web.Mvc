#region File Header and License
// /*
//    AgentActionContext.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Mvc;
#pragma warning disable 1591

namespace Gibraltar.Agent.Web.Mvc.Internal
{
    public class AgentActionContext
    {
        private readonly ActionExecutingContext _mvcContext;
        private readonly HttpActionContext _webApiContext;

        public AgentActionContext(ActionExecutingContext context)
        {
            _mvcContext = context;
        }

        public AgentActionContext(HttpActionContext context)
        {
            _webApiContext = context;
        }

        public Type Controller()
        {
            if (_webApiContext != null)
            {
                return _webApiContext.ActionDescriptor.ControllerDescriptor.ControllerType;
            }

            if (_mvcContext != null)
            {
                return _mvcContext.ActionDescriptor.ControllerDescriptor.ControllerType;
            }

            return null;
        }

        public string Action()
        {
            if (_webApiContext != null)
            {
                return _webApiContext.ActionDescriptor.ActionName;
            }


            if (_mvcContext != null)
            {
                return _mvcContext.ActionDescriptor.ActionName;
            }

            return null;
        }

        public IEnumerable<string> GetParameters()
        {
            if (_mvcContext != null)
            {
                return _mvcContext.ActionDescriptor.GetParameters().Select(x => x.ParameterName);
            }

            if (_webApiContext != null)
            {
                return _webApiContext.ActionDescriptor.GetParameters().Select(x => x.ParameterName);
            }

            return null;
        }

        public object GetParameter(string parameterName)
        {
            if (_mvcContext != null)
            {
                return _mvcContext.ActionParameters[parameterName];
            }

            if (_webApiContext != null)
            {
                return _webApiContext.ActionArguments[parameterName];
            }

            return null;
        }
    }
}