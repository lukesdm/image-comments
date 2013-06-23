using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LM.RichComments.Domain
{
    /// <summary>
    /// Represents a manager for a specific type of rich comment item
    /// </summary>
    interface IRichCommentItemManager
    {
        IRichCommentParser CommentParser { get; }
    }
}
