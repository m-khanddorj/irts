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

        UserModel userModel;


        public MainWindow()
        {
            InitializeComponent();

            meetingController = new MeetingController();
            meetingModel = meetingController.meetingModel;
            userModel = new Model<User>();

            userModel = new UserModel();
        }
        
        private void showMenu(object sender, RoutedEventArgs e)
        {
            LeftSide.Children.Clear();
            LeftSide.HorizontalAlignment = HorizontalAlignment.Center;
            LeftSide.VerticalAlignment = VerticalAlignment.Center;

            StackPanel Menu = new StackPanel();
            Menu.Name = "MainMenu";

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

            LeftSide.Children.Add(Menu);

            RightSide.Children.Clear();
            RightSide.HorizontalAlignment = HorizontalAlignment.Center;

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
            LeftSide.HorizontalAlignment = HorizontalAlignment.Center;

            DockPanel dockPanel = new DockPanel();

            DockPanel headerPanel = new DockPanel();
            headerPanel.Width = 430;
            headerPanel.Margin = new Thickness(10, 5, 10, -5);

            Button back = new Button();

            back.Content = "<";
            back.Click += showMenu;
            back.Background = Brushes.White;
            back.Height = 30;
            back.Width = 30;

            headerPanel.Children.Add(back);
            DockPanel.SetDock(headerPanel, Dock.Top);

            if (controls != null)
            {
                foreach (Object control in controls)
                {
                    if(control is Button)
                    {
                        Button tmp = (Button)control;
                        tmp.Height = 30;
                        tmp.Width = 30;
                        tmp.Background = Brushes.White;
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
                        tmp.Background = Brushes.White;
                        tmp.Margin = new Thickness(5, 0, 5, 0);
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

            dockPanel.Children.Add(headerPanel);

            LeftSide.Children.Add(dockPanel);
            return dockPanel;
        }

        void OnSelectedDateChange(object sender, RoutedEventArgs e)
        {
            Calendar calendar = (Calendar)sender;

            LeftSide.Children.Clear();

            List<Object> list = new List<Object>();
            Label header = new Label();
            header.Content = "Тухайн өдрийн хурлууд";
            list.Add(header);

            Button rButton = new Button();
            rButton.Content = "c";
            rButton.Click += ShowCalendar;

            List<Object> rControls = new List<Object>();
            rControls.Add(rButton);

            DockPanel dockPanel = addHeader(list, rControls);

            List<Meeting> meetings = meetingController.FindByDate( (DateTime)calendar.SelectedDate );

            ListBox listbox = new ListBox();
            listbox.Margin = new Thickness(10, 10, 10, 10);
            listbox.MinWidth = 430;
           

            foreach (Meeting meeting in meetings)
            {
                ListBoxItem listBoxItem = new ListBoxItem();

                listBoxItem.Content = meeting.name;
                listBoxItem.Uid = meeting.id.ToString();
                listBoxItem.Height = 25;
                listbox.Items.Add(listBoxItem);
            }
            listbox.SelectionChanged +=ModifyMeeting;

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
            rButton.Content = "c";
            rButton.Click += ShowCalendar;

            List<Object> rControls = new List<Object>();
            rControls.Add(rButton);

            DockPanel dockPanel = addHeader(list,rControls);

            Viewbox ll = new Viewbox();

            dockPanel.MinWidth = 430;
            dockPanel.Children.Add(ll);

            Calendar calendar = new Calendar();
            Viewbox viewbox = new Viewbox();
            viewbox.Stretch = Stretch.Uniform;
            viewbox.Child = calendar;
            calendar.SelectedDatesChanged += OnSelectedDateChange;
            RightSide.Children.Add(viewbox);

            ListBox listbox = new ListBox();
            listbox.Margin = new Thickness(10, 10, 10, 10);
            listbox.MinWidth = 430;

            List<Meeting> meetings = meetingController.FindByDate(DateTime.Today);

            foreach (Meeting meeting in meetings)
            {
                ListBoxItem listBoxItem = new ListBoxItem();

                listBoxItem.Content = meeting.name;
                listBoxItem.Uid = meeting.id.ToString();
                listBoxItem.Height = 25;
                listbox.Items.Add(listBoxItem);
            }
            listbox.SelectionChanged += ModifyMeeting;

            dockPanel.Children.Add(listbox);
        }
        void insertMeeting(object sender, RoutedEventArgs e)
        {
            List<Object> controls = (List<Object>)((Button)sender).Tag;

            string name = ((TextBox)controls[0]).Text;
            string st = ((TextBox)controls[1]).Text;
            string sd = ((DateTime)((DatePicker)controls[2]).SelectedDate).ToShortDateString();
            
        }
        void setMeeting(object sender, RoutedEventArgs e)
        {
           List<Object> controls= (List<Object>)((Button)sender).Tag;
            TextBox name = (TextBox)controls[0];
            TextBox st = (TextBox)controls[1];
            DatePicker sd = (DatePicker)controls[2];

            TextBox duration = (TextBox)controls[3];
            DatePicker ed = (DatePicker)controls[4];
            TextBox freq = (TextBox)controls[5];
            Meeting meeting = (Meeting)controls[6];

            meeting.name = name.Text;

            string datetimeString = sd.Text +" "+ st.Text;
            //check and set meeting startDateTime
            try
            {
                meeting.startDatetime = DateTime.Parse(datetimeString);
            }
            catch( Exception ex )
            {
                MessageBox.Show("Та цагаа цаг:минут гэсэн хэлбэрээр бичнэ үү!","Өөрчилсөнгүй");
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
                MessageBox.Show("Та хурал болох давтамжаа  шалгана уу", "Өөрчилсөнгүй");
            }
            //check and set meeting intervalday
            try
            {
                meeting.intervalDay = Int32.Parse(freq.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Та хурал болох давтамжаа  шалгана уу", "Өөрчилсөнгүй");
            }
            //check and update meeting
            try
            {
                meetingModel.Set(meeting);
                MessageBox.Show("Амжилттай өөрчиллөө.","Өөрчлөгдлөө");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Та программ ажиллуулж буй орчиноо шалгана уу","Өөрчилсөнгүй");
            }
            ShowMeetings(sender, null);
        }
        /** Is called when meeting time has to change only 1 time
         * Gets data from meeting , modifies and inserts into Modified meeting
         */
        void ModifyMeeting(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            Label label = new Label();
            string id = ((ListBoxItem)listBox.SelectedValue).Uid;

            RightSide.Children.Clear();

            Meeting meeting = meetingModel.Get(Int32.Parse(id));
            List<Object> controls = new List<Object>();

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
            st.Text = meeting.startDatetime.ToShortTimeString();
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
            duration.Text = meeting.duration.ToString();
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
            saveButton.Click += setMeeting;

            saveStack.Children.Add(saveButton);

            stackPanel.Children.Add(nameStack);
            stackPanel.Children.Add(stStack);
            stackPanel.Children.Add(durationStack);
            stackPanel.Children.Add(saveStack);

            RightSide.Children.Add(stackPanel);
        }
        void onMeetingNameChanged(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            Label label = new Label();
            string id = ((ListBoxItem)listBox.SelectedValue).Uid;

            RightSide.Children.Clear();

            Meeting meeting = meetingModel.Get( Int32.Parse(id) );
            List<Object> controls = new List<Object>();
            
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.Margin = new Thickness(10);

            /**
                * Name stack
                */

            StackPanel nameStack = new StackPanel();
            nameStack.Orientation = Orientation.Horizontal;
            nameStack.Margin = new Thickness(0,5,0,5);

            Label nameLabel = new Label();
            nameLabel.Content = "Хурлын нэр:";
            nameLabel.Width = 215;
            TextBox name = new TextBox();
            name.Width = 215;
            name.Text =(string) meeting.name;
            controls.Add(name);

            nameStack.Children.Add(nameLabel);
            nameStack.Children.Add(name);

            /**
                * Starting time stack
                */

            StackPanel stStack = new StackPanel();
            stStack.Orientation = Orientation.Horizontal;
            stStack.Margin = new Thickness(0,5,0,5);

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
            sdStack.Margin = new Thickness(0,5,0,5);

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
            fStack.Margin = new Thickness(0,5,0,5);

            Label fLabel = new Label();
            fLabel.Content = "Хурал болох давтамж";
            fLabel.Width = 215;
            TextBox freq = new TextBox();
            freq.Text = meeting.intervalDay.ToString();
            freq.Width = 25;
            controls.Add(freq);
            Label fUnitLabel = new Label();
            fUnitLabel.Content = "Хоног";


            fStack.Children.Add(fLabel);
            fStack.Children.Add(freq);
            fStack.Children.Add(fUnitLabel);

            /**
            * Save button 
            */
            StackPanel saveStack = new StackPanel();
            saveStack.HorizontalAlignment = HorizontalAlignment.Right;
            saveStack.Margin = new Thickness(0,5,0,5);

            Button saveButton = new Button();
            saveButton.Content = "Хадгалах";
            saveButton.Background = Brushes.White;
            saveButton.Height = 25;
            saveButton.Width = 100;

            controls.Add(meeting);

            saveButton.Tag = controls;
            saveButton.Click += setMeeting;

            saveStack.Children.Add(saveButton);

            stackPanel.Children.Add(nameStack);
            stackPanel.Children.Add(stStack);
            stackPanel.Children.Add(sdStack);
            stackPanel.Children.Add(durationStack);
            stackPanel.Children.Add(edStack);
            stackPanel.Children.Add(fStack);
            stackPanel.Children.Add(saveStack);

            RightSide.Children.Add(stackPanel);
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
            fStack.Margin = new Thickness(0, 5, 0, 5);

            Label fLabel = new Label();
            fLabel.Content = "Хурал болох давтамж";
            fLabel.Width = 215;
            TextBox freq = new TextBox();
            freq.Width = 25;
            Label fUnitLabel = new Label();
            fUnitLabel.Content = "Хоног";
            controls.Add(freq);


            fStack.Children.Add(fLabel);
            fStack.Children.Add(freq);
            fStack.Children.Add(fUnitLabel);

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
            stackPanel.Children.Add(saveStack);

            RightSide.Children.Add(stackPanel);
        }
        void ShowMeetings(object sender, RoutedEventArgs e)
        {
            LeftSide.Children.Clear();
            Label label = new Label();
            label.Content = "Нийт хурлууд:";

            Button import = new Button();
            import.Content = "+";
            import.Click += addMeeting;

            List<Object> controls = new List<Object>();
            controls.Add(import);
            controls.Add(label);
            DockPanel dockPanel = addHeader(controls);

            ListBox listbox = new ListBox();
            listbox.Margin = new Thickness(10, 10, 10, 10);
            listbox.HorizontalAlignment = HorizontalAlignment.Stretch;

            List < Meeting >  meetings = meetingModel.GetAll();

            foreach(Meeting meeting in meetings)
            {
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

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.Margin = new Thickness(10);
            /**
             * lName stack
             */

            StackPanel lNameStack = new StackPanel();
            lNameStack.Orientation = Orientation.Horizontal;
            lNameStack.Margin = new Thickness(0, 5, 0, 5);

            Label lNameLabel = new Label();
            lNameLabel.Content = "Овог:";
            lNameLabel.Width = 215;
            TextBox lName = new TextBox();
            lName.Width = 215;
            lName.Text = user.lname;
            controls.Add(lName);

            lNameStack.Children.Add(lNameLabel);
            lNameStack.Children.Add(lName);


            /**
             * fName stack
             */

            StackPanel fNameStack = new StackPanel();
            fNameStack.Orientation = Orientation.Horizontal;
            fNameStack.Margin = new Thickness(0, 5, 0, 5);

            Label fNameLabel = new Label();
            fNameLabel.Content = "Нэр:";
            fNameLabel.Width = 215;
            TextBox fName = new TextBox();
            fName.Width = 215;
            fName.Text = user.fname;
            controls.Add(fName);

            fNameStack.Children.Add(fNameLabel);
            fNameStack.Children.Add(fName);


            /**
             * Save button 
             */
            StackPanel saveStack = new StackPanel();
            saveStack.Orientation = Orientation.Horizontal;
            saveStack.HorizontalAlignment = HorizontalAlignment.Right;
            saveStack.Margin = new Thickness(0, 5, 0, 5);

            Button saveButton = new Button();
            saveButton.Content = "Хадгалах";
            saveButton.Background = Brushes.White;
            saveButton.Height = 25;
            saveButton.Width = 105;

            controls.Add(user);
            saveButton.Tag = controls;
            saveButton.Click += setUser;

            Button deleteButton = new Button();
            deleteButton.Content = "Устгах";
            deleteButton.Background = Brushes.White;
            deleteButton.Foreground = Brushes.Red;
            deleteButton.Width = 105;
            deleteButton.Height = 25;
            deleteButton.Margin = new Thickness(0, 0, 5, 0);
            saveStack.Children.Add(deleteButton);
            saveStack.Children.Add(saveButton);

            stackPanel.Children.Add(lNameStack);
            stackPanel.Children.Add(fNameStack);
            stackPanel.Children.Add(saveStack);

            RightSide.Children.Add(stackPanel);

        }
        void importData(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".dat";
            dlg.Filter = "DAT Files (*.dat)|*.dat";

            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                MessageBox.Show(filename);
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

            List<Object> buttons = new List<Object>();
            buttons.Add(import);
            buttons.Add(label);

            DockPanel dockPanel = addHeader(buttons);

            ListBox listbox = new ListBox();
            listbox.Margin = new Thickness(10, 10, 10, 10);
            listbox.MinWidth = 430;
            
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
            MessageBox.Show("h");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScannerHandler sh = new ScannerHandler(userModel.GetAll());
            ExternalDataImporter imp = new ExternalDataImporter();

            imp.ImportUserData("C:\\Users\\Orgio\\Documents\\SampleData.xlsx", "C:\\Users\\Orgio\\Documents\\Attendance Registration\\Dat File\\template.fp10.1");
            
            sh.InitializeDevice();
            sh.StartCaptureThread();
            
        }

        public void ChangeImage(byte[] stream)
        {
            var bitmapImage = new BitmapImage();
            MemoryStream ms = new MemoryStream();
            byte[] haha = stream;
            BitmapFormat.GetBitmap(haha, 300, 375, ref ms);

            bitmapImage.BeginInit();
            bitmapImage.StreamSource = ms;
            bitmapImage.EndInit();
            image.Source = bitmapImage;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            userModel.Add(new User
            {
                id = -1,
                fname = "testFName",
                lname = "TestLname",
                fingerprint0 = "asfaf",
                fingerprint1 = "asfaawdadf",
                isDeleted = false
            });
        }
    }
}

