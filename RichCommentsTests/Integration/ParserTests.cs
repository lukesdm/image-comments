using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LM.RichComments.Domain;
using RichCommentsTests.Properties;

namespace RichCommentsTests.Integration
{
    [TestClass]
    public class ParserTests
    {
        private ImageItem.Parameters _expectedParameters;
        private string _contentTypeName;
        ImageItemParser _imageItemParser;

        [TestInitialize]
        public void Initialize()
        {
            _expectedParameters = new ImageItem.Parameters(@"C:\a.jpg", 0.1);
            _contentTypeName = "CSharp";
            _imageItemParser = new ImageItemParser();
        }

        [TestMethod]
        public void ImageItemParserTest_1()
        {
            string inputLineText = Resources.ImageComment_ShouldParse1; 
            IRichCommentItemParameters parsedParameters;
            Exception parseException;
            int? xmlStartPosition;

            bool parsedSuccessfully = _imageItemParser.TryParse(_contentTypeName, inputLineText, out parsedParameters, out parseException, out xmlStartPosition);

            var actualImageItemParameters = (ImageItem.Parameters)parsedParameters;

            Assert.AreEqual(_expectedParameters.Url, actualImageItemParameters.Url);
            Assert.AreEqual(_expectedParameters.Scale, actualImageItemParameters.Scale);
            Assert.IsNull(parseException);
            Assert.AreEqual(3, xmlStartPosition);
        }

        [TestMethod]
        public void ImageItemParserTest_2()
        {
            string inputLineText = Resources.ImageComment_ShouldParse2;
            IRichCommentItemParameters parsedParameters;
            Exception parseException;
            int? xmlStartPosition;

            bool parsedSuccessfully = _imageItemParser.TryParse(_contentTypeName, inputLineText, out parsedParameters, out parseException, out xmlStartPosition);

            var actualImageItemParameters = (ImageItem.Parameters)parsedParameters;

            Assert.AreEqual(_expectedParameters.Url, actualImageItemParameters.Url);
            Assert.AreEqual(_expectedParameters.Scale, actualImageItemParameters.Scale);
            Assert.IsNull(parseException);
            Assert.AreEqual(3, xmlStartPosition);
        }

        [TestMethod]
        public void ImageItemParserTest_3()
        {
            string inputLineText = Resources.ImageComment_ShouldParse3;
            IRichCommentItemParameters parsedParameters;
            Exception parseException;
            int? xmlStartPosition;

            bool parsedSuccessfully = _imageItemParser.TryParse(_contentTypeName, inputLineText, out parsedParameters, out parseException, out xmlStartPosition);

            var actualImageItemParameters = (ImageItem.Parameters)parsedParameters;

            Assert.AreEqual(_expectedParameters.Url, actualImageItemParameters.Url);
            Assert.AreEqual(_expectedParameters.Scale, actualImageItemParameters.Scale);
            Assert.IsNull(parseException);
            Assert.AreEqual(5, xmlStartPosition);
        }

        [TestMethod]
        public void ImageItemParserTest_4()
        {
            string inputLineText = Resources.ImageComment_ShouldParse4;
            IRichCommentItemParameters parsedParameters;
            Exception parseException;
            int? xmlStartPosition;

            bool parsedSuccessfully = _imageItemParser.TryParse(_contentTypeName, inputLineText, out parsedParameters, out parseException, out xmlStartPosition);

            var actualImageItemParameters = (ImageItem.Parameters)parsedParameters;

            Assert.AreEqual(_expectedParameters.Url, actualImageItemParameters.Url);
            Assert.AreEqual(_expectedParameters.Scale, actualImageItemParameters.Scale);
            Assert.IsNull(parseException);
            Assert.AreEqual(7, xmlStartPosition);
        }
    }
}
