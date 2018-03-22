using System;
using System.Collections.Generic;
using System.Linq;
using InSituVisualization.Filter;
using InSituVisualization.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Testing Filter Classes
    /// </summary>
    [TestClass]
    public class FilterTests
    {
        private readonly List<RecordedMethodTelemetry> _testlist = new List<RecordedMethodTelemetry>
        {
            new RecordedMethodTelemetry("SomeDocumentateionCommentId", "someId1", new DateTime(2010, 1, 1), null),
            new RecordedMethodTelemetry("SomeMethodHere", "someId2", new DateTime(2010, 3, 2), null),
            new RecordedMethodTelemetry("TestDocumentationId", "someId3", new DateTime(2010, 3, 3), null),
            new RecordedMethodTelemetry("SomeDocumentateionCommentId2", "someId4", new DateTime(2010, 3, 4), null)
        };

        [TestMethod]
        public void StringFilterContainsTest()
        {
            var filter = new StringFilter(telemetry => telemetry.DocumentationCommentId, "SomeMethod") {FilterKind = FilterKind.Contains};
            var newList = filter.ApplyFilter(_testlist);

            Assert.IsNotNull(newList);
            Assert.AreEqual(1, newList.Count);
            Assert.AreEqual("SomeMethodHere", newList.First().DocumentationCommentId);
        }

        [TestMethod]
        public void StringFilterEqualsTest()
        {
            var filter = new StringFilter(telemetry => telemetry.DocumentationCommentId, "SomeDocumentateionCommentId2") { FilterKind = FilterKind.IsEqual };
            var newList = filter.ApplyFilter(_testlist);

            Assert.IsNotNull(newList);
            Assert.AreEqual(1, newList.Count);
            Assert.AreEqual("SomeDocumentateionCommentId2", newList.First().DocumentationCommentId);
        }

        [TestMethod]
        public void ComparableFilterEqualsTest()
        {
            var filter = new ComparableFilter<DateTime>(telemetry => telemetry.Timestamp, new DateTime(2010, 3, 2)) { FilterKind = FilterKind.IsEqual };
            var newList = filter.ApplyFilter(_testlist);

            Assert.IsNotNull(newList);
            Assert.AreEqual(1, newList.Count);
            Assert.AreEqual("SomeMethodHere", newList.First().DocumentationCommentId);
        }

        [TestMethod]
        public void ComparableFilterGreaterEqualsTest()
        {
            var filter = new ComparableFilter<DateTime>(telemetry => telemetry.Timestamp, new DateTime(2010, 3, 3)) { FilterKind = FilterKind.IsGreaterEqualThen };
            var newList = filter.ApplyFilter(_testlist);

            Assert.IsNotNull(newList);
            Assert.AreEqual(2, newList.Count);
            Assert.AreEqual("SomeDocumentateionCommentId2", newList.Last().DocumentationCommentId);
        }

        [TestMethod]
        public void ComparableFilterSmallerEqualsTest()
        {
            var filter = new ComparableFilter<DateTime>(telemetry => telemetry.Timestamp, new DateTime(2010, 3, 3)) { FilterKind = FilterKind.IsSmallerEqualThen };
            var newList = filter.ApplyFilter(_testlist);

            Assert.IsNotNull(newList);
            Assert.AreEqual(3, newList.Count);
            Assert.AreEqual("TestDocumentationId", newList.Last().DocumentationCommentId);
        }
    }
}
