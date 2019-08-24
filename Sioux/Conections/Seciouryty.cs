using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagingWebSerwer.Conections
{
    public class Seciouryty
    {
        private static Seciouryty instance;
        private string _24H_tocken;
        private  List<string> _techTokens;

        private Seciouryty()
        {
            _techTokens= new List<string>();
            _techTokens.Add("qazxsw123");
            _techTokens.Add("dupa");
            _24H_tocken = "qazTOCKEN";
        }

        public bool TechTokenValid(string s)
        {
            if (_techTokens.Contains(s))
            {
                return true;
            }
            return false;
        }

        public string Get24h_token()
        {
            return _24H_tocken;
        }

        public static Seciouryty GetSecioutytyEngine()
        {
            if (instance == null)
            {
                instance = new Seciouryty();
                return instance;
            }
            return instance;
        }
    }
}
