using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Formatting;

namespace LM.RichComments.EditorComponent
{
    [Export(typeof(ILineTransformSourceProvider))]
    [ContentType("CSharp"), ContentType("C/C++"), ContentType("Basic")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal class MyLineTransformSourceProvider : ILineTransformSourceProvider
    {
        ILineTransformSource ILineTransformSourceProvider.Create(IWpfTextView view)
        {
            RichCommentItemManager manager = view.Properties.GetOrCreateSingletonProperty<RichCommentItemManager>(() => new RichCommentItemManager(view));
            return new MyLineTransformSource(manager);
        }
    }
}