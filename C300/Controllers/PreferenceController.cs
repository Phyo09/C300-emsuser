using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using C300.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace C300.Controllers
{
    public class PreferenceController : Controller
    {
        private AppDbContext _dbContext;
        private IHttpContextAccessor _contextAccessor;
        private HttpContext _context { get { return _contextAccessor.HttpContext; } }

        public PreferenceController(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
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

        [Authorize] 
        [HttpGet]
        public IActionResult Setting()
        {
            PreparePreference();
            DbSet<Preference> dbs = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model = dbs.Where(p => p.UserId == userId).FirstOrDefault();
            return View(model);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Setting(Preference set)
        {
            BacklogActivity("Accessed Preference Setting Page");
            PreparePreference();
            
            DbSet<Preference> dbs = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model = dbs.Where(p => p.UserId == userId).FirstOrDefault();
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    if (set.LowestTemp<=set.HighestTemp && set.LowestHumidity<= set.HighestHumidity &&
                        set.LowestLight<=set.HighestLight && set.LowestWeight <= set.HighestWeight)
                    {
                        if ((model.HighestTemp == set.HighestTemp && model.LowestTemp == set.LowestTemp && model.TempUnit == set.TempUnit && model.TempNoti == set.TempNoti) &&
                    (model.HighestHumidity == set.HighestHumidity && model.LowestHumidity == set.LowestHumidity && model.HumUnit == set.HumUnit && model.HumNoti == set.HumNoti) &&
                    (model.HighestWeight == set.HighestWeight && model.LowestWeight == set.LowestWeight && model.LightUnit == set.LightUnit && model.LightNoti == set.LightNoti) &&
                    (model.HighestLight == set.HighestLight && model.LowestLight == set.LowestLight && model.WeightUnit == set.WeightUnit && model.WeightNoti == set.WeightNoti))
                        {
                            TempData["Msg"] = "Same info entered! ";
                            return View(model);
                        }
                        else
                        {
                            model.HighestTemp = set.HighestTemp;
                            model.LowestTemp = set.LowestTemp;
                            model.TempUnit = set.TempUnit;
                            model.TempNoti = set.TempNoti;
                            model.HighestHumidity = set.HighestHumidity;
                            model.LowestHumidity = set.LowestHumidity;
                            model.HumUnit = set.HumUnit;
                            model.HumNoti = set.HumNoti;
                            model.HighestWeight = set.HighestWeight;
                            model.LowestWeight = set.LowestWeight;
                            model.LightUnit = set.LightUnit;
                            model.LightNoti = set.LightNoti;
                            model.HighestLight = set.HighestLight;
                            model.LowestLight = set.LowestLight;
                            model.WeightUnit = set.WeightUnit;
                            model.WeightNoti = set.WeightNoti;

                            if (_dbContext.SaveChanges() == 1)
                            {
                                BacklogActivity("Updated Preference Settings");
                                TempData["Msg"] = "Info updated. ";
                                return View(model);
                            }
                            else
                            {
                                TempData["Msg"] = "Failed to update";
                                return View(model);
                            }
                        }
                    }
                    TempData["Msg"] = "Lowest must be lower than highest";
                    return View(model);

                }
                else
                {
                    TempData["Msg"] = "Invalid input!";
                    return View(model);
                }

            }
            else
            {
                TempData["Msg"] = "User not found!";
                return View(model);
            }
            
            
            
        }

        [Authorize]
        [HttpGet]
        public IActionResult Setting_t()
        {
            DbSet<Preference> dbs = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model = dbs.Where(p => p.UserId == userId).FirstOrDefault();
            //if (model == null)
            //{

            //}
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Setting_t(Preference set)
        {
            if (ModelState.IsValid)
            {
                DbSet<Preference> dbs = _dbContext.Preference;
                int userId = 0;
                Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
                Preference model = dbs.Where(p => p.UserId == userId).FirstOrDefault();
                if (model != null)
                {
                    model.HighestTemp = set.HighestTemp;
                    model.LowestTemp = set.LowestTemp;
                    model.HighestHumidity = set.HighestHumidity;
                    model.LowestHumidity = set.LowestHumidity;
                    model.HighestWeight = set.HighestWeight;
                    model.LowestWeight = set.LowestWeight;
                    model.HighestLight = set.HighestLight;
                    model.LowestLight = set.LowestLight;
                    //model.HighestTemp = 21;
                    //model.LowestTemp = 12;
                    //model.HighestHumidity = 21;
                    //model.LowestHumidity = 43;
                    //model.HighestWeight = 23;
                    //model.LowestWeight = 32;
                    //model.HighestLight = 23;
                    //model.LowestLight = 43;

                    if (_dbContext.SaveChanges() == 1)
                    {
                        BacklogActivity("Updated Preference Settings");
                        TempData["Msg"] = "Info updated. ";
                        return View(model);
                    }
                    else
                    {
                        TempData["Msg"] = "Failed to update";
                        return View(set);
                    }

                }
                TempData["Msg"] = "User not found!";
                return View(set);
            }
            else
            {
                TempData["Msg"] = "Invalid input!";
                return View(set);
            }
            
        }
    }
}