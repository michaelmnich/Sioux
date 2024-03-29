﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ManagingWebSerwer.Conections;
using ManagingWebSerwer.dao;

namespace ManagingWebSerwer.pages
{


    /// <summary>
    ///  Fabric for Web pages, Contain TCp ip serwer that can be manage from pages. 
    ///  INFO: Intresting methods TcpSerwer ->  Client_MessageReceived  & SendMessage_to_Client
    ///         
    /// </summary>
    public class PagesWorker
    {
        private Page P_main;
        private Dictionary<string, Page> _pages;
        private Dictionary<string, string> _postParams;
        private Dictionary<string, string> _getParams;
        private Dictionary<string, Cookie> _coockiesParams;
        TcpSerwer TcpSerwer;

        private bool mainPageFound;

        public HttpListenerContext Context { get; private set; }
        public PagesWorker(TcpSerwer tcp)
        {
            TcpSerwer = tcp;
            P_main = new MainPage();
            P_main.Init(this);
            _pages = new Dictionary<string, Page>();
            _pages.Add("/", P_main);
            _pages.Add("/about/", P_main);
            _pages.Add("/about", P_main);

        }

     

        #region TCP ===================================================
        public void SendTcpIp(string url, Page p)
        {
            TcpSerwer.SendMessage_to_Client("message", "ClientName");
        }

        public List<string> GetTcpLogs()
        {
            return TcpSerwer.Logs;
        }
        public string GetClientStatus(string clientName)
        {
            return TcpSerwer.GetStatus(clientName);
        }
        #endregion

        public void AddPage(string url, Page p)
        {
           
            if (url.Equals("main"))
            {
                _pages["/"] = p;
            }
            else
            {
                _pages.Add("/" + url + "/", p);
                _pages.Add("/" + url + "", p);
            }
        }

        public static void ShowRequestProperties2(HttpListenerRequest request)
        {

            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("KeepAlive: {0}", request.KeepAlive);
            Console.WriteLine("Local end point: {0}", request.LocalEndPoint.ToString());
            Console.WriteLine("Remote end point: {0}", request.RemoteEndPoint.ToString());
            Console.WriteLine("Is local? {0}", request.IsLocal);
            Console.WriteLine("HTTP method: {0}", request.HttpMethod);
            Console.WriteLine("Protocol version: {0}", request.ProtocolVersion);
            Console.WriteLine("Is authenticated: {0}", request.IsAuthenticated);
            Console.WriteLine("Is secure: {0}", request.IsSecureConnection);
            Console.WriteLine("Comand: {0}", request.RawUrl);
            Console.WriteLine("------------------------------------------------");

        }

        public string ReturnPageContent(HttpListenerContext ctx)
        {
            Context = ctx;
            ShowRequestProperties2(ctx.Request);
            HttpListenerRequest request = ctx.Request;
            System.IO.Stream body = request.InputStream;
            System.Text.Encoding encoding = request.ContentEncoding;
            System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);
            string s = reader.ReadToEnd();

            _getParams = GetDataParser(request.QueryString);
            _postParams = PostDataParser(s);
            _coockiesParams = CockiesDataParser(request.Cookies);

            string Defoult_page = "404";
            string url_AbsolutePath = request.Url.AbsolutePath;
            if (_pages.ContainsKey(url_AbsolutePath))
            {
                _pages[url_AbsolutePath].Set_GET_Params(_getParams);
                _pages[url_AbsolutePath].Set_SET_Params(_postParams);
                _pages[url_AbsolutePath].Set_Cockie_Params(_coockiesParams);


                Defoult_page = _pages[url_AbsolutePath].GetContent();
            }
            return Defoult_page;
        }

        /// <summary>
        ///    That Mehod Creates Dictionary from all -> Cockies
        /// </summary>
        /// <param name="requestQueryString"></param>
        /// <returns></returns>
        Dictionary<string, Cookie> CockiesDataParser(CookieCollection requestCookies)
        {
            Dictionary<string, Cookie> query_pairs = new Dictionary<string, Cookie>();
            foreach (Cookie cookie in requestCookies)
            {
                query_pairs.Add(cookie.Name, cookie);
            }
            return query_pairs;
        }

        /// <summary>
        ///    That Mehod Creates Dictionary from all -> Get
        /// </summary>
        /// <param name="requestQueryString"></param>
        /// <returns></returns>
        Dictionary<string, string> GetDataParser(NameValueCollection requestQueryString)
        {
            Dictionary<string, string> query_pairs = new Dictionary<string, string>();
            foreach (string q in requestQueryString)
            {
                query_pairs.Add(q, requestQueryString[q]);
            }
            return query_pairs;
        }
        /// <summary>
        ///     That Mehod Creates Dictionary from all -> Set
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Dictionary<string, string> PostDataParser(String url)
        {
            Dictionary<string, string> query_pairs = new Dictionary<string, string>();
            string query = url;
            if (query == "") return query_pairs;
            string[] pairs = query.Split('&');
            foreach (string pair in pairs)
            {
                int idx = pair.IndexOf('=');
                query_pairs.Add(Uri.UnescapeDataString(pair.Substring(0, idx)), Uri.UnescapeDataString(pair.Substring(idx + 1)));
            }
            return query_pairs;
        }






    }


}
