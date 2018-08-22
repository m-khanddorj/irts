using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class MeetingController
    {
        public Model<Meeting> meetingModel;
        public Model<ModifiedMeeting> modifiedMeetingModel;
        public Model<Event> eventModel;
        public Model<MeetingAndUser> muModel;
        public Model<User> userModel;
        public Model<UserStatus> userStatusModel;

        public MeetingController()
        {
            meetingModel = new Model<Meeting>();
            modifiedMeetingModel = new Model<ModifiedMeeting>();
            eventModel = new Model<Event>();
            muModel = new Model<MeetingAndUser>();
            userModel = new Model<User>();
            userStatusModel = new Model<UserStatus>();
        }
        
        public List<Meeting> FindByDate(DateTime date)
        {
            List<Meeting> list = meetingModel.GetAll();
            List<Meeting> result = new List<Meeting>();

            foreach (Meeting meeting in list)
            {
                bool inDate = IsInDate(date, meeting.intervalType, meeting.week, meeting.intervalDay, meeting.startDatetime);
                
                if (DateTime.Compare(meeting.endDate, new DateTime()) != 0)
                {
                    inDate = inDate && ((int)((meeting.endDate.Date - date.Date).TotalDays) >= 0);
                }

                if (inDate)
                {
                    // TODO: Hardcoded. Improvement needed.
                    string sql = "SELECT * FROM modified_meeting WHERE meeting_id = @meeting_id AND (cast(start_datetime as date) ='" + date.Date.ToString("yyyyMMdd") + "' OR event_id IS NOT NULL)";
                    List<Object[]> parms = new List<Object[]>();
                    parms.Add(new Object[] { "meeting_id", meeting.id });
                    
                    List<ModifiedMeeting> mMeetings = modifiedMeetingModel.SelectBare(sql, parms);
                    if (mMeetings != null && mMeetings.Count > 0)
                    {
                        bool added = false;
                        foreach(ModifiedMeeting mMeeting in mMeetings)
                        {
                            if(mMeeting.event_id == -1)
                            {
                                result.Add(mMeeting);
                                added = true;
                                break;
                            }
                            else
                            {
                                Event ev = eventModel.Get(mMeeting.event_id);
                                if(IsInDate(date, ev.intervalType, ev.week, ev.intervalDay, ev.startDate))
                                {
                                    if(mMeeting.duration != 0)
                                    {
                                        result.Add(mMeeting);
                                    }
                                    added = true;
                                    break;
                                }
                            }
                        }
                        if(!added)
                        {
                            result.Add(meeting);
                        }
                    }
                    else
                    {
                        result.Add(meeting);
                    }
                }
            }

            return result;
        }

        private static bool IsInDate(DateTime date, byte intervalType, byte week, int intervalDay, DateTime startdate)
        {
            if((int)((date.Date - startdate.Date).TotalDays) < 0)
            {
                return false;
            }
            // No interval
            if (intervalType == 0)
            {
                return (int)(date.Date - startdate.Date).TotalDays == 0;
            }
            // Every week
            else if (intervalType == 1)
            {
                int weekNum = ((int)date.DayOfWeek == 0) ? 7 : (int)date.DayOfWeek;
                string binary = Convert.ToString(week, 2);
                while (binary.Length < 7)
                {
                    binary = "0" + binary;
                }
                return binary[weekNum - 1] == '1';
            }
            // Every 2 week
            else if (intervalType == 2)
            {
                bool in2Week = GetIso8601WeekOfYear(date.Date) % 2 == GetIso8601WeekOfYear(startdate) % 2;
                int weekNum = ((int)date.DayOfWeek == 0) ? 7 : (int)date.DayOfWeek;
                string binary = Convert.ToString(week, 2);
                while(binary.Length < 7)
                {
                    binary = "0" + binary;
                }
                return (binary[weekNum - 1] == '1') && in2Week;
            }
            // Every month
            else if (intervalType == 3)
            {
                return startdate.Day == date.Day;
            }
            // Every month start
            else if (intervalType == 4)
            {
                return 1 == date.Day;
            }
            // Every month end
            else if (intervalType == 5)
            {
                return date.Day == DateTime.DaysInMonth(date.Year, date.Month);
            }
            // Every year
            else if (intervalType == 6)
            {
                return date.Day == startdate.Day && date.Month == startdate.Month;
            }
            else
            {
                return (int)((date.Date - startdate.Date).TotalDays) % intervalDay == 0;
            }

        }

        public List<Meeting> GetAllMeeting()
        {
            return meetingModel.GetAll();
        }

        // This function creates and drops lot of connection because model has no function to get data from joined table.
        // TODO: Needs to be improved
        public List<Object[]> GetMeetingUserAttendances(ArchivedMeeting archivedMeeting, DateTime date)
        {
            List<Object[]> attendances = new List<Object[]>();

            List<MeetingAndUser> mulist =  muModel.GetByFK("meeting_id", archivedMeeting.meeting_id);
            List<int> uids = new List<int>();

            foreach (MeetingAndUser mu in mulist)
            {
                uids.Add(mu.userId);
            }

            List<User> users = userModel.Get(uids.ToArray());

            foreach(User user in users)
            {
                Attendance attendance = new Attendance();
                attendance.archivedMeetingId = archivedMeeting.id;

                List<UserStatus> userStatusHistory = userStatusModel.GetByFK(user.IDName, user.id);
                foreach (UserStatus userStatus in userStatusHistory)
                {
                    if(userStatus.startDate.Date <= date.Date && userStatus.endDate.Date >= date.Date)
                    {
                        attendance.statusId = userStatus.statusId;
                        break;
                    }
                }


            }

            return attendances;
        }

        public static int GetIso8601WeekOfYear(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}
