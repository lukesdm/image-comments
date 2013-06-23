using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace LM.RichComments.Utility
{
    /// <summary>
    /// Provides convenience methods for XML comment parsing
    /// </summary>
    static class ParseHelper
    {
        private static readonly Regex _csharpRichCommentRegex;
        private static readonly Regex _vbRichCommentRegex;
        //private const int xmlTagNameGroupIndex = 1;

        private const string _xmlContentGroupName = "XmlContent";
        private const string _xmlElementNameGroupName = "XmlElementName";

        static ParseHelper()
        {
            string xmlTagPattern = string.Format(@"(?<{0}><(?<{1}>[\d\w]+).*>)", _xmlContentGroupName, _xmlElementNameGroupName);

            string cSharpCommentPattern = @"///.*";
            _csharpRichCommentRegex = new Regex(cSharpCommentPattern + xmlTagPattern, RegexOptions.Compiled);

            string vbCommentPattern = @"'.*";
            _vbRichCommentRegex = new Regex(vbCommentPattern + xmlTagPattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// Tries to determine if there's an XML comment in the provided line text.
        /// </summary>
        /// <param name="contentTypeName">VS content type name, e.g. CSharp, Basic...</param>
        /// <param name="lineText">The line's text.</param>
        /// <param name="matchedXml">Output: the entire matched XML element's contents (or null).</param>
        /// <param name="xmlStartPosition">Output: The position in the line at start of matched comment (or null).</param>
        /// <param name="elementName">Output: The matched XML element name (or null).</param>
        /// <returns>true if an XML comment was found on the line, false otherwise.</returns>
        /// <remarks>This method can be used as a first pass in parsing item parameters, before using a dedicated XML parser.</remarks>
        public static bool MatchXmlComment(string contentTypeName, string lineText, out string matchedXml, out int? xmlStartPosition, out string elementName)
        {
            Match match = null;
            xmlStartPosition = null;
            matchedXml = null;
            elementName = null;

            switch (contentTypeName)
            {
                case "C/C++":
                case "CSharp":
                    match = _csharpRichCommentRegex.Match(lineText);
                    break;
                case "Basic":
                    match = _vbRichCommentRegex.Match(lineText);
                    break;
                //TODO: Add support for more languages
                default:
                    Trace.WriteLine("Unsupported content type: " + contentTypeName);
                    return false;
            }

            if (!match.Success)
            {
                return false;
            }

            matchedXml = match.Groups[_xmlContentGroupName].Value;
            xmlStartPosition = match.Groups[_xmlContentGroupName].Index; // TODO: Check this is the correct index!!!
            elementName = match.Groups[_xmlElementNameGroupName].Value;

            return true;
        }
    }
}
