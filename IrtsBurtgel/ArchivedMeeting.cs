using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class ArchivedMeeting : Entity
    {
        public override string TableName => "archived_meeting";
        public override string IDName => "a_meeting_id";

        public string protocol;
        public int id;
        public int meeting_id;
        public int modifiedMeeting_id;
        public string name;
        public int duration;
        public DateTime meetingDatetime;

        public ArchivedMeeting()
        {
            id = -1;
            name = "";
            meeting_id = -1;
            modifiedMeeting_id = -1;
            meetingDatetime = new DateTime();
            protocol = "";
            duration = 0;
        }

        public override List<Object[]> ToKVStringList()
        {
            List<Object[]> list = new List<Object[]>();
            if (id != -1)
            {
                list.Add(new Object[] { "a_meeting_id", id });
            }
            list.Add(new Object[] { "meeting_id", meeting_id });
            list.Add(new Object[] { "protocol", protocol });
            list.Add(new Object[] { "name", name });
            list.Add(new Object[] { "duration", duration });
            list.Add(new Object[] { "meeting_datetime", meetingDatetime });
            if (modifiedMeeting_id != -1)
            {
                list.Add(new Object[] { "m_meeting_id", modifiedMeeting_id });
            }
            return list;
        }

        public override Entity GetObj(SqlDataReader reader)
        {
            return new ArchivedMeeting
            {
                id = (int)reader["a_meeting_id"],
                name = (string)reader["name"],
                meeting_id = (int)reader["meeting_id"],
                duration = (int)reader["duration"],
                modifiedMeeting_id = reader["m_meeting_id"].GetType() != typeof(int) ? -1 : (int)reader["m_meeting_id"],
                meetingDatetime = (DateTime)reader["meeting_datetime"],
                protocol = (string)reader["protocol"]
            };
        }
    }
}
