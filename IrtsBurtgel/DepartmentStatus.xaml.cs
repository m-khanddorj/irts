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
    /// Interaction logic for DepartmentStatus.xaml
    /// </summary>
    public partial class DepartmentStatus : Window
    {
        MeetingController meetingController;
        Dictionary<int, string> statuses;
        Dictionary<int, string> departments;
        Dictionary<int, Grid> userGrids;
        int departmentId;
        int[] last;
        int status = 0;

        public DepartmentStatus(MeetingController mc, int depid)
        {
            InitializeComponent();

            meetingController = mc;
            statuses = meetingController.statusModel.GetAll().ToDictionary(x => x.id, x => x.name);
            departments = meetingController.departmentModel.GetAll().ToDictionary(x => x.id, x => x.name);
            departments.Add(-1, "Хэлтэсгүй");
            userGrids = new Dictionary<int, Grid>();
            last = new int[4];
            departmentId = depid;
            if (meetingController.status == MeetingController.MEETING_STARTED)
            {
                depNameTextBox.Text = departments[departmentId];
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

                    if (user.departmentId != departmentId)
                    {
                        continue;
                    }

                    Grid DynamicGrid = new Grid
                    {
                        Background = new SolidColorBrush(Colors.White),
                        Width = 120
                    };

                    Border border = new Border { BorderThickness = new Thickness(1, 1, 1, 0), Margin = new Thickness(5, 5, 5, 5) };
                    border.Child = DynamicGrid;

                    DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    DynamicGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100) });
                    DynamicGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
                    DynamicGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });

                    BitmapImage webImage;
                    try
                    {
                        webImage = new BitmapImage(new Uri(meetingController.GetUserImage(user)));
                    }
                    catch (Exception ex)
                    {
                        webImage = new BitmapImage(new Uri("./images/user.png", UriKind.Relative));
                    }
                    float scaleHeight = (float)120 / (float)webImage.Height;
                    float scaleWidth = (float)100 / (float)webImage.Width;
                    float scale = Math.Max(scaleHeight, scaleWidth);

                    Image image = new Image
                    {
                        Source = webImage,
                        Height = (int)(webImage.Width * scale),
                        Width = (int)(webImage.Height * scale)
                    };
                    
                    Label name = new Label {
                        Content = user.fname + " " + user.lname,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontSize = 17
                    };

                    ComboBox combobox = new ComboBox
                    {
                        FontWeight = FontWeights.Bold,
                        FontSize = 17
                    };

                    foreach(KeyValuePair<int, string> entry in statuses)
                    {
                        if (entry.Key == 14)
                        {
                            continue;
                        }

                        ComboBoxItem comboboxItem = new ComboBoxItem
                        {
                            Content = entry.Key == 2 ? statuses[entry.Key] + " (0)" : entry.Value,
                            Foreground = Brushes.Black,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            FontWeight = FontWeights.Bold,
                            FontSize = 15
                        };

                        comboboxItem.Tag = entry.Key;

                        switch (entry.Key)
                        {
                            case 1: comboboxItem.Background = Brushes.LightGreen; break;
                            case 2: comboboxItem.Background = Brushes.LightSalmon; break;
                            case 15: comboboxItem.Background = Brushes.LightCoral; break;
                            default: comboboxItem.Background = Brushes.LightBlue; break;
                        }

                        combobox.Items.Add(comboboxItem);
                        if (entry.Key == att.statusId)
                        {
                            combobox.SelectedItem = comboboxItem;

                            switch (entry.Key)
                            {
                                case 1: border.BorderBrush = Brushes.DarkGreen; break;
                                case 2: border.BorderBrush = Brushes.DarkOrange; break;
                                case 15: border.BorderBrush = Brushes.DarkRed; break;
                                default: border.BorderBrush = Brushes.DarkSlateBlue; break;
                            }
                        }
                    }

                    combobox.SelectionChanged += (e, o) =>
                    {
                        ChangeUserAttendance(obj, e);
                    };

                    Grid.SetColumn(image, 0);
                    Grid.SetColumn(name, 0);
                    Grid.SetColumn(combobox, 0);
                    Grid.SetRow(image, 0);
                    Grid.SetRow(name, 1);
                    Grid.SetRow(combobox, 2);
                    DynamicGrid.Children.Add(image);
                    DynamicGrid.Children.Add(name);
                    DynamicGrid.Children.Add(combobox);

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

                depAttTextBox1.Text = (last[0] + last[1] + last[3]).ToString();
                depAttTextBox2.Text = (last[3]).ToString();
                depAttTextBox3.Text = (last[1]).ToString();
                depAttTextBox4.Text = (last[0]).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Хурлын цонх нээхэд алдаа гарлаа. Алдааны мессеж: " + ex.ToString());
            }
        }

        public void Update(object[] identifiedUserAttendance, int oldstatusid = 15)
        {
            User user = (User)identifiedUserAttendance[0];
            Attendance attendance = (Attendance)identifiedUserAttendance[1];
            if (user.departmentId != departmentId)
            {
                return;
            }

            if (meetingController.status == MeetingController.MEETING_STARTED)
            {
                switch (attendance.statusId)
                {
                    case 1: ((Border)userGrids[user.id].Parent).BorderBrush = Brushes.DarkGreen; break;
                    case 2: ((Border)userGrids[user.id].Parent).BorderBrush = Brushes.DarkOrange; break;
                    case 15: ((Border)userGrids[user.id].Parent).BorderBrush = Brushes.DarkRed; break;
                    default: ((Border)userGrids[user.id].Parent).BorderBrush = Brushes.DarkSlateBlue; break;
                }

                ComboBox combobox = ((ComboBox)userGrids[user.id].Children[2]);
                foreach (ComboBoxItem cbi in combobox.Items)
                {
                    if ((int)(cbi.Tag) == attendance.statusId)
                    {
                        combobox.SelectedItem = cbi;
                        break;
                    }
                }

                depAttWrapPanel.Children.Remove((Border)userGrids[user.id].Parent);
                switch (oldstatusid)
                {
                    case 1: last[3]--; break;
                    case 2: last[1]--; break;
                    case 15: last[0]--; break;
                    default: last[2]--; break;
                }

                if (attendance.statusId == 1)
                {
                    depAttWrapPanel.Children.Insert(last[0] + last[1] + last[2], (Border)userGrids[user.id].Parent);
                    last[3]++;
                }
                else if (attendance.statusId == 2)
                {
                    depAttWrapPanel.Children.Insert(last[0], (Border)userGrids[user.id].Parent);
                    last[1]++;
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
                }
            }

            depAttTextBox1.Text = (last[0] + last[1] + last[3]).ToString();
            depAttTextBox2.Text = (last[3]).ToString();
            depAttTextBox3.Text = (last[1]).ToString();
            depAttTextBox4.Text = (last[0]).ToString();
        }

        public void ChangeUserAttendance(object[] obj, object sender)
        {
            ComboBox comboBox = (ComboBox)sender;
            User user = (User)obj[0];
            Attendance attendance = (Attendance)obj[1];
            int oldstatus = attendance.statusId;
            attendance.statusId = (int)((ComboBoxItem)(comboBox.SelectedItem)).Tag;
            switch (attendance.statusId)
            {
                case 1: ((Border)userGrids[user.id].Parent).BorderBrush = Brushes.DarkGreen; break;
                case 2: ((Border)userGrids[user.id].Parent).BorderBrush = Brushes.DarkOrange; attendance.regTime = attendance.regTime < 0 ? 0:attendance.regTime; break;
                case 15: ((Border)userGrids[user.id].Parent).BorderBrush = Brushes.DarkRed; break;
                default: ((Border)userGrids[user.id].Parent).BorderBrush = Brushes.DarkSlateBlue; break;
            }

            meetingController.attendanceModel.Set(attendance);

            this.Dispatcher.Invoke(() =>
            {
                foreach (MeetingStatus ms in meetingController.mainWindow.meetingStatusWindows)
                {
                    if (ms.IsLoaded)
                    {
                        ms.Update(obj, oldstatus);
                    }
                }
            });
        }
    }
}
