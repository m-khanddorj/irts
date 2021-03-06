﻿using Microsoft.Win32;
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
        public string fpdatPath;
        public string userdatPath;
        public string[] imagePaths;
        public ImportUser()
        {
            InitializeComponent();
            imagePaths = new string[0];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".1";
            dlg.Filter = "1 file (*.1)|*.1";
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                fpdatPath = filename;
                fpdatImage.Source = new BitmapImage(new Uri("images/tick.png", UriKind.Relative));
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
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
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (xlPath != "") this.DialogResult = true;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Images (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg";

            dlg.Multiselect = true;
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string[] filenames = dlg.FileNames;
                imagePaths = filenames;
                imageImage.Source = new BitmapImage(new Uri("images/tick.png", UriKind.Relative));
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".dat";
            dlg.Filter = "User data file (user.dat)|user.dat";
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                userdatPath = filename;
                userdatImage.Source = new BitmapImage(new Uri("images/tick.png", UriKind.Relative));
            }

        }
    }
}
