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
    /// Interaction logic for ChangeUserStatus.xaml
    /// </summary>
    public partial class ChangeUserStatus : Window
    {
        Model<User> userModel;
        Model<UserStatus> usModel;
        Model<Status> statModel;
        User user;
        int uid;
        public ChangeUserStatus(int uid)
        {
            InitializeComponent();
            userModel = new Model<User>();
            usModel = new Model<UserStatus>();
            statModel = new Model<Status>();
            this.uid = uid;

            List<Status> stats = statModel.GetAll();
            foreach(Status stat in stats)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = stat.name;
                cbi.Uid = stat.id.ToString();

                combobox.Items.Add(cbi);
            }

            user = userModel.Get(uid);

            userName.Content = user.fname + " " + user.lname;
        }
        void ChangeStatus(object sender, RoutedEventArgs e)
        {
            UserStatus us = new UserStatus();
            if (startDate.SelectedDate == null)
            {
                MessageBox.Show("Төлөв эхлэх огноог сонгоно уу!");
                return;
            }
            if(endDate.SelectedDate == null)
            {
                MessageBox.Show("Төлөв дуусах огноог сонгоно уу!");
                return;
            }
            if(combobox.SelectedItem == null)
            {
                MessageBox.Show("Төлөв сонгоно уу!");
                return;
            }
            us.endDate = (DateTime)endDate.SelectedDate;
            us.startDate = (DateTime)startDate.SelectedDate;
            us.statusId = Int32.Parse( ((ComboBoxItem)combobox.SelectedItem).Uid );
            us.userId = uid;
            usModel.Add(us);
            MessageBox.Show("Амжилттай");
            this.Close();
        }
    }
}
