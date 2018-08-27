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
        public MainWindow()
        {
            InitializeComponent();

            meetingController = new MeetingController();
            meetingModel = meetingController.meetingModel;
            userModel = new Model<User>();
            depModel = new Model<Department>();
            posModel = new Model<Position>();
            mauModel = new Model<MeetingAndUser>();
            madModel = new Model<MeetingAndDepartment>();
            mapModel = new Model<MeetingAndPosition>();
            modifiedMeetingModel = new Model<ModifiedMeeting>();
            
        }
        
        private void showStatus(object sender, RoutedEventArgs e)
        {
            MeetingStatus meetingStatus = new MeetingStatus();
            meetingStatus.Visibility = Visibility.Visible;
            meetingController.StartMeeting(meetingModel.Get(3));
        }

        private void showMenu(object sender, RoutedEventArgs e)
        {
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

            Menu.Children.Add(Calendar);
            Menu.Children.Add(Meetings);
            Menu.Children.Add(Members);
            Menu.Children.Add(Report);

            dock.Children.Add(Menu);

            RightSide.Children.Clear();
            RightSide.HorizontalAlignment = HorizontalAlignment.Stretch;

            Label title = new Label();
            title.Content = "Дараагийн хурал:";
            title.FontSize = 22;
            title.Margin = new Thickness(10, 0, 0, 0);
            Label time = new Label();
            time.Content = "2018/09/01 15:00";
            time.FontSize = 24;
            time.Margin = new Thickness(10, 0, 0, 0);


            RightSide.Children.Add(title);
            RightSide.Children.Add(time);
        }

        DockPanel addHeader(List<Object> controls =null, List<Object> rControls = null)
        {
            LeftSide.VerticalAlignment = VerticalAlignment.Stretch;
            LeftSide.HorizontalAlignment = HorizontalAlignment.Stretch;

            DockPanel dockPanel = new DockPanel();

            DockPanel headerPanel = new DockPanel();
            headerPanel.Margin = new Thickness(10, 5, 10, -5);
            headerPanel.HorizontalAlignment = HorizontalAlignment.Stretch;

            Button back = new Button();

            back.Content = "<";
            back.Click += showMenu;
            back.Foreground = Brushes.White;
            back.BorderBrush = Brushes.Transparent;
            back.Background = Brushes.Transparent;
            back.Height = 30;
            back.Width = 30;

            headerPanel.Children.Add(back);

            Button status = new Button();

            Image eyeImage = new Image();
            eyeImage.Source = new BitmapImage(new Uri("images/eye.png", UriKind.Relative));
            eyeImage.Width = 20;
            eyeImage.Height = 20;

            status.Content = eyeImage;

            status.Click += showStatus;
            status.Foreground = Brushes.White;
            status.BorderBrush = Brushes.Transparent;
            status.Background = Brushes.Transparent;
            status.Height = 30;
            status.Width = 30;
            status.HorizontalAlignment = HorizontalAlignment.Right;

            DockPanel.SetDock(headerPanel, Dock.Left);

            if (controls != null)
            {
                foreach (Object control in controls)
                {
                    if(control is Button)
                    {
                        Button tmp = (Button)control;
                        tmp.Height = 30;
                        tmp.Width = 30;
                        tmp.Foreground = Brushes.White;
                        tmp.BorderBrush = Brushes.Transparent;
                        tmp.Background = Brushes.Transparent;
                        headerPanel.Children.Add(tmp);
                    }
                    if(control is Label)
                    {
                        Label tmp = (Label)control;
                        tmp.Foreground = Brushes.White;
                        tmp.FontSize = 16;
                        headerPanel.Children.Add(tmp);
                    }
                }
            }
            if(rControls!=null)
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

            DockPanel.SetDock(status, Dock.Right);
            headerPanel.Children.Add(status);

            DockPanel.SetDock(headerPanel, Dock.Top);
            dockPanel.Children.Add(headerPanel);
            try
            {
                RegisterName("headerPanel", headerPanel);
            }
            catch(Exception ex)
            {
                UnregisterName("headerPanel");
                RegisterName("headerPanel", headerPanel);
            }

            LeftSide.Children.Add(dockPanel);
            return dockPanel;
        }
        void Search(object sender, RoutedEventArgs e)
        {
            ListBox listbox;
            try
            {
                listbox = (ListBox)FindName("listbox");
            }
            catch(Exception ex)
            {
                UnregisterName("listbox");
                listbox = (ListBox)FindName("listbox");
            }

            if(FindName("SearchBox") == null)
            {
                TextBox searchBox= new TextBox();
                RegisterName("SearchBox",searchBox);
                ((DockPanel)FindName("headerPanel")).Children.Add(searchBox);
            }
            else
            {
                listbox.Items.Clear();
                if( (string)((Button)sender).Tag == "meeting" )
                {
                    foreach (Meeting meeting in meetingModel.GetAll())
                    {
                        ListBoxItem lbi = new ListBoxItem();
                        lbi.Content = meeting.name;
                        lbi.Tag = meeting.id;
                        listbox.Items.Add(lbi);
                    }
                }
                else if((string)((Button)sender).Tag == "user")
                {
                    foreach (User user in userModel.GetAll())
                    {
                        ListBoxItem lbi = new ListBoxItem();
                        lbi.Content = user.fname + " " +user.lname;
                        lbi.Tag = user.id;
                        listbox.Items.Add(lbi);
                    }
                }
                TextBox searchBox = (TextBox)FindName("SearchBox");
                if (searchBox.Text == "" || searchBox.Text == null)
                {
                    if ((string)((Button)sender).Tag == "meeting")
                        ShowMeetings();
                    else
                        ShowMembers(null,null);
                    UnregisterName("SearchBox");
                    return;
                }
                for(int i=listbox.Items.Count-1 ; i>=0 ;i--)
                {
                    Regex regex = new Regex(@"[a-zA-Z0-9]*"+ searchBox.Text + @"[a-zA-Z0-9]*");
                    Match match = regex.Match((string)((ListBoxItem)listbox.Items[i]).Content);
                    if (!match.Success)
                    {
                        listbox.Items.Remove((ListBoxItem)listbox.Items[i]);
                    }
                }
            }
        }

        void MarkDayAsIncative(object sender, RoutedEventArgs e)
        {
            Calendar calendar = (Calendar)((Button)sender).Tag;
            AskTheReason ask = new AskTheReason();
            ask.ShowDialog();
            if ((bool)ask.DialogResult)
            {
                meetingController.CancelMeetingsByDate((DateTime)calendar.SelectedDate,ask.text);
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
            calendarImage.Source = new BitmapImage(new Uri("images/calendar.png", UriKind.Relative));
            calendarImage.Width = 20;
            calendarImage.Height = 20;
            rButton.Content = calendarImage;
            rButton.Click += ShowCalendar;

            Button xButton = new Button();
            Image xImage = new Image();
            xImage.Source = new BitmapImage(new Uri("images/x.png", UriKind.Relative));
            xImage.Width = 20;
            xImage.Height = 20;
            xButton.Tag = calendar;

            xButton.Content = xImage;
            xButton.Click += MarkDayAsIncative;
            List<Object> rControls = new List<Object>();
            rControls.Add(rButton);
            rControls.Add(xButton);
            DockPanel dockPanel = addHeader(list, rControls);

            ListBox listbox = new ListBox();
            listbox.Margin = new Thickness(10, 10, 10, 10);
            if ((DateTime)calendar.SelectedDate < DateTime.Today)
            {
                List<ArchivedMeeting> meetings =  meetingController.GetArchivedMeetingByDate((DateTime)calendar.SelectedDate);
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
                foreach (Meeting meeting in meetings)
                {
                    if (meeting.isDeleted) continue;
                    ListBoxItem listBoxItem = new ListBoxItem();
                    if (meeting.duration == 0) listBoxItem.Content = "Цуцлагдсан";
                    else listBoxItem.Content = meeting.name;
                    listBoxItem.Tag = meeting;//end tagiig n zooj ugch bn
                    listBoxItem.Uid = meeting.id.ToString();
                    listBoxItem.Height = 25;
                    listbox.Items.Add(listBoxItem);
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
            RightSide.Children.Clear();
            LeftSide.Children.Clear();

            List<Object> list = new List<Object>();
            Label header = new Label();
            header.Content = "Тухайн өдрийн хурлууд:";
            header.Foreground = Brushes.White;
            list.Add(header);


            Button rButton = new Button();
            Image calendarImage = new Image();
            calendarImage.Source = new BitmapImage(new Uri("images/calendar.png", UriKind.Relative));
            calendarImage.Width = 20;
            calendarImage.Height = 20;

            Button xButton = new Button();
            Image xImage = new Image();
            xImage.Source = new BitmapImage(new Uri("images/x.png", UriKind.Relative));
            xImage.Width = 20;
            xImage.Height = 20;
            xButton.Content = xImage;
            xButton.Click += MarkDayAsIncative;

            rButton.Content = calendarImage;
            rButton.Click += ShowCalendar;

            xButton.Content = xImage;

            List<Object> rControls = new List<Object>();
            rControls.Add(rButton);
            rControls.Add(xButton);

            DockPanel dockPanel = addHeader(list,rControls);

            Viewbox ll = new Viewbox();
            
            dockPanel.Children.Add(ll);

            Calendar calendar = new Calendar();
            Viewbox viewbox = new Viewbox();
            viewbox.Stretch = Stretch.Uniform;
            viewbox.Child = calendar;
            calendar.SelectedDatesChanged += OnSelectedDateChange;
            RightSide.Children.Add(viewbox);

            ListBox listbox = new ListBox();
            listbox.Margin = new Thickness(10, 10, 10, 10);

            List<Meeting> meetings = meetingController.FindByDate(DateTime.Today);

            foreach (Meeting meeting in meetings)
            {
                ListBoxItem listBoxItem = new ListBoxItem();

                listBoxItem.Content = meeting.name;
                listBoxItem.Uid = meeting.id.ToString();
                listBoxItem.Height = 25;
                listBoxItem.Tag = meeting; //done fixing. Try it now
                listbox.Items.Add(listBoxItem);
            }
            listbox.SelectionChanged += ModifyMeeting;

            dockPanel.Children.Add(listbox);
        }

        void RemoveFromList(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (ListBox)((Button)sender).Tag;
            listBox.Items.Remove(listBox.SelectedItem);
            return;
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
            nameLabel.Width = 215;
            TextBox name = new TextBox();
            name.Width = 215;
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
            stLabel.Width = 215;
            TextBox st = new TextBox();
            st.Width = 215;
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
            sdLabel.Width = 215;
            DatePicker sd = new DatePicker();
            sd.Width = 215;
            controls.Add(sd);

            sdStack.Children.Add(sdLabel);
            sdStack.Children.Add(sd);
            /**
                * duration Stack
                */

            StackPanel durationStack = new StackPanel();
            durationStack.Orientation = Orientation.Horizontal;
            durationStack.Margin = new Thickness(0, 5, 0, 5);

            Label durationLabel = new Label();
            durationLabel.Content = "Хурал үргэлжлэх хугацаа:";
            durationLabel.Width = 215;
            TextBox duration = new TextBox();
            duration.Width = 215;
            controls.Add(duration);

            durationStack.Children.Add(durationLabel);
            durationStack.Children.Add(duration);

            /**
                * ending date Stack
                */
            StackPanel edStack = new StackPanel();
            edStack.Orientation = Orientation.Horizontal;
            edStack.Margin = new Thickness(0, 5, 0, 5);

            Label edLabel = new Label();
            edLabel.Content = "Хурал дуусах өдөр:";
            edLabel.Width = 215;
            DatePicker ed = new DatePicker();
            ed.Width = 215;
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
            fLabel.Width = 215;
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
            freqType.Width = 215;
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
            try
            {
                RegisterName("Groups", pGroupList);
            }
            catch(Exception ex)
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
            try
            {
                RegisterName("Users", pUserList);
            }
            catch(Exception ex)
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

            stackPanel.Children.Add(durationStack);
            stackPanel.Children.Add(edStack);
            stackPanel.Children.Add(fStack);

            stackPanel.Children.Add(pStack);
            stackPanel.Children.Add(saveStack);

            RightSide.Children.Add(stackPanel);
        }
        void insertMeeting(object sender, RoutedEventArgs e)
        {
            List<Object> controls = (List<Object>)((Button)sender).Tag;
            for(int i=0;i<9;i++)
            {
                if (controls[i] == null)
                {
                    MessageBox.Show("Та бүрэн бөглөнө үү", "Амжилтгүй");
                    return;
                }
            }
            string name = ((TextBox)controls[0]).Text;
            string st = ((TextBox)controls[1]).Text;
            string sd = ((DateTime)((DatePicker)controls[2]).SelectedDate).ToShortDateString();
            string duration = ((TextBox)controls[3]).Text;
            string ed = ((DatePicker)controls[4]).Text;
            ComboBox freqType = (ComboBox)controls[5];
            ListBox pDeps = (ListBox)controls[6];
            ListBox pUsers = (ListBox)controls[7];
            ListBox pPositions = (ListBox)controls[8];

            Meeting meeting = new Meeting();
            meeting.name = name;
            /**checking time and inserting*/
            try
            {
                meeting.startDatetime = DateTime.Parse(sd + " " + st);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Та хурал эхлэх цагаа цаг:минут гэсэн хэлбэртэйгээр оруулна уу!");
                return;
            }
            /** checking and inserting duration*/
            try
            {
                meeting.duration = Int32.Parse(duration);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Та үргэлжлэх хугацаагаа зөв оруулна уу!\n" +
                      "Зөвхөн үргэлжлэх хугацааг минутаар илэхийлэх тоо байхыг анхаарна уу!");
                return;
            }
            meeting.endDate = DateTime.Parse(ed);
            /** checking and inserting interval()freq*/
            try
            {
                int type = Byte.Parse(((ComboBoxItem)freqType.SelectedItem).Tag.ToString());
                if (type == 1)
                {
                    meeting.intervalType = 7;
                    meeting.intervalDay = 7;
                }
                else if(type == 2)
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
                MessageBox.Show("Та хурал болох давтамжаа  шалгана уу", "Өөрчилсөнгүй");
                return;
            }
            /** checking and inserting meeting itself*/
            int meetingid = meetingModel.Add(meeting);
            if (meetingid != -1)
            {
                MessageBox.Show("Амжилттай нэмлээ!");

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
                    MessageBox.Show("Бүтэлгүйтлээ.");
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
                    MessageBox.Show("Бүтэлгүйтлээ.");
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
                    MessageBox.Show("Бүтэлгүйтлээ.");
                }
                ShowMeetings();
            }
            else
            {
                MessageBox.Show("Өгөгдлийн сантай холбоотой асуудал гарлаа.",
                    "Бүтэлгүйтлээ!");
            }

        }
        /**to insert to modified meeting
         */

        void onMeetingNameChanged(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            Label label = new Label();
            string id = ((ListBoxItem)listBox.SelectedValue).Uid;

            RightSide.Children.Clear();

            Meeting meeting = meetingModel.Get(Int32.Parse(id));
            List<Object> controls = new List<Object>();

            StackPanel stackPanel = new StackPanel();
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
            nameLabel.Width = 215;
            TextBox name = new TextBox();
            name.Width = 215;
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
            stLabel.Width = 215;
            TextBox st = new TextBox();
            st.Width = 215;
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
            sdLabel.Width = 215;
            DatePicker sd = new DatePicker();
            sd.Width = 215;
            sd.DisplayDate = ((DateTime)meeting.startDatetime);
            sd.Text = ((DateTime)meeting.startDatetime).ToShortDateString();
            controls.Add(sd);

            sdStack.Children.Add(sdLabel);
            sdStack.Children.Add(sd);
            /**
                * ending time Stack
                */

            StackPanel durationStack = new StackPanel();
            durationStack.Orientation = Orientation.Horizontal;
            durationStack.Margin = new Thickness(0, 5, 0, 5);

            Label durationLabel = new Label();
            durationLabel.Content = "Хурал үргэлжлэх хугацаа:";
            durationLabel.Width = 215;
            TextBox duration = new TextBox();
            duration.Width = 215;
            duration.Text = meeting.duration.ToString();
            controls.Add(duration);

            durationStack.Children.Add(durationLabel);
            durationStack.Children.Add(duration);

            /**
                * ending date Stack
                */
            StackPanel edStack = new StackPanel();
            edStack.Orientation = Orientation.Horizontal;
            edStack.Margin = new Thickness(0, 5, 0, 5);

            Label edLabel = new Label();
            edLabel.Content = "Хурал дуусах өдөр:";
            edLabel.Width = 215;
            DatePicker ed = new DatePicker();
            ed.Width = 215;
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
            fLabel.Width = 215;
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
            freqType.Width = 215;
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
            controls.Add(pUserList);
            pUserList.Margin = new Thickness(0, 0, 0, 10);
            List<MeetingAndUser> maus = mauModel.GetByFK(meeting.IDName, meeting.id);
            foreach (MeetingAndUser mau in maus)
            {
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = userModel.Get(mau.userId).fname + " " + userModel.Get(mau.userId).lname;
                listBoxItem.Tag = userModel.Get(mau.userId
).id;
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
            stackPanel.Children.Add(durationStack);
            stackPanel.Children.Add(edStack);
            stackPanel.Children.Add(fStack);
            stackPanel.Children.Add(pStack);
            stackPanel.Children.Add(saveStack);

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Content = stackPanel;
            scrollViewer.Margin = new Thickness(0, 0, -20, 0);
            RightSide.Children.Add(scrollViewer);
            scrollViewer.Height = this.ActualHeight + 50;
        }
        void setMeeting(object sender, RoutedEventArgs e)
        {
            List<Object> controls = (List<Object>)((Button)sender).Tag;
            TextBox name = (TextBox)controls[0];
            TextBox st = (TextBox)controls[1];
            DatePicker sd = (DatePicker)controls[2];

            TextBox duration = (TextBox)controls[3];
            DatePicker ed = (DatePicker)controls[4];
            ComboBox freqType = (ComboBox)controls[5];
            ListBox deps = (ListBox)controls[6];
            ListBox users = (ListBox)controls[7];
            ListBox positions = (ListBox)controls[8];
            Meeting meeting = (Meeting)controls[9];

            meeting.name = name.Text;

            string datetimeString = sd.Text + " " + st.Text;
            //check and set meeting startDateTime
            try
            {
                meeting.startDatetime = DateTime.Parse(datetimeString);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Та цагаа цаг:минут гэсэн хэлбэрээр бичнэ үү!", "Өөрчилсөнгүй");
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
                MessageBox.Show("Та цагаа цаг:минут гэсэн хэлбэрээр бичнэ үү!", "Өөрчилсөнгүй");
                return;
            }
            //check and set meeting duration
            try
            {
                meeting.duration = Int32.Parse(duration.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Та хурал болох хугацаагаа шалгана уу", "Өөрчилсөнгүй");
                return;
            }
            //check and set meeting intervalType(frq)
            try
            {
                meeting.intervalType = Byte.Parse( ((ComboBoxItem)freqType.SelectedItem).Tag.ToString() );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Та хурал болох давтамжаа  шалгана уу", "Өөрчилсөнгүй");
                return;
            }
            //check and update meeting
            try
            {
                List<MeetingAndUser> oldMaus = mauModel.GetByFK(meeting.IDName, meeting.id);
                foreach(MeetingAndUser mau in oldMaus)
                {
                    mauModel.Remove(mau.id);
                }
                foreach(ListBoxItem user in users.Items)
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
                MessageBox.Show("Амжилттай өөрчиллөө.", "Өөрчлөгдлөө");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Та программ ажиллуулж буй орчиноо шалгана уу", "Өөрчилсөнгүй");
            }
            ShowMeetings(sender, null);
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
            nameLabel.Width = 215;
            TextBox name = new TextBox();
            name.Width = 215;
            name.Text = ((Meeting)meeting).name;
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
            stLabel.Width = 215;
            TextBox st = new TextBox();
            st.Width = 215;
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
            durationLabel.Content = "Хурал үргэлжлэх хугацаа:";
            durationLabel.Width = 215;
            TextBox duration = new TextBox();
            duration.Width = 215;
            duration.Text = ((Meeting)meeting).duration.ToString();
            controls.Add(duration);

            durationStack.Children.Add(durationLabel);
            durationStack.Children.Add(duration);

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

            controls.Add(meeting);

            saveButton.Tag = controls;
            saveButton.Click += setMMeeting;

            saveStack.Children.Add(saveButton);

            stackPanel.Children.Add(nameStack);
            stackPanel.Children.Add(stStack);
            stackPanel.Children.Add(durationStack);
            stackPanel.Children.Add(saveStack);

            RightSide.Children.Add(stackPanel);
        }
        void setMMeeting(object sender, RoutedEventArgs e)
        {
            List<Object> controls= (List<Object>)((Button)sender).Tag;
            TextBox name = (TextBox)controls[0];
            TextBox st = (TextBox)controls[1];
            TextBox duration = (TextBox)controls[2];
            Object meeting = controls[3];
            if(meeting is ModifiedMeeting)
            {
                Meeting mmeeting = (Meeting)meeting;
                mmeeting.name = name.Text;
                mmeeting.startDatetime = DateTime.Parse(((DateTime)LeftSide.Tag).ToShortDateString() +
                    " " + st.Text);
                mmeeting.duration = Int32.Parse(duration.Text);
                modifiedMeetingModel.Set((ModifiedMeeting)meeting);
            }
            else
            {
                ModifiedMeeting mmeeting = new ModifiedMeeting();
                mmeeting.meeting_id = ((Meeting)meeting).id;
                mmeeting.name = name.Text;
                mmeeting.startDatetime = DateTime.Parse(((DateTime)LeftSide.Tag).ToShortDateString() +
                    " " + st.Text);
                mmeeting.duration = Int32.Parse(duration.Text);
                modifiedMeetingModel.Add(mmeeting);
            }
            MessageBox.Show("Done");
        }

        void addParticipantsToMeeting(object sender, RoutedEventArgs e)
        {
            string type = (string)((List<Object>)(((Button)sender).Tag))[0];
            ListBox list = (ListBox)((List<Object>)(((Button)sender).Tag))[1];
            AddParticipantToMeeting addWindow = new AddParticipantToMeeting(type,list);
            addWindow.ShowDialog();
            if(type == "group")
            {
                ListBox pGroupList = (ListBox)this.FindName("Groups");
                ListBoxItem newGroup = new ListBoxItem();
                if ((bool)addWindow.DialogResult)
                {
                    newGroup.Content = depModel.Get(addWindow.id).name;
                    newGroup.Tag = addWindow.id;
                    pGroupList.Items.Add(newGroup);
                }
            }
            else if(type == "user")
            {
                ListBox pGroupList = (ListBox)this.FindName("Users");
                ListBoxItem newGroup = new ListBoxItem();
                if (userModel.Get(addWindow.id) != null)
                {
                    newGroup.Content = userModel.Get(addWindow.id).fname + " "+ userModel.Get(addWindow.id).lname;
                    newGroup.Tag = addWindow.id;
                    pGroupList.Items.Add(newGroup);
                }
            }
            else
            {
                ListBox pGroupList = (ListBox)FindName("Positions");
                ListBoxItem newGroup = new ListBoxItem();
                if (posModel.Get(addWindow.id) != null)
                {
                    newGroup.Content = posModel.Get(addWindow.id).name;
                    newGroup.Tag = addWindow.id;
                    pGroupList.Items.Add(newGroup);
                }
            }
        }

        void removeMeeting(object sender, RoutedEventArgs e)
        {
            Meeting meeting = (Meeting)((Button)sender).Tag;
            if(meetingModel.MarkAsDeleted(meeting.id))
            {
                MessageBox.Show("Амжилттай устгалаа");
                ShowMeetings();
            }
            else
            {
                MessageBox.Show( "Та орчноо шалгаад дахин оролдоно уу!","Бүтэлгүйтлээ");
            }

            
        }
        /** Is called when meeting time has to change only 1 time
         * Gets data from meeting , modifies and inserts into Modified meeting
         */
        void ShowMeetings(object sender=null, RoutedEventArgs e=null)
        {
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
            DockPanel dockPanel = addHeader(controls,rcontrols);

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
            listbox.Margin = new Thickness(10, 10, 10, 10);
            listbox.HorizontalAlignment = HorizontalAlignment.Stretch;

            List < Meeting >  meetings = meetingModel.GetAll();

            foreach(Meeting meeting in meetings)
            {
                if(meeting.isDeleted)
                {
                    continue;
                }
                ListBoxItem listBoxItem = new ListBoxItem();

                listBoxItem.Content = meeting.name;
                listBoxItem.Uid = meeting.id.ToString();
                listBoxItem.Height = 25;
                listbox.Items.Add(listBoxItem);
            }
            listbox.SelectionChanged += onMeetingNameChanged;

            dockPanel.Children.Add(listbox);


        }

        void setUser(object sender,RoutedEventArgs e)
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
                MessageBox.Show("Амжилттай өөрчлөгдлөө", "Өөрчлөгдлөө");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Та программ ажиллуулж байгаа орчиноо шалгана уу", "Өөрчилсөнгүй");
            }
            ShowMembers(sender,null);
        }
        void onUserChanged(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            string id = ((ListBoxItem)listBox.SelectedValue).Uid;

            RightSide.Children.Clear();

            User user = userModel.Get(Int32.Parse(id));
            List<Object> controls = new List<Object>();

            Grid grid = new Grid();
            grid.ShowGridLines = true;
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(.5);

            ColumnDefinition col0 = new ColumnDefinition();
            col0.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition col1 = new ColumnDefinition();
            col1.Width = new GridLength(1, GridUnitType.Star);

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

            grid.RowDefinitions.Add(row0);
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            grid.RowDefinitions.Add(row3);
            
            for(int i = 0;i<5;i++)
            {
                Label nameLabel = new Label();
                Label valueLabel = new Label();
                Grid.SetColumn(nameLabel, 0);
                Grid.SetColumn(valueLabel, 1);
                Grid.SetRow(nameLabel, i);
                Grid.SetRow(valueLabel, i);
                switch (i)
                {
                    case 0:
                        nameLabel.Content = "Овог:";
                        valueLabel.Content = user.lname;
                        break;
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
                }
                grid.Children.Add(nameLabel);
                grid.Children.Add(valueLabel);
            }

            border.Child = grid;
            RightSide.Children.Add(border);

        }
        void importData(object sender, RoutedEventArgs e)
        {
            ImportUser iuser = new ImportUser();
            iuser.ShowDialog();

            if (iuser.DialogResult == true)
            {
                ExternalDataImporter edi = new ExternalDataImporter();
                edi.ImportUserData(iuser.xlPath, iuser.datPath);
                ShowMembers(null,null);
            }
        }
        void ShowMembers(object sender, RoutedEventArgs e)
        {
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

            DockPanel dockPanel = addHeader(buttons,rbuttons);

            ListBox listbox = new ListBox();
            try
            {
                RegisterName("listbox", listbox);
            }
            catch(Exception ex)
            {
                UnregisterName("listbox");
                RegisterName("listbox", listbox);
            }
            listbox.Margin = new Thickness(10, 10, 10, 10);
            
            List<User> users = userModel.GetAll();
            foreach(User user in users)
            {
                ListBoxItem listBoxItem = new ListBoxItem();

                listBoxItem.Content = user.lname + " " + user.fname;
                listBoxItem.Uid = user.id.ToString();
                listBoxItem.Height = 25;
                listbox.Items.Add(listBoxItem);
            }
            listbox.SelectionChanged += onUserChanged;

            dockPanel.Children.Add(listbox);

        }
        void ShowReport(object sender, RoutedEventArgs e)
        {
            ReportExporter re = new ReportExporter(meetingController);
            re.ExportAttendance(meetingModel.Get(4), DateTime.Parse("2018-08-19"), DateTime.Parse("2018-08-30"), "sample");
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            meetingController.aTimer.Stop();
            meetingController.StopMeeting();
            base.OnClosing(e);
        }
    }
}

