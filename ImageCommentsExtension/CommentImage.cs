using System.IO;
using System.Windows;

namespace LM.ImageComments.EditorComponent
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class ImageAttributes
    {
        public string Url;
        public double Scale;
        public double Opacity;
        public Color Background;
        private const double Tolerance = 0.001;

        public ImageAttributes()
        {
            Url = "";
            Scale = 1.0;
            Opacity = 1.0;
        }

        public   bool IsEquals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is ImageAttributes other))
                return false;
            return other.Url == Url &&
                   Math.Abs(other.Scale - Scale) < Tolerance &&
                   Math.Abs(other.Opacity - Opacity) < Tolerance &&
                   other.Background.Equals(Background);
        }
    }

    /// <summary>
    /// Sub-class of Image with convenient URL-based Source changing
    /// </summary>
    public class CommentImage : Image
    {

        private readonly VariableExpander _variableExpander;
        private FileSystemWatcher _watcher;

        public ImageAttributes Attributes;

        public CommentImage(VariableExpander variableExpander)
            : base()
        {
            _variableExpander = variableExpander ?? throw new ArgumentNullException(nameof(variableExpander));
            this.VisualBitmapScalingMode = BitmapScalingMode.HighQuality;
        }

        private bool LoadFromUri(string rawUri, string sourceFileDir, Action refreshAction, out string errorString)
        {
            if (string.IsNullOrWhiteSpace(rawUri))
            {
                Source = null;
                errorString = "No image specified";
                return false;
            }

            var expandedUrl = _variableExpander.ProcessText(rawUri);
            if (!File.Exists(expandedUrl)) //TODO: Refactor this eg. post processing step
            {
                // if the file does not exists, but we have an existing "docfx.json", lets try to find file in "$(ProjectDir)\images" directory
                var jsonFile = _variableExpander.ProcessText("$(ProjectDir)\\docfx.json");
                if (File.Exists(jsonFile))
                {
                    // Example: we replace in "..\\images\picture.png" all the ".." with "$ProjectDir" --> "$ProjectDir\\images\\picture.png"
                    expandedUrl = rawUri.Replace("..", "$(ProjectDir)");
                    expandedUrl = _variableExpander.ProcessText(expandedUrl);
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
            else if (canLoadFromWeb)
            {
                expandedUrl = WebLoader.Load(uri);
            }
            else if (!success && !Path.IsPathRooted(expandedUrl) && sourceFileDir != null)
            {
                expandedUrl = Path.Combine(sourceFileDir, expandedUrl);
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

                void Refresh(object sender, FileSystemEventArgs e)
                {
                    try
                    {
                        var enableRaisingEvents = w.EnableRaisingEvents;
                        w.EnableRaisingEvents = false;
                        if (!enableRaisingEvents)
                            return;

                        Attributes.Url = null;
                        refreshAction();
                    }
                    catch
                    {
                        // ignored
                    }
                }

                _watcher.Changed += Refresh;
                _watcher.Renamed += Refresh;
                _watcher.Deleted += Refresh;
                _watcher.EnableRaisingEvents = true;

                errorString = null;
                return true;
            }

            Source = null;
            errorString = $"Could not load image '{uri}' (resolved to '{expandedUrl}')";
            return false;
        }

        /// <summary>
        /// Sets image source and size (by scale factor)
        /// </summary>
        /// <param name="directory">The directory where the text document referencing the image lives</param>
        /// <param name="attribs"> The image attributes used to set the image</param>
        /// <param name="error">Error message if image couldn't be loaded, otherwise null</param>
        /// <param name="refreshAction">The action to be performed if the image was successfully refreshed</param>
        /// <returns>Returns true if image was successfully loaded, otherwise false</returns>
        public bool TrySet(string directory, ImageAttributes attribs, out string error, Action refreshAction)
        {
            // Remove old watcher.
            var watcher = _watcher;
            _watcher = null;
            watcher?.Dispose();
            // ---
            error = null;

            try
            {
                if (LoadFromUri(attribs.Url, directory, refreshAction, out var errorString))
                {
                    Attributes = attribs;
                    ProcessBitmap();
                }

                error = errorString;
            }
            catch (Exception ex)
            {
                Source = null;
                error = ex.Message;
                return false;
            }

            return true;
        }

        private void ProcessBitmap()
        {
            if (Source == null)
                return;

            var scale = Math.Min(Math.Max(Attributes.Scale, 0.01), 100);

            var scaledRect = new Rect(0, 0, (int)(Source.Width * scale), (int)(Source.Height * scale));
            var rect = new Rect(0, 0, (int)Source.Width, (int)Source.Height);
            var visual = new DrawingVisual();
            var context = visual.RenderOpen();
            context.PushTransform(new ScaleTransform(scale, scale));
            context.PushOpacity(Math.Min(Math.Max(Attributes.Opacity, 0),1));
            context.DrawRectangle(new SolidColorBrush(Attributes.Background), null, rect);
            context.DrawImage(Source, rect);
            context.Close();

            var render = new RenderTargetBitmap((int)scaledRect.Width, (int)scaledRect.Height,
                96, 96, PixelFormats.Pbgra32);
            RenderOptions.SetEdgeMode(render, EdgeMode.Aliased);
            RenderOptions.SetBitmapScalingMode(render, BitmapScalingMode.HighQuality);
            
            render.Render(visual);
            Source = BitmapFrame.Create(render);
            Height = Source.Height;
            Width = Source.Width;
        }
    }
}
