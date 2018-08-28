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
    /// Interaction logic for ImportUser.xaml
    /// </summary>
    public partial class ImportUser : Window
    {
        public string xlPath;
        public string datPath;
        public int c;
        public ImportUser()
        {
            InitializeComponent();
            c = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".1";
            dlg.Filter = "1 file (*.1)|*.1";
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                datPath = filename;
                datImage.Source = new BitmapImage(new Uri("images/tick.png", UriKind.Relative));
                c++;
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Excel Files (*.xlsx)|*.xlsx";
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                xlPath = filename;
                xlImage.Source = new BitmapImage(new Uri("images/tick.png", UriKind.Relative));
                c++;
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if(c==3) this.DialogResult = true;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
