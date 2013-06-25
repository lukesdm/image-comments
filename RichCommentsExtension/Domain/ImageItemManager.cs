using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LM.RichComments.Utility;
using LM.RichComments.Domain;

namespace LM.RichComments.Domain
{
    // TODO: Get rid of this if it's not needed - might get away with using just a collection of parser instances
    class ImageItemManager : IRichCommentItemManager
    {
        public ImageItemManager()
        {
        //    this.CommentParser = new ImageItemParser();
            throw new NotImplementedException();
        }

        public IRichCommentParser CommentParser { get; private set; }

        public VariableExpander VariableExpander { get; private set; }
    }
}
