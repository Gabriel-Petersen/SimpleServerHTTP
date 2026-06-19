using FirstSimpleServerHTTP.Pages;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FirstSimpleServerHTTP.Server
{
    public class HttpServer
    {
        private TcpListener? Controller { get; set; }
        private int Port { get; set; }
        private int RequestAmount;
        private readonly SortedList<string, string> MimeTypes = [];

        public HttpServer(int p = 8080)
        {
            Port = p;
            RequestAmount = 0;
            PopulateMimeTypes();
            try
            {
                Controller = new TcpListener(IPAddress.Parse("127.0.0.1"), Port);
                Controller.Start();
                Console.WriteLine($"Servidor HTTP está rodando na porta {Port}");
                Console.WriteLine($"Para acessar, digite no navegador: http://localhost:{Port}");
                Task httpServerTask = Task.Run(AwaitRequest);
                httpServerTask.GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao iniciar o servidor na porta {Port}:\n{e.Message}");
            }
        }

        private async Task AwaitRequest()
        {
            while (true)
            {
                if (Controller is null) continue;
                Socket connection = await Controller.AcceptSocketAsync();
                Interlocked.Increment(ref RequestAmount);
                Task task = Task.Run(() => ProcessRequest(connection, RequestAmount));
            }
        }

        private void ProcessRequest(Socket connection, int requestNumber)
        {
            Console.WriteLine($"Processando request #{requestNumber}...\n");
            if (connection.Connected)
            {
                byte[] buffer = new byte[1024];
                connection.Receive(buffer, buffer.Length, 0);
                string requestTxt = Encoding.UTF8.GetString(buffer).Replace((char)0, ' ').Trim();

                if (requestTxt.Length > 0)
                {
                    Console.WriteLine($"\n{requestTxt}\n");

                    var data = new DataProcessor(requestTxt);
                    byte[]? content;
                    byte[]? header;
                    FileInfo file = new(GetFilePath(data.Resource));
                    if (file.Exists)
                    {
                        if (MimeTypes.TryGetValue(file.Extension.ToLower(), out string? type))
                        {
                            if (TryGetDynamicPageClass(file, data.ParamPairs, out DynamicPage? page))
                            {
                                page!.HtmlModel = File.ReadAllText(file.FullName);
                                switch (data.Method.ToLower())
                                {
                                    case "get":
                                        content = page.Get(data.ParamPairs);
                                        header = HeaderBuilder(data.HttpVersion, type, "200", content.Length);
                                        break;
                                    case "post":
                                        content = page.Post(data.ParamPairs);
                                        header = HeaderBuilder(data.HttpVersion, type, "200", content.Length);
                                        break;
                                    default:
                                        content = Encoding.UTF8.GetBytes("<h1>Error 405 - Method not allowed</h1>");
                                        header = HeaderBuilder(data.HttpVersion, type, "405", content.Length);
                                        break;
                                }
                            }
                            else
                            {
                                content = File.ReadAllBytes(file.FullName);
                                header = HeaderBuilder(data.HttpVersion, type, "200", content.Length);
                            }
                        }
                        else
                        {
                            content = Encoding.UTF8.GetBytes("<h1>Error 415 - File type not supported</h1>");
                            header = HeaderBuilder(data.HttpVersion, "text/html;charset=utf-8", "415", content.Length);
                        }
                    }
                    else
                    {
                        content = Encoding.UTF8.GetBytes("<h1>Error 404 - Not Found</h1>");
                        header = HeaderBuilder(data.HttpVersion, "text/html;charset=utf-8", "404", content.Length);
                    }


                    int bytesSended = connection.Send(header, header.Length, 0);
                    bytesSended += connection.Send(content, content.Length, 0);

                    connection.Close();
                    Console.WriteLine($"\n{bytesSended} bytes enviados em resposta à requisição {requestNumber}");
                }
            }
            Console.WriteLine($"Request {requestNumber} finalizado");
        }

        private static bool TryGetDynamicPageClass(FileInfo file, SortedList<string, string> parameters, out DynamicPage? page)
        {
            page = null;
            if (file.Extension == ".html")
            {
                string className = file.Name.Replace(file.Extension, "") + "Page";
                Type? tp = Type.GetType(className, false, true);
                if (tp == null) return false;
                if (Activator.CreateInstance(tp) is not DynamicPage dp)
                {
                    throw new Exception($"Instância de {tp} não herda de páginas dinâmicas");
                }

                page = dp;
                return true;
            }
            else
                return false;
        }

        private static byte[] HeaderBuilder(string httpVersion, string mimeType, string httpCode, int byteAmount)
        {
            string endl = Environment.NewLine;
            StringBuilder txt = new();
            txt.Append($"{httpVersion} {httpCode}{endl}");
            txt.Append($"Server: HttpSimpleServer 1.0{endl}");
            txt.Append($"Content-Type: {mimeType}{endl}");
            txt.Append($"Content-Length: {byteAmount}{endl}{endl}");

            return Encoding.UTF8.GetBytes(txt.ToString());
        }

        private void PopulateMimeTypes()
        {
            MimeTypes.Add(".html", "text/html;charset=utf-8");
            MimeTypes.Add(".htm", "text/html;charset=utf-8");
            MimeTypes.Add(".css", "text/css");
            MimeTypes.Add(".js", "application/javascript");
            MimeTypes.Add(".png", "image/png");
            MimeTypes.Add(".jpg", "image/jpeg");
            MimeTypes.Add(".gif", "image/gif");
            MimeTypes.Add(".svg", "image/svg+xml");
            MimeTypes.Add(".webp", "image/webp");
            MimeTypes.Add(".ico", "image/ico");
            MimeTypes.Add(".woff", "font/woff");
            MimeTypes.Add(".woff2", "font/woff2");
        }

        public static string GetFilePath(string file)
        {
            string directory = Path.Combine(Directory.GetCurrentDirectory(), "www");
            return Path.Combine(directory, file.TrimStart('/'));
        }
    }
}