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
using System.Text;
using System.Net.Mail;

namespace C300.Controllers
{
    public class CommentController : Controller
    {
        private AppDbContext _dbContext;
        private IHttpContextAccessor _contextAccessor;
        private HttpContext _context { get { return _contextAccessor.HttpContext; } }


        public IActionResult Wordlist()
        {
            DbSet<Words> dataword = _dbContext.Words;
            var badword = dataword.Select(o => o.Word).ToList();
            if (badword == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(badword);
            }

        }

        public CommentController(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
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
        public IActionResult Index(int id)
        {
            PreparePreference();

            DbSet<Comment> dbs = _dbContext.Comment;
            DbSet<Thread> dbs2 = _dbContext.Thread;
            DbSet<Emsuser> dbs4 = _dbContext.Emsuser;

            List<Comment> model = dbs.Where(o => o.ThreadId == id).ToList();
            TempData["SelectedThreadID"] = id;
            ViewData["id"] = id;
            DbSet<Words> data = _dbContext.Words;

            var wordlist = data.Select(g => g.Word).ToList<string>();
            ViewData["words"] = wordlist;
            var name = new List<string>();
            var model2 = dbs.Select(o => o.UserId).ToList();
            foreach (var i in model2)
            {
                name.Add(dbs4.Where(o => o.UserId == i).Select(o => o.Name).SingleOrDefault());
            }
            ViewData["name"] = name;

            var pic = new List<string>();
            foreach (var picture in model2)
            {
                pic.Add(dbs4.Where(o => o.UserId == picture).Select(o => o.Picture).SingleOrDefault());
            }
            ViewData["pic"] = pic;

            var loggeduser = Convert.ToInt32(User.FindFirst(ClaimTypes.Sid).Value);
            var profilepic = dbs4.Where(o => o.UserId == loggeduser).Select(o => o.Picture).FirstOrDefault();
            ViewData["picture"] = profilepic;

            //var firstuserid = dbs.Where(o => o.ThreadId == id).Select(o => o.UserId).FirstOrDefault();
            //var firstusername = dbs4.Where(o => o.UserId == firstuserid).Select(o => o.Name).FirstOrDefault();
            //ViewData["firstname"] = firstusername;


            Thread thread = dbs2.Where(o => o.ThreadId == id).FirstOrDefault();
            var uid = thread.UserId;
            var firstusername = dbs4.Where(o => o.UserId == uid).Select(o => o.Name).FirstOrDefault();
            ViewData["firstname"] = firstusername;
            //var modeluser = Convert.ToInt32(thread.UserId);
            //var picuser = dbs4.Where(o => o.UserId == modeluser).Select(o => o.Picture).First();
            //ViewData["picuser"] = picuser;
            ViewData["firstthread"] = thread;


            return View(model);

        }
        [HttpGet]
        public IActionResult Word()
        {
            PreparePreference();
            DbSet<Words> dbs5 = _dbContext.Words;
            List<Words> modelword = dbs5.ToList();
            ViewData["modelword"] = modelword;
            return View();
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            PreparePreference();
            DbSet<Words> dbs5 = _dbContext.Words;
            Words badword = dbs5.Where(o => o.Wordid == id).FirstOrDefault();
            return View(badword);
        }
        public IActionResult DeleteWord(int id)
        {
            PreparePreference();
            DbSet<Words> dbs5 = _dbContext.Words;
            Words badword = dbs5.Where(o => o.Wordid == id).FirstOrDefault();
            if (badword == null)
            {
                TempData["Msg"] = "Deleted Already";
            }
            else
            {
                dbs5.Remove(badword);
            }
            if (_dbContext.SaveChanges() == 1)
                TempData["Msg"] = " Deleted";
            else
                TempData["Msg"] = "Cannot Delete";

            return RedirectToAction("Word");

        }

        [HttpPost]
        public IActionResult Word(Words item)
        {
            PreparePreference();
            DbSet<Words> dbs5 = _dbContext.Words;
            List<string> check = dbs5.Select(o => o.Word).ToList();
            if (check.Contains(item.ToString(), StringComparer.OrdinalIgnoreCase))
            {
                TempData["Msg"] = "Word Already Exists";
                return RedirectToAction("Word");
            }
            else
            {
                dbs5.Add(item);

                if (_dbContext.SaveChanges() == 1)
                    TempData["Msg"] = "success";
                else
                    TempData["Msg"] = "Cannot submit";
            }
            return RedirectToAction("Word");

        }
        public IActionResult Delete(int id)
        {
            PreparePreference();
            DbSet<Thread> dbs2 = _dbContext.Thread;
            DbSet<Comment> dbs = _dbContext.Comment;
            Comment obj = dbs.Where(o => o.CommentId == id).FirstOrDefault();
            var threadid = obj.ThreadId;
            if (obj != null)
            {
                Thread thread = dbs2.Where(o => o.ThreadId == threadid).FirstOrDefault();
                thread.CommentCount = thread.CommentCount - 1;
                dbs.Remove(obj);
                if (_dbContext.SaveChanges() == 1)
                {
                    BacklogActivity("Deleted Comment");
                    TempData["Msg"] = "Comment Deleted";
                }
                else
                    TempData["Msg"] = "Comment Deleted";


            }
            return RedirectToAction("Index", new { id = threadid });
        }
        public IActionResult Likes(int x)
        {
            PreparePreference();
            DbSet<Comment> likes = _dbContext.Comment;
            Comment commentlike = likes.Where(o => o.CommentId == x).FirstOrDefault();
            commentlike.Like = commentlike.Like + 1;
            if (_dbContext.SaveChanges() == 1)
                TempData["Msg"] = "Liked!";
            else
                TempData["Msg"] = "Error Liking!";

            return RedirectToAction("Index", new { id = commentlike.ThreadId });
        }
        public IActionResult Report(int id)
        {
            PreparePreference();
            DbSet<Comment> dbs = _dbContext.Comment;
            DbSet<Thread> dbs2 = _dbContext.Thread;
            DbSet<Emsuser> dbs4 = _dbContext.Emsuser;
            Comment reportcomment = dbs.Where(o => o.CommentId == id).FirstOrDefault();
            var threadid = reportcomment.ThreadId;

            List<string> emaillist = dbs4.Where(o => o.Role == "Admin").Select(o => o.Email).ToList();

            if (reportcomment.Report >= 3)
            {
                foreach (var email in emaillist)
                {
                    SendVerificationLinkEmail(email, "The  comment has been reported many times.Please check the following comment" + "\n" + reportcomment.Content);
                }
                return RedirectToAction("Index", new { id = threadid });
            }
            else
            {
                reportcomment.Report += 1;

            }
            if (_dbContext.SaveChanges() == 1)
            {
                BacklogActivity("Comment has been reported");
                TempData["Msg"] = "sucessfully Reported";
            }
            else
            {
                TempData["report"] = "Could not Report";
            }
            return RedirectToAction("Index", new { id = threadid });
        }



        public IActionResult _CommentList(String comment, int id, int anon)
        {
            PreparePreference();
            DbSet<Comment> dbs = _dbContext.Comment;
            DbSet<Emsuser> dbs4 = _dbContext.Emsuser;
            DbSet<Thread> dbs2 = _dbContext.Thread;
            Thread thread = dbs2.Where(o => o.ThreadId == id).FirstOrDefault();

            int threadID = Convert.ToInt32(TempData["SelectedThreadID"]);

            Comment commentObj = new Comment();
            if (anon == 2)
            {
                commentObj.Anonymous = anon;
            }


            commentObj.Content = comment;
            commentObj.CreatedDate = DateTime.Now;
            commentObj.ThreadId = id;
            commentObj.UserId = Convert.ToInt32(User.FindFirst(ClaimTypes.Sid).Value);
            dbs.Add(commentObj);
            thread.CommentCount = thread.CommentCount + 1;
            dbs2.Update(thread);
            var model3 = dbs.Where(o => o.ThreadId == id).Select(o => o.UserId).ToList();
            var email = new List<string>();
            foreach (var uid in model3)
            {
                var emailid = dbs4.Where(o => o.UserId == uid).Select(o => o.Email).SingleOrDefault();
                if (email.Contains(emailid))
                {

                }
                else
                {
                    email.Add(emailid);
                }
            }
            foreach (var send in email)
            {
                SendVerificationLinkEmail(send, "Reply on the Thread-" + System.Environment.NewLine + comment + System.Environment.NewLine + "  By UserID-" + commentObj.UserId);
            }
            if (_dbContext.SaveChanges() == 1)
            {
                TempData["Msg"] = "comment added";

            }
            else
                TempData["Msg"] = "comment not added";
            List<Comment> model = dbs.Where(o => o.ThreadId == id).ToList();
            var model2 = User.FindFirst(ClaimTypes.GivenName).Value;
            ViewData["name"] = model2;

            var pic = new List<string>();

            foreach (var i in model3)
            {
                pic.Add(dbs4.Where(o => o.UserId == i).Select(o => o.Picture).SingleOrDefault());
            }
            ViewData["pic"] = pic;
            //return PartialView("_CommentList",model);
            return RedirectToAction("Index", new { id = id });
        }
        [NonAction]
        public void SendVerificationLinkEmail(string Email, string reply)
        {



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
                try
                {
                    smtp.Send(message);
                }
                catch (Exception e)
                {
                    string ErrorString = e.Message;
                    TempData["Msg"] = ErrorString;
                }
        }
    }




}


