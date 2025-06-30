using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using GemBox.Spreadsheet;
// TODO P08 1-1: import namespaces P08.Models and Microsoft.EnitityFrameworkCore
using C300.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Net.Mail;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace C300.Controllers
{
    public class FeedbackController : Controller
    {
        private AppDbContext _dbContext;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;
        private readonly IList<Feedback> data;
        private IHttpContextAccessor _contextAccessor;
        private HttpContext _context { get { return _contextAccessor.HttpContext; } }
        // TODO P08 1-3: Create constructor to receive dbContext and initialize the _dbContext variable
        public FeedbackController(AppDbContext dbContext, IHostingEnvironment environment, IHttpContextAccessor contextAccessor)
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
                    Name = userEmail,
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

        public IActionResult Test()
        {
            PreparePreference();
            List<Feedback> model = _dbContext.Feedback.ToList();
            return View(model);
        }

        public IActionResult Index()
        {
            //DbSet<Feedback> feedback = _dbContext.Feedback;
            //List<Feedback> model = feedback.ToList();
            BacklogActivity("Accessed Feedback Page");
            PreparePreference();

            return View();
        }


        [HttpGet]
        public IActionResult Create()
        {
            BacklogActivity("Accessed Feedback Page");
            PreparePreference();
            //var email = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            //ViewData["email"] = email;
            if (User.Identity.IsAuthenticated)
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
                ViewData["email"] = email;

            }

            DbSet<FeedbackType> dbs = _dbContext.FeedbackType;
            // where value is Id and text is Name and pass the list to the view through ViewData
            // var lstPokes = new List<dynamic>() { new { value = "0", text = "" } }; // replace this line with your code
            var lstFeedback = dbs.ToList<FeedbackType>()
                .OrderBy(p => p.Description)
                .Select(p =>
                {
                    dynamic d = new ExpandoObject();
                    d.value = p.Id;
                    d.text = p.Description;
                    return d;
                })
                .ToList<dynamic>();
            ViewData["feedbacks"] = lstFeedback;

            Feedback model = new Feedback();
            return View(model);
        }


        [HttpPost]
        public IActionResult Create(Feedback feedback)
        {
            PreparePreference();

            if (User.Identity.IsAuthenticated)
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
                ViewData["email"] = email;

            }
            DbSet<FeedbackType> dbs1 = _dbContext.FeedbackType;
            // where value is Id and text is Name and pass the list to the view through ViewData
            // var lstPokes = new List<dynamic>() { new { value = "0", text = "" } }; // replace this line with your code
            var lstFeedback = dbs1.ToList<FeedbackType>()
                .OrderBy(p => p.Description)
                .Select(p =>
                {
                    dynamic d = new ExpandoObject();
                    d.value = p.Id;
                    d.text = p.Description;
                    return d;
                })
                .ToList<dynamic>();
            ViewData["feedbacks"] = lstFeedback;

            if (ModelState.IsValid)
            {

                DbSet<Feedback> dbs = _dbContext.Feedback;
                feedback.Datetime = DateTime.Now;

                dbs.Add(feedback);
                if (_dbContext.SaveChanges() == 1) { 
                    BacklogActivity("Submitted Feedback");
                    ViewData["Msg"] = "Thank you for your feedback";
                }


                else
                    ViewData["Msg"] = "Failed to update database!";
            }
            else
            {
                ViewData["Msg"] = "Invalid information entered";
            }
            Feedback model = new Feedback();
            return View(model);
            //return RedirectToAction("Create");
            //Feedback model = new Feedback();
            //return View(model);
        }

        [Authorize(Roles ="Admin,Staff")]
        public IActionResult ViewFeedback()
        {
            BacklogActivity("Accessed Feedback List");
            PreparePreference();
            List<Feedback> model = _dbContext.Feedback.ToList();
            ViewData["all"] = model.Select(p => p.Reply).Count();
            ViewData["pending"] = model.Where(p => p.Reply == null).Count();
            ViewData["solved"] = model.Where(p => p.Reply != null).Count();
            return View(model);
        }

        [Authorize(Roles = "Admin,Staff")]
        public IActionResult ReplyFeedback(int id)
        {
            PreparePreference();
            DbSet<Feedback> dbs = _dbContext.Feedback;
            Feedback feedback = dbs.Where(p => p.FeedbackId == id).FirstOrDefault();


            //FeedbackReply model = new FeedbackReply();
            // model.Email = feedback.Email;
            //model.des = feedback.Description;

            return View(feedback);
            //return View();

        }
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult ReplyFeedback(Feedback feedback)
        {
            PreparePreference();
            //dbs.Add(feedback);
            //Feedback feedback = dbs.Where(p => p.FeedbackId == id).FirstOrDefault();

            if (ModelState.IsValid)
            {
                DbSet<Feedback> dbs = _dbContext.Feedback;
                string solver = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

                Feedback reply = dbs.Where(p => p.FeedbackId == feedback.FeedbackId).FirstOrDefault();
                if (reply != null)
                {
                    if (reply.Reply == feedback.Reply)
                    {
                        ViewData["Msg"] = "Same feedback";
                    }
                    else
                    {

                        // update reply 
                        // reply = database , feedback = user's input 
                        reply.Reply = feedback.Reply;
                        reply.Subject = feedback.Subject;
                        reply.Solvedby = solver;
                        reply.SolvedTime = DateTime.Now;
                        if (_dbContext.SaveChanges() == 1)
                        {
                            SendVerificationLinkEmail(feedback.Email, feedback.Reply, feedback.Subject);
                            BacklogActivity("Replied to Feedback");
                            ViewData["Msg"] = "Your reply is sent to " + feedback.Email;
                        }

                        else

                            ViewData["Msg"] = "Failed to send the reply to  " + feedback.Email;
                    }

                }
                else
                {
                    ViewData["Msg"] = "No feedback given";
                }

            }

            else
                ViewData["Msg"] = "Invalid input!";


            // Feedback model = new Feedback();
            return View(feedback);

            //return View();

        }


        [NonAction]
        public void SendVerificationLinkEmail(string Email, string reply, string sub)
        {

            // var verifyUrl = "/Account/VerifyAccount/" + activationCode;
            //var link = Request.Url.AbsoluteUri.Replace(Request.Url)
            // string link = ControllerContext.HttpContext.Request.Scheme.ToString() + "://" + ControllerContext.HttpContext.Request.Host.Value.ToString()
            // + verifyUrl;

            var fromEmail = new MailAddress("hyejinfyp98@gmail.com");
            var toEmail = new MailAddress(Email);
            var fromEmailPassword = "Iloveusehun1@";
            string subject = sub;

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

        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Create1()
        {
            PreparePreference();
            return View(new FeedbackModel() { Items = data, SelectedFormat = "XLSX" });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [ValidateAntiForgeryToken]

        public IActionResult Create1(FeedbackModel format)
        {
            BacklogActivity("Generated Feedback Report");
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
            sheet.Columns[1].SetWidth(200, LengthUnit.Pixel);
            sheet.Columns[2].SetWidth(100, LengthUnit.Pixel);
            sheet.Columns[3].SetWidth(200, LengthUnit.Pixel);
            sheet.Columns[4].SetWidth(300, LengthUnit.Pixel);
            sheet.Columns[5].SetWidth(100, LengthUnit.Pixel);
            sheet.Columns[6].SetWidth(100, LengthUnit.Pixel);
            sheet.Columns[7].SetWidth(100, LengthUnit.Pixel);
            sheet.Columns[8].SetWidth(100, LengthUnit.Pixel);


            sheet.Cells["A1"].Value = "ID";
            sheet.Cells["B1"].Value = "Email";
            sheet.Cells["C1"].Value = "Feedback Type";
            sheet.Cells["D1"].Value = "Created Time";
            sheet.Cells["E1"].Value = "Feedback Content";
            sheet.Cells["F1"].Value = "Solved Time";
            sheet.Cells["G1"].Value = "Action";
            sheet.Cells["H1"].Value = "Status";
            sheet.Cells["I1"].Value = "Solved By";

            DbSet<Feedback> dbs = _dbContext.Feedback;

            for (int i = 1; i <= dbs.Count(); i++)
            {


                sheet.Cells[i, 0].Value = dbs.Select(b => b.FeedbackId).ToList()[i - 1];
                sheet.Cells[i, 1].Value = dbs.Select(b => b.Email).ToList()[i - 1];

                string type = "";
                if (dbs.Select(b => b.FeedbackType).ToList()[i - 1] == 1)
                {
                    type = "Bug Reports";
                }
                else if (dbs.Select(b => b.FeedbackType).ToList()[i - 1] == 2)
                {
                    type = "Comment";
                }
                else if (dbs.Select(b => b.FeedbackType).ToList()[i - 1] == 3)
                {
                    type = "Question";
                }
                sheet.Cells[i, 2].Value = type;
                sheet.Cells[i, 3].Value = dbs.Select(b => b.Datetime).ToList()[i - 1];
                sheet.Cells[i, 4].Value = dbs.Select(b => b.Description).ToList()[i - 1];
                sheet.Cells[i, 5].Value = dbs.Select(b => b.SolvedTime).ToList()[i - 1];

                string action = "Reply";
                sheet.Cells[i, 6].Value = action;

                string status = "";
                if (dbs.Select(b => b.Reply).ToList()[i - 1] == null)
                {
                    status = "PENDING";
                }

                else if (dbs.Select(b => b.Reply).ToList()[i - 1] != null)
                {
                    status = "SOLVED";
                }
                sheet.Cells[i, 7].Value = status;
                sheet.Cells[i, 8].Value = dbs.Select(b => b.Solvedby).ToList()[i - 1];
            }

            return File(GetBytes(book, options), options.ContentType, "Feedback " + DateTime.Now + "." + format.SelectedFormat.ToLowerInvariant());

        }


        public class FeedbackModel
        {
            public string SelectedFormat { get; set; }
            public IList<Feedback> Items { get; set; }
        }
    }
}