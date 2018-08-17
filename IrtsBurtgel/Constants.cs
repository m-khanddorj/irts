using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    class Constants
    {

        public static int WINDOWS_AUTH = 0, SQL_AUTH = 1;
        public static int dbauthtype = WINDOWS_AUTH;
        public static string servername = "DESKTOP-G49ADVI\\SQLEXPRESS";
        public static string dbname = "IrtsBurtgel";
        public static string username = "TESTROLE";
        public static string password = "password";
        public static string ints = "SSPI";

        public static string GetConnectionString()
        {

            if (dbauthtype == SQL_AUTH)
            {
                return "Data Source=" + Constants.servername + ";Initial Catalog=" + Constants.dbname + ";User ID=" + Constants.username + ";Password=" + Constants.password;
            }
            else if (dbauthtype == WINDOWS_AUTH)
            {
                return "Data Source=" + Constants.servername + ";Initial Catalog=" + Constants.dbname + ";Integrated Security="+Constants.ints;
            }
            else
            {
                return "";
            }

        }
    }
}
