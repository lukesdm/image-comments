using System.Globalization;

namespace LM.ImageComments.EditorComponent
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Windows.Media;
    using System.Xml;
    using System.Xml.Linq;

    // TODO [?]: Could make this a non-static class and use instances, but ensure a new instance is created when content type of a view is changed.
    internal static class ImageCommentParser
    {


        private static Regex _csharpImageCommentRegex;
        private static Regex _csharpIndentRegex;
        private static Regex _vbImageCommentRegex;
        private static Regex _vbIndentRegex;
		private static Regex _pythonImageCommentRegex;
		private static Regex _pythonIndentRegex;
        private static Regex _xmlImageTagRegex;

        // Initialize regex objects
        static ImageCommentParser()
        {
            const string xmlImageTagPattern = @"<image.*>";

            // C/C++/C#
            const string cSharpIndent = @"//\s+";
            _csharpIndentRegex = new Regex(cSharpIndent, RegexOptions.Compiled);
            const string cSharpCommentPattern = @"//.*";
            _csharpImageCommentRegex = new Regex(cSharpCommentPattern + xmlImageTagPattern, RegexOptions.Compiled);

            // VB
            const string vbIndent = @"'\s+";
            _vbIndentRegex = new Regex(vbIndent, RegexOptions.Compiled);
            const string vbCommentPattern = @"'.*";
            _vbImageCommentRegex = new Regex(vbCommentPattern + xmlImageTagPattern, RegexOptions.Compiled);

            //Python
			const string pythonIndent = @"#\s+";
            _pythonIndentRegex = new Regex(pythonIndent, RegexOptions.Compiled);
            const string pythonCommentPattern = @"#.*";
            _pythonImageCommentRegex = new Regex(pythonCommentPattern + xmlImageTagPattern, RegexOptions.Compiled);

            _xmlImageTagRegex = new Regex(xmlImageTagPattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// Tries to match Regex on input text
        /// </summary>
        /// <returns>Position in line at start of matched image comment. -1 if not matched</returns>
        public static int Match(string contentTypeName, string lineText, out string matchedText)
        {
            Match commentMatch;
            Match indentMatch;
            switch (contentTypeName)
            {
                case "C/C++":
                case "CSharp":
				case "F#":
                    commentMatch = _csharpImageCommentRegex.Match(lineText);
                    indentMatch = _csharpIndentRegex.Match(lineText);
                    break;
                case "Basic":
                    commentMatch = _vbImageCommentRegex.Match(lineText);
                    indentMatch = _vbIndentRegex.Match(lineText);
                    break;
                case "Python":
                    commentMatch = _pythonImageCommentRegex.Match(lineText);
                    indentMatch = _pythonIndentRegex.Match(lineText);
                    break;
                //TODO: Add support for more languages
                default:
                    //Console.WriteLine("Unsupported content type: " + contentTypeName);
                    matchedText = "";
                    return -1;
            }

            matchedText = commentMatch.Value;
            if (matchedText == "")
                return -1;
            
            return indentMatch.Index + indentMatch.Length;
        }
        /// <summary>
        /// Looks for well formed image comment in line of text and tries to parse parameters
        /// </summary>
        /// <param name="matchedText">Input: Line of text in editor window</param>
        /// <param name="imageUrl">Output: URL of image</param>
        /// <param name="imageScale">Output: Scale factor of image </param>
        /// <param name="ex">Instance of any exception generated. Null if function finished succesfully</param>
        /// <returns>Returns true if successful, otherwise false</returns>
        public static bool TryParse(string matchedText, out string imageUrl, out double imageScale, ref Color bgColor, out Exception exception)
        {
            exception = null;
            imageUrl = "";
            imageScale = 0; // See MyImage.cs for explanation of default value here
            
            // Try parse text
            if (matchedText != "")
            {
                string tagText = _xmlImageTagRegex.Match(matchedText).Value;
                try
                {
                    XElement imgEl = XElement.Parse(tagText);
                    XAttribute srcAttr = imgEl.Attribute("url");
                    if (srcAttr == null)
                    {
                        exception = new XmlException("url attribute not specified.");
                        return false;
                    }
                    imageUrl = srcAttr.Value;
                    XAttribute scaleAttr = imgEl.Attribute("scale");
                    if (scaleAttr != null)
                    {
                        double.TryParse(scaleAttr.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out imageScale);
                    }

                    XAttribute bgColorAttr = imgEl.Attribute("bgcolor");
                    if (bgColorAttr != null)
                    {
                        UInt32 color;
                        if( UInt32.TryParse(bgColorAttr.Value.Replace("#", "").Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out color) )
                        {
                            bgColor.A = 255;
                            bgColor.B = (byte)color;
                            bgColor.G = (byte)(color>>8);
                            bgColor.R = (byte)(color>>16);
                        }
                    }
                    else
                    {
                        bgColor.A = 0;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    exception = ex;
                    return false;
                }
            }
            else
            {
                exception = new XmlException("<image... /> tag not in correct format.");
                return false;
            }
        }
    }
}
