using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using C300.Models;
using System.Dynamic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using GemBox.Spreadsheet;
using System.IO;
using System.Data.Common;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Text;
using System.Web;
using LINQtoCSV;
using System.Net.Mail;
using C300.Models;

namespace C300.Controllers
{
    public class EmsuserController : Controller
    {

        private AppDbContext _dbContext;
        private IHostingEnvironment _env;
        private readonly IList<Emsuser> data;
        private IHttpContextAccessor _contextAccessor;
        private HttpContext _context { get { return _contextAccessor.HttpContext; } }

        public EmsuserController(AppDbContext dbContext, IHostingEnvironment environment, IHttpContextAccessor contextAccessor)
        {
            _dbContext = dbContext;
            _env = environment;
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

        public IActionResult Index()
        {
            BacklogActivity("Accessed View Users Page");
            PreparePreference();
            DbSet<Emsuser> dbs = _dbContext.Emsuser;
            List<Emsuser> model = dbs.OrderBy(p => p.UserId)
                                    .ToList();

            return View(model);

        }
        public IActionResult EmsuserInfo(int id)
        {
            PreparePreference();
            DbSet<Emsuser> dbs = _dbContext.Emsuser;
            List<Emsuser> EmsInfo = dbs.Where(o => o.UserId == id).ToList();
            return View(EmsInfo);
        }

        public IActionResult ViewUsers()
        {
            BacklogActivity("Accessed View Users Page");
            PreparePreference();
            DbSet<Emsuser> dbs = _dbContext.Emsuser;
            List<Emsuser> model = dbs.OrderBy(p => p.UserId)
                                    .ToList();
            return View(model);
        }
        [HttpGet]
        public IActionResult Deactivate(int id)
        {
            BacklogActivity("Deactivated User");
            PreparePreference();
            DbSet<Emsuser> dbs = _dbContext.Emsuser;
            Emsuser user = dbs.Where(o => o.UserId == id).FirstOrDefault();
            user.Status = "Inactive";
            user.Password = CreatePassword(8);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Activate(int id)
        {
            BacklogActivity("Activated User");
            PreparePreference();
            DbSet<Emsuser> dbs = _dbContext.Emsuser;
            Emsuser user = dbs.Where(o => o.UserId == id).FirstOrDefault();
            user.Status = "Active";
            user.Password = CreatePassword(8);
            _dbContext.SaveChanges();
            //SendVerificationLinkEmail(user.Email, "You are now Active in the system and Password is- " + user.Password); // Service ended
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            //DbSet<Emsuser> dbs = _dbContext.Emsuser;
            //var adduser =
            //    dbs.ToList<Emsuser>()
            //       .OrderBy(p => p.Name)
            //       .Select(
            //           p =>
            //           {
            //               dynamic d = new ExpandoObject();
            //               d.value = p.UserId;
            //               d.text = p.Name;
            //               return d;
            //           }
            //       )
            //       .ToList<dynamic>();
            //ViewData["userform"] = adduser;
            PreparePreference();
            return View();
        }
        [HttpPost]

        public IActionResult AddUser(Emsuser emsuser)
        {
            PreparePreference();
            if (!ModelState.IsValid)
            {
                TempData["Msg"] = "Invalid data. Please check the input.";
                return RedirectToAction("Index");
            }
            try
            {
                DbSet<Emsuser> dbs = _dbContext.Emsuser;
                emsuser.IsVerified = true;

                if (HttpContext.Request.Form.Files != null)
                {
                    var files = HttpContext.Request.Form.Files;

                    string base64string = string.Empty;
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                var fileBytes = ms.ToArray();
                                base64string = Convert.ToBase64String(fileBytes);
                            }
                        }
                    }

                    emsuser.Picture = base64string;
                    emsuser.Status = "Active";
                    dbs.Add(emsuser);
                }
                if (_dbContext.SaveChanges() > 0)
                {
                    BacklogActivity("Added New User");
                    TempData["Msg"] = "New user added!";
                    // Add default Preference for new user
                    var newPreference = new Preference
                    {
                        UserId = emsuser.UserId,
                        HighestTemp = 0,
                        TempNoti = 0,
                        HighestHumidity = 0,
                        HumNoti = 0,
                        HighestLight = 0,
                        LightNoti = 0,
                        HighestWeight = 0,
                        WeightNoti = 0
                    };

                    _dbContext.Preference.Add(newPreference);
                    _dbContext.SaveChanges();

                    BacklogActivity("Added New User with default preference");
                }

                else
                { 
                    TempData["Msg"] = "Failed to update database!";

                }
              
                return RedirectToAction("Index");
            }

            catch (Exception)
            {
                throw;
            }

        }



        [HttpGet]
        public IActionResult Update(int id)
        {
            PreparePreference();
            DbSet<Emsuser> dbs = _dbContext.Emsuser;
            Emsuser tuser = dbs.Where(p => p.UserId == id)
                                      .FirstOrDefault();

            //if (tuser == null)
            //{

            return View(tuser);
            //}
            //else
            //{
            //    TempData["Msg"] = "Ems User not found!";

            //    return RedirectToAction("Index");
            //}

        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(Emsuser emsuser,
                                  IFormFile photo)
        {

            PreparePreference();
            DbSet<Emsuser> dbs = _dbContext.Emsuser;
            Emsuser tuser = dbs.Where(p => p.UserId == emsuser.UserId)
                                      .FirstOrDefault();
            var id = Convert.ToInt32(User.FindFirst(ClaimTypes.Sid).Value);
            var name = dbs.Where(o => o.UserId == id).Select(o => o.Name).FirstOrDefault();
            if (HttpContext.Request.Form.Files != null)
            {
                var files = HttpContext.Request.Form.Files;

                string base64string = string.Empty;
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            base64string = Convert.ToBase64String(fileBytes);
                        }
                    }
                }


                if (tuser != null)
                {
                    tuser.Name = emsuser.Name;
                    tuser.Email = emsuser.Email;
                    tuser.Role = emsuser.Role;
                    tuser.UserId = emsuser.UserId;
                    tuser.Dob = emsuser.Dob;
                    tuser.Country = emsuser.Country;
                    tuser.Picture = base64string;
                    tuser.UpdatedBy = name;


                    string msg = "";
                    if (_dbContext.SaveChanges() == 1)
                        msg = "Emsuser  info updated. ";


                    TempData["Msg"] = msg;
                }
                else
                    TempData["Msg"] = "EMS user not found!";

            }

            return RedirectToAction("Index");

        }

        public string CreatePassword(int length)
        {
            PreparePreference();
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new System.Text.StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        public IActionResult Status(int id)
        {
            BacklogActivity("Accessed Status Page");
            PreparePreference();
            DbSet<Temperature> dbs = _dbContext.Temperature;
            DbSet<Humidity> dbs2 = _dbContext.Humidity;
            DbSet<Light> dbs3 = _dbContext.Light;
            DbSet<Weight> dbs4 = _dbContext.Weight;
            var max = dbs.OrderByDescending(p => p.TDatetime).FirstOrDefault();
            if (id == 1)
            {
                if (max != null)
                {
                    if ((DateTime.Now - max.TDatetime).TotalSeconds <= 31)
                    {
                        ViewData["MSG"] = "The  Temperature sensor is Active";
                    }
                    else
                    {
                        ViewData["MSG"] = "The Temperature sensor is Inactive";
                    }
                }
                else
                {
                    ViewData["MSG"] = " Temperature data is empty";
                }
            }
            else if (id == 2)
            {

                var max2 = dbs2.OrderByDescending(p => p.HDatetime).FirstOrDefault();
                if (max2 != null)
                {

                    if ((DateTime.Now - max2.HDatetime).TotalSeconds <= 31)
                    {
                        ViewData["MSG2"] = "The  Humdity sensor is Active";
                    }
                    else
                    {
                        ViewData["MSG2"] = "The Humidity  sensor is Inactive";
                    }
                }
                else
                {
                    ViewData["MSG2"] = " Humidity data is empty";
                }
            }
            else if (id == 3)
            {
                var max3 = dbs3.OrderByDescending(p => p.LDatetime).FirstOrDefault();
                if (max3 != null)
                {
                    if ((DateTime.Now - max3.LDatetime).TotalSeconds <= 31)
                    {
                        ViewData["MSG3"] = "The Light sensor is Active";
                    }
                    else
                    {
                        ViewData["MSG3"] = "The  Light sensor is Inactive";
                    }
                }
                else
                {
                    ViewData["MSG3"] = "Light data is empty";
                }
            }
            else if (id == 4)
            {
                var max4 = dbs4.OrderByDescending(p => p.WDatetime).FirstOrDefault();
                if (max4 != null)
                {
                    if ((DateTime.Now - max4.WDatetime).TotalSeconds <= 31)
                    {
                        ViewData["MSG4"] = "The Weight sensor is Active";
                    }
                    else
                    {
                        ViewData["MSG4"] = "The Weight sensor is Inactive";
                    }
                }
                else
                {
                    ViewData["MSG4"] = " Weight data is empty ";
                }
            }
            else if (id == 5)
            {
                var temp = dbs.OrderByDescending(p => p.TDatetime).FirstOrDefault();
                var max2 = dbs2.OrderByDescending(p => p.HDatetime).FirstOrDefault();
                var max3 = dbs3.OrderByDescending(p => p.LDatetime).FirstOrDefault();
                var max4 = dbs4.OrderByDescending(p => p.WDatetime).FirstOrDefault();

                if (temp != null)
                {
                    if ((DateTime.Now - temp.TDatetime).TotalSeconds <= 31)
                    {
                        ViewData["Temp"] = "The  Temperature sensor is Active";
                    }
                    else
                    {
                        ViewData["Temp"] = "The Temperature sensor is Inactive";
                    }
                }
                else
                {
                    ViewData["Temp"] = "Temperature data is empty";
                }

                if (max2 != null)
                {

                    if ((DateTime.Now - max2.HDatetime).TotalSeconds <= 31)
                    {
                        ViewData["Humd"] = "The  Humdity sensor is Active";
                    }
                    else
                    {
                        ViewData["Humd"] = "The Humidity  sensor is Inactive";
                    }
                }
                else
                {
                    ViewData["Humd"] = " Humdity data is empty";
                }


                if (max3 != null)
                {
                    if ((DateTime.Now - max3.LDatetime).TotalSeconds <= 31)
                    {
                        ViewData["Light"] = "The Light sensor is Active";
                    }
                    else
                    {
                        ViewData["Light"] = "The  Light sensor is Inactive";
                    }
                }
                else
                {
                    ViewData["Light"] = " Light data is empty";
                }

                if (max4 != null)
                {
                    if ((DateTime.Now - max4.WDatetime).TotalSeconds <= 31)
                    {
                        ViewData["Weight"] = "The Weight sensor is Active";
                    }
                    else
                    {
                        ViewData["Weight"] = "The Weight sensor is Inactive";
                    }
                }
                else
                {
                    ViewData["Weight"] = " Weight data is empty ";
                }
            }
            ViewData["id"] = id;
            return View();
        }

        [HttpPost]
        public ActionResult UploadCsv(IFormFile attachmentcsv)
        {
            PreparePreference();
            CsvFileDescription csvFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true
            };
            var ext = Path.GetExtension(attachmentcsv.FileName);
            if (ext != ".csv")
            {
                TempData["Msg"] = " Invalid file format";
                return Redirect("Index");
            }
            if (attachmentcsv.Length > 2000000)
            {
                TempData["Msg"] = " File Size above 2MB not Allowed";
                return Redirect("Index");
            }
            CsvContext csvContext = new CsvContext();
            StreamReader streamReader = new StreamReader(attachmentcsv.OpenReadStream());
            IEnumerable<Emsuser> list = csvContext.Read<Emsuser>(streamReader, csvFileDescription);


            _dbContext.Emsuser.AddRange(list);
            _dbContext.SaveChanges();
            return Redirect("Index");
        }


        [HttpGet]
        [Authorize]
        public IActionResult Profile()
        {
            BacklogActivity("Accessed Profile Page");

            PreparePreference();
            var userid = Convert.ToInt32(User.FindFirst(ClaimTypes.Sid).Value);
            DbSet<Emsuser> dbs = _dbContext.Emsuser;
            Emsuser user = dbs.Where(o => o.UserId == (userid)).FirstOrDefault();
            return View(user);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Profile(Emsuser user)
        {
            PreparePreference();
            DbSet<Emsuser> dbs = _dbContext.Emsuser;
            var userid = Convert.ToInt32(User.FindFirst(ClaimTypes.Sid).Value);
            Emsuser tuser = dbs.Where(p => p.UserId == userid)
                                      .FirstOrDefault();
            if (tuser != null)
            {
                tuser.Name = user.Name;
                tuser.Email = user.Email;
                tuser.Password = user.Password;
                tuser.Dob = user.Dob;
                tuser.Country = user.Country;


                if (_dbContext.SaveChanges() == 1) {
                    BacklogActivity("Updated User Profile");

                    TempData["Msg"] = "updated profile";
            }
            else
                TempData["Msg"] = "could not update";
            }


            return RedirectToAction("Index");
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
            return View(new EmsuserModel() { Items = data, SelectedFormat = "XLSX" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EmsuserModel format)
        {
            BacklogActivity("Generated User List");

            PreparePreference();
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
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
            sheet.Columns[2].SetWidth(100, LengthUnit.Pixel);
            sheet.Columns[3].SetWidth(200, LengthUnit.Pixel);
            sheet.Columns[4].SetWidth(200, LengthUnit.Pixel);


            sheet.Cells["A1"].Value = "Name";
            sheet.Cells["B1"].Value = "Name";
            sheet.Cells["C1"].Value = "Backlog Id";
            sheet.Cells["D1"].Value = "DateTime";
            sheet.Cells["E1"].Value = "Action";

            DbSet<Emsuser> dbs = _dbContext.Emsuser;

            for (int i = 0; i < dbs.Count(); i++)
            {
                sheet.Cells[i, 0].Value = dbs.Select(b => b.Name).ToList()[i];
                sheet.Cells[i, 1].Value = dbs.Select(b => b.Dob).ToList()[i];
                sheet.Cells[i, 2].Value = dbs.Select(b => b.UserId).ToList()[i];
                sheet.Cells[i, 3].Value = dbs.Select(b => b.Email).ToList()[i];
                sheet.Cells[i, 4].Value = dbs.Select(b => b.Role).ToList()[i];
            }

            return File(GetBytes(book, options), options.ContentType, "Backlog " + DateTime.Now + "." + format.SelectedFormat.ToLowerInvariant());

        }


        public class EmsuserModel
        {
            public string SelectedFormat { get; set; }
            public IList<Emsuser> Items { get; set; }
        }
        [NonAction]
        public void SendVerificationLinkEmail(string Email, string reply)
        {
            PreparePreference();
            // var verifyUrl = "/Account/VerifyAccount/" + activationCode;
            //var link = Request.Url.AbsoluteUri.Replace(Request.Url)
            // string link = ControllerContext.HttpContext.Request.Scheme.ToString() + "://" + ControllerContext.HttpContext.Request.Host.Value.ToString()
            // + verifyUrl;

            var fromEmail = new MailAddress("hyejinfyp98@gmail.com");
            var toEmail = new MailAddress(Email);
            var fromEmailPassword = "Iloveusehun1@";
            string subject = "Reply from Environmental Monitoring System";

            string body = reply;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(fromEmail.Address, fromEmailPassword)

            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
        //public ActionResult Index(HttpPostedFileBase postedFile)
        //{
        //    List<Emsuser> user = new List<Emsuser>();
        //    string filePath = string.Empty;
        //    if (postedFile != null)
        //    {
        //        string path = Server.MapPath("~/Uploads/");
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }

        //        filePath = path + Path.GetFileName(postedFile.FileName);
        //        string extension = Path.GetExtension(postedFile.FileName);
        //        postedFile.SaveAs(filePath);

        //        //Read the contents of CSV file.
        //        string csvData = System.IO.File.ReadAllText(filePath);

        //        //Execute a loop over the rows.
        //        foreach (string row in csvData.Split('\n'))
        //        {
        //            if (!string.IsNullOrEmpty(row))
        //            {
        //                user.Add(new Emsuser
        //                {
        //                    UserId = Convert.ToInt32(row.Split(',')[0]),
        //                    Name = row.Split(',')[1],
        //                    Country = row.Split(',')[2]

        //                });
        //            }
        //        }
        //    }

        //    return View(user);
        //}
        //}

        public IActionResult checkStatus(string date)
        {
            DateTime date1 = Convert.ToDateTime(date);
            //DateTime date = DateTime.Parse(date1);
            if ((DateTime.Now - date1).TotalSeconds <= 31)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

    }



}

