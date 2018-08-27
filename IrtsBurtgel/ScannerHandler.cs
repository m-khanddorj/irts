using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using libzkfpcsharp;
using Sample;
using System.Windows.Media.Imaging;
using ExcelDataReader;
using System.Collections.Generic;

namespace IrtsBurtgel
{
    public class ScannerHandler
    {
        IntPtr mDevHandle = IntPtr.Zero;
        IntPtr mDBHandle = IntPtr.Zero;
        IntPtr FormHandle = IntPtr.Zero;
        bool bIsTimeToDie = false;
        byte[] FPBuffer;
        const int REGISTER_FINGER_COUNT = 3;

        byte[][] RegTmps = new byte[3][];
        byte[] RegTmp = new byte[2048];
        byte[] CapTmp = new byte[2048];
        public byte[] last;

        int cbCapTmp = 2048;

        private int mfpWidth = 0;
        private int mfpHeight = 0;
        private int mfpDpi = 0;
        
        private MeetingController meetingController;

        const int MESSAGE_CAPTURED_OK = 0x0400 + 6;

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        public ScannerHandler(MeetingController mc)
        {
            this.meetingController = mc;
        }

        public bool InitializeDevice()
        {
            int ret = zkfperrdef.ZKFP_ERR_OK;
            if ( (ret = zkfp2.Init()) == zkfperrdef.ZKFP_ERR_OK)
            {
                int nCount = zkfp2.GetDeviceCount() - 1;
                if (nCount > 0)
                {
                    return true;
                }
                else
                {
                    zkfp2.Terminate();
                    Console.WriteLine("No device connected!");
                    return false;
                }
            }
            else
            {
                if (ret == zkfperrdef.ZKFP_ERR_ALREADY_INIT)
                {
                    return true;
                }
                Console.WriteLine("Initialize fail, ret=" + ret + " !");
                return false;
            }
        }

        public bool StartCaptureThread()
        {
            int ret = zkfp.ZKFP_ERR_OK;
            if (IntPtr.Zero == (mDevHandle = zkfp2.OpenDevice(0)))
            {
                Console.WriteLine("OpenDevice fail");
                return false;
            }
            if (IntPtr.Zero == (mDBHandle = zkfp2.DBInit()))
            {
                MessageBox.Show("Init DB fail");
                zkfp2.CloseDevice(mDevHandle);
                mDevHandle = IntPtr.Zero;
                return false;
            }
            // 0x00007ffbbba33028
            byte[] paramValue = new byte[4];
            int size = 4;
            zkfp2.GetParameters(mDevHandle, 1, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpWidth);

            size = 4;
            zkfp2.GetParameters(mDevHandle, 2, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpHeight);

            FPBuffer = new byte[mfpWidth * mfpHeight];

            size = 4;
            zkfp2.GetParameters(mDevHandle, 3, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpDpi);

            Console.WriteLine("reader parameter, image width:" + mfpWidth + ", height:" + mfpHeight + ", dpi:" + mfpDpi);

            Thread captureThread = new Thread(new ThreadStart(DoCapture));
            captureThread.IsBackground = true;
            captureThread.Start();
            bIsTimeToDie = false;
            Console.WriteLine("Open succ");

            return true;
        }

        private void DoCapture()
        {
            while (!bIsTimeToDie)
            {
                cbCapTmp = 2048;
                int ret = zkfp2.AcquireFingerprint(mDevHandle, FPBuffer, CapTmp, ref cbCapTmp);
                if (ret == zkfp.ZKFP_ERR_OK)
                {
                    Console.Beep();
                    if (ReadFPCapture(MESSAGE_CAPTURED_OK))
                    {
                        Console.Beep();
                    }
                }
                Thread.Sleep(200);
            }
        }

        public void Stop()
        {
            bIsTimeToDie = true;
            Thread.Sleep(1000);
            zkfp2.CloseDevice(mDevHandle);
        }

        private bool ReadFPCapture(int msg)
        {
            bool result = true;
            if (msg == MESSAGE_CAPTURED_OK)
            {
                String strShow = zkfp2.BlobToBase64(CapTmp, cbCapTmp);
                Console.WriteLine("capture template data:" + strShow + "\n");

                Object[] identifiedAttendance = null;
                int maxRet = -100;

                byte[] blob1 = CapTmp;

                foreach (Object[] userAttendance in meetingController.onGoingMeetingUserAttendance)
                {
                    User user = (User) userAttendance[0];
                    byte[] blob3 = Convert.FromBase64String(user.fingerprint0.Trim());

                    //0x0f309420
                    int ret = zkfp2.DBMatch(mDBHandle, blob1, blob3);

                    if (ret > 80)
                    {
                        maxRet = ret;
                        identifiedAttendance = userAttendance;
                        break;
                    }

                    if (user.fingerprint1 != "")
                    {
                        blob3 = Convert.FromBase64String(user.fingerprint1);
                        ret = zkfp2.DBMatch(mDBHandle, blob1, blob3);

                        if (ret > 80)
                        {
                            maxRet = ret;
                            identifiedAttendance = userAttendance;
                            break;
                        }
                    }
                }

                if(identifiedAttendance == null)
                {
                    return false;
                }

                ((Attendance)identifiedAttendance[1]).statusId = 1;
                ((Attendance)identifiedAttendance[1]).regTime = DateTime.Now;
                meetingController.attendanceModel.Set(((Attendance)identifiedAttendance[1]));

            }
            else
            {
                result = false;
            }
            return result;
        }
    }
}
