using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class User : Entity
    {
        public override string TableName => "user";
        public override string IDName => "user_id";

        public int id;
        public int pin;
        public string fname;
        public string lname;
        public string fingerprint0;
        public string fingerprint1;
        public bool isDeleted;
        public int positionId;
        public int departmentId;

        public User()
        {
            id = -1;
            pin = -1;
            fname = "";
            lname = "";
            fingerprint0 = "";
            fingerprint1 = "";
            isDeleted = false;
            positionId = -1;
            departmentId = -1;
        }
        
        public override List<Object[]> ToKVStringList()
        {
            List<Object[]> list = new List<Object[]>();
            if (id != -1)
            {
                list.Add(new Object[] { "user_id", id });
            }
            if (pin != -1)
            {
                list.Add(new Object[] { "pin", pin });
            }
            list.Add(new Object[] { "fname", fname });
            list.Add(new Object[] { "lname", lname });
            list.Add(new Object[] { "fingerprint_0", fingerprint0 });
            list.Add(new Object[] { "fingerprint_1", fingerprint1 });
            list.Add(new Object[] { "is_deleted", isDeleted });
            if (positionId != -1)
            {
                list.Add(new Object[] { "position_id", positionId });
            }
            if (departmentId != -1)
            {
                list.Add(new Object[] { "department_id", departmentId });
            }
            return list;
        }

        public override Entity GetObj(SqlDataReader reader)
        {
            return new User
            {
                id = (int)reader["user_id"],
                pin = reader["pin"].GetType() != typeof(int) ? -1 : (int)reader["pin"],
                fname = (string)reader["fname"],
                lname = (string)reader["lname"],
                fingerprint0 = (string)reader["fingerprint_0"],
                fingerprint1 = (string)reader["fingerprint_1"],
                isDeleted = (bool)reader["is_deleted"],
                positionId = reader["position_id"].GetType() != typeof(int) ? -1 : (int)reader["position_id"],
                departmentId = reader["department_id"].GetType() != typeof(int) ? -1 : (int)reader["department_id"]
            };
        }
    }

    public class UserComparer : IEqualityComparer<User>
    {
        public bool Equals(User x, User y)
        {
            //Check whether the objects are the same object. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether the products' properties are equal. 
            return x != null && y != null && x.id.Equals(y.id);
        }

        public int GetHashCode(User obj)
        {
            //Get hash code for the Name field if it is not null. 
            int hashProductName = obj.fname == null ? 0 : obj.fname.GetHashCode();

            //Get hash code for the Code field. 
            int hashProductCode = obj.id.GetHashCode();

            //Calculate the hash code for the product. 
            return hashProductName ^ hashProductCode;
        }
    }
}
