using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for MeetingStatus.xaml
    /// </summary>
    public partial class MeetingStatus : Window
    {
        Model<Admin> adminModel;
        MeetingController meetingController;
        Dictionary<int, string> statuses;
        Dictionary<int, string> departments;
        Dictionary<int, WrapPanel> departmentWrapPanels;
        Dictionary<int, Grid> userGrids;
        Dictionary<int, int[]> last;

        //dummy data
        public MeetingStatus(MeetingController mc)
        {
            InitializeComponent();
            adminModel = new Model<Admin>();
            //Showing the org title
            OrgTitle.Content = adminModel.GetAll()[0].organizationName;

            meetingController = mc;
            statuses = meetingController.statusModel.GetAll().ToDictionary(x => x.id, x => x.name);
            departments = meetingController.departmentModel.GetAll().ToDictionary(x => x.id, x => x.name);
            departments.Add(-1, "Хэлтэсгүй");
            departmentWrapPanels = new Dictionary<int, WrapPanel>();
            userGrids = new Dictionary<int, Grid>(); 
            last = new Dictionary<int, int[]>();
            if (meetingController.status == MeetingController.MEETING_STARTED)
            {
                BuildDepartControls();
                PlaceUsers(meetingController.onGoingMeetingUserAttendance);
            }
        }

        private void Expand(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized && this.WindowStyle != WindowStyle.None)
            {
                fullScreenToggleImage.Source = new BitmapImage(new Uri("images/shrink.png", UriKind.Relative));
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
            else if (this.WindowState == WindowState.Maximized && this.WindowStyle != WindowStyle.None)
            {
                fullScreenToggleImage.Source = new BitmapImage(new Uri("images/shrink.png", UriKind.Relative));
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                fullScreenToggleImage.Source = new BitmapImage(new Uri("images/expand.png", UriKind.Relative));
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.ThreeDBorderWindow;
            }
        }

        public void BuildDepartControls()
        {
            gridDeparts.Children.Clear();

            for (int i = 0; i < 5; i++)
            {
                gridDeparts.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
            int count = 0;
            foreach (KeyValuePair<int, string> entry in departments)
            {
                if (count % 5 == 0)
                {
                    gridDeparts.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                }

                Border border1 = new Border { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1, 1, 1, 1) };

                Grid DynamicGrid = new Grid
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top,
                    ShowGridLines = true
                };

                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                DynamicGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
                DynamicGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                Border border = new Border { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1, 1, 1, 1) };
                border.Child = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = entry.Value,
                    Margin = new Thickness(5, 5, 5, 5)
                };

                WrapPanel wp = new WrapPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Orientation = Orientation.Horizontal
                };

                ScrollViewer sv = new ScrollViewer
                {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Name = "scroll" + count
                };
                sv.Content = wp;

                Grid.SetRow(border, 0);
                Grid.SetColumn(border, 0);
                DynamicGrid.Children.Add(border);

                Grid.SetRow(sv, 1);
                Grid.SetColumn(sv, 0);
                DynamicGrid.Children.Add(sv);

                Grid.SetRow(DynamicGrid, count / 5);
                Grid.SetColumn(DynamicGrid, count % 5);
                gridDeparts.Children.Add(DynamicGrid);
                DynamicGrid.Name = "dynamicGrid" + count;

                departmentWrapPanels.Add(entry.Key, wp);

                count++;
            }
        }

        public void PlaceUsers(List<object[]> userAttendances)
        {
            last = new Dictionary<int, int[]>();
            foreach (object[] obj in userAttendances)
            {
                User user = (User)obj[0];
                Attendance att = (Attendance)obj[1];

                Grid DynamicGrid = new Grid
                {
                    Background = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(10, 10, 10, 10),
                    Width = 100
                };

                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                DynamicGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
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
                    case 1: status.Background = Brushes.DarkGreen; break;
                    case 2: status.Background = Brushes.DarkOrange; break;
                    case 15: status.Background = Brushes.DarkRed; break;
                    default: status.Background = Brushes.DarkTurquoise; break;
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

                if (!last.ContainsKey(user.departmentId))
                {
                    last.Add(user.departmentId, new int[4]);
                }

                if (att.statusId == 1)
                {
                    departmentWrapPanels[user.departmentId].Children.Insert(last[user.departmentId][0] + last[user.departmentId][1] + last[user.departmentId][2], DynamicGrid);
                    last[user.departmentId][1]++;
                }
                else if (att.statusId == 2)
                {
                    departmentWrapPanels[user.departmentId].Children.Insert(last[user.departmentId][0], DynamicGrid);
                    last[user.departmentId][1]++; 
                }
                else if (att.statusId == 15)
                {
                    departmentWrapPanels[user.departmentId].Children.Insert(0, DynamicGrid);
                    last[user.departmentId][0]++;
                }
                else
                {
                    departmentWrapPanels[user.departmentId].Children.Insert(last[user.departmentId][0] + last[user.departmentId][1], DynamicGrid);
                    last[user.departmentId][2]++;
                }
                userGrids.Add(user.id, DynamicGrid);
            }

            AttendanceLabel.Content = last.Sum(x => x.Value[1] + x.Value[3]) + "/" + last.Sum(x => x.Value[0] + x.Value[1] + x.Value[2] + x.Value[3]);
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
                    case 1: ((Label)userGrids[user.id].Children[2]).Background = Brushes.DarkGreen; break;
                    case 2: ((Label)userGrids[user.id].Children[2]).Background = Brushes.DarkOrange; break;
                    case 15: ((Label)userGrids[user.id].Children[2]).Background = Brushes.DarkRed; break;
                    default: ((Label)userGrids[user.id].Children[2]).Background = Brushes.DarkTurquoise; break;
                }

                departmentWrapPanels[user.departmentId].Children.Remove(userGrids[user.id]);
                last[user.departmentId][0]--;

                if (attendance.statusId == 1)
                {
                    departmentWrapPanels[user.departmentId].Children.Insert(last[user.departmentId][0] + last[user.departmentId][1] + last[user.departmentId][2], userGrids[user.id]);
                }
                else if (attendance.statusId == 2)
                {
                    departmentWrapPanels[user.departmentId].Children.Insert(last[user.departmentId][0], userGrids[user.id]);
                    last[user.departmentId][1]++;
                }
                else if (attendance.statusId == 15)
                {
                    departmentWrapPanels[user.departmentId].Children.Insert(0, userGrids[user.id]);
                    last[user.departmentId][0]++;
                }
                else
                {
                    departmentWrapPanels[user.departmentId].Children.Insert(last[user.departmentId][0] + last[user.departmentId][1], userGrids[user.id]);
                    last[user.departmentId][2]++;
                }
            }
            AttendanceLabel.Content = last.Sum(x => x.Value[1] + x.Value[3]) + "/" + last.Sum(x => x.Value[0] + x.Value[1] + x.Value[2] + x.Value[3]);
        }
    }
}
