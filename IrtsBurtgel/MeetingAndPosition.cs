using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class MeetingAndPosition : Entity
    {
        public override string TableName => "meeting_and_position";
        public override string IDName => "mpid";

        public int id;
        public int meetingId;
        public int positionId;

        public MeetingAndPosition()
        {
            id = -1;
            meetingId = -1;
            positionId = -1;
        }

        public override List<Object[]> ToKVStringList()
        {
            List<Object[]> list = new List<Object[]>();
            if (id != -1)
            {
                list.Add(new Object[] { "mpid", id });
            }
            list.Add(new Object[] { "meeting_id", meetingId });
            list.Add(new Object[] { "position_id", positionId });
            return list;
        }

        public override Entity GetObj(SqlDataReader reader)
        {
            return new MeetingAndPosition
            {
                id = (int)reader["mpid"],
                meetingId = (int)reader["meeting_id"],
                positionId = (int)reader["position_id"]
            };
        }
    }
}
