using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LM.RichComments.Utility;

namespace LM.RichComments.Domain
{
    interface IRichCommentParser
    {
        /// <summary>
        /// Tries to parse a rich comment item's parameters from a line of text.
        /// </summary>
        /// <param name="contentTypeName">VS file content-type, e.g. CSharp, Basic etc.</param>
        /// <param name="text">Text line to parse.</param>
        /// <param name="richCommentItemParameters">Output: the parsed parameters.</param>
        /// <param name="parseException">Output: An exception encountered while parsing, or null. NOTE: parsing can be unsuccesful without an exception being thrown.</param>
        /// <returns>true if parameters were successfully parsed, false otherwise.</returns>
        /// <remarks>Performance critical: this method will be called for *every line* of text in the Editor view when it becomes visible.</remarks>
        bool TryParse(string contentTypeName, string lineText, out IRichCommentItemParameters richCommentItemParameters, out Exception parseException, out int? xmlStartPosition);

        /// <summary>
        /// The XML tag name to match during parse. Keep lowercase for simplicity.
        /// </summary>
        string ExpectedTagName { get; }

        //VariableExpander VariableExpander { get; }
        UrlProcessor UrlProcessor { get; }
    }
}
