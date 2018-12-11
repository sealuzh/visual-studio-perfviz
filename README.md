# Visual Studio Perfviz (Perfviz)

Perfviz enables [Feedback Driven Development](http://cloudwave-fp7.eu/content/feedback-driven-development) and displays in situ performance metrics in the Visual Studio IDE. Performance metrics may be collected from live applications deployed in a cloud environment. The data is mapped to code entities and visualized inside the Visual Studio IDE in a convenient way for developers to comprehend.

Currently the [Microsoft Azure Cloud Computing Platform](https://azure.microsoft.com/) is the only supported plattform. The application architecture allows for support of other cloud plattforms with only minor changes. All Perfviz applications are currently set to use [Azure Application Insights](https://azure.microsoft.com/en-us/services/application-insights/) to store and manage collected metrics.

## Components
Perfviz consists of three components

### 1. Probes
The process of detecting bad performance starts with data collection. In Perfviz performance metrics are collected by code instrumentation via probe assemblies. These probes get injected into the application and are responsible to **collect and handle (store etc.) performance metrics**.

There are multiple probes implemented in Perfviz to suit different needs.

### 2. ProbeInjector
The ProbeInjector is responsible to **inject the probes into an application**. It is a command line tool that employs reweaving of the [.Net IL code](https://en.wikipedia.org/wiki/Common_Intermediate_Language) using [Mono.Cecil](http://www.mono-project.com/docs/tools+libraries/libraries/Mono.Cecil/).

Currently the ProbeInjector inserts code into the control flow of the application:
* Before every Method Body
* After every Method Body
* Before every Exception throw

Usage:

```
Probeinjector.exe -r "ApplicationToInstrument.dll" -w "Output.dll" -p "ProbeToInject.dll"
```


### 3. InSituVisualization
The InSituVisualization is an **extension for the Visual Studio IDE** and the main component of Perfviz. The Extension is responsible to get the collected performance data from the cloud and to map it onto code entities in the IDE. There are many different views available to support developers in the process of understanding performance problems. The origin of the available data is always trackable.

This is achieved trough the use of the [The .NET Compiler Platform ("Roslyn")](https://github.com/dotnet/roslyn)


#### In Situ Visualization
In situ visualizations are attached to **methods**, **methodInvocations** and **loops**. The displayed values are calculated by an exchangable model. The default model shows mean execution times over all executions. For faster recognition, there are color codes available which are displayed behind the actual mean execution time. The default setting is a Transition from Green (120° Hue) to Red (0° Hue) using the HSV Color Value. The user is able to define custom color codes individually for both, the method visualization as well as the visualization for the method invocation. A converter is available to set the color coding. Additionally to the color coding, the impact of the method is displayed in form of a progressbar like visualization. The progressbar fills up the in situ from left to right. For methods, with available data on, there are sparkline like visualizations that display the recent data in the background. The recent data visualization helps to identify trends (e.g. if the method execution times are rising) as well as outliner data with a simple glance. Red questionmarks behind the visualizations show the value has been predicted.

![In Situ Visualization](https://github.com/sealuzh/visual-studio-perfviz/blob/master/Screenshots/SimpleMethod.PNG)

#### Predictions
Whenever changes are made to a method, a simple prediction of the new mean execution time is given. The prediction currently consists of a simple sum of the known execution times. 
For loops there is a prediction of the average loop times displayed, which is calculated trough division of the method execution time by the sum of the known execution times inside the loop. An additional slider allows the adjustment of the estimated loop iterations. The newly predicted execution time of the method is propagated upwards in the abstract syntax tree, so that the predicted execution time of all callers is adjusted.

![Loop Predictions](https://github.com/sealuzh/visual-studio-perfviz/blob/master/Screenshots/Slider.gif)

#### Detailed Breakdown
A detail window is available to show further information about a certain telemetry. It is available by clicking an In Situ Visualization.

![Details](https://github.com/sealuzh/visual-studio-perfviz/blob/master/Screenshots/PerformanceDetails.PNG)

From the detail window further data analysis can be started and recorded telemetry data is available directly.

![Recorded Telemetry](https://github.com/sealuzh/visual-studio-perfviz/blob/master/Screenshots/RecordedTelemetry.PNG)

#### Filters
To support a developer in narrowing down a performance problem, a set of filters can be employed. All views of the extension are automatically adjusted to use the filtered set of data.

![Filters](https://github.com/sealuzh/visual-studio-perfviz/blob/master/Screenshots/PerformanceFilters.PNG)

Learn more in the [WIKI Here](../../wiki/Home)



## Requirements

* Visual Studio 2017 15.5.7 or newer

## Open Points / Improvements

* Switch from using IWpfTextViewCreationListener to Microsoft.VisualStudio.Text.Tagging as in the [VSSDK-Extensibility-Samples](https://github.com/Microsoft/VSSDK-Extensibility-Samples/tree/master/Intra-text_Adornment)
* Currently, the data is pulled using the Azure External References API. This could be changed by using more appropriate method calls such as TrackEvent(), where for example exceptions could be handled with.
* To be able to submit data when the injected application is running locally, the ApplicationInsights calls not only need a call as TrackDependency, but also the call Flush() that clears the in-memory buffer. This call is not needed when the instance is running in the cloud and slows down the application itself.
* Currently, the solution lacks an user interace where filters can be added and removed.
* The extension could track when the solution is published and therefore set a global datetime filter to only display the newest data that matches the current architecture.

## Known Bugs

* Loop-Inference: Detection of changes and updating the inferred values accordingly is currently not working as intended, as the previous state is currently unknown (e.g. when inserting a new method into the loop, the plugin assumes that this is the currently deployed version and therefore calculates the inferred value wrongly).
