using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IrtsBurtgel
{
    public class ReportExporter
    {
        MeetingController meetingController;

        public ReportExporter(MeetingController mc)
        {
            meetingController = mc;
        }

        public string ExportAttendance(List<Meeting> meetings, DateTime startDate, DateTime endDate)
        {
            if(meetings == null || meetings.Count == 0)
            {
                MessageBox.Show("Тайлан гаргах хурлаа сонгоно уу!");
                return "";
            }

            using (var package = new ExcelPackage())
            {
                // Add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Ажилтнаар");
                //Add the headers
                worksheet.Cells[1, 1].Value = "№";
                worksheet.Cells[1, 2].Value = "Нэр";
                worksheet.Cells[1, 3].Value = "Хэлтэс";
                worksheet.Cells[1, 4].Value = "Албан тушаал";

                List<ArchivedMeeting> archivedMeetings = meetingController.archivedMeetingModel.GetByFK(meetings.First().IDName, meetings.Select(x => x is ModifiedMeeting? (((ModifiedMeeting)x).meeting_id):x.id).ToArray());

                archivedMeetings.RemoveAll(x => x.meetingDatetime.Date < startDate || x.meetingDatetime.Date > endDate);

                if (archivedMeetings.Count == 0)
                {
                    throw new Exception("Таны сонгосон хурал " + startDate.ToString("yyyy/MM/dd") + "-с " + endDate.ToString("yyyy/MM/dd") + "-нд хийгдээгүй байна.");
                }

                for (int i = 0; i < archivedMeetings.Count; i++)
                {
                    worksheet.Cells[1, i + 5].Value = archivedMeetings[i].meetingDatetime.ToString("yyyy/MM/dd HH:mm") + "\n" + archivedMeetings[i].name;
                    worksheet.Cells[1, i + 5].Style.TextRotation = 90;
                }

                List<User> users= new List<User>();
                List<Attendance> attendances = meetingController.attendanceModel.GetByFK(archivedMeetings.First().IDName, archivedMeetings.Select(x => x.id).ToArray());
                if (attendances.Count > 1900)
                {
                    IEnumerable<List<Attendance>> splitedAttendances = SplitList<Attendance>(attendances, 1900);
                    foreach (List<Attendance> chunkAttendance in splitedAttendances)
                    {
                        users.AddRange(meetingController.userModel.Get(chunkAttendance.Select(x => x.userId).ToArray()));
                    }
                }
                else
                {
                    users = meetingController.userModel.Get(attendances.Select(x => x.userId).ToArray());
                }
                Dictionary<int, string> positions = meetingController.positionModel.GetAll().ToDictionary(x => x.id, x => x.name);
                Dictionary<int, string> departments = meetingController.departmentModel.GetAll().ToDictionary(x => x.id, x => x.name);

                Dictionary<int, int[,]> departmentAttendance = new Dictionary<int, int[,]>();

                if (users.Count == 0)
                {
                    throw new Exception("Таны сонгосон хуралд " + startDate.ToString("yyyy/MM/dd") + "-с " + endDate.ToString("yyyy/MM/dd") + "-нд ямар ч хүн суугаагүй байна.");
                }

                users = users.OrderBy(x => x.departmentId).ToList();

                worksheet.Cells[1, archivedMeetings.Count + 5].Value = "Нийт ирсэн";
                worksheet.Cells[1, archivedMeetings.Count + 6].Value = "Нийт хоцорсон";
                worksheet.Cells[1, archivedMeetings.Count + 7].Value = "Нийт тасалсан";
                worksheet.Cells[1, archivedMeetings.Count + 8].Value = "Ирц";


                for (int i = 0; i < users.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = i + 1;
                    worksheet.Cells[i + 2, 2].Value = users[i].fname;
                    worksheet.Cells[i + 2, 3].Value = departments.ContainsKey(users[i].departmentId) ? departments[users[i].departmentId] : "";
                    worksheet.Cells[i + 2, 4].Value = positions.ContainsKey(users[i].positionId) ? positions[users[i].positionId] : "";

                    string columnName1 = GetExcelColumnName(5);
                    string columnName2 = GetExcelColumnName(archivedMeetings.Count + 4);
                    string columnRange = columnName1 + (i + 2).ToString() + ":" + columnName2 + (i + 2).ToString();

                    worksheet.Cells[i + 2, archivedMeetings.Count + 5].Formula = "COUNTIF(" + columnRange + ", \"И\")";
                    worksheet.Cells[i + 2, archivedMeetings.Count + 6].Formula = "COUNTIF(" + columnRange + ", \"Х*\")";
                    worksheet.Cells[i + 2, archivedMeetings.Count + 7].Formula = "COUNTIF(" + columnRange + ", \"Т\")";
                    worksheet.Cells[i + 2, archivedMeetings.Count + 8].Formula = "(COUNTIF(" + columnRange + ", \"И\") + COUNTIF(" + columnRange + ", \"Х*\") + COUNTIF(" + columnRange + ", \"Ч\"))/COUNTA(" + columnRange + ")";

                    worksheet.Cells[i + 2, archivedMeetings.Count + 8].Style.Numberformat.Format = "#0%";

                    if (!departmentAttendance.ContainsKey(users[i].departmentId))
                    {
                        departmentAttendance.Add(users[i].departmentId, new int[archivedMeetings.Count, 18]);
                    }
                }

                for (int i = 0; i < archivedMeetings.Count; i++)
                {
                    for (int j = 0; j < users.Count; j++)
                    {
                        Attendance attendance = attendances.FindAll(x => x.userId == users[j].id).Find(x => x.archivedMeetingId == archivedMeetings[i].id);
                        if (attendance != null)
                        {
                            string status;
                            Color colFromHex;
                            switch (attendance.statusId)
                            {
                                case 1: status = "И"; colFromHex = ColorTranslator.FromHtml("#adebad"); break;
                                case 2: status = "Х(" + attendance.regTime.ToString() + ")"; colFromHex = ColorTranslator.FromHtml("#ffd480"); break;
                                case 14: status = "Т"; colFromHex = ColorTranslator.FromHtml("#ffcccc"); break;
                                case 15: status = "Б"; colFromHex = ColorTranslator.FromHtml("#d9d9d9"); break;
                                default: status = "Ч"; colFromHex = ColorTranslator.FromHtml("#b3d9ff"); break;
                            }
                            worksheet.Cells[j + 2, i + 5].Value = status;
                            worksheet.Cells[j + 2, i + 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[j + 2, i + 5].Style.Fill.BackgroundColor.SetColor(colFromHex);
                            departmentAttendance[users[j].departmentId][i, attendance.statusId]++;
                            departmentAttendance[users[j].departmentId][i, 16]++;
                        }
                    }

                    string columnName = GetExcelColumnName(i + 5);
                    string columnRange = columnName + "2:" + columnName + (users.Count + 1).ToString();

                    worksheet.Cells[users.Count + 2, i + 5].Formula = "COUNTIF(" + columnRange + ", \"И\")/COUNTA(" + columnRange + ")";
                    worksheet.Cells[users.Count + 3, i + 5].Formula = "COUNTIF(" + columnRange + ", \"Х*\")";
                    worksheet.Cells[users.Count + 4, i + 5].Formula = "COUNTIF(" + columnRange + ", \"Т\")";
                    
                    worksheet.Cells[users.Count + 2, i + 5].Style.Numberformat.Format = "#0%";

                }

                worksheet.Cells[users.Count + 2, 2].Value = "Нийт ирсэн";
                worksheet.Cells[users.Count + 3, 2].Value = "Нийт хоцорсон";
                worksheet.Cells[users.Count + 4, 2].Value = "Нийт тасалсан";

                //Ok now format the values;
                using (var range = worksheet.Cells[1, 1, 1, archivedMeetings.Count + 4])
                {
                    range.Style.Font.Bold = true;
                }

                worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

                // lets set the header text 
                worksheet.HeaderFooter.OddHeader.CenteredText = "&24&U&\"Arial,Regular Bold\" Inventory";
                // add the page number to the footer plus the total number of pages
                worksheet.HeaderFooter.OddFooter.RightAlignedText =
                    string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
                // add the sheet name to the footer
                worksheet.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
                // add the file path to the footer
                worksheet.HeaderFooter.OddFooter.LeftAlignedText = ExcelHeaderFooter.FilePath + ExcelHeaderFooter.FileName;
                
                if (departmentAttendance.Count > 0)
                {

                    // Add a new worksheet to the empty workbook
                    ExcelWorksheet worksheet2 = package.Workbook.Worksheets.Add("Хэлтсээр");

                    //Add the headers
                    worksheet2.Cells[1, 1].Value = "№";
                    worksheet2.Cells[1, 2].Value = "Хэлтэс";

                    for (int i = 0; i < archivedMeetings.Count; i++)
                    {
                        worksheet2.Cells[1, i + 3].Value = archivedMeetings[i].meetingDatetime.ToString("yyyy/MM/dd HH:mm") + "\n" + archivedMeetings[i].name;
                        worksheet2.Cells[1, i + 3].Style.TextRotation = 90;
                    }

                    worksheet2.Cells[1, archivedMeetings.Count + 3].Value = "Нийт ирц";

                    Dictionary<int, double> departmentAttendancePercent = new Dictionary<int, double>();

                    for (int i = 0; i < archivedMeetings.Count; i++)
                    {
                        int j = 0;
                        foreach (KeyValuePair<int, int[,]> entry in departmentAttendance)
                        {
                            worksheet2.Cells[j + 2, i + 3].Value = (entry.Value[i, 1] + entry.Value[i, 2]) + "/" + entry.Value[i, 16];
                            if (!departmentAttendancePercent.ContainsKey(entry.Key))
                            {
                                departmentAttendancePercent.Add(entry.Key, ((double)(entry.Value[i, 1] + entry.Value[i, 2]))/entry.Value[i, 16]);
                            }
                            else
                            {
                                departmentAttendancePercent[entry.Key] += ((double)(entry.Value[i, 1] + entry.Value[i, 2])) / entry.Value[i, 16];
                            }
                            j++;
                        }
                    }

                    {
                        int i = 0;
                        foreach (KeyValuePair<int, int[,]> entry in departmentAttendance)
                        {
                            worksheet2.Cells[i + 2, 1].Value = i + 1;
                            if (entry.Key != -1)
                            {
                                worksheet2.Cells[i + 2, 2].Value = departments[entry.Key];
                            }
                            else
                            {
                                worksheet2.Cells[i + 2, 2].Value = "Бусад";
                            }
                            
                            worksheet2.Cells[i + 2, archivedMeetings.Count + 3].Value = departmentAttendancePercent[entry.Key] / departmentAttendance.Count;
                            worksheet2.Cells[i + 2, archivedMeetings.Count + 3].Style.Numberformat.Format = "#0%";

                            i++;
                        }
                    }

                    //Ok now format the values;
                    using (var range = worksheet2.Cells[1, 1, 1, archivedMeetings.Count + 2])
                    {
                        range.Style.Font.Bold = true;
                    }

                    worksheet2.Cells.AutoFitColumns(0);  //Autofit columns for all cells

                    // lets set the header text 
                    worksheet2.HeaderFooter.OddHeader.CenteredText = "&24&U&\"Arial,Regular Bold\" Inventory";
                    // add the page number to the footer plus the total number of pages
                    worksheet2.HeaderFooter.OddFooter.RightAlignedText =
                        string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
                    // add the sheet name to the footer
                    worksheet2.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
                    // add the file path to the footer
                    worksheet2.HeaderFooter.OddFooter.LeftAlignedText = ExcelHeaderFooter.FilePath + ExcelHeaderFooter.FileName;

                    package.Workbook.Worksheets.MoveToStart("Хэлтсээр");

                }

                // set some document properties
                package.Workbook.Properties.Title = "Хурлын ирц";
                package.Workbook.Properties.Author = "BORTS";
                package.Workbook.Properties.Comments = "Хурлын ирцийн тайлан";

                // set some extended property values
                package.Workbook.Properties.Company = "BolorSoft LLC.";
                var xlFile = Utils.GetFileInfo(startDate.ToString("yyyyMMdd") + "_" + endDate.ToString("yyyyMMdd") + "_report.xlsx");
                // save our new workbook in the output directory and we are done!
                package.SaveAs(xlFile);
                MessageBox.Show(startDate.ToString("yyyy / MM / dd") + " - с " + endDate.ToString("yyyy / MM / dd") + " хүртэлх " + " тайлан " + xlFile.FullName + " файлд амжилттай гарлаа.", "Тайлан амжилттай гарлаа");

                System.Diagnostics.Process.Start(xlFile.FullName);
                return xlFile.FullName;
            }
        }

        public static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }

        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }
    }
}
