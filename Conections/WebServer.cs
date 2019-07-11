using System;
using System.Net;
using System.Threading;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ManagingWebSerwer.Conections
{
    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
      //  private readonly TcpListener _listenerTCP;
        private readonly Func<HttpListenerContext, string> _responderMethod;

        public WebServer(string[] prefixes, Func<HttpListenerContext, string> method)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");

            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // A responder method is required
            if (method == null)
                throw new ArgumentException("method");

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);
             //   _listener.Prefixes.Add("http://192.168.1.120:8080/index/");
         //   _listenerTCP = new TcpListener(IPAddress.Any, 8080);
            _responderMethod = method;
            _listener.Start();
           // _listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
            //  _listenerTCP.Start();
        }

        public WebServer(Func<HttpListenerContext, string> method, params string[] prefixes)
            : this(prefixes, method) { }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {

                            var ctx = c as HttpListenerContext;
                            try
                            {

                                string rstr = _responderMethod(ctx);
                                //byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                //ctx.Response.ContentLength64 = buf.Length;
                                //ctx.Response.OutputStream.Write(buf, 0, buf.Length);


                                HttpListenerResponse response = ctx.Response;

                                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(rstr);
                                // Get the response stream and write the response to it.
                                response.ContentLength64 = buffer.Length;
                                System.IO.Stream output = response.OutputStream;
                                output.Write(buffer, 0, buffer.Length);
                            }
                            catch { } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}