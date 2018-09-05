using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class Meeting : Entity
    {
        public override string TableName => "meeting";
        public override string IDName => "meeting_id";

        public int id;
        public int duration;
        public byte intervalType;
        public byte week;
        public int intervalDay;
        public int regMinBefMeeting;
        public string name;
        public DateTime startDatetime;
        public DateTime endDate;
        public bool isDeleted;

        public Meeting()
        {
            id = -1;
            duration = 0;
            intervalDay = 0;
            intervalType = 0;
            week = 0;
            name = "";
            startDatetime = new DateTime();
            endDate = new DateTime();
            isDeleted = false;
        }

        public override List<Object[]> ToKVStringList()
        {
            List<Object[]> list = new List<Object[]>();
            if (id != -1)
            {
                list.Add(new Object[] { "meeting_id", id });
            }
            list.Add(new Object[] { "duration", duration });
            list.Add(new Object[] { "interval_type", intervalType });
            list.Add(new Object[] { "interval_day", intervalDay });
            list.Add(new Object[] { "reg_bef_meeting", regMinBefMeeting });
            list.Add(new Object[] { "week", week });
            list.Add(new Object[] { "is_deleted", isDeleted });
            list.Add(new Object[] { "start_datetime", startDatetime });
            if (endDate != new DateTime())
            {
                list.Add(new Object[] { "end_date", endDate });
            }
            else
            {
                list.Add(new Object[] { "end_date", null });
            }
            list.Add(new Object[] { "name", name });
            return list;
        }


        public override Entity GetObj(SqlDataReader reader)
        {
            return new Meeting
            {
                id = (int)reader["meeting_id"],
                name = (string)reader["name"],
                startDatetime = (DateTime)reader["start_datetime"],
                endDate = reader["end_date"].GetType() != typeof(DateTime) ? new DateTime() : (DateTime)reader["end_date"],
                duration = (int)reader["duration"],
                intervalType = (byte)reader["interval_type"],
                intervalDay = (int)reader["interval_day"],
                regMinBefMeeting = (int)reader["reg_bef_meeting"],
                week = (byte)reader["Week"],
                isDeleted = (bool)reader["is_deleted"]
            };
        }
    }
}
