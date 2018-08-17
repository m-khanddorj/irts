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
            meetingModel = new MeetingModel();
            modifiedMeetingModel = new ModifiedMeetingModel();
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
                    inDate = (int)((date.Date - meeting.startDatetime.Date).TotalDays) % meeting.intervalDay == 0;
                }
                
                if (DateTime.Compare(meeting.endDate, new DateTime()) != 0)
                {
                    inDate = inDate && ((int)((meeting.endDate.Date - date.Date).TotalDays) > 0);
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

        public List<Meeting> GetAllMeeting()
        {
            return meetingModel.GetAll();
        }
    }
}
