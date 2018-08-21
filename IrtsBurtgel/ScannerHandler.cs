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

        private List<User> users;

        const int MESSAGE_CAPTURED_OK = 0x0400 + 6;

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        public ScannerHandler(List<User> users)
        {
            this.users = users;
        }

        public bool InitializeDevice()
        {
            int ret = zkfperrdef.ZKFP_ERR_OK;
            if ((ret = zkfp2.Init()) == zkfperrdef.ZKFP_ERR_OK)
            {
                int nCount = zkfp2.GetDeviceCount() - 1;
                if (nCount > 0)
                {
                    MessageBox.Show(nCount.ToString() + " device connected!");
                    return true;
                }
                else
                {
                    zkfp2.Terminate();
                    MessageBox.Show("No device connected!");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Initialize fail, ret=" + ret + " !");
                return false;
            }
        }

        public bool StartCaptureThread()
        {
            int ret = zkfp.ZKFP_ERR_OK;
            if (IntPtr.Zero == (mDevHandle = zkfp2.OpenDevice(0)))
            {
                MessageBox.Show("OpenDevice fail");
                return false;
            }
            if (IntPtr.Zero == (mDBHandle = zkfp2.DBInit()))
            {
                MessageBox.Show("Init DB fail");
                zkfp2.CloseDevice(mDevHandle);
                mDevHandle = IntPtr.Zero;
                return false;
            }

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
                    ReadFPCapture(MESSAGE_CAPTURED_OK);
                }
                Thread.Sleep(200);
            }
        }


        private bool ReadFPCapture(int msg)
        {
            bool result = true;
            if (msg == MESSAGE_CAPTURED_OK)
            {
                String strShow = zkfp2.BlobToBase64(CapTmp, cbCapTmp);
                Console.WriteLine("capture template data:" + strShow + "\n");

                User identifiedUser = null;
                int maxRet = -100;

                byte[] blob1 = CapTmp;

                foreach (User user in users)
                {
                    byte[] blob3 = Convert.FromBase64String(user.fingerprint0);
                    byte[] blob2 = new byte[2048];
                    Array.Copy(blob3, blob2, blob3.Length);

                    int ret = zkfp2.DBMatch(mDBHandle, blob1, blob2);

                    if (ret > 80)
                    {
                        maxRet = ret;
                        identifiedUser = user;
                        break;
                    }

                    if (ret > maxRet)
                    {
                        maxRet = ret;
                        identifiedUser = user;
                    }
                    if (user.fingerprint1 != "")
                    {
                        blob3 = Convert.FromBase64String(user.fingerprint1);
                        blob2 = new byte[2048];
                        ret = zkfp2.DBMatch(mDBHandle, blob1, blob2);

                        if (ret > 80)
                        {
                            maxRet = ret;
                            identifiedUser = user;
                            break;
                        }

                        if (ret > maxRet)
                        {
                            maxRet = ret;
                            identifiedUser = user;
                        }
                    }
                }

                Console.WriteLine(identifiedUser.fname);

            }
            return result;
        }
    }
}
