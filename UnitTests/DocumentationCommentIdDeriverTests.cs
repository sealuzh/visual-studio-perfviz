using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProbeInjector;

namespace UnitTests
{
    [TestClass]
    public class DocumentationCommentIdDeriverTests
    {
        [TestMethod]
        public void InjectionVictimMainTest()
        {
            var documentationCommentId = DocumentationCommentIdDeriver.GetDocumentationCommentId("System.Void InjectionVictim.Program::Main(System.String[])");
            Assert.AreEqual("M:InjectionVictim.Program.Main(System.String[])", documentationCommentId);
        }

        [TestMethod]
        public void InjectionSimpleMethodTest()
        {
            var documentationCommentId = DocumentationCommentIdDeriver.GetDocumentationCommentId("System.Void InjectionVictim.Program::SimpleMethodTest(System.String)");
            Assert.AreEqual("M:InjectionVictim.Program.SimpleMethodTest(System.String)", documentationCommentId);
        }

        [TestMethod]
        public void InjectionVictimMultipleReturnsTest()
        {
            var documentationCommentId = DocumentationCommentIdDeriver.GetDocumentationCommentId("System.Void InjectionVictim.Program::MultipleReturnsTest(System.Boolean)");
            Assert.AreEqual("M:InjectionVictim.Program.MultipleReturnsTest(System.Boolean)", documentationCommentId);
        }

        [TestMethod]
        public void InjectionVictimExceptionsTest()
        {
            var documentationCommentId = DocumentationCommentIdDeriver.GetDocumentationCommentId("System.Void InjectionVictim.Program::ExceptionsTest(System.Boolean)");
            Assert.AreEqual("M:InjectionVictim.Program.ExceptionsTest(System.Boolean)", documentationCommentId);
        }

    }
}
