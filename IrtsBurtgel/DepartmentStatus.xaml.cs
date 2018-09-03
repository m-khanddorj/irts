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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DepartmentStatus : Window
    {
        MeetingController meetingController;
        Dictionary<int, string> statuses;
        Dictionary<int, string> departments;
        Dictionary<int, Grid> userGrids;
        int[] last;
        int status = 0;

        public DepartmentStatus(MeetingController mc)
        {
            InitializeComponent();

            meetingController = mc;
            statuses = meetingController.statusModel.GetAll().ToDictionary(x => x.id, x => x.name);
            departments = meetingController.departmentModel.GetAll().ToDictionary(x => x.id, x => x.name);
            departments.Add(-1, "Хэлтэсгүй");
            userGrids = new Dictionary<int, Grid>();
            last = new int[4];
            if (meetingController.status == MeetingController.MEETING_STARTED)
            {
                status = 1;
                PlaceUsers(meetingController.onGoingMeetingUserAttendance);
            }
        }

        public void PlaceUsers(List<object[]> userAttendances)
        {
            try
            {
                if (userAttendances == null || userAttendances.Count == 0)
                {
                    return;
                }

                last = new int[4];

                ArchivedMeeting archivedMeeting = meetingController.archivedMeetingModel.Get(((Attendance)(userAttendances.First()[1])).archivedMeetingId);
                foreach (object[] obj in userAttendances)
                {
                    User user = (User)obj[0];
                    Attendance att = (Attendance)obj[1];

                    Grid DynamicGrid = new Grid
                    {
                        Background = new SolidColorBrush(Colors.White),
                        Width = 60
                    };

                    Border border = new Border { BorderThickness = new Thickness(1, 1, 1, 0), Margin = new Thickness(5, 5, 5, 5), CornerRadius = new CornerRadius(10, 10, 0, 0) };
                    border.Child = DynamicGrid;

                    DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    DynamicGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80) });
                    DynamicGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
                    DynamicGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });

                    Image image = new Image { Source = new BitmapImage(new Uri(meetingController.GetUserImage(user))), HorizontalAlignment = HorizontalAlignment.Center };
                    Label name = new Label { Content = user.fname + " " + user.lname, HorizontalAlignment = HorizontalAlignment.Center };
                    Label status = new Label
                    {
                        Content = att.statusId == 2 ? statuses[att.statusId] + " (" + att.regTime + ")" : statuses[att.statusId],
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        FontWeight = FontWeights.Bold
                    };

                    switch (att.statusId)
                    {
                        case 1: border.BorderBrush = status.Background = Brushes.DarkGreen; break;
                        case 2: border.BorderBrush = status.Background = Brushes.DarkOrange; break;
                        case 15: border.BorderBrush = status.Background = Brushes.DarkRed; break;
                        default: border.BorderBrush = status.Background = Brushes.DarkTurquoise; break;
                    }

                    Grid.SetColumn(image, 0);
                    Grid.SetColumn(name, 0);
                    Grid.SetColumn(status, 0);
                    Grid.SetRow(image, 0);
                    Grid.SetRow(name, 1);
                    Grid.SetRow(status, 2);
                    DynamicGrid.Children.Add(image);
                    DynamicGrid.Children.Add(name);
                    DynamicGrid.Children.Add(status);

                    if (att.statusId == 1)
                    {
                        depAttWrapPanel.Children.Insert(last[0] + last[1] + last[2], border);
                        last[3]++;
                    }
                    else if (att.statusId == 2)
                    {
                        depAttWrapPanel.Children.Insert(last[0], border);
                        last[1]++;
                    }
                    else if (att.statusId == 15)
                    {
                        depAttWrapPanel.Children.Insert(0, border);
                        last[0]++;
                    }
                    else
                    {
                        depAttWrapPanel.Children.Insert(last[0] + last[1], border);
                        last[2]++;
                    }
                    userGrids.Add(user.id, DynamicGrid);
                }

                depAttTextBox.Text = "Ирц-" + last[1] + "/" + (last[0] + last[1] + last[3]) + "\nХ-" + last[1] + " Ч-" + last[2];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Хурлын цонх нээхэд алдаа гарлаа. Алдааны мессеж: " + ex.Message);
            }
        }

        public void Update(object[] identifiedUserAttendance)
        {
            User user = (User)identifiedUserAttendance[0];
            Attendance attendance = (Attendance)identifiedUserAttendance[1];
            if (meetingController.status == MeetingController.MEETING_STARTED)
            {
                ((Label)userGrids[user.id].Children[2]).Content = attendance.statusId == 2 ? statuses[attendance.statusId] + " (" + attendance.regTime + ")" : statuses[attendance.statusId];

                switch (attendance.statusId)
                {
                    case 1: ((Border)userGrids[user.id].Parent).BorderBrush = ((Label)userGrids[user.id].Children[2]).Background = Brushes.DarkGreen; break;
                    case 2: ((Border)userGrids[user.id].Parent).BorderBrush = ((Label)userGrids[user.id].Children[2]).Background = Brushes.DarkOrange; break;
                    case 15: ((Border)userGrids[user.id].Parent).BorderBrush = ((Label)userGrids[user.id].Children[2]).Background = Brushes.DarkRed; break;
                    default: ((Border)userGrids[user.id].Parent).BorderBrush = ((Label)userGrids[user.id].Children[2]).Background = Brushes.DarkTurquoise; break;
                }

                depAttWrapPanel.Children.Remove((Border)userGrids[user.id].Parent);
                last[0]--;

                if (attendance.statusId == 1)
                {
                    depAttWrapPanel.Children.Insert(last[0] + last[1] + last[2], (Border)userGrids[user.id].Parent);
                    last[3]++;
                    last[0]--;
                }
                else if (attendance.statusId == 2)
                {
                    depAttWrapPanel.Children.Insert(last[0], (Border)userGrids[user.id].Parent);
                    last[1]++;
                    last[0]--;
                }
                else if (attendance.statusId == 15)
                {
                    depAttWrapPanel.Children.Insert(0, (Border)userGrids[user.id].Parent);
                    last[0]++;
                }
                else
                {
                    depAttWrapPanel.Children.Insert(last[0] + last[1], (Border)userGrids[user.id].Parent);
                    last[2]++;
                    last[0]--;
                }
            }
            depAttTextBox.Text = "Ирц-" + last[1] + "/" + (last[0] + last[1] + last[3]) + "\nХ-" + last[1] + " Ч-" + last[2];
        }
    }
}
