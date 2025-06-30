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
using System.IO;
using System.Data.Common;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
namespace C300.Controllers
{
    public class CalenderController : Controller
    {
        private AppDbContext _dbContext;
        private IHttpContextAccessor _contextAccessor;
        private HttpContext _context { get { return _contextAccessor.HttpContext; } }




        public CalenderController(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
        {
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
            BacklogActivity("Accessed Calendar");
            PreparePreference();
            return View();
        }
        public IActionResult GetEvents()
        {
            PreparePreference();
            DbSet<Event> dbs = _dbContext.Event;
            var events = dbs.ToList();
            var sa = new JsonSerializerSettings();
            return Json(events, sa);
        }

        public IActionResult SaveEvent(Event e)
        {
            PreparePreference();
            var status = false;
            var sa = new JsonSerializerSettings();
            DbSet<Event> dbs = _dbContext.Event;
            if (e.EventId > 0)
            {
                var v = dbs.Where(o => o.EventId == e.EventId).FirstOrDefault();
                if (v != null)
                {
                    v.Subject = e.Subject;
                    v.Start = e.Start;
                    v.EndDay = e.EndDay;
                    v.Description = e.Description;
                    v.IsFullDay = e.IsFullDay;
                    v.ThemeColor = e.ThemeColor;
                }
            }
            else
            {
                dbs.Add(e);
            }
            if (_dbContext.SaveChanges() == 1)
            {
                status = true;
            }

            return Json(status);
        }
        [HttpPost]

        public IActionResult DeleteEvent(int eventID)
        {
            BacklogActivity("Deleted Calendar Event");
            PreparePreference();
            var status = false;
            DbSet<Event> dbs = _dbContext.Event;
            var v = dbs.Where(o => o.EventId == eventID).FirstOrDefault();
            if (v != null)
            {
                dbs.Remove(v);

                if (_dbContext.SaveChanges() == 1)
                {
                    status = true;
                }

            }
            return Json(status);
        }

    }
}

