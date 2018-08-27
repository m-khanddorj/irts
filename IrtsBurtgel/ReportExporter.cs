using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class ReportExporter
    {
        MeetingController meetingController;

        public ReportExporter(MeetingController mc)
        {
            meetingController = mc;
        }

        public string ExportAttendance(Meeting meeting, DateTime startDate, DateTime endDate, string filename)
        {
            using (var package = new ExcelPackage())
            {
                // Add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Ирц");
                //Add the headers
                worksheet.Cells[1, 1].Value = "№";
                worksheet.Cells[1, 2].Value = "Нэр";
                worksheet.Cells[1, 3].Value = "Хэлтэс";
                worksheet.Cells[1, 4].Value = "Албан тушаал";

                List<ArchivedMeeting> archivedMeetings = meetingController.archivedMeetingModel.GetByFK(meeting.IDName, meeting.id);
                foreach (ArchivedMeeting archivedMeeting in archivedMeetings)
                {
                    if (archivedMeeting.meetingDatetime.Date < startDate || archivedMeeting.meetingDatetime.Date > endDate)
                    {
                        archivedMeetings.Remove(archivedMeeting);
                    }
                }

                for (int i = 0; i < archivedMeetings.Count; i++)
                {
                    worksheet.Cells[1, i + 5].Value = archivedMeetings[i].meetingDatetime.ToString("yyyy/MM/dd HH:mm");
                }


                List<Attendance> attendances = meetingController.attendanceModel.GetByFK(archivedMeetings.First().IDName, archivedMeetings.Select(x => x.id).ToArray());
                List<User> users = meetingController.userModel.Get(attendances.Select(x => x.userId).ToArray());
                Dictionary<int, string> positions = meetingController.positionModel.GetAll().ToDictionary(x => x.id, x => x.name);
                Dictionary<int, string> departments = meetingController.departmentModel.GetAll().ToDictionary(x => x.id, x => x.name);

                users.OrderBy(x => x.departmentId);

                worksheet.Cells[1, archivedMeetings.Count + 5].Value = "Нийт ирсэн";
                worksheet.Cells[1, archivedMeetings.Count + 6].Value = "Нийт хоцорсон";
                worksheet.Cells[1, archivedMeetings.Count + 7].Value = "Нийт тасалсан";

                for (int i = 0; i < users.Count; i ++)
                {
                    worksheet.Cells[i + 2, 1].Value = i + 1;
                    worksheet.Cells[i + 2, 2].Value = users[i].fname;
                    worksheet.Cells[i + 2, 3].Value = departments.ContainsKey(users[i].departmentId) ? departments[users[i].departmentId]:"";
                    worksheet.Cells[i + 2, 4].Value = positions.ContainsKey(users[i].positionId) ? positions[users[i].positionId] :"";

                    string columnName1 = GetExcelColumnName(4);
                    string columnName2 = GetExcelColumnName(archivedMeetings.Count + 4);
                    string columnRange = columnName1 + (i + 1).ToString() + ":" + columnName2 + (i + 1).ToString();

                    worksheet.Cells[i + 2, archivedMeetings.Count + 5].Formula = "COUNTIF(" + columnRange + ", \"И\")";
                    worksheet.Cells[i + 2, archivedMeetings.Count + 6].Formula = "COUNTIF(" + columnRange + ", \"Х*\")";
                    worksheet.Cells[i + 2, archivedMeetings.Count + 7].Formula = "COUNTIF(" + columnRange + ", \"Т\")";
                }

                for (int i = 0; i < archivedMeetings.Count; i++)
                {
                    for (int j = 0; j < users.Count; j++)
                    {
                        Attendance attendance =  attendances.FindAll(x => x.userId == users[j].id).Find(x => x.archivedMeetingId == archivedMeetings[i].id);
                        if(attendance != null)
                        {
                            string status;
                            switch (attendance.statusId)
                            {
                                case 1: status = "И"; break;
                                case 13: status = "Т"; break;
                                case 14: status = "Б"; break;
                                default: status = "Ш"; break;
                            }
                            worksheet.Cells[j + 2, i + 5].Value = status;
                        }
                    }

                    string columnName = GetExcelColumnName(i + 5);
                    string columnRange = columnName + "2:" + columnName + (users.Count + 1).ToString();

                    worksheet.Cells[users.Count + 2, i + 5].Formula = "COUNTIF(" + columnRange + ", \"И\")";
                    worksheet.Cells[users.Count + 3, i + 5].Formula = "COUNTIF(" + columnRange + ", \"Х*\")";
                    worksheet.Cells[users.Count + 4, i + 5].Formula = "COUNTIF(" + columnRange + ", \"Т\")";
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

                worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:2"];
                worksheet.PrinterSettings.RepeatColumns = worksheet.Cells["A:G"];

                // Change the sheet view to show it in page layout mode
                worksheet.View.PageLayoutView = true;

                // set some document properties
                package.Workbook.Properties.Title = "Хурлын ирц";
                package.Workbook.Properties.Author = "BORTS";
                package.Workbook.Properties.Comments = "Хурлын ирцийн тайлан";

                // set some extended property values
                package.Workbook.Properties.Company = "BolorSoft LLC.";

                var xlFile = Utils.GetFileInfo(filename + ".xlsx");
                // save our new workbook in the output directory and we are done!
                package.SaveAs(xlFile);
                return xlFile.FullName;
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
