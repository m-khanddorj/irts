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
            if (this.id != -1)
            {
                list.Add(new Object[] { "m_meeting_id", this.id });
            }
            list.Add(new Object[] { "duration", this.duration });
            list.Add(new Object[] { "is_deleted", this.isDeleted });
            list.Add(new Object[] { "start_time", this.startTime });
            list.Add(new Object[] { "start_date", this.startDate });
            list.Add(new Object[] { "end_date", this.endDate });
            list.Add(new Object[] { "reason", this.reason });
            if (this.event_id != -1)
            {
                list.Add(new Object[] { "event_id", this.event_id });
            }
            if (this.meeting_id != -1)
            {
                list.Add(new Object[] { "meeting_id", this.meeting_id });
            }
            return list;
        }
    }
}
