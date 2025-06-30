using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using C300.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Dynamic;
using Microsoft.AspNetCore.Http;

namespace C300.Controllers
{
    public class ProgressiveController : Controller
    {        
        private AppDbContext _dbContext;
        private IHttpContextAccessor _contextAccessor;
        private HttpContext _context { get { return _contextAccessor.HttpContext; } }

        public ProgressiveController(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
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
        public IActionResult Update()
        {
            Random gen = new Random();
            int[] data = new int[1];
            data[0] = 0;
            string sql1 = @"Insert into temperature(T_level,T_datetime) 
                            Values ({0},'{1}')";
            if (DBUtl.ExecSQL(sql1, Convert.ToDouble(gen.Next(0, 40)), string.Format("{0:yyyy-MM-dd hh:mm:ss}", DateTime.Now)) == 1)
            {
                data[0] = data[0] + 1;
            }
            string sql2 = @"Insert into humidity(H_level,H_datetime) 
                            Values ({0},'{1}')";
            if (DBUtl.ExecSQL(sql2, Convert.ToDouble(gen.Next(0, 40)), string.Format("{0:yyyy-MM-dd hh:mm:ss}", DateTime.Now)) == 1)
            {
                data[0] = data[0] + 1;
            }
            string sql3 = @"Insert into Light(L_level,L_datetime) 
                            Values ({0},'{1}')";
            if (DBUtl.ExecSQL(sql3, Convert.ToDouble(gen.Next(0, 40)), string.Format("{0:yyyy-MM-dd hh:mm:ss}", DateTime.Now)) == 1)
            {
                data[0] = data[0] + 1;
            }
            string sql4 = @"Insert into Weight(W_level,W_datetime) 
                            Values ({0},'{1}')";
            if (DBUtl.ExecSQL(sql4, Convert.ToDouble(gen.Next(0, 40)), string.Format("{0:yyyy-MM-dd hh:mm:ss}", DateTime.Now)) == 1)
            {
                data[0] = data[0]+1;
                
            }
            return Json(data);
        }
        public IActionResult Dynamic()
        {
            BacklogActivity("Accessed Live Page");
            PreparePreference();
            return View();
        }
        [Authorize]
        public IActionResult Live(int id)
        {
            PreparePreference();
            DbSet<Preference> dbs1 = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
            if (id == 1)
            {
                ViewData["Title"] = "Progressive Line Graph of Temperature";
                ViewData["id"] = 1;
                ViewData["type"] = "Temperature";
                ViewData["color"] = /*"#D95350";*/ "red";
                if (model1.TempUnit == 2)
                {
                    ViewData["unit"] = "K";
                }
                else if (model1.TempUnit == 3)
                {
                    ViewData["unit"] = "'F";
                }
                else
                {
                    ViewData["unit"] = "'C";
                }
            }
            else if (id == 2)
            {
                ViewData["id"] = 2;
                ViewData["type"] = "Humidity";
                ViewData["color"] = "#0375D8";/*"#0375D8";*/ /*"blue";*/
                ViewData["unit"] = "%";
            }
            else if (id == 3)
            {
                ViewData["Title"] = "Progressive Line Graph of Light";
                ViewData["id"] = 3;
                ViewData["type"] = "Light";
                ViewData["color"] = "#FFC000";/*"#ECAF4E";*/ /*"yellow";*/
                ViewData["unit"] = "lx";
            }
            else if (id == 4)
            {
                ViewData["Title"] = "Progressive Line Graph of Force";
                ViewData["id"] = 4;
                ViewData["type"] = "Force";
                ViewData["color"] = /*"#5DB75D";*/ "green";
                if (model1.WeightUnit == 2)
                {
                    ViewData["unit"] = "g";
                }
                else if (model1.WeightUnit == 3)
                {
                    ViewData["unit"] = "kg";
                }
                else if (model1.WeightUnit == 4)
                {
                    ViewData["unit"] = "lb";
                }
                else 
                {
                    ViewData["unit"] = "N";
                }
            }
            else
            {
                return RedirectToAction("Forbidden", "Account");
            }
            return View();
        }
        public IActionResult GetUnit(int id)
        {
            string[] data = new string[1];
            if (User.Identity.IsAuthenticated)
            {
                if (id == 1)
                {
                    data[0] = "'C";
                }
                else if (id == 2)
                {
                    data[0] = "%";
                }
                else if (id == 3)
                {
                    data[0] = "lx";
                }
                else
                {
                    data[0] = "K";
                }
            }
            else
            {
                DbSet<Preference> dbs1 = _dbContext.Preference;
                int userId = 0;
                Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
                Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
                if (id == 1)
                {
                    if (model1.TempUnit == 2)
                    {
                        data[0] = "K";
                    }
                    else if (model1.TempUnit == 3)
                    {
                        data[0] = "'F";
                    }
                    else
                    {
                        data[0] = "'C";
                    }
                }
                else if (id == 2)
                {
                    data[0] = "%";
                }
                else if (id == 3)
                {
                    data[0] = "lx";
                }
                else if (id == 4)
                {
                    if (model1.TempUnit == 2)
                    {
                        data[0] = "g";
                    }
                    else if (model1.TempUnit == 3)
                    {
                        data[0] = "kg";
                    }
                    else if (model1.TempUnit == 4)
                    {
                        data[0] = "lb";
                    }
                    else
                    {
                        data[0] = "K";
                    }
                }
            }
            return Json(data);
            
        }
        public IActionResult GetDataTemp()
        {
            //Random gen = new Random();
            //string[] data = new string[1];
            //data[0] = string.Format("{0:0.00}", Convert.ToDouble(gen.Next(1, 5)));
            //return Json(data);
            //DbSet<Temperature> dbs = _dbContext.Temperature;
            //dynamic temp = dbs.Select(p => p.Temperature).LastOrDefault();
            //temp = string.Format("{0:0.00}", Convert.ToDouble(temp));
            //data[0] = temp;
            //return Json(data);

            DbSet<Temperature> dbs = _dbContext.Temperature;
            Temperature model = dbs.OrderBy(p => p.TDatetime).LastOrDefault();
            if (model != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    DbSet<Preference> dbs1 = _dbContext.Preference;
                    int userId = 0;
                    Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
                    Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
                    if (model1.TempUnit == 2)
                    {
                        return Json(string.Format("{0:0.00}", Convert.ToDouble(model.TLevel + 273.15)));
                    }
                    else if (model1.TempUnit == 3)
                    {
                        return Json(string.Format("{0:0.00}", Convert.ToDouble((model.TLevel * 1.8) + 32)));
                    }
                    else
                    {
                        return Json(string.Format("{0:0.00}", Convert.ToDouble(model.TLevel)));
                    }
                }
                else
                {
                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.TLevel)));
                }
            }
            else
            {
                return Json("0.00");
            }
            
        }

        public IActionResult GetDataTempTime()
        {
            //string[] model = new string[1];
            //DbSet<Record> dbs = _dbContext.Record;
            //dynamic time = dbs.Select(p => p.RDatetime).LastOrDefault();
            //time = string.Format("{0:hh:mm:ss tt}", time);
            //model[0] = time;
            //model[0] = string.Format("{0:hh:mm:ss tt}", DateTime.Now);
            //return Json(model);
            DbSet<Temperature> dbs = _dbContext.Temperature;
            Temperature model = dbs.OrderBy(p => p.TDatetime).LastOrDefault();
            if (model != null)
            {
                return Json(string.Format("{0:hh:mm:ss tt}", model.TDatetime));
            }
            else
            {
                return Json(string.Format("{0:hh:mm:ss tt}", DateTime.Now));
            }
        }

        

        public IActionResult GetDataHum()
        {
            //Random gen = new Random();
            //string[] data = new string[1];
            //for (int i = 0; i < data.Length; i++)
            //{
            //    data[i] = string.Format("{0:0.00}",Convert.ToDouble(gen.Next(0, 40)));
            //}
            //data[0] = string.Format("{0:0.00}", Convert.ToDouble(gen.Next(6, 10)));
            //return Json(data);
            //DbSet<Temperature> dbs = _dbContext.Temperature;
            //dynamic temp = dbs.Select(p => p.Temperature).LastOrDefault();
            //temp = string.Format("{0:0.00}", Convert.ToDouble(temp));
            //data[0] = temp;
            //return Json(data);
            DbSet<Humidity> dbs = _dbContext.Humidity;
            Humidity model = dbs.OrderBy(p => p.HDatetime).LastOrDefault();
            if (model != null)
            {
                return Json(string.Format("{0:0.00}", Convert.ToDouble(model.HLevel)));
            }
            else
            {
                return Json("0.00");
            }
        }

        public IActionResult GetDataHumTime()
        {
            //string[] model = new string[1];
            //model[0] = string.Format("{0:hh:mm:ss tt}", DateTime.Now);
            //return Json(model);
            //DbSet<Record> dbs = _dbContext.Record;
            //dynamic time = dbs.Select(p => p.RDatetime).LastOrDefault();
            //time = string.Format("{0:hh:mm:ss tt}", time);
            //model[0] = time;
            DbSet<Humidity> dbs = _dbContext.Humidity;
            Humidity model = dbs.OrderBy(p => p.HDatetime).LastOrDefault();
            if (model != null)
            {
                return Json(string.Format("{0:hh:mm:ss tt}", model.HDatetime));
            }
            else
            {
                return Json(string.Format("{0:hh:mm:ss tt}", DateTime.Now));
            }

        }

        public IActionResult GetDataLight()
        {
            //Random gen = new Random();
            //string[] data = new string[1];
            //data[0] = string.Format("{0:0.00}", Convert.ToDouble(gen.Next(11, 15)));
            //return Json(data);
            DbSet<Light> dbs = _dbContext.Light;
            Light model = dbs.OrderBy(p => p.LDatetime).LastOrDefault();
            if (model != null)
            {
                return Json(string.Format("{0:0.00}", Convert.ToDouble(model.LLevel)));
            }
            else
            {
                return Json("0.00");
            }
            
        }

        public IActionResult GetDataLightTime()
        {
            //string[] model = new string[1];
            //model[0] = string.Format("{0:hh:mm:ss tt}", DateTime.Now);
            //return Json(model);
            DbSet<Light> dbs = _dbContext.Light;
            Light model = dbs.OrderBy(p => p.LDatetime).LastOrDefault();
            if (model != null)
            {
                return Json(string.Format("{0:hh:mm:ss tt}", model.LDatetime));
            }
            else
            {
                return Json(string.Format("{0:hh:mm:ss tt}", DateTime.Now));
            }
            
        }

        public IActionResult GetDataWeight()
        {
            //Random gen = new Random();
            //string[] data = new string[1];
            //data[0] = string.Format("{0:0.00}", Convert.ToDouble(gen.Next(15, 20)));
            //return Json(data);
            DbSet<Weight> dbs = _dbContext.Weight;
            Weight model = dbs.OrderBy(p => p.WDatetime).LastOrDefault();
            if (model != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    DbSet<Preference> dbs1 = _dbContext.Preference;
                    int userId = 0;
                    Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
                    Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
                    if (model1.WeightUnit == 2)
                    {
                        return Json(string.Format("{0:0.00}", Convert.ToDouble(model.WLevel * 101.97)));
                    }
                    else if (model1.WeightUnit == 3)
                    {
                        return Json(string.Format("{0:0.00}", Convert.ToDouble(model.WLevel * 0.101972)));
                    }
                    else if (model1.WeightUnit == 4)
                    {
                        return Json(string.Format("{0:0.00}", Convert.ToDouble(model.WLevel * 0.224809)));
                    }
                    else
                    {
                        return Json(string.Format("{0:0.00}", Convert.ToDouble(model.WLevel)));
                    }
                }
                else
                {
                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.WLevel)));
                }
            }
            else
            {
                return Json("0.00");
            }
            
        }

        public IActionResult GetDataWeightTime()
        {
            //string[] model = new string[1];
            //model[0] = string.Format("{0:hh:mm:ss tt}", DateTime.Now);
            //return Json(model);
            DbSet<Weight> dbs = _dbContext.Weight;
            Weight model = dbs.OrderBy(p => p.WDatetime).LastOrDefault();
            if (model != null)
            {
                return Json(string.Format("{0:hh:mm:ss tt}", model.WDatetime));
            }
            else
            {
                return Json(string.Format("{0:hh:mm:ss tt}", DateTime.Now));
            }
            

        }

        [Authorize]
        public IActionResult GetAvgTempTdy()
        {
            DbSet<Preference> dbs1 = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Temperature> dbs = _dbContext.Temperature;
            var model = dbs.Where(p => p.TDatetime.Date == DateTime.Now.Date).Select(p => p.TLevel);
            if (model.Count() != 0)
            {
                if (model1.TempUnit == 2)
                {

                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Average() + 273.15)));
                }
                else if (model1.TempUnit == 3)
                {
                    return Json(string.Format("{0:0.00}", Convert.ToDouble((model.Average() * 1.8) + 32)));
                }
                else
                {
                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Average())));
                }
            }
            else
            {
                return Json("0.00");
            }

        }

        [Authorize]
        public IActionResult GetHighestTempTdy()
        {
            DbSet<Preference> dbs1 = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Temperature> dbs = _dbContext.Temperature;
            var model = dbs.Where(p => p.TDatetime.Date == DateTime.Now.Date).Select(p => p.TLevel);
            if (model.Count() != 0)
            {
                if (model1.TempUnit == 2)
                {
                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Max() + 273.15)));
                }
                else if (model1.TempUnit == 3)
                {
                    return Json(string.Format("{0:0.00}", Convert.ToDouble((model.Max() * 1.8) + 32)));
                }
                else
                {
                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Max())));
                }
            }
            else
            {
                return Json("0.00");
            }

        }

        [Authorize]
        public IActionResult GetLowestTempTdy()
        {
            DbSet<Preference> dbs1 = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Temperature> dbs = _dbContext.Temperature;
            var model = dbs.Where(p => p.TDatetime.Date == DateTime.Now.Date).Select(p => p.TLevel);
            if (model.Count() != 0)
            {
                if (model1.TempUnit == 2)
                {
                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Min() + 273.15)));
                }
                else if (model1.TempUnit == 3)
                {
                    return Json(string.Format("{0:0.00}", Convert.ToDouble((model.Min() * 1.8) + 32)));
                }
                else
                {
                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Min())));
                }
            }
            else
            {
                return Json("0.00");
            }

        }

        [Authorize]
        public IActionResult GetAvgHumTdy()
        {
            DbSet<Preference> dbs1 = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Humidity> dbs = _dbContext.Humidity;
            var model = dbs.Where(p => p.HDatetime.Date == DateTime.Now.Date).Select(p => p.HLevel);
            if (model.Count() != 0)
            {
                
                return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Average())));
            }
            else
            {
                return Json("0.00");
            }

        }

        [Authorize]
        public IActionResult GetHighestHumTdy()
        {
            DbSet<Preference> dbs1 = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Humidity> dbs = _dbContext.Humidity;
            var model = dbs.Where(p => p.HDatetime.Date == DateTime.Now.Date).Select(p => p.HLevel);
            if (model.Count() != 0)
            {
               
                return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Max())));
            }
            else
            {
                return Json("0.00");
            }

        }

        [Authorize]
        public IActionResult GetLowestHumTdy()
        {
            DbSet<Preference> dbs1 = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Humidity> dbs = _dbContext.Humidity;
            var model = dbs.Where(p => p.HDatetime.Date == DateTime.Now.Date).Select(p => p.HLevel);
            if (model.Count() != 0)
            {
                
                return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Min())));
            }
            else
            {
                return Json("0.00");
            }

        }

        [Authorize]
        public IActionResult GetAvgLightTdy()
        {
            DbSet<Preference> dbs1 = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Light> dbs = _dbContext.Light;
            var model = dbs.Where(p => p.LDatetime.Date == DateTime.Now.Date).Select(p => p.LLevel);
            if (model.Count() != 0)
            {

                return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Average())));
            }
            else
            {
                return Json("0.00");
            }

        }

        [Authorize]
        public IActionResult GetHighestLightTdy()
        {
            DbSet<Preference> dbs1 = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Light> dbs = _dbContext.Light;
            var model = dbs.Where(p => p.LDatetime.Date == DateTime.Now.Date).Select(p => p.LLevel);
            if (model.Count() != 0)
            {

                return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Max())));
            }
            else
            {
                return Json("0.00");
            }

        }

        [Authorize]
        public IActionResult GetLowestLightTdy()
        {
            DbSet<Preference> dbs1 = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Light> dbs = _dbContext.Light;
            var model = dbs.Where(p => p.LDatetime.Date == DateTime.Now.Date).Select(p => p.LLevel);
            if (model.Count() != 0)
            {

                return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Min())));
            }
            else
            {
                return Json("0.00");
            }

        }

        [Authorize]
        public IActionResult GetAvgWeightTdy()
        {
            DbSet<Preference> dbs1 = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Weight> dbs = _dbContext.Weight;
            var model = dbs.Where(p => p.WDatetime.Date == DateTime.Now.Date).Select(p => p.WLevel);
            if (model.Count() != 0)
            {
                if (model1.WeightUnit == 2)
                {

                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Average() * 101.97)));
                }
                else if (model1.WeightUnit == 3)
                {
                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Average() * 0.101972)));
                }
                else if (model1.WeightUnit == 4){
                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Average() * 0.224809)));
                }
                else
                {
                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Average())));
                }
            }
            else
            {
                return Json("0.00");
            }

        }

        [Authorize]
        public IActionResult GetHighestWeightTdy()
        {
            DbSet<Preference> dbs1 = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Temperature> dbs = _dbContext.Temperature;
            var model = dbs.Where(p => p.TDatetime.Date == DateTime.Now.Date).Select(p => p.TLevel);
            if (model.Count() != 0)
            {
                if (model1.WeightUnit == 2)
                {

                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Max() * 101.97)));
                }
                else if (model1.WeightUnit == 3)
                {
                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Max() * 0.101972)));
                }
                else if (model1.WeightUnit == 4)
                {
                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Max() * 0.224809)));
                }
                else
                {
                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Max())));
                }
            }
            else
            {
                return Json("0.00");
            }

        }

        [Authorize]
        public IActionResult GetLowestWeightTdy()
        {
            DbSet<Preference> dbs1 = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Temperature> dbs = _dbContext.Temperature;
            var model = dbs.Where(p => p.TDatetime.Date == DateTime.Now.Date).Select(p => p.TLevel);
            if (model.Count() != 0)
            {
                if (model1.WeightUnit == 2)
                {

                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Min() * 101.97)));
                }
                else if (model1.WeightUnit == 3)
                {
                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Min() * 0.101972)));
                }
                else if (model1.WeightUnit == 4)
                {
                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Min() * 0.224809)));
                }
                else
                {
                    return Json(string.Format("{0:0.00}", Convert.ToDouble(model.Min())));
                }
            }
            else
            {
                return Json("0.00");
            }

        }
    }
}