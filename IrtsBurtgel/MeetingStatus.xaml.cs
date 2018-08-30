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
    /// Interaction logic for MeetingStatus.xaml
    /// </summary>
    public partial class MeetingStatus : Window
    {
        Model<Admin> adminModel;
        MeetingController meetingController;
        Dictionary<int, string> statuses;
        Dictionary<int, string> departments;
        Dictionary<int, Grid> departmentGrids;

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
            if(meetingController.status == MeetingController.MEETING_STARTED)
            {
                departmentGrids = new Dictionary<int, Grid>();
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

        public void BuildDepartControls()
        {
            for(int i = 0; i < 5; i++)
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

                Grid DynamicGrid = new Grid();
                DynamicGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                DynamicGrid.VerticalAlignment = VerticalAlignment.Top;
                DynamicGrid.ShowGridLines = true;
                DynamicGrid.Background = new SolidColorBrush(Colors.White);

                ColumnDefinition gridCol1 = new ColumnDefinition();
                gridCol1.Width = new GridLength(1, GridUnitType.Star);
                DynamicGrid.ColumnDefinitions.Add(gridCol1);

                ColumnDefinition gridCol2 = new ColumnDefinition();
                gridCol2.Width = new GridLength(70);
                DynamicGrid.ColumnDefinitions.Add(gridCol2);

                RowDefinition gridRow1 = new RowDefinition();
                gridRow1.Height = new GridLength(50);
                DynamicGrid.RowDefinitions.Add(gridRow1);

                TextBlock textBlock = new TextBlock();
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.Text = entry.Value;
                textBlock.Margin = new Thickness(5, 5, 5, 5);
                Grid.SetRow(textBlock, 0);
                Grid.SetColumn(textBlock, 0);
                Grid.SetColumnSpan(textBlock, 2);
                Grid.SetRowSpan(textBlock, 2);
              
                DynamicGrid.Children.Add(textBlock);

                Grid.SetRow(DynamicGrid, count/5);
                Grid.SetColumn(DynamicGrid, count%5);
                gridDeparts.Children.Add(DynamicGrid);

                departmentGrids.Add(entry.Key, DynamicGrid);
                count++;
            }
        }

        public void PlaceUsers(List<object[]> userAttendances)
        {
            foreach (object[] obj in userAttendances)
            {
                RowDefinition tmprow = new RowDefinition();
                tmprow.Height = new GridLength(40, GridUnitType.Pixel);
                

                User user = (User)obj[0];
                Attendance att = (Attendance)obj[1];
                
                Label userlabel = new Label
                {
                    Content = user.fname + " " + user.lname,
                };
                Label latelabel = new Label
                {
                    Content = att.statusId == 2 ? statuses[att.statusId] + " (" + att.regTime + ")" : statuses[att.statusId]
                };

                Grid.SetRow(userlabel, departmentGrids[user.departmentId].RowDefinitions.Count);
                Grid.SetRow(latelabel, departmentGrids[user.departmentId].RowDefinitions.Count);
                Grid.SetColumn(userlabel, 0);
                Grid.SetColumn(latelabel, 1);

                departmentGrids[user.departmentId].RowDefinitions.Add(tmprow);
                departmentGrids[user.departmentId].Children.Add(userlabel);
                departmentGrids[user.departmentId].Children.Add(latelabel);
            }
        }

        //public void Update(List<object[]> latepeople)
        //{
        //    attendancelabel.content = latepeople.count;
        //    status1.rowdefinitions.clear();
        //    status1.children.clear();

        //    rowdefinition row = new rowdefinition();
        //    row.height = new gridlength(1, gridunittype.star);

        //    status1.rowdefinitions.add(row);
        //    label namelabel = new label();
        //    namelabel.content = "ажилтны нэр";
        //    label lateminute = new label();
        //    lateminute.content = "төлөв";

        //    foreach (object[] obj in latepeople)
        //    {
        //        rowdefinition tmprow = new rowdefinition();
        //        tmprow.height = new gridlength(40, gridunittype.pixel);

        //        user user = (user)obj[0];
        //        attendance att = (attendance)obj[1];

        //        label userlabel = new label();
        //        userlabel.content = user.fname + " " + user.lname;
        //        label latelabel = new label();
        //        latelabel.content = statuses[att.statusid];

        //        status1.rowdefinitions.add(tmprow);
        //        int rownum = status1.rowdefinitions.count - 1;

        //        grid.setcolumn(userlabel, 0);
        //        grid.setcolumn(latelabel, 1);

        //        grid.setrow(userlabel, rownum);
        //        grid.setrow(latelabel, rownum);

        //        status1.children.add(userlabel);
        //        status1.children.add(latelabel);

        //    }
        //}
    }
}
