using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace LM.RichComments.Domain
{
    class WebItem : Control, IRichCommentItem
    {
        public WebItem() : base()
        {
            _webBrowser = new WebBrowser();
            
            //...

            throw new NotImplementedException();
        }

        private WebBrowser _webBrowser;

        public void AddToAdornmentLayer(Microsoft.VisualStudio.Text.Editor.IAdornmentLayer adornmentLayer, double lineTextLeft, double lineTextBottom, Microsoft.VisualStudio.Text.SnapshotSpan lineExtent)
        {
            throw new NotImplementedException();
        }

        public void RemoveFromAdornmentLayer(Microsoft.VisualStudio.Text.Editor.IAdornmentLayer adornmentLayer)
        {
            throw new NotImplementedException();
        }

        public void Deactivate()
        {
            throw new NotImplementedException();
        }

        public string MakeFriendlyErrorMessage(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void Update(IRichCommentItemParameters parameters, out Exception exception)
        {
            throw new NotImplementedException();
        }

        public double Height
        {
            get { throw new NotImplementedException(); }
        }

        class Parameters : IRichCommentItemParameters
        {
            public Type RichCommentItemType
            {
                get { return typeof(WebItem); }
            }
        }

    }
}
