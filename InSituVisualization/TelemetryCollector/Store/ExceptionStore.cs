using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Timers;
using InSituVisualization.TelemetryCollector.Filter;
using InSituVisualization.TelemetryCollector.Filter.Property;
using InSituVisualization.TelemetryCollector.Model.AveragedMember;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;

namespace InSituVisualization.TelemetryCollector.Store
{
    public class ExceptionStore : Store<ConcreteMethodException>
    {
        //private readonly Type _modelType = typeof(ConcreteMethodTelemetry);
        //private readonly FilterController<ConcreteMethodTelemetry> _filterController;

        public ExceptionStore(string fileName) : base(fileName)
        {
            //As second attribute return the position inside the _filterController.GetFilterProperties()[0].GetFilterKinds() list, which is used for displaying possible filterparameters.
            //_filterController.AddFilterGlobal(_filterController.GetFilterProperties()[1], FilterKind.IsGreaterEqualThen, new DateTime(2017, 11, 21));
            //_filterController.AddFilterGlobal(_filterController.GetFilterProperties()[2], IntFilterProperty.IsGreaterEqualThen, 1000);
            //_filterController.AddFilterLocal(_filterController.GetFilterProperties()[2], IntFilterProperty.IsGreaterEqualThen, 100, "ASP.testbuttonpage_aspx.Counter2");
        }


    }
}
