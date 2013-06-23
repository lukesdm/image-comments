using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using LM.RichComments.Utility;

namespace LM.RichComments.Domain
{
    static class RichCommentItemFactory // TODO: This is ugly. Think of a reflection-free design to do this
    {
        public static IRichCommentItem Create(IRichCommentItemParameters parameters, VariableExpander variableExpander)
        {
            Debug.Assert(parameters.RichCommentItemType.GetConstructor(new[] { typeof(VariableExpander) }) != null);
            IRichCommentItem retVal = Activator.CreateInstance(parameters.RichCommentItemType, variableExpander) as IRichCommentItem;
            Debug.Assert(retVal != null);
            return retVal;
        }
    }
}
