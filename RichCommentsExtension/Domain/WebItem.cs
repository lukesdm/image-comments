using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text.Editor;
using System.Diagnostics;
using System.Security;
using LM.RichComments.Utility;
using System.Runtime.InteropServices;
using Awesomium.Windows.Controls;
using System.IO;

namespace LM.RichComments.Domain
{
    class WebItem : ContentControl, IRichCommentItem
    {
        public WebItem() : base()
        {
            _parameters = new Parameters(0, 0, new Uri("about:blank", UriKind.RelativeOrAbsolute)); 
            _webBrowser = new WebControl();
            this.Content = _webBrowser;
        }

        //private WebBrowser _webBrowser;
        private WebControl _webBrowser;
        private Parameters _parameters;

        public void AddToAdornmentLayer(Microsoft.VisualStudio.Text.Editor.IAdornmentLayer adornmentLayer, double lineTextLeft, double lineTextBottom, Microsoft.VisualStudio.Text.SnapshotSpan lineExtent)
        {
            // TODO: This code will probably be shared for all richcommentitem types... put in abstract class.
            Canvas.SetLeft(this, lineTextLeft);
            Canvas.SetTop(this, lineTextBottom);
            adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, lineExtent, null, this, null);
        }

        public void RemoveFromAdornmentLayer(Microsoft.VisualStudio.Text.Editor.IAdornmentLayer adornmentLayer)
        {
            adornmentLayer.RemoveAdornment(this);
        }

        public void Deactivate()
        {
            throw new NotImplementedException();
        }

        public string MakeFriendlyErrorMessage(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void Update(IRichCommentItemParameters parameters, out Exception itemUpdateException)
        {
            itemUpdateException = null;
            _parameters = parameters as Parameters;
            Debug.Assert(_parameters != null);

            try
            {
                _webBrowser.Source = _parameters.Url;
                _webBrowser.Width = _parameters.Width;
                _webBrowser.Height = _parameters.Height;
            }
            catch (SecurityException ex) // Thrown when URL is in a forbidden location
            {
                itemUpdateException = ex;
            }
            catch (InvalidOperationException ex) // Thrown when there was some internal problem. TODO: Better handling for this
            {
                itemUpdateException = ex;
            }
            catch (ArgumentException ex) // Thrown when the URI is relative
            {
                itemUpdateException = ex;
            }
            catch (COMException ex) // Thrown when there was some internal problem. TODO: Better handling for this
            {
                itemUpdateException = ex;
            }
        }


        public double ItemHeight
        {
            get { return _parameters.Height; }
        }

        public class Parameters : IRichCommentItemParameters
        {
            public Parameters(double width, double height, Uri url)
            {
                this.Width = width;
                this.Height = height;
                this.Url = url;
            }
            public double Height { get; private set; }
            public double Width { get; private set; }
            public Uri Url { get; private set; }
            
            public Type RichCommentItemType
            {
                get { return typeof(WebItem); }
            }
        }

        // TODO: Refactor - put this in a more suitable place.
        #region load dependencies
        
        static WebItem()
        {
            loadDependencies();
        }

        private static void loadDependencies()
        {
            string[] dependencyFilenames = { 
                "Awesomium.Core.dll", 
                "Awesomium.Windows.Controls.dll" };

            string extensionFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            foreach (string dependencyFilename in dependencyFilenames)
            {
                System.Reflection.Assembly.LoadFrom(string.Format(@"{0}\{1}", extensionFolder, dependencyFilename));
            }

        }

        #endregion
    }
}