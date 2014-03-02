using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

#pragma warning disable 0067 // Ignore 'not used' warnings for stubbed members

namespace RichCommentsTests.Stubs
{
    class WpfTextViewStub : IWpfTextView
    {
        public ITextDataModel TextDataModel
        {
            get { return new TextDataModelStub(); }
        }
        
        #region unimplemented
        public System.Windows.Media.Brush Background
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<BackgroundBrushChangedEventArgs> BackgroundBrushChanged;

        public Microsoft.VisualStudio.Text.Formatting.IFormattedLineSource FormattedLineSource
        {
            get { throw new NotImplementedException(); }
        }

        public IAdornmentLayer GetAdornmentLayer(string name)
        {
            throw new NotImplementedException();
        }

        public ISpaceReservationManager GetSpaceReservationManager(string name)
        {
            throw new NotImplementedException();
        }

        public Microsoft.VisualStudio.Text.Formatting.IWpfTextViewLine GetTextViewLineContainingBufferPosition(Microsoft.VisualStudio.Text.SnapshotPoint bufferPosition)
        {
            throw new NotImplementedException();
        }

        public Microsoft.VisualStudio.Text.Formatting.ILineTransformSource LineTransformSource
        {
            get { throw new NotImplementedException(); }
        }

        public IWpfTextViewLineCollection TextViewLines
        {
            get { throw new NotImplementedException(); }
        }

        public System.Windows.FrameworkElement VisualElement
        {
            get { throw new NotImplementedException(); }
        }

        public double ZoomLevel
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<ZoomLevelChangedEventArgs> ZoomLevelChanged;

        public Microsoft.VisualStudio.Text.Projection.IBufferGraph BufferGraph
        {
            get { throw new NotImplementedException(); }
        }

        public ITextCaret Caret
        {
            get { throw new NotImplementedException(); }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public event EventHandler Closed;

        public void DisplayTextLineContainingBufferPosition(Microsoft.VisualStudio.Text.SnapshotPoint bufferPosition, double verticalDistance, ViewRelativePosition relativeTo, double? viewportWidthOverride, double? viewportHeightOverride)
        {
            throw new NotImplementedException();
        }

        public void DisplayTextLineContainingBufferPosition(Microsoft.VisualStudio.Text.SnapshotPoint bufferPosition, double verticalDistance, ViewRelativePosition relativeTo)
        {
            throw new NotImplementedException();
        }

        public Microsoft.VisualStudio.Text.SnapshotSpan GetTextElementSpan(Microsoft.VisualStudio.Text.SnapshotPoint point)
        {
            throw new NotImplementedException();
        }

        Microsoft.VisualStudio.Text.Formatting.ITextViewLine ITextView.GetTextViewLineContainingBufferPosition(Microsoft.VisualStudio.Text.SnapshotPoint bufferPosition)
        {
            throw new NotImplementedException();
        }

        public event EventHandler GotAggregateFocus;

        public bool HasAggregateFocus
        {
            get { throw new NotImplementedException(); }
        }

        public bool InLayout
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsClosed
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsMouseOverViewOrAdornments
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<TextViewLayoutChangedEventArgs> LayoutChanged;

        public double LineHeight
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler LostAggregateFocus;

        public double MaxTextRightCoordinate
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<MouseHoverEventArgs> MouseHover;

        public IEditorOptions Options
        {
            get { throw new NotImplementedException(); }
        }

        public Microsoft.VisualStudio.Text.ITrackingSpan ProvisionalTextHighlight
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void QueueSpaceReservationStackRefresh()
        {
            throw new NotImplementedException();
        }

        public ITextViewRoleSet Roles
        {
            get { throw new NotImplementedException(); }
        }

        public ITextSelection Selection
        {
            get { throw new NotImplementedException(); }
        }

        public Microsoft.VisualStudio.Text.ITextBuffer TextBuffer
        {
            get { throw new NotImplementedException(); }
        }

        public Microsoft.VisualStudio.Text.ITextSnapshot TextSnapshot
        {
            get { throw new NotImplementedException(); }
        }

        ITextViewLineCollection ITextView.TextViewLines
        {
            get { throw new NotImplementedException(); }
        }

        public ITextViewModel TextViewModel
        {
            get { throw new NotImplementedException(); }
        }

        public IViewScroller ViewScroller
        {
            get { throw new NotImplementedException(); }
        }

        public double ViewportBottom
        {
            get { throw new NotImplementedException(); }
        }

        public double ViewportHeight
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler ViewportHeightChanged;

        public double ViewportLeft
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler ViewportLeftChanged;

        public double ViewportRight
        {
            get { throw new NotImplementedException(); }
        }

        public double ViewportTop
        {
            get { throw new NotImplementedException(); }
        }

        public double ViewportWidth
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler ViewportWidthChanged;

        public Microsoft.VisualStudio.Text.ITextSnapshot VisualSnapshot
        {
            get { throw new NotImplementedException(); }
        }

        public Microsoft.VisualStudio.Utilities.PropertyCollection Properties
        {
            get { throw new NotImplementedException(); }
        }

        #endregion unimplemented
    }

    class TextDataModelStub : ITextDataModel
    {
        public ITextBuffer DocumentBuffer
        {
            get { return new TextBufferStub(); }
        }

        #region unimplemented

        public Microsoft.VisualStudio.Utilities.IContentType ContentType
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<TextDataModelContentTypeChangedEventArgs> ContentTypeChanged;

        public ITextBuffer DataBuffer
        {
            get { throw new NotImplementedException(); }
        }

        #endregion unimplemented 
    }

    class TextBufferStub : ITextBuffer
    {
        public Microsoft.VisualStudio.Utilities.PropertyCollection Properties
        {
            get
            {
                var pc = new PropertyCollection();
                pc.AddProperty(typeof(ITextDocument), new TextDocumentStub());
                return pc;
            }
        }
        
        #region unimplemented

        public void ChangeContentType(Microsoft.VisualStudio.Utilities.IContentType newContentType, object editTag)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<TextContentChangedEventArgs> Changed;

        public event EventHandler<TextContentChangedEventArgs> ChangedHighPriority;

        public event EventHandler<TextContentChangedEventArgs> ChangedLowPriority;

        public event EventHandler<TextContentChangingEventArgs> Changing;

        public bool CheckEditAccess()
        {
            throw new NotImplementedException();
        }

        public Microsoft.VisualStudio.Utilities.IContentType ContentType
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<ContentTypeChangedEventArgs> ContentTypeChanged;

        public ITextEdit CreateEdit()
        {
            throw new NotImplementedException();
        }

        public ITextEdit CreateEdit(EditOptions options, int? reiteratedVersionNumber, object editTag)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyRegionEdit CreateReadOnlyRegionEdit()
        {
            throw new NotImplementedException();
        }

        public ITextSnapshot CurrentSnapshot
        {
            get { throw new NotImplementedException(); }
        }

        public ITextSnapshot Delete(Span deleteSpan)
        {
            throw new NotImplementedException();
        }

        public bool EditInProgress
        {
            get { throw new NotImplementedException(); }
        }

        public NormalizedSpanCollection GetReadOnlyExtents(Span span)
        {
            throw new NotImplementedException();
        }

        public ITextSnapshot Insert(int position, string text)
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly(Span span, bool isEdit)
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly(Span span)
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly(int position, bool isEdit)
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly(int position)
        {
            throw new NotImplementedException();
        }

        public event EventHandler PostChanged;

        public event EventHandler<SnapshotSpanEventArgs> ReadOnlyRegionsChanged;

        public ITextSnapshot Replace(Span replaceSpan, string replaceWith)
        {
            throw new NotImplementedException();
        }

        public void TakeThreadOwnership()
        {
            throw new NotImplementedException();
        }

        #endregion unimplemented
    }

    class TextDocumentStub : ITextDocument
    {
        public string FilePath
        {
            get { return Properties.Resources.DummyClassFilePath; }
        }

        #region unimplemented

        public event EventHandler DirtyStateChanged;

        public Encoding Encoding
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<EncodingChangedEventArgs> EncodingChanged;

        public event EventHandler<TextDocumentFileActionEventArgs> FileActionOccurred;

        public bool IsDirty
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReloading
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime LastContentModifiedTime
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime LastSavedTime
        {
            get { throw new NotImplementedException(); }
        }

        public ReloadResult Reload(EditOptions options)
        {
            throw new NotImplementedException();
        }

        public ReloadResult Reload()
        {
            throw new NotImplementedException();
        }

        public void Rename(string newFilePath)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void SaveAs(string filePath, bool overwrite, bool createFolder, IContentType newContentType)
        {
            throw new NotImplementedException();
        }

        public void SaveAs(string filePath, bool overwrite, IContentType newContentType)
        {
            throw new NotImplementedException();
        }

        public void SaveAs(string filePath, bool overwrite, bool createFolder)
        {
            throw new NotImplementedException();
        }

        public void SaveAs(string filePath, bool overwrite)
        {
            throw new NotImplementedException();
        }

        public void SaveCopy(string filePath, bool overwrite, bool createFolder)
        {
            throw new NotImplementedException();
        }

        public void SaveCopy(string filePath, bool overwrite)
        {
            throw new NotImplementedException();
        }

        public void SetEncoderFallback(EncoderFallback fallback)
        {
            throw new NotImplementedException();
        }

        public ITextBuffer TextBuffer
        {
            get { throw new NotImplementedException(); }
        }

        public void UpdateDirtyState(bool isDirty, DateTime lastContentModifiedTime)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion unimplemented
    }
}

#pragma warning restore 0067