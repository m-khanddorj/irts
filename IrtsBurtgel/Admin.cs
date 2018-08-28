using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class Admin : Entity
    {
        public override string TableName => "admin";
        public override string IDName => "admin_id";

        public int id;
        public string username;
        public string password;
        public string organizationName;

        public Admin()
        {
            id = -1;
            username = "";
            password = "";
            organizationName = "";
        }

        public override List<Object[]> ToKVStringList()
        {
            List<Object[]> list = new List<Object[]>();
            if (id != -1)
            {
                list.Add(new Object[] { "admin_id", id });
            }
            list.Add(new Object[] { "username", username });
            list.Add(new Object[] { "password", password });
            list.Add(new Object[] { "organization_name", organizationName });
            return list;
        }

        public override Entity GetObj(SqlDataReader reader)
        {
            return new Admin
            {
                id = (int)reader["admin_id"],
                username = (string)reader["username"],
                password = (string)reader["password"],
                organizationName = (string)reader["organization_name"]
            };
        }
    }
}
