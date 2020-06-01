#region File Header and License
// /*
//    RequestMetric.cs
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
using System.Diagnostics;
using System.Web;
using Gibraltar.Agent.Metrics;
using Gibraltar.Agent.Web.Mvc.Internal;

namespace Gibraltar.Agent.Web.Mvc.Filters.Internal
{
    [EventMetric("Loupe", "Web Site.Requests", "Controller Hit", Caption = "Controller Hit", Description = "Performance data for every call to an MVC controller or Web API controller in the application")]
    internal abstract class RequestMetric: IMessageSourceProvider
    {
        private readonly Stopwatch _timer;

        internal RequestMetric()
        {
            StartTimestamp = DateTimeOffset.Now;
            _timer = Stopwatch.StartNew();

            SessionId = HttpContext.Current.GetSessionId();
            AgentSessionId = HttpContext.Current.GetAgentSessionId();
        }

        [EventMetricValue("startTimestamp", SummaryFunction.Count, null, Caption = "Started", Description = "Timestamp the request started")]
        public DateTimeOffset StartTimestamp { get; set; }

        /// <summary>
        /// The duration the request has been running
        /// </summary>
        /// <remarks>Once the request has been told to record it the timer duration will stop increasing.</remarks>
        [EventMetricValue("duration", SummaryFunction.Average, "ms", Caption = "Duration", Description = "The entire time spent processing the request, excluding time to return the response", IsDefaultValue = true)]
        public TimeSpan Duration { get { return _timer.Elapsed; }}

        /// <summary>
        /// The controller and action referenced by the request
        /// </summary>
        [EventMetricValue("request", SummaryFunction.Count, null, Caption = "Request", Description = "The controller and action referenced by the request")]
        public string Request { get { return string.Format("{0}:{1}", ControllerName, ActionName); } }

        /// <summary>
        /// Gets/Sets a String which indicates if the Action was an MVC or WebApi action
        /// </summary>
        [EventMetricValue("subCategory", SummaryFunction.Count, null, Caption = "Subcategory", Description = "The type of API this request came from (MVC or Web API)")]
        public String SubCategory { get; protected set; }

        /// <summary>
        /// Gets/Sets the name of the controller this action belongs to
        /// </summary>
        [EventMetricValue("controllerName", SummaryFunction.Count, null, Caption = "Controller", Description = "The short-form name of the controller used for the request (not the .NET class name)")]
        public String ControllerName { get; protected set; }

        /// <summary>
        /// Gets/sets the name of this action
        /// </summary>
        [EventMetricValue("actionName", SummaryFunction.Count, null, Caption = "Action", Description = "The short-form name of the action used for the request (not the .NET method name)")]
        public String ActionName { get; protected set; }

        /// <summary>
        /// The class name of the controller used for the request
        /// </summary>
        [EventMetricValue("controllerType", SummaryFunction.Count, null, Caption = "Controller Type", Description = "The class name of the controller used for the request")]
        public String ControllerType { get { return ClassName; } }

        /// <summary>
        /// Gets/Sets the HttpMethod (GET, POST, PUT, DELETE, etc) used for this action.
        /// </summary>
        /// <remarks>
        /// In MVC, some actions (typically an EDIT) have both definition for both GET and
        /// POST.  This value helps differentiate between those two calls
        /// </remarks>
        [EventMetricValue("httpMethod", SummaryFunction.Count, null, Caption = "Method", Description = "The HTTP Method (GET, POST, PUT, DELETE, etc) used for this action")]
        public String HttpMethod { get; protected set; }

        /// <summary>
        /// Gets/Sets a String that represents the parameters passed to this action
        /// </summary>
        /// <remarks></remarks>
        [EventMetricValue("parameters", SummaryFunction.Count, null, Caption = "Parameters", Description = "The list of parameters used for this action")]
        public String Parameters { get; protected set; }

        /// <summary>
        /// The unique Id of this controller &amp; action from the framework; MVC only.
        /// </summary>
        public string UniqueId { get; protected set; }

        /// <summary>
        /// The user name for the action being performed.
        /// </summary>
        [EventMetricValue("userName", SummaryFunction.Count, null, Caption = "User", Description = "The user associated with the action being performed")]
        public string UserName { get; set; }

        /// <summary>
        /// The exception, if any, thrown at the completion of the routine
        /// </summary>
        [EventMetricValue("exception", SummaryFunction.Count, null, Caption = "Exception", Description = "The exception, if any, thrown at the completion of the routine")]
        public Exception Exception { get; set; }

        /// <summary>
        /// Id to identify a user session from the web browser
        /// </summary>
        [EventMetricValue("SessionId", SummaryFunction.Count, null, Caption = "SessionId", Description = "Session Id associated with action being performed")]
        public string SessionId { get; set; }

        [EventMetricValue("AgentSessionId", SummaryFunction.Count, null, Caption = "AgentSessionId", Description = "Id from JavaScript agent for session")]
        public string AgentSessionId { get; set; }


        /// <summary>
        /// Suppress recording this metric
        /// </summary>
        /// <remarks>This is used by Loupe to prevent recording of Loupe Module requests but can be used by any code to prevent recording a metric for a request.</remarks>
        public bool Suppress { get; set; }

        /// <summary>
        /// Records the metrics for this request
        /// </summary>
        public void Record()
        {
            _timer.Stop(); 

            if (!Suppress)
                EventMetric.Write(this);
        }

        public string MethodName { get; protected set; }

        public string ClassName { get; protected set; }

        public string FileName { get; protected set; }

        public int LineNumber { get; protected set; }
    }
}
