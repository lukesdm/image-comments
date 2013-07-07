namespace LM.RichComments.EditorComponent
{
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Formatting;
    using LM.RichComments.Domain;

    /// <summary>
    /// Resizes rich comment lines in the editor
    /// </summary>
    internal class MyLineTransformSource : ILineTransformSource
    {
        private RichCommentManager _manager;

        public MyLineTransformSource(RichCommentManager manager)
        {
            _manager = manager;
        }

        LineTransform ILineTransformSource.GetLineTransform(ITextViewLine line, double yPosition, ViewRelativePosition placement)
        {
#pragma warning disable 219
            bool imageOnLine = false; // useful for tracing
#pragma warning restore 219

            int lineNumber = line.Snapshot.GetLineFromPosition(line.Start.Position).LineNumber;
            LineTransform lineTransform; 
            
            // Look up Image for current line and increase line height as necessary
            if (_manager.RichCommentItems.ContainsKey(lineNumber) && RichCommentManager.Enabled)
            {
                double defaultHeight = line.DefaultLineTransform.BottomSpace;
                IRichCommentItem richCommentItem = _manager.RichCommentItems[lineNumber];
                lineTransform = new LineTransform(0, richCommentItem.ItemHeight + defaultHeight, 1.0);

                imageOnLine = true;
            }
            else
            {
                lineTransform = new LineTransform(0, 0, 1.0);
            }
            return lineTransform;
        }
    }
}
