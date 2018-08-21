using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class Status : Entity
    {
        public override string TableName => "status";
        public override string IDName => "status_id";

        public int id;
        public string name;
        public bool isPrimary;

        public Status()
        {
            id = -1;
            name = "";
            isPrimary = false;
        }

        public override List<Object[]> ToKVStringList()
        {
            List<Object[]> list = new List<Object[]>();
            if (id != -1)
            {
                list.Add(new Object[] { "status_id", id });
            }
            list.Add(new Object[] { "name", name });
            list.Add(new Object[] { "is_primary", isPrimary });
            return list;
        }

        public override Entity GetObj(SqlDataReader reader)
        {
            return new Status
            {
                id = (int)reader["status_id"],
                name = (string)reader["name"],
                isPrimary = (bool)reader["is_primary"]
            };
        }
    }
}
