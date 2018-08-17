using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class AttendanceModel
    {
        string connectionString;

        public AttendanceModel()
        {
            connectionString = Constants.GetConnectionString();
        }
        
    }
}
