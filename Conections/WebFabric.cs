﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ManagingWebSerwer.pages;

namespace ManagingWebSerwer.Conections
{
    class WebFabric
    {
        private WebServer _hhtpSerwer;
        public PagesWorker PagesWoeker { get; private set; }
        private HttpReqestWorker _http_reqestWorker;
        private TcpSerwer My_tcpSerwer;
        public WebFabric()
        {//
            My_tcpSerwer = new TcpSerwer();
            PagesWoeker = new PagesWorker(My_tcpSerwer);

        }

        //_______________________________________________________________________________________________________________________________________________________
        //HTTP SECTION-------------------------------------------------------------------------------------------------------------------------------------------
        //_______________________________________________________________________________________________________________________________________________________

        public void MakeHttpSerwer()
        {
            _http_reqestWorker = new HttpReqestWorker();
            //_hhtpSerwer = new WebServer(SendResponse, "http://192.168.1.120:8080/");
            _hhtpSerwer = new WebServer(SendResponse, "http://localhost:8080/");
          //  _hhtpSerwer = new WebServer(SendResponse, "http://*:8080/");
            _hhtpSerwer.Run();
        }


        public void AddPage(string url, Page p)
        {
            PagesWoeker.AddPage(url, p);
        }

        /// <summary>
        /// Odbierawnie wiadomosci http
        /// </summary>
        /// <param name="request">Parametry sterujace stronami</param>
        /// <returns></returns>
        public string SendResponse(HttpListenerContext ctx)
        {


           // ShowRequestData(ctx);
           // var cleaned_data = System.Web.HttpUtility.UrlDecode(data_text);
            return PagesWoeker.ReturnPageContent(ctx);
        }

        

        public void StopHttpSerwer()
        {
            if(_hhtpSerwer!=null)
            _hhtpSerwer.Stop();
        }

        public void GetMyLocalAdress()
        {
            String strHostName = string.Empty;
            // Getting Ip address of local machine...
            // First get the host name of local machine.
            strHostName = Dns.GetHostName();
            Console.WriteLine("Local Machine's Host Name: " + strHostName);
            // Then using host name, get the IP address list..
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;

            for (int i = 0; i < addr.Length; i++)
            {
                Console.WriteLine("IP Address {0}: {1} ", i, addr[i].ToString());
            }
        }


        //_______________________________________________________________________________________________________________________________________________________
        //TCP IP SECTION-----------------------------------------------------------------------------------------------------------------------------------------
        //_______________________________________________________________________________________________________________________________________________________
     








    }


}
