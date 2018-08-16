using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class MeetingController
    {
        public MeetingModel meetingModel;
        public ModifiedMeetingModel modifiedMeetingModel;

        public MeetingController()
        {
            string servername = "DESKTOP-G49ADVI\\SQLEXPRESS";
            string dbname = "IrtsBurtgel";
            string ints = "SSPI";

            meetingModel = new MeetingModel(servername, dbname, ints);
            modifiedMeetingModel = new ModifiedMeetingModel(servername, dbname, ints);
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
                    inDate = (date.Date - meeting.startDate.Date).TotalDays == 0;
                }
                else
                {
                    inDate = (date.Date - meeting.startDate.Date).TotalDays == 0 % meeting.intervalDay;
                }

                if (inDate)
                {
                    ModifiedMeeting mMeeting = modifiedMeetingModel.FindByDateAndMid(date.Date, meeting.id);
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
    }
}
