namespace LM.ImageComments.EditorComponent
{
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Formatting;

    /// <summary>
    /// Resizes image comment lines in the editor
    /// </summary>
    internal class MyLineTransformSource : ILineTransformSource
    {
        private ImageAdornmentManager _manager;

        public MyLineTransformSource(ImageAdornmentManager manager)
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
            if (_manager.Images.ContainsKey(lineNumber) && ImageAdornmentManager.Enabled)
            {
                double defaultHeight = line.DefaultLineTransform.BottomSpace;
                MyImage image = _manager.Images[lineNumber];
                lineTransform = new LineTransform(0, image.Height + defaultHeight, 1.0);

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
