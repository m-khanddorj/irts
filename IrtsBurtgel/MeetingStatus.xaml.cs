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
        Dictionary<int, ObservableCollection<UserDataItem>> userDataItemsByDepId;
        Dictionary<int, UserDataItem> userDataItems;

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
            userDataItemsByDepId = new Dictionary<int, ObservableCollection<UserDataItem>>();
            userDataItems = new Dictionary<int, UserDataItem>();
            if (meetingController.status == MeetingController.MEETING_STARTED)
            {
                BuildDepartControls();
                PlaceUsers(meetingController.onGoingMeetingUserAttendance);
            }
        }

        private void Expand(object sender, RoutedEventArgs e)
        {
            if(this.WindowState != WindowState.Maximized && this.WindowStyle != WindowStyle.None)
            {
                fullScreenToggleImage.Source = new BitmapImage(new Uri("images/shrink.png", UriKind.Relative));
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
            else if ( this.WindowState == WindowState.Maximized &&this.WindowStyle != WindowStyle.None)
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
        /*
        public void DisplayCountDown(string textToDisplay)
        {
            ColumnDefinition gridCol = new ColumnDefinition();
            gridCol.Width = new GridLength(1, GridUnitType.Star);
            gridDeparts.ColumnDefinitions.Add(gridCol);
            RowDefinition gridRow = new RowDefinition();
            gridRow.Height = new GridLength(1, GridUnitType.Star);
            gridDeparts.RowDefinitions.Add(gridRow);

            TextBlock textBlock = new TextBlock();
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.Text = textToDisplay;
            textBlock.Margin = new Thickness(5, 5, 5, 5);
            textBlock.FontSize = 25;
            textBlock.FontWeight = FontWeights.Light;
            textBlock.Foreground = Brushes.White;
        }*/

        public void BuildDepartControls()
        {
            gridDeparts.Children.Clear();

            for (int i = 0; i < 5; i++)
            {
                ColumnDefinition gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(1, GridUnitType.Star);
                gridDeparts.ColumnDefinitions.Add(gridCol);
            }
            int count = 0;
            foreach (KeyValuePair<int, string> entry in departments)
            {
                if (count % 5 == 0)
                {
                    RowDefinition gridRow = new RowDefinition();
                    gridRow.Height = new GridLength(1, GridUnitType.Star);
                    gridDeparts.RowDefinitions.Add(gridRow);
                }

                Border border1 = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1, 1, 1, 1)
                };

                Grid DynamicGrid = new Grid();
                DynamicGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                DynamicGrid.VerticalAlignment = VerticalAlignment.Top;
                DynamicGrid.Background = new SolidColorBrush(Colors.White);

                ColumnDefinition gridCol1 = new ColumnDefinition();
                gridCol1.Width = new GridLength(1, GridUnitType.Star);
                DynamicGrid.ColumnDefinitions.Add(gridCol1);

                RowDefinition gridRow1 = new RowDefinition();
                gridRow1.Height = new GridLength(50);
                DynamicGrid.RowDefinitions.Add(gridRow1);
                
                RowDefinition gridRow2 = new RowDefinition();
                gridRow2.Height = new GridLength(1, GridUnitType.Star);
                DynamicGrid.RowDefinitions.Add(gridRow2);

                Border border = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1, 1, 1, 1)
                };
                
                TextBlock textBlock = new TextBlock();
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.Text = entry.Value;
                textBlock.Margin = new Thickness(5, 5, 5, 5);
                border.Child = textBlock;

                Grid.SetRow(border, 0);
                Grid.SetColumn(border, 0);

                ObservableCollection<UserDataItem> udi = new ObservableCollection<UserDataItem>();
                userDataItemsByDepId.Add(entry.Key, udi);

                var datagrid = new DataGrid();
                datagrid.ItemsSource = udi;
                datagrid.IsReadOnly = true;
                datagrid.AutoGenerateColumns = false;
                datagrid.HorizontalAlignment = HorizontalAlignment.Stretch;

                datagrid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Нэр",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Name")
                });
                datagrid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Төлөв",
                    Width = new DataGridLength(100),
                    Binding = new Binding("Status")
                });

                Grid.SetRow(datagrid, 1);
                Grid.SetColumn(datagrid, 0);

                DynamicGrid.Children.Add(border);
                DynamicGrid.Children.Add(datagrid);

                Grid.SetRow(DynamicGrid, count / 5);
                Grid.SetColumn(DynamicGrid, count % 5);
                gridDeparts.Children.Add(DynamicGrid);
                count++;
            }
        }

        public void PlaceUsers(List<object[]> userAttendances)
        {
            userAttendances = userAttendances.OrderBy(x => ((Attendance)x[1]).statusId).ToList();

            foreach (KeyValuePair<int, string> entry in departments) {
                userDataItemsByDepId[entry.Key].Clear();
            }

            foreach (object[] obj in userAttendances)
            {
                RowDefinition tmprow = new RowDefinition();
                tmprow.Height = new GridLength(40, GridUnitType.Pixel);

                User user = (User)obj[0];
                Attendance att = (Attendance)obj[1];

                if(att.statusId == 1)
                {
                    continue;
                }

                UserDataItem userData = new UserDataItem
                {
                    Name = user.fname + " " + user.lname,
                    Status = att.statusId == 2 ? statuses[att.statusId] + " (" + att.regTime + ")" : statuses[att.statusId],
                    StatusID = att.statusId.ToString()
                };
                userDataItemsByDepId[user.departmentId].Add(userData);
                userDataItems.Add(user.id, userData);
            }
        }

        public void Update(object[] identifiedUserAttendance)
        {
            User user = (User)identifiedUserAttendance[0];
            Attendance attendance = (Attendance)identifiedUserAttendance[1];
            if (meetingController.status == MeetingController.MEETING_STARTED)
            {
                if (attendance.statusId == 1)
                {
                    userDataItemsByDepId[user.departmentId].Remove(userDataItems[user.id]);
                }
                else
                {
                    userDataItems[user.id].Status = attendance.statusId == 2 ? statuses[attendance.statusId] + " (" + attendance.regTime + ")" : statuses[attendance.statusId];
                    if (attendance.statusId == 2)
                    {
                        int len = userDataItemsByDepId[user.departmentId].Count;
                        int i;
                        for (i = 0; i < len; i++)
                        {
                            if(userDataItemsByDepId[user.departmentId][i].statusId != 15)
                            {
                                userDataItemsByDepId[user.departmentId].Move(userDataItemsByDepId[user.departmentId].IndexOf(userDataItems[user.id]), i);
                                break;
                            }
                        }

                        if(i == len)
                        {
                            userDataItemsByDepId[user.departmentId].Move(userDataItemsByDepId[user.departmentId].IndexOf(userDataItems[user.id]), i - 1);
                        }
                    }
                }
            }
        }
        
    }
}
