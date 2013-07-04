using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using LM.RichComments.Utility;

namespace LM.RichComments.Domain
{
    static class RichCommentItemFactory // TODO: This is a little ugly. Think of a reflection-free design to do this
    {
        public static IRichCommentItem Create(IRichCommentItemParameters parameters)
        {
            Debug.Assert(parameters.RichCommentItemType.GetConstructor(new Type[] { }) != null, "Couldn't get constructor for type " + parameters.RichCommentItemType.Name);
            IRichCommentItem retVal = Activator.CreateInstance(parameters.RichCommentItemType) as IRichCommentItem;
            Debug.Assert(retVal != null, "Couldn't cast created instance to IRichCommentItem");
            return retVal;
        }
    }
}
