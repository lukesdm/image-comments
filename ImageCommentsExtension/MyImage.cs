namespace LM.ImageComments.EditorComponent
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Convenience sub-class of Image
    /// </summary>
    internal class MyImage : Image
    {
        public string Url { get; private set; }
        
        /// <summary>
        /// Scale image if value is greater than 0, otherwise use source dimensions
        /// </summary>
        public double Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                if (this.Source != null)
                {
                    if (value > 0)
                    {
                        this.Width = this.Source.Width * value;
                        this.Height = this.Source.Height * value;
                    }
                    else
                    {
                        this.Width = this.Source.Width;
                        this.Height = this.Source.Height;
                    }
                }
            }
        }

        /// <summary>
        /// Sets image source and size (by scale factor)
        /// </summary>
        /// <param name="scale">If > 0, scales the image by the specified amount, otherwise uses source image dimensions</param>
        /// <param name="exception">Is set to the Exception instance if image couldn't be loaded, otherwise null</param>
        /// <returns>Returns true if image was succesfully loaded, otherwise false</returns>
        public bool TrySet(string imageUrl, double scale, out Exception exception)
        {
            exception = null;
            try
            {
                this.Source = BitmapFrame.Create(new Uri(imageUrl));
                this.Url = imageUrl;
            }
            catch (Exception ex)
            {
                if (ExceptionHandler.IsCritical(ex)) 
                    throw;

                exception = ex;
                return false;
            }
            this.Scale = scale;
            return true;
        }

        public override string ToString()
        {
            return Url;
        }

        private  double _scale;
    }
}
