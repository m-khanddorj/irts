using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class ModifiedMeeting : Meeting
    {
        public override string TableName => "modified_meeting";
        public override string IDName => "m_meeting_id";
        
        public string reason;
        public int event_id, meeting_id;

        public ModifiedMeeting()
        {
            id = -1;
            name = "";
            duration = 0;
            intervalDay = 0;
            name = "";
            startDatetime = new DateTime();
            endDate = new DateTime();
            isDeleted = false;
            reason = "";
            event_id = -1;
            meeting_id = -1;
        }
        
        public override List<Object[]> ToKVStringList()
        {
            List<Object[]> list = new List<Object[]>();
            if (id != -1)
            {
                list.Add(new Object[] { "m_meeting_id", id });
            }
            list.Add(new Object[] { "name", name });
            list.Add(new Object[] { "duration", duration });
            list.Add(new Object[] { "is_deleted", isDeleted });
            list.Add(new Object[] { "start_datetime", startDatetime });
            list.Add(new Object[] { "end_date", endDate });
            list.Add(new Object[] { "reason", reason });
            if (event_id != -1)
            {
                list.Add(new Object[] { "event_id", event_id });
            }
            if (meeting_id != -1)
            {
                list.Add(new Object[] { "meeting_id", meeting_id });
            }
            return list;
        }

        public override Entity GetObj(SqlDataReader reader)
        {
            return new ModifiedMeeting
            {
                id = (int)reader["m_meeting_id"],
                name = (string)reader["name"],
                startDatetime = (DateTime)reader["start_datetime"],
                endDate = (DateTime)reader["end_date"],
                duration = (int)reader["duration"],
                reason = (string)reader["reason"],
                isDeleted = (bool)reader["is_deleted"],
                event_id = reader["event_id"].GetType() != typeof(int) ? -1 : (int)reader["event_id"],
                meeting_id = (int)reader["meeting_id"]
            };
        }
    }
}
