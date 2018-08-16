using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class Meeting
    {
        public int id;
        public int duration;
        public int intervalDay;
        public string name;
        public DateTime startTime;
        public DateTime startDate;
        public DateTime endDate;
        public bool isDeleted;
        public int[] uids;
        public int[] gids;

        public Meeting()
        {
            this.id = -1;
            this.duration = 0;
            this.intervalDay = 0;
            this.name = "";
            this.startTime = new DateTime();
            this.startDate = new DateTime();
            this.endDate = new DateTime();
            this.isDeleted = false;
        }

        public virtual List<Object[]> ToKVStringList()
        {
            List<Object[]> list = new List<Object[]>();
            if (this.id != -1)
            {
                list.Add(new Object[] { "meeting_id", this.id });
            }
            list.Add(new Object[] { "duration", this.duration });
            list.Add(new Object[] { "interval_day", this.intervalDay });
            list.Add(new Object[] { "is_deleted", this.isDeleted });
            list.Add(new Object[] { "start_time", this.startTime });
            list.Add(new Object[] { "start_date", this.startDate });
            list.Add(new Object[] { "end_date", this.endDate });
            list.Add(new Object[] { "name", this.name });
            return list;
        }
    }
}
