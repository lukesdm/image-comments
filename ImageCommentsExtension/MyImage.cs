using System.IO;
using System.Windows;
using LM.ImageComments;
namespace LM.ImageComments.EditorComponent
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Sub-class of Image with convenient URL-based Source changing
    /// </summary>
    internal class MyImage : Image
    {
		private double _scale;
        private VariableExpander _variableExpander;
        private FileSystemWatcher _watcher;

        public string Url { get; private set; }
        public Color BgColor { get; private set; }

        public MyImage(VariableExpander variableExpander)
            : base()
        {
            if (variableExpander == null)
            {
                throw new ArgumentNullException("variableExpander");
            }
            _variableExpander = variableExpander;
        }

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
        /// <returns>Returns true if image was successfully loaded, otherwise false</returns>
        public bool TrySet(string imageUrl, double scale, Color bgColor, out Exception exception, Action refreshAction)
        {
            // Remove old watcher.
            var watcher = _watcher;
            _watcher = null;
            if(watcher!=null)
                watcher.Dispose();
            // ---
            exception = null;
            try
            {
                var expandedUrl = _variableExpander.ProcessText(imageUrl);
                if (File.Exists(expandedUrl))
                {
                    var data = new MemoryStream(File.ReadAllBytes(expandedUrl));
                    Source = BitmapFrame.Create(data);
                    // Create file system watcher to update changed image file.
                    _watcher = new FileSystemWatcher
                    {
                        //NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size,
                        Path = Path.GetDirectoryName(expandedUrl),
                        Filter = Path.GetFileName(expandedUrl)
                    };
                    var w = _watcher;
                    FileSystemEventHandler refresh = delegate
                    {
                        try
                        {
                            var enableRaisingEvents = w.EnableRaisingEvents;
                            w.EnableRaisingEvents = false;
                            if (enableRaisingEvents)
                            {
                                Url = null;
                                refreshAction();
                            }
                        }
                        catch { }
                    };
                    _watcher.Changed += refresh;
                    _watcher.Renamed += (s, a) => refresh(s, a);
                    _watcher.Deleted += refresh;
                    _watcher.EnableRaisingEvents = true;
                }
				else
				{
               		//TODO [!]: Currently, this loading system prevents images from being changed on disk, fix this
					//  e.g. using http://stackoverflow.com/questions/1763608/display-an-image-in-wpf-without-holding-the-file-open
					Uri uri = new Uri(_variableExpander.ProcessText(expandedUrl), UriKind.Absolute);

					if (uri.Scheme == "data")
						Source = BitmapFrame.Create(DataUriLoader.Load(uri));
					else
						Source = BitmapFrame.Create(uri);
				}

                if (bgColor.A != 0)
                {
                    Source = ReplaceTransparency(Source, bgColor);
                    BgColor = bgColor;
                }

                Url = imageUrl;
            }
            catch (Exception ex)
            {
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

        private static BitmapFrame ReplaceTransparency(ImageSource bitmap, Color color)
        {
            var rect = new Rect(0, 0, (int)bitmap.Width, (int)bitmap.Height);
            var visual = new DrawingVisual();
            var context = visual.RenderOpen();
            context.DrawRectangle(new SolidColorBrush(color), null, rect);
            context.DrawImage(bitmap, rect);
            context.Close();

            var render = new RenderTargetBitmap((int)bitmap.Width, (int)bitmap.Height,
                96, 96, PixelFormats.Pbgra32);
            render.Render(visual);
            return BitmapFrame.Create(render);
        }
    }
}
