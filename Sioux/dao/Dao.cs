using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagingWebSerwer.dao
{

    /// <summary>
    ///  Dao singleton/Fabric If you whant to add new Dao impelment Idao Interface and use it heare. 
    /// </summary>
    class Dao
    {

        private static Dao instacne;
       

        private Dao()
        {
          //  _dao01_test = new MySqlDao();
        }

        /// <summary>
        ///  Dao return method. 
        /// </summary>
        /// <returns></returns>
        public Idao GetDao()
        {
            return null;
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
