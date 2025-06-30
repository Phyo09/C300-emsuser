using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using C300.Models;
using System.Xml.Linq;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using GemBox.Spreadsheet;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace C300.Controllers
{
    public class PrintController : Controller
    {
        private AppDbContext _dbContext;
        private IHostingEnvironment _env;
        private readonly IList<Backlog> data;
        private IHttpContextAccessor _contextAccessor;
        private HttpContext _context { get { return _contextAccessor.HttpContext; } }

        public PrintController(AppDbContext dbContext, IHostingEnvironment env, IHttpContextAccessor contextAccessor)
        {
            _env = env;
            _dbContext = dbContext;
            _contextAccessor = contextAccessor;
        }

        public string userEmail
        {
            get
            {

                var userEmail = "SystemGenerated - Guest";
                if (_context != null)
                {
                    if (_context.User != null)
                    {
                        var identity = _context.User.Identity;
                        if (identity != null && identity.IsAuthenticated)
                        {
                            userEmail = identity.Name;
                        }
                    }
                }
                return userEmail;
            }
        }

        private void BacklogActivity(string action)
        {
            if (User.Identity.IsAuthenticated)
            {
                var backlog = new Backlog()
                {
                    Name = userEmail,
                    Action = action,
                    Datetime = DateTime.Now,
                    UserId = Convert.ToInt32(User.FindFirst(ClaimTypes.Sid).Value)
                };

                _dbContext.Backlog.Add(backlog);

                _dbContext.SaveChanges();
            }
            else
            {
                var backlog = new Backlog()
                {
                    Name = "SystemGenerated - Guest",
                    Action = action,
                    Datetime = DateTime.Now,
                    UserId = 0
                };

                _dbContext.Backlog.Add(backlog);

                _dbContext.SaveChanges();
            }
        }

        public IActionResult Export()
        {
            BacklogActivity("Accessed Print Page");
            PreparePreference();
            return View();
        }
        private static GemBox.Spreadsheet.SaveOptions GetSaveOptions(string format)
        {
            switch (format.ToUpperInvariant())
            {
                case "XLSX":
                    return GemBox.Spreadsheet.SaveOptions.XlsxDefault;
                case "XLS":
                    return GemBox.Spreadsheet.SaveOptions.XlsDefault;
                case "ODS":
                    return GemBox.Spreadsheet.SaveOptions.OdsDefault;
                case "CSV":
                    return GemBox.Spreadsheet.SaveOptions.CsvDefault;
                case "PDF":
                    return GemBox.Spreadsheet.SaveOptions.PdfDefault;
                default:
                    throw new NotSupportedException("Format '" + format + "' is not supported.");
            }
        }
        //private static GemBox.Spreadsheet.SaveOptions GetSaveOptions(int format)
        //{
        //    switch (format)
        //    {
        //        case 1:
        //            return GemBox.Spreadsheet.SaveOptions.XlsxDefault;
        //        case 2:
        //            return GemBox.Spreadsheet.SaveOptions.XlsDefault;
        //        case 3:
        //            return GemBox.Spreadsheet.SaveOptions.OdsDefault;
        //        case 4:
        //            return GemBox.Spreadsheet.SaveOptions.CsvDefault;
        //        case 5:
        //            return GemBox.Spreadsheet.SaveOptions.PdfDefault;
        //        default:
        //            throw new NotSupportedException("Format '" + format + "' is not supported.");
        //    }
        //}

        private static byte[] GetBytes(ExcelFile file, GemBox.Spreadsheet.SaveOptions options)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                file.Save(stream, options);
                return stream.ToArray();
            }
        }
        public class BacklogModel
        {
            public string SelectedFormat { get; set; }
            public IList<Backlog> Items { get; set; }
        }

        public IActionResult Create()
        {
            PreparePreference();
            return View(new BacklogModel() { Items = data, SelectedFormat = "XLSX" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BacklogModel format)
        {
            PreparePreference();
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            SpreadsheetInfo.FreeLimitReached += (sender, e) => e.FreeLimitReachedAction = FreeLimitReachedAction.ContinueAsTrial;


            if (!ModelState.IsValid)
                return View(format);

            GemBox.Spreadsheet.SaveOptions options = GetSaveOptions(format.SelectedFormat);
            ExcelFile book = new ExcelFile();
            ExcelWorksheet sheet = book.Worksheets.Add("Sheet1");

            CellStyle style = sheet.Rows[0].Style;
            style.Font.Weight = ExcelFont.BoldWeight;
            style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            sheet.Columns[0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

            sheet.Columns[0].SetWidth(100, LengthUnit.Pixel);
            sheet.Columns[1].SetWidth(50, LengthUnit.Pixel);
            sheet.Columns[2].SetWidth(100, LengthUnit.Pixel);
            sheet.Columns[3].SetWidth(200, LengthUnit.Pixel);
            sheet.Columns[4].SetWidth(200, LengthUnit.Pixel);


            sheet.Cells["A1"].Value = "User Id";
            sheet.Cells["B1"].Value = "Name";
            sheet.Cells["C1"].Value = "Backlog Id";
            sheet.Cells["D1"].Value = "DateTime";
            sheet.Cells["E1"].Value = "Action";

            DbSet<Backlog> dbs = _dbContext.Backlog;

            for (int i = 1; i < dbs.Count(); i++)
            {
                sheet.Cells[i, 0].Value = dbs.Select(b => b.UserId).ToList()[i];
                sheet.Cells[i, 1].Value = dbs.Select(b => b.Name).ToList()[i];
                sheet.Cells[i, 2].Value = dbs.Select(b => b.BacklogId).ToList()[i];
                sheet.Cells[i, 3].Value = dbs.Select(b => b.Datetime).ToList()[i];
                sheet.Cells[i, 4].Value = dbs.Select(b => b.Action).ToList()[i];
            }

            return File(GetBytes(book, options), options.ContentType, "Backlog " + DateTime.Now + "." + format.SelectedFormat.ToLowerInvariant());

        }

        [HttpGet("Print/Test/{format}/{id}")]
        public IActionResult Test(string format,int id)
        {
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            SpreadsheetInfo.FreeLimitReached += (sender, e) => e.FreeLimitReachedAction = FreeLimitReachedAction.ContinueAsTrial;
            GemBox.Spreadsheet.SaveOptions options = GetSaveOptions(format);
            ExcelFile book = new ExcelFile();
            ExcelWorksheet sheet = book.Worksheets.Add("Sheet1");

            CellStyle style = sheet.Rows[0].Style;
            style.Font.Weight = ExcelFont.BoldWeight;
            style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            if (id >=1 && id <= 4)
            {
                sheet.Columns[0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                sheet.Columns[1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                sheet.Columns[2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                sheet.Columns[0].SetWidth(100, LengthUnit.Pixel);
                sheet.Columns[1].SetWidth(100, LengthUnit.Pixel);
                sheet.Columns[2].SetWidth(200, LengthUnit.Pixel);


                sheet.Cells["A1"].Value = "ID";
                sheet.Cells["B1"].Value = "Level";
                sheet.Cells["C1"].Value = "Datetime";
            }
            else if (id == 5)
            {
                //if (User.IsInRole("Admin"))
                //{
                    sheet.Columns[0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    sheet.Columns[1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    sheet.Columns[2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    sheet.Columns[3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    sheet.Columns[4].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    sheet.Columns[5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                    sheet.Columns[0].SetWidth(50, LengthUnit.Pixel);
                    sheet.Columns[1].SetWidth(100, LengthUnit.Pixel);
                    sheet.Columns[2].SetWidth(150, LengthUnit.Pixel);
                    sheet.Columns[3].SetWidth(75, LengthUnit.Pixel);
                    sheet.Columns[4].SetWidth(75, LengthUnit.Pixel);
                    sheet.Columns[5].SetWidth(50, LengthUnit.Pixel);


                    sheet.Cells["A1"].Value = "User Id";
                    sheet.Cells["B1"].Value = "Name";
                    sheet.Cells["C1"].Value = "Email";
                    sheet.Cells["D1"].Value = "Date of Birth";
                    sheet.Cells["E1"].Value = "Country";
                    sheet.Cells["F1"].Value = "Role";
                //}
                
            }
            else if (id == 6)
            {
                sheet.Columns[0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                sheet.Columns[0].SetWidth(100, LengthUnit.Pixel);
                sheet.Columns[1].SetWidth(50, LengthUnit.Pixel);
                sheet.Columns[2].SetWidth(100, LengthUnit.Pixel);
                sheet.Columns[3].SetWidth(200, LengthUnit.Pixel);
                sheet.Columns[4].SetWidth(200, LengthUnit.Pixel);


                sheet.Cells["A1"].Value = "User Id";
                sheet.Cells["B1"].Value = "Name";
                sheet.Cells["C1"].Value = "Backlog Id";
                sheet.Cells["D1"].Value = "DateTime";
                sheet.Cells["E1"].Value = "Action";
            }
            
            if (id == 1)
            {
                DbSet<Temperature> dbs = _dbContext.Temperature;
                for (int i = 1; i < dbs.Count(); i++)
                {
                    sheet.Cells[i, 0].Value = dbs.Select(b => b.TemperatureId).ToList()[i];
                    sheet.Cells[i, 1].Value = dbs.Select(b => b.TLevel).ToList()[i];
                    sheet.Cells[i, 2].Value = dbs.Select(b => b.TDatetime).ToList()[i];
                }
                return File(GetBytes(book, options), options.ContentType, "Temperature " + DateTime.Now + "." + format);

            }
            else if (id == 2)
            {
                DbSet<Humidity> dbs = _dbContext.Humidity;
                for (int i = 1; i < dbs.Count(); i++)
                {
                    sheet.Cells[i, 0].Value = dbs.Select(b => b.HumidityId).ToList()[i];
                    sheet.Cells[i, 1].Value = dbs.Select(b => b.HLevel).ToList()[i];
                    sheet.Cells[i, 2].Value = dbs.Select(b => b.HDatetime).ToList()[i];
                }
                return File(GetBytes(book, options), options.ContentType, "Humidity " + DateTime.Now + "." + format);
            }
            else if (id == 3)
            {
                DbSet<Light> dbs = _dbContext.Light;
                for (int i = 1; i < dbs.Count(); i++)
                {
                    sheet.Cells[i, 0].Value = dbs.Select(b => b.LightId).ToList()[i];
                    sheet.Cells[i, 1].Value = dbs.Select(b => b.LLevel).ToList()[i];
                    sheet.Cells[i, 2].Value = dbs.Select(b => b.LDatetime).ToList()[i];
                }
                return File(GetBytes(book, options), options.ContentType, "Light " + DateTime.Now + "." + format);
            }
            else if (id == 4)
            {
                DbSet<Weight> dbs = _dbContext.Weight;
                for (int i = 1; i < dbs.Count(); i++)
                {
                    sheet.Cells[i, 0].Value = dbs.Select(b => b.WeightId).ToList()[i];
                    sheet.Cells[i, 1].Value = dbs.Select(b => b.WLevel).ToList()[i];
                    sheet.Cells[i, 2].Value = dbs.Select(b => b.WDatetime).ToList()[i];
                }
                return File(GetBytes(book, options), options.ContentType, "Weight " + DateTime.Now + "." + format);
            }
            else if (id == 5)
            {
                DbSet<Emsuser> dbs = _dbContext.Emsuser;
                for (int i = 1; i < dbs.Count(); i++)
                {
                    sheet.Cells[i, 0].Value = dbs.Select(b => b.UserId).ToList()[i-1];
                    sheet.Cells[i, 1].Value = dbs.Select(b => b.Name).ToList()[i-1];
                    sheet.Cells[i, 2].Value = dbs.Select(b => b.Email).ToList()[i-1];
                    sheet.Cells[i, 3].Value = dbs.Select(b => String.Format("{0:dd-MMM-yyyy}", b.Dob)).ToList()[i-1];
                    sheet.Cells[i, 4].Value = dbs.Select(b => b.Country).ToList()[i-1];
                    sheet.Cells[i, 5].Value = dbs.Select(b => b.Role).ToList()[i-1];
                }
                return File(GetBytes(book, options), options.ContentType, "Account " + DateTime.Now + "." + format);
            }
            else if (id == 6)
            {
                DbSet<Backlog> dbs = _dbContext.Backlog;
                for (int i = 1; i < dbs.Count(); i++)
                {
                    sheet.Cells[i, 0].Value = dbs.Select(b => b.UserId).ToList()[i];
                    sheet.Cells[i, 1].Value = dbs.Select(b => b.Name).ToList()[i];
                    sheet.Cells[i, 2].Value = dbs.Select(b => b.BacklogId).ToList()[i];
                    sheet.Cells[i, 3].Value = dbs.Select(b => b.Datetime).ToList()[i];
                    sheet.Cells[i, 4].Value = dbs.Select(b => b.Action).ToList()[i];
                }
                return File(GetBytes(book, options), options.ContentType, "Backlog " + DateTime.Now + "." + format);
            }
            else
            {
                return BadRequest();
            }
            

        }

            private void PreparePreference()
        {
            if (User.Identity.IsAuthenticated)
            {
                DbSet<Preference> dbs1 = _dbContext.Preference;
                int userId = 0;
                Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
                Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
                ViewData["preference"] = model1;
            }
            else
            {
                ViewData["preference"] = new Preference();
            }
        }
    }
}