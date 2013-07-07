namespace LM.RichComments.Domain
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using System.Diagnostics;
    using System.Xml;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text;
    using LM.RichComments;
    using LM.RichComments.Utility;

    /// <summary>
    /// Sub-class of Image with convenient URL-based Source changing
    /// </summary>
    internal class ImageItem : Image, IRichCommentItem
    {
        private Parameters _parameters;

        public ImageItem()
            : base()
        {
            _parameters = new Parameters(null);
        }
        
        //public string Url { get; private set; }
        
        /// <summary>
        /// Scale image if value is greater than 0, otherwise use source dimensions
        /// </summary>
        //public double Scale
        //{
        //    get { return _scale; }
        //    set
        //    {
        //        _scale = value;
        //        if (this.Source != null)
        //        {
        //            if (value > 0)
        //            {
        //                this.Width = this.Source.Width * value;
        //                this.Height = this.Source.Height * value;
        //            }
        //            else
        //            {
        //                this.Width = this.Source.Width;
        //                this.Height = this.Source.Height;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Sets image source and size (by scale factor)
        /// </summary>
        /// <param name="scale">If > 0, scales the image by the specified amount, otherwise uses source image dimensions</param>
        /// <param name="exception">Is set to the Exception instance if image couldn't be loaded, otherwise null</param>
        /// <returns>Returns true if image was succesfully loaded, otherwise false</returns>
        public bool TryUpdateSource(Uri imageUrl, out Exception exception)
        {
            exception = null;
            try
            {
                //TODO [!]: Currently, this loading system prevents images from being changed on disk, fix this
                //  e.g. using http://stackoverflow.com/questions/1763608/display-an-image-in-wpf-without-holding-the-file-open
                this.Source = BitmapFrame.Create(imageUrl);
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
            
            return true;
        }

        private void resize()
        {
            if (this.Source == null)
            {
                return;
            }

            this.Width = this.Source.Width * _parameters.Scale;
            this.Height = this.Source.Height * _parameters.Scale;
        }

        //public override string ToString()
        //{
        //    return Url;
        //}

        //private  double _scale;

        #region IRichCommentItem members

        public void AddToAdornmentLayer(Microsoft.VisualStudio.Text.Editor.IAdornmentLayer adornmentLayer, double lineTextLeft, double lineTextBottom, SnapshotSpan lineExtent)
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

        public void Update(IRichCommentItemParameters parameters, out Exception loadingException)
        {
            var newParameters = parameters as Parameters;
            Debug.Assert(newParameters != null);
            loadingException = null;

            if (_parameters.Url != newParameters.Url) // URL different, so update image source
            {
                this.TryUpdateSource(newParameters.Url, out loadingException);
            }

            _parameters = newParameters;

            resize();
        }

        public string MakeFriendlyErrorMessage(Exception exception)
        {
            string message;
            if (exception is NotSupportedException)
                message = exception.Message + "\nThis problem could be caused by a corrupt, invalid or unsupported image file.";
            else
                message = exception.Message;
            return message;
        }

        public double ItemHeight 
        {
            get
            {
                if (this.Source == null)
                {
                    return 0;
                }

                return this.Source.Height * _parameters.Scale;
            }
        }
        
        #endregion

        internal class Parameters : IRichCommentItemParameters
        {
            public Parameters(Uri url, double scale = 1.0)
            {
                this.Url = url;
                this.Scale = scale;
            }

            public Uri Url { get; private set; }
            public double Scale { get; set; }

            public Type RichCommentItemType
            {
                get { return typeof(ImageItem); }
            }
        }
    }
}
