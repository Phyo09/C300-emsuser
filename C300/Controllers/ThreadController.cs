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
using System.Security.Claims;
using System.Security.Principal;

namespace C300.Controllers
{
    public class ThreadController : Controller
    {
        private AppDbContext _dbContext;
        private IHttpContextAccessor _contextAccessor;

        private HttpContext _context { get { return _contextAccessor.HttpContext; } }

        public ThreadController(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
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

        [Authorize]
        public IActionResult Index(int id)
        {
            BacklogActivity("Accessed Forum Page");
            PreparePreference();
            DbSet<Thread> dbs = _dbContext.Thread;
            DbSet<Comment> dbs3 = _dbContext.Comment;
            DbSet<Emsuser> dbs4 = _dbContext.Emsuser;
            List<Thread> threadlist = dbs.Where(o => o.TopicId == id).ToList();
            DbSet<Topic> dbs2 = _dbContext.Topic;
            var name = new List<string>();
            var model2 = dbs.Select(o => o.UserId).ToList();
            foreach (var i in model2)
            {
                name.Add(dbs4.Where(o => o.UserId == i).Select(o => o.Name).SingleOrDefault());
            }
            ViewData["name"] = name;

            var comment = dbs3.Count();

            foreach (var item in dbs3.Select(o => o.ThreadId).ToList())
            {
                List<Comment> count = dbs3.Where(o => o.ThreadId == item).ToList();
                int c = count.Count();
                ViewData["Count"] = c;

            }



            ViewData["id"] = id;

            return View(threadlist);
        }

        public IActionResult Comment()
        {

            PreparePreference();
            return View();
        }
        public IActionResult Likes(int x)
        {
            PreparePreference();
            DbSet<Thread> likes = _dbContext.Thread;
            Thread threadlike = likes.Where(o => o.ThreadId == x).FirstOrDefault();
            threadlike.Like = threadlike.Like + 1;
            if (_dbContext.SaveChanges() == 1)
            {
                BacklogActivity("Liked a Thread");
                TempData["Msg"] = "Liked!";
            }
            else
                TempData["Msg"] = "Error Liking!";

            return RedirectToAction("Index", new { id = threadlike.TopicId });
        }
        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            PreparePreference();
            ViewData["CategoryID"] = TempData["Category"];
            DbSet<Topic> dbs = _dbContext.Topic;
            List<Topic> topicList = dbs.ToList();
            ViewData["topic"] = topicList;
            return View();
        }
        [HttpPost]
        [Authorize]
        public IActionResult Create(Thread thread)
        {
            PreparePreference();
            if (ModelState.IsValid)
            {
                DbSet<Topic> dbs3 = _dbContext.Topic;
                Topic topic = dbs3.Where(o => o.TopicId == thread.TopicId).FirstOrDefault();
                DbSet<Thread> dbs = _dbContext.Thread;
                DbSet<Comment> dbs1 = _dbContext.Comment;
                thread.CommentCount = 1;
                thread.CreatedDate = DateTime.Now;
                thread.UserId = Convert.ToInt32(User.FindFirst(ClaimTypes.Sid).Value);


                dbs.Add(thread);
                topic.ThreadCount = topic.ThreadCount + 1;
                dbs3.Update(topic);

                if (_dbContext.SaveChanges() == 1)
                {
                    BacklogActivity("Created a New Thread");
                    TempData["Msg"] = "NEW THREAD ADDED";

                }
                else
                {
                    TempData["Msg"] = "NEW THREAD ADDED";
                }
            }
            else
            {
                TempData["Msg"] = "Invalid Information";

            }
            return RedirectToAction("Index", new { id = thread.TopicId });

        }
        public IActionResult Delete(int id)
        {
            PreparePreference();
            DbSet<Comment> dbs = _dbContext.Comment;
            DbSet<Topic> dbs3 = _dbContext.Topic;
            DbSet<Thread> dbs2 = _dbContext.Thread;

            Thread del = dbs2.Where(o => o.ThreadId == id).FirstOrDefault();
            var topicid = del.TopicId;
            var count = dbs.Where(o => o.ThreadId == id).Count();

            if (del != null)
            {
                if (count == 0)
                {
                    Topic topic = dbs3.Where(o => o.TopicId == topicid).FirstOrDefault();
                    topic.ThreadCount = topic.ThreadCount - 1;
                    dbs2.Remove(del);
                    if (_dbContext.SaveChanges() == 1)
                    {
                        BacklogActivity("Deleted a Thread");
                        TempData["Msg"] = "Thread Deleted";
                    }
                    else
                        TempData["Msg"] = "Thread deleted";
                }
                else
                {
                    TempData["Msg"] = "Thread is Not Empty";
                }
            }
            return RedirectToAction("Index", new { id = del.TopicId });

        }

    }
}

