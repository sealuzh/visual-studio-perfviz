# visual-studio-perfviz

Visual Studio Perfviz (Perfviz) is a set of programs that allows visualization in Visual Studio of execution times and errors of software deployed in the cloud. It works with any .NET 4.6 program or later and uses Azure Application Insights as data store and to query data. To inject code, Mono.Cecil is used. It consists of 3 major parts:

* Visualization: The InSituVisualization plugin for Visual Studio, used for visualization of values
* Injector: The Probe Injection Executable that injects code into built assemblies
* Probe: The Probe (an assembly) that contains the to be injected code
* Some provided assembly of the user 

Perfviz is programmed to work together with Microsoft Azure, but it contains interfaces to work with any kind of online database or REST interface to query data. Effort has been put into designing a very responsive data store that can be filtered.

Learn more in the wiki here: https://github.com/sealuzh/visual-studio-perfviz/wiki/Summary
