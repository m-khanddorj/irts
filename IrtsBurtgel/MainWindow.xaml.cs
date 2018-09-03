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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.IO;
using Sample;
using System.Text.RegularExpressions;
using Xceed.Wpf.Toolkit;

namespace IrtsBurtgel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Model<Meeting> meetingModel;
        Model<User> userModel;
        MeetingController meetingController;
        Model<Department> depModel;
        Model<Position> posModel;
        Model<MeetingAndUser> mauModel;
        Model<MeetingAndDepartment> madModel;
        Model<MeetingAndPosition> mapModel;
        Model<ModifiedMeeting> modifiedMeetingModel;
        Model<Attendance> attModel;
        Model<ArchivedMeeting> archModel;
        public List<MeetingStatus> meetingStatusWindows;
        bool is_home = false;

        public MainWindow()
        {
            InitializeComponent();

            meetingController = new MeetingController(this);
            meetingModel = meetingController.meetingModel;
            userModel = meetingController.userModel;
            depModel = meetingController.departmentModel;
            posModel = meetingController.positionModel;
            mauModel = meetingController.muModel;
            madModel = meetingController.mdModel;
            mapModel = meetingController.mpModel;
            modifiedMeetingModel = meetingController.modifiedMeetingModel;
            meetingStatusWindows = new List<MeetingStatus>();
            archModel = new Model<ArchivedMeeting>();
            attModel = new Model<Attendance>();
            
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            List<Object[]> objs = meetingController.GetClosestMeetings(1);
            if (objs == null || objs.Count == 0)
            {
                time.Text = "Хурал байхгүй байна.";
                return;
            }
            else
            {
                Object[] obj = objs[0];
                string texttodisplay = meetingController.TextToDisplay();
                time.Text = texttodisplay;
                meetingController.CheckMeeting();
                foreach (MeetingStatus ms in meetingStatusWindows)
                {
                    ms.UpdateCountDown(obj);
                }
            }
        }

        public void login(object sender, RoutedEventArgs e)
        {
            if (username.Text == "admin" && password.Password == "admin") showMenu();
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Та нэр, нууц үгээ дахин шалгана уу!", "Нэр эсвэл нууц үг буруу байна");
            }
        }
        private void showStatus(object sender, RoutedEventArgs e)
        {
            MeetingStatus meetingStatus = new MeetingStatus(meetingController);
            meetingStatusWindows.Add(meetingStatus);
            //meetingStatus.WindowStartupLocation = WindowStartupLocation.Manual;

            //Debug.Assert(System.Windows.Forms.SystemInformation.MonitorCount > 1);
            if (System.Windows.Forms.Screen.AllScreens.Length > 1)
            {
                System.Drawing.Rectangle workingArea = System.Windows.Forms.Screen.AllScreens[1].WorkingArea;
                meetingStatus.Left = workingArea.Left;
                meetingStatus.Top = workingArea.Top;
                meetingStatus.Width = workingArea.Width;
                meetingStatus.Height = workingArea.Height;
                //meetingStatus.WindowState = WindowState.Maximized;
                meetingStatus.WindowStyle = WindowStyle.None;
                meetingStatus.Topmost = true;
                meetingStatus.Show();
            }
            else
            {
                meetingStatus.Show();
            }
            //meetingStatus.Visibility = Visibility.Visible;
            //meetingController.StartMeeting(meetingModel.Get(3));
        }

        private void showMenu(object sender = null, RoutedEventArgs e = null)
        {
            is_home = true;

            try
            {
                UnregisterName("SearchBox");
            }
            catch(Exception ex)
            {

            }
            //meetingController.StopMeeting();
            LeftSide.Children.Clear();
            LeftSide.HorizontalAlignment = HorizontalAlignment.Stretch;
            LeftSide.VerticalAlignment = VerticalAlignment.Center;
            DockPanel dock = addHeader();

            StackPanel Menu = new StackPanel();
            Menu.Name = "MainMenu";
            Menu.VerticalAlignment = VerticalAlignment.Center;

            Thickness margin = new Thickness();
            margin.Bottom = 10;

            Image img = new Image();
            img.Source = new BitmapImage(new Uri("images/time-logo.png", UriKind.Relative));
            img.Width = 210;
            img.Margin = margin;

            Button Calendar = new Button();
            Calendar.Content = "Календар";
            Calendar.Margin = margin;
            Calendar.Click += ShowCalendar;
            Calendar.Width = 210;
            Calendar.Height = 30;
            Calendar.Background = Brushes.White;

            Button Meetings = new Button();
            Meetings.Content = "Хурлууд";
            Meetings.Margin = margin;
            Meetings.Click += ShowMeetings;
            Meetings.Width = 210;
            Meetings.Height = 30;
            Meetings.Background = Brushes.White;

            Button Members = new Button();
            Members.Content = "Гишүүд";
            Members.Margin = margin;
            Members.Click += ShowMembers;
            Members.Width = 210;
            Members.Height = 30;
            Members.Background = Brushes.White;


            Button Report = new Button();
            Report.Content = "Тайлан";
            Report.Margin = margin;
            Report.Click += ShowReport;
            Report.Width = 210;
            Report.Height = 30;
            Report.Background = Brushes.White;

            Menu.Children.Add(img);
            Menu.Children.Add(Calendar);
            Menu.Children.Add(Meetings);
            Menu.Children.Add(Members);
            Menu.Children.Add(Report);

            dock.Children.Add(Menu);

            RightSide.Children.Clear();
            RightSide.HorizontalAlignment = HorizontalAlignment.Stretch;

            List<Object[]> closestMeetings = meetingController.GetClosestMeetings(10);

            Label headerLabel = new Label();
            headerLabel.Content = "Хамгийн ойрын "+ closestMeetings.Count.ToString()+ " хурал";
            headerLabel.Margin = new Thickness(0,0,0,10);
            headerLabel.FontSize = 20;
            headerLabel.HorizontalAlignment = HorizontalAlignment.Center;

            Border border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Black;
            border.HorizontalAlignment = HorizontalAlignment.Center;

            Grid grid = new Grid();
            grid.ShowGridLines = true;

            ColumnDefinition col0 = new ColumnDefinition();
            col0.Width = new GridLength(1,GridUnitType.Star);
            ColumnDefinition col1 = new ColumnDefinition();
            col1.Width = new GridLength(1, GridUnitType.Star);

            grid.ColumnDefinitions.Add(col0);
            grid.ColumnDefinitions.Add(col1);

            RowDefinition header = new RowDefinition();
            header.Height = new GridLength(30);

            grid.RowDefinitions.Add(header);

            Label timeHeaderLabel = new Label();
            timeHeaderLabel.Content = "Огноо";
            timeHeaderLabel.Background =  new SolidColorBrush(Color.FromArgb(0xFF, 00, 0x7A, 0xCC));
            timeHeaderLabel.Foreground = Brushes.White;

            Grid.SetRow(timeHeaderLabel, 0);
            Grid.SetColumn(timeHeaderLabel, 0);

            Label closestMeetingLabel = new Label();
            closestMeetingLabel.Content = "Хурлын нэр";
            closestMeetingLabel.Background = new SolidColorBrush(Color.FromArgb(0xFF, 00, 0x7A, 0xCC));
            closestMeetingLabel.Foreground = Brushes.White;

            Grid.SetRow(closestMeetingLabel,0);
            Grid.SetColumn(closestMeetingLabel, 1);

            grid.Children.Add(timeHeaderLabel);
            grid.Children.Add(closestMeetingLabel);

            int rowNum = 1;
            foreach( Object[] obj in closestMeetings)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(30);
                grid.RowDefinitions.Add(row);

                Label timeLabel = new Label();
                timeLabel.HorizontalAlignment = HorizontalAlignment.Center;
                timeLabel.Content = ((DateTime)obj[0]).ToString("yyyy/MM/dd HH:mm");

                Grid.SetColumn(timeLabel, 0);
                Grid.SetRow(timeLabel, rowNum);

                Label nameLabel = new Label();
                nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
                nameLabel.Content = ((Meeting)obj[1]).name;

                Grid.SetColumn(nameLabel, 1);
                Grid.SetRow(nameLabel, rowNum);
                rowNum++;

                grid.Children.Add(timeLabel);
                grid.Children.Add(nameLabel);
            }
            border.Child = grid;

            Button status = new Button();
            status.Margin = new Thickness(0, 10, 0, 0);
            status.Height = 30;
            status.Background = new SolidColorBrush(Color.FromArgb(0xFF, 00, 0x7A, 0xCC));

            Image eyeImage = new Image();
            eyeImage.Source = new BitmapImage(new Uri("images/eye.png",UriKind.Relative));
            eyeImage.Width = 30;

            StackPanel tmp = new StackPanel();
            tmp.Orientation = Orientation.Horizontal;
            tmp.Children.Add(eyeImage);

            Label label = new Label();
            label.Content = "Ирц харах";
            label.Foreground = Brushes.White;
            tmp.Children.Add(label);
            
            status.Click += showStatus;
            status.Content = tmp;

            status.HorizontalAlignment = HorizontalAlignment.Center;

            RightSide.Children.Add(headerLabel);
            RightSide.Children.Add(border);
            RightSide.Children.Add(status);
        }

        void goBack(object sender = null, RoutedEventArgs e = null)
        {
            if (is_home)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            else showMenu();
        }
        DockPanel addHeader(List<Object> controls = null, List<Object> rControls = null)
        {
            LeftSide.VerticalAlignment = VerticalAlignment.Stretch;
            LeftSide.HorizontalAlignment = HorizontalAlignment.Stretch;

            DockPanel dockPanel = new DockPanel();

            DockPanel headerPanel = new DockPanel();
            headerPanel.Margin = new Thickness(10, 5, 10, -5);
            headerPanel.HorizontalAlignment = HorizontalAlignment.Stretch;

            Button back = new Button();

            back.Content = "⯇";
            back.FontSize = 20;
            back.Click += goBack;
            back.Foreground = Brushes.White;
            back.BorderBrush = Brushes.Transparent;
            back.Background = Brushes.Transparent;
            back.Height = 30;
            back.Width = 30;
            back.HorizontalAlignment = HorizontalAlignment.Left;

            headerPanel.Children.Add(back);
            
            DockPanel.SetDock(headerPanel, Dock.Left);

            if (controls != null)
            {
                foreach (Object control in controls)
                {
                    if (control is Button)
                    {
                        Button tmp = (Button)control;
                        tmp.Height = 30;
                        tmp.Width = 30;
                        tmp.Foreground = Brushes.White;
                        tmp.BorderBrush = Brushes.Transparent;
                        tmp.Background = Brushes.Transparent;
                        headerPanel.Children.Add(tmp);
                    }
                    if (control is Label)
                    {
                        Label tmp = (Label)control;
                        tmp.Foreground = Brushes.White;
                        tmp.FontSize = 16;
                        headerPanel.Children.Add(tmp);
                    }
                }
            }
            if (rControls != null)
            {
                foreach (Object control in rControls)
                {
                    if (control is Button)
                    {
                        Button tmp = (Button)control;
                        tmp.Height = 30;
                        tmp.Width = 30;
                        tmp.Foreground = Brushes.White;
                        tmp.BorderBrush = Brushes.Transparent;
                        tmp.Background = Brushes.Transparent;
                        tmp.HorizontalAlignment = HorizontalAlignment.Right;

                        DockPanel.SetDock(tmp, Dock.Right);
                        headerPanel.Children.Add(tmp);
                    }
                    if (control is Label)
                    {
                        Label tmp = (Label)control;
                        tmp.Foreground = Brushes.White;
                        tmp.FontSize = 16;
                        headerPanel.Children.Add(tmp);
                    }
                }
            }

            DockPanel.SetDock(headerPanel, Dock.Top);
            dockPanel.Children.Add(headerPanel);
            try
            {
                RegisterName("headerPanel", headerPanel);
            }
            catch (Exception ex)
            {
                UnregisterName("headerPanel");
                RegisterName("headerPanel", headerPanel);
            }

            LeftSide.Children.Add(dockPanel);
            return dockPanel;
        }
        void Search(object sender, RoutedEventArgs e)
        {
            ListBox listbox = (ListBox)FindName("listbox");
            TextBox searchBox = (TextBox)FindName("SearchBox");
            listbox.Items.Clear();

            if (searchBox == null)
            {
                searchBox = new TextBox();
                RegisterName("SearchBox", searchBox);
                ((DockPanel)FindName("headerPanel")).Children.Add(searchBox);
            }
            else
            {
                if ((string)((Button)sender).Tag == "meeting")
                {
                    foreach (Meeting meeting in meetingModel.GetAll())
                    {
                        ListBoxItem lbi = new ListBoxItem();
                        lbi.Content = meeting.name;
                        lbi.Tag = meeting.id;
                        lbi.Uid = meeting.id.ToString();
                        listbox.Items.Add(lbi);
                    }
                }
                else if ((string)((Button)sender).Tag == "user")
                {
                    foreach (User user in userModel.GetAll())
                    {
                        ListBoxItem lbi = new ListBoxItem();
                        lbi.Content = user.fname + " " + user.lname;
                        lbi.Tag = user.id;
                        lbi.Uid = user.id.ToString();
                        listbox.Items.Add(lbi);
                    }
                }
                
                for (int i = listbox.Items.Count - 1; i >= 0; i--)
                {
                    Regex regex = new Regex(@"[a-zA-Z0-9]*" + searchBox.Text.ToLower() + @"[a-zA-Z0-9]*");
                    Match match = regex.Match(((string)((ListBoxItem)listbox.Items[i]).Content).ToLower());
                    if (!match.Success)
                    {
                        listbox.Items.Remove((ListBoxItem)listbox.Items[i]);
                    }
                }
            }
        }

        void MarkDayAsInactive(object sender, RoutedEventArgs e)
        {
            Calendar calendar = (Calendar)((Button)sender).Tag;
            AskTheReason ask = new AskTheReason();
            ask.ShowDialog();
            if ((bool)ask.DialogResult)
            {
                if ((DateTime)calendar.SelectedDate == null)
                {
                    meetingController.CancelMeetingsByDate(DateTime.Today, ask.text);
                }
                meetingController.CancelMeetingsByDate((DateTime)calendar.SelectedDate, ask.text);
            }
        }
        void OnSelectedDateChange(object sender, RoutedEventArgs e)
        {
            Calendar calendar = (Calendar)sender;

            LeftSide.Tag = calendar.SelectedDate;
            LeftSide.Children.Clear();

            List<Object> list = new List<Object>();
            Label header = new Label();
            header.Content = "Тухайн өдрийн хурлууд";
            list.Add(header);

            Button rButton = new Button();

            Image calendarImage = new Image();
            calendarImage.Source = new BitmapImage(new Uri("images/cal-+.png", UriKind.Relative));
            calendarImage.Width = 20;
            calendarImage.Height = 20;
            rButton.Content = calendarImage;
            rButton.Click += ShowEventAdder;

            Button xButton = new Button();
            Image xImage = new Image();
            xImage.Source = new BitmapImage(new Uri("images/cal-x.png", UriKind.Relative));
            xImage.Width = 20;
            xImage.Height = 20;
            xButton.Tag = calendar;

            xButton.Content = xImage;
            xButton.Click += MarkDayAsInactive;
            List<Object> rControls = new List<Object>();
            rControls.Add(rButton);
            rControls.Add(xButton);
            DockPanel dockPanel = addHeader(list, rControls);

            ListBox listbox = new ListBox();
            listbox.Margin = new Thickness(10, 10, 10, 20);
            if ((DateTime)calendar.SelectedDate >= DateTime.Today)
            {
                List<Meeting> meetings = meetingController.FindByDate((DateTime)calendar.SelectedDate);
                int i = 1;
                foreach (Meeting meeting in meetings)
                {
                    if (meeting.isDeleted) continue;
                    ListBoxItem listBoxItem = new ListBoxItem();
                    if (meeting.duration == 0) listBoxItem.Content = listBoxItem.Content = i + ". " + meeting.name + ", " + meeting.startDatetime.ToString("HH:mm") + " цагийн хурал цуцлагдсан";
                    else listBoxItem.Content = i + ". " + meeting.name + ", " + meeting.startDatetime.ToString("HH:mm") + ", " + meeting.duration + " минут";

                    listBoxItem.Uid = meeting.id.ToString();
                    listBoxItem.Height = 25;
                    listBoxItem.Tag = meeting; //done fixing. Try it now
                    listbox.Items.Add(listBoxItem);
                    i++;
                }
                listbox.SelectionChanged += ModifyMeeting;
            }

            dockPanel.Children.Add(listbox);

        }
        /** Shows Calendar on rightside
         * And meetings that will happen on that day on the Left side
         * 
         */
        void ShowCalendar(object sender, RoutedEventArgs e)
        {
            is_home = false;
            RightSide.Children.Clear();
            LeftSide.Children.Clear();
            LeftSide.Tag = DateTime.Now;

            List<Object> list = new List<Object>();
            Label header = new Label();
            header.Content = "Тухайн өдрийн хурлууд:";
            header.Foreground = Brushes.White;
            list.Add(header);


            Button rButton = new Button();
            Image calendarImage = new Image();
            calendarImage.Source = new BitmapImage(new Uri("images/cal-+.png", UriKind.Relative));
            calendarImage.Width = 20;
            calendarImage.Height = 20;

            Button xButton = new Button();
            Image xImage = new Image();
            xImage.Source = new BitmapImage(new Uri("images/cal-x.png", UriKind.Relative));
            xImage.Width = 20;
            xImage.Height = 20;
            xButton.Content = xImage;
            xButton.Click += MarkDayAsInactive;

            rButton.Content = calendarImage;
            rButton.Click += ShowEventAdder;

            xButton.Content = xImage;

            List<Object> rControls = new List<Object>();
            rControls.Add(rButton);
            rControls.Add(xButton);

            DockPanel dockPanel = addHeader(list, rControls);

            Viewbox ll = new Viewbox();

            dockPanel.Children.Add(ll);

            Calendar calendar = new Calendar();
            ImageBrush calendarBackround = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "images/time-logo.png")));
            calendarBackround.Opacity = 0.3;
            calendar.Background = calendarBackround;
            Viewbox viewbox = new Viewbox();
            viewbox.Stretch = Stretch.Uniform;
            viewbox.Child = calendar;
            calendar.SelectedDatesChanged += OnSelectedDateChange;
            calendar.SelectedDate = DateTime.Today;
            RightSide.Children.Add(viewbox);


            ListBox listbox = new ListBox();
            listbox.Margin = new Thickness(10, 10, 10, 20);
            if ((DateTime)calendar.SelectedDate < DateTime.Today)
            {
                List<ArchivedMeeting> meetings = meetingController.GetArchivedMeetingByDate((DateTime)calendar.SelectedDate);
                foreach (ArchivedMeeting meeting in meetings)
                {
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = meeting.name;
                    listBoxItem.Uid = meeting.id.ToString();
                    listBoxItem.Height = 25;
                    listbox.Items.Add(listBoxItem);
                }
            }
            else
            {
                List<Meeting> meetings = meetingController.FindByDate((DateTime)calendar.SelectedDate);
                int i = 1;
                foreach (Meeting meeting in meetings)
                {
                    if (meeting.isDeleted) continue;
                    ListBoxItem listBoxItem = new ListBoxItem();
                    if (meeting.duration == 0) listBoxItem.Content = listBoxItem.Content = i + ". " + meeting.name + ", " + meeting.startDatetime.ToString("HH:mm") + " цагийн хурал цуцлагдсан";
                    else listBoxItem.Content = i + ". " + meeting.name + ", " + meeting.startDatetime.ToString("HH:mm") + ", " + meeting.duration + " минут";

                    listBoxItem.Uid = meeting.id.ToString();
                    listBoxItem.Height = 25;
                    listBoxItem.Tag = meeting; //done fixing. Try it now
                    listbox.Items.Add(listBoxItem);
                    i++;
                }
                listbox.SelectionChanged += ModifyMeeting;
            }


            dockPanel.Children.Add(listbox);
        }
        
        void RemoveFromList(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (ListBox)((Button)sender).Tag;
            var selectedItems = listBox.SelectedItems;

            if (listBox.SelectedIndex != -1)
            {
                for (int i = selectedItems.Count - 1; i >= 0; i--)
                    listBox.Items.Remove(selectedItems[i]);
            }
            else
                Xceed.Wpf.Toolkit.MessageBox.Show("Та жагсаалтаас хасах хүмүүсээ эхлээд сонгоно уу.");
        }

        void addMeeting(object sender, RoutedEventArgs e)
        {
            RightSide.Children.Clear();

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.Margin = new Thickness(10);

            List<Object> controls = new List<Object>();

            /**
             * Name stack
             */

            StackPanel nameStack = new StackPanel();
            nameStack.Orientation = Orientation.Horizontal;
            nameStack.Margin = new Thickness(0, 5, 0, 5);

            Label nameLabel = new Label();
            nameLabel.Content = "Хурлын нэр:";
            nameLabel.Width = 200;
            TextBox name = new TextBox();
            name.Width = 200;
            controls.Add(name);

            nameStack.Children.Add(nameLabel);
            nameStack.Children.Add(name);

            /**
             * Starting time stack
             */

            StackPanel stStack = new StackPanel();
            stStack.Orientation = Orientation.Horizontal;
            stStack.Margin = new Thickness(0, 5, 0, 5);

            Label stLabel = new Label();
            stLabel.Content = "Хурал эхлэх цаг:";
            stLabel.Width = 200;
            TimePicker st = new TimePicker();
            st.Width = 200;
            controls.Add(st);

            stStack.Children.Add(stLabel);
            stStack.Children.Add(st);

            /**
             * Starting date stack
             */
            StackPanel sdStack = new StackPanel();
            sdStack.Orientation = Orientation.Horizontal;
            sdStack.Margin = new Thickness(0, 5, 0, 5);

            Label sdLabel = new Label();
            sdLabel.Content = "Хурал эхлэх өдөр:";
            sdLabel.Width = 200;
            DatePicker sd = new DatePicker();
            sd.DisplayDateStart = DateTime.Now;
            sd.Width = 200;
            controls.Add(sd);

            sdStack.Children.Add(sdLabel);
            sdStack.Children.Add(sd);

            /**Registration start time
             */
            StackPanel regStartStack = new StackPanel();
            regStartStack.Orientation = Orientation.Horizontal;
            regStartStack.Margin = new Thickness(0, 5, 0, 5);

            TextBlock regStartLabel = new TextBlock();
            regStartLabel.Text = "Хурлаас хэдэн минутын өмнө бүртгэл эхлэх:";
            regStartLabel.TextWrapping = TextWrapping.Wrap;
            regStartLabel.Width = 200;
            TextBox regStart = new TextBox();
            regStart.Width = 200;
            controls.Add(regStart);

            regStartStack.Children.Add(regStartLabel);
            regStartStack.Children.Add(regStart);
            /**
                * duration Stack
                */

            StackPanel durationStack = new StackPanel();
            durationStack.Orientation = Orientation.Horizontal;
            durationStack.Margin = new Thickness(0, 5, 0, 5);

            Label durationLabel = new Label();
            durationLabel.Content = "Хурал үргэлжлэх хугацаа минутаар:";
            durationLabel.Width = 200;
            TextBox duration = new TextBox();
            duration.Width = 100;
            controls.Add(duration);

            Label uDurationUnit = new Label();
            uDurationUnit.Content = "Минут";

            durationStack.Children.Add(durationLabel);
            durationStack.Children.Add(duration);
            durationStack.Children.Add(uDurationUnit);

            /**
                * ending date Stack
                */
            StackPanel edStack = new StackPanel();
            edStack.Orientation = Orientation.Horizontal;
            edStack.Margin = new Thickness(0, 5, 0, 5);

            Label edLabel = new Label();
            edLabel.Content = "Хурал дуусах өдөр:";
            edLabel.Width = 200;
            DatePicker ed = new DatePicker();
            ed.DisplayDateStart = DateTime.Now;
            ed.Width = 200;
            controls.Add(ed);
            edStack.Children.Add(edLabel);
            edStack.Children.Add(ed);

            /**
             * Frequency stack
             */
            StackPanel fStack = new StackPanel();
            fStack.Orientation = Orientation.Horizontal;
            Label fLabel = new Label();
            fLabel.Content = "Хурал болох давтамж";
            fLabel.Width = 200;
            ComboBox freqType = new ComboBox();
            for (int i = 0; i < 7; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                switch (i)
                {
                    case 0:
                        item.Content = "1 удаагийн хурал";
                        item.Tag = 0;
                        break;
                    case 1:
                        item.Content = "7 хоног бүр";
                        item.Tag = 1;
                        break;
                    case 2:
                        item.Content = "14 хоног бүрт";
                        item.Tag = 2;
                        break;
                    case 3:
                        item.Content = "Сар бүр";
                        item.Tag = 3;
                        break;
                    case 4:
                        item.Content = "Сар бүрийн эхэнд";
                        item.Tag = 4;
                        break;
                    case 5:
                        item.Content = "Сар бүрийн төгсгөлд";
                        item.Tag = 5;
                        break;
                    case 6:
                        item.Content = "Жил бүр";
                        item.Tag = 6;
                        break;
                }
                freqType.Items.Add(item);
            }
            freqType.Width = 200;
            controls.Add(freqType);


            fStack.Children.Add(fLabel);
            fStack.Children.Add(freqType);
            /**Participating groups
             */
            StackPanel pStack = new StackPanel();
            Label pGroupsLabel = new Label();
            pGroupsLabel.Content = "Оролцогч албууд";
            ListBox pGroupList = new ListBox();
            controls.Add(pGroupList);
            pGroupList.Margin = new Thickness(0, 0, 0, 10);
            pGroupList.SelectionMode = SelectionMode.Multiple;
            try
            {
                RegisterName("Groups", pGroupList);
            }
            catch (Exception ex)
            {
                UnregisterName("Groups");
                RegisterName("Groups", pGroupList);
            }

            DockPanel gButtonPanel = new DockPanel();
            gButtonPanel.HorizontalAlignment = HorizontalAlignment.Stretch;

            Button gRemoveButton = new Button();
            gRemoveButton.Content = "Хасах";
            gRemoveButton.Tag = pGroupList;
            gRemoveButton.Click += RemoveFromList;
            gRemoveButton.Background = Brushes.White;
            gRemoveButton.Width = 50;

            Button gAddButton = new Button();
            gAddButton.Content = "Нэмэх";
            gAddButton.Background = Brushes.White;
            gAddButton.Width = 50;
            DockPanel.SetDock(gAddButton, Dock.Right);
            gAddButton.HorizontalAlignment = HorizontalAlignment.Right;
            List<Object> typeAndList = new List<Object>();

            typeAndList.Add("group");
            typeAndList.Add(pGroupList);

            gAddButton.Tag = typeAndList;
            gAddButton.Click += addParticipantsToMeeting;

            gButtonPanel.Children.Add(gRemoveButton);
            gButtonPanel.Children.Add(gAddButton);

            pStack.Children.Add(pGroupsLabel);
            pStack.Children.Add(pGroupList);
            pStack.Children.Add(gButtonPanel);

            Label participantsLabel = new Label();
            participantsLabel.Content = "Оролцогч гишүүд:";
            ListBox pUserList = new ListBox();
            controls.Add(pUserList);
            pUserList.Margin = new Thickness(0, 0, 0, 10);
            pUserList.SelectionMode = SelectionMode.Multiple;
            try
            {
                RegisterName("Users", pUserList);
            }
            catch (Exception ex)
            {
                UnregisterName("Users");
                RegisterName("Users", pUserList);
            }


            DockPanel uButtonPanel = new DockPanel();
            uButtonPanel.HorizontalAlignment = HorizontalAlignment.Stretch;

            Button uRemoveButton = new Button();
            uRemoveButton.Content = "Хасах";
            uRemoveButton.Tag = pUserList;
            uRemoveButton.Click += RemoveFromList;
            uRemoveButton.Background = Brushes.White;
            uRemoveButton.Width = 50;

            Button uAddButton = new Button();
            uAddButton.Content = "Нэмэх";
            uAddButton.Background = Brushes.White;
            uAddButton.Width = 50;
            DockPanel.SetDock(uAddButton, Dock.Right);
            uAddButton.HorizontalAlignment = HorizontalAlignment.Right;
            List<Object> utypeAndList = new List<Object>();

            utypeAndList.Add("user");
            utypeAndList.Add(pUserList);

            uAddButton.Tag = utypeAndList;
            uAddButton.Click += addParticipantsToMeeting;

            uButtonPanel.Children.Add(uRemoveButton);
            uButtonPanel.Children.Add(uAddButton);

            pStack.Children.Add(participantsLabel);
            pStack.Children.Add(pUserList);
            pStack.Children.Add(uButtonPanel);

            Label pPositionLabel = new Label();
            pPositionLabel.Content = "Оролцогч албан тушаалтнууд:";
            ListBox pPositionList = new ListBox();
            controls.Add(pPositionList);
            pPositionList.Margin = new Thickness(0, 0, 0, 10);
            pPositionList.SelectionMode = SelectionMode.Multiple;
            try
            {
                RegisterName("Positions", pPositionList);
            }
            catch
            {
                UnregisterName("Positions");
                RegisterName("Positions", pPositionList);
            }

            DockPanel posButtonPanel = new DockPanel();
            posButtonPanel.HorizontalAlignment = HorizontalAlignment.Stretch;

            Button posRemoveButton = new Button();
            posRemoveButton.Content = "Хасах";
            posRemoveButton.Tag = pPositionList;
            posRemoveButton.Click += RemoveFromList;
            posRemoveButton.Background = Brushes.White;
            posRemoveButton.Width = 50;

            Button posAddButton = new Button();
            posAddButton.Content = "Нэмэх";
            posAddButton.Background = Brushes.White;
            posAddButton.Width = 50;
            DockPanel.SetDock(posAddButton, Dock.Right);
            posAddButton.HorizontalAlignment = HorizontalAlignment.Right;
            List<Object> ptypeAndList = new List<Object>();

            ptypeAndList.Add("position");
            ptypeAndList.Add(pPositionList);

            posAddButton.Tag = ptypeAndList;
            posAddButton.Click += addParticipantsToMeeting;

            posButtonPanel.Children.Add(posRemoveButton);
            posButtonPanel.Children.Add(posAddButton);

            pStack.Children.Add(pPositionLabel);
            pStack.Children.Add(pPositionList);
            pStack.Children.Add(posButtonPanel);

            /**
             * Save button 
             */
            StackPanel saveStack = new StackPanel();
            saveStack.HorizontalAlignment = HorizontalAlignment.Right;
            saveStack.Margin = new Thickness(0, 5, 0, 5);

            Button saveButton = new Button();
            saveButton.Content = "Хадгалах";
            saveButton.Background = Brushes.White;
            saveButton.Height = 25;
            saveButton.Width = 100;
            saveButton.Tag = controls;
            saveButton.Click += insertMeeting;
            saveStack.Children.Add(saveButton);

            stackPanel.Children.Add(nameStack);
            stackPanel.Children.Add(stStack);
            stackPanel.Children.Add(sdStack);
            stackPanel.Children.Add(regStartStack);

            stackPanel.Children.Add(durationStack);
            stackPanel.Children.Add(edStack);
            stackPanel.Children.Add(fStack);

            stackPanel.Children.Add(pStack);
            stackPanel.Children.Add(saveStack);

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Content = stackPanel;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            if (FindName("scroll") != null) UnregisterName("scroll");
            RegisterName("scroll", scrollViewer);

            scrollViewer.Height = ActualHeight - 50;
            scrollViewer.Margin = new Thickness(0, 0, 0, 0);
            RightSide.Children.Add(scrollViewer);
        }
        void insertMeeting(object sender, RoutedEventArgs e)
        {
            List<Object> controls = (List<Object>)((Button)sender).Tag;
            for (int i = 0; i < 9; i++)
            {
                if (controls[i] == null)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("Та бүрэн бөглөнө үү", "Амжилтгүй");
                    return;
                }
            }
            string name = ((TextBox)controls[0]).Text;
            string st = ((TimePicker)controls[1]).Text;
            string sd = ((DateTime)((DatePicker)controls[2]).SelectedDate).ToShortDateString();
            string regStart = ((TextBox)controls[3]).Text;
            string duration = ((TextBox)controls[4]).Text;
            string ed = ((DatePicker)controls[5]).Text;
            ComboBox freqType = (ComboBox)controls[6];
            ListBox pDeps = (ListBox)controls[7];
            ListBox pUsers = (ListBox)controls[8];
            ListBox pPositions = (ListBox)controls[9];

            Meeting meeting = new Meeting();
            meeting.name = name;
            /**checking time and inserting*/
            try
            {
                meeting.startDatetime = DateTime.Parse(sd + " " + st);
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Та хурал эхлэх цагаа цаг:минут гэсэн хэлбэртэйгээр оруулна уу!");
                return;
            }
            /**checking time and inserting*/
            try
            {
                meeting.regMinBefMeeting = Int32.Parse(regStart);
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Хурлын бүртгэл хэдэн минутын өмнө эхлэх талбарыг зөв бөглөнө үү");
                return;
            }
            /** checking and inserting duration*/
            try
            {
                meeting.duration = Int32.Parse(duration);
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Та үргэлжлэх хугацаагаа зөв оруулна уу!\n" +
                      "Зөвхөн үргэлжлэх хугацааг минутаар илэхийлэх тоо байхыг анхаарна уу!");
                return;
            }
            /** checking and inserting duration*/
            if (ed.Trim() != "" && ed != null)
            {
                meeting.endDate = DateTime.Parse(ed);
            }
            else
            {
                meeting.endDate = new DateTime();
            }
            /** checking and inserting interval()freq*/
            try
            {
                int type = Byte.Parse(((ComboBoxItem)freqType.SelectedItem).Tag.ToString());
                if (type == 1)
                {
                    meeting.intervalType = 7;
                    meeting.intervalDay = 7;
                }
                else if (type == 2)
                {
                    meeting.intervalType = 7;
                    meeting.intervalDay = 14;
                }
                else
                {
                    meeting.intervalDay = type;
                }
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Та хурал болох давтамжаа  шалгана уу", "Өөрчилсөнгүй");
                return;
            }
            /** checking and inserting meeting itself*/
            int meetingid = meetingModel.Add(meeting);
            if (meetingid != -1)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Амжилттай нэмлээ!");

                /**Checking and inserting ppositions*/
                try
                {
                    foreach (ListBoxItem listBoxItem in pUsers.Items)
                    {
                        MeetingAndUser mau = new MeetingAndUser();
                        mau.meetingId = meetingid;
                        mau.userId = (Int32)listBoxItem.Tag;
                        mauModel.Add(mau);
                    }
                }
                catch (Exception ex)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("Бүтэлгүйтлээ.");
                }
                try
                {
                    foreach (ListBoxItem listBoxItem in pDeps.Items)
                    {
                        MeetingAndDepartment mad = new MeetingAndDepartment();
                        mad.meetingId = meetingid;
                        mad.departmentId = (Int32)listBoxItem.Tag;
                        madModel.Add(mad);
                    }
                }
                catch (Exception ex)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("Бүтэлгүйтлээ.");
                }
                try
                {
                    foreach (ListBoxItem listBoxItem in pPositions.Items)
                    {
                        MeetingAndPosition map = new MeetingAndPosition();
                        map.meetingId = meetingid;
                        map.positionId = (Int32)listBoxItem.Tag;
                        mapModel.Add(map);
                    }
                }
                catch (Exception ex)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("Бүтэлгүйтлээ.");
                }
                ShowMeetings();
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Өгөгдлийн сантай холбоотой асуудал гарлаа.",
                    "Бүтэлгүйтлээ!");
            }

        }
        /**to insert to modified meeting
         */

        void onMeetingNameChanged(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            if ((ListBoxItem)listBox.SelectedValue == null)
            {
                return;
            }
            Label label = new Label();
            string id = ((ListBoxItem)listBox.SelectedValue).Uid;

            RightSide.Children.Clear();

            Meeting meeting = meetingModel.Get(Int32.Parse(id));
            List<Object> controls = new List<Object>();

            StackPanel stackPanel = new StackPanel();
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanel.VerticalAlignment = VerticalAlignment.Center;
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.Margin = new Thickness(10, 10, 10, 10);

            /**
                * Name stack
                */

            StackPanel nameStack = new StackPanel();
            nameStack.Orientation = Orientation.Horizontal;
            nameStack.Margin = new Thickness(0, 5, 0, 5);

            Label nameLabel = new Label();
            nameLabel.Content = "Хурлын нэр:";
            nameLabel.Width = 200;
            TextBox name = new TextBox();
            name.Width = 200;
            name.Text = meeting.name;
            controls.Add(name);

            nameStack.Children.Add(nameLabel);
            nameStack.Children.Add(name);

            /**
                * Starting time stack
                */

            StackPanel stStack = new StackPanel();
            stStack.Orientation = Orientation.Horizontal;
            stStack.Margin = new Thickness(0, 5, 0, 5);

            Label stLabel = new Label();
            stLabel.Content = "Хурал эхлэх цаг:";
            stLabel.Width = 200;
            TimePicker st = new TimePicker();
            st.Width = 200;
            st.Text = ((DateTime)meeting.startDatetime).ToShortTimeString();
            controls.Add(st);

            stStack.Children.Add(stLabel);
            stStack.Children.Add(st);

            /**
                * Starting date stack
                */
            StackPanel sdStack = new StackPanel();
            sdStack.Orientation = Orientation.Horizontal;
            sdStack.Margin = new Thickness(0, 5, 0, 5);

            Label sdLabel = new Label();
            sdLabel.Content = "Хурал эхлэх өдөр:";
            sdLabel.Width = 200;
            DatePicker sd = new DatePicker();
            sd.Width = 200;
            sd.DisplayDate = ((DateTime)meeting.startDatetime);
            sd.Text = ((DateTime)meeting.startDatetime).ToShortDateString();
            controls.Add(sd);

            sdStack.Children.Add(sdLabel);
            sdStack.Children.Add(sd);
            /**Registration start time
 */
            StackPanel regStartStack = new StackPanel();
            regStartStack.Orientation = Orientation.Horizontal;
            regStartStack.Margin = new Thickness(0, 5, 0, 5);

            TextBlock regStartLabel = new TextBlock();
            regStartLabel.Text = "Хурлаас хэдэн минутын өмнө бүртгэл эхлэх:";
            regStartLabel.TextWrapping = TextWrapping.Wrap;
            regStartLabel.Width = 200;
            TextBox regStart = new TextBox();
            regStart.Text = meeting.regMinBefMeeting.ToString();
            regStart.Width = 200;
            controls.Add(regStart);

            regStartStack.Children.Add(regStartLabel);
            regStartStack.Children.Add(regStart);
            /**
                * ending time Stack
                */

            StackPanel durationStack = new StackPanel();
            durationStack.Orientation = Orientation.Horizontal;
            durationStack.Margin = new Thickness(0, 5, 0, 5);

            Label durationLabel = new Label();
            durationLabel.Content = "Хурал үргэлжлэх хугацаа:";
            durationLabel.Width = 200;
            TextBox duration = new TextBox();
            duration.Width = 100;
            duration.Text = meeting.duration.ToString();
            controls.Add(duration);
            Label uDurationUnit = new Label();
            uDurationUnit.Content = "Минут";

            durationStack.Children.Add(durationLabel);
            durationStack.Children.Add(duration);
            durationStack.Children.Add(uDurationUnit);

            /**
                * ending date Stack
                */
            StackPanel edStack = new StackPanel();
            edStack.Orientation = Orientation.Horizontal;
            edStack.Margin = new Thickness(0, 5, 0, 5);

            Label edLabel = new Label();
            edLabel.Content = "Хурал дуусах өдөр:";
            edLabel.Width = 200;
            DatePicker ed = new DatePicker();
            ed.Width = 200;
            ed.DisplayDate = ((DateTime)meeting.startDatetime);
            ed.Text = ((DateTime)meeting.endDate).ToShortDateString();
            controls.Add(ed);
            edStack.Children.Add(edLabel);
            edStack.Children.Add(ed);

            /**
                * Frequency stack
                */
            StackPanel fStack = new StackPanel();
            fStack.Orientation = Orientation.Horizontal;
            fStack.Margin = new Thickness(0, 5, 0, 5);

            Label fLabel = new Label();
            fLabel.Content = "Хурал болох давтамж";
            fLabel.Width = 200;
            ComboBox freqType = new ComboBox();
            for (int i = 0; i < 8; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                switch (i)
                {
                    case 0:
                        item.Content = "1 удаагийн хурал";
                        item.Tag = 0;
                        break;
                    case 1:
                        item.Content = "7 хоног бүр";
                        item.Tag = 1;
                        break;
                    case 2:
                        item.Content = "14 хоног бүрт";
                        item.Tag = 2;
                        break;
                    case 3:
                        item.Content = "Сар бүр";
                        item.Tag = 3;
                        break;
                    case 4:
                        item.Content = "Сар бүрийн эхэнд";
                        item.Tag = 4;
                        break;
                    case 5:
                        item.Content = "Сар бүрийн төгсгөлд";
                        item.Tag = 5;
                        break;
                    case 6:
                        item.Content = "Жил бүр";
                        item.Tag = 6;
                        break;
                }
                freqType.Items.Add(item);
            }
            freqType.Width = 200;
            int it = meeting.intervalType;
            if (meeting.intervalType == 7)
            {
                it = meeting.intervalDay == 7 ? 1 : 2;
            }
            freqType.SelectedIndex = it;
            controls.Add(freqType);


            fStack.Children.Add(fLabel);
            fStack.Children.Add(freqType);

            /** Add user, group part
             * 

             */
            StackPanel pStack = new StackPanel();
            pStack.Orientation = Orientation.Vertical;

            /**Participating groups
             */
            Label pGroupsLabel = new Label();
            pGroupsLabel.Content = "Оролцогч албууд:";
            ListBox pGroupList = new ListBox();
            pGroupList.MaxHeight = 105;
            List<MeetingAndDepartment> mads = madModel.GetByFK(meeting.IDName, meeting.id);
            foreach (MeetingAndDepartment mad in mads)
            {
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = depModel.Get(mad.departmentId).name;
                listBoxItem.Tag = mad.departmentId;
                pGroupList.Items.Add(listBoxItem);
            }
            controls.Add(pGroupList);
            pGroupList.Margin = new Thickness(0, 0, 0, 10);
            pGroupList.SelectionMode = SelectionMode.Multiple;
            try
            {
                RegisterName("Groups", pGroupList);
            }
            catch
            {
                UnregisterName("Groups");
                RegisterName("Groups", pGroupList);
            }

            DockPanel gButtonPanel = new DockPanel();
            gButtonPanel.HorizontalAlignment = HorizontalAlignment.Stretch;

            Button gRemoveButton = new Button();
            gRemoveButton.Content = "Хасах";
            gRemoveButton.Background = Brushes.White;
            gRemoveButton.Tag = pGroupList;
            gRemoveButton.Click += RemoveFromList;
            gRemoveButton.Width = 50;

            Button gAddButton = new Button();
            gAddButton.Content = "Нэмэх";
            gAddButton.Background = Brushes.White;
            gAddButton.Width = 50;
            DockPanel.SetDock(gAddButton, Dock.Right);
            gAddButton.HorizontalAlignment = HorizontalAlignment.Right;
            List<Object> typeAndList = new List<Object>();

            typeAndList.Add("group");
            typeAndList.Add(pGroupList);

            gAddButton.Tag = typeAndList;
            gAddButton.Click += addParticipantsToMeeting;

            gButtonPanel.Children.Add(gRemoveButton);
            gButtonPanel.Children.Add(gAddButton);

            pStack.Children.Add(pGroupsLabel);
            pStack.Children.Add(pGroupList);
            pStack.Children.Add(gButtonPanel);

            Label participantsLabel = new Label();
            participantsLabel.Content = "Оролцогч гишүүд:";
            ListBox pUserList = new ListBox();
            pUserList.MaxHeight = 105;
            controls.Add(pUserList);
            pUserList.Margin = new Thickness(0, 0, 0, 10);
            pUserList.SelectionMode = SelectionMode.Multiple;
            List<MeetingAndUser> maus = mauModel.GetByFK(meeting.IDName, meeting.id);
            foreach (MeetingAndUser mau in maus)
            {
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = userModel.Get(mau.userId).fname + " " + userModel.Get(mau.userId).lname;
                listBoxItem.Tag = userModel.Get(mau.userId).id;
                pUserList.Items.Add(listBoxItem);
            }
            //Registering the name
            try
            {
                RegisterName("Users", pUserList);
            }
            catch (Exception ex)
            {
                UnregisterName("Users");
                RegisterName("Users", pUserList);
            }
            DockPanel uButtonPanel = new DockPanel();
            uButtonPanel.HorizontalAlignment = HorizontalAlignment.Stretch;

            Button uRemoveButton = new Button();
            uRemoveButton.Content = "Хасах";
            uRemoveButton.Background = Brushes.White;
            uRemoveButton.Tag = pUserList;
            uRemoveButton.Click += RemoveFromList;
            uRemoveButton.Width = 50;

            Button uAddButton = new Button();
            uAddButton.Content = "Нэмэх";
            uAddButton.Background = Brushes.White;
            uAddButton.Width = 50;
            DockPanel.SetDock(uAddButton, Dock.Right);
            uAddButton.HorizontalAlignment = HorizontalAlignment.Right;
            List<Object> utypeAndList = new List<Object>();

            utypeAndList.Add("user");
            utypeAndList.Add(pUserList);

            uAddButton.Tag = utypeAndList;
            uAddButton.Click += addParticipantsToMeeting;

            uButtonPanel.Children.Add(uRemoveButton);
            uButtonPanel.Children.Add(uAddButton);

            pStack.Children.Add(participantsLabel);
            pStack.Children.Add(pUserList);
            pStack.Children.Add(uButtonPanel);

            Label pPositionLabel = new Label();
            pPositionLabel.Content = "Оролцогч албан тушаалтнууд:";
            ListBox pPositionList = new ListBox();
            pPositionList.MaxHeight = 105;

            List<MeetingAndPosition> maps = mapModel.GetByFK(meeting.IDName, meeting.id);
            foreach (MeetingAndPosition map in maps)
            {
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = posModel.Get(map.positionId).name;
                listBoxItem.Tag = map.positionId;
                pPositionList.Items.Add(listBoxItem);
            }
            controls.Add(pPositionList);
            pPositionList.Margin = new Thickness(0, 0, 0, 10);
            pPositionList.SelectionMode = SelectionMode.Multiple;
            try
            {
                RegisterName("Positions", pPositionList);
            }
            catch
            {
                UnregisterName("Positions");
                RegisterName("Positions", pPositionList);
            }

            DockPanel posButtonPanel = new DockPanel();
            posButtonPanel.HorizontalAlignment = HorizontalAlignment.Stretch;

            Button posRemoveButton = new Button();
            posRemoveButton.Content = "Хасах";
            posRemoveButton.Background = Brushes.White;
            posRemoveButton.Tag = pPositionList;
            posRemoveButton.Click += RemoveFromList;
            posRemoveButton.Width = 50;

            Button posAddButton = new Button();
            posAddButton.Content = "Нэмэх";
            posAddButton.Background = Brushes.White;
            posAddButton.Width = 50;
            DockPanel.SetDock(posAddButton, Dock.Right);
            posAddButton.HorizontalAlignment = HorizontalAlignment.Right;
            List<Object> ptypeAndList = new List<Object>();

            ptypeAndList.Add("position");
            ptypeAndList.Add(pPositionList);

            posAddButton.Tag = ptypeAndList;
            posAddButton.Click += addParticipantsToMeeting;

            posButtonPanel.Children.Add(posRemoveButton);
            posButtonPanel.Children.Add(posAddButton);

            pStack.Children.Add(pPositionLabel);
            pStack.Children.Add(pPositionList);
            pStack.Children.Add(posButtonPanel);
            /**
            * Save button 
            */
            StackPanel saveStack = new StackPanel();
            saveStack.HorizontalAlignment = HorizontalAlignment.Right;
            saveStack.Orientation = Orientation.Horizontal;
            saveStack.Margin = new Thickness(0, 5, 0, 5);

            Button saveButton = new Button();
            saveButton.Content = "Хадгалах";
            saveButton.Background = Brushes.White;
            saveButton.Height = 25;
            saveButton.Width = 100;

            Button removeButton = new Button();
            removeButton.Content = "Устгах";
            removeButton.Background = Brushes.White;
            removeButton.Height = 25;
            removeButton.Width = 100;

            controls.Add(meeting);

            saveButton.Tag = controls;
            saveButton.Click += setMeeting;

            removeButton.Tag = meeting;
            removeButton.Click += removeMeeting;

            saveStack.Children.Add(removeButton);
            saveStack.Children.Add(saveButton);

            stackPanel.Children.Add(nameStack);
            stackPanel.Children.Add(stStack);
            stackPanel.Children.Add(sdStack);
            stackPanel.Children.Add(regStartStack);
            stackPanel.Children.Add(durationStack);
            stackPanel.Children.Add(edStack);
            stackPanel.Children.Add(fStack);
            stackPanel.Children.Add(pStack);
            stackPanel.Children.Add(saveStack);

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Content = stackPanel;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            if (FindName("scroll") != null) UnregisterName("scroll");
            RegisterName("scroll", scrollViewer);

            scrollViewer.Height = ActualHeight - 50;
            scrollViewer.Margin = new Thickness(0, 0, 0, 0);
            RightSide.Children.Add(scrollViewer);
        }
        void setMeeting(object sender, RoutedEventArgs e)
        {
            List<Object> controls = (List<Object>)((Button)sender).Tag;
            TextBox name = (TextBox)controls[0];
            TimePicker st = (TimePicker)controls[1];
            DatePicker sd = (DatePicker)controls[2];
            TextBox rs = (TextBox)controls[3];

            TextBox duration = (TextBox)controls[4];
            DatePicker ed = (DatePicker)controls[5];
            ComboBox freqType = (ComboBox)controls[6];
            ListBox deps = (ListBox)controls[7];
            ListBox users = (ListBox)controls[8];
            ListBox positions = (ListBox)controls[9];
            Meeting meeting = (Meeting)controls[10];

            meeting.name = name.Text;

            string datetimeString = sd.Text + " " + st.Text;
            //check and set meeting startDateTime
            try
            {
                meeting.startDatetime = DateTime.Parse(datetimeString);
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Та цагаа цаг:минут гэсэн хэлбэрээр бичнэ үү!", "Өөрчилсөнгүй");
                return;
            }
            string endDateString = ed.Text + " " + meeting.endDate.ToShortTimeString();
            //check and set meeting endDate
            try
            {
                meeting.endDate = DateTime.Parse(endDateString);
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Та цагаа цаг:минут гэсэн хэлбэрээр бичнэ үү!", "Өөрчилсөнгүй");
                return;
            }
            //check and set meeting rs
            try
            {
                meeting.regMinBefMeeting = Int32.Parse(rs.Text);
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Та хурлын бүртгэл эхлэх хугацаагаа шалгана уу", "Өөрчилсөнгүй");
                return;
            }
            //check and set meeting duration
            try
            {
                meeting.duration = Int32.Parse(duration.Text);
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Та хурал болох хугацаагаа шалгана уу", "Өөрчилсөнгүй");
                return;
            }
            //check and set meeting intervalType(frq)
            try
            {
                meeting.intervalType = Byte.Parse(((ComboBoxItem)freqType.SelectedItem).Tag.ToString());
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Та хурал болох давтамжаа  шалгана уу", "Өөрчилсөнгүй");
                return;
            }
            //check and update meeting
            try
            {
                List<MeetingAndUser> oldMaus = mauModel.GetByFK(meeting.IDName, meeting.id);
                foreach (MeetingAndUser mau in oldMaus)
                {
                    mauModel.Remove(mau.id);
                }
                foreach (ListBoxItem user in users.Items)
                {
                    MeetingAndUser newMau = new MeetingAndUser();
                    newMau.meetingId = meeting.id;
                    newMau.userId = (int)user.Tag;
                    mauModel.Add(newMau);
                }
                List<MeetingAndDepartment> oldMads = madModel.GetByFK(meeting.IDName, meeting.id);
                foreach (MeetingAndDepartment mad in oldMads)
                {
                    madModel.Remove(mad.id);
                }
                foreach (ListBoxItem user in deps.Items)
                {
                    MeetingAndDepartment newMad = new MeetingAndDepartment();
                    newMad.meetingId = meeting.id;
                    newMad.departmentId = (int)user.Tag;
                    madModel.Add(newMad);
                }

                List<MeetingAndPosition> oldMaps = mapModel.GetByFK(meeting.IDName, meeting.id);
                foreach (MeetingAndPosition map in oldMaps)
                {
                    mapModel.Remove(map.id);
                }
                foreach (ListBoxItem user in positions.Items)
                {
                    MeetingAndPosition newMap = new MeetingAndPosition();
                    newMap.meetingId = meeting.id;
                    newMap.positionId = (int)user.Tag;
                    mapModel.Add(newMap);
                }
                meetingModel.Set(meeting);
                Xceed.Wpf.Toolkit.MessageBox.Show("Амжилттай өөрчиллөө.", "Өөрчлөгдлөө");
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Та программ ажиллуулж буй орчиноо шалгана уу", "Өөрчилсөнгүй");
            }
            ShowMeetings(sender, null);
        }
        void ShowEventAdder(object sender, RoutedEventArgs e)
        {
            RightSide.Children.Clear();

            List<Object> controls = new List<Object>(); //shine medeenuudiig zooh bus 

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.Margin = new Thickness(10);

            /**
                * Name stack
                */


            Label header = new Label
            {
                Content = "Тэмдэглэлт өдөр нэмэх",
                FontWeight = FontWeights.Bold,
                Width = 200
            };

            StackPanel nameStack = new StackPanel();
            nameStack.Orientation = Orientation.Horizontal;
            nameStack.Margin = new Thickness(0, 5, 0, 5);

            Label nameLabel = new Label
            {
                Content = "Тэмдэглэлт өдрийн нэр:",
                Width = 200
            };

            TextBox name = new TextBox
            {
                Width = 200
            };
            controls.Add(name);

            nameStack.Children.Add(nameLabel);
            nameStack.Children.Add(name);

            /**
                * Start date stack
                */

            StackPanel sdStack = new StackPanel();
            sdStack.Orientation = Orientation.Horizontal;
            sdStack.Margin = new Thickness(0, 5, 0, 5);

            Label sdLabel = new Label();
            sdLabel.Content = "Эхлэх огноо:";
            sdLabel.Width = 200;
            DatePicker sd = new DatePicker
            {
                SelectedDate = DateTime.Today,
                Width = 200
            };
            controls.Add(sd);

            sdStack.Children.Add(sdLabel);
            sdStack.Children.Add(sd);

            /**
                * End date stack
                */

            StackPanel edStack = new StackPanel();
            edStack.Orientation = Orientation.Horizontal;
            edStack.Margin = new Thickness(0, 5, 0, 5);

            Label edLabel = new Label();
            edLabel.Content = "Дуусах огноо:";
            edLabel.Width = 200;
            DatePicker ed = new DatePicker
            {
                SelectedDate = DateTime.Today,
                Width = 200
            };
            controls.Add(ed);

            edStack.Children.Add(edLabel);
            edStack.Children.Add(ed);

            /**
                * Interval Type stack
                */

            StackPanel itStack = new StackPanel();
            itStack.Orientation = Orientation.Horizontal;

            Label itLabel = new Label
            {
                Content = "Давтамжийн төрөл:",
                Width = 200
            };
            ComboBox it = new ComboBox
            {
                Width = 200
            };
            it.Items.Insert(0, "Жилийн энэ өдрүүдэд");
            it.Items.Insert(1, "Сарын энэ өдрүүдэд");
            it.Items.Insert(2, "Нэг удаагийн");
            it.SelectedIndex = 0;

            controls.Add(it);

            itStack.Children.Add(itLabel);
            itStack.Children.Add(it);

            /**
            * Save button 
            */
            StackPanel saveStack = new StackPanel();
            saveStack.HorizontalAlignment = HorizontalAlignment.Right;
            saveStack.Orientation = Orientation.Horizontal;
            saveStack.Margin = new Thickness(0, 5, 0, 5);

            Button saveButton = new Button();
            saveButton.Content = "Хадгалах";
            saveButton.Background = Brushes.White;
            saveButton.Height = 25;
            saveButton.Width = 100;

            saveButton.Tag = controls;
            saveButton.Click += AddEvent;

            Button backButton = new Button();
            backButton.Content = "Буцах";
            backButton.Background = Brushes.White;
            backButton.Height = 25;
            backButton.Width = 100;
            
            backButton.Click += ShowCalendar;

            saveStack.Children.Add(saveButton);
            saveStack.Children.Add(backButton);

            stackPanel.Children.Add(header);
            stackPanel.Children.Add(nameStack);
            stackPanel.Children.Add(sdStack);
            stackPanel.Children.Add(edStack);
            stackPanel.Children.Add(itStack);
            stackPanel.Children.Add(saveStack);

            RightSide.Children.Add(stackPanel);
        }

        void AddEvent(object sender, RoutedEventArgs e)
        {
            List<Object> controls = (List<Object>)((Button)sender).Tag;
            TextBox nameTB = (TextBox)controls[0];
            DatePicker sdDP = (DatePicker)controls[1];
            DatePicker edDP = (DatePicker)controls[2];
            ComboBox itCB = (ComboBox)controls[3];
            try
            {
                int id = meetingController.eventModel.Add(new Event
                {
                    name = nameTB.Text,
                    startDate = (DateTime)sdDP.SelectedDate,
                    endDate = (DateTime)edDP.SelectedDate,
                    intervalType = (byte)itCB.SelectedIndex
                });

                if (id != -1)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("Тэмдэглэлт өдөр амжилттай нэмэгдлээ.");
                    ShowCalendar(null, null);
                }
                else
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("Тэмдэглэлт өдөр нэмэхэд алдаа гарлаа.");
                }
            }
            catch(Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Тэмдэглэлт өдөр нэмэхэд алдаа гарлаа. Алдааны мессеж: " + ex.Message);
            }
        }
        
        void ModifyMeeting(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            Label label = new Label();
            string id = ((ListBoxItem)listBox.SelectedValue).Uid;
            Object meeting;
            if (((ListBoxItem)listBox.SelectedItem).Tag is ModifiedMeeting)
            {
                meeting = modifiedMeetingModel.Get(Int32.Parse(id));
            }
            else
            {
                meeting = meetingModel.Get(Int32.Parse(id));
            }
            RightSide.Children.Clear();

            List<Object> controls = new List<Object>(); //shine medeenuudiig zooh bus 

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.Margin = new Thickness(10);

            /**
                * Name stack
                */

            StackPanel nameStack = new StackPanel();
            nameStack.Orientation = Orientation.Horizontal;
            nameStack.Margin = new Thickness(0, 5, 0, 5);

            Label nameLabel = new Label();
            nameLabel.Content = "Хурлын нэр:";
            nameLabel.Width = 200;
            TextBox name = new TextBox();
            name.Width = 200;
            name.Text = ((Meeting)meeting).name;
            name.IsEnabled = false;
            controls.Add(name);

            nameStack.Children.Add(nameLabel);
            nameStack.Children.Add(name);

            /**
                * Starting time stack
                */

            StackPanel stStack = new StackPanel();
            stStack.Orientation = Orientation.Horizontal;
            stStack.Margin = new Thickness(0, 5, 0, 5);

            Label stLabel = new Label();
            stLabel.Content = "Хурал эхлэх цаг:";
            stLabel.Width = 200;
            TextBox st = new TextBox();
            st.Width = 200;
            st.Text = ((Meeting)meeting).startDatetime.ToShortTimeString();
            controls.Add(st);

            stStack.Children.Add(stLabel);
            stStack.Children.Add(st);

            /**
                * Duration time Stack
                */

            StackPanel durationStack = new StackPanel();
            durationStack.Orientation = Orientation.Horizontal;
            durationStack.Margin = new Thickness(0, 5, 0, 5);

            Label durationLabel = new Label();
            durationLabel.Content = "Хурал үргэлжлэх хугацаа минутаар:";
            durationLabel.Width = 200;
            TextBox duration = new TextBox();
            duration.Width = 200;
            duration.Text = ((Meeting)meeting).duration.ToString();
            controls.Add(duration);

            durationStack.Children.Add(durationLabel);
            durationStack.Children.Add(duration);
            /**Changed or not */
            StackPanel isChangedStack = new StackPanel();
            isChangedStack.Orientation = Orientation.Horizontal;
            Label isChangedLabel = new Label();
            isChangedLabel.Content = "Өөрчлөгдсөн эсэх";
            isChangedLabel.Width = 200;
            Label isChanged = new Label();
            isChanged.Content = ((ListBoxItem)listBox.SelectedItem).Tag is ModifiedMeeting ? "Тийм" : "Үгүй";

            isChangedStack.Children.Add(isChangedLabel);
            isChangedStack.Children.Add(isChanged);

            StackPanel reasonStack = new StackPanel();
            reasonStack.Orientation = Orientation.Horizontal;
            Label reasonLabel = new Label();
            reasonLabel.Content = ((ListBoxItem)listBox.SelectedItem).Tag is ModifiedMeeting ? "Өөрчлөгдсөн шалтгаан:":"Өөрчлөх шалтгаан";
            reasonLabel.Width = 200;

            TextBox reason = new TextBox();
            reason.TextWrapping = TextWrapping.Wrap;
            reason.Text = ((ListBoxItem)listBox.SelectedItem).Tag is ModifiedMeeting ? ((ModifiedMeeting)((ListBoxItem)listBox.SelectedItem).Tag).reason : "";
            controls.Add(reason);
            reason.Height = 60;
            reason.Width = 200;
            reasonStack.Children.Add(reasonLabel);
            reasonStack.Children.Add(reason);
            /**
            * Save button 
            */
            StackPanel saveStack = new StackPanel();
            saveStack.HorizontalAlignment = HorizontalAlignment.Right;
            saveStack.Orientation = Orientation.Horizontal;
            saveStack.Margin = new Thickness(0, 5, 0, 5);

            Button saveButton = new Button();
            saveButton.Content = "Хадгалах";
            saveButton.Background = Brushes.White;
            saveButton.Height = 25;
            saveButton.Width = 100;

            Button backButton = new Button();
            backButton.Content = "Буцах";
            backButton.Background = Brushes.White;
            backButton.Height = 25;
            backButton.Width = 100;
            
            backButton.Click += ShowCalendar;

            controls.Add(meeting);

            saveButton.Tag = controls;
            saveButton.Click += setMMeeting;

            saveStack.Children.Add(saveButton);
            saveStack.Children.Add(backButton);

            stackPanel.Children.Add(nameStack);
            stackPanel.Children.Add(stStack);
            stackPanel.Children.Add(durationStack);
            stackPanel.Children.Add(isChangedStack);
            stackPanel.Children.Add(reasonStack);

            stackPanel.Children.Add(saveStack);

            RightSide.Children.Add(stackPanel);
        }
        void setMMeeting(object sender, RoutedEventArgs e)
        {
            List<Object> controls = (List<Object>)((Button)sender).Tag;
            TextBox name = (TextBox)controls[0];
            TextBox st = (TextBox)controls[1];
            TextBox duration = (TextBox)controls[2];
            TextBox reason = (TextBox)controls[3];

            Object meeting = controls[4];
            if (meeting is ModifiedMeeting)
            {
                ModifiedMeeting mmeeting = (ModifiedMeeting)meeting;
                mmeeting.name = name.Text;
                mmeeting.startDatetime = DateTime.Parse(((DateTime)LeftSide.Tag).ToShortDateString() +
                    " " + st.Text);
                mmeeting.duration = Int32.Parse(duration.Text);
                mmeeting.reason = reason.Text;
                modifiedMeetingModel.Set(mmeeting);
                Xceed.Wpf.Toolkit.MessageBox.Show(mmeeting.startDatetime.ToString("yyyy/MM/dd") + " өдрийн " + mmeeting.name + " шинэчлэгдлээ.");
            }
            else
            {
                ModifiedMeeting mmeeting = new ModifiedMeeting();
                mmeeting.meeting_id = ((Meeting)meeting).id;
                mmeeting.name = name.Text;
                mmeeting.startDatetime = DateTime.Parse(((DateTime)LeftSide.Tag).ToShortDateString() +
                    " " + st.Text);
                mmeeting.duration = Int32.Parse(duration.Text);
                mmeeting.reason = reason.Text;
                modifiedMeetingModel.Add(mmeeting);
                Xceed.Wpf.Toolkit.MessageBox.Show(mmeeting.startDatetime.ToString("yyyy/MM/dd") + " өдрийн " + mmeeting.name + " шинэчлэгдлээ.");
            }
        }

        void addParticipantsToMeeting(object sender, RoutedEventArgs e)
        {
            string type = (string)((List<Object>)(((Button)sender).Tag))[0];
            ListBox list = (ListBox)((List<Object>)(((Button)sender).Tag))[1];
            AddParticipantToMeeting addWindow = new AddParticipantToMeeting(type, list);
            addWindow.ShowDialog();
            if (type == "group")
            {
                ListBox gGroupList = (ListBox)this.FindName("Groups");
                if ((bool)addWindow.DialogResult)
                {
                    List<Department> departments = depModel.Get(addWindow.ids);
                    foreach (Department department in departments)
                    {
                        ListBoxItem newGroup = new ListBoxItem();
                        newGroup.Content = department.name;
                        newGroup.Tag = department.id;
                        gGroupList.Items.Add(newGroup);
                    }
                }
            }
            else if (type == "user")
            {
                ListBox uGroupList = (ListBox)this.FindName("Users");
                if ((bool)addWindow.DialogResult)
                {
                    List<User> users = userModel.Get(addWindow.ids);
                    foreach (User user in users)
                    {
                        ListBoxItem newGroup = new ListBoxItem();
                        newGroup.Content = user.fname + " " + user.lname;
                        newGroup.Tag = user.id;
                        uGroupList.Items.Add(newGroup);
                    }
                }
            }
            else
            {
                ListBox pGroupList = (ListBox)FindName("Positions");
                if ((bool)addWindow.DialogResult)
                {
                    List<Position> positions = posModel.Get(addWindow.ids);
                    foreach (Position position in positions)
                    {
                        ListBoxItem newGroup = new ListBoxItem();
                        newGroup.Content = position.name;
                        newGroup.Tag = position.id;
                        pGroupList.Items.Add(newGroup);
                    }
                }
            }
        }

        void removeMeeting(object sender, RoutedEventArgs e)
        {
            Meeting meeting = (Meeting)((Button)sender).Tag;
            if (meetingModel.MarkAsDeleted(meeting.id))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Амжилттай устгалаа");
                ShowMeetings();
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Та орчноо шалгаад дахин оролдоно уу!", "Бүтэлгүйтлээ");
            }


        }
        /** Is called when meeting time has to change only 1 time
         * Gets data from meeting , modifies and inserts into Modified meeting
         */
        void ShowMeetings(object sender = null, RoutedEventArgs e = null)
        {
            is_home = false;
            LeftSide.Children.Clear();
            Label label = new Label();
            label.Content = "Нийт хурлууд:";

            Button import = new Button();
            import.Content = "+";
            import.Click += addMeeting;

            Button search = new Button();
            Image searchImage = new Image();
            searchImage.Source = new BitmapImage(new Uri("images/searchwhite.png", UriKind.Relative));
            searchImage.Width = 20;
            search.Content = searchImage;
            search.Tag = "meeting";
            search.Click += Search;

            List<Object> controls = new List<Object>();
            List<Object> rcontrols = new List<Object>();
            controls.Add(import);
            controls.Add(label);

            rcontrols.Add(search);
            DockPanel dockPanel = addHeader(controls, rcontrols);

            ListBox listbox = new ListBox();
            try
            {
                RegisterName("listbox", listbox);
            }
            catch (Exception ex)
            {
                UnregisterName("listbox");
                RegisterName("listbox", listbox);
            }
            listbox.Margin = new Thickness(10, 10, 10, 20);
            listbox.HorizontalAlignment = HorizontalAlignment.Stretch;

            List<Meeting> meetings = meetingModel.GetAll();
            int i = 1;
            foreach (Meeting meeting in meetings)
            {
                if (meeting.isDeleted)
                {
                    continue;
                }
                ListBoxItem listBoxItem = new ListBoxItem();
                if (meeting.duration == 0) listBoxItem.Content = listBoxItem.Content = i + ". " + meeting.name + ", " + meeting.startDatetime.ToString("HH:mm") + " цагийн хурал цуцлагдсан";
                else listBoxItem.Content = i + ". " + meeting.name + ", " + meeting.startDatetime.ToString("HH:mm") + ", " + meeting.duration + " минут";
                listBoxItem.Uid = meeting.id.ToString();
                listBoxItem.Height = 25;
                listbox.Items.Add(listBoxItem);
                i++;
            }
            listbox.SelectionChanged += onMeetingNameChanged;

            dockPanel.Children.Add(listbox);


        }

        void setUser(object sender, RoutedEventArgs e)
        {
            List<Object> controls = (List<Object>)(((Button)sender).Tag);

            TextBox lName = (TextBox)(controls[0]);
            TextBox fName = (TextBox)(controls[1]);
            User user = (User)controls[2];

            user.fname = fName.Text;
            user.lname = lName.Text;

            try
            {
                userModel.Set(user);
                Xceed.Wpf.Toolkit.MessageBox.Show("Амжилттай өөрчлөгдлөө", "Өөрчлөгдлөө");
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Та программ ажиллуулж байгаа орчиноо шалгана уу", "Өөрчилсөнгүй");
            }
            ShowMembers(sender, null);
        }
        void onUserChanged(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            if ((ListBoxItem)listBox.SelectedValue == null)
            {
                return;
            }
            string id = ((ListBoxItem)listBox.SelectedValue).Uid;

            RightSide.Children.Clear();

            User user = userModel.Get(Int32.Parse(id));
            List<Object> controls = new List<Object>();
            
            IEnumerable<UserStatus> userStatuses = meetingController.userStatusModel.GetByFK(user.IDName, user.id).OrderBy(x => x.startDate);
            int currentStatus = -1;
            DateTime now = DateTime.Now;

            foreach (UserStatus us in userStatuses)
            {
                if (currentStatus == -1 && us.endDate.Date >= now.Date && now.Date >= us.startDate.Date)
                {
                    currentStatus = us.statusId;
                    break;
                }
            }

            Grid grid = new Grid();
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(.5);

            ColumnDefinition col0 = new ColumnDefinition();
            col0.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition col1 = new ColumnDefinition();
            col1.Width = new GridLength(2, GridUnitType.Star);

            grid.ColumnDefinitions.Add(col0);
            grid.ColumnDefinitions.Add(col1);

            RowDefinition row0 = new RowDefinition();
            row0.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition row1 = new RowDefinition();
            row1.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition row2 = new RowDefinition();
            row2.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition row3 = new RowDefinition();
            row3.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition row4 = new RowDefinition();
            row3.Height = new GridLength(1, GridUnitType.Star);

            grid.RowDefinitions.Add(row0);
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            grid.RowDefinitions.Add(row3);
            grid.RowDefinitions.Add(row4);

            BitmapImage webImage;
            try
            {
                webImage = new BitmapImage(new Uri(meetingController.GetUserImage(user)));
            }
            catch(Exception ex)
            {
                webImage = new BitmapImage(new Uri("images/user.png",UriKind.Relative));
            }
            float scaleHeight = (float)350 / (float)webImage.Height;
            float scaleWidth = (float)350 / (float)webImage.Width;
            float scale = Math.Min(scaleHeight, scaleWidth);

            Image imageControl = new Image
            {
                Source = webImage,
                Height = (int)(webImage.Width * scale),
                Width = (int)(webImage.Height * scale)
            };

            Grid.SetColumn(imageControl, 0);
            Grid.SetColumnSpan(imageControl, 2);
            Grid.SetRow(imageControl, 0);
            grid.Children.Add(imageControl);

            for (int i = 1; i < 5; i++)
            {
                Label nameLabel = new Label();
                Label valueLabel = new Label();
                Grid.SetColumn(nameLabel, 0);
                Grid.SetColumn(valueLabel, 1);
                Grid.SetRow(nameLabel, i);
                Grid.SetRow(valueLabel, i);
                switch (i)
                {
                    case 1:
                        nameLabel.Content = "Нэр:";
                        valueLabel.Content = user.fname;
                        break;
                    case 2:
                        nameLabel.Content = "Алба:";
                        valueLabel.Content = user.departmentId != -1 ? meetingController.departmentModel.Get(user.departmentId).name : "";
                        break;
                    case 3:
                        nameLabel.Content = "Тушаал:";
                        valueLabel.Content = user.positionId != -1 ? meetingController.positionModel.Get(user.positionId).name : "";
                        break;
                    case 4:
                        nameLabel.Content = "Одоогийн төлөв:";
                        valueLabel.Content = user.positionId != -1 ? meetingController.positionModel.Get(user.positionId).name : "";
                        if (currentStatus == -1)
                        {
                            valueLabel.Content = "Идэвхитэй";
                        }
                        else
                        {
                            valueLabel.Content = meetingController.statusModel.Get(currentStatus).name;
                        }
                        break;
                }
                grid.Children.Add(nameLabel);
                grid.Children.Add(valueLabel);
            }

            border.Child = grid;

            Button changeStatus = new Button();
            changeStatus.Content = "Хэрэглэгчийн төлөв өөрчлөх";
            changeStatus.Background = Brushes.White;
            changeStatus.Height = 30;
            changeStatus.Uid = id;
            changeStatus.Click += setStatus;

            List<Attendance> allAtts = attModel.GetByFK(user.IDName, user.id);
            List<Attendance> atts = new List<Attendance>();
            foreach(Attendance att in allAtts)
            {
                if(archModel.Get(att.archivedMeetingId).meetingDatetime > DateTime.Today.AddMonths(-1))
                {
                    atts.Add(att);
                }
            }
            int came = 0;
            int lateMin = 0;
            int abs = 0;
            foreach(Attendance att in atts)
            {
                if (att.statusId == 1) came++;
                if (att.statusId == 2) lateMin += att.regTime;
                if (att.statusId == 14) abs++;
            }

            Label statHeader = new Label();
            statHeader.Content = "Сүүлийн сард:";
            statHeader.FontSize = 20;

            Grid statGrid = new Grid();

            ColumnDefinition scol0 = new ColumnDefinition();
            ColumnDefinition scol1 = new ColumnDefinition();
            scol0.Width =new  GridLength(1, GridUnitType.Star);
            scol1.Width =new  GridLength(40);

            statGrid.ColumnDefinitions.Add(scol0);
            statGrid.ColumnDefinitions.Add(scol1);

            RowDefinition[] srow = new RowDefinition[4];
            for(int i=0;i<4;i++)
            {
                srow[i] = new RowDefinition();
                srow[i].Height = new GridLength(30);
                statGrid.RowDefinitions.Add(srow[i]);
            }
            string[] names = {"Нийт хурлын тоо:","Ирсэн хурлын тоо:","Тасалсан тоо:","Хоцорсон минут:" };
            string[] values = { atts.Count.ToString(), came.ToString(), abs.ToString(), lateMin.ToString() };
            for(int i=0;i<4;i++)
            {
                Label nameLabel = new Label();
                Label valueLabel = new Label();
                nameLabel.Content = names[i];
                valueLabel.Content = values[i];

                Grid.SetRow(nameLabel, i);
                Grid.SetRow(valueLabel, i);
                Grid.SetColumn(nameLabel, 0);
                Grid.SetColumn(valueLabel, 1);
                statGrid.Children.Add(nameLabel);
                statGrid.Children.Add(valueLabel);
            }
            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(border);
            stackPanel.Children.Add(changeStatus);
            stackPanel.Children.Add(statHeader);
            stackPanel.Children.Add(statGrid);
            
            ScrollViewer scv = new ScrollViewer();
            scv.Height = ActualHeight-50;
            if (FindName("scroll") != null) UnregisterName("scroll");
            RegisterName("scroll", scv);

            scv.Content = stackPanel;
            RightSide.Children.Add(scv);
        }
        void setStatus(object sender,RoutedEventArgs e)
        {
            string id = ((Button)sender).Uid;
            ChangeUserStatus ch = new ChangeUserStatus(Int32.Parse(id));
            ch.Owner = this;
            ch.Visibility = Visibility.Visible;
        }
        void importData(object sender, RoutedEventArgs e)
        {
            ImportUser iuser = new ImportUser();
            iuser.ShowDialog();

            if (iuser.DialogResult == true)
            {
                ExternalDataImporter edi = new ExternalDataImporter();
                edi.ImportUserData(iuser.xlPath, iuser.datPath, iuser.imagePaths.ToList());
                ShowMembers(null, null);
            }
        }
        void ShowMembers(object sender, RoutedEventArgs e)
        {
            is_home = false;
            LeftSide.Children.Clear();

            Label label = new Label();
            label.Content = "Нийт гишүүд:";

            Button import = new Button();
            import.Content = "+";
            import.Click += importData;

            Button search = new Button();
            Image searchImage = new Image();
            searchImage.Source = new BitmapImage(new Uri("images/searchwhite.png", UriKind.Relative));
            searchImage.Width = 20;
            search.Tag = "user";
            search.Content = searchImage;
            search.Click += Search;

            List<Object> buttons = new List<Object>();
            List<Object> rbuttons = new List<Object>();
            buttons.Add(import);
            buttons.Add(label);
            rbuttons.Add(search);

            DockPanel dockPanel = addHeader(buttons, rbuttons);

            ListBox listbox = new ListBox();
            try
            {
                RegisterName("listbox", listbox);
            }
            catch (Exception ex)
            {
                UnregisterName("listbox");
                RegisterName("listbox", listbox);
            }
            listbox.Margin = new Thickness(10, 10, 10, 20);

            List<User> users = userModel.GetAll();
            users = users.OrderBy(x => x.departmentId).ToList();
            Dictionary<int, string> departmentNames = meetingController.departmentModel.GetAll().ToDictionary(x => x.id, x => x.name);
            int i = 1;
            foreach (User user in users)
            {
                ListBoxItem listBoxItem = new ListBoxItem();

                listBoxItem.Content = i + ". " + user.lname + " " + user.fname + ", " + (user.departmentId != -1? departmentNames[user.departmentId]:"Хэлтэсгүй");
                listBoxItem.Uid = user.id.ToString();
                listBoxItem.Height = 25;
                listbox.Items.Add(listBoxItem);
                i++;
            }
            listbox.SelectionChanged += onUserChanged;

            dockPanel.Children.Add(listbox);

        }

        void getReport(object sender, RoutedEventArgs e)
        {
            List<Object> controls = (List<Object>)((Button)sender).Tag;
            ListBox listBox = (ListBox)controls[0];
            DatePicker st = (DatePicker)controls[1];
            DatePicker et = (DatePicker)controls[2];
            if (listBox == null || st == null || et == null)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Та дээрх бүх талбарыг бөглөнө үү!", "Алдаа");
                return;
            }
            ReportExporter re = new ReportExporter(meetingController);
            List<Meeting> meetings;

            if ((string)((ListBoxItem)listBox.SelectedItem).Content == "Бүх хурлууд")
            {
                meetings = meetingModel.GetAll();
            }
            else
            {
                meetings = new List<Meeting>();
                meetings.Add(meetingModel.Get(Int32.Parse(((ListBoxItem)listBox.SelectedItem).Uid)));
            }
            try
            {
                re.ExportAttendance(meetings, (DateTime)st.SelectedDate, (DateTime)et.SelectedDate, "report");
            }
            catch(Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Тайлан гаргах явцад алдаа гарлаа. Алдааны мессеж: " + ex.Message);
            }

        }
        void ShowReport(object sender, RoutedEventArgs e)
        {
            is_home = false;
            RightSide.Children.Clear();

            List<Object> controls = new List<Object>();

            Grid grid = new Grid();
            grid.Margin = new Thickness(10);
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            //Defining the cols
            ColumnDefinition col0 = new ColumnDefinition();
            ColumnDefinition col1 = new ColumnDefinition();

            col0.Width = new GridLength(1, GridUnitType.Star);
            col1.Width = new GridLength(30);

            grid.ColumnDefinitions.Add(col0);
            grid.ColumnDefinitions.Add(col1);
            //Defining the rows
            RowDefinition row0 = new RowDefinition();
            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            RowDefinition row3 = new RowDefinition();
            RowDefinition row4 = new RowDefinition();
            RowDefinition row5 = new RowDefinition();

            row0.Height = new GridLength(30);
            row1.Height = new GridLength(0);
            row2.Height = new GridLength(1, GridUnitType.Star);
            row3.Height = new GridLength(30);
            row4.Height = new GridLength(30);
            row5.Height = new GridLength(30);

            grid.RowDefinitions.Add(row0);
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            grid.RowDefinitions.Add(row3);
            grid.RowDefinitions.Add(row4);
            grid.RowDefinitions.Add(row5);


            Label title = new Label();
            title.Content = "Хурал сонгох";
            Grid.SetColumnSpan(title, 2);
            Grid.SetRow(title, 0);
            Grid.SetColumn(title, 0);

            ListBox listBox = new ListBox();
            controls.Add(listBox);
            listBox.Margin = new Thickness(0, 10, 0, 10);
            ListBoxItem allMeeting = new ListBoxItem();
            allMeeting.Content = "Бүх хурлууд";
            listBox.SelectedItem = allMeeting;

            listBox.Items.Add(allMeeting);

            List<Meeting> meetings = meetingModel.GetAll();

            foreach (Meeting meeting in meetings)
            {
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = meeting.name;
                listBoxItem.Uid = meeting.id.ToString();

                listBox.Items.Add(listBoxItem);
            }

            Grid.SetColumnSpan(listBox, 2);
            Grid.SetColumn(listBox, 0);
            Grid.SetRow(listBox, 2);

            Label timeSelectorTitle = new Label();
            timeSelectorTitle.Content = "Хугацаа сонгох";
            Grid.SetRow(timeSelectorTitle, 3);
            Grid.SetColumn(timeSelectorTitle, 0);

            StackPanel timeSelector = new StackPanel();
            timeSelector.Orientation = Orientation.Horizontal;
            timeSelector.HorizontalAlignment = HorizontalAlignment.Stretch;

            DatePicker startTime = new DatePicker();
            controls.Add(startTime);
            startTime.SelectedDate = DateTime.Today.AddDays(-7);
            Label label = new Label();
            label.Content = "-";
            DatePicker endTime = new DatePicker();
            endTime.SelectedDate = DateTime.Today;
            controls.Add(endTime);

            Button saveButton = new Button();
            saveButton.Tag = controls;
            saveButton.Content = "Тайлан гаргах";
            saveButton.Click += getReport;
            saveButton.Margin = new Thickness(10, 0, 0, 0);
            saveButton.HorizontalAlignment = HorizontalAlignment.Right;
            


            timeSelector.Children.Add(startTime);
            timeSelector.Children.Add(label);
            timeSelector.Children.Add(endTime);
            timeSelector.Children.Add(saveButton);

            Grid.SetColumnSpan(timeSelector, 2);
            Grid.SetRow(timeSelector, 4);
            Grid.SetColumn(timeSelector, 0);

            grid.Children.Add(title);
            grid.Children.Add(listBox);
            grid.Children.Add(timeSelectorTitle);
            grid.Children.Add(timeSelector);

            RightSide.Children.Add(grid);
        }
        void MainWindow_SizeChanged(object sender, EventArgs e)
        {
            if(FindName("scroll")!=null)
            {
                ((ScrollViewer)FindName("scroll")).Height = ActualHeight-50;
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {

            if (meetingController.onGoingMeetingUserAttendance != null && meetingController.onGoingMeetingUserAttendance.Count > 0)
            {
                meetingController.scannerHandler.Stop();
            }
            base.OnClosing(e);
        }
    }
}

