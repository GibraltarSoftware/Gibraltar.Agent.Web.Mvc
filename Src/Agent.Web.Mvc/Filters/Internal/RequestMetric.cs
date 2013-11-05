using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using Gibraltar.Agent.Metrics;

namespace Gibraltar.Agent.Web.Mvc.Filters.Internal
{
    internal abstract class RequestMetric: IMessageSourceProvider
    {
        private readonly Stopwatch _timer;

        internal RequestMetric()
        {
            StartTimestamp = DateTimeOffset.Now;
            _timer = Stopwatch.StartNew();
        }

        public DateTimeOffset StartTimestamp { get; set; }

        /// <summary>
        /// The duration the request has been running
        /// </summary>
        /// <remarks>Once the request has been told to record it the timer duration will stop increasing.</remarks>
        public TimeSpan Duration { get { return _timer.Elapsed; }}

        /// <summary>
        /// Gets/Sets a String which indicates if the Action was an MVC or WebApi action
        /// </summary>
        public String SubCategory { get; protected set; }

        /// <summary>
        /// Gets/Sets the nme of the controller this action belongs to
        /// </summary>
        public String ControllerName { get; protected set; }

        /// <summary>
        /// Gets/sets the name of this action
        /// </summary>
        public String ActionName { get; protected set; }

        /// <summary>
        /// Gets/Sets the HttpMethod (GET, POST, PUT, DELETE, etc) used for this action.
        /// </summary>
        /// <remarks>
        /// In MVC, some actions (typically an EDIT) have both definition for both GET and
        /// POST.  This value helps differentiate between those two calls
        /// </remarks>
        public String HttpMethod { get; protected set; }

        /// <summary>
        /// Gets/Sets a String that represents the parameters passed to this action
        /// </summary>
        /// <remarks></remarks>
        public String Parameters { get; protected set; }

        /// <summary>
        /// The unique Id of this request from the framework
        /// </summary>
        public string UniqueId { get; protected set; }

        /// <summary>
        /// The user name for the action being performed.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The exception, if any, thrown at the completion of the routine
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Records the metrics for this request
        /// </summary>
        public void Record()
        {
            _timer.Stop(); 
            EventMetric.Write(this);
            SampledMetric.Write(this);
        }

        public string MethodName { get; protected set; }

        public string ClassName { get; protected set; }

        public string FileName { get; protected set; }

        public int LineNumber { get; protected set; }
    }
}
