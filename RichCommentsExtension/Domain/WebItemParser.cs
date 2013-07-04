using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LM.RichComments.Utility;
using System.Xml.Linq;
using System.Xml;

namespace LM.RichComments.Domain
{
    class WebItemParser : IRichCommentParser
    {
        public WebItemParser(UrlProcessor urlProcessor)
        {
            this.UrlProcessor = urlProcessor;
        }

        public string ExpectedTagName
        {
            get { return "webpage"; }
        }

        public bool TryParse(string contentTypeName, string lineText, out IRichCommentItemParameters richCommentItemParameters, out Exception parseException, out int? xmlStartPosition)
        {
            richCommentItemParameters = null;
            parseException = null;
            string elementName;
            string matchedXml;

            bool matchedComment = ParseHelper.MatchXmlComment(contentTypeName, lineText, out matchedXml, out xmlStartPosition, out elementName);

            if (!matchedComment || elementName.ToLower() != this.ExpectedTagName)
            {
                return false;
            }

            Uri webpageUri;
            //string webPageUrlString;
            double width;
            double height;

            // Parse XML element contents
            try
            {
                XElement webPageElement = XElement.Parse(matchedXml);
                XAttribute urlAttribute = webPageElement.Attribute("url");

                if (urlAttribute == null)
                {
                    parseException = new XmlException("url attribute not specified.");
                    return false;
                }

                webpageUri = this.UrlProcessor.MakeUrlFromString(urlAttribute.Value);

                XAttribute widthAttr = webPageElement.Attribute("width");
                try
                {
                    width = double.Parse(widthAttr.Value);
                }
                catch
                {
                    throw new ArgumentException("Couldn't find width attribute or convert it to a number.");
                }
                
                XAttribute heightAttr = webPageElement.Attribute("height");
                try
                {
                    height = double.Parse(heightAttr.Value);
                }
                catch
                {
                    throw new ArgumentException("Couldn't find height attribute or convert it to a number.");
                }
                
                richCommentItemParameters = new WebItem.Parameters(width, height, webpageUri);
                return true;
            }
            catch (Exception ex)
            {
                parseException = ex;
                return false;
            }
        }

        public UrlProcessor UrlProcessor { get; private set; }
    }
}
