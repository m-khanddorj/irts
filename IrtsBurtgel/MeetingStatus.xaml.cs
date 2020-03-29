using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms.DataVisualization.Charting;
using System.Net.Cache;

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
        Dictionary<int, TextBlock[]> departmentAttendance;
        Dictionary<int, System.Windows.Controls.Grid> userGrids;
        Dictionary<int, int[]> last;
        public Dictionary<int, DepartmentStatus> departmentStatusWindows;
        public Dictionary<string, AttendanceStatus> attendanceStatusWindows;
        Chart chart;
        int status = 0;

        //dummy data
        public MeetingStatus(MeetingController mc)
        {
            InitializeComponent();
            adminModel = new Model<Admin>();

            meetingController = mc;
            statuses = meetingController.statusModel.GetAll().ToDictionary(x => x.id, x => x.name);
            departments = meetingController.departmentModel.GetAll().ToDictionary(x => x.id, x => x.name);
            departmentWrapPanels = new Dictionary<int, WrapPanel>();
            departmentAttendance = new Dictionary<int, TextBlock[]>();
            departmentStatusWindows = new Dictionary<int, DepartmentStatus>();
            attendanceStatusWindows = new Dictionary<string, AttendanceStatus>();
            userGrids = new Dictionary<int, System.Windows.Controls.Grid>();
            last = new Dictionary<int, int[]>();
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
            departmentWrapPanels = new Dictionary<int, WrapPanel>();
            departmentAttendance = new Dictionary<int, TextBlock[]>();
            departmentStatusWindows = new Dictionary<int, DepartmentStatus>();
            userGrids = new Dictionary<int, System.Windows.Controls.Grid>();
            last = new Dictionary<int, int[]>();
            gridDeparts.Children.Clear();
            gridDeparts.RowDefinitions.Clear();
            gridDeparts.ColumnDefinitions.Clear();

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

                System.Windows.Controls.Grid DynamicGrid = new System.Windows.Controls.Grid
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top,
                    Background = new SolidColorBrush(Colors.White)
                };

                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
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
                    Text = "0",
                    FontWeight = FontWeights.Bold,
                    FontSize = 25,
                    Background = Brushes.LightGray,
                    TextAlignment = TextAlignment.Center
                };

                Border border3 = new Border { BorderBrush = Brushes.Gray, BorderThickness = new Thickness(0, 0, 0, 1) };
                border3.Child = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,   
                    Text = "0",
                    FontWeight = FontWeights.Bold,
                    FontSize = 25,
                    Background = Brushes.LightGreen,
                    TextAlignment = TextAlignment.Center
                };

                Border border4 = new Border { BorderBrush = Brushes.Gray, BorderThickness = new Thickness(0, 0, 0, 1) };
                border4.Child = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,   
                    Text = "0",
                    FontWeight = FontWeights.Bold,
                    FontSize = 25,
                    Background = Brushes.LightSalmon,
                    TextAlignment = TextAlignment.Center
                };

                Border border5 = new Border { BorderBrush = Brushes.Gray, BorderThickness = new Thickness(0, 0, 0, 1) };
                border5.Child = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,   
                    Text = "0",
                    FontWeight = FontWeights.Bold,
                    FontSize = 25,
                    Background = Brushes.LightCoral,
                    TextAlignment = TextAlignment.Center
                };

                WrapPanel wp = new WrapPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Orientation = System.Windows.Controls.Orientation.Horizontal
                };

                ScrollViewer sv = new ScrollViewer
                {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Name = "scroll" + count
                };
                sv.Content = wp;

                System.Windows.Controls.Grid.SetRow(border, 0);
                System.Windows.Controls.Grid.SetColumn(border, 0);
                DynamicGrid.Children.Add(border);
                System.Windows.Controls.Grid.SetRow(border2, 0);
                System.Windows.Controls.Grid.SetColumn(border2, 1);
                DynamicGrid.Children.Add(border2);
                System.Windows.Controls.Grid.SetRow(border3, 0);
                System.Windows.Controls.Grid.SetColumn(border3, 2);
                DynamicGrid.Children.Add(border3);
                System.Windows.Controls.Grid.SetRow(border4, 0);
                System.Windows.Controls.Grid.SetColumn(border4, 3);
                DynamicGrid.Children.Add(border4);
                System.Windows.Controls.Grid.SetRow(border5, 0);
                System.Windows.Controls.Grid.SetColumn(border5, 4);
                DynamicGrid.Children.Add(border5);

                System.Windows.Controls.Grid.SetRow(sv, 1);
                System.Windows.Controls.Grid.SetColumn(sv, 0);
                System.Windows.Controls.Grid.SetColumnSpan(sv, 5);
                DynamicGrid.Children.Add(sv);

                System.Windows.Controls.Grid.SetRow(border1, count / 5);
                System.Windows.Controls.Grid.SetColumn(border1, count % 5);
                gridDeparts.Children.Add(border1);

                departmentWrapPanels.Add(entry.Key, wp);
                departmentAttendance.Add(entry.Key, new TextBlock[5]);
                departmentAttendance[entry.Key][0] = (TextBlock)border2.Child;
                departmentAttendance[entry.Key][1] = (TextBlock)border3.Child;
                departmentAttendance[entry.Key][2] = (TextBlock)border4.Child;
                departmentAttendance[entry.Key][3] = (TextBlock)border5.Child;
                count++;
            }

            // Create the interop host control.
            System.Windows.Forms.Integration.WindowsFormsHost host =
                new System.Windows.Forms.Integration.WindowsFormsHost
                {
                    FontSize = 20,
                    FontWeight = FontWeights.Bold
                };
            chart = new Chart();
            Title title = chart.Titles.Add("Нийт албан хаагч");
            title.Font = new System.Drawing.Font("Arial", 15f, System.Drawing.FontStyle.Bold);
            chart.Legends.Add("Тайлбар");
            chart.Legends[0].LegendStyle = LegendStyle.Table;
            chart.Legends[0].Docking = Docking.Bottom;
            chart.Legends[0].Alignment = System.Drawing.StringAlignment.Center;
            chart.Legends[0].BorderColor = System.Drawing.Color.Black;
            chart.Legends[0].Font = new System.Drawing.Font("Arial", 12f);

            //Add a new chart-series
            chart.Series.Clear();

            //Add some datapoints so the series. in this case you can pass the values to this method
            chart.ChartAreas.Add("Default");

            //Add some datapoints so the series. in this case you can pass the values to this method
            host.Child = chart;
            System.Windows.Controls.Grid.SetRow(host, 1);
            System.Windows.Controls.Grid.SetColumn(host, 4);
            gridDeparts.Children.Add(host);

            ArchivedMeeting archivedMeeting = meetingController.onGoingArchivedMeeting;
            meetingName.Text = archivedMeeting.name;
            meetingDate.Text = archivedMeeting.meetingDatetime.ToString("yyyy/MM/dd HH:mm");
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

                ArchivedMeeting archivedMeeting = meetingController.onGoingArchivedMeeting;
                foreach (object[] obj in userAttendances)
                {
                    User user = (User)obj[0];
                    Attendance att = (Attendance)obj[1];    
                    System.Windows.Controls.Grid DynamicGrid = new System.Windows.Controls.Grid
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

                    BitmapImage webImage = meetingController.GetUserImage(user);
                    float scaleHeight = (float)60 / (float)webImage.Height;
                    float scaleWidth = (float)60 / (float)webImage.Width;
                    float scale = Math.Max(scaleHeight, scaleWidth);

                    Image image = new Image
                    {
                        Source = webImage,
                        Height = (int)(webImage.Width * scale),
                        Width = (int)(webImage.Height * scale)
                    };

                    Label name = new Label { Content = user.fname + " " + user.lname, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 15 };
                    Label status = new Label
                    {
                        Content = att.statusId == 2 ? "(" + att.regTime + ") " + statuses[att.statusId] : statuses[att.statusId],
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
                        default: border.BorderBrush = status.Background = Brushes.DarkSlateBlue; break;
                    }
                    System.Windows.Controls.Grid.SetColumn(image, 0);
                    System.Windows.Controls.Grid.SetColumn(name, 0);
                    System.Windows.Controls.Grid.SetColumn(status, 0);
                    System.Windows.Controls.Grid.SetRow(image, 0);
                    System.Windows.Controls.Grid.SetRow(name, 1);
                    System.Windows.Controls.Grid.SetRow(status, 2);
                    DynamicGrid.Children.Add(image);
                    DynamicGrid.Children.Add(name);
                    DynamicGrid.Children.Add(status);

                    if (att.statusId == 1)
                    {
                        if(last[user.departmentId][0] + last[user.departmentId][1] + last[user.departmentId][2] < departmentWrapPanels[user.departmentId].Children.Count)
                        {
                            departmentWrapPanels[user.departmentId].Children.Insert(last[user.departmentId][0] + last[user.departmentId][1] + last[user.departmentId][2], border);
                        }
                        else
                        {
                            departmentWrapPanels[user.departmentId].Children.Add(border);
                        }
                        last[user.departmentId][3]++;
                        border.Visibility = Visibility.Hidden;
                    }
                    else if (att.statusId == 2)
                    {
                        if (last[user.departmentId][0] < departmentWrapPanels[user.departmentId].Children.Count)
                        {
                            departmentWrapPanels[user.departmentId].Children.Insert(last[user.departmentId][0], border);
                        }
                        else
                        {
                            departmentWrapPanels[user.departmentId].Children.Add(border);
                        }
                        last[user.departmentId][1]++;
                        border.Visibility = Visibility.Visible;
                    }
                    else if (att.statusId == 15)
                    {
                        departmentWrapPanels[user.departmentId].Children.Insert(0, border);
                        last[user.departmentId][0]++;
                        border.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (last[user.departmentId][0] + last[user.departmentId][1] < departmentWrapPanels[user.departmentId].Children.Count)
                        {
                            departmentWrapPanels[user.departmentId].Children.Insert(last[user.departmentId][0] + last[user.departmentId][1], border);
                        }
                        else
                        {
                            departmentWrapPanels[user.departmentId].Children.Add(border);
                        }
                        last[user.departmentId][2]++;
                        border.Visibility = Visibility.Hidden;
                    }
                    userGrids.Add(user.id, DynamicGrid);
                }

                foreach (KeyValuePair<int, string> entry in departments)
                {
                    if (!last.ContainsKey(entry.Key))
                    {
                        last.Add(entry.Key, new int[4]);
                    }
                    
                    departmentAttendance[entry.Key][0].Text = ((int)(last[entry.Key][0] + last[entry.Key][1] + last[entry.Key][3])).ToString();
                    departmentAttendance[entry.Key][1].Text = last[entry.Key][3].ToString();
                    departmentAttendance[entry.Key][2].Text = last[entry.Key][1].ToString();
                    departmentAttendance[entry.Key][3].Text = last[entry.Key][0].ToString();
                }
                int grandTotal = last.Sum(x => x.Value[0] + x.Value[1] + x.Value[2] + x.Value[3]);
                int totalAttendance = last.Sum(x => x.Value[0] + x.Value[1] + x.Value[3]);
                int totalArrived = last.Sum(x => x.Value[3]);
                int totalFree = last.Sum(x => x.Value[2]);
                int totalLate = last.Sum(x => x.Value[1]);
                int totalMissing = last.Sum(x => x.Value[0]);
                AttendanceLabel1.Text = totalAttendance.ToString();
                AttendanceLabel2.Text = totalArrived.ToString();
                AttendanceLabel3.Text = totalLate.ToString();
                AttendanceLabel4.Text = totalMissing.ToString();
                AttendancePercentageLabel1.Text = Math.Round((((double)totalArrived / totalAttendance) * 100)).ToString() + '%';
                AttendancePercentageLabel2.Text = Math.Round((((double)totalLate / totalAttendance) * 100)).ToString() + '%';
                AttendancePercentageLabel3.Text = Math.Round((((double)totalMissing / totalAttendance) * 100)).ToString() + '%';
                //Add a new chart-series
                chart.Series.Clear();
                string seriesname = "Хүмүүс";
                chart.Series.Add(seriesname);
                //set the chart-type to "Pie"
                chart.Series[seriesname].ChartType = SeriesChartType.Pie;
                chart.Series[seriesname].Font = new System.Drawing.Font("Arial", 13f, System.Drawing.FontStyle.Bold);
                chart.Series[seriesname].Label = "#PERCENT{0 %}";
                chart.Series[seriesname].LegendText = "#VALX";
                //Add some datapoints so the series. in this case you can pass the values to this method
                chart.Series[seriesname].Points.AddXY("Ирсэн", totalArrived);
                chart.Series[seriesname].Points.AddXY("Хоцорсон", totalLate);
                chart.Series[seriesname].Points.AddXY("Чөлөөтэй", totalFree);
                chart.Series[seriesname].Points.AddXY("Ирээгүй", totalMissing);
                chart.Series[0].Points[0].Color = System.Drawing.Color.LightGreen;
                chart.Series[0].Points[1].Color = System.Drawing.Color.LightSalmon;
                chart.Series[0].Points[2].Color = System.Drawing.Color.LightSkyBlue;
                chart.Series[0].Points[3].Color = System.Drawing.Color.LightCoral;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Хурлын цонх нээхэд алдаа гарлаа. Алдааны мессеж: " + ex.ToString());
            }
        }

        public void UpdateCountDown()
        {
            if (meetingController.status == MeetingController.MEETING_STARTED && status == 0)
            {
                status = 1;
                BuildDepartControls();
                PlaceUsers(meetingController.onGoingMeetingUserAttendance);
            }
            else if (meetingController.status == MeetingController.IDLE && status == 1)
            {
                status = 0;
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
                ((Label)userGrids[user.id].Children[2]).Content = attendance.statusId == 2 ? "(" + attendance.regTime + ") " + statuses[attendance.statusId] : statuses[attendance.statusId];

                switch (attendance.statusId)
                {
                    case 1: ((Border)userGrids[user.id].Parent).BorderBrush = ((Label)userGrids[user.id].Children[2]).Background = Brushes.DarkGreen; break;
                    case 2: ((Border)userGrids[user.id].Parent).BorderBrush = ((Label)userGrids[user.id].Children[2]).Background = Brushes.DarkOrange; break;
                    case 15: ((Border)userGrids[user.id].Parent).BorderBrush = ((Label)userGrids[user.id].Children[2]).Background = Brushes.DarkRed; break;
                    default: ((Border)userGrids[user.id].Parent).BorderBrush = ((Label)userGrids[user.id].Children[2]).Background = Brushes.DarkSlateBlue; break;
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
                    ((Border)userGrids[user.id].Parent).Visibility = Visibility.Hidden;
                }
                else if (attendance.statusId == 2)
                {
                    departmentWrapPanels[user.departmentId].Children.Insert(last[user.departmentId][0], (Border)userGrids[user.id].Parent);
                    last[user.departmentId][1]++;
                    ((Border)userGrids[user.id].Parent).Visibility = Visibility.Visible;
                }
                else if (attendance.statusId == 15)
                {
                    departmentWrapPanels[user.departmentId].Children.Insert(0, (Border)userGrids[user.id].Parent);
                    last[user.departmentId][0]++;
                    ((Border)userGrids[user.id].Parent).Visibility = Visibility.Visible;
                }
                else
                {
                    departmentWrapPanels[user.departmentId].Children.Insert(last[user.departmentId][0] + last[user.departmentId][1], (Border)userGrids[user.id].Parent);
                    last[user.departmentId][2]++;
                    ((Border)userGrids[user.id].Parent).Visibility = Visibility.Hidden;
                }

                foreach (KeyValuePair<int, DepartmentStatus> entry in departmentStatusWindows)
                {
                    if(entry.Value != null && entry.Value.IsLoaded && entry.Value.IsActive)
                    {
                        entry.Value.Update(identifiedUserAttendance, oldStatusId);
                    }
                }
            }
            foreach (KeyValuePair<string, AttendanceStatus> entry in attendanceStatusWindows)
            {
                if (entry.Value != null && entry.Value.IsLoaded && entry.Value.IsActive)
                {
                    entry.Value.Update();
                }
            }
            foreach (KeyValuePair<int, string> entry in departments)
            {
                departmentAttendance[entry.Key][0].Text = (last[entry.Key][0] + last[entry.Key][1] + last[entry.Key][3]).ToString();
                departmentAttendance[entry.Key][1].Text = (last[entry.Key][3]).ToString();
                departmentAttendance[entry.Key][2].Text = (last[entry.Key][1]).ToString();
                departmentAttendance[entry.Key][3].Text = (last[entry.Key][0]).ToString();
            }


            int grandTotal = last.Sum(x => x.Value[0] + x.Value[1] + x.Value[2] + x.Value[3]);
            int totalAttendance = last.Sum(x => x.Value[0] + x.Value[1] + x.Value[3]);
            int totalArrived = last.Sum(x => x.Value[3]);
            int totalFree = last.Sum(x => x.Value[2]);
            int totalLate = last.Sum(x => x.Value[1]);
            int totalMissing = last.Sum(x => x.Value[0]);
            AttendanceLabel1.Text = totalAttendance.ToString();
            AttendanceLabel2.Text = totalArrived.ToString();
            AttendanceLabel3.Text = totalLate.ToString();
            AttendanceLabel4.Text = totalMissing.ToString();
            AttendancePercentageLabel1.Text = Math.Round((((double)totalArrived / totalAttendance) * 100)).ToString() + '%';
            AttendancePercentageLabel2.Text = Math.Round((((double)totalLate / totalAttendance) * 100)).ToString() + '%';
            AttendancePercentageLabel3.Text = Math.Round((((double)totalMissing / totalAttendance) * 100)).ToString() + '%';
            //Add a new chart-series
            chart.Series.Clear();
            string seriesname = "Хүмүүс";
            chart.Series.Add(seriesname);
            //set the chart-type to "Pie"
            chart.Series[seriesname].ChartType = SeriesChartType.Pie;
            chart.Series[seriesname].Font = new System.Drawing.Font("Arial", 13f, System.Drawing.FontStyle.Bold);
            chart.Series[seriesname].Label = "#PERCENT{0 %}";
            chart.Series[seriesname].LegendText = "#VALX";
            //Add some datapoints so the series. in this case you can pass the values to this method
            chart.Series[seriesname].Points.AddXY("Ирсэн", totalArrived);
            chart.Series[seriesname].Points.AddXY("Хоцорсон", totalLate);
            chart.Series[seriesname].Points.AddXY("Чөлөөтэй", totalFree);
            chart.Series[seriesname].Points.AddXY("Ирээгүй", totalMissing);
            chart.Series[0].Points[0].Color = System.Drawing.Color.LightGreen;
            chart.Series[0].Points[1].Color = System.Drawing.Color.LightSalmon;
            chart.Series[0].Points[2].Color = System.Drawing.Color.LightSkyBlue;
            chart.Series[0].Points[3].Color = System.Drawing.Color.LightCoral;

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

        public void ShowAttendanceStatus(string msg = "")
        {
            if (attendanceStatusWindows.ContainsKey(msg))
            {
                if (attendanceStatusWindows[msg].IsLoaded && attendanceStatusWindows[msg].IsActive && attendanceStatusWindows[msg].IsEnabled)
                {
                    attendanceStatusWindows[msg].Focus();
                    attendanceStatusWindows[msg].Visibility = Visibility.Visible;
                }
                else
                {
                    attendanceStatusWindows[msg] = new AttendanceStatus(meetingController, msg);
                    attendanceStatusWindows[msg].Owner = this;
                    attendanceStatusWindows[msg].Visibility = Visibility.Visible;
                }
            }
            else
            {
                attendanceStatusWindows.Add(msg, new AttendanceStatus(meetingController, msg));
                attendanceStatusWindows[msg].Owner = this;
                attendanceStatusWindows[msg].Visibility = Visibility.Visible;
            }
            attendanceStatusWindows[msg].Update();
        }

        public void ShowMoreStatus(object sender, RoutedEventArgs e)
        {
            if (meetingController.status == MeetingController.MEETING_STARTED)
                ShowAttendanceStatus();
        }

        public void StopMeetingClicked(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Хурлыг зогсоох уу?";
            string caption = "Хурал зогсоох";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            // Display message box
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            // Process message box results
            switch (result)
            {
                case MessageBoxResult.Yes:
                    meetingController.ForceStopMeeting();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }
        
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            Owner = null;
        }

    }
}
