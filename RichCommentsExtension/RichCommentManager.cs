namespace LM.RichComments.EditorComponent
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
    using LM.RichComments.EditorComponent.Utility;
    using LM.RichComments.Utility;
    using LM.RichComments.Domain;

    /// <summary>
    /// Important class. Handles creation of rich comment adornments on appropriate lines and associated error tags.
    /// </summary>
    public class RichCommentManager : ITagger<ErrorTag>, IDisposable
    {
        /// <summary>
        /// Initializes static members of the <see cref="RichCommentManager"/> class
        /// </summary>
        static RichCommentManager()
        {
            Enabled = true;
        }

        /// <summary>
        /// Enables or disables rich comments. TODO: Make enable/disable mechanism better, e.g. specific to each editor instance and persistent
        /// </summary>
        public static void ToggleEnabled()
        {
            Enabled = !Enabled;
            string message = string.Format("Rich comments {0}. Scroll editor window(s) to update.",
                Enabled ? "enabled" : "disabled");
            UIMessage.Show(message);
        }

        public static bool Enabled { get; set; }
        
        // Dictionary to map line number to rich comment item
        internal Dictionary<int, IRichCommentItem> RichCommentItems { get; set; }

        private IAdornmentLayer _layer;
        private IWpfTextView _view;
        private VariableExpander _variableExpander;
        private string _contentTypeName;
        private bool _initialised1 = false;
        private bool _initialised2 = false;
        private List<ITagSpan<ErrorTag>> _errorTags;
        private HashSet<IRichCommentParser> _parsers;
        
        public RichCommentManager(IWpfTextView view)
        {
            _view = view;
            _layer = view.GetAdornmentLayer("RichCommentLayer");
            RichCommentItems = new Dictionary<int, IRichCommentItem>();
            _view.LayoutChanged += layoutChangedHandler;

            _contentTypeName = view.TextBuffer.ContentType.TypeName;
            _view.TextBuffer.ContentTypeChanged += contentTypeChangedHandler;

            _errorTags = new List<ITagSpan<ErrorTag>>();
            _variableExpander = new VariableExpander(_view);

            _parsers = new HashSet<IRichCommentParser>();
            _parsers.Add(new ImageItemParser());
            _parsers.Add(new WebItemParser());
            // Add new parser types here!
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
            if (!RichCommentManager.Enabled)
                return;

            _errorTags.Clear();

            TagsChanged(this, new SnapshotSpanEventArgs(new SnapshotSpan(_view.TextSnapshot, new Span(0, _view.TextSnapshot.Length))));

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

            // Sometimes, on loading a file in an editor view, the line transform gets triggered before the rich comment item adornments 
            // have been added, so the lines don't resize to the rich comment item height. So here's a workaround:
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
        /// Scans text line for matching rich comment signature, then adds new or updates existing rich comment item adornment
        /// </summary>
        private void createVisuals(ITextViewLine line, int lineNumber)
        {
#pragma warning disable 219
            bool imageDetected = false; // useful for tracing
#pragma warning restore 219

            string lineText = line.Extent.GetText();

            IRichCommentItemParameters richCommentItemParameters = null;
            Exception parseException = null;
            bool parsedSuccessfully = false;
            int? xmlPosition = null;
            foreach(IRichCommentParser parser in _parsers)
            {
                parsedSuccessfully = parser.TryParse(_contentTypeName, lineText, out richCommentItemParameters, out parseException, out xmlPosition);
                if (parsedSuccessfully)
                {
                    break;
                }
            }

            // Get coordinates of xml (even if it's not valid)
            SnapshotSpan span = new SnapshotSpan();
            if (xmlPosition != null)
            {
                int start = line.Extent.Start.Position + (int)xmlPosition;
                int end = line.Start + (line.Extent.Length - 1);
                span = new SnapshotSpan(_view.TextSnapshot, Span.FromBounds(start, end));
            }

            // If there was a parsing exception, remove any existing item associated with the line.
            if (parseException != null)
            {
                if (RichCommentItems.ContainsKey(lineNumber))
                {
                    RichCommentItems[lineNumber].RemoveFromAdornmentLayer(_layer);
                    RichCommentItems.Remove(lineNumber);
                }

                _errorTags.Add(new TagSpan<ErrorTag>(span, new ErrorTag("XML parse error", getErrorMessage(parseException))));

                return;
            }

            if (parsedSuccessfully)
            {
                IRichCommentItem richCommentItem = null;
                Exception itemLoadingException = null;
                
                // Check for and update existing rich comment item
                IRichCommentItem existingRichCommentItem = RichCommentItems.ContainsKey(lineNumber) ? RichCommentItems[lineNumber] : null;
                if (existingRichCommentItem != null)
                {
                    existingRichCommentItem.Update(richCommentItemParameters, out itemLoadingException);
                    richCommentItem = existingRichCommentItem;
                }
                else // No existing rich comment item, so create new one
                {
                    richCommentItem = RichCommentItemFactory.Create(richCommentItemParameters, _variableExpander);
                    richCommentItem.Update(richCommentItemParameters, out itemLoadingException);
                    RichCommentItems.Add(lineNumber, richCommentItem);
                }

                // Position rich comment item and add as adornment
                if (itemLoadingException == null)
                {
                    Geometry g = _view.TextViewLines.GetMarkerGeometry(span);
                    
                    if (g == null) // Exceptional case when adornment dimensions are massive (e.g. specifying very large image scale factor)
                    {
                        throw new InvalidOperationException("Couldn't get source code line geometry. Is the loaded rich comment item massive?");
                    }
                    double textLeft = g.Bounds.Left;
                    double textBottom = line.TextBottom;

                    // Add rich comment item to editor view
                    try
                    {
                        richCommentItem.RemoveFromAdornmentLayer(_layer);
                        richCommentItem.AddToAdornmentLayer(_layer, textLeft, textBottom, line.Extent);
                    }
                    catch (Exception ex)
                    {
                        // No expected exceptions, so tell user something is wrong.
                        ExceptionHandler.Notify(ex, true);
                    }
                }
                else
                {
                    if (RichCommentItems.ContainsKey(lineNumber))
                        RichCommentItems.Remove(lineNumber);

                    _errorTags.Add(new TagSpan<ErrorTag>(span, new ErrorTag("Trouble loading rich comment item", getErrorMessage(itemLoadingException))));
                }
                imageDetected = true;
            }
            else
            {
                if (RichCommentItems.ContainsKey(lineNumber))
                    RichCommentItems.Remove(lineNumber);
            }
        }

        private void unsubscribeFromViewerEvents()
        {
            _view.LayoutChanged -= layoutChangedHandler;
            _view.TextBuffer.ContentTypeChanged -= contentTypeChangedHandler;
        }

        private string getErrorMessage(Exception exception, IRichCommentItem richCommentItem = null)
        {
            Trace.WriteLine("Problem parsing comment text or loading rich comment item...\n" + exception);

            if (richCommentItem != null)
            {
                return richCommentItem.MakeFriendlyErrorMessage(exception);
            }
            else if (exception is XmlException)
            {
                return "Problem with comment format: " + exception.Message;
            }
            else
            {
                return exception.Message;
            }
        }

        #region ITagger<ErrorTag> Members

        public IEnumerable<ITagSpan<ErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return _errorTags;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

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
