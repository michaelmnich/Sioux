using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ManagingWebSerwer.pages
{
    public abstract class BasePage: Page
    {
        protected Dictionary<string, string> _Get_params;
        protected Dictionary<string, string> _Set_params;
        protected Dictionary<string, Cookie> _Cookie_params;
        protected PagesWorker _pagesWorker;
        public abstract string GetContent();
        public abstract void Set_Cockie_Params(Dictionary<string, Cookie> Cockie_params);
        public abstract void Set_GET_Params(Dictionary<string, string> Get_params);
        public abstract void Set_SET_Params(Dictionary<string, string> Set_params);
        public abstract string GetUri();
        protected void GetCreditials()
        {
            Console.WriteLine("Base Page Creditials");
        }

        public void Init(PagesWorker pagesWorker)
        {
            _Get_params = new Dictionary<string, string>();
            _Set_params = new Dictionary<string, string>();
            _Cookie_params = new Dictionary<string, Cookie>();
            _pagesWorker = pagesWorker;
        }


    }
}
