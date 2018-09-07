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
        ListBox cList;

        Model<Department> model = new Model<Department>();
        Model<User> uModel = new Model<User>();
        Model<Position> pModel = new Model<Position>();

        public AddParticipantToMeeting(string type,ListBox oList)
        {
            InitializeComponent();
            this.type = type;
            cList = oList;
            if (type == "group")
            {
                List<Department> deps= model.GetAll();
                foreach (ListBoxItem listBoxItem in oList.Items)
                {
                    foreach(Department dep in deps)
                    {
                        if(dep.id == Int32.Parse(listBoxItem.Tag.ToString()))
                        {
                            deps.Remove(dep);
                            break;
                        }
                    }
                }
                int i = 1;
                foreach (Department dep in deps)
                {
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = i + ". " + dep.name;
                    listBoxItem.Tag = dep.id;
                    listbox.Items.Add(listBoxItem);
                    i++;
                }
            }
            else if(type == "user")
            {
                List<User> users = uModel.GetAll();
                foreach (ListBoxItem listBoxItem in oList.Items)
                {
                    foreach (User user in users)
                    {
                        if (user.id == Int32.Parse(listBoxItem.Tag.ToString()))
                        {
                            users.Remove(user);
                            break;
                        }
                    }
                }

                Dictionary<int, string> departments = model.GetAll().ToDictionary(x => x.id, x => x.name);
                int i = 1;
                foreach (User user in users)
                {
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = i + ". " + user.fname + " " + user.lname + ", " + (user.departmentId != -1 ? departments[user.departmentId] : "Хэлтэсгүй");
                    listBoxItem.Tag = user.id;
                    listbox.Items.Add(listBoxItem);
                    i++;
                }
            }
            else
            {
                List<Position> users = pModel.GetAll();
                foreach (ListBoxItem listBoxItem in oList.Items)
                {
                    foreach (Position user in users)
                    {
                        if (user.id == Int32.Parse(listBoxItem.Tag.ToString()))
                        {
                            users.Remove(user);
                            break;
                        }
                    }
                }
                int i = 1;
                foreach (Position user in users)
                {
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = i + ". " + user.name;
                    listBoxItem.Tag = user.id;
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
                List<Department> deps = model.GetAll();
                foreach (ListBoxItem listBoxItem in cList.Items)
                {
                    foreach (Department dep in deps)
                    {
                        if (dep.id == Int32.Parse(listBoxItem.Tag.ToString()))
                        {
                            deps.Remove(dep);
                            break;
                        }
                    }

                }
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
                    listbox.Items.Add(listBoxItem);
                    i++;
                }
            }
            else if (type == "user")
            {
                List<User> users = uModel.GetAll();
                foreach (ListBoxItem listBoxItem in cList.Items)
                {
                    foreach (User user in users)
                    {
                        if (user.id == Int32.Parse(listBoxItem.Tag.ToString()))
                        {
                            users.Remove(user);
                            break;
                        }
                    }
                }
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
                    listbox.Items.Add(listBoxItem);
                    i++;
                }
            }
            else
            {
                List<Position> positions = pModel.GetAll();
                foreach (ListBoxItem listBoxItem in cList.Items)
                {
                    foreach (Position position in positions)
                    {
                        if (position.id == Int32.Parse(listBoxItem.Tag.ToString()))
                        {
                            positions.Remove(position);
                            break;
                        }
                    }
                }
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
