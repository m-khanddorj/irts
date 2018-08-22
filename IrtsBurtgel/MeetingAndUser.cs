using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class MeetingAndUser : Entity
    {
        public override string TableName => "meeting_and_user";
        public override string IDName => "muid";

        public int id;
        public int meetingId;
        public int userId;

        public MeetingAndUser()
        {
            id = -1;
            meetingId = -1;
            userId = -1;
        }

        public override List<Object[]> ToKVStringList()
        {
            List<Object[]> list = new List<Object[]>();
            if (id != -1)
            {
                list.Add(new Object[] { "muid", id });
            }
            list.Add(new Object[] { "meeting_id", meetingId });
            list.Add(new Object[] { "user_id", userId });
            return list;
        }

        public override Entity GetObj(SqlDataReader reader)
        {
            return new MeetingAndUser
            {
                id = (int)reader["muid"],
                meetingId = (int)reader["meeting_id"],
                userId = (int)reader["user_id"]
            };
        }
    }
}
