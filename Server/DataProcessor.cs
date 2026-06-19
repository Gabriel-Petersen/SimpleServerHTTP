using System.Text;
using System.Web;

namespace FirstSimpleServerHTTP.Server
{
    internal class DataProcessor
    {
        private readonly string[] lines;

        public string Method { get; set; }
        public string Resource { get; set; }
        public string HttpVersion { get; set; }
        public string HostName { get; set; }
        public readonly SortedList<string, string> ParamPairs = [];

        public DataProcessor(string requestTxt)
        {
            lines = requestTxt.Split("\r\n");
            int i = lines[0].IndexOf(' ');
            int j = lines[0].LastIndexOf(' ');

            Method = lines[0][..i];
            string fullResource = lines[0].Substring(i + 1, j - i - 1);
            HttpVersion = lines[0][(j + 1)..];
            HostName = lines[1][(lines[1].IndexOf(' ') + 1)..];

            if (fullResource == "/") fullResource = "/index.html";
            Resource = fullResource.Split('?')[0];
            ProcessParams(fullResource.Contains('?') ? fullResource.Split('?')[1] : string.Empty);

            string postData = requestTxt.Contains("\r\n\r\n") ? requestTxt.Split("\r\n\r\n")[1] : string.Empty;
            ProcessParams(HttpUtility.UrlDecode(postData, Encoding.UTF8));
        }

        private void ProcessParams(string txt)
        {
            if (!string.IsNullOrEmpty(txt.Trim()))
            {
                string[] pairs = txt.Split('&');
                foreach (var pair in pairs)
                {
                    var x = pair.Split('=');
                    if (x.Length == 2)
                        ParamPairs.Add(x[0].ToLower(), x[1]);
                }
            }
        }
    }
}