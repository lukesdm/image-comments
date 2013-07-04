using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.IO;

namespace LM.RichComments.Utility
{
    class UrlProcessor
    {
        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <param name="variableExpander">A VariableExpander instance.</param>
        /// <param name="view">The file's IWpfTextView - used for resolving URLs' absolute path. </param>
        public UrlProcessor(IWpfTextView view)
        {
            Debug.Assert(view != null);
            _variableExpander = new VariableExpander(view);
            _view = view;
        }

        private VariableExpander _variableExpander;
        private IWpfTextView _view;
        
        /// <summary>
        /// Processes URL string and creates a Uri instance. Variables are expanded and relative paths 
        /// (which are expected to be relative to the code file) are turned absolute.
        /// </summary>
        /// <param name="urlString"></param>
        /// <returns></returns>
        public Uri MakeUrlFromString(string urlString)
        {
            string processedUrlString = _variableExpander.ProcessText(urlString);
            
            // First, create a Uri that may be relative.
            Uri firstUri = null;
            try
            {
                firstUri = new Uri(processedUrlString, UriKind.RelativeOrAbsolute);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Couldn't create Uri from string '{0}'.", processedUrlString) , ex);
            }

            // If that was a relative URI, make an absolute one based on it. Otherwise return it.
            if (firstUri.IsAbsoluteUri)
            {
                return firstUri;
            }
            else
            {
                string fileDirectory = getFileDirectory();
                Uri absoluteUri;

                try
                {
                    Uri baseUri = new Uri(fileDirectory);
                    absoluteUri = new Uri(baseUri, firstUri);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format("Couldn't create absolute uri from component strings '{0}' and '{1}'", fileDirectory, urlString), ex);
                }
                return absoluteUri;
            }
            
        }

        private string getFileDirectory()
        {
            ITextDocument document;
            _view.TextDataModel.DocumentBuffer.Properties.TryGetProperty(typeof(ITextDocument), out document);
            Debug.Assert(document != null); // TODO: Check handling for unsaved files!
            return Path.GetDirectoryName(document.FilePath) + @"\"; // Trailing slash indicates to URI parser that it's a directory.
        }
    }
}
