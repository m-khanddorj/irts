using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IrtsBurtgel
{
    public class MeetingController
    {
        public static int MEETING_STARTED = 1, IDLE = 0, MEETING_STARTING = 2;

        public Model<Meeting> meetingModel;
        public Model<ModifiedMeeting> modifiedMeetingModel;
        public Model<ArchivedMeeting> archivedMeetingModel;
        public Model<Event> eventModel;
        public Model<User> userModel;
        public Model<UserStatus> userStatusModel;
        public Model<Attendance> attendanceModel;
        public Model<Department> departmentModel;
        public Model<Position> positionModel;
        public Model<Status> statusModel;
        public Model<Admin> adminModel;

        public Model<MeetingAndUser> muModel;
        public Model<MeetingAndDepartment> mdModel;
        public Model<MeetingAndPosition> mpModel;

        public List<Object[]> onGoingMeetingUserAttendance;
        public ArchivedMeeting onGoingArchivedMeeting;
        public Meeting onGoingMeeting;
        public ScannerHandler scannerHandler;

        public Meeting closestMeeting;
        public int status;

        public Timer aTimer;
        public MainWindow mainWindow;

        public MeetingController(MainWindow mw)
        {
            meetingModel = new Model<Meeting>();
            modifiedMeetingModel = new Model<ModifiedMeeting>();
            archivedMeetingModel = new Model<ArchivedMeeting>();
            eventModel = new Model<Event>();
            userModel = new Model<User>();
            userStatusModel = new Model<UserStatus>();
            attendanceModel = new Model<Attendance>();
            departmentModel = new Model<Department>();
            positionModel = new Model<Position>();
            statusModel = new Model<Status>();
            adminModel = new Model<Admin>();

            muModel = new Model<MeetingAndUser>();
            mdModel = new Model<MeetingAndDepartment>();
            mpModel = new Model<MeetingAndPosition>();

            scannerHandler = new ScannerHandler(mc: this);
            mainWindow = mw;
        }

        public void CheckMeeting()
        {
            DateTime now = DateTime.Now;

            Console.WriteLine("Status = " + status.ToString());
            if (status == IDLE)
            {
                Console.WriteLine(now.TimeOfDay);
                List<Meeting> meetings = FindByDate(now);
                if (meetings.Count == 0)
                {
                    return;
                }

                bool catchedClosest = false;
                closestMeeting = null;
                foreach (Meeting meeting in meetings)
                {
                    if (meeting.duration > 0)
                    {
                        int regbefminute = meeting is ModifiedMeeting ? meetingModel.Get(((ModifiedMeeting)meeting).meeting_id).regMinBefMeeting : meeting.regMinBefMeeting;

                        Console.WriteLine(meeting.startDatetime.Add(new TimeSpan(0, -regbefminute, 0)).TimeOfDay);
                        Console.WriteLine(now.TimeOfDay);
                        Console.WriteLine(meeting.startDatetime.AddMinutes(meeting.duration).TimeOfDay);
                        if (meeting.startDatetime.Add(new TimeSpan(0, -regbefminute, 0)).Hour == now.Hour && meeting.startDatetime.Add(new TimeSpan(0, -regbefminute, 0)).Minute == now.Minute)
                        {
                            Console.WriteLine("Meeting time occured. Start meeting registration.");
                            status = MEETING_STARTING;
                            StartMeeting(meeting);
                            status = MEETING_STARTED;
                            Console.WriteLine("Setted Status = " + status.ToString());
                            return;
                        }
                        if (meeting.startDatetime.Add(new TimeSpan(0, -regbefminute, 0)).TimeOfDay < now.TimeOfDay && meeting.startDatetime.AddMinutes(meeting.duration).TimeOfDay > now.TimeOfDay)
                        {
                            Console.WriteLine("Detected ongoing meeting. Fast forwarding meeting.");
                            status = MEETING_STARTING;
                            StartMeeting(meeting);
                            status = MEETING_STARTED;
                            Console.WriteLine("Setted Status = " + status.ToString());
                            return;
                        }

                        if (meeting.startDatetime.TimeOfDay > now.TimeOfDay && catchedClosest == false)
                        {
                            catchedClosest = true;
                            closestMeeting = meeting;
                        }
                    }
                }
                if (closestMeeting != null)
                {
                    Console.WriteLine(now - closestMeeting.startDatetime);
                }
                else
                {
                    Console.WriteLine("No meeting today.");
                }
            }
            else if(status == MEETING_STARTED)
            {
                DateTime date = onGoingArchivedMeeting.meetingDatetime.AddMinutes(onGoingArchivedMeeting.duration);
                if (date.Hour <= now.Hour && date.Minute <= now.Minute)
                {
                    StopMeeting();
                    status = IDLE;
                    Console.WriteLine("Meeting ended.");
                }
            }
        }

        public string TextToDisplay()
        {
            DateTime now = DateTime.Now;

            //Checking todays meetings
            List<Meeting> meetings = FindByDate(now);
            
            if (meetings == null || meetings.Count == 0) return "Өнөөдөр хуралгүй.";
            int i;
            for (i=0;i<meetings.Count;i++)
            {
                if(now.TimeOfDay > meetings[i].startDatetime.TimeOfDay.Add(new TimeSpan(0,meetings[i].duration,0)) )
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            //i is now equal to present or next meeting index of meetings
            if (i== meetings.Count)
            {
                return "Өнөөдрийн хурал дууссан.";
            }
            else if (meetings[i].startDatetime.TimeOfDay > now.TimeOfDay && now.AddMinutes(meetings[i].regMinBefMeeting).TimeOfDay > meetings[i].startDatetime.TimeOfDay)
            {
                return "Хурлын бүртгэл явагдаж байна.\n Хурал эхлэхэд: " + (meetings[i].startDatetime.TimeOfDay - now.TimeOfDay).ToString(@"mm\:ss");
            }
            else if (meetings[i].startDatetime.TimeOfDay > now.TimeOfDay)
            {
                return "Дараагийн хурал " + meetings[i].startDatetime.ToShortTimeString() + "-с эхэлнэ.";
            }
            else if (meetings[i].startDatetime.TimeOfDay < now.TimeOfDay && meetings[i].startDatetime.AddMinutes(meetings[i].duration).TimeOfDay > now.TimeOfDay)
            {
                return "Хурал эхлээд " + (now.TimeOfDay - meetings[i].startDatetime.TimeOfDay).ToString(@"mm\:ss") + "";
            }
            else return "";
        }

        public List<Meeting> FindByDate(DateTime date)
        {
            List<Meeting> list = meetingModel.GetAll();
            List<Meeting> result = new List<Meeting>();

            List<Event> allEvents = eventModel.GetAll();
            List<Event> events = new List<Event>();
            foreach(Event ev in allEvents)
            {
                if ((ev.endDate.Date >= date.Date) || (ev.startDate.Date >= date.Date && ev.intervalType == 2))
                {
                    events.Add(ev);
                }
            }

            foreach(Event ev in events)
            {
                DateTime endDate = ev.endDate, startDate = ev.startDate;
                if(endDate.Date <= date.Date)
                {
                    if(ev.intervalType == 1)
                    {
                        endDate = endDate.AddMonths((date.Month - ev.endDate.Month) + 12 * (date.Year - ev.endDate.Year));
                        startDate = startDate.AddMonths((date.Month - ev.endDate.Month) + 12 * (date.Year - ev.endDate.Year));
                    }
                    else if(ev.intervalType == 0)
                    {
                        endDate = endDate.AddYears(date.Year - endDate.Year);
                        startDate = startDate.AddYears(date.Year - endDate.Year);
                    }
                }
                if(date.Date >= startDate.Date && date.Date <= endDate.Date)
                {
                    return result;
                }
            }


            foreach (Meeting meeting in list)
            {
                if (meeting.isDeleted)
                {
                    continue;
                } 
                bool inDate = IsInDate(date, meeting.intervalType, meeting.week, meeting.intervalDay, meeting.startDatetime);

                if (meeting.endDate != new DateTime())
                {
                    inDate = inDate && ((int)((meeting.endDate.Date - date.Date).TotalDays) >= 0);
                }

                if (inDate)
                {
                    // TODO: Hardcoded. Improvement needed.
                    string sql = "SELECT * FROM modified_meeting WHERE meeting_id = @meeting_id AND (cast(start_datetime as date) ='" + date.Date.ToString("yyyyMMdd") + "')";
                    List<Object[]> parms = new List<Object[]>();

                    parms.Add(new Object[] { "meeting_id", meeting.id });

                    List<ModifiedMeeting> mMeetings = modifiedMeetingModel.SelectBare(sql, parms);
                    if (mMeetings != null && mMeetings.Count > 0)
                    {
                        bool added = false;
                        mMeetings = mMeetings.OrderByDescending(x => x.order).ToList();
                        foreach (ModifiedMeeting mMeeting in mMeetings)
                        {
                            result.Add(mMeeting);
                            added = true;
                            break;
                        }
                        if (!added)
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
            result = result.OrderBy(x => x.startDatetime.TimeOfDay).ToList();            
            return result;
        }

        public List<ArchivedMeeting> GetArchivedMeetingByDate(DateTime date)
        {
            // TODO: Hardcoded. Improvement needed.
            string sql = "SELECT * FROM archived_meeting WHERE (cast(meeting_datetime as date) ='" + date.Date.ToString("yyyyMMdd") + "')";
            List<Object[]> parms = new List<Object[]>();

            return archivedMeetingModel.SelectBare(sql);
        }

        private static bool IsInDate(DateTime date, byte intervalType, byte week, int intervalDay, DateTime startdate)
        {
            if ((int)((date.Date - startdate.Date).TotalDays) < 0)
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
                //int weekNum = ((int)date.DayOfWeek == 0) ? 7 : (int)date.DayOfWeek;
                //string binary = Convert.ToString(week, 2);
                //while (binary.Length < 7)
                //{
                //    binary = "0" + binary;
                //}
                //return binary[weekNum - 1] == '1';
                return (int)((date.Date - startdate.Date).TotalDays) % 7 == 0;
            }
            // Every 2 week
            else if (intervalType == 2)
            {
                //bool in2Week = GetIso8601WeekOfYear(date.Date) % 2 == GetIso8601WeekOfYear(startdate) % 2;
                //int weekNum = ((int)date.DayOfWeek == 0) ? 7 : (int)date.DayOfWeek;
                //string binary = Convert.ToString(week, 2);
                //while (binary.Length < 7)
                //{
                //    binary = "0" + binary;
                //}
                //return (binary[weekNum - 1] == '1') && in2Week;
                return (int)((date.Date - startdate.Date).TotalDays) % 14 == 0;
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
        public List<Object[]> GetMeetingUserAttendances(ArchivedMeeting archivedMeeting)
        {
            List<Attendance> attendances = attendanceModel.GetByFK(archivedMeeting.IDName, archivedMeeting.id);
            List<Object[]> userAttendance = new List<Object[]>();

            List<MeetingAndUser> mulist = muModel.GetByFK("meeting_id", archivedMeeting.meeting_id);

            List<User> users = GetMeetingUser(meetingModel.Get(archivedMeeting.meeting_id));

            foreach (User user in users)
            {
                Attendance attendance = attendances.Find(x => x.userId == user.id);
                if (attendance == null)
                {
                    attendance = new Attendance();
                    attendance.archivedMeetingId = archivedMeeting.id;
                    attendance.statusId = 15;
                    attendance.regTime = -1;
                    attendance.userId = user.id;

                    List<UserStatus> userStatusHistory = userStatusModel.GetByFK(user.IDName, user.id);
                    foreach (UserStatus userStatus in userStatusHistory)
                    {
                        if (userStatus.startDate.Date <= archivedMeeting.meetingDatetime.Date && userStatus.endDate.Date >= archivedMeeting.meetingDatetime.Date)
                        {
                            attendance.statusId = userStatus.statusId;
                            break;
                        }
                    }

                    int id = attendanceModel.Add(attendance);
                    if (id == -1)
                    {
                        continue;
                    }
                    else
                    {
                        attendance.id = id;
                    }
                }

                userAttendance.Add(new Object[] { user, attendance });
            }

            return userAttendance;
        }

        public bool StartMeeting(Meeting meeting)
        {
            DateTime now = DateTime.Now;
            List<ArchivedMeeting> archivedMeetings = GetArchivedMeetingByDate(now);
            ArchivedMeeting archivedMeeting;

            int regbefminute = meeting is ModifiedMeeting ? meetingModel.Get(((ModifiedMeeting)meeting).meeting_id).regMinBefMeeting : meeting.regMinBefMeeting;

            if (meeting.GetType() == typeof(ModifiedMeeting))
            {
                archivedMeeting = archivedMeetings.FindAll(x => x.meeting_id == ((ModifiedMeeting)meeting).meeting_id).Find(x => (x.meetingDatetime.AddMinutes(-regbefminute) < now && x.meetingDatetime.AddMinutes(x.duration) > now));
            }
            else
            {
                archivedMeeting = archivedMeetings.FindAll(x => x.meeting_id == meeting.id).Find(x => (x.meetingDatetime.AddMinutes(-regbefminute) < now && x.meetingDatetime.AddMinutes(x.duration) > now));
            }


            if (archivedMeeting == null)
            {
                Console.WriteLine("Creating archive of meeting");
                archivedMeeting = new ArchivedMeeting();

                if (meeting.GetType() == typeof(ModifiedMeeting))
                {
                    archivedMeeting.meeting_id = ((ModifiedMeeting)meeting).meeting_id;
                    archivedMeeting.modifiedMeeting_id = meeting.id;
                }
                else
                {
                    archivedMeeting.meeting_id = meeting.id;
                }

                archivedMeeting.meetingDatetime = now.Date + (meeting.startDatetime).TimeOfDay;
                archivedMeeting.duration = meeting.duration;
                archivedMeeting.name = meeting.name;

                int archivedMeetingId = archivedMeetingModel.Add(archivedMeeting);

                if (archivedMeetingId != -1)
                {
                    archivedMeeting.id = archivedMeetingId;
                }
                else
                {
                    return false;
                }
            }

            onGoingMeeting = meeting;
            onGoingArchivedMeeting = archivedMeeting;
            onGoingMeetingUserAttendance = GetMeetingUserAttendances(archivedMeeting);

            scannerHandler.InitializeDevice();
            scannerHandler.StartCaptureThread();
            return true;
        }

        public bool StopMeeting()
        {
            foreach (MeetingStatus ms in mainWindow.meetingStatusWindows)
            {
                ms.Dispatcher.Invoke(() =>
                {
                    foreach (KeyValuePair<int,DepartmentStatus> ds in ms.departmentStatusWindows)
                    {
                        ds.Value.Close();
                    }
                    ms.departmentStatusWindows.Clear();
                });
            }

            if (onGoingMeetingUserAttendance != null && onGoingMeetingUserAttendance.Count > 0)
            {
                foreach (Object[] userAttendance in onGoingMeetingUserAttendance)
                {
                    Attendance attendance = (Attendance)userAttendance[1];
                    if (attendance.statusId == 15)
                    {
                        attendance.statusId = 14;
                        attendanceModel.Set(attendance);
                    }
                }
                scannerHandler.Stop();
                return true;
            }

            return false;
        }

        public void CancelMeetingsByDate(DateTime date, string reason)
        {
            List<Meeting> meetings = FindByDate(date);
            foreach (Meeting meeting in meetings)
            {
                if (meeting.GetType() == typeof(ModifiedMeeting))
                {
                    ModifiedMeeting mMeeting = (ModifiedMeeting)meeting;
                    modifiedMeetingModel.Set(new ModifiedMeeting
                    {
                        name = mMeeting.name,
                        startDatetime = date,
                        duration = 0,
                        reason = reason,
                        meeting_id = mMeeting.meeting_id,
                        order = mMeeting.order + 1
                    });
                }
                else
                {
                    modifiedMeetingModel.Add(new ModifiedMeeting
                    {
                        name = meeting.name,
                        startDatetime = date,
                        duration = 0,
                        reason = reason,
                        meeting_id = meeting.id
                    });
                }
            }
        }

        public bool SetMeetingUser(Meeting meeting, List<Department> departments, List<Position> positions, List<User> users)
        {
            try
            {
                List<MeetingAndUser> mus = muModel.GetByFK(meeting.IDName, meeting.id);
                List<MeetingAndDepartment> mds = mdModel.GetByFK(meeting.IDName, meeting.id);
                List<MeetingAndPosition> mps = mpModel.GetByFK(meeting.IDName, meeting.id);
                List<int> toInsert = new List<int>();
                List<int> toDelete = new List<int>();
                Object[] result;

                // Set users
                result = GetDifference(mus.Select(x => x.userId).ToArray(), users.Select(x => x.id).ToArray());

                toInsert = (List<int>)result[1];
                toDelete = (List<int>)result[2];

                muModel.Remove(mus.FindAll(x => toDelete.Contains(x.userId)).Select(x => x.id).ToArray());

                List<MeetingAndUser> insertMus = new List<MeetingAndUser>();
                while (toInsert.Count > 0)
                {
                    int first = toInsert.First();
                    insertMus.Add(new MeetingAndUser
                    {
                        userId = first,
                        meetingId = meeting.id
                    });
                    toInsert.Remove(0);
                }
                muModel.BulkAdd(insertMus);

                // Set departments
                result = GetDifference(mds.Select(x => x.departmentId).ToArray(), departments.Select(x => x.id).ToArray());

                toInsert = (List<int>)result[1];
                toDelete = (List<int>)result[2];

                mdModel.Remove(mds.FindAll(x => toDelete.Contains(x.departmentId)).Select(x => x.id).ToArray());

                List<MeetingAndDepartment> insertMds = new List<MeetingAndDepartment>();
                while (toInsert.Count > 0)
                {
                    int first = toInsert.First();
                    insertMds.Add(new MeetingAndDepartment
                    {
                        departmentId = first,
                        meetingId = meeting.id
                    });
                    toInsert.Remove(0);
                }
                mdModel.BulkAdd(insertMds);

                // Set positions
                result = GetDifference(mps.Select(x => x.positionId).ToArray(), positions.Select(x => x.id).ToArray());

                toInsert = (List<int>)result[1];
                toDelete = (List<int>)result[2];

                mpModel.Remove(mps.FindAll(x => toDelete.Contains(x.positionId)).Select(x => x.id).ToArray());

                List<MeetingAndPosition> insertMps = new List<MeetingAndPosition>();
                while (toInsert.Count > 0)
                {
                    int first = toInsert.First();
                    insertMps.Add(new MeetingAndPosition
                    {
                        positionId = first,
                        meetingId = meeting.id
                    });
                    toInsert.Remove(0);
                }
                mpModel.BulkAdd(insertMps);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public List<User> GetMeetingUser(Meeting meeting)
        {
            try
            {
                List<MeetingAndUser> mus = muModel.GetByFK(meeting.IDName, meeting.id);
                List<MeetingAndDepartment> mds = mdModel.GetByFK(meeting.IDName, meeting.id);
                List<MeetingAndPosition> mps = mpModel.GetByFK(meeting.IDName, meeting.id);
                List<User> tempUsers = new List<User>();
                List<User> depUsers = new List<User>();
                List<User> posUsers = new List<User>();

                if (mus != null && mus.Any())
                {
                    tempUsers.AddRange(userModel.Get(mus.Select(x => x.userId).ToArray()));
                }

                if (mds != null && mds.Any())
                {
                    depUsers.AddRange(userModel.GetByFK(departmentModel.staticObj.IDName, mds.Select(x => x.departmentId).ToArray()));
                }

                if (mps != null && mps.Any())
                {
                    posUsers.AddRange(userModel.GetByFK(positionModel.staticObj.IDName, mps.Select(x => x.positionId).ToArray()));
                }

                tempUsers = tempUsers.Union(depUsers, new UserComparer()).ToList();
                tempUsers = tempUsers.Union(posUsers, new UserComparer()).ToList();

                return tempUsers;
            }
            catch (Exception ex)
            {
                return new List<User>();
            }
        }

        public string GetUserImage(User user)
        {
            if (File.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\userimages\\" + user.pin + ".jpg"))
            {
                return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\userimages\\" + user.pin + ".jpg";
            }
            else
            {
                return "images\\user.png";
            }
        }

        public List<UserStatus> GetUserStatuses(User user)
        {
            return userStatusModel.GetByFK(user.IDName, user.id);
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

        public static Object[] GetDifference(int[] oldIds, int[] newIds)
        {
            List<int> toInsert = new List<int>();
            List<int> toDelete = new List<int>();
            List<int> toUpdate = new List<int>();
            toInsert.AddRange(newIds);
            toDelete.AddRange(oldIds);

            if (newIds.Length == 0)
            {
                toDelete.AddRange(oldIds);
            }
            else
            {
                foreach (int value1 in newIds)
                {
                    foreach (int value2 in oldIds)
                    {
                        if (value1 == value2)
                        {
                            toUpdate.Add(value1);
                            toInsert.Remove(value1);
                            toDelete.Remove(value1);
                        }
                    }
                }
            }
            return new Object[] { toUpdate, toInsert, toDelete };
        }

        public List<Object[]> GetClosestMeetings(int count)
        {
            DateTime today = DateTime.Today;
            DateTime now = DateTime.Now;

            List<Meeting> allMeetings = meetingModel.GetAll();
            //Meetings that will happen in the future
            List<Meeting> meetings = new List<Meeting>();

            List<Event> allEvents = eventModel.GetAll();
            List<Event> events = new List<Event>();
            //return if there is no meetings
            if(allMeetings == null || allMeetings.Count == 0)
            {
                return new List<Object[]>();
            }

            //filtering allEvents to events
            foreach (Event ev in allEvents)
            {
                if ((ev.endDate.Date >= today) || (ev.startDate.Date >= today && ev.intervalType == 2))
                {
                    events.Add(ev);
                }
            }

            //filtering allMeetings to meetings
            foreach (Meeting meeting in allMeetings)
            {
                if (((meeting.intervalType == 0 && meeting.startDatetime.Date >= now) || 
                    (meeting.intervalType != 0 && today <= meeting.endDate.Date) || 
                    (meeting.endDate == new DateTime() && meeting.intervalType != 0)) &&
                    meeting.isDeleted == false )
                    meetings.Add(meeting);
                else if((meeting.intervalType == 0 && meeting.startDatetime.Date < now)&& meeting.isDeleted ==false)
                {
                    List<ModifiedMeeting> mmeetings = modifiedMeetingModel.GetByFK(meeting.IDName, meeting.id);
                    bool does_exists = false;
                    foreach(ModifiedMeeting mmeeting in mmeetings)
                    {
                        if(mmeeting.startDatetime.Date == today.Date && mmeeting.startDatetime > now)
                        {
                            meetings.Add(mmeeting);
                        }
                    }
                }
            }

            //Dates and meetings of closest occuring meetings
            List<Object[]> closestDates = new List<Object[]>();

            //next dates of closestDats
            List<Object[]> nextDates = new List<Object[]>();

            //filling nextDates initially
            foreach (Meeting meeting in meetings)
            {
                Object[] obj = new Object[3];
                if (meeting.startDatetime < DateTime.Now)
                {
                    switch (meeting.intervalType)
                    {
                        case 0:
                            obj[0] = meeting.startDatetime;
                            break;
                        case 1:
                            obj[0] = meeting.startDatetime.AddDays(((today - meeting.startDatetime).Days / 7 + 1) * 7);
                            break;
                        case 2:
                            obj[0] = meeting.startDatetime.AddDays(((today - meeting.startDatetime).Days / 14 + 1) * 14);
                            break;
                        case 3:
                        case 4:
                        case 5:
                            obj[0] = meeting.startDatetime.AddMonths((today.Month - meeting.startDatetime.Month) + 12 * (today.Year - meeting.startDatetime.Year));
                            break;
                        case 6:
                            obj[0] = meeting.startDatetime.AddYears(today.Year - meeting.startDatetime.Year);
                            break;
                        case 7:
                            obj[0] = meeting.startDatetime.AddDays((today - meeting.startDatetime).Days);
                            break;
                    }
                }
                else obj[0] = meeting.startDatetime;
                obj[1] = meeting;
                bool is_event = false;
                //checking if nextOccurance is in any event.
                foreach (Event ev in events)
                {
                    DateTime sdt = ev.startDate;
                    DateTime edt = ev.endDate;
                    if (edt.Date <= ((DateTime)obj[0]).Date) 
                    {
                        switch (ev.intervalType)
                        {
                            case 0:
                                sdt = ev.startDate.AddYears((((DateTime)obj[0]).Year - ev.startDate.Year));
                                edt = ev.endDate.AddYears((((DateTime)obj[0]).Year - ev.endDate.Year));
                                break;
                            case 1:
                                sdt = ev.startDate.AddMonths((((DateTime)obj[0]).Month - ev.startDate.Month) + 12 * (((DateTime)obj[0]).Year - ev.startDate.Year));
                                edt = ev.endDate.AddMonths((((DateTime)obj[0]).Month - ev.endDate.Month) + 12 * (((DateTime)obj[0]).Year - ev.endDate.Year));
                                break;
                        }
                    }
                    if (((DateTime)obj[0]).Date >= sdt.Date && ((DateTime)obj[0]).Date <= edt.Date) is_event = true;

                }
                obj[2] = is_event;
                nextDates.Add(obj);
            }

            //sorting and cutting nextDates
            if (nextDates.Count > count) nextDates = nextDates.OrderBy(o => o[0]).ToList().GetRange(0, count);
            else nextDates = nextDates.OrderBy(o => (DateTime)o[0]).ToList();

            //if the array has length below count, fills it
            for( int step = 0; step<count && nextDates.Count >0 ;step++)
            {
                //adding nextDates into closest dates 
                for(int i=nextDates.Count-1;i>=0;i--)
                {
                    //not needed anymore
                    if(closestDates.Count >= count && (DateTime)nextDates[i][0] > (DateTime)closestDates[closestDates.Count-1][0])
                    {
                        nextDates.RemoveAt(i);
                        continue;
                    }
                    Object[] insertObj = new Object[3];
                    insertObj[0] = nextDates[i][0];
                    insertObj[1] = nextDates[i][1];
                    insertObj[2] = nextDates[i][2];
                    //if meeting is in event canceling it 
                    if ((bool)nextDates[i][2])
                    {
                        ((Meeting)insertObj[1]).duration = 0;
                    }
                    //inserting it to closestDates 
                    closestDates.Add(insertObj);
                    //sorting and cuttin closest dates
                    if (closestDates.Count > count) closestDates = closestDates.OrderBy(o => o[0]).ToList().GetRange(0, count);
                    else closestDates = closestDates.OrderBy( o => (DateTime)o[0] ).ToList();

                }

                //updating nextDates
                for (int i = nextDates.Count-1; i>=0;i--)
                {
                    //removing 1 time meetings
                    if(((Meeting)nextDates[i][1]).intervalType == 0)
                    {
                        nextDates.RemoveAt(i);
                        continue;
                    }

                    //finding next occurance of the meeting
                    DateTime nextOccurance = (DateTime)nextDates[i][0];
                    bool is_event = false;

                    switch(((Meeting)nextDates[i][1]).intervalType)
                    {
                        case 1:
                            nextOccurance = nextOccurance.AddDays(7);
                            break;
                        case 2:
                            nextOccurance = nextOccurance.AddDays(14);
                            break;
                        case 3:
                        case 4:
                        case 5:
                            nextOccurance = nextOccurance.AddMonths(1);
                            break;
                        case 6:
                            nextOccurance = nextOccurance.AddYears(1);
                            break;
                        case 7:
                            nextOccurance = nextOccurance.AddDays(((Meeting)nextDates[i][1]).intervalDay);
                            break;
                    }

                    //removing it from the nextDates if it is ended
                    if(((Meeting)nextDates[i][1]).endDate < nextOccurance  && ((Meeting)nextDates[i][1]).endDate != new DateTime())
                    {
                        nextDates.RemoveAt(i);
                        continue;
                    }

                    //checking if nextOccurance is in any event.
                    foreach(Event ev in events)
                    {
                        DateTime sdt = ev.startDate;
                        DateTime edt = ev.endDate;
                        if(edt.Date <= nextOccurance.Date)
                        {
                            switch (ev.intervalType)
                            {
                                case 0:
                                    sdt = ev.startDate.AddYears( (nextOccurance.Year - ev.startDate.Year) );
                                    edt = ev.endDate.AddYears( (nextOccurance.Year - ev.endDate.Year) );
                                    break;
                                case 1:
                                    sdt = ev.startDate.AddMonths((nextOccurance.Month - ev.startDate.Month) + 12 * (nextOccurance.Year - ev.startDate.Year));
                                    edt = ev.endDate.AddMonths((nextOccurance.Month - ev.endDate.Month) + 12 * (nextOccurance.Year - ev.endDate.Year));
                                    break;
                            }
                        }
                        if (nextOccurance.Date >= sdt.Date && nextOccurance.Date <= edt.Date) is_event = true;
                        
                    }

                    nextDates[i][0] = nextOccurance;
                    nextDates[i][2] = is_event;
                }

                //sorting and cutting nextDates
                if (nextDates.Count > count) nextDates = nextDates.OrderBy(o => o[0]).ToList().GetRange(0, count);
                else nextDates = nextDates.OrderBy(o => (DateTime)o[0]).ToList();
            }

            //Check if it is in Modified meeting
            for (int i = 0; i < closestDates.Count; i++)
            {
                List<ModifiedMeeting> mmeetings = modifiedMeetingModel.GetByFK( ((Meeting)closestDates[i][1]).IDName, ((Meeting)closestDates[i][1]).id);

                foreach(ModifiedMeeting mmeeting in mmeetings)
                {
                    if(mmeeting.startDatetime.Date == ((DateTime)closestDates[i][0]).Date)
                    {
                        //is_modified = true;

                        closestDates[i][1] = mmeeting;
                        break;
                    }
                }
                
            }
            
            return closestDates;
        }
    }
}
