namespace LM.ImageComments.EditorComponent
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text;
    using System.Diagnostics;

    [Export(typeof(IViewTaggerProvider))]
    [ContentType("CSharp"), ContentType("C/C++"), ContentType("Basic"), ContentType("Python"), ContentType("F#")]
    [TagType(typeof(ErrorTag))]
    internal class ErrorTaggerProvider : IViewTaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView == null)
            {
                return null;
            }

            if (textView.TextBuffer != buffer)
            {
                return null;
            }

            Trace.Assert(textView is IWpfTextView);

            ImageAdornmentManager imageAdornmentManager = textView.Properties.GetOrCreateSingletonProperty<ImageAdornmentManager>(() => new ImageAdornmentManager((IWpfTextView)textView));
            return imageAdornmentManager as ITagger<T>;
        }
    }
}
