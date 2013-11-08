Loupe Agent for ASP.NET MVC 4 and Web API
===================

This agent adds Loupe to your ASP.NET MVC 4 web site to automatically record performance
and health information for your MVC and Web API application.  If you don't need
to modify the source code just download the latest [Loupe Agent for ASP.NET MVC]().  
It extends the [Loupe Agent](https://nuget.org/packages/Gibraltar.Agent/) so you can 
use any viewer for Loupe to review the agent's information

Using the Agent
---------------

To activate the agent you will need to register the relevant filters in your Global.asax.cs
file or related initialization sequence.  An example would be:

```C#
using Gibraltar.Agent;
using Gibraltar.Agent.Web.Mvc.Filters;

protected void Application_Start()
{
    Log.StartSession(); //Prompt the Loupe Agent to start immediately

	//Register the three filters
	GlobalConfiguration.Configuration.Filters.Add(new WebApiRequestMonitorAttribute());
	GlobalFilters.Filters.Add(new MvcRequestMonitorAttribute());
	GlobalFilters.Filters.Add(new UnhandledExceptionAttribute());
}
```


Building the Agent
------------------

This project is designed for use with Visual Studio 2012 with NuGet package restore enabled.
When you build it the first time it will retrieve dependencies from NuGet.

Contributing
------------

Feel free to branch this project and contribute a pull request to the development branch. 
If your changes are incorporated into the master version they'll be published out to NuGet for
everyone to use!