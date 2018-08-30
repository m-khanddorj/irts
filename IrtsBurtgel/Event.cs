using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class Event : Entity
    {
        public override string TableName => "event";
        public override string IDName => "event_id";

        public int id;
        public byte intervalType;
        public string name;
        public DateTime startDate;
        public DateTime endDate;
        public bool isDeleted;

        public Event()
        {
            id = -1;
            intervalType = 0;
            name = "";
            startDate = new DateTime();
            endDate = new DateTime();
            isDeleted = false;
        }

        public override List<Object[]> ToKVStringList()
        {
            List<Object[]> list = new List<Object[]>();
            if (id != -1)
            {
                list.Add(new Object[] { "event_id", id });
            }
            list.Add(new Object[] { "interval_type", intervalType });
            list.Add(new Object[] { "is_deleted", isDeleted });
            list.Add(new Object[] { "start_date", startDate });
            if (DateTime.Compare(endDate, new DateTime()) != 0)
            {
                list.Add(new Object[] { "end_date", endDate });
            }
            list.Add(new Object[] { "name", name });
            return list;
        }


        public override Entity GetObj(SqlDataReader reader)
        {
            return new Event
            {
                id = (int)reader["event_id"],
                name = (string)reader["name"],
                startDate = (DateTime)reader["start_date"],
                endDate = reader["end_date"].GetType() != typeof(DateTime) ? new DateTime() : (DateTime)reader["end_date"],
                intervalType = (byte)reader["interval_type"],
                isDeleted = (bool)reader["is_deleted"]
            };
        }
    }
}
