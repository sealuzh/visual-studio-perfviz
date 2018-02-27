# visual-studio-perfviz

Visual Studio Perfviz (Perfviz) allows  Visual Studio (15.5.7+) to display in situ visualization of performance metrics collected from the cloud. It works with any .NET 4.6 program or later and uses Azure Application Insights as data store and to query data. To inject code, Mono.Cecil is used. 

It consists of 3 major parts:

* Visualization: The InSituVisualization plugin for Visual Studio, used for visualization of values
* Injector: The Probe Injection Executable that injects code into built assemblies
* Probe: The Probe (an assembly) that contains the to be injected code
* Some provided assembly of the user 

Perfviz is primarily designed for cloud applications on Microsoft Azure with Application Insights. The provided interfaces allow easy extension to work with any kind of online database or REST interface to query data. Effort has been put into designing a very responsive data store that can be filtered.

Learn more in the wiki here: https://github.com/sealuzh/visual-studio-perfviz/wiki/Home

# Requirements

* Visual Studio 2017 15.5.7 or newer

# Open Points / Improvements

* Currently, the data is pulled using the Azure External References API. This could be changed by using more appropriate method calls such as TrackEvent(), where for example exceptions could be handled with.
* To be able to submit data when the injected application is running locally, the ApplicationInsights calls not only need a call as TrackDependency, but also the call Flush() that clears the in-memory buffer. This call is not needed when the instance is running in the cloud and slows down the application itself.
* Currently, the solution lacks an user interace where filters can be added and removed.
* The plugin should track when the solution is published and therefore set a global datetime filter to only display the newest data that matches the current architecture.

# Bug Tracker

* Loop-Inference: Detection of changes and updating the inferred values accordingly is currently not working, as the previous state is currently unknown (e.g. when inserting a new method into the loop, the plugin assumes that this is the currently deployed version and therefore calculates the inferred value wrongly).
* Dependency problem: For some reason some dependencies are not loaded into the build correctly. This differs from development environment to development environment (for some development environments, e.g., Newtonsoft.Json v10.0.0 is working fine, but v11.0.0 cannot be loaded. For some other development environment, this is exactly the other way round). This doesnt only account for one dependency, but for a random selection of them.
