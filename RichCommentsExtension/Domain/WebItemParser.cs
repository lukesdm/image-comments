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

            string webPageUrlString;
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

                webPageUrlString = urlAttribute.Value;

                XAttribute widthAttr = webPageElement.Attribute("width");
                double.TryParse(widthAttr.Value, out width);
                
                XAttribute heightAttr = webPageElement.Attribute("height");
                double.TryParse(heightAttr.Value, out height);
                
                richCommentItemParameters = new WebItem.Parameters(width, height, webPageUrlString);
                return true;
            }
            catch (Exception ex)
            {
                parseException = ex;
                return false;
            }
        }

        public string ExpectedTagName
        {
            get { return "webpage"; }
        }
    }
}
