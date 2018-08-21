using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class MeetingController
    {
        public Model<Meeting> meetingModel;
        public Model<ModifiedMeeting> modifiedMeetingModel;

        public MeetingController()
        {
            meetingModel = new Model<Meeting>();
            modifiedMeetingModel = new Model<ModifiedMeeting>();
        }
        
        public List<Meeting> FindByDate(DateTime date)
        {
            List<Meeting> list = meetingModel.GetAll();
            List<Meeting> result = new List<Meeting>();

            foreach (Meeting meeting in list)
            {
                bool inDate = false;

                if (meeting.intervalDay == 0)
                {
                    inDate = (int)(date.Date - meeting.startDatetime.Date).TotalDays == 0;
                }
                else
                {
                    Console.WriteLine((int)((date.Date - meeting.startDatetime.Date).TotalDays) % meeting.intervalDay);
                    inDate = (int)((date.Date - meeting.startDatetime.Date).TotalDays) % meeting.intervalDay == 0;
                }
                
                if (DateTime.Compare(meeting.endDate, new DateTime()) != 0)
                {
                    inDate = inDate && ((int)((meeting.endDate.Date - date.Date).TotalDays) >= 0);
                }

                if (inDate)
                {
                    // TODO: Hardcoded. Improvement needed.
                    string sql = "SELECT * FROM modified_meeting WHERE meeting_id = @meeting_id AND cast(start_datetime as date) <= '" + date.Date.ToString("yyyyMMdd") + "' AND  cast(start_datetime as date)  >='" + date.Date.ToString("yyyyMMdd") + "'";
                    List<Object[]> parms = new List<Object[]>();
                    parms.Add(new Object[] { "meeting_id", meeting.id });
                    
                    ModifiedMeeting mMeeting = modifiedMeetingModel.SelectBare(sql, parms);
                    if (mMeeting != null)
                    {
                        result.Add(mMeeting);
                    }
                    else
                    {
                        result.Add(meeting);
                    }
                }
            }

            return result;
        }

        public List<Meeting> GetAllMeeting()
        {
            return meetingModel.GetAll();
        }
    }
}
