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

namespace IrtsBurtgel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MeetingModel meetingModel;
        MeetingController meetingController;

        SqlConnection conn;
        SqlCommand cmd;
        public MainWindow()
        {
            InitializeComponent();

            meetingController = new MeetingController();
            meetingModel = meetingController.meetingModel;

            conn = new SqlConnection();
            cmd = new SqlCommand();
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

        DockPanel addButton(Array buttons = null)
        {
            LeftSide.VerticalAlignment = VerticalAlignment.Stretch;
            LeftSide.HorizontalAlignment = HorizontalAlignment.Center;

            DockPanel dockPanel = new DockPanel();

            StackPanel menuPanel = new StackPanel();
            menuPanel.HorizontalAlignment = HorizontalAlignment.Left;
            menuPanel.Orientation = Orientation.Horizontal;
            menuPanel.Margin = new Thickness(10, 5, 0, -5);

            Button back = new Button();

            back.Content = "<";
            back.Click += showMenu;
            back.Background = Brushes.White;
            back.Height = 30;
            back.Width = 30;

            menuPanel.Children.Add(back);
            DockPanel.SetDock(menuPanel, Dock.Top);

            if (buttons != null)
            {
                foreach (Button button in buttons)
                {
                    button.Height = 30;
                    button.Width = 30;
                    button.Background = Brushes.White;
                    button.Margin = new Thickness(5, 0, 0, 0);
                    menuPanel.Children.Add(button);
                }
            }

            dockPanel.Children.Add(menuPanel);

            LeftSide.Children.Add(dockPanel);
            return dockPanel;
        }

        void OnSelectedDateChange(object sender, RoutedEventArgs e)
        {
            Calendar calendar = (Calendar)sender;

            LeftSide.Children.Clear();
            DockPanel dockPanel = addButton();

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
            listbox.SelectionChanged += onMeetingNameChanged;

            dockPanel.Children.Add(listbox);

        }
        void ShowCalendar(object sender, RoutedEventArgs e)
        {
            RightSide.Children.Clear();
            LeftSide.Children.Clear();

            DockPanel dockPanel = addButton();

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
            listbox.SelectionChanged += onMeetingNameChanged;

            dockPanel.Children.Add(listbox);
        }

        void onMeetingNameChanged(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            Label label = new Label();
            string id = ((ListBoxItem)listBox.SelectedValue).Uid;

            RightSide.Children.Clear();

            Meeting meeting = meetingModel.Get( Int32.Parse(id) );
            
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
            st.Text = ((DateTime)meeting.startTime).ToShortTimeString();

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
            sd.DisplayDate = ((DateTime)meeting.startDate);
            sd.Text = ((DateTime)meeting.startDate).ToShortDateString();

            sdStack.Children.Add(sdLabel);
            sdStack.Children.Add(sd);

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

            saveStack.Children.Add(saveButton);

            stackPanel.Children.Add(nameStack);
            stackPanel.Children.Add(stStack);
            stackPanel.Children.Add(sdStack);
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

            sdStack.Children.Add(sdLabel);
            sdStack.Children.Add(sd);

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

            saveStack.Children.Add(saveButton);

            stackPanel.Children.Add(nameStack);
            stackPanel.Children.Add(stStack);
            stackPanel.Children.Add(sdStack);
            stackPanel.Children.Add(fStack);
            stackPanel.Children.Add(saveStack);

            RightSide.Children.Add(stackPanel);
        }
        void ShowMeetings(object sender, RoutedEventArgs e)
        {
            LeftSide.Children.Clear();

            Button import = new Button();
            import.Content = "+";
            import.Click += addMeeting;

            Button[] buttons = new Button[1];
            buttons[0] = import;

            DockPanel dockPanel = addButton(buttons);

            ListBox listbox = new ListBox();
            listbox.Margin = new Thickness(10, 10, 10, 10);
            listbox.MinWidth=430;

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

        void onUserChanged(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            string id = ((ListBoxItem)listBox.SelectedValue).Uid;

            RightSide.Children.Clear();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
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
                    lName.Text = (string)reader["lName"];

                    lNameStack.Children.Add(lNameLabel);
                    lNameStack.Children.Add(lName);


                    /**
                     * ffName stack
                     */

                    StackPanel fNameStack = new StackPanel();
                    fNameStack.Orientation = Orientation.Horizontal;
                    fNameStack.Margin = new Thickness(0, 5, 0, 5);

                    Label fNameLabel = new Label();
                    fNameLabel.Content = "Нэр:";
                    fNameLabel.Width = 215;
                    TextBox fName = new TextBox();
                    fName.Width = 215;
                    fName.Text = (string)reader["fname"];

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
                conn.Close();
            }
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

            Button import = new Button();
            import.Content = "+";
            import.Click += importData;

            Button[] buttons = new Button[1];
            buttons[0] = import;

            DockPanel dockPanel = addButton(buttons);

            ListBox listbox = new ListBox();
            listbox.Margin = new Thickness(10, 10, 10, 10);
            listbox.MinWidth = 430;

            conn.Open();
            cmd = new SqlCommand("SELECT * FROM \"user\"", conn);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ListBoxItem listBoxItem = new ListBoxItem();

                    listBoxItem.Content = ((string)reader["lname"])+" "+reader["fname"];
                    listBoxItem.Uid = reader["user_id"].ToString();
                    listBoxItem.Height = 25;
                    listbox.Items.Add(listBoxItem);
                }
                conn.Close();
            }
            listbox.SelectionChanged += onUserChanged;

            dockPanel.Children.Add(listbox);

        }

        void ShowReport(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("h");
        }
    }
}

