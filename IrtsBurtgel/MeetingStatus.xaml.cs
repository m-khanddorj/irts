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
        //dummy data
        public MeetingStatus()
        {
            InitializeComponent();
            adminModel = new Model<Admin>();
            //Showing the org title
            OrgTitle.Content = adminModel.GetAll()[0].organizationName;
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
        private void Update(List<Object[]> latePeople)
        {
            AttendanceLabel.Content = latePeople.Count;
            status.RowDefinitions.Clear();
            status.Children.Clear();

            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(1, GridUnitType.Star);

            status.RowDefinitions.Add(row);
            Label nameLabel = new Label();
            nameLabel.Content = "Ажилтны нэр";
            Label lateMinute = new Label();
            lateMinute.Content = "Хоцорсон минут";

            foreach(Object[] obj in latePeople)
            {
                RowDefinition tmpRow = new RowDefinition();
                tmpRow.Height = new GridLength(1, GridUnitType.Star);

                User user = (User)obj[0];
                Attendance att = (Attendance)obj[1];

                Label userLabel = new Label();
                userLabel.Content = user.fname + " " + user.lname;
                Label lateLabel = new Label();
                lateLabel.Content = att.regTime;

                status.RowDefinitions.Add(tmpRow);
                int rowNum = status.RowDefinitions.Count-1;

                Grid.SetColumn(userLabel, 0);
                Grid.SetColumn(lateLabel, 1);

                Grid.SetRow(userLabel, rowNum);
                Grid.SetRow(lateLabel, rowNum);

                status.Children.Add(userLabel);
                status.Children.Add(lateLabel);
            }
        }
    }
}
