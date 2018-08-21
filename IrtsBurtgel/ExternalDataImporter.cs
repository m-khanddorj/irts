using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    class ExternalDataImporter
    {
        public bool ImportUserData(string excelpath, string datpath)
        {
            Model<User> model = new Model<User>();
            return model.BulkAdd(ImportFromExcel(excelpath, ImportDat(datpath)));
        }

        public List<User> ImportFromExcel(string filename, Dictionary<int, Dictionary<int, string>> fingerprints)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            List<User> tempUsers = new List<User>();
            using (var stream = File.Open(filename, FileMode.Open, FileAccess.Read))
            {

                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {

                    // Choose one of either 1 or 2:

                    // 1. Use the reader methods
                    do
                    {
                        while (reader.Read())
                        {
                            if (typeof(string) == reader.GetFieldType(0))
                            {
                                continue;
                            }
                            else
                            {
                                int pin = (int)reader.GetDouble(0);

                                Dictionary<int, string> fingerprint = fingerprints[pin];
                                string fingerprint0 = fingerprint.ContainsKey(0)? fingerprint[0]:"";
                                string fingerprint1 = fingerprint.ContainsKey(1) ? fingerprint[1] : "";

                                tempUsers.Add(new User
                                {
                                    fname = reader.GetString(1),
                                    lname = reader.GetString(2),
                                    fingerprint0 = fingerprint0,
                                    fingerprint1 = fingerprint1,
                                    department = reader.GetString(3),
                                    position = reader.GetString(4)
                                });
                            }
                        }
                    } while (reader.NextResult());
                }
            }
            return tempUsers;
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
                    if(!dict.ContainsKey(iPIN))
                    {
                        Dictionary<int, string> fps = new Dictionary<int, string>();
                        fps.Add(fid, Convert.ToBase64String(byTmpInfo));
                        dict.Add(iPIN, fps);
                    }
                    else
                    {
                        dict[iPIN].Add(fid, Convert.ToBase64String(byTmpInfo));
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
