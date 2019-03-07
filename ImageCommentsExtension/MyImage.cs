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
        /// <param name="directory">The directory where the text document referencing the image lives</param>
        /// <param name="imageUrl">The url to the image</param>
        /// <param name="scale">If > 0, scales the image by the specified amount, otherwise uses source image dimensions</param>
        /// <param name="bgColor">The background color used for transparent images</param>
        /// <param name="error">Error message if image couldn't be loaded, otherwise null</param>
        /// <param name="refreshAction">The action to be performed if the image was successfully refreshed</param>
        /// <returns>Returns true if image was successfully loaded, otherwise false</returns>
        public bool TrySet(string directory, string imageUrl, double scale, Color bgColor, out String error, Action refreshAction)
        {
            // Remove old watcher.
            var watcher = _watcher;
            _watcher = null;
            watcher?.Dispose();
            // ---
            error = null;

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                Source = null;
                return false;
            }

            try
            {
                var expandedUrl = _variableExpander.ProcessText(imageUrl);
                if (!File.Exists(expandedUrl)) //TODO: Refactor this eg. post processing step
                {
                  // if the file does not exists, but we have an existing "docfx.json", lets try to find file in "$(ProjectDir)\images" directory
                  var jsonFile = _variableExpander.ProcessText("$(ProjectDir)\\docfx.json");
                  if (File.Exists(jsonFile))
                  {
                    // Example: we replace in "..\\images\picture.png" all the ".." with "$ProjectDir" --> "$ProjectDir\\images\\picture.png"
                    imageUrl = imageUrl.Replace("..", "$(ProjectDir)");
                    expandedUrl = _variableExpander.ProcessText(imageUrl);
                  }
                }

                var success = Uri.TryCreate(_variableExpander.ProcessText(expandedUrl), UriKind.Absolute, out var uri);
                var canLoadData = success && DataUriLoader.CanLoad(uri);
                var canLoadFromWeb = success && WebLoader.CanLoad(uri);
                if (canLoadData)
                {
                    //TODO [!]: Currently, this loading system prevents images from being changed on disk, fix this
                    //  e.g. using http://stackoverflow.com/questions/1763608/display-an-image-in-wpf-without-holding-the-file-open
                    Source = BitmapFrame.Create(DataUriLoader.Load(uri));
                }
                else if(canLoadFromWeb)
                {
                    expandedUrl = WebLoader.Load(uri);
                }
                else if (!success && !Path.IsPathRooted(expandedUrl) && directory != null)
                {
                    expandedUrl = Path.Combine(directory, expandedUrl);
                    expandedUrl = Path.GetFullPath((new Uri(expandedUrl)).LocalPath);
                }

                if (!canLoadData && File.Exists(expandedUrl))
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
                    Source = null;
                    error = $"Could not load image '{expandedUrl}'";
                    return false;
                }

                if (Source != null)
                {
                    if (bgColor.A != 0)
                    {
                        Source = ReplaceTransparency(Source, bgColor);
                        BgColor = bgColor;
                    }
                }

                Url = imageUrl;
            }
            catch (Exception ex)
            {
                Source = null;
                error = ex.Message;
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
