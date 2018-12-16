using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IrtsBurtgel
{
    /// <summary>
    /// Interaction logic for AttendanceList.xaml
    /// </summary>
    public partial class AttendanceList : Window
    {
        MeetingController meetingController;
        Dictionary<int, string> departments;
        Dictionary<int, string> statuses;

        public AttendanceList(MeetingController mc, int statusId)
        {
            InitializeComponent();
            meetingController = mc;
            if(meetingController.status == MeetingController.MEETING_STARTED)
            {
                departments = meetingController.departmentModel.GetAll().ToDictionary(x => x.id, x => x.name);
                statuses = meetingController.statusModel.GetAll().ToDictionary(x => x.id, x => x.name);
                if(statusId == -1)
                {
                    this.Title = "Хуралд оролцох нийт хүмүүсийн жагсаалт";
                }
                else if(statusId == 0)
                {
                    this.Title = "Хурал оролцох боломжтой хүмүүсийн жагсаалт";
                }
                else
                {
                    this.Title = "Хуралд \'" + statuses[statusId] + "\' төлөвтэй байгаа хүмүүсийн жагсаалт";
                }
                PlaceUsers(statusId);
            }
        }
        
        private void PlaceUsers(int statusId)
        {
            int index = 1;
            List<object[]> userAttendances = meetingController.onGoingMeetingUserAttendance;
            for (int i = userAttendances.Count - 1; i >= 0; i--)
            {
                object[] userAttendance = userAttendances[i];
                User user = (User)userAttendance[0];
                Attendance attendance = (Attendance)userAttendance[1];
                if(statusId == 0 && (attendance.statusId == 1 || attendance.statusId == 2 || attendance.statusId == 14 || attendance.statusId == 15))
                {
                    addUser(userAttendance, index);
                    index++;
                }
                else if(statusId == -1)
                {
                    addUser(userAttendance, index);
                    index++;
                }
                else if(statusId == attendance.statusId)
                {
                    addUser(userAttendance, index);
                    index++;
                }
            }
        }

        private void addUser(object[] userAttendance, int index)
        {
            User user = (User)userAttendance[0];
            Attendance attendance = (Attendance)userAttendance[1];
            ListBoxItem listboxitem = new ListBoxItem();
            Label label = new Label
            {
                Content = index.ToString() + ". " + user.lname + " " + user.fname + ", " + (user.departmentId != -1 ? departments[user.departmentId] : "Хэлтэсгүй"),
                FontSize = 18
            };
            listboxitem.Content = label;
            listbox.Items.Add(listboxitem);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            Owner = null;
        }
    }
}
