namespace LM.ImageComments.EditorComponent
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Xml;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Formatting;
    using Microsoft.VisualStudio.Text.Tagging;

    /// <summary>
    /// Important class. Handles creation of image adornments on appropriate lines and associated error tags.
    /// </summary>
    internal class ImageAdornmentManager : ITagger<ErrorTag>, IDisposable
    {
        /// <summary>
        /// Initializes static members of the <see cref="ImageAdornmentManager"/> class
        /// </summary>
        static ImageAdornmentManager()
        {
            Enabled = true;
        }

        /// <summary>
        /// Enables or disables image comments. TODO: Make enable/disable mechanism better, e.g. specific to each editor instance and persistent
        /// </summary>
        public static void ToggleEnabled()
        {
            Enabled = !Enabled;
            string message = string.Format("Image comments {0}. Scroll editor window(s) to update.",
                Enabled ? "enabled" : "disabled");
            UIMessage.Show(message);
        }

        public static bool Enabled { get; set; }
        
        // Dictionary to map line number to image
        public Dictionary<int, MyImage> Images { get; set; }

        private IAdornmentLayer _layer;
        private IWpfTextView _view;
        private VariableExpander _variableExpander;
        private string _contentTypeName;
        private bool _initialised1 = false;
        private bool _initialised2 = false;
        private List<ITagSpan<ErrorTag>> _errorTags;
        
        public ImageAdornmentManager(IWpfTextView view)
        {
            _view = view;
            _layer = view.GetAdornmentLayer("ImageCommentLayer");
            Images = new Dictionary<int, MyImage>();
            _view.LayoutChanged += layoutChangedHandler;

            _contentTypeName = view.TextBuffer.ContentType.TypeName;
            _view.TextBuffer.ContentTypeChanged += contentTypeChangedHandler;

            _errorTags = new List<ITagSpan<ErrorTag>>();
            _variableExpander = new VariableExpander(_view);
        }

        private void contentTypeChangedHandler(object sender, ContentTypeChangedEventArgs e)
        {
            _contentTypeName = e.AfterContentType.TypeName;
        }

        /// <summary>
        /// On layout change add the adornment to any reformatted lines
        /// </summary>
        private void layoutChangedHandler(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (!ImageAdornmentManager.Enabled)
                return;

            _errorTags.Clear();

            OnTagsChanged(new SnapshotSpan(_view.TextSnapshot, new Span(0, _view.TextSnapshot.Length)));

            foreach (ITextViewLine line in _view.TextViewLines) // TODO [?]: implement more sensible handling of removing error tags, then use e.NewOrReformattedLines
            {
                int lineNumber = line.Snapshot.GetLineFromPosition(line.Start.Position).LineNumber;
                //TODO [?]: Limit rate of calls to the below when user is editing a line
                try
                {
                    this.createVisuals(line, lineNumber);
                }
                catch (InvalidOperationException ex)
                {
                    ExceptionHandler.Notify(ex, true);
                }
            }

            // Sometimes, on loading a file in an editor view, the line transform gets triggered before the image adornments 
            // have been added, so the lines don't resize to the image height. So here's a workaround:
            // Changing the zoom level triggers the required update.
            // Need to do it twice - once to trigger the event, and again to change it back to the user's expected level.
            if (!_initialised1)
            {
                _view.ZoomLevel++;
                _initialised1 = true;
            }
            if (!_initialised2)
            {
                _view.ZoomLevel--;
                _initialised2 = true;
            }
        }

        /// <summary>
        /// Scans text line for matching image comment signature, then adds new or updates existing image adornment
        /// </summary>
        private void createVisuals(ITextViewLine line, int lineNumber)
        {
#pragma warning disable 219
            bool imageDetected = false; // useful for tracing
#pragma warning restore 219

            string lineText = line.Extent.GetText();
            string imageUrl;
            double scale;
            string matchedText;
            int matchIndex = ImageCommentParser.Match(_contentTypeName, lineText, out matchedText);
            if (matchIndex >= 0)
            {
                // Get coordinates of text
                int start = line.Extent.Start.Position + matchIndex;
                int end = line.Start + (line.Extent.Length - 1);
                var span = new SnapshotSpan(_view.TextSnapshot, Span.FromBounds(start, end));

                Exception xmlParseException;
                ImageCommentParser.TryParse(matchedText, out imageUrl, out scale, out xmlParseException);

                if (xmlParseException != null)
                {
                    if (Images.ContainsKey(lineNumber))
                    {
                        _layer.RemoveAdornment(Images[lineNumber]);
                        Images.Remove(lineNumber);
                    }

                    _errorTags.Add(new TagSpan<ErrorTag>(span, new ErrorTag("XML parse error", getErrorMessage(xmlParseException))));

                    return;
                }

                MyImage image = null;
                Exception imageLoadingException = null;
                
                // Check for and update existing image
                MyImage existingImage = Images.ContainsKey(lineNumber) ? Images[lineNumber] : null;
                if (existingImage != null)
                {
                    image = existingImage;
                    if (existingImage.Url == imageUrl && existingImage.Scale != scale) // URL same but scale changed
                    {
                        existingImage.Scale = scale;
                    }
                    else if (existingImage.Url != imageUrl) // URL different, so set new source
                    {
                        existingImage.TrySet(imageUrl, scale, out imageLoadingException);
                    }
                }
                else // No existing image, so create new one
                {
                    image = new MyImage(_variableExpander);
                    image.TrySet(imageUrl, scale, out imageLoadingException);
                    Images.Add(lineNumber, image);
                }

                // Position image and add as adornment
                if (imageLoadingException == null)
                {
                    
                    Geometry g = _view.TextViewLines.GetMarkerGeometry(span);
                    if (g == null) // Exceptional case when image dimensions are massive (e.g. specifying very large scale factor)
                    {
                        throw new InvalidOperationException("Couldn't get source code line geometry. Is the loaded image massive?");
                    }
                    double textLeft = g.Bounds.Left;
                    double textBottom = line.TextBottom;
                    Canvas.SetLeft(image, textLeft);
                    Canvas.SetTop(image, textBottom);

                    // Add image to editor view
                    try
                    {
                        _layer.RemoveAdornment(image);
                        _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, line.Extent, null, image, null);
                    }
                    catch (Exception ex)
                    {
                        // No expected exceptions, so tell user something is wrong.
                        ExceptionHandler.Notify(ex, true);
                    }
                }
                else
                {
                    if (Images.ContainsKey(lineNumber))
                        Images.Remove(lineNumber);

                    _errorTags.Add(new TagSpan<ErrorTag>(span, new ErrorTag("Trouble loading image", getErrorMessage(imageLoadingException))));
                }
                imageDetected = true;
            }
            else
            {
                if (Images.ContainsKey(lineNumber))
                    Images.Remove(lineNumber);
            }
        }

        private string getErrorMessage(Exception exception)
        {
            Trace.WriteLine("Problem parsing comment text or loading image...\n" + exception);

            string message;
            if (exception is XmlException)
                message = "Problem with comment format: " + exception.Message;
            else if (exception is NotSupportedException)
                message = exception.Message + "\nThis problem could be caused by a corrupt, invalid or unsupported image file.";
            else
                message = exception.Message;
            return message;
        }

        private void unsubscribeFromViewerEvents()
        {
            _view.LayoutChanged -= layoutChangedHandler;
            _view.TextBuffer.ContentTypeChanged -= contentTypeChangedHandler;
        }

        #region ITagger<ErrorTag> Members

        public IEnumerable<ITagSpan<ErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return _errorTags;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
        protected void OnTagsChanged(SnapshotSpan span)
        {
            EventHandler<SnapshotSpanEventArgs> tagsChanged = TagsChanged;
            if (tagsChanged != null)
                tagsChanged(this, new SnapshotSpanEventArgs(span));
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// This is called by the TextView when closing. Events are unsubscribed here.
        /// </summary>
        /// <remarks>
        /// It's actually called twice - once by the IPropertyOwner instance, and again by the ITagger instance
        /// </remarks>
        public void Dispose()
        {
            unsubscribeFromViewerEvents();
        }

        #endregion
    }
}
