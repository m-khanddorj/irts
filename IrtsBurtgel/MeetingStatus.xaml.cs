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
            BuildDepartControls();
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
            foreach (KeyValuePair<int, string> entry in departments)
            {
                ColumnDefinition gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(1, GridUnitType.Star);
                gridDeparts.ColumnDefinitions.Add(gridCol);

                TextBlock textBlock = new TextBlock();
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.Text = entry.Value;
                textBlock.SetValue(Grid.ColumnProperty, entry.Key);
                gridDeparts.Children.Add(textBlock);
            }
        }

        //public void update(list<object[]> latepeople)
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

        //    foreach(object[] obj in latepeople)
        //    {
        //        //rowdefinition tmprow = new rowdefinition();
        //        //tmprow.height = new gridlength(40, gridunittype.pixel);

        //        //user user = (user)obj[0];
        //        //attendance att = (attendance)obj[1];

        //        //label userlabel = new label();
        //        //userlabel.content = user.fname + " " + user.lname;
        //        //label latelabel = new label();
        //        //latelabel.content = statuses[att.statusid];

        //        //status1.rowdefinitions.add(tmprow);
        //        //int rownum = status1.rowdefinitions.count - 1;

        //        //grid.setcolumn(userlabel, 0);
        //        //grid.setcolumn(latelabel, 1);

        //        //grid.setrow(userlabel, rownum);
        //        //grid.setrow(latelabel, rownum);

        //        //status1.children.add(userlabel);
        //        //status1.children.add(latelabel);

        //    }
        //}
    }
}
