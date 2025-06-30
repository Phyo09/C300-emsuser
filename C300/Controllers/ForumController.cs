using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Data.Common;
using System.Data.SqlClient;
using System.Security.Claims;
using C300.Models;

namespace C300.Controllers
{
    public class ForumController : Controller
    {
        private AppDbContext _dbContext;
        private IHttpContextAccessor _contextAccessor;
        private HttpContext _context { get { return _contextAccessor.HttpContext; } }

        public ForumController(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
        {
            _dbContext = dbContext;
            _contextAccessor = contextAccessor;

        }
        [Authorize]

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


        public IActionResult Index()
        {
            BacklogActivity("Accessed Forum Page");
            PreparePreference();
            DbSet<Topic> dbs = _dbContext.Topic;
            DbSet<Thread> dbs5 = _dbContext.Thread;
            DbSet<Comment> dbs2 = _dbContext.Comment;
            DbSet<Emsuser> dbs4 = _dbContext.Emsuser;
            int count = dbs2.Count();
            ViewData["count"] = count;
            List<Topic> model = null;
            string userid = (User.FindFirst(ClaimTypes.GivenName).Value);
            if (userid != null)
            {
                ViewData["name"] = userid;
            }

            model = dbs.ToList();
            var topiclist = dbs.Select(o => o.TopicId).ToList();
            List<Thread> threadlist = new List<Thread>();
            foreach (var item in topiclist)
            {
                Thread lastthread = dbs5.Where(o => o.TopicId == item).LastOrDefault();
                threadlist.Add(lastthread);
            }
            ViewData["threadlist"] = threadlist;

            //var name = new List<string>();
            //var model2 = dbs.Select(o => o.UserId).ToList();
            //foreach (var i in model2)
            //{
            //    name.Add(dbs4.Where(o => o.UserId == i).Select(o => o.Name).SingleOrDefault());
            //}
            //ViewData["names"] = name;
            return View(model);
        }
        [HttpGet]
        public IActionResult Create()
        {

            PreparePreference();
            return View();
        }
        [HttpPost]
        public IActionResult Create(Topic topic)
        {
            PreparePreference();
            if (ModelState.IsValid)
            {
                DbSet<Topic> dbs = _dbContext.Topic;
                topic.ThreadCount = 0;
                topic.DateTime = DateTime.Now;
                topic.UserId = Convert.ToInt32(User.FindFirst(ClaimTypes.Sid).Value);
                dbs.Add(topic);
                if (_dbContext.SaveChanges() == 1)
                {
                    BacklogActivity("Created Topic");
                    TempData["Msg"] = "New Topic added!";
                }
                else
                    TempData["Msg"] = "Topic could not be added";
                var model2 = User.FindFirst(ClaimTypes.GivenName).Value;
                ViewData["name"] = model2;
            }
            else
            {
                TempData["Msg"] = "Inavlid information added";
            }
            return RedirectToAction("Index");

        }
        public IActionResult Delete(int id)
        {
            PreparePreference();
            DbSet<Topic> dbs = _dbContext.Topic;
            DbSet<Thread> dbs2 = _dbContext.Thread;
            Topic del = dbs.Where(o => o.TopicId == id).FirstOrDefault();
            var count = dbs2.Where(o => o.TopicId == id).Count();

            if (del != null)
            {
                if (count == 0)
                {
                    dbs.Remove(del);
                    if (_dbContext.SaveChanges() == 1)
                    {
                        BacklogActivity("Deleted Topic");
                        TempData["Msg"] = "Topic Deleted";
                    }
                    else
                        TempData["Msg"] = "Topic Could not be deleted";
                }
                else
                {
                    TempData["Msg"] = "Topic is Not Empty";
                }
            }
            return RedirectToAction("Index");

        }
    }
}

