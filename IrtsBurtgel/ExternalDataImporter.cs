using ExcelDataReader;
using libzkfpcsharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace IrtsBurtgel
{
    class ExternalDataImporter
    {
        private Model<User> userModel;
        private Model<Department> departmentModel;
        private Model<Position> positionModel;
        private MeetingController meetingController;

        public ExternalDataImporter(MeetingController mc)
        {
            userModel = new Model<User>();
            departmentModel = new Model<Department>(true);
            positionModel = new Model<Position>(true);
            meetingController = mc;
        }

        public bool ImportUserData(string excelpath, string datpath, List<string> imagepath)
        {
            bool result;
            try
            {
                ImportImagesOfUserFromFolder(imagepath);
                result = ImportFromExcel(excelpath, ImportDat(datpath));
                return result;
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    MessageBox.Show(excelpath + " файл олдсонгүй.");
                }
                else if (ex is ArgumentNullException)
                {
                    MessageBox.Show("Файл сонгогдоогүй байна.");
                }
                else
                {
                    MessageBox.Show(ex.ToString());
                }
                return false;
            }
        }


        // To import department, position and user excel file must be formatted by following rule.
        // First row of all sheets are assumed to be header so it will be ignored.
        // First sheet must contain department id and department name pairs in first two column.
        // Second sheet must contain position id and position name pairs in first two column.
        // Third sheet must contain user data. User id -> first row 
        //                                     User name -> second row
        //                                     User department id -> third row
        //                                     User position id -> fourth row
        public bool ImportFromExcel(string filename, Dictionary<int, Dictionary<int, string>> fingerprints)
        {
            bool result = true;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);


            using (var stream = File.Open(filename, FileMode.Open, FileAccess.Read))
            {

                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    reader.Read(); // Ignoring header
                    ImportDepartmentFromReader(reader);
                    reader.NextResult(); // Next sheet
                    reader.Read(); //Ignoring header
                    ImportPositionFromReader(reader);
                    reader.NextResult(); // Next sheet
                    reader.Read(); // Ignoring header
                    ImportUserFromReader(reader, fingerprints);
                }
            }

            return result;
        }

        //Reader should be selected department sheet
        private bool ImportPositionFromReader(IExcelDataReader reader)
        {
            bool result = true;

            List<Position> positions = positionModel.GetAll();

            positionModel.MarkAllAsDeleted();

            while (reader.Read())
            {
                int posid = GetInt(reader, 0);
                if (posid == -1)
                {
                    continue;
                }
                string posname = (string)reader.GetString(1);
                Position position = positions.Find(x => x.id == posid);

                if (position == null)
                {
                    result = result && -1 != positionModel.Add(new Position
                    {
                        id = posid,
                        name = posname,
                        isDeleted = false
                    });
                }
                else
                {
                    result = result && positionModel.Set(new Position
                    {
                        id = posid,
                        name = posname,
                        isDeleted = false
                    });
                }
            }

            return result;
        }

        //Reader should be selected department sheet
        private bool ImportDepartmentFromReader(IExcelDataReader reader)
        {
            bool result = true;

            List<Department> departments = departmentModel.GetAll();

            departmentModel.MarkAllAsDeleted();

            while (reader.Read())
            {
                int depid = GetInt(reader, 0);
                if (depid == -1)
                {
                    continue;
                }
                string depname = (string)reader.GetString(1);
                Department dep = departments.Find(x => x.id == depid);

                if (dep == null)
                {
                    result = result && -1 != departmentModel.Add(new Department
                    {
                        id = depid,
                        name = depname,
                        isDeleted = false
                    });
                }
                else
                {
                    result = result && departmentModel.Set(new Department
                    {
                        id = depid,
                        name = depname,
                        isDeleted = false
                    });
                }
            }

            return result;
        }

        public void ImportImagesOfUserFromFolder(List<string> paths)
        {
            try
            {
                UnloadUserImages();
                string targetDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\userimages";
                Directory.CreateDirectory(targetDir);
                Console.WriteLine("Copying user images into " + targetDir);
                foreach (string file in paths)
                {
                    Console.WriteLine("Copying image from " + file + " to " + Path.Combine(targetDir, Path.GetFileName(file)));
                    File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)), true);
                }
                LoadUserImages();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Гишүүдийн зураг хуулах явцад алдаа гарлаа. Алдааны мессеж: " + ex.Message);
            }
        }

        public void UnloadUserImages()
        {
            foreach (object[] userImage in meetingController.userImagePool)
            {
                Image imageControl = (Image)userImage[0];
                BitmapImage image = (BitmapImage)userImage[1];
                if (imageControl != null && imageControl.IsEnabled)
                {
                    imageControl.Source = null;
                    image.Freeze();
                    userImage[1] = null;
                }
            }
        }

        public void LoadUserImages()
        {
            foreach (object[] userImage in meetingController.userImagePool)
            {
                Image imageControl = (Image)userImage[0];
                BitmapImage image = (BitmapImage)userImage[1];
                User user = (User)userImage[2];
                BitmapImage webImage;
                try
                {
                    webImage = new BitmapImage(new Uri(meetingController.GetUserImage(user)));
                }
                catch (Exception ex)
                {
                    webImage = new BitmapImage(new Uri("./images/user.png", UriKind.Relative));
                }
                if(imageControl != null && imageControl.IsEnabled)
                {
                    imageControl.Source = webImage;
                    userImage[1] = webImage;
                }
            }
        }

        //Reader should be selected user sheet
        private bool ImportUserFromReader(IExcelDataReader reader, Dictionary<int, Dictionary<int, string>> fingerprints)
        {
            List<Department> departments = departmentModel.GetAll();
            List<Position> positions = positionModel.GetAll();
            List<User> users = userModel.GetAll();
            List<User> tempUsers = new List<User>();

            bool result = true;

            userModel.MarkAllAsDeleted();

            while (reader.Read())
            {
                int pinnum = GetInt(reader, 0);
                if (pinnum == -1)
                {
                    continue;
                }
                Dictionary<int, string> fingerprint;
                
                string fingerprint0 = "";
                string fingerprint1 = "";
                if (fingerprints.ContainsKey(pinnum))
                {
                    fingerprint = fingerprints[pinnum];
                    fingerprint0 = fingerprint.ContainsKey(0) ? fingerprint[0] : "";
                    fingerprint1 = fingerprint.ContainsKey(1) ? fingerprint[1] : "";
                }

                int depid = GetInt(reader, 8);
                int posid = GetInt(reader, 2);

                posid = posid == -1 ? 0 : posid;

                if (depid != -1 && departments.Find(x => x.id == depid) == null)
                {
                    throw new Exception("Мэдээлэл шинэчилэлт амжилтгүй боллоо. Хэлтсийн жагсаалтанд байхгүй дугаар гишүүдийн мэдээлэлд байна.");
                }

                if (positions.Find(x => x.id == posid) == null)
                {
                    throw new Exception("Мэдээлэл шинэчилэлт амжилтгүй боллоо. Албан тушаалын жагсаалтанд байхгүй дугаар гишүүдийн мэдээлэлд байна.");
                }
                User u = users.Find(x => x.pin == pinnum);

                if (u != null)
                {
                    result = result && userModel.Set(new User
                    {
                        id = u.id,
                        pin = pinnum,
                        fname = reader.GetString(3) ?? "",
                        lname = "",
                        fingerprint0 = fingerprint0,
                        fingerprint1 = fingerprint1,
                        departmentId = depid,
                        positionId = posid,
                        isDeleted = false
                    });
                }
                else
                {
                    result = result && 0 <= userModel.Add(new User
                    {
                        pin = pinnum,
                        fname = reader.GetString(3) ?? "",
                        lname = "",
                        fingerprint0 = fingerprint0,
                        fingerprint1 = fingerprint1,
                        departmentId = depid,
                        positionId = posid,
                        isDeleted = false
                    });
                }
            }
            return result;
        }

        private int GetInt(IExcelDataReader reader, int index)
        {
            if (typeof(string) == reader.GetFieldType(index))
            {
                string str = reader.GetString(index);
                bool isNumeric = int.TryParse(str, out int pin);
                if (!isNumeric && str != null && str != "")
                {
                    throw new Exception("Excel файлын тоо агуулах ёстой баганад өөр төрлийн мэдээлэл орсон байна. Мөрийн дугаар: " + index.ToString());
                }
                else if (str == null)
                {
                    return -1;
                }

                return pin;
            }
            else if (typeof(double) == reader.GetFieldType(index))
            {
                return (int)reader.GetDouble(index);
            }
            else if (reader.GetFieldType(index) == null)
            {
                return -1;
            }
            else
            {
                throw new Exception("Excel файлын тоо агуулах ёстой баганад өөр төрлийн мэдээлэл орсон байна. Мөрийн дугаар: " + index.ToString());
            }
        }

        public Dictionary<int, Dictionary<int, string>> ImportDat(string filename)
        {
            UDisk udisk = new UDisk();

            byte[] byDataBuf = null;
            int iLength;
            int iStartIndex;

            int iSize = 0;
            int iPIN = 0;
            int fid = 0;
            int iValid = 0;
            string sTemplate = "";

            Dictionary<int, Dictionary<int, string>> dict = new Dictionary<int, Dictionary<int, string>>();

            if (filename != null)
            {
                FileStream stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Read);
                byDataBuf = File.ReadAllBytes(filename);

                iLength = Convert.ToInt32(stream.Length);

                iStartIndex = 0;
                for (int i = 0; i < iLength; i++)
                {
                    iSize = byDataBuf[i] + byDataBuf[i + 1] * 256;//the variable length of the 10.0 arithmetic template
                    byte[] byTmpInfo = new byte[iSize];

                    Array.Copy(byDataBuf, iStartIndex, byTmpInfo, 0, iSize);

                    iStartIndex += iSize;
                    i = iStartIndex - 1;

                    udisk.GetTmp10FromFp10(byTmpInfo, iSize, out iPIN, out fid, out iValid, out sTemplate);
                    if (!dict.ContainsKey(iPIN))
                    {
                        Dictionary<int, string> fps = new Dictionary<int, string>();
                        fps.Add(fid, sTemplate);
                        dict.Add(iPIN, fps);
                    }
                    else
                    {
                        dict[iPIN].Add(fid, sTemplate);
                    }

                    byTmpInfo = null;
                }
                stream.Close();
            }
            else
            {
                return dict;
            }
            return dict;
        }
    }
}
