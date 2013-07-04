using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using LM.RichComments.Utility;
using System.Xml.Linq;

namespace LM.RichComments.Domain
{
    class ImageItemParser : IRichCommentParser
    {

        public ImageItemParser(UrlProcessor urlProcessor)
        {
            //this.VariableExpander = variableExpander;
            this.UrlProcessor = urlProcessor;
        }

        public string ExpectedTagName 
        { 
            get { return "image"; }
        }

        //public VariableExpander VariableExpander { get; private set; }

        public bool TryParse(string contentTypeName, string lineText, out IRichCommentItemParameters itemParameters, out Exception parseException, out int? xmlStartPosition)
        {
            parseException = null;
            itemParameters = null;
            xmlStartPosition = null;

            string matchedXml;
            
            string elementName;
            bool matchedComment = ParseHelper.MatchXmlComment(contentTypeName, lineText, out matchedXml, out xmlStartPosition, out elementName);

            if (!matchedComment || elementName.ToLower() != this.ExpectedTagName)
            {
                return false;
            }

            string imageUrl;
            double imageScale = 0; // See ImageItem.cs for explanation of default value here

            // Parse XML element contents
            try
            {
                XElement imageElement = XElement.Parse(matchedXml);
                XAttribute urlAttribute = imageElement.Attribute("url");
                
                if (urlAttribute == null)
                {
                    parseException = new XmlException("url attribute not specified.");
                    return false;
                }
                
                imageUrl = urlAttribute.Value;
                
                XAttribute scaleAttr = imageElement.Attribute("scale");
                if (scaleAttr != null)
                {
                    double.TryParse(scaleAttr.Value, out imageScale);
                }
                
                itemParameters = new ImageItem.Parameters(imageUrl, imageScale);
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
