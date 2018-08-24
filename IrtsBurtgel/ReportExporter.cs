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

        public string ExportAttendance(Meeting meeting, DateTime startDate, DateTime endDate)
        {
            using (var package = new ExcelPackage())
            {
                // Add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Ирц");
                //Add the headers
                worksheet.Cells[1, 1].Value = "№";
                worksheet.Cells[1, 2].Value = "Нэр";
                worksheet.Cells[1, 3].Value = "Хэлтэс";

                List<MeetingAndUser> mus = meetingController.muModel.GetByFK(meeting.IDName, meeting.id);
                List<User> users = meetingController.userModel.Get(mus.Select(x => x.userId).ToArray());

                List<ArchivedMeeting> archivedMeetings = meetingController.archivedMeetingModel.GetByFK(meeting.IDName, meeting.id);
                foreach (ArchivedMeeting archivedMeeting in archivedMeetings)
                {
                    if(archivedMeeting.meetingDatetime.Date < startDate || archivedMeeting.meetingDatetime.Date > endDate)
                    {
                        archivedMeetings.Remove(archivedMeeting);
                    }
                }

                for (int i = 0; i < archivedMeetings.Count; i ++)
                {
                    worksheet.Cells[1, i + 4].Value = archivedMeetings[i].meetingDatetime.ToString("yyyyMMdd");
                }

                //Add some items...
                worksheet.Cells["A2"].Value = 12001;
                worksheet.Cells["B2"].Value = "Nails";
                worksheet.Cells["C2"].Value = 37;
                worksheet.Cells["D2"].Value = 3.99;

                worksheet.Cells["A3"].Value = 12002;
                worksheet.Cells["B3"].Value = "Hammer";
                worksheet.Cells["C3"].Value = 5;
                worksheet.Cells["D3"].Value = 12.10;

                worksheet.Cells["A4"].Value = 12003;
                worksheet.Cells["B4"].Value = "Saw";
                worksheet.Cells["C4"].Value = 12;
                worksheet.Cells["D4"].Value = 15.37;

                //Add a formula for the value-column
                worksheet.Cells["E2:E4"].Formula = "C2*D2";

                //Ok now format the values;
                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                    range.Style.Font.Color.SetColor(Color.White);
                }

                worksheet.Cells["A5:E5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A5:E5"].Style.Font.Bold = true;

                worksheet.Cells[5, 3, 5, 5].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(2, 3, 4, 3).Address);
                worksheet.Cells["C2:C5"].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["D2:E5"].Style.Numberformat.Format = "#,##0.00";

                //Create an autofilter for the range
                worksheet.Cells["A1:E4"].AutoFilter = true;

                worksheet.Cells["A2:A4"].Style.Numberformat.Format = "@";   //Format as text

                //There is actually no need to calculate, Excel will do it for you, but in some cases it might be useful. 
                //For example if you link to this workbook from another workbook or you will open the workbook in a program that hasn't a calculation engine or 
                //you want to use the result of a formula in your program.
                worksheet.Calculate();

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

                var xlFile = Utils.GetFileInfo("sample1.xlsx");
                // save our new workbook in the output directory and we are done!
                package.SaveAs(xlFile);
                return xlFile.FullName;
            }
        }
    }
}
