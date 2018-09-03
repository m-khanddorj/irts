using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        Dictionary<int, TextBlock> departmentAttendance;
        Dictionary<int, Grid> userGrids;
        Dictionary<int, int[]> last;
        public Dictionary<int, DepartmentStatus> departmentStatusWindows;
        int status = 0;
        object[] ongoingObj;

        //dummy data
        public MeetingStatus(MeetingController mc)
        {
            InitializeComponent();
            adminModel = new Model<Admin>();

            meetingController = mc;
            statuses = meetingController.statusModel.GetAll().ToDictionary(x => x.id, x => x.name);
            departments = meetingController.departmentModel.GetAll().ToDictionary(x => x.id, x => x.name);
            departments.Add(-1, "Хэлтэсгүй");
            departmentWrapPanels = new Dictionary<int, WrapPanel>();
            departmentAttendance = new Dictionary<int, TextBlock>();
            departmentStatusWindows = new Dictionary<int, DepartmentStatus>();

            userGrids = new Dictionary<int, Grid>();
            last = new Dictionary<int, int[]>();
            try
            {
                ongoingObj = meetingController.GetClosestMeetings(1)[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Хурлын цонх нээхэд алдаа гарлаа. Алдааны мессеж: " + ex.Message);
            }
            if (meetingController.status == MeetingController.MEETING_STARTED)
            {
                status = 1;
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

                Border border1 = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1, 1, 1, 1),
                    Margin = new Thickness(10, 10, 10, 10)
                };

                Grid DynamicGrid = new Grid
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top,
                    Background = new SolidColorBrush(Colors.White)
                };

                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
                DynamicGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
                DynamicGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                border1.Child = DynamicGrid;

                Border border = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(0, 0, 1, 1)
                };
                var converter = new BrushConverter();
                border.Child = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = entry.Value,
                    FontWeight = FontWeights.Bold,
                    FontSize = 17,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    TextAlignment = TextAlignment.Center,
                    Background = (Brush)converter.ConvertFromString("#FF007ACC"),
                };

                border.PreviewMouseUp += (e, o) =>
                {
                    ShowDepartmentStatus(entry.Key);
                    Console.WriteLine("Clicked textbox");
                };

                Border border2 = new Border { BorderBrush = Brushes.Gray, BorderThickness = new Thickness(0, 0, 0, 1) };
                border2.Child = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,   
                    Text = "0/0",
                    Margin = new Thickness(5, 5, 5, 5),
                    FontWeight = FontWeights.Bold
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
                Grid.SetRow(border2, 0);
                Grid.SetColumn(border2, 1);
                DynamicGrid.Children.Add(border2);

                Grid.SetRow(sv, 1);
                Grid.SetColumn(sv, 0);
                Grid.SetColumnSpan(sv, 2);
                DynamicGrid.Children.Add(sv);

                Grid.SetRow(border1, count / 5);
                Grid.SetColumn(border1, count % 5);
                gridDeparts.Children.Add(border1);
                DynamicGrid.Name = "dynamicGrid" + count;

                departmentWrapPanels.Add(entry.Key, wp);
                departmentAttendance.Add(entry.Key, (TextBlock)border2.Child);

                count++;
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

                last = new Dictionary<int, int[]>();
                foreach (KeyValuePair<int, string> entry in departments)
                {
                    last.Add(entry.Key, new int[4]);
                }

                ArchivedMeeting archivedMeeting = meetingController.archivedMeetingModel.Get(((Attendance)(userAttendances.First()[1])).archivedMeetingId);
                meetingName.Text = archivedMeeting.name;
                meetingDate.Text = archivedMeeting.meetingDatetime.ToString("yyyy/MM/dd HH:mm");
                foreach (object[] obj in userAttendances)
                {
                    User user = (User)obj[0];
                    Attendance att = (Attendance)obj[1];

                    Grid DynamicGrid = new Grid
                    {
                        Background = new SolidColorBrush(Colors.White),
                        Width = 60
                    };

                    Border border = new Border { BorderThickness = new Thickness(1, 1, 1, 0), Margin = new Thickness(5, 5, 5, 5)};
                    border.Child = DynamicGrid;

                    DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    DynamicGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) });
                    DynamicGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
                    DynamicGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });

                    Image image = new Image { Source = new BitmapImage(new Uri(meetingController.GetUserImage(user))), HorizontalAlignment = HorizontalAlignment.Center };
                    Label name = new Label { Content = user.fname + " " + user.lname, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 15 };
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
                        departmentWrapPanels[user.departmentId].Children.Insert(last[user.departmentId][0] + last[user.departmentId][1] + last[user.departmentId][2], border);
                        last[user.departmentId][3]++;
                    }
                    else if (att.statusId == 2)
                    {
                        departmentWrapPanels[user.departmentId].Children.Insert(last[user.departmentId][0], border);
                        last[user.departmentId][1]++;
                    }
                    else if (att.statusId == 15)
                    {
                        departmentWrapPanels[user.departmentId].Children.Insert(0, border);
                        last[user.departmentId][0]++;
                    }
                    else
                    {
                        departmentWrapPanels[user.departmentId].Children.Insert(last[user.departmentId][0] + last[user.departmentId][1], border);
                        last[user.departmentId][2]++;
                    }
                    userGrids.Add(user.id, DynamicGrid);
                }

                foreach (KeyValuePair<int, string> entry in departments)
                {
                    if (!last.ContainsKey(entry.Key))
                    {
                        last.Add(entry.Key, new int[4]);
                    }
                    departmentAttendance[entry.Key].Text = "Ирц-" + last[entry.Key][1] + "/" + (last[entry.Key][0] + last[entry.Key][1] + last[entry.Key][3]) + "\nХ-" + last[entry.Key][1] + " Ч-" + last[entry.Key][2];
                }

                AttendanceLabel.Content = "Ирц: " + last.Sum(x => x.Value[1] + x.Value[3]) + "/" + last.Sum(x => x.Value[0] + x.Value[1] + x.Value[3]) + "\nЧөлөөтэй: " + last.Sum(x => x.Value[2]);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Хурлын цонх нээхэд алдаа гарлаа. Алдааны мессеж: " + ex.Message);
            }
        }

        public void UpdateCountDown(object[] obj)
        {
            if (meetingController.status == MeetingController.MEETING_STARTED && status == 0)
            {
                status = 1;
                ongoingObj = obj;
                BuildDepartControls();
                PlaceUsers(meetingController.onGoingMeetingUserAttendance);
            }
            else if (meetingController.status == MeetingController.IDLE && status == 1)
            {
                status = 0;
                ongoingObj = obj;
            }
            else if (meetingController.status == MeetingController.IDLE && status == 0)
            {
                ongoingObj = obj;
            }
            
            string texttodisplay = meetingController.TextToDisplay().Replace("\n", " ");
            currentTime.Text = texttodisplay;
        }

        public void Update(object[] identifiedUserAttendance, int oldStatusId = 15)
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

                departmentWrapPanels[user.departmentId].Children.Remove((Border)userGrids[user.id].Parent);
                switch (oldStatusId)
                {
                    case 1: last[user.departmentId][3]--; break;
                    case 2: last[user.departmentId][1]--; break;
                    case 15: last[user.departmentId][0]--; break;
                    default: last[user.departmentId][2]--; break;
                }

                if (attendance.statusId == 1)
                {
                    departmentWrapPanels[user.departmentId].Children.Insert(last[user.departmentId][0] + last[user.departmentId][1] + last[user.departmentId][2], (Border)userGrids[user.id].Parent);
                    last[user.departmentId][3]++;
                }
                else if (attendance.statusId == 2)
                {
                    departmentWrapPanels[user.departmentId].Children.Insert(last[user.departmentId][0], (Border)userGrids[user.id].Parent);
                    last[user.departmentId][1]++;
                }
                else if (attendance.statusId == 15)
                {
                    departmentWrapPanels[user.departmentId].Children.Insert(0, (Border)userGrids[user.id].Parent);
                    last[user.departmentId][0]++;
                }
                else
                {
                    departmentWrapPanels[user.departmentId].Children.Insert(last[user.departmentId][0] + last[user.departmentId][1], (Border)userGrids[user.id].Parent);
                    last[user.departmentId][2]++;
                }

                foreach (KeyValuePair<int, DepartmentStatus> entry in departmentStatusWindows)
                {
                    if(entry.Value != null && entry.Value.IsLoaded && entry.Value.IsActive)
                    {
                        entry.Value.Update(identifiedUserAttendance, oldStatusId);
                    }
                }
            }
            foreach (KeyValuePair<int, string> entry in departments)
            {
                departmentAttendance[entry.Key].Text = "Ирц-" + last[entry.Key][1] + "/" + (last[entry.Key][0] + last[entry.Key][1] + last[entry.Key][3]) + "\nХ-" + last[entry.Key][1] + " Ч-" + last[entry.Key][2];
            }
            AttendanceLabel.Content = "Ирц: " + last.Sum(x => x.Value[1] + x.Value[3]) + "/" + last.Sum(x => x.Value[0] + x.Value[1] + x.Value[3]) + "\nЧөлөөтэй: " + last.Sum(x => x.Value[2]);
        }

        public void ShowDepartmentStatus(int departmentId)
        {
            if (departmentStatusWindows.ContainsKey(departmentId))
            {
                if (departmentStatusWindows[departmentId].IsLoaded)
                {
                    departmentStatusWindows[departmentId].Focus();
                    departmentStatusWindows[departmentId].Visibility = Visibility.Visible;
                }
                else
                {
                    departmentStatusWindows[departmentId] = new DepartmentStatus(meetingController, departmentId);
                    departmentStatusWindows[departmentId].Owner = this;
                    departmentStatusWindows[departmentId].Visibility = Visibility.Visible;
                }
            }
            else
            {
                departmentStatusWindows.Add(departmentId, new DepartmentStatus(meetingController, departmentId));
                departmentStatusWindows[departmentId].Owner = this;
                departmentStatusWindows[departmentId].Visibility = Visibility.Visible;
            }
        }
    }
}
