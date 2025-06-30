using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using C300.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using GemBox.Spreadsheet;
using System.Configuration;
using System.Data;
using System.Web;
using System.Data.Sql;
using System.Security.Claims;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;

namespace C300.Controllers
{
    public class BacklogController : Controller
    {
        private AppDbContext _dbContext;
        private IHostingEnvironment _env;
        private IHttpContextAccessor _contextAccessor;
        private readonly IList<Backlog> data;

        private HttpContext _context { get { return _contextAccessor.HttpContext; } }

        public BacklogController(AppDbContext dbContext, IHostingEnvironment environment, IHttpContextAccessor contextAccessor)
        {
            _dbContext = dbContext;
            _env = environment;
            _contextAccessor = contextAccessor;
        }

        public string browser
        {
            get
            {
                //var browsers = Request.Headers["User-Agent"].ToString();
                var browser = HttpContext.Request.Headers["User-Agent"].ToString();
                return browser;
            }
        }

        public string ip
        {
            get
            {
                var ip = Request.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                return ip.ToString();
            }
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

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {

            PreparePreference();
            DbSet<Backlog> dbs = _dbContext.Backlog;
            List<Backlog> model = dbs.OrderBy(b => b.Datetime)
                                        .ThenBy(b => b.BacklogId)
                                        .ToList();
            var backlog = new Backlog()
            {
                //Browser = Request.Headers["Browser-Type"],

                Name = userEmail,
                Action = "Accessed Backlog Page",
                Datetime = DateTime.Now,
                UserId = Convert.ToInt32(User.FindFirst(ClaimTypes.Sid).Value)
            };

            _dbContext.Backlog.Add(backlog);
            _dbContext.SaveChanges();

            return View(model);
        }
        

        private static SaveOptions GetSaveOptions(string format)
        {
            switch (format.ToUpperInvariant())
            {
                case "XLSX":
                    return SaveOptions.XlsxDefault;
                case "XLS":
                    return SaveOptions.XlsDefault;
                case "ODS":
                    return SaveOptions.OdsDefault;
                case "CSV":
                    return SaveOptions.CsvDefault;
                case "PDF":
                    return SaveOptions.PdfDefault;
                default:
                    throw new NotSupportedException("Format '" + format + "' is not supported.");
            }
        }

        private static byte[] GetBytes(ExcelFile file, SaveOptions options)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                file.Save(stream, options);
                return stream.ToArray();
            }
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
            BacklogActivity("Generated Backlog Report");
            PreparePreference();
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            // Continue to use the component in a Trial mode when free limit is reached.
            SpreadsheetInfo.FreeLimitReached += (sender, e) => e.FreeLimitReachedAction = FreeLimitReachedAction.ContinueAsTrial;


            if (!ModelState.IsValid)
                return View(format);

            SaveOptions options = GetSaveOptions(format.SelectedFormat);
            ExcelFile book = new ExcelFile();
            ExcelWorksheet sheet = book.Worksheets.Add("Sheet1");

            CellStyle style = sheet.Rows[0].Style;
            style.Font.Weight = ExcelFont.BoldWeight;
            style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            sheet.Columns[0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

            sheet.Columns[0].SetWidth(100, LengthUnit.Pixel);
            sheet.Columns[1].SetWidth(50, LengthUnit.Pixel);
            sheet.Columns[2].SetWidth(50, LengthUnit.Pixel);
            sheet.Columns[3].SetWidth(100, LengthUnit.Pixel);
            sheet.Columns[4].SetWidth(100, LengthUnit.Pixel);


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

        public class BacklogModel
        {
            public string SelectedFormat { get; set; }
            public IList<Backlog> Items { get; set; }
        }

    }
}