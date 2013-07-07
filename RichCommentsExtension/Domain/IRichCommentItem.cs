using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;

namespace LM.RichComments.Domain
{
    interface IRichCommentItem
    {
        /// <summary>
        /// Adds this instance as an adornment on the specified layer
        /// </summary>
        /// <param name="adornmentLayer">Adornment layer</param>
        /// <param name="lineTextLeft">Left coordinate of the line's first non-whitespace text (view-space relative, px)</param>
        /// <param name="lineTextBottom">Bottom coordinate of the line's text (view-space relative, px)</param>
        /// <param name="lineExtent">SnapshotSpan of the extent of the line</param>
        void AddToAdornmentLayer(IAdornmentLayer adornmentLayer, double lineTextLeft, double lineTextBottom, SnapshotSpan lineExtent);
        
        void RemoveFromAdornmentLayer(IAdornmentLayer adornmentLayer);
        
        /// <summary>
        /// Determines behavior when rich comment item is no longer in view, e.g when user scrolls the page enough
        /// </summary>
        void Deactivate();
        
        /// <summary>
        /// Generates an error message relevant to the specific media item from an exception
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>A friendly error messag.e</returns>
        string MakeFriendlyErrorMessage(Exception exception);
        
        void Update(IRichCommentItemParameters parameters, out Exception exception);

        /// <summary>
        /// The item's height (px)
        /// </summary>
        double ItemHeight { get; }
    }
}
