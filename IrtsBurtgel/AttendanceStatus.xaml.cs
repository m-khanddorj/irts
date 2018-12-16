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
    /// Interaction logic for AttendanceStatus.xaml
    /// </summary>
    public partial class AttendanceStatus : Window
    {
        MeetingController meetingController;
        Dictionary<int, string> statuses;
        Dictionary<int, Label> statusNumberLabels;
        AttendanceList attendanceList;

        public AttendanceStatus(MeetingController mc, string msg = "")
        {
            InitializeComponent();
            statusNumberLabels = new Dictionary<int, Label>();
            meetingController = mc;
            statuses = new Dictionary<int, string>();
            statuses.Add(-1, "Нийт албан хаагч");
            statuses.Add(0, "Ирэх хүний тоо");
            Dictionary<int, string> dbstatuses = meetingController.statusModel.GetAll().ToDictionary(x => x.id, x => x.name);
            statuses = statuses.Concat(dbstatuses).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            if (meetingController.status == MeetingController.MEETING_STARTED)
            {
                PlaceStatuses(msg);
                Update();
            }
        }

        private void PlaceStatuses(string msg)
        {
            if (msg != "" && msg.Count() > 0)
            {
                Label msgLabel = new Label
                {
                    Content = msg,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                };
                ListBoxItem listboxitem = new ListBoxItem
                {
                    Content = msgLabel,
                    FontSize = 15,
                    FontWeight = FontWeights.Bold,
                    Background = Brushes.White
                };
                listbox.Items.Add(listboxitem);
            }
            foreach (KeyValuePair<int, string> status in statuses)
            {
                if (status.Key == 14)
                {
                    continue;
                }
                ListBoxItem listboxitem = new ListBoxItem();
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                Label statusNameLabel = new Label
                {
                    Content = status.Value + ":",
                    Foreground = Brushes.White,
                    FontSize = 15,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                Label statusNumberLabel = new Label
                {
                    Content = "0",
                    Foreground = Brushes.White,
                    FontSize = 15,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                statusNumberLabels.Add(status.Key, statusNumberLabel);
                var converter = new BrushConverter();
                Button showMore = new Button
                {
                    Background = (Brush)converter.ConvertFromString("#FF007ACC"),
                    Foreground = Brushes.Gray,
                };
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri("images/eye.PNG", UriKind.Relative);
                bi3.EndInit();
                Image img = new Image
                {
                    Source = bi3,
                    Width = 20,
                    Height = 20
                };
                showMore.Content = img;
                showMore.Tag = status.Key;
                showMore.Click += showMoreClicked;
                Grid.SetColumn(statusNameLabel, 0);
                Grid.SetColumn(statusNumberLabel, 1);
                Grid.SetColumn(showMore, 2);
                Grid.SetRow(statusNameLabel, 0);
                Grid.SetRow(statusNumberLabel, 0);
                Grid.SetRow(showMore, 0);
                grid.Children.Add(statusNameLabel);
                grid.Children.Add(statusNumberLabel);
                grid.Children.Add(showMore);
                listboxitem.Content = grid;
                listbox.Items.Add(listboxitem);
            }
        }

        public void Update()
        {
            if(meetingController.status != MeetingController.MEETING_STARTED)
            {
                return;
            }
            List<object[]> userAttendances = meetingController.onGoingMeetingUserAttendance;
            Dictionary<int, string> statuses = meetingController.statusModel.GetAll().ToDictionary(x => x.id, x => x.name);
            Dictionary<int, int> userStatusCount = new Dictionary<int, int>();
            if (userAttendances == null || userAttendances.Count == 0)
            {
                return;
            }
            foreach (KeyValuePair<int, string> status in statuses)
            {
                if (status.Key == 14)
                {
                    continue;
                }
                userStatusCount.Add(status.Key, 0);
            }
            int total = 0;
            foreach (object[] userAttendance in userAttendances)
            {
                Attendance attendance = (Attendance)userAttendance[1];
                if (attendance.statusId == 14)
                {
                    continue;
                }
                userStatusCount[attendance.statusId]++;
                if (attendance.statusId == 1 || attendance.statusId == 2 || attendance.statusId == 15)
                {
                    total++;
                }
            }
            userStatusCount[-1] = userAttendances.Count;
            userStatusCount[0] = total;
            foreach (KeyValuePair<int, int> statusCount in userStatusCount)
            {
                statusNumberLabels[statusCount.Key].Content = statusCount.Value.ToString() + " (" + Math.Round((double)statusCount.Value / userAttendances.Count * 100) + "%)";
            }
        }

        private void showMoreClicked(object sender, RoutedEventArgs e)
        {
            if(meetingController.status == MeetingController.MEETING_STARTED)
                ShowAttendanceList((int)((Button)sender).Tag);
        }

        private void ShowAttendanceList(int statusId)
        {
            if(attendanceList != null)
            {
                attendanceList.Close();
            }
            attendanceList = new AttendanceList(meetingController, statusId);
            attendanceList.Owner = this;
            attendanceList.Visibility = Visibility.Visible;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            Owner = null;
        }
    }
}
