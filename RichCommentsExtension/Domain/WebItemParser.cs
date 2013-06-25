using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LM.RichComments.Domain
{
    class WebItemParser : IRichCommentParser
    {
        public bool TryParse(string contentTypeName, string text, out IRichCommentItemParameters richCommentItemParameters, out Exception parseException, out int? xmlStartPosition)
        {
            throw new NotImplementedException();
        }

        public string ExpectedTagName
        {
            get { return "webpage"; }
        }
    }
}
