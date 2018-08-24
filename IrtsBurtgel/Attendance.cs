using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class Attendance : Entity
    {
        public override string TableName => "attendance";
        public override string IDName => "attendance_id";

        public int id;
        public int userId;
        public int statusId;
        public int archivedMeetingId;
        public DateTime regTime;

        public Attendance()
        {
            id = -1;
            userId = -1;
            statusId = -1;
            archivedMeetingId = -1;
            regTime = new DateTime();
        }
        
        public override List<Object[]> ToKVStringList()
        {
            List<Object[]> list = new List<Object[]>();
            if (id != -1) list.Add(new Object[] { "attendance_id", id });
            if (userId != -1) list.Add(new Object[] { "user_id", userId });
            if (statusId != -1) list.Add(new Object[] { "status_id", statusId });
            if (archivedMeetingId != -1) list.Add(new Object[] { "a_meeting_id", archivedMeetingId });
            if (DateTime.Compare(regTime, new DateTime()) != 0)
            {
                list.Add(new Object[] { "reg_time", regTime });
            }
            return list;
        }

        public override Entity GetObj(SqlDataReader reader)
        {
            return new Attendance
            {
                id = (int)reader["attendance_id"],
                userId = (int)reader["user_id"],
                archivedMeetingId = (int)reader["a_meeting_id"],
                statusId = (int)reader["status_id"],
                regTime = (DateTime)reader["reg_time"]
            };
        }
    }
}
