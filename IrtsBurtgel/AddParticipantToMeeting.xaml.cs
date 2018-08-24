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
        public int id;
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
                foreach (Department dep in deps)
                {
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = dep.name;
                    listBoxItem.Tag = dep.id;
                    listbox.Items.Add(listBoxItem);
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
                foreach (User user in users)
                {
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = user.fname + " " + user.lname;
                    listBoxItem.Tag = user.id;
                    listbox.Items.Add(listBoxItem);
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
                foreach (Position user in users)
                {
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = user.name;
                    listBoxItem.Tag = user.id;
                    listbox.Items.Add(listBoxItem);
                }
            }
        }
        void Add(object sender,RoutedEventArgs e)
        {
            DialogResult = listbox.SelectedItem !=null;
            if((bool)DialogResult) id = (Int32)((ListBoxItem)listbox.SelectedItem).Tag;
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
                foreach (Department dep in deps)
                {
                    if (!Regex.IsMatch(dep.name.ToString().ToLower(), "[a-zA-Z\\d]*" + searchText.ToLower() + "[a-zA-Z\\d]*"))
                    {
                        continue;
                    }
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = dep.name;
                    listBoxItem.Tag = dep.id;
                    listbox.Items.Add(listBoxItem);
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
                foreach (User user in users)
                {
                    if (!Regex.IsMatch(user.fname.ToString().ToLower(), "[a-zA-Z\\d]*" + searchText.ToLower() + "[a-zA-Z\\d]*")
                        ||
                        !Regex.IsMatch(user.lname.ToString().ToLower(), "[a-zA-Z\\d]*" + searchText.ToLower() + "[a-zA-Z\\d]*"))
                    {
                        continue;
                    }
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = user.fname + " " + user.lname;
                    listBoxItem.Tag = user.id;
                    listbox.Items.Add(listBoxItem);
                }
            }
            else
            {
                List<Position> users = pModel.GetAll();
                foreach (ListBoxItem listBoxItem in cList.Items)
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
                foreach (Position user in users)
                {
                    if (!Regex.IsMatch(user.name.ToString().ToLower(), "[a-zA-Z\\d]*" + searchText.ToLower() + "[a-zA-Z\\d]*"))
                    {
                        continue;
                    }
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = user.name;
                    listBoxItem.Tag = user.id;
                    listbox.Items.Add(listBoxItem);
                }
            }
        }
    }
}
