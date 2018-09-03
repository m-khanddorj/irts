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
        Dictionary<int, string> stats;

        public ChangeUserStatus(int uid)
        {
            InitializeComponent();
            userModel = new Model<User>();
            usModel = new Model<UserStatus>();
            statModel = new Model<Status>();
            this.uid = uid;

            stats = statModel.GetAll().ToDictionary(x => x.id, x => x.name);
            foreach (KeyValuePair<int, string> entry in stats)
            {
                if (entry.Key != 1 && entry.Key != 2 && entry.Key != 14 && entry.Key != 15)
                {
                    ComboBoxItem cbi = new ComboBoxItem();
                    cbi.Content = entry.Value;
                    cbi.Uid = entry.Key.ToString();

                    combobox.Items.Add(cbi);
                }
            }

            user = userModel.Get(uid);

            Load();
        }

        void Load()
        {
            userStatusStory.Items.Clear();
            userName.Content = user.fname + " " + user.lname;
            IEnumerable<UserStatus> userStatuses = usModel.GetByFK(user.IDName, user.id).OrderByDescending(x => x.endDate);
            int i = 1;
            DateTime maxDate = DateTime.Now;
            int currentStatus = -1;
            DateTime now = DateTime.Now;

            foreach (UserStatus us in userStatuses)
            {
                userStatusStory.Items.Add(new ListBoxItem
                {
                    Content = i + ". " + us.startDate.ToString("yyyy/MM/dd") + "-с " + us.endDate.ToString("yyyy/MM/dd") + " хүртэл " + stats[us.statusId].ToLower()
                });
                if (us.endDate > maxDate)
                {
                    maxDate = us.endDate;
                }
                if (currentStatus == -1 && us.endDate.Date >= now.Date && now.Date >= us.startDate.Date)
                {
                    currentStatus = us.statusId;
                }
                i++;
            }

            switch (currentStatus)
            {
                case -1: currentState.Content = "Идэвхитэй"; currentState.Background = Brushes.DarkGreen; break;
                default: currentState.Content = stats[currentStatus]; currentState.Background = Brushes.DarkOrange; break;
            }

            startDate.DisplayDateStart = maxDate;
            endDate.DisplayDateStart = maxDate;
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
            startDate.SelectedDate = null;
            endDate.SelectedDate = null;
            combobox.SelectedItem = null;

            Load();
        }

        private void startDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            endDate.DisplayDateStart = startDate.SelectedDate;
        }
    }
}
