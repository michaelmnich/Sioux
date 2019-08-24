using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ManagingWebSerwer.Conections;
using ManagingWebSerwer.dao;
namespace ManagingWebSerwer.pages.Test
{
    class SamplePageToCompile : BasePage
    {

        public SamplePageToCompile()
        {          
        }

        public override string GetContent()
        {
            return "<br><hr><p style='text-align: center;'>Sample compilated page</p>";
        }


        public override void Set_GET_Params(Dictionary<string, string> Get_params)
        {
            _Get_params = Get_params;
        }

        public override void Set_SET_Params(Dictionary<string, string> Set_params)
        {
            _Set_params = Set_params;
        }

        public override void Set_Cockie_Params(Dictionary<string, Cookie> Cockie_params)
        {
            _Cookie_params = Cockie_params;
        }

        public override string GetUri()
        {
            return "sample";
        }
    }


}
