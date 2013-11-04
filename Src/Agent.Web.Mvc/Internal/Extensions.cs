using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Gibraltar.Agent.Web.Mvc.Filters.Internal;

namespace Gibraltar.Agent.Web.Mvc.Internal
{
    /// <summary>
    /// Extension methods for the agent
    /// </summary>
    internal static class Extensions
    {
        private const string HttpContextMetricPrefix = "Loupe Request Metric";

        /// <summary>
        /// Store a request metric into the http context for later use
        /// </summary>
        /// <param name="context"></param>
        /// <param name="metricTracker"></param>
        internal static void Store(this HttpContext context, RequestMetric metricTracker)
        {
            string key = HttpContextMetricPrefix + metricTracker.UniqueId;
            context.Items.Add(key, metricTracker);
        }

        /// <summary>
        /// Retrieve a request metric from the http context
        /// </summary>
        /// <typeparam name="TMetric">The specific type of request metric to return</typeparam>
        /// <param name="context">The current Http context</param>
        /// <param name="uniqueId">the unique id of the request to retrieve the metric for</param>
        /// <returns>The request metric or null if it couldn't be found or was of an incompatible type</returns>
        internal static TMetric Retrieve<TMetric>(this HttpContext context, string uniqueId)
            where TMetric : RequestMetric
        {
            if (string.IsNullOrWhiteSpace(uniqueId))
                throw new ArgumentNullException("uniqueId");

            string key = HttpContextMetricPrefix + uniqueId;

            return context.Items[key] as TMetric;
        }
    }
}
