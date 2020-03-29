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
    /// Interaction logic for UpdateFingerprint.xaml
    /// </summary>
    public partial class UpdateFingerprint : Window
    {
        int uid;
        User user;
        Model<User> userModel;
        ScannerHandler scannerHandler;
        MeetingController mc;
        public int RegisterCount;

        public UpdateFingerprint(int uid, MeetingController mc)
        {
            InitializeComponent();
            this.uid = uid;
            this.mc = mc;   
            scannerHandler = mc.scannerHandler;

            userModel = new Model<User>();
            user = userModel.Get(uid);

            RegisterCount = 0;

            if (mc.onGoingMeeting != null)
            {
                scannerHandler.Stop();
            }
            scannerHandler.InitializeDevice();
            scannerHandler.StartCaptureThread(scannerHandler.DoCaptureForMember);
            InfoLabel.Content = "Та хуруугаа 3 удаа уншуулна уу.";
        }
        public void updateUserFingerPrint(string fingerprint)
        {
            stopScanner();
            Console.WriteLine(fingerprint);
            user.fingerprint0 = fingerprint;
            userModel.Set(user);
            Close();
        }
        public void stopScanner()
        {
            scannerHandler.Stop();
            if (mc.onGoingMeeting != null)
            {
                scannerHandler.InitializeDevice();
                scannerHandler.StartCaptureThread(scannerHandler.DoCaptureForMeeting);
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            stopScanner();
            mc.mainWindow.updateFingerPrintWindow = null;
            base.OnClosing(e);
            Owner = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
