using LM.RichComments.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.Text.Editor;
using System.IO;

namespace RichCommentsTests
{
    [TestClass()]
    public class UrlProcessorTest
    {
        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod()]
        public void MakeUrlFromStringTest()
        {
            // ARRANGE
            IWpfTextView view = new Stubs.WpfTextViewStub();
            VariableExpander variableExpander = new VariableExpander(view);
            UrlProcessor target = new UrlProcessor(view);
            string urlString = @"..\Docs\picture.jpg";
            Uri expected = new Uri(Path.GetDirectoryName(Properties.Resources.DummyClassFilePath) + @"\" + urlString);

            // ACT
            Uri actual = target.MakeUrlFromString(urlString);

            // ASSERT
            Assert.AreEqual(expected, actual);
        }
    }
}