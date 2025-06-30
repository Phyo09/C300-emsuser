using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.StaticFiles; /*<-- file*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using NToastNotify;
using C300.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace C300
{
    public class HomeController : Controller
    {
        private IHostingEnvironment _env;
        private readonly IToastNotification _toastNotification;
        private AppDbContext _dbContext;
        private IHttpContextAccessor _contextAccessor;

        private HttpContext _context { get { return _contextAccessor.HttpContext; } }
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

        public HomeController(AppDbContext dbContext,IHostingEnvironment env,IToastNotification toastNotification, IHttpContextAccessor contextAccessor)
        {
            _env = env;
            _toastNotification = toastNotification;
            _dbContext = dbContext;
            _contextAccessor = contextAccessor;

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
                    Name = "System Generated Guest",
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
        //private void Notification()
        //{
        //    DbSet<Light> dbs = _dbContext.Light;
        //    DbSet<Preference> dbslmt = _dbContext.Preference;
        //    Random gen = new Random();
        //    dynamic light = Convert.ToDouble(gen.Next(1, 50));
        //    dynamic high = Convert.ToDouble(dbslmt.Where(p => p.UserId == 101).Select(p => p.HighestLight).FirstOrDefault());
        //    dynamic low = Convert.ToDouble(dbslmt.Where(p => p.UserId == 101).Select(p => p.LowestLight).FirstOrDefault());
        //    if (light >= high)
        //    {
        //        _toastNotification.AddWarningToastMessage("The light is over " + high + ". The current temperature is " + light);
        //    }
        //    else if (light <= low)
        //    {
        //        _toastNotification.AddWarningToastMessage("The light is over " + low + ". The current humidity is " + light);
        //    }
        //}
        // GET: /<controller>/
        public IActionResult Index()
        {
            BacklogActivity("Accessed Home Page");
            PreparePreference();
            //_toastNotification.AddWarningToastMessage("System initializating");
            return View();
        }

        public IActionResult FAQ()
        {
            BacklogActivity("Accessed FAQ Page");
            PreparePreference();
            return View();
        }

        public IActionResult AboutUs()
        {
            BacklogActivity("Accessed About Us Page");
            PreparePreference();
            return View();
        }
        public IActionResult ContactUs()
        {
            BacklogActivity("Accessed Contact Us Page");
            PreparePreference();
            return View();
        }
        [HttpGet]
        public IActionResult Contact()
        {
            BacklogActivity("Accessed Contact Us Page");
            PreparePreference();
            return View();
        }
        [HttpPost]
        public IActionResult Contact(Contact item)
        {
            PreparePreference();
            DbSet<Contact> dbs = _dbContext.Contact;
            dbs.Add(item);
            if (_dbContext.SaveChanges() == 1)
                TempData["Msg"] = "Submitted Query";
            else
                TempData["Msg"] = "Cannot submit Query";

            return View();
        }


        //[Authorize]
        public IActionResult Test()
        {
            PreparePreference();
            
            //String line;
            //try
            //{
            //    Pass the file path and file name to the StreamReader constructor
            //    StreamReader sr = new StreamReader("~/txt/FirstText.txt");

            //    Read the first line of text
            //    line = sr.ReadLine();

            //    Continue to read until you reach end of file
            //    while (line != null)
            //    {
            //        write the lie to console window
            //        Console.WriteLine(line);
            //        Read the next line
            //        line = sr.ReadLine();
            //    }

            //    close the file
            //    sr.Close();
            //    Console.ReadLine();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Exception: " + e.Message);
            //}
            //finally
            //{
            //    Console.WriteLine("Executing finally block.");
            //}
            //string dt = DateTimeOffset.Now.ToString("ddMMyyyy");

            //string path = Path.Combine(_env.WebRootPath, $"txt");
            //var initialJson = "";

            //if (!Directory.Exists(path))
            //{
            //    Directory.CreateDirectory(path);
            //}

            //string filePath = Path.Combine(_env.WebRootPath, $"txt/FirstText.txt");

            //if (System.IO.File.Exists(filePath))
            //{
            //    //initialJson = System.IO.File.ReadAllText(path);
            //}
            //ViewData["text"] = initialJson;

            //    //using (FileStream fs = System.IO.File.Create(filePath))
            //    //{
            //    //    AddText(fs, "foo");
            //    //    AddText(fs, "bar\tbaz");
            //    //}

            return View();
        }
        //private void AddText(FileStream fs, string value)
        //{
        //    byte[] info = new System.Text.UTF8Encoding(true).GetBytes(value);
        //    fs.Write(info, 0, info.Length);
        //}
    }
}
