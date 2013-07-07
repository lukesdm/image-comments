using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LM.RichComments.Domain;
using RichCommentsTests.Properties;
using RichCommentsTests.DataSchema;
using LM.RichComments.Utility;
using RichCommentsTests.Stubs;

namespace RichCommentsTests.Integration
{
    [TestClass]
    public class ParserTests
    {
        private TestContext _testContext;
        private string _contentTypeName;
        UrlProcessor _urlProcessor;
        ImageItemParser _imageItemParser;

        public TestContext TestContext
        {
            get { return _testContext; }
            set { _testContext = value; }
        }

        [TestInitialize]
        public void Initialize()
        {
            _contentTypeName = "CSharp";
            var viewStub = new WpfTextViewStub();
            _urlProcessor = new UrlProcessor(viewStub);
            _imageItemParser = new ImageItemParser(_urlProcessor);
        }

        [DataSource("System.Data.SqlServerCe.3.5", "data source=|DataDirectory|\\TestData.sdf", TestData.ParserData.NAME, DataAccessMethod.Sequential), DeploymentItem("RichCommentsTests\\TestData.sdf"), TestMethod]
        public void ImageItemParserTest()
        {
            // ARRANGE
            string inputLineText = _testContext.DataRow[TestData.ParserData.LineText].ToString();

            bool expectedToParse = bool.Parse(_testContext.DataRow[TestData.ParserData.ShouldBeParsable].ToString());

            ImageItem.Parameters expectedParameters = new ImageItem.Parameters(
                _urlProcessor.MakeUrlFromString(_testContext.DataRow[TestData.ParserData.ExpectedParameter1].ToString()),
                Convert.ToDouble(_testContext.DataRow[TestData.ParserData.ExpectedParameter2]));
            
            int expectedXmlStartPosition = Convert.ToInt32(_testContext.DataRow[TestData.ParserData.ExpectedXmlPosition]);

            string expectedExceptionMessage = _testContext.DataRow[TestData.ParserData.ExpectedException].ToString();

            IRichCommentItemParameters parsedParameters;
            Exception parseException;
            int? xmlStartPosition;

            // ACT
            bool parsedSuccessfully = _imageItemParser.TryParse(_contentTypeName, inputLineText, out parsedParameters, out parseException, out xmlStartPosition);

            var actualImageItemParameters = (ImageItem.Parameters)parsedParameters;

            // ASSERT
            Assert.AreEqual(expectedToParse, parsedSuccessfully);
            Assert.AreEqual(expectedParameters.Url, actualImageItemParameters.Url);
            Assert.AreEqual(expectedParameters.Scale, actualImageItemParameters.Scale);
            Assert.AreEqual(expectedExceptionMessage, (parseException ?? new Exception("")).Message);
            Assert.AreEqual(expectedXmlStartPosition, xmlStartPosition);
        }

        [TestMethod, Description("Tests a single, known good fully specified example string. TODO: More tests.")]
        public void WebItemParserTest() 
        {
            // ARRANGE
            string lineText = @"/// <webpage url=""C:\a.htm"" width=""200"" height=""100"" />";

            WebItem.Parameters expectedParameters = new WebItem.Parameters(200, 100, new Uri(@"C:\a.htm"));
            var view = new WpfTextViewStub();
            WebItemParser webItemParser = new WebItemParser(new UrlProcessor(view));
            Exception parseException;
            int? xmlStartPosition;
            IRichCommentItemParameters itemParameters;
            
            // ACT
            bool parsedSuccesfully = webItemParser.TryParse(_contentTypeName, lineText, out itemParameters, out parseException, out xmlStartPosition);
            WebItem.Parameters actualParameters = (WebItem.Parameters)itemParameters;
            
            // ASSERT
            Assert.IsTrue(parsedSuccesfully);
            Assert.AreEqual(expectedParameters.Url, actualParameters.Url);
            Assert.AreEqual(expectedParameters.Width, actualParameters.Width);
            Assert.AreEqual(expectedParameters.Height, actualParameters.Height);
        }
    }
}
