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
    public class NotificationController : Controller
    {
        private AppDbContext _dbContext;



        public NotificationController(AppDbContext dbContext)
        {
            _dbContext = dbContext;

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

        public IActionResult Temperature()
        {
            //Random gen = new Random();
            //string[] data = new string[1];
            //dynamic temp = Convert.ToDouble(gen.Next(1, 50));
            //data[0] = string.Format("{0:0.00}", temp);
            //return Json(data);
            DbSet<Temperature> dbs = _dbContext.Temperature;
            Temperature model = dbs.OrderByDescending(p => p.TDatetime).LastOrDefault();
            if (model != null)
            {
                return Json(model.TLevel);
            }
            else
            {
                return Json("0.00");
            }

        }
        public IActionResult Humidity()
        {
            //Random gen = new Random();
            //string[] data = new string[1];
            //dynamic hum = Convert.ToDouble(gen.Next(1, 50));
            //data[0] = string.Format("{0:0.00}", hum);
            //return Json(data);
            DbSet<Humidity> dbs = _dbContext.Humidity;
            Humidity model = dbs.OrderByDescending(p => p.HDatetime).LastOrDefault();
            if (model != null)
            {
                return Json(model.HLevel);
            }
            else
            {
                return Json("0.00");
            }

        }
        public IActionResult Light()
        {
            //Random gen = new Random();
            //string[] data = new string[1];
            //dynamic light = Convert.ToDouble(gen.Next(1, 50));
            //data[0] = string.Format("{0:0.00}", light);
            //return Json(data);
            DbSet<Light> dbs = _dbContext.Light;
            Light model = dbs.OrderByDescending(p => p.LDatetime).LastOrDefault();
            if (model != null)
            {
                return Json(model.LLevel);
            }
            else
            {
                return Json("0.00");
            }

        }
        public IActionResult Weight()
        {
            //Random gen = new Random();
            //string[] data = new string[1];
            //dynamic w8 = Convert.ToDouble(gen.Next(1, 50));            
            //data[0] = string.Format("{0:0.00}", w8);
            //return Json(data);
            DbSet<Weight> dbs = _dbContext.Weight;
            Weight model = dbs.OrderByDescending(p => p.WDatetime).LastOrDefault();
            if (model != null)
            {
                return Json(model.WLevel);
            }
            else
            {
                return Json("0.00");
            }
        }

        [Authorize]
        public IActionResult _RetrieveTemp()
        {
            PreparePreference();
            DbSet<Temperature> dbs = _dbContext.Temperature;
            DbSet<Humidity> dbs3 = _dbContext.Humidity;
            DbSet<Light> db4 = _dbContext.Light;
            DbSet<Weight> db5 = _dbContext.Weight;
            DbSet<Preference> dbs2 = _dbContext.Preference;
            DbSet<Emsuser> user = _dbContext.Emsuser;
            DbSet<Thread> threaduser = _dbContext.Thread;
            DbSet<Comment> comment = _dbContext.Comment;
            var userid = Convert.ToInt32(User.FindFirst(ClaimTypes.Sid).Value);
            String message = "";

            List<int> usercomment = new List<int>();
            var userthread = threaduser.Where(o => o.UserId == userid).Select(o => o.ThreadId).Distinct().ToList();
            foreach (var item in userthread)
            {
                List<Comment> commentList = comment.Where(o => o.ThreadId == item && o.CreatedDate.Date == DateTime.Now.Date && o.UserId != userid).ToList();
                foreach (var item2 in commentList)
                {
                    var particularUser = user.Where(o => o.UserId == item2.UserId).FirstOrDefault();
                    if (message.Length > 0)
                    {
                        if (item2.Content.Length > 15)
                        {
                            message += ";" + particularUser.Name + " commented " + item2.Content.Substring(0, 15) + "...";
                        }
                        else
                        {
                            message += ";" + particularUser.Name + " commented " + item2.Content + ".";
                        }
                    }
                    else
                    {
                        if (item2.Content.Length > 15)
                        {
                            message += particularUser.Name + " commented " + item2.Content.Substring(0, 15) + "...";
                        }
                        else
                        {
                            message += particularUser.Name + " commented " + item2.Content + ".";
                        }
                    }
                }
            }

            float hight = dbs2.Where(o => o.UserId == userid).Select(o => o.HighestTemp).FirstOrDefault();
            float highL = dbs2.Where(o => o.UserId == userid).Select(o => o.HighestLight).FirstOrDefault();
            float highW = dbs2.Where(o => o.UserId == userid).Select(o => o.HighestWeight).FirstOrDefault();
            float highH = dbs2.Where(o => o.UserId == userid).Select(o => o.HighestHumidity).FirstOrDefault();
            double temp = dbs
                .Where(o => o.TDatetime.Day == DateTime.Now.Day && o.TLevel > hight)
                .Select(g => g.TLevel).FirstOrDefault();

            double light = db4
                .Where(o => o.LDatetime.Day == DateTime.Now.Day && o.LLevel > highL)
                .Select(g => g.LLevel).FirstOrDefault();

            double humidity = dbs3
                .Where(o => o.HDatetime.Day == DateTime.Now.Day && o.HLevel > highH)
                .Select(g => g.HLevel).FirstOrDefault();

            double weight = db5
                .Where(o => o.WDatetime.Day == DateTime.Now.Day && o.WLevel > highW)
                .Select(g => g.WLevel).FirstOrDefault();

            string json = "";

            if (temp > 0)
            {
                if (message.Length > 0)
                {
                    message += ";Temperature has reached max capacity!";
                }
                else
                {

                    message += "Temperature has reached max capacity!";
                }
            }

            if (light > 0)
            {
                if (message.Length > 0)
                {
                    message += ";Light has reached max capacity!";
                }
                else
                {
                    message += "Light has reached max capacity!";
                }
            }
            if (weight > 0)
            {
                if (message.Length > 0)
                {
                    message += ";Weight has reached max capacity!";
                }
                else
                {
                    message += "Weight has reached max capacity!";
                }
            }
            if (humidity > 0)
            {
                if (message.Length > 0)
                {
                    message += ";Humidity has reached max capacity!";
                }
                else
                {
                    message += "Humidity has reached max capacity!";
                }
            }

            json = JsonConvert.SerializeObject(message);

            return Ok(json);
        }
    }
}

    

