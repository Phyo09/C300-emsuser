using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using C300.Models;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;


namespace C300.Controllers
{
    public class PastHistoryController : Controller
    {
        private AppDbContext _dbContext;
        private IHttpContextAccessor _contextAccessor;
        private HttpContext _context { get { return _contextAccessor.HttpContext; } }

        public PastHistoryController(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
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

        [Authorize]
        public IActionResult Dashboard()
        {
            BacklogActivity("Accessed Dashboard");
            PreparePreference();
            PrepareToday();
            PrepareYesterday();
            PrepareLastWeek();
            PrepareLast15Days();
            PrepareLastMonth();
            PrepareUnit();
            PrepareTableLimit();
            PrepareYesAHL();
            PrepareLWAHL();
            PrepareL15AHL();
            PrepareLMAHL();
            return View();
        }
        private void PrepareToday()
        {
            DbSet<Preference> dbs = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model = dbs.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Temperature> dbs1 = _dbContext.Temperature;
            List<double> temp = new List<double>();
            List<string> tHour = new List<string>();
            if (model.TempUnit == 2)
            {
                var model1 = dbs1.ToList()
                    .Where(p => p.TDatetime.Date == DateTime.Now.Date)
                    .GroupBy(p => p.TDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.TLevel + 273.15);
                    return d;
                });
                if (model1 != null)
                {
                    ViewData["modelT1"] = model1.Select(g => g.average);
                }
            }
            else if (model.TempUnit == 3)
            {
                var model1 = dbs1.ToList()
                    .Where(p => p.TDatetime.Date == DateTime.Now.Date)
                    .GroupBy(p => p.TDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => (i.TLevel * 1.8) + 32);
                    return d;
                });
                if (model1 != null)
                {
                    ViewData["modelT1"] = model1.Select(g => g.average);
                }
            }
            else
            {
                var model1 = dbs1.ToList()
                    .Where(p => p.TDatetime.Date == DateTime.Now.Date)
                    .GroupBy(p => p.TDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.TLevel);
                    return d;
                });
                if (model1 != null)
                {
                    ViewData["modelT1"] = model1.Select(g => g.average);
                }
            }


            DbSet<Humidity> dbs2 = _dbContext.Humidity;
            List<double> hum = new List<double>();
            List<string> hHour = new List<string>();
            var model2 = dbs2.ToList()
                    .Where(p => p.HDatetime.Date == DateTime.Now.Date)
                    .GroupBy(p => p.HDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.HLevel);
                    return d;
                });
            if (model2 != null)
            {
                ViewData["modelT2"] = model2.Select(g => g.average);
                ViewData["hourT"] = model2.Select(g => g.id);
            }

            DbSet<Light> dbs3 = _dbContext.Light;
            List<double> light = new List<double>();
            List<string> lHour = new List<string>();
            var model3 = dbs3.ToList()
                    .Where(p => p.LDatetime.Date == DateTime.Now.Date)
                    .GroupBy(p => p.LDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.LLevel);
                    return d;
                });
            if (model3 != null)
            {
                ViewData["modelT3"] = model3.Select(g => g.average);
            }

            DbSet<Weight> dbs4 = _dbContext.Weight;
            List<double> weight = new List<double>();
            List<string> wHour = new List<string>();
            if (model.WeightUnit == 2)
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date == DateTime.Now.Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel * 101.97);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelT4"] = model4.Select(g => g.average);
                }
            }
            else if (model.WeightUnit == 3)
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date == DateTime.Now.Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel * 0.101972);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelT4"] = model4.Select(g => g.average);
                }
            }
            else if (model.WeightUnit == 4)
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date == DateTime.Now.Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelT4"] = model4.Select(g => g.average * 0.224809);
                }
            }
            else
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date == DateTime.Now.Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelT4"] = model4.Select(g => g.average);
                }
            }

        }

        private void PrepareYesterday()
        {
            DbSet<Preference> dbs = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model = dbs.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Temperature> dbs1 = _dbContext.Temperature;
            List<double> temp = new List<double>();
            List<string> tHour = new List<string>();
            if (model.TempUnit == 2)
            {
                var model1 = dbs1.ToList()
                    .Where(p => p.TDatetime.Date == DateTime.Now.AddDays(-1).Date)
                    .GroupBy(p => p.TDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.TLevel + 273.15);
                    return d;
                });
                if (model1 != null)
                {
                    ViewData["modelY1"] = model1.Select(g => g.average);
                }
            }
            else if (model.TempUnit == 3)
            {
                var model1 = dbs1.ToList()
                    .Where(p => p.TDatetime.Date == DateTime.Now.AddDays(-1).Date)
                    .GroupBy(p => p.TDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => (i.TLevel * 1.8) + 32);
                    return d;
                });
                if (model1 != null)
                {
                    ViewData["modelY1"] = model1.Select(g => g.average);
                }
            }
            else
            {
                var model1 = dbs1.ToList()
                    .Where(p => p.TDatetime.Date == DateTime.Now.AddDays(-1).Date)
                    .GroupBy(p => p.TDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.TLevel);
                    return d;
                });
                if (model1 != null)
                {
                    ViewData["modelY1"] = model1.Select(g => g.average);
                }
            }


            DbSet<Humidity> dbs2 = _dbContext.Humidity;
            List<double> hum = new List<double>();
            List<string> hHour = new List<string>();
            var model2 = dbs2.ToList()
                    .Where(p => p.HDatetime.Date == DateTime.Now.AddDays(-1).Date)
                    .GroupBy(p => p.HDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.HLevel);
                    return d;
                });
            if (model2 != null)
            {
                ViewData["modelY2"] = model2.Select(g => g.average);
                ViewData["hourY"] = model2.Select(g => g.id);
            }

            DbSet<Light> dbs3 = _dbContext.Light;
            List<double> light = new List<double>();
            List<string> lHour = new List<string>();
            var model3 = dbs3.ToList()
                    .Where(p => p.LDatetime.Date == DateTime.Now.AddDays(-1).Date)
                    .GroupBy(p => p.LDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.LLevel);
                    return d;
                });
            if (model3 != null)
            {
                ViewData["modelY3"] = model3.Select(g => g.average);
            }

            DbSet<Weight> dbs4 = _dbContext.Weight;
            List<double> weight = new List<double>();
            List<string> wHour = new List<string>();
            if (model.WeightUnit == 2)
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date == DateTime.Now.AddDays(-1).Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel * 101.97);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelY4"] = model4.Select(g => g.average);
                }
            }
            else if (model.WeightUnit == 3)
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date == DateTime.Now.AddDays(-1).Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel * 0.101972);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelY4"] = model4.Select(g => g.average);
                }
            }
            else if (model.WeightUnit == 4)
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date == DateTime.Now.AddDays(-1).Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelY4"] = model4.Select(g => g.average * 0.224809);
                }
            }
            else
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date == DateTime.Now.AddDays(-1).Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelY4"] = model4.Select(g => g.average);
                }
            }

        }

        private void PrepareLastWeek()
        {
            DbSet<Preference> dbs = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model = dbs.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Temperature> dbs1 = _dbContext.Temperature;
            List<double> temp = new List<double>();
            List<string> tHour = new List<string>();
            if (model.TempUnit == 2)
            {
                var model1 = dbs1.ToList()
                    .Where(p => p.TDatetime.Date >= DateTime.Now.AddDays(-7).Date && p.TDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.TDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.TLevel + 273.15);
                    return d;
                });
                if (model1 != null)
                {
                    ViewData["modelW1"] = model1.Select(g => g.average);
                }
            }
            else if (model.TempUnit == 3)
            {
                var model1 = dbs1.ToList()
                    .Where(p => p.TDatetime.Date >= DateTime.Now.AddDays(-7).Date && p.TDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.TDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => (i.TLevel * 1.8) + 32);
                    return d;
                });
                if (model1 != null)
                {
                    ViewData["modelW1"] = model1.Select(g => g.average);
                }
            }
            else
            {
                var model1 = dbs1.ToList()
                    .Where(p => p.TDatetime.Date >= DateTime.Now.AddDays(-7).Date && p.TDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.TDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.TLevel);
                    return d;
                });
                if (model1 != null)
                {
                    ViewData["modelW1"] = model1.Select(g => g.average);
                }
            }


            DbSet<Humidity> dbs2 = _dbContext.Humidity;
            List<double> hum = new List<double>();
            List<string> hHour = new List<string>();
            var model2 = dbs2.ToList()
                    .Where(p => p.HDatetime.Date >= DateTime.Now.AddDays(-7).Date && p.HDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.HDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.HLevel);
                    return d;
                });
            if (model2 != null)
            {
                ViewData["modelW2"] = model2.Select(g => g.average);
                ViewData["hourW"] = model2.Select(g => g.id);
            }

            DbSet<Light> dbs3 = _dbContext.Light;
            List<double> light = new List<double>();
            List<string> lHour = new List<string>();
            var model3 = dbs3.ToList()
                    .Where(p => p.LDatetime.Date >= DateTime.Now.AddDays(-7).Date && p.LDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.LDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.LLevel);
                    return d;
                });
            if (model3 != null)
            {
                ViewData["modelW3"] = model3.Select(g => g.average);
            }

            DbSet<Weight> dbs4 = _dbContext.Weight;
            List<double> weight = new List<double>();
            List<string> wHour = new List<string>();
            if (model.WeightUnit == 2)
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date >= DateTime.Now.AddDays(-7).Date && p.WDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel * 101.97);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelW4"] = model4.Select(g => g.average);
                }
            }
            else if (model.WeightUnit == 3)
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date >= DateTime.Now.AddDays(-7).Date && p.WDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel * 0.101972);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelW4"] = model4.Select(g => g.average);
                }
            }
            else if (model.WeightUnit == 4)
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date >= DateTime.Now.AddDays(-7).Date && p.WDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelW4"] = model4.Select(g => g.average * 0.224809);
                }
            }
            else
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date >= DateTime.Now.AddDays(-7).Date && p.WDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelW4"] = model4.Select(g => g.average);
                }
            }

        }

        private void PrepareLast15Days()
        {
            DbSet<Preference> dbs = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model = dbs.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Temperature> dbs1 = _dbContext.Temperature;
            List<double> temp = new List<double>();
            List<string> tHour = new List<string>();
            if (model.TempUnit == 2)
            {
                var model1 = dbs1.ToList()
                    .Where(p => p.TDatetime.Date >= DateTime.Now.AddDays(-15).Date && p.TDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.TDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.TLevel + 273.15);
                    return d;
                });
                if (model1 != null)
                {
                    ViewData["modelF1"] = model1.Select(g => g.average);
                }
            }
            else if (model.TempUnit == 3)
            {
                var model1 = dbs1.ToList()
                    .Where(p => p.TDatetime.Date >= DateTime.Now.AddDays(-15).Date && p.TDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.TDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => (i.TLevel * 1.8) + 32);
                    return d;
                });
                if (model1 != null)
                {
                    ViewData["modelF1"] = model1.Select(g => g.average);
                }
            }
            else
            {
                var model1 = dbs1.ToList()
                    .Where(p => p.TDatetime.Date >= DateTime.Now.AddDays(-15).Date && p.TDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.TDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.TLevel);
                    return d;
                });
                if (model1 != null)
                {
                    ViewData["modelF1"] = model1.Select(g => g.average);
                }
            }


            DbSet<Humidity> dbs2 = _dbContext.Humidity;
            List<double> hum = new List<double>();
            List<string> hHour = new List<string>();
            var model2 = dbs2.ToList()
                    .Where(p => p.HDatetime.Date >= DateTime.Now.AddDays(-15).Date && p.HDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.HDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.HLevel);
                    return d;
                });
            if (model2 != null)
            {
                ViewData["modelF2"] = model2.Select(g => g.average);
                ViewData["hourF"] = model2.Select(g => g.id);
            }

            DbSet<Light> dbs3 = _dbContext.Light;
            List<double> light = new List<double>();
            List<string> lHour = new List<string>();
            var model3 = dbs3.ToList()
                    .Where(p => p.LDatetime.Date >= DateTime.Now.AddDays(-15).Date && p.LDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.LDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.LLevel);
                    return d;
                });
            if (model3 != null)
            {
                ViewData["modelF3"] = model3.Select(g => g.average);
            }

            DbSet<Weight> dbs4 = _dbContext.Weight;
            List<double> weight = new List<double>();
            List<string> wHour = new List<string>();
            if (model.WeightUnit == 2)
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date >= DateTime.Now.AddDays(-15).Date && p.WDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel * 101.97);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelF4"] = model4.Select(g => g.average);
                }
            }
            else if (model.WeightUnit == 3)
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date >= DateTime.Now.AddDays(-15).Date && p.WDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel * 0.101972);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelF4"] = model4.Select(g => g.average);
                }
            }
            else if (model.WeightUnit == 4)
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date >= DateTime.Now.AddDays(-15).Date && p.WDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelF4"] = model4.Select(g => g.average * 0.224809);
                }
            }
            else
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date >= DateTime.Now.AddDays(-15).Date && p.WDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelF4"] = model4.Select(g => g.average);
                }
            }

        }

        private void PrepareLastMonth()
        {
            DbSet<Preference> dbs = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model = dbs.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Temperature> dbs1 = _dbContext.Temperature;
            List<double> temp = new List<double>();
            List<string> tHour = new List<string>();
            if (model.TempUnit == 2)
            {
                var model1 = dbs1.ToList()
                    .Where(p => p.TDatetime.Date >= DateTime.Now.AddMonths(-1).Date && p.TDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.TDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.TLevel + 273.15);
                    return d;
                });
                if (model1 != null)
                {
                    ViewData["modelM1"] = model1.Select(g => g.average);
                }
            }
            else if (model.TempUnit == 3)
            {
                var model1 = dbs1.ToList()
                    .Where(p => p.TDatetime.Date >= DateTime.Now.AddMonths(-1).Date && p.TDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.TDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => (i.TLevel * 1.8) + 32);
                    return d;
                });
                if (model1 != null)
                {
                    ViewData["modelM1"] = model1.Select(g => g.average);
                }
            }
            else
            {
                var model1 = dbs1.ToList()
                    .Where(p => p.TDatetime.Date >= DateTime.Now.AddMonths(-1).Date && p.TDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.TDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.TLevel);
                    return d;
                });
                if (model1 != null)
                {
                    ViewData["modelM1"] = model1.Select(g => g.average);
                }
            }


            DbSet<Humidity> dbs2 = _dbContext.Humidity;
            List<double> hum = new List<double>();
            List<string> hHour = new List<string>();
            var model2 = dbs2.ToList()
                    .Where(p => p.HDatetime.Date >= DateTime.Now.AddMonths(-1).Date && p.HDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.HDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.HLevel);
                    return d;
                });
            if (model2 != null)
            {
                ViewData["modelM2"] = model2.Select(g => g.average);
                ViewData["hourM"] = model2.Select(g => g.id);
            }

            DbSet<Light> dbs3 = _dbContext.Light;
            List<double> light = new List<double>();
            List<string> lHour = new List<string>();
            var model3 = dbs3.ToList()
                    .Where(p => p.LDatetime.Date >= DateTime.Now.AddMonths(-1).Date && p.LDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.LDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.LLevel);
                    return d;
                });
            if (model3 != null)
            {
                ViewData["modelM3"] = model3.Select(g => g.average);
            }

            DbSet<Weight> dbs4 = _dbContext.Weight;
            List<double> weight = new List<double>();
            List<string> wHour = new List<string>();
            if (model.WeightUnit == 2)
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date >= DateTime.Now.AddMonths(-1).Date && p.WDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel * 101.97);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelM4"] = model4.Select(g => g.average);
                }
            }
            else if (model.WeightUnit == 3)
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date >= DateTime.Now.AddMonths(-1).Date && p.WDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel * 0.101972);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelM4"] = model4.Select(g => g.average);
                }
            }
            else if (model.WeightUnit == 4)
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date >= DateTime.Now.AddMonths(-1).Date && p.WDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelM4"] = model4.Select(g => g.average * 0.224809);
                }
            }
            else
            {
                var model4 = dbs4.ToList()
                    .Where(p => p.WDatetime.Date >= DateTime.Now.AddMonths(-1).Date && p.WDatetime.Date < DateTime.Now.Date)
                    .GroupBy(p => p.WDatetime.Hour)
                    .OrderBy(g => g.Key)
                .Select(g =>
                {
                    dynamic d = new ExpandoObject();
                    d.id = g.Key;
                    d.average = g.Average(i => i.WLevel);
                    return d;
                });
                if (model4 != null)
                {
                    ViewData["modelM4"] = model4.Select(g => g.average);
                }
            }

        }

        private void PrepareUnit()
        {
            DbSet<Preference> dbs1 = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();

            if (model1.TempUnit == 2)
            {
                ViewData["unit1"] = "K";
            }
            else if (model1.TempUnit == 3)
            {
                ViewData["unit1"] = "'F";
            }
            else
            {
                ViewData["unit1"] = "'C";
            }
            if (model1.HumUnit == 1)
            {
                ViewData["unit2"] = "%";
            }
            if (model1.LightUnit == 1)
            {
                ViewData["unit3"] = "lx";
            }

            if (model1.WeightUnit == 2)
            {
                ViewData["unit4"] = "g";
            }
            else if (model1.WeightUnit == 3)
            {
                ViewData["unit4"] = "kg";
            }
            else if (model1.WeightUnit == 4)
            {
                ViewData["unit4"] = "lb";
            }
            else
            {
                ViewData["unit4"] = "N";
            }

        }

        private void PrepareTableLimit()
        {
            DbSet<Preference> dbs = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model = dbs.Where(p => p.UserId == userId).FirstOrDefault();
            DbSet<Temperature> dbs1 = _dbContext.Temperature;
            DbSet<Humidity> dbs2 = _dbContext.Humidity;
            DbSet<Light> dbs3 = _dbContext.Light;
            DbSet<Weight> dbs4 = _dbContext.Weight;
            ViewData["OTH"] = dbs1.Where(p => p.TLevel > model.HighestTemp).OrderByDescending(p => p.TDatetime).ToList();
            ViewData["OTL"] = dbs1.Where(p => p.TLevel < model.LowestTemp).OrderByDescending(p => p.TDatetime).ToList();
            ViewData["highT"] = dbs.Select(p => p.HighestTemp).FirstOrDefault();
            ViewData["lowT"] = dbs.Select(p => p.LowestTemp).FirstOrDefault();

            ViewData["OHH"] = dbs2.Where(p => p.HLevel > model.HighestHumidity).OrderByDescending(p => p.HDatetime).ToList();
            ViewData["OHL"] = dbs2.Where(p => p.HLevel < model.LowestHumidity).OrderByDescending(p => p.HDatetime).ToList();
            ViewData["highH"] = dbs.Select(p => p.HighestHumidity).FirstOrDefault();
            ViewData["lowH"] = dbs.Select(p => p.LowestHumidity).FirstOrDefault();

            ViewData["OLH"] = dbs3.Where(p => p.LLevel > model.HighestLight).OrderByDescending(p => p.LDatetime).ToList();
            ViewData["OLL"] = dbs3.Where(p => p.LLevel < model.LowestLight).OrderByDescending(p => p.LDatetime).ToList();
            ViewData["highL"] = dbs.Select(p => p.HighestLight).FirstOrDefault();
            ViewData["lowL"] = dbs.Select(p => p.LowestLight).FirstOrDefault();

            ViewData["OWH"] = dbs4.Where(p => p.WLevel > model.HighestWeight).OrderByDescending(p => p.WDatetime).ToList();
            ViewData["OWL"] = dbs4.Where(p => p.WLevel < model.LowestWeight).OrderByDescending(p => p.WDatetime).ToList();
            ViewData["highW"] = dbs.Select(p => p.HighestWeight).FirstOrDefault();
            ViewData["lowW"] = dbs.Select(p => p.LowestWeight).FirstOrDefault();
        }

        private void PrepareYesAHL()
        {
            DbSet<Preference> dbs = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model = dbs.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Temperature> dbs1 = _dbContext.Temperature;
            var model1 = dbs1.Where(p => p.TDatetime.Date == DateTime.Now.AddDays(-1).Date).Select(p => p.TLevel);
            if (model1.Count() != 0)
            {
                if (model.TempUnit == 2)
                {
                    ViewData["ATY"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average() + 273.15));
                    ViewData["HTY"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max() + 273.15));
                    ViewData["LTY"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min() + 273.15));
                }
                else if (model.TempUnit == 3)
                {
                    ViewData["ATY"] = string.Format("{0:0.00}", Convert.ToDouble((model1.Average() * 1.8) + 32));
                    ViewData["HTY"] = string.Format("{0:0.00}", Convert.ToDouble((model1.Max() * 1.8) + 32));
                    ViewData["LTY"] = string.Format("{0:0.00}", Convert.ToDouble((model1.Min() * 1.8) + 32));
                }
                else
                {
                    ViewData["ATY"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average()));
                    ViewData["HTY"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max()));
                    ViewData["LTY"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min()));
                }
            }
            else
            {
                ViewData["ATY"] = "0.00";
                ViewData["HTY"] = "0.00";
                ViewData["LTY"] = "0.00";
            }

            DbSet<Humidity> dbs2 = _dbContext.Humidity;
            var model2 = dbs2.Where(p => p.HDatetime.Date == DateTime.Now.AddDays(-1).Date).Select(p => p.HLevel);
            if (model2.Count() != 0)
            {
                ViewData["AHY"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Average()));
                ViewData["HHY"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Max()));
                ViewData["LHY"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Min()));
            }
            else
            {
                ViewData["AHY"] = "0.00";
                ViewData["HHY"] = "0.00";
                ViewData["LHY"] = "0.00";
            }

            DbSet<Light> dbs3 = _dbContext.Light;
            var model3 = dbs3.Where(p => p.LDatetime.Date == DateTime.Now.AddDays(-1).Date).Select(p => p.LLevel);
            if (model3.Count() != 0)
            {
                ViewData["ALY"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Average()));
                ViewData["HLY"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Max()));
                ViewData["LLY"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Min()));
            }
            else
            {
                ViewData["ALY"] = "0.00";
                ViewData["HLY"] = "0.00";
                ViewData["LLY"] = "0.00";
            }

            DbSet<Weight> dbs4 = _dbContext.Weight;
            var model4 = dbs4.Where(p => p.WDatetime.Date == DateTime.Now.AddDays(-1).Date).Select(p => p.WLevel);
            if (model4.Count() != 0)
            {
                if (model.WeightUnit == 2)
                {
                    ViewData["AWY"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average() * 101.97));
                    ViewData["HWY"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max() * 101.97));
                    ViewData["LWY"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min() * 101.97));
                }
                else if (model.WeightUnit == 3)
                {
                    ViewData["AWY"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average() * 0.101972));
                    ViewData["HWY"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max() * 0.101972));
                    ViewData["LWY"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min() * 0.101972));
                }
                else if (model.WeightUnit == 4)
                {
                    ViewData["AWY"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average() * 0.224809));
                    ViewData["HWY"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max() * 0.224809));
                    ViewData["LWY"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min() * 0.224809));
                }
                else
                {
                    ViewData["AWY"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average()));
                    ViewData["HWY"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max()));
                    ViewData["LWY"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min()));
                }
            }
            else
            {
                ViewData["AWY"] = "0.00";
                ViewData["HWY"] = "0.00";
                ViewData["LWY"] = "0.00";
            }
        }

        private void PrepareLWAHL()
        {
            DbSet<Preference> dbs = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model = dbs.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Temperature> dbs1 = _dbContext.Temperature;
            var model1 = dbs1.Where(p => p.TDatetime.Date >= DateTime.Now.AddDays(-6).Date && p.TDatetime.Date <= DateTime.Now.Date)
                .Select(p => p.TLevel);
            if (model1.Count() != 0)
            {
                if (model.TempUnit == 2)
                {
                    ViewData["ATLW"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average() + 273.15));
                    ViewData["HTLW"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max() + 273.15));
                    ViewData["LTLW"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min() + 273.15));
                }
                else if (model.TempUnit == 3)
                {
                    ViewData["ATLW"] = string.Format("{0:0.00}", Convert.ToDouble((model1.Average() * 1.8) + 32));
                    ViewData["HTLW"] = string.Format("{0:0.00}", Convert.ToDouble((model1.Max() * 1.8) + 32));
                    ViewData["LTLW"] = string.Format("{0:0.00}", Convert.ToDouble((model1.Min() * 1.8) + 32));
                }
                else
                {
                    ViewData["ATLW"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average()));
                    ViewData["HTLW"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max()));
                    ViewData["LTLW"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min()));
                }
            }
            else
            {
                ViewData["ATLW"] = "0.00";
                ViewData["HTLW"] = "0.00";
                ViewData["LTLW"] = "0.00";
            }

            DbSet<Humidity> dbs2 = _dbContext.Humidity;
            var model2 = dbs2.Where(p => p.HDatetime.Date >= DateTime.Now.AddDays(-6).Date && p.HDatetime.Date <= DateTime.Now.Date)
                .Select(p => p.HLevel);
            if (model2.Count() != 0)
            {
                ViewData["AHLW"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Average()));
                ViewData["HHLW"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Max()));
                ViewData["LHLW"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Min()));
            }
            else
            {
                ViewData["AHLW"] = "0.00";
                ViewData["HHLW"] = "0.00";
                ViewData["LHLW"] = "0.00";
            }

            DbSet<Light> dbs3 = _dbContext.Light;
            var model3 = dbs3.Where(p => p.LDatetime.Date >= DateTime.Now.AddDays(-6).Date && p.LDatetime.Date <= DateTime.Now.Date)
                .Select(p => p.LLevel);
            if (model3.Count() != 0)
            {
                ViewData["ALLW"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Average()));
                ViewData["HLLW"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Max()));
                ViewData["LLLW"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Min()));
            }
            else
            {
                ViewData["ALLW"] = "0.00";
                ViewData["HLLW"] = "0.00";
                ViewData["LLLW"] = "0.00";
            }

            DbSet<Weight> dbs4 = _dbContext.Weight;
            var model4 = dbs4.Where(p => p.WDatetime.Date >= DateTime.Now.AddDays(-6).Date && p.WDatetime.Date <= DateTime.Now.Date)
                .Select(p => p.WLevel);
            if (model4.Count() != 0)
            {
                if (model.WeightUnit == 2)
                {
                    ViewData["AWLW"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average() * 101.97));
                    ViewData["HWLW"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max() * 101.97));
                    ViewData["LWLW"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min() * 101.97));
                }
                else if (model.WeightUnit == 3)
                {
                    ViewData["AWLW"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average() * 0.101972));
                    ViewData["HWLW"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max() * 0.101972));
                    ViewData["LWLW"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min() * 0.101972));
                }
                else if (model.WeightUnit == 4)
                {
                    ViewData["AWLW"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average() * 0.224809));
                    ViewData["HWLW"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max() * 0.224809));
                    ViewData["LWLW"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min() * 0.224809));
                }
                else
                {
                    ViewData["AWLW"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average()));
                    ViewData["HWLW"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max()));
                    ViewData["LWLW"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min()));
                }
            }
            else
            {
                ViewData["AWLW"] = "0.00";
                ViewData["HWLW"] = "0.00";
                ViewData["LWLW"] = "0.00";
            }
        }

        private void PrepareL15AHL()
        {
            DbSet<Preference> dbs = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model = dbs.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Temperature> dbs1 = _dbContext.Temperature;
            var model1 = dbs1.Where(p => p.TDatetime.Date >= DateTime.Now.AddDays(-14).Date && p.TDatetime.Date <= DateTime.Now.Date)
                .Select(p => p.TLevel);
            if (model1.Count() != 0)
            {
                if (model.TempUnit == 2)
                {
                    ViewData["ATL15"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average() + 273.15));
                    ViewData["HTL15"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max() + 273.15));
                    ViewData["LTL15"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min() + 273.15));
                }
                else if (model.TempUnit == 3)
                {
                    ViewData["ATL15"] = string.Format("{0:0.00}", Convert.ToDouble((model1.Average() * 1.8) + 32));
                    ViewData["HTL15"] = string.Format("{0:0.00}", Convert.ToDouble((model1.Max() * 1.8) + 32));
                    ViewData["LTL15"] = string.Format("{0:0.00}", Convert.ToDouble((model1.Min() * 1.8) + 32));
                }
                else
                {
                    ViewData["ATL15"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average()));
                    ViewData["HTL15"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max()));
                    ViewData["LTL15"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min()));
                }
            }
            else
            {
                ViewData["ATL15"] = "0.00";
                ViewData["HTL15"] = "0.00";
                ViewData["LTL15"] = "0.00";
            }

            DbSet<Humidity> dbs2 = _dbContext.Humidity;
            var model2 = dbs2.Where(p => p.HDatetime.Date >= DateTime.Now.AddDays(-14).Date && p.HDatetime.Date <= DateTime.Now.Date)
                .Select(p => p.HLevel);
            if (model2.Count() != 0)
            {
                ViewData["AHL15"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Average()));
                ViewData["HHL15"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Max()));
                ViewData["LHL15"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Min()));
            }
            else
            {
                ViewData["AHL15"] = "0.00";
                ViewData["HHL15"] = "0.00";
                ViewData["LHL15"] = "0.00";
            }

            DbSet<Light> dbs3 = _dbContext.Light;
            var model3 = dbs3.Where(p => p.LDatetime.Date >= DateTime.Now.AddDays(-14).Date && p.LDatetime.Date <= DateTime.Now.Date)
                .Select(p => p.LLevel);
            if (model3.Count() != 0)
            {
                ViewData["ALL15"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Average()));
                ViewData["HLL15"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Max()));
                ViewData["LLL15"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Min()));
            }
            else
            {
                ViewData["ALL15"] = "0.00";
                ViewData["HLL15"] = "0.00";
                ViewData["LLL15"] = "0.00";
            }

            DbSet<Weight> dbs4 = _dbContext.Weight;
            var model4 = dbs4.Where(p => p.WDatetime.Date >= DateTime.Now.AddDays(-14).Date && p.WDatetime.Date <= DateTime.Now.Date)
                .Select(p => p.WLevel);
            if (model4.Count() != 0)
            {
                if (model.WeightUnit == 2)
                {
                    ViewData["AWL15"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average() * 101.97));
                    ViewData["HWL15"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max() * 101.97));
                    ViewData["LWL15"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min() * 101.97));
                }
                else if (model.WeightUnit == 3)
                {
                    ViewData["AWL15"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average() * 0.101972));
                    ViewData["HWL15"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max() * 0.101972));
                    ViewData["LWL15"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min() * 0.101972));
                }
                else if (model.WeightUnit == 4)
                {
                    ViewData["AWL15"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average() * 0.224809));
                    ViewData["HWL15"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max() * 0.224809));
                    ViewData["LWL15"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min() * 0.224809));
                }
                else
                {
                    ViewData["AWL15"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average()));
                    ViewData["HWL15"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max()));
                    ViewData["LWL15"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min()));
                }
            }
            else
            {
                ViewData["AWL15"] = "0.00";
                ViewData["HWL15"] = "0.00";
                ViewData["LWL15"] = "0.00";
            }
        }

        private void PrepareLMAHL()
        {
            DbSet<Preference> dbs = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model = dbs.Where(p => p.UserId == userId).FirstOrDefault();

            DbSet<Temperature> dbs1 = _dbContext.Temperature;
            var model1 = dbs1.Where(p => p.TDatetime.Date >= DateTime.Now.AddMonths(-1).Date && p.TDatetime.Date <= DateTime.Now.Date)
                .Select(p => p.TLevel);
            if (model1.Count() != 0)
            {
                if (model.TempUnit == 2)
                {
                    ViewData["ATLM"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average() + 273.15));
                    ViewData["HTLM"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max() + 273.15));
                    ViewData["LTLM"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min() + 273.15));
                }
                else if (model.TempUnit == 3)
                {
                    ViewData["ATLM"] = string.Format("{0:0.00}", Convert.ToDouble((model1.Average() * 1.8) + 32));
                    ViewData["HTLM"] = string.Format("{0:0.00}", Convert.ToDouble((model1.Max() * 1.8) + 32));
                    ViewData["LTLM"] = string.Format("{0:0.00}", Convert.ToDouble((model1.Min() * 1.8) + 32));
                }
                else
                {
                    ViewData["ATLM"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average()));
                    ViewData["HTLM"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max()));
                    ViewData["LTLM"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min()));
                }
            }
            else
            {
                ViewData["ATLM"] = "0.00";
                ViewData["HTLM"] = "0.00";
                ViewData["LTLM"] = "0.00";
            }

            DbSet<Humidity> dbs2 = _dbContext.Humidity;
            var model2 = dbs2.Where(p => p.HDatetime.Date >= DateTime.Now.AddMonths(-1).Date && p.HDatetime.Date <= DateTime.Now.Date)
                .Select(p => p.HLevel);
            if (model2.Count() != 0)
            {
                ViewData["AHLM"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Average()));
                ViewData["HHLM"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Max()));
                ViewData["LHLM"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Min()));
            }
            else
            {
                ViewData["AHLM"] = "0.00";
                ViewData["HHLM"] = "0.00";
                ViewData["LHLM"] = "0.00";
            }

            DbSet<Light> dbs3 = _dbContext.Light;
            var model3 = dbs3.Where(p => p.LDatetime.Date >= DateTime.Now.AddMonths(-1).Date && p.LDatetime.Date <= DateTime.Now.Date)
                .Select(p => p.LLevel);
            if (model3.Count() != 0)
            {
                ViewData["ALLM"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Average()));
                ViewData["HLLM"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Max()));
                ViewData["LLLM"] = string.Format("{0:0.00}", Convert.ToDouble(model2.Min()));
            }
            else
            {
                ViewData["ALLM"] = "0.00";
                ViewData["HLLM"] = "0.00";
                ViewData["LLLM"] = "0.00";
            }

            DbSet<Weight> dbs4 = _dbContext.Weight;
            var model4 = dbs4.Where(p => p.WDatetime.Date >= DateTime.Now.AddMonths(-1).Date && p.WDatetime.Date <= DateTime.Now.Date)
                .Select(p => p.WLevel);
            if (model4.Count() != 0)
            {
                if (model.WeightUnit == 2)
                {
                    ViewData["AWLM"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average() * 101.97));
                    ViewData["HWLM"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max() * 101.97));
                    ViewData["LWLM"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min() * 101.97));
                }
                else if (model.WeightUnit == 3)
                {
                    ViewData["AWLM"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average() * 0.101972));
                    ViewData["HWLM"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max() * 0.101972));
                    ViewData["LWLM"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min() * 0.101972));
                }
                else if (model.WeightUnit == 4)
                {
                    ViewData["AWLM"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average() * 0.224809));
                    ViewData["HWLM"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max() * 0.224809));
                    ViewData["LWLM"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min() * 0.224809));
                }
                else
                {
                    ViewData["AWLM"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Average()));
                    ViewData["HWLM"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Max()));
                    ViewData["LWLM"] = string.Format("{0:0.00}", Convert.ToDouble(model1.Min()));
                }
            }
            else
            {
                ViewData["AWLM"] = "0.00";
                ViewData["HWLM"] = "0.00";
                ViewData["LWLM"] = "0.00";
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
        [HttpGet]
        public IActionResult Table(int id)
        {
            PreparePreference();
            ViewData["update"] = DateTime.Now;
            DbSet<Preference> dbs1 = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
            if (id == 1)
            {
                DbSet<Temperature> dbs = _dbContext.Temperature;
                List<Temperature> model = dbs.OrderByDescending(p => p.TDatetime).ToList();

                ViewData["Year"] = dbs.ToList()
                    .GroupBy(p => p.TDatetime.Year)
                    .OrderByDescending(g => g.Key)
                    .Select(o =>
                    {
                        dynamic d = new ExpandoObject();
                        d.value = o.Key;
                        d.text = o.Key;
                        return d;
                    })
                    .ToList<dynamic>();

                ViewData["High"] = model1.HighestTemp;
                ViewData["Low"] = model1.LowestTemp;
                ViewData["id"] = 1;
                ViewData["type"] = "Temperature";
                ViewData["Model"] = model;
                //ViewData["update"] = dbs.ToList()
                //    .OrderByDescending(p => p.TDatetime)
                //    .Select(p => p.TDatetime)
                //    .FirstOrDefault();
                if (model1.TempUnit == 1)
                {
                    ViewData["unit"] = "'C";
                }
                else if (model1.TempUnit == 2)
                {
                    ViewData["unit"] = "K";
                }
                else if (model1.TempUnit == 3)
                {
                    ViewData["unit"] = "'F";
                }
            }
            else if (id == 2)
            {
                DbSet<Humidity> dbs = _dbContext.Humidity;
                List<Humidity> model = dbs.OrderByDescending(p => p.HDatetime).ToList();
                ViewData["Year"] = dbs.ToList()
                    .GroupBy(p => p.HDatetime.Year)
                    .OrderByDescending(g => g.Key)
                    .Select(o =>
                    {
                        dynamic d = new ExpandoObject();
                        d.value = o.Key;
                        d.text = o.Key;
                        return d;
                    })
                    .ToList<dynamic>();
                ViewData["High"] = model1.HighestHumidity;
                ViewData["Low"] = model1.LowestHumidity;
                ViewData["id"] = 2;
                ViewData["type"] = "Humidity";
                ViewData["Model"] = model;
                //ViewData["update"] = dbs.ToList()
                //    .OrderByDescending(p => p.HDatetime)
                //    .Select(p => p.HDatetime)
                //    .FirstOrDefault();
                if (model1.HumUnit == 1)
                {
                    ViewData["unit"] = "%";
                }
            }

            else if (id == 3)
            {
                DbSet<Light> dbs = _dbContext.Light;
                List<Light> model = dbs.OrderByDescending(p => p.LDatetime).ToList();
                ViewData["Year"] = dbs.ToList()
                    .GroupBy(p => p.LDatetime.Year)
                    .OrderByDescending(g => g.Key)
                    .Select(o =>
                    {
                        dynamic d = new ExpandoObject();
                        d.value = o.Key;
                        d.text = o.Key;
                        return d;
                    })
                    .ToList<dynamic>();
                ViewData["High"] = model1.HighestLight;
                ViewData["Low"] = model1.LowestLight;
                ViewData["id"] = 3;
                ViewData["type"] = "Light";
                ViewData["Model"] = model;
                //ViewData["update"] = dbs.ToList()
                //    .OrderByDescending(p => p.LDatetime)
                //    .Select(p => p.LDatetime)
                //    .FirstOrDefault();
                if (model1.LightUnit == 1)
                {
                    ViewData["unit"] = "lx";
                }
            }

            else if (id == 4)
            {
                DbSet<Weight> dbs = _dbContext.Weight;
                List<Weight> model = dbs.OrderByDescending(p => p.WDatetime).ToList();
                ViewData["Year"] = dbs.ToList()
                    .GroupBy(p => p.WDatetime.Year)
                    .OrderByDescending(g => g.Key)
                    .Select(o =>
                    {
                        dynamic d = new ExpandoObject();
                        d.value = o.Key;
                        d.text = o.Key;
                        return d;
                    })
                    .ToList<dynamic>();
                ViewData["High"] = model1.HighestWeight;
                ViewData["Low"] = model1.LowestWeight;
                ViewData["id"] = 4;
                ViewData["type"] = "Force";
                ViewData["Model"] = model;
                //ViewData["update"] = dbs.ToList()
                //    .OrderByDescending(p => p.WDatetime)
                //    .Select(p => p.WDatetime)
                //    .FirstOrDefault();
                if (model1.WeightUnit == 1)
                {
                    ViewData["unit"] = "N";
                }
                else if (model1.WeightUnit == 2)
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
            }
            else
            {
                return RedirectToAction("Forbidden", "Account");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Chart(int id, int type)
        {
            PreparePreference();
            PrepareData(id);
            DbSet<Preference> dbs1 = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
            string chartType = "";
            if (type == 1)
            {
                chartType = "Vertical Bar Chart";
                ViewData["chart"] = "bar";
            }
            else if (type == 2)
            {
                chartType = "Horizontal Bar Chart";
                ViewData["chart"] = "horizontalBar";
            }
            else if (type == 3)
            {
                chartType = "Line Graph";
                ViewData["chart"] = "line";
            }
            if (id == 1)
            {
                ViewData["Title"] = "Temperature " + chartType;
                if (model1.TempUnit == 1)
                {
                    ViewData["unit"] = "Celcius ('C)";
                }
                else if (model1.TempUnit == 2)
                {
                    ViewData["unit"] = "Kelvin (K)";
                }
                else if (model1.TempUnit == 3)
                {
                    ViewData["unit"] = "Fahrenheit (F)";
                }
            }
            else if (id == 2)
            {
                ViewData["Title"] = "Humidity " + chartType;
                ViewData["unit"] = "Percentage (%)";
            }
            else if (id == 3)
            {
                ViewData["Title"] = "Light " + chartType;
                ViewData["unit"] = "lx";
            }
            else if (id == 4)
            {
                ViewData["Title"] = "Force " + chartType;
                if (model1.WeightUnit == 1)
                {
                    ViewData["unit"] = "Newton (N)";
                }
                else if (model1.WeightUnit == 2)
                {
                    ViewData["unit"] = "Gram (g)";
                }
                else if (model1.WeightUnit == 3)
                {
                    ViewData["unit"] = "Kilogram (kg)";
                }
                else if (model1.WeightUnit == 4)
                {
                    ViewData["unit"] = "Pound (lb)";
                }
            }
            return View();
        }

        private void PrepareData(int id)
        {
            DbSet<Preference> dbs1 = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
            if (id == 1)
            {
                DbSet<Temperature> dbs = _dbContext.Temperature;
                List<Temperature> model = dbs.OrderBy(p => p.TDatetime).ToList();
                List<double> temp = new List<double>();
                List<string> date = new List<string>();
                ViewData["Year"] = dbs.ToList()
                    .GroupBy(p => p.TDatetime.Year)
                    .OrderByDescending(g => g.Key)
                    .Select(o =>
                    {
                        dynamic d = new ExpandoObject();
                        d.value = o.Key;
                        d.text = o.Key;
                        return d;
                    })
                    .ToList<dynamic>();
                if (model1.TempUnit == 1)
                {
                    foreach (Temperature poke in model)
                    {
                        double level = Convert.ToDouble(poke.TLevel);
                        temp.Add(level);
                        date.Add(string.Format("{0:dd/MM/yyyy hh:mm:ss tt}", poke.TDatetime));
                    }
                }
                else if (model1.TempUnit == 2)
                {
                    foreach (Temperature poke in model)
                    {
                        double level = Convert.ToDouble(poke.TLevel + 273.15);
                        temp.Add(level);
                        date.Add(string.Format("{0:dd/MM/yyyy hh:mm:ss tt}", poke.TDatetime));
                    }
                }
                else if (model1.TempUnit == 3)
                {
                    foreach (Temperature poke in model)
                    {
                        double level = Convert.ToDouble((poke.TLevel * 1.8) + 32);
                        temp.Add(level);
                        date.Add(string.Format("{0:dd/MM/yyyy hh:mm:ss tt}", poke.TDatetime));
                    }
                }


                ViewData["model"] = temp;
                ViewData["date"] = date;
                ViewData["id"] = 1;
                ViewData["type"] = "Temperature";
                ViewData["color"] = "red"/*"#D95350"*/;/* "red";*/
            }
            else if (id == 2)
            {
                DbSet<Humidity> dbs = _dbContext.Humidity;
                List<Humidity> model = dbs.OrderBy(p => p.HDatetime).ToList();
                List<double> hum = new List<double>();
                List<string> date = new List<string>();
                ViewData["Year"] = dbs.ToList()
                    .GroupBy(p => p.HDatetime.Year)
                    .OrderByDescending(g => g.Key)
                    .Select(o =>
                    {
                        dynamic d = new ExpandoObject();
                        d.value = o.Key;
                        d.text = o.Key;
                        return d;
                    })
                    .ToList<dynamic>();
                foreach (Humidity poke in model)
                {
                    double level = Convert.ToDouble(poke.HLevel);
                    hum.Add(level);
                    date.Add(string.Format("{0:dd/MM/yyyy hh:mm:ss tt}", poke.HDatetime));
                }

                ViewData["model"] = hum;
                ViewData["date"] = date;
                ViewData["id"] = 2;
                ViewData["type"] = "Humidity";
                ViewData["color"] = "#0375D8"; /*"blue";*/
            }

            else if (id == 3)
            {
                DbSet<Light> dbs = _dbContext.Light;
                List<Light> model = dbs.OrderBy(p => p.LDatetime).ToList();
                List<double> Light = new List<double>();
                List<string> date = new List<string>();
                ViewData["Year"] = dbs.ToList()
                    .GroupBy(p => p.LDatetime.Year)
                    .OrderByDescending(g => g.Key)
                    .Select(o =>
                    {
                        dynamic d = new ExpandoObject();
                        d.value = o.Key;
                        d.text = o.Key;
                        return d;
                    })
                    .ToList<dynamic>();
                foreach (Light poke in model)
                {
                    double level = Convert.ToDouble(poke.LLevel);
                    Light.Add(level);
                    date.Add(string.Format("{0:dd/MM/yyyy hh:mm:ss tt}", poke.LDatetime));
                }

                ViewData["model"] = Light;
                ViewData["date"] = date;
                ViewData["id"] = 3;
                ViewData["type"] = "Light";
                ViewData["color"] = "#FFC000"; /*"yellow";*//*#ECAF4E*/
            }

            else if (id == 4)
            {
                DbSet<Weight> dbs = _dbContext.Weight;
                List<Weight> model = dbs.OrderBy(p => p.WDatetime).ToList();
                List<double> Weight = new List<double>();
                List<string> date = new List<string>();
                ViewData["Year"] = dbs.ToList()
                    .GroupBy(p => p.WDatetime.Year)
                    .OrderByDescending(g => g.Key)
                    .Select(o =>
                    {
                        dynamic d = new ExpandoObject();
                        d.value = o.Key;
                        d.text = o.Key;
                        return d;
                    })
                    .ToList<dynamic>();

                foreach (Weight poke in model)
                {
                    double level = Convert.ToDouble(poke.WLevel * 101.97);
                    Weight.Add(level);
                    date.Add(string.Format("{0:dd/MM/yyyy hh:mm:ss tt}", poke.WDatetime));
                }

                if (model1.WeightNoti == 1)
                {
                    foreach (Weight poke in model)
                    {
                        double level = Convert.ToDouble(poke.WLevel);
                        Weight.Add(level);
                        date.Add(string.Format("{0:dd/MM/yyyy hh:mm:ss tt}", poke.WDatetime));
                    }
                }
                else if (model1.WeightNoti == 2)
                {
                    foreach (Weight poke in model)
                    {
                        double level = Convert.ToDouble(poke.WLevel * 101.97);
                        Weight.Add(level);
                        date.Add(string.Format("{0:dd/MM/yyyy hh:mm:ss tt}", poke.WDatetime));
                    }
                }
                else if (model1.WeightNoti == 3)
                {
                    foreach (Weight poke in model)
                    {
                        double level = Convert.ToDouble(poke.WLevel * 0.101972);
                        Weight.Add(level);
                        date.Add(string.Format("{0:dd/MM/yyyy hh:mm:ss tt}", poke.WDatetime));
                    }
                }
                else if (model1.WeightNoti == 4)
                {
                    foreach (Weight poke in model)
                    {
                        double level = Convert.ToDouble(poke.WLevel * 0.224809);
                        Weight.Add(level);
                        date.Add(string.Format("{0:dd/MM/yyyy hh:mm:ss tt}", poke.WDatetime));
                    }
                }


                ViewData["model"] = Weight;
                ViewData["date"] = date;
                ViewData["id"] = 4;
                ViewData["type"] = "Force";
                ViewData["color"] = /*"#5DB75D";*/ "green";
            }


        }

        //public IActionResult TableTest(int id)
        //{
        //    PreparePreference();
        //    //Temperature
        //    if (id == 1)
        //    {
        //        DbSet<Temperature> dbs = _dbContext.Temperature;
        //        DbSet<Preference> dbs1 = _dbContext.Preference;
        //        int userId = 0;
        //        Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
        //        Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
        //        if (model1.TempUnit == 1)
        //        {
        //            var model = dbs.ToList()
        //                .OrderByDescending(p => p.TDatetime)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.TemperatureId;
        //                d.level = String.Format("{0:0.00}", g.TLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
        //                return d;
        //            });
        //            if (model != null)
        //            {
        //                return Ok(model);
        //            }
        //            else
        //            {
        //                return BadRequest();
        //            }
        //        }
        //        else if (model1.TempUnit == 2)
        //        {
        //            var model = dbs.ToList()
        //                .OrderByDescending(p => p.TDatetime)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.TemperatureId;
        //                d.level = String.Format("{0:0.00}", g.TLevel + 273.15);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
        //                return d;
        //            });
        //            if (model != null)
        //            {
        //                return Ok(model);
        //            }
        //            else
        //            {
        //                return BadRequest();
        //            }
        //        }
        //        else if (model1.TempUnit == 3)
        //        {
        //            var model = dbs.ToList()
        //                .OrderByDescending(p => p.TDatetime)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.TemperatureId;
        //                d.level = String.Format("{0:0.00}", (g.TLevel * 1.8) + 32);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
        //                return d;
        //            });
        //            if (model != null)
        //            {
        //                return Ok(model);
        //            }
        //            else
        //            {
        //                return BadRequest();
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }

        //    }
        //    //Humidity
        //    else if (id == 2)
        //    {
        //        DbSet<Humidity> dbs = _dbContext.Humidity;
        //        DbSet<Preference> dbs1 = _dbContext.Preference;
        //        int userId = 0;
        //        Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
        //        Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
        //        if (model1.HumUnit == 1)
        //        {
        //            var model = dbs.ToList()
        //                .OrderByDescending(p => p.HDatetime)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.HumidityId;
        //                d.level = String.Format("{0:0.00}", g.HLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.HDatetime);
        //                return d;
        //            });
        //            if (model != null)
        //            {
        //                return Ok(model);
        //            }
        //            else
        //            {
        //                return BadRequest();
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }

        //    }
        //    //Light
        //    else if (id == 3)
        //    {
        //        DbSet<Light> dbs = _dbContext.Light;
        //        DbSet<Preference> dbs1 = _dbContext.Preference;
        //        int userId = 0;
        //        Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
        //        Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
        //        if (model1.LightUnit == 1)
        //        {
        //            var model = dbs.ToList()
        //                .OrderByDescending(p => p.LDatetime)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.LightId;
        //                d.level = String.Format("{0:0.00}", g.LLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.LDatetime);
        //                return d;
        //            });
        //            if (model != null)
        //            {
        //                return Ok(model);
        //            }
        //            else
        //            {
        //                return BadRequest();
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }

        //    }
        //    //Weight
        //    if (id == 4)
        //    {
        //        DbSet<Weight> dbs = _dbContext.Weight;
        //        DbSet<Preference> dbs1 = _dbContext.Preference;
        //        int userId = 0;
        //        Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
        //        Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
        //        if (model1.WeightUnit == 1)
        //        {
        //            var model = dbs.ToList()
        //                .OrderByDescending(p => p.WDatetime)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.WeightId;
        //                d.level = String.Format("{0:0.00}", g.WLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
        //                return d;
        //            });
        //            if (model != null)
        //            {
        //                return Ok(model);
        //            }
        //            else
        //            {
        //                return BadRequest();
        //            }
        //        }
        //        else if (model1.WeightUnit == 2)
        //        {
        //            var model = dbs.ToList()
        //                .OrderByDescending(p => p.WDatetime)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.WeightId;
        //                d.level = String.Format("{0:0.00}", g.WLevel* 101.97);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
        //                return d;
        //            });
        //            if (model != null)
        //            {
        //                return Ok(model);
        //            }
        //            else
        //            {
        //                return BadRequest();
        //            }
        //        }
        //        else if (model1.TempUnit == 3)
        //        {
        //            var model = dbs.ToList()
        //                .OrderByDescending(p => p.WDatetime)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.WeightId;
        //                d.level = String.Format("{0:0.00}", g.WLevel * 0.101972);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
        //                return d;
        //            });
        //            if (model != null)
        //            {
        //                return Ok(model);
        //            }
        //            else
        //            {
        //                return BadRequest();
        //            }
        //        }
        //        else if (model1.TempUnit == 4)
        //        {
        //            var model = dbs.ToList()
        //                .OrderByDescending(p => p.WDatetime)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.WeightId;
        //                d.level = String.Format("{0:0.00}", g.WLevel * 0.224809);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
        //                return d;
        //            });
        //            if (model != null)
        //            {
        //                return Ok(model);
        //            }
        //            else
        //            {
        //                return BadRequest();
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }

        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }

        //}

        //private void PrepareData(DateTime start, DateTime end)
        //{
        //    DbSet<Temperature> dbs = _dbContext.Temperature;
        //    List<Temperature> model = dbs.OrderBy(p => p.TDatetime).ToList();
        //    model = model.Where(p => p.TDatetime >= start && p.TDatetime <= end).ToList();
        //    if (model != null)
        //    {
        //        List<double> temp = new List<double>();
        //        List<string> date = new List<string>();
        //        foreach (Temperature poke in model)
        //        {
        //            double Temperature = Convert.ToDouble(poke.TLevel);
        //            temp.Add(Temperature);
        //            date.Add(string.Format("{0:dd/MM/yyyy hh:mm:ss tt}", poke.TDatetime));
        //        }

        //        ViewData["Title"] = "Temperature Vertical Bar Chart";
        //        ViewData["temp"] = temp;
        //        ViewData["date"] = date;
        //    }
        //}

        //public IActionResult Notification()
        //{
        //DbSet<Light> dbs = _dbContext.Light;
        //DbSet<Preference> dbslmt = _dbContext.Preference;
        //Random gen = new Random();
        //string[] data = new string[1];            
        //dynamic light = Convert.ToDouble(gen.Next(1, 50));
        //dynamic high = Convert.ToDouble(dbslmt.Where(p => p.UserId == 101).Select(p => p.HighestLight).FirstOrDefault());
        //dynamic low = Convert.ToDouble(dbslmt.Where(p => p.UserId == 101).Select(p => p.LowestLight).FirstOrDefault());
        //if (light >= high)
        //{
        //    _toastNotification.AddWarningToastMessage("The light is over " + high + ". The current temperature is " + light);
        //}
        //else if (light <= low)
        //{
        //    _toastNotification.AddWarningToastMessage("The light is over " + low + ". The current humidity is " + light);
        //}
        //data[0] = string.Format("{0:0.00}", light); 
        //return Json(data);
        //    Random gen = new Random();
        //    string[] data = new string[1];
        //    data[0] = string.Format("{0:0.00}", Convert.ToDouble(gen.Next(11, 15)));
        //    return Json(data);
        //}

        //public IActionResult Notification()
        //{
        //    Random gen = new Random();
        //    string[] data = new string[1];
        //    dynamic light = Convert.ToDouble(gen.Next(1, 50));
        //    DbSet<Preference> dbslmt = _dbContext.Preference;
        //    dynamic high = Convert.ToDouble(dbslmt.Where(p => p.UserId == 101).Select(p => p.HighestLight).FirstOrDefault());
        //    dynamic low = Convert.ToDouble(dbslmt.Where(p => p.UserId == 101).Select(p => p.LowestLight).FirstOrDefault());
        //    if (light >= high)
        //    {
        //        _toastNotification.AddWarningToastMessage("The light is over " + high + ". The current light is " + light);
        //    }
        //    else if (light <= low)
        //    {
        //        _toastNotification.AddWarningToastMessage("The light is over " + low + ". The current light is " + light);
        //    }
        //    data[0] = string.Format("{0:0.00}",light );
        //    return Json(data);
        //}

        public IActionResult Test()
        {
            PreparePreference();
            DbSet<Emsuser> dbs = _dbContext.Emsuser;
            var model = dbs.ToList()
                .Select(p => p.Name)
                .Distinct();
            ViewData["keyword"] = model;
            return View();
        }

        //[HttpGet]
        //public IActionResult Template(int id)
        //{
        //    DbSet<Temperature> dbs = _dbContext.Temperature;
        //    List<Temperature> model = dbs.ToList();
        //    DbSet<Preference> dbs1 = _dbContext.Preference;
        //    Preference model1 = dbs1.Where(p => p.UserId == 101).FirstOrDefault();
        //    ViewData["High"] = model1.HighestTemp;
        //    ViewData["Low"] = model1.LowestTemp;
        //    PrepareData(id);
        //    return View(model);
        //}

        //[HttpGet]
        //private IActionResult GetByYear(string i, string y)
        //{
        //    //IActionResult
        //    //IEnumerable < C300.Models.Temperature >
        //    int id = Convert.ToInt32(i);
        //    int year = Convert.ToInt32(y);
        //    if (id == 1)
        //    {
        //        DbSet<Temperature> dbs = _dbContext.Temperature;
        //        var model = dbs.ToList()
        //            .Where(p => p.TDatetime.Year == year);
        //        ViewData["Model"] = model;
        //        return View("Table");
        //        //return model;
        //    }
        //    if (id == 2)
        //    {
        //        DbSet<Humidity> dbs = _dbContext.Humidity;
        //        var model = dbs.ToList()
        //            .Where(p => p.HDatetime.Year == year);
        //        ViewData["Model"] = model;
        //        return View("Table");
        //        //return model;
        //    }
        //    else
        //    {
        //        return View("Table");
        //        //return null;
        //    }
        //}

        //private IEnumerable<C300.Models.Temperature> GetByYear(int id, int year)
        //{
        //    if (id == 1)
        //    {
        //        DbSet<Temperature> dbs = _dbContext.Temperature;
        //        var model = dbs.ToList()
        //            .Where(p => p.TDatetime.Year == year);
        //        ViewData["Model"] = model;
        //        return model;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}


        //[HttpGet]
        //public IActionResult VBar(int id)
        //{
        //    PrepareData(id);
        //    if (id == 1)
        //    {
        //        ViewData["Title"] = "Temperature Vertical Bar Chart";
        //    }
        //    else if (id == 2)
        //    {
        //        ViewData["Title"] = "Humidity Vertical Bar Chart";
        //    }
        //    else if (id == 3)
        //    {
        //        ViewData["Title"] = "Light Vertical Bar Chart";
        //    }
        //    else if (id == 4)
        //    {
        //        ViewData["Title"] = "Weight Vertical Bar Chart";
        //    }
        //    return View();
        //}

        //[HttpGet]
        //public IActionResult HBar(int id)
        //{
        //    PrepareData(id);
        //    if (id == 1)
        //    {
        //        ViewData["Title"] = "Temperature Horizontal Bar Chart";
        //    }
        //    else if (id == 2)
        //    {
        //        ViewData["Title"] = "Humidity Horizontal Bar Chart";
        //    }
        //    else if (id == 3)
        //    {
        //        ViewData["Title"] = "Light Horizontal Bar Chart";
        //    }
        //    else if (id == 4)
        //    {
        //        ViewData["Title"] = "Weight Horizontal Bar Chart";
        //    }
        //    return View();
        //}

        //[HttpGet]
        //public IActionResult Line(int id)
        //{
        //    PrepareData(id);
        //    if (id == 1)
        //    {
        //        ViewData["Title"] = "Temperature Line Graph";
        //    }
        //    else if (id == 2)
        //    {
        //        ViewData["Title"] = "Humidity Line Graph";
        //    }
        //    else if (id == 3)
        //    {
        //        ViewData["Title"] = "Light Line Graph";
        //    }
        //    else if (id == 4)
        //    {
        //        ViewData["Title"] = "Weight Line Graph";
        //    }
        //    return View();
        //}



        //public ActionResult Chart()
        //{
        //    PrepareData();
        //    ViewData["Chart"] = "bar";
        //    DateTimeRange model = new DateTimeRange();
        //    return View(model);

        //}

        //[HttpPost]
        //public ActionResult Chart(DateTimeRange model)
        //{
        //    PrepareData(model.Start, model.End);
        //    ViewData["Chart"] = "bar";
        //    return View();

        //}


    }
}