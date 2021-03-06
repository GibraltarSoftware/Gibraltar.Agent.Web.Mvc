﻿#region File Header and License
// /*
//    MvcAgentElement.cs
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
using System.Configuration;
using System.Web.UI;
#pragma warning disable 1591

namespace Gibraltar.Agent.Web.Mvc.Configuration
{
    public class MvcAgentElement : ConfigurationSection
    {
        /// <summary>
        /// The root log category for this agent
        /// </summary>
        internal const string LogCategory = "Web Site";

        /// <summary>
        /// Determines if any agent functionality should be enabled.  Defaults to true.
        /// </summary>
        /// <remarks>To disable the entire agent set this option to false.  Even if individual
        /// options are enabled they will be ignored if this is set to false.</remarks>
        [ConfigurationProperty("enabled", DefaultValue = true, IsRequired = false)]
        public bool Enabled { get { return (bool)this["enabled"]; } set { this["enabled"] = value; } }

        /// <summary>
        /// Determines if a log message is written for each request handled by MVC and Web API. Defaults to true.
        /// </summary>
        [ConfigurationProperty("logRequests", DefaultValue = true, IsRequired = false)]
        public bool LogRequests { get { return (bool)this["logRequests"]; } set { this["logRequests"] = value; } }

        /// <summary>
        /// Determines if request log messages should include the parameter values used for the request. Defaults to true.
        /// </summary>
        [ConfigurationProperty("logRequestParameters", DefaultValue = true, IsRequired = false)]
        public bool LogRequestParameters { get { return (bool)this["logRequestParameters"]; } set { this["logRequestParameters"] = value; } }

        /// <summary>
        /// Determines if request log messages should include object details. Defaults to false.
        /// </summary>
        /// <remarks>This setting has no effect if LogRequestParameters is false.</remarks>
        [ConfigurationProperty("logRequestParameterDetails", DefaultValue = true, IsRequired = false)]
        public bool LogRequestParameterDetails { get { return (bool)this["logRequestParameterDetails"]; } set { this["logRequestParameterDetails"] = value; } }
        
        /// <summary>
        /// The severity used for log messages for the start of each request handled by MVC and Web API. Defaults to Verbose.
        /// </summary>
        [ConfigurationProperty("requestMessageSeverity", DefaultValue = LogMessageSeverity.Verbose, IsRequired = false)]
        public LogMessageSeverity RequestMessageSeverity { get { return (LogMessageSeverity)this["requestMessageSeverity"]; } set { this["requestMessageSeverity"] = value; } }

        /// <summary>
        /// The severity used for log messages for the end of each failed request handled by MVC and Web API. Defaults to Warning.
        /// </summary>
        [ConfigurationProperty("requestExceptionSeverity", DefaultValue = LogMessageSeverity.Warning, IsRequired = false)]
        public LogMessageSeverity RequestExceptionSeverity { get { return (LogMessageSeverity)this["requestExceptionSeverity"]; } set { this["requestExceptionSeverity"] = value; } }

        /// <summary>
        /// Determines if a log message is written for unhandled exceptions on requests handled by MVC and Web API. Defaults to true.
        /// </summary>
        [ConfigurationProperty("logUnhandledExceptions", DefaultValue = true, IsRequired = false)]
        public bool LogUnhandledExceptions { get { return (bool)this["logUnhandledExceptions"]; } set { this["logUnhandledExceptions"] = value; } }

        /// <summary>
        /// The severity used for log messages for the end of each failed request handled by MVC and Web API. Defaults to Error.
        /// </summary>
        [ConfigurationProperty("unhandledExceptionSeverity", DefaultValue = LogMessageSeverity.Error, IsRequired = false)]
        public LogMessageSeverity UnhandledExceptionSeverity { get { return (LogMessageSeverity)this["unhandledExceptionSeverity"]; } set { this["unhandledExceptionSeverity"] = value; } }

        /// <summary>
        /// Load the element from the system configuration file, falling back to defaults if it can't be parsed
        /// </summary>
        /// <returns>A new element object</returns>
        internal static MvcAgentElement SafeLoad()
        {
            MvcAgentElement configuration = null;
            try
            {
                //see if we can get a configuration section
                configuration = ConfigurationManager.GetSection("gibraltar/mvcAgent") as MvcAgentElement;
            }
            catch (Exception ex)
            {
                Log.Error(ex, LogCategory + ".Agent", "Unable to load the MVC Agent configuration from the config file",
                          "The default configuration will be used which will undoubtedly create unexpected behavior.  Exception:\r\n{0}", ex.Message);
            }

            return configuration ?? new MvcAgentElement();            
        }
    }
}
