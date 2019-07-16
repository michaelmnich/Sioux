using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ManagingWebSerwer.pages
{
    public interface Page
    {
       string GetContent();

        void Set_GET_Params(Dictionary<string,string> Get_params);
        void Set_SET_Params(Dictionary<string,string> Set_params);
        void Set_Cockie_Params(Dictionary<string, Cookie> Cockie_params);
    }
}
