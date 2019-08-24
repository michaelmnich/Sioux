using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
namespace ManagingWebSerwer.Conections
{
    class HttpReqestWorker
    {

        public HttpReqestWorker()
        {
            
        }

        public void test()
        {
            using (var wb = new WebClient())
            {
                var response = wb.DownloadString("https://api.ipify.org?format=json");
                Console.WriteLine(response);
            }
        }



    }
}
