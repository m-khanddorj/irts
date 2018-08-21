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

        public DateTime meeting_date;
        public string protocol;
        public int id;
        public int meeting_id;
        public int modifiedMeeting_id;

        public ArchivedMeeting()
        {
            id = -1;
            meeting_id = -1;
            modifiedMeeting_id = -1;
            meeting_date = new DateTime();
            protocol = "";
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
            list.Add(new Object[] { "meeting_date", meeting_date });
            if (modifiedMeeting_id != -1)
            {
                list.Add(new Object[] { "m_meeting_id", modifiedMeeting_id });
            }
            else
            {
                list.Add(new Object[] { "m_meeting_id", null });
            }
            return list;
        }

        public override Entity GetObj(SqlDataReader reader)
        {
            return new ArchivedMeeting
            {
                id = (int)reader["a_meeting_id"],
                meeting_id = (int)reader["meeting_id"],
                modifiedMeeting_id = reader["m_meeting_id"].GetType() != typeof(int) ? -1 : (int)reader["m_meeting_id"],
                meeting_date = (DateTime)reader["meeting_date"],
                protocol = (string)reader["protocol"]
            };
        }
    }
}
