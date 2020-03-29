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
        int scanThreadID;
        byte[] FPBuffer;
        const int REGISTER_FINGER_COUNT = 3;

        byte[][] RegTmps = new byte[3][];
        byte[] RegTmp = new byte[2048];
        byte[] CapTmp = new byte[2048];
        public byte[] last;

        int cbCapTmp = 2048;
        int cbRegTmp = 0;

        private int mfpWidth = 0;
        private int mfpHeight = 0;
        private int mfpDpi = 0;

        public Thread captureThread;
        public MeetingController meetingController;

        const int MESSAGE_CAPTURED_OK = 0x0400 + 6;

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        public ScannerHandler(MeetingController mc)
        {
            this.meetingController = mc;
        }

        public bool InitializeDevice()
        {
            for (int i = 0; i< 3; i++)
            {
                RegTmps[i] = new byte[2048];
            }
            int ret = zkfperrdef.ZKFP_ERR_OK;
            try
            {
                if ((ret = zkfp2.Init()) == zkfperrdef.ZKFP_ERR_OK)
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
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool StartCaptureThread(Action captureFunction)
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

            bIsTimeToDie = false;
            captureThread = new Thread(new ThreadStart(captureFunction));
            captureThread.IsBackground = true;
            captureThread.Start();
            scanThreadID = captureThread.ManagedThreadId;
            Console.WriteLine("Open succ");

            return true;
        }

        public void Stop()
        {
            bIsTimeToDie = true;
            scanThreadID = 0;
            if (captureThread != null && captureThread.IsAlive)
            {
                Thread.Sleep(1000);
            }
            zkfp2.CloseDevice(mDevHandle);
            zkfp2.Terminate();
            Thread.Sleep(500);
        }

        public void DoCaptureForMeeting()
        {
            while (!bIsTimeToDie && scanThreadID == Thread.CurrentThread.ManagedThreadId)
            {
                cbCapTmp = 2048;
                int ret = zkfp2.AcquireFingerprint(mDevHandle, FPBuffer, CapTmp, ref cbCapTmp);
                if (ret == zkfp.ZKFP_ERR_OK)
                {
                    Console.Beep();
                    if (!ReadFPCaptureForMeeting(MESSAGE_CAPTURED_OK))
                    {
                        Console.Beep();
                    }
                }
                Console.WriteLine("Capturing for meeting");
                Thread.Sleep(200);
            }
            Console.WriteLine("Stopping capture thread for meeting.");
        }

        private bool ReadFPCaptureForMeeting(int msg)
        {
            bool result = true;
            if (msg == MESSAGE_CAPTURED_OK)
            {

                Object[] identifiedAttendance = null;
                int maxRet = -100;

                byte[] blob1 = CapTmp;

                String strShow = zkfp2.BlobToBase64(blob1, cbCapTmp);
                Console.WriteLine("capture template data:" + strShow + "\n");

                foreach (Object[] userAttendance in meetingController.onGoingMeetingUserAttendance)
                {
                    User user = (User)userAttendance[0];
                    byte[] blob3 = Convert.FromBase64String(user.fingerprint0.Trim());

                    //0x0f309420
                    int ret = zkfp2.DBMatch(mDBHandle, blob1, blob3);

                    if (ret > 50)
                    {
                        maxRet = ret;
                        identifiedAttendance = userAttendance;
                        Console.WriteLine("user =" + user.pin);
                        Console.WriteLine("ret =" + ret);
                    }

                    if (user.fingerprint1 != "")
                    {
                        blob3 = Convert.FromBase64String(user.fingerprint1);
                        ret = zkfp2.DBMatch(mDBHandle, blob1, blob3);

                        if (ret > 50)
                        {
                            maxRet = ret;
                            identifiedAttendance = userAttendance;
                        }
                    }
                }

                if (identifiedAttendance == null)
                {
                    return false;
                }

                if (((Attendance)identifiedAttendance[1]).statusId == 15)
                {
                    DateTime now = DateTime.Now;
                    if (now > meetingController.onGoingArchivedMeeting.meetingDatetime && now.Minute != meetingController.onGoingArchivedMeeting.meetingDatetime.Minute)
                    {
                        ((Attendance)identifiedAttendance[1]).statusId = 2;
                        ((Attendance)identifiedAttendance[1]).regTime = (int)Math.Floor((now - meetingController.onGoingArchivedMeeting.meetingDatetime).TotalMinutes);
                    }
                    else
                    {
                        ((Attendance)identifiedAttendance[1]).statusId = 1;
                        ((Attendance)identifiedAttendance[1]).regTime = -1;
                    }
                    meetingController.attendanceModel.Set(((Attendance)identifiedAttendance[1]));

                    if (meetingController.mainWindow != null)
                    {
                        meetingController.mainWindow.Dispatcher.Invoke(() =>
                        {
                            foreach (MeetingStatus ms in meetingController.mainWindow.meetingStatusWindows)
                            {
                                if (ms.IsLoaded)
                                {
                                    ms.Update(identifiedAttendance);
                                }
                            }
                        });
                    }
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        public void DoCaptureForMember()
        {
            while (!bIsTimeToDie && scanThreadID == Thread.CurrentThread.ManagedThreadId)
            {
                cbCapTmp = 2048;
                int ret = zkfp2.AcquireFingerprint(mDevHandle, FPBuffer, CapTmp, ref cbCapTmp);
                if (ret == zkfp.ZKFP_ERR_OK)
                {
                    Console.Beep();
                    if (!ReadFPCaptureForMember(MESSAGE_CAPTURED_OK))
                    {
                        Console.Beep();
                    }
                }
                Console.WriteLine("Capturing for member");
                Thread.Sleep(200);
            }
            Console.WriteLine("Stopping capture for member");
        }

        private bool ReadFPCaptureForMember(int msg)
        {
            bool result = true;
            if (msg == MESSAGE_CAPTURED_OK)
            {
                var window = meetingController.mainWindow.updateFingerPrintWindow;
                byte[] blob1 = CapTmp;

                String showFP = zkfp2.BlobToBase64(blob1, cbCapTmp);
                Console.WriteLine("capture template data:" + showFP + "\n");

                if (window.RegisterCount > 0 && zkfp2.DBMatch(mDBHandle, CapTmp, RegTmps[window.RegisterCount - 1]) <= 0)
                {
                    window.Dispatcher.Invoke(() =>
                    {
                        Xceed.Wpf.Toolkit.MessageBox.Show("Та нэг л хуруугаа 3 удаа уншуулна уу!");
                    });
                    return false;
                }

                Array.Copy(CapTmp, RegTmps[window.RegisterCount], cbCapTmp);

                if (window.RegisterCount+1 >= REGISTER_FINGER_COUNT)
                {
                    int ret = zkfp.ZKFP_ERR_OK;

                    window.RegisterCount = 0;
                    if (zkfp.ZKFP_ERR_OK == (ret = zkfp2.DBMerge(mDBHandle, RegTmps[0], RegTmps[1], RegTmps[2], RegTmp, ref cbRegTmp)))
                    {
                        String base64fp = zkfp2.BlobToBase64(RegTmp, cbCapTmp);
                        Console.WriteLine("merged template data:" + base64fp + "\n");
                        window.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            window.updateUserFingerPrint(base64fp);
                        }));
                        return true;
                    }
                    else
                    {
                        window.Dispatcher.Invoke(() =>
                        {
                            Xceed.Wpf.Toolkit.MessageBox.Show("Бүртгэхэд алдаа гарлаа. Та түр хүлээгээд ахин оролдож үзнэ үү. ");
                        });
                        return false;
                    }
                }

                window.Dispatcher.Invoke(() =>
                {
                    window.RegisterCount++;
                    var v = "Та хуруугаа " + (REGISTER_FINGER_COUNT - window.RegisterCount).ToString() + " удаа уншуулна уу!";
                    MemoryStream ms = new MemoryStream();
                    BitmapFormat.GetBitmap(FPBuffer, mfpWidth, mfpHeight, ref ms);
                    BitmapImage ObjBitmapImage = new BitmapImage();
                    ObjBitmapImage.BeginInit();
                    ObjBitmapImage.StreamSource = ms;
                    ObjBitmapImage.EndInit();
                    window.image.Source = ObjBitmapImage;
                    window.InfoLabel.Content = v;
                });
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
    }
}
