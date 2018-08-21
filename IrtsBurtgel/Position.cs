using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class Position : Entity
    {
        public override string TableName => "position";
        public override string IDName => "position_id";

        public int id;
        public string name;
        public bool isDeleted;

        public Position()
        {
            id = -1;
            name = "";
            isDeleted = false;
        }

        public override List<Object[]> ToKVStringList()
        {
            List<Object[]> list = new List<Object[]>();
            if (id != -1)
            {
                list.Add(new Object[] { "position_id", id });
            }
            list.Add(new Object[] { "name", name });
            list.Add(new Object[] { "is_deleted", isDeleted });
            return list;
        }

        public override Entity GetObj(SqlDataReader reader)
        {
            return new Position
            {
                id = (int)reader["position_id"],
                name = (string)reader["name"],
                isDeleted = (bool)reader["is_deleted"]
            };
        }
    }
}
