#region File Header and License
// /*
//    MonitorRequestLogging.cs
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

using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Gibraltar.Agent.Web.Mvc.Configuration;

namespace Gibraltar.Agent.Web.Mvc.Internal
{
    public class MonitorRequestLogging
    {
        private const string LogSystem = "Loupe";
        private readonly AgentActionContext _context;
        private readonly MvcAgentElement _configuration;
        private readonly string _logSystem;

        public MonitorRequestLogging(HttpActionContext actionContext, MvcAgentElement configuration)
        {
            _configuration = configuration;
            _context = new AgentActionContext(actionContext);
        }

        public MonitorRequestLogging(ActionExecutingContext actionContext, MvcAgentElement configuration)
        {
            _configuration = configuration;
            _context = new AgentActionContext(actionContext);
        }

        public void Log(string category, string caption, string userName, IMessageSourceProvider tracker)
        {
            var descriptionBuilder = new StringBuilder(1024);

            AddSessionIds(descriptionBuilder);

            descriptionBuilder.AppendFormat("Controller: {0}\r\n", _context.Controller());
            descriptionBuilder.AppendFormat("Action: {0}\r\n", _context.Action());
            if (_configuration.LogRequestParameters)
            {
                descriptionBuilder.AppendLine("Parameters:");
                foreach (var param in _context.GetParameters())
                {
                    object value = _context.GetParameter(param);
                    descriptionBuilder.AppendFormat("- {0}: {1}\r\n", param,
                        Extensions.ObjectToString(value, _configuration.LogRequestParameterDetails));
                }
            }

            descriptionBuilder.AppendFormat("\r\nURL: {0}\r\n", HttpContext.Current.Request.Url);

            var details = CreateDetails();

            Agent.Log.Write(_configuration.RequestMessageSeverity, LogSystem, tracker, userName, null,
                LogWriteMode.Queued, details, category, caption, descriptionBuilder.ToString());            
        }

        private void AddSessionIds(StringBuilder descriptionBuilder)
        {
            string sessionId = HttpContext.Current.GetSessionId();
            string agentId = HttpContext.Current.GetAgentSessionId();

            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                descriptionBuilder.AppendFormat("SessionId: {0}\r\n", sessionId);
            }

            if (!string.IsNullOrWhiteSpace(agentId))
            {
                descriptionBuilder.AppendFormat("JS Agent SessionId: {0}\r\n", agentId);
            }
        }

        private string CreateDetails()
        {
            var details = new StringBuilder();
            string sessionId = HttpContext.Current.GetSessionId();
            string agentSessionId = HttpContext.Current.GetAgentSessionId();

            details.Append("<Details>");
            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                details.AppendFormat("<SessionId>{0}</SessionId>", sessionId);
            }

            if (!string.IsNullOrWhiteSpace(agentSessionId))
            {
                details.AppendFormat("<AgentSessionId>{0}</AgentSessionId>", agentSessionId);
            }

            if (!string.IsNullOrWhiteSpace(sessionId) && HttpContext.Current.Cache[sessionId] != null)
            {
                details.Append(HttpContext.Current.Cache[sessionId]);
            }

            details.AppendFormat("<Controller>{0}</Controller>", _context.Controller());
            details.AppendFormat("<Action>{0}</Action>", _context.Action());
            if (_configuration.LogRequestParameters)
            {
                details.Append("<Parameters>");
                foreach (var param in _context.GetParameters())
                {
                    object value = _context.GetParameter(param);
                    details.AppendFormat("<Parameter name='{0}' value='{1}' />", param,
                       Extensions.ObjectToString(value, _configuration.LogRequestParameterDetails));
                }
                details.Append("</Parameters>");
            }

            details.AppendFormat("<Url>{0}</Url>", HttpContext.Current.Request.Url);

            details.Append("</Details>");

            return details.ToString();
        }
    }
}