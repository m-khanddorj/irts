using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for AddParticipantToMeeting.xaml
    /// </summary>
    public partial class AddParticipantToMeeting : Window
    {
        string type;
        public int[] ids;

        Model<Department> model = new Model<Department>();
        Model<User> uModel = new Model<User>();
        Model<Position> pModel = new Model<Position>();

        List<Department> deps = new List<Department>();
        List<User> users = new List<User>();
        List<Position> positions = new List<Position>();

        Dictionary<int, bool> isAlreadyIn = new Dictionary<int, bool>();

        public AddParticipantToMeeting(string type, ListBox dList, ListBox uList, ListBox pList)
        {
            InitializeComponent();
            this.type = type;
            if (type == "group")
            {
                deps = model.GetAll();
                deps = deps.OrderBy(x => x.name).ToList();
                foreach (Department dep in deps)
                {
                    bool inList = false;
                    foreach (ListBoxItem listBoxItem in dList.Items)
                    {
                        if(dep.id == Int32.Parse(listBoxItem.Tag.ToString()))
                        {
                            inList = true;
                            break;
                        }
                    }

                    isAlreadyIn.Add(dep.id, inList);
                }
                int i = 1;
                foreach (Department dep in deps)
                {
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = i + ". " + dep.name;
                    listBoxItem.Tag = dep.id;
                    listBoxItem.IsEnabled = !isAlreadyIn[dep.id];
                    listbox.Items.Add(listBoxItem);
                    i++;
                }
            }
            else if(type == "user")
            {
                users = uModel.GetAll();
                users = users.OrderBy(x => x.fname).ToList();
                foreach (User user in users)
                {
                    bool inList = false;
                    foreach (ListBoxItem listBoxItem in uList.Items)
                    {
                        if (user.id == Int32.Parse(listBoxItem.Tag.ToString()))
                        {
                            inList = true;
                            break;
                        }
                    }

                    foreach (ListBoxItem listBoxItem in dList.Items)
                    {
                        if (user.departmentId == Int32.Parse(listBoxItem.Tag.ToString()))
                        {
                            inList = true;
                            break;
                        }
                    }

                    foreach (ListBoxItem listBoxItem in pList.Items)
                    {
                        if (user.positionId == Int32.Parse(listBoxItem.Tag.ToString()))
                        {
                            inList = true;
                            break;
                        }
                    }
                    
                     isAlreadyIn.Add(user.id, inList);
                }

                Dictionary<int, string> departments = model.GetAll().ToDictionary(x => x.id, x => x.name);
                int i = 1;
                foreach (User user in users)
                {
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = i + ". " + user.fname + " " + user.lname + ", " + (user.departmentId != -1 ? departments[user.departmentId] : "Хэлтэсгүй");
                    listBoxItem.Tag = user.id;
                    listBoxItem.IsEnabled = !isAlreadyIn[user.id];
                    listbox.Items.Add(listBoxItem);
                    i++;
                }
            }
            else
            {
                positions = pModel.GetAll();
                positions = positions.OrderBy(x => x.name).ToList();
                foreach (Position position in positions)
                {
                    bool inList = false;
                    foreach (ListBoxItem listBoxItem in pList.Items)
                    {
                        if (position.id == Int32.Parse(listBoxItem.Tag.ToString()))
                        {
                            inList = true;
                            break;
                        }
                    }
                    isAlreadyIn.Add(position.id, inList);
                }

                int i = 1;
                foreach (Position position in positions)
                {
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = i + ". " + position.name;
                    listBoxItem.Tag = position.id;
                    listBoxItem.IsEnabled = !isAlreadyIn[position.id];
                    listbox.Items.Add(listBoxItem);
                    i++;
                }
            }
        }
        private void ListBoxItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ListBoxItem lbi = sender as ListBoxItem;
                lbi.IsSelected = !lbi.IsSelected;
                lbi.Focus();
                listbox.SelectedItems.Add(lbi);
            }
        }

        void Add(object sender,RoutedEventArgs e)
        {
            DialogResult = listbox.SelectedItem !=null;
            List<int> selectedIds = new List<int>();
            if ((bool)DialogResult)
            {
                foreach (ListBoxItem lbi in listbox.SelectedItems)
                {
                    selectedIds.Add((int)lbi.Tag);
                }
            }
            ids = selectedIds.ToArray();
        }

        void Search(object sender, RoutedEventArgs e)
        {
            string searchText = searchBox.Text;
            listbox.Items.Clear();
            if (type == "group")
            {
                int i = 1;
                foreach (Department dep in deps)
                {
                    if (!Regex.IsMatch(dep.name.ToString().ToLower(), "[.]*" + searchText.ToLower() + "[.]*"))
                    {
                        continue;
                    }

                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = i + ". " + dep.name;
                    listBoxItem.Tag = dep.id;
                    listBoxItem.IsEnabled = !isAlreadyIn[dep.id];
                    listbox.Items.Add(listBoxItem);
                    i++;
                }
            }
            else if (type == "user")
            {
                Dictionary<int, string> departments = model.GetAll().ToDictionary(x => x.id, x => x.name);
                int i = 1;
                foreach (User user in users)
                {
                    string name = (user.fname + " " + user.lname).ToLower();
                    if ( !Regex.IsMatch(name, "[.]*" + searchText.ToLower() + "[.]*") )
                    {
                        continue;
                    }
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = i + ". " + user.fname + " " + user.lname + ", " + (user.departmentId != -1 ? departments[user.departmentId]:"Хэлтэсгүй");
                    listBoxItem.Tag = user.id;
                    listBoxItem.IsEnabled = !isAlreadyIn[user.id];
                    listbox.Items.Add(listBoxItem);
                    i++;
                }
            }
            else
            {
                int i = 1;
                foreach (Position position in positions)
                {
                    if (!Regex.IsMatch(position.name.ToString().ToLower(), "[.]*" + searchText.ToLower() + "[.]*"))
                    {
                        continue;
                    }
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = i + ". " + position.name;
                    listBoxItem.Tag = position.id;
                    listBoxItem.IsEnabled = !isAlreadyIn[position.id];
                    listbox.Items.Add(listBoxItem);
                    i++;
                }
            }
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
                listbox.SelectedItems.Clear();
        }
    }
}
