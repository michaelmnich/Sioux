﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ManagingWebSerwer.Conections;
using ManagingWebSerwer.dao;

namespace ManagingWebSerwer.pages
{


    class MainPage : Page
    {
        string PageContent;
        private string _helper = "";
        private string _footer = "";
        private string username;
        private string _header;
        private Dictionary<string, string> _Get_params;
        private Dictionary<string, string> _Set_params;
        private Dictionary<string, Cookie> _Cookie_params;
        private PagesWorker _pagesWorker;

        public MainPage(PagesWorker pagesWorker)
        {
            _Get_params= new Dictionary<string, string>();
            _Set_params = new Dictionary<string, string>();
            _Cookie_params = new Dictionary<string, Cookie>();
            _pagesWorker = pagesWorker;
        }



        public string GetContent( )
        {
            string toRet = "<table><tr><td>aaa</td>bbb<td></td></tr><tr><td>aaa</td>bbb<td></td></tr></table>";
            Cookie cook01 = new Cookie("testCockie", "zielony kapturek");

            _pagesWorker.Context.Response.AppendCookie(cook01);

            string cockie ="";

            if (_Cookie_params.ContainsKey("testCockie"))
            {
                cockie = _Cookie_params["testCockie"].Value;
            }



            if (_Get_params.ContainsKey("info"))
            {
            }



            return toRet;

        }


        public void Set_GET_Params(Dictionary<string, string> Get_params)
        {
            _Get_params = Get_params;
        }

        public void Set_SET_Params(Dictionary<string, string> Set_params)
        {
            _Set_params = Set_params;
        }

        public void Set_Cockie_Params(Dictionary<string, Cookie> Cockie_params)
        {
            _Cookie_params = Cockie_params;
        }


    }


}
