using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class ModifiedMeeting : Meeting
    {
        public string reason;
        public int event_id, meeting_id;

        public ModifiedMeeting()
        {
            this.reason = "";
            this.event_id = -1;
            this.meeting_id = -1;
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
            else
            {
                list.Add(new Object[] { "event_id", null });
            }
            if (meeting_id != -1)
            {
                list.Add(new Object[] { "meeting_id", meeting_id });
            }
            else
            {
                list.Add(new Object[] { "meeting_id", null });
            }
            return list;
        }
    }
}
