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
        public DateTime startDatetime;
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
            this.startDatetime = new DateTime();
            this.endDate = new DateTime();
            this.isDeleted = false;
        }
        
        public virtual List<Object[]> ToKVStringList()
        {
            List<Object[]> list = new List<Object[]>();
            if(id != -1)
            {
                list.Add(new Object[] { "meeting_id", id });
            }
            list.Add(new Object[] { "duration", duration});
            list.Add(new Object[] { "interval_day", intervalDay});
            list.Add(new Object[] { "is_deleted", isDeleted});
            list.Add(new Object[] { "start_datetime", startDatetime});
            if (DateTime.Compare(endDate, new DateTime()) != 0)
            {
                list.Add(new Object[] { "end_date", endDate });
            }
            else
            {
                list.Add(new Object[] { "end_date", null });
            }
            list.Add(new Object[] { "name", name});
            return list;
        }
    }
}
