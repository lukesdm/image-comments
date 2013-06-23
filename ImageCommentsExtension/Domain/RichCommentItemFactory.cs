using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LM.RichComments.Domain
{
    static class RichCommentItemFactory // TODO: Think of a reflection-free design to do this
    {
        public static IRichCommentItem Create(IRichCommentItemParameters parameters)
        {
            IRichCommentItem retVal = Activator.CreateInstance(parameters.RichCommentItemType) as IRichCommentItem;
            Debug.Assert(retVal != null);
            return retVal;
        }
    }
}
