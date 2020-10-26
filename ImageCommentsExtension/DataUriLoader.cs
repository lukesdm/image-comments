using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Net;

namespace LM.ImageComments
{
    public class WebLoader
    {
        public static bool CanLoad(Uri dataUri)
        {
            return dataUri.Scheme.StartsWith("http");
        }

        public static string GetTempPath(Uri dataUri)
        {
            if (!dataUri.Scheme.StartsWith("http"))
                return null;

            var temp = Path.GetTempPath();
            var invalids = Path.GetInvalidFileNameChars();
            var localPath = new string(dataUri.LocalPath.Substring(1).Select(c => invalids.Contains(c) ? '-' : c).ToArray());
            return Path.Combine(temp, localPath);
        }

        public static string Load(Uri dataUri)
        {
            var tempPath = GetTempPath(dataUri);

            if (tempPath == null || File.Exists(tempPath))
                return tempPath;

            new WebClient().DownloadFile(dataUri, tempPath);

            return tempPath;
        }
    }

    public class DataUriLoader
    {
        public static bool CanLoad(Uri dataUri)
        {
            return dataUri.Scheme == "data";
        }

        public static Stream Load(Uri dataUri)
        {
            var regex = new Regex(@"data:(?<mime>[\w/]+);(?<encoding>\w+),(?<data>.*)", RegexOptions.Compiled);
            var match = regex.Match(dataUri.OriginalString);

            var mimeType = match.Groups["mime"].Value;
            var encoding = match.Groups["encoding"].Value;
            var base64Data = match.Groups["data"].Value;

            if (encoding != "base64")
                throw new NotSupportedException();

            var data = Convert.FromBase64String(base64Data);

            return new MemoryStream(data);
        }
    }
}
