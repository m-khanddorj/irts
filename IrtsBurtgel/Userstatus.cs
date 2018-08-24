using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class UserStatus : Entity
    {
        public override string TableName => "user_status";
        public override string IDName => "user_status_id";

        public int id;
        public int statusId;
        public int userId;
        public DateTime startDate;
        public DateTime endDate;
        public bool isDeleted;

        public UserStatus()
        {
            id = -1;
            statusId = -1;
            userId = -1;
            startDate = new DateTime();
            endDate = new DateTime();
            isDeleted = false;
        }

        public override List<Object[]> ToKVStringList()
        {
            List<Object[]> list = new List<Object[]>();
            if (id != -1)
            {
                list.Add(new Object[] { "user_status_id", id });
            }
            list.Add(new Object[] { "user_id", userId });
            list.Add(new Object[] { "status_id", statusId });
            list.Add(new Object[] { "start_date", startDate });
            list.Add(new Object[] { "end_date", endDate });
            list.Add(new Object[] { "is_deleted", isDeleted });
            return list;
        }

        public override Entity GetObj(SqlDataReader reader)
        {
            return new UserStatus
            {
                id = (int)reader["user_status_id"],
                userId = (int)reader["user_id"],
                statusId = (int)reader["status_id"],
                startDate = (DateTime)reader["start_date"],
                endDate = (DateTime)reader["end_date"],
                isDeleted = (bool)reader["is_deleted"]
            };
        }
    }
}
