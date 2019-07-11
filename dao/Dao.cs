using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagingWebSerwer.dao
{
    class Dao
    {

        private static Dao instacne;
        private MySqlDao _dao01_test;

        private Dao()
        {
            _dao01_test = new MySqlDao();
        }

        public Idao GetDao()
        {
            return _dao01_test;
        }

        public static Dao Instance()
        {
            if (instacne == null)
            {
                instacne = new Dao();
            }
            return instacne;
        }
    }
}
