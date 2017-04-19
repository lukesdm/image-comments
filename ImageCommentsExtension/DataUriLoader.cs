using System;
using System.Text.RegularExpressions;
using System.IO;

namespace LM.ImageComments
{
    class DataUriLoader
    {
        public static Stream Load(Uri dataUri)
        {
            if (dataUri == null)
                throw new ArgumentException();
            if (dataUri.Scheme != "data")
                throw new ArgumentException();

            Regex regex = new Regex(@"data:(?<mime>[\w/]+);(?<encoding>\w+),(?<data>.*)", RegexOptions.Compiled);
            Match match = regex.Match(dataUri.OriginalString);

            string mimeType = match.Groups["mime"].Value;
            string encoding = match.Groups["encoding"].Value;
            string base64Data = match.Groups["data"].Value;

            if (encoding != "base64")
                throw new NotSupportedException();

            byte[] data = Convert.FromBase64String(base64Data);

            return new MemoryStream(data);
        }
    }
}
