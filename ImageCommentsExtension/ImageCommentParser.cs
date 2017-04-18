using System.Globalization;

namespace LM.ImageComments.EditorComponent
{
    using System;
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
        private static Regex _xmlImageTagRegex;

        // Initialize regex objects
        static ImageCommentParser()
        {
            string xmlImageTagPattern = @"<image.*>";

            // C/C++/C#
            string cSharpIndent = @"[^\s\/]";
            _csharpIndentRegex = new Regex(cSharpIndent, RegexOptions.Compiled);
            string cSharpCommentPattern = @"//.*";
            _csharpImageCommentRegex = new Regex(cSharpCommentPattern + xmlImageTagPattern, RegexOptions.Compiled);

            // VB
            string vbIndent = @"[^\s\']";
            _vbIndentRegex = new Regex(vbIndent, RegexOptions.Compiled);
            string vbCommentPattern = @"'.*";
            _vbImageCommentRegex = new Regex(vbCommentPattern + xmlImageTagPattern, RegexOptions.Compiled);

            _xmlImageTagRegex = new Regex(xmlImageTagPattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// Tries to match Regex on input text
        /// </summary>
        /// <returns>Position in line at start of matched image comment. -1 if not matched</returns>
        public static int Match(string contentTypeName, string lineText, out string matchedText)
        {
            Match match = null;
            switch (contentTypeName)
            {
                case "C/C++":
                case "CSharp":
                    match = _csharpImageCommentRegex.Match(lineText);
                    break;
                case "Basic":
                    match = _vbImageCommentRegex.Match(lineText);
                    break;
                //TODO: Add support for more languages
                default:
                    Console.WriteLine("Unsupported content type: " + contentTypeName);
                    matchedText = "";
                    return -1;
            }

            matchedText = match.Value;
            if (matchedText == "")
                return -1;
            else
            {
                  switch (contentTypeName)
                {
                    case "C/C++":
                    case "CSharp":
                        return match.Index + _csharpIndentRegex.Match(lineText).Index;
                    case "Basic":
                        return match.Index + _vbIndentRegex.Match(lineText).Index;
                    default:
                        return match.Index;
                }
            }
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
                        double.TryParse(scaleAttr.Value
                            .Replace(".", NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                            .Replace(",", NumberFormatInfo.CurrentInfo.NumberDecimalSeparator), out imageScale);
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
