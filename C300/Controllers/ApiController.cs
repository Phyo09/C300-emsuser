using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using C300.Models;
using System.Dynamic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace C300.Controllers
{
    [Route("api/Past")]
    public class ApiController : Controller
    {
        private AppDbContext _dbContext;

        public ApiController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("GetDash/{name}")]
        [Authorize]
        public IActionResult GetDash(string name)
        {
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            string sql = @"SELECT {0} as value FROM dash WHERE User_id = {1}";
            var output = DBUtl.GetList(sql, name, userId);
            if (output.Count >0)
            {
                int value = output[0].value;
                return Json(value);
            }
            else
            {
                return Json("Fail");
            }
        }

        [HttpGet("UpdateDash/{name}/{num}")]
        [Authorize]
        public IActionResult UpdateDash(string name,int num)
        {
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            string sql = @"UPDATE dash SET {0}={1} WHERE User_id = {2}";
            if (DBUtl.ExecSQL(sql,name,num,userId) == 1)
            {
                string sql1 = @"SELECT {0} as value FROM dash WHERE User_id = {1}";
                var output = DBUtl.GetList(sql1, name, userId);
                if (output.Count > 0)
                {
                    int value = output[0].value;
                    return Json(value);
                }
                else
                {
                    return Json("Fail");
                }
            }
            else
            {
                return Json("Fail");
            }
        }

        [HttpGet("GetAverage/{id}")]
        [Authorize]
        public IActionResult GetAverage(int id)
        {
            DbSet<Preference> dbs1 = _dbContext.Preference;
            int userId = 0;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
            Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
            if (id == 1)
            {                
                DbSet<Temperature> dbs = _dbContext.Temperature;
                if (model1.TempUnit == 1)
                {
                    var model = dbs.ToList()
                        .Where(p => p.TDatetime.Day == DateTime.Now.Day)
                        .GroupBy(p => p.TDatetime.Hour)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.Key;
                        d.average = g.Average(i => i.TLevel);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.TempUnit == 2)
                {
                    int day = DateTime.Now.Day - 1;
                    var model = dbs.ToList()
                        .Where(p => p.TDatetime.Day == DateTime.Now.Day)
                        .GroupBy(p => p.TDatetime.Hour)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.Key;
                        d.average = string.Format("{0:0.00}",g.Average(i => i.TLevel));
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.TempUnit == 3)
                {
                    var model = dbs.ToList()
                        .Where(p => p.TDatetime.Day == DateTime.Now.Day - 1)
                        .GroupBy(p => p.TDatetime.Hour)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.Key;
                        d.average = g.Average(i => i.TLevel);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            //else if (id == 2)
            //{
            //    DbSet<Humidity> dbs = _dbContext.Humidity;
            //    var model = dbs.ToList()
            //        .Where(p => p.HDatetime.Year == year)
            //        .Select(p => p.HDatetime.Month)
            //        .Distinct();
            //    if (model != null)
            //    {
            //        return Ok(model);
            //    }
            //    else
            //    {
            //        return BadRequest();
            //    }
            //}
            //else if (id == 3)
            //{
            //    DbSet<Light> dbs = _dbContext.Light;
            //    var model = dbs.ToList()
            //        .Where(p => p.LDatetime.Year == year)
            //        .Select(p => p.LDatetime.Month)
            //        .Distinct();
            //    if (model != null)
            //    {
            //        return Ok(model);
            //    }
            //    else
            //    {
            //        return BadRequest();
            //    }
            //}
            //else if (id == 4)
            //{
            //    DbSet<Weight> dbs = _dbContext.Weight;
            //    var model = dbs.ToList()
            //        .Where(p => p.WDatetime.Year == year)
            //        .Select(p => p.WDatetime.Month)
            //        .Distinct();
            //    if (model != null)
            //    {
            //        return Ok(model);
            //    }
            //    else
            //    {
            //        return BadRequest();
            //    }
            //}
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("Year/{id}/{year}")]
        //[Authorize()]
        public IActionResult GetMonth(int id, int year)
        {
            if (id == 1)
            {
                DbSet<Temperature> dbs = _dbContext.Temperature;
                var model = dbs.ToList()
                    .Where(p => p.TDatetime.Year == year)
                    .Select(p => p.TDatetime.Month)
                    .Distinct();
                if (model != null)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else if (id == 2)
            {
                DbSet<Humidity> dbs = _dbContext.Humidity;
                var model = dbs.ToList()
                    .Where(p => p.HDatetime.Year == year)
                    .Select(p => p.HDatetime.Month)
                    .Distinct();
                if (model != null)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else if (id == 3)
            {
                DbSet<Light> dbs = _dbContext.Light;
                var model = dbs.ToList()
                    .Where(p => p.LDatetime.Year == year)
                    .Select(p => p.LDatetime.Month)
                    .Distinct();
                if (model != null)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else if (id == 4)
            {
                DbSet<Weight> dbs = _dbContext.Weight;
                var model = dbs.ToList()
                    .Where(p => p.WDatetime.Year == year)
                    .Select(p => p.WDatetime.Month)
                    .Distinct();
                if (model != null)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("GetByYear/{id}/{year}")]
        //[Authorize()]
        public IActionResult GetByYear(int id, int year)
        {
            if (id == 1)
            {
                DbSet<Preference> dbs1 = _dbContext.Preference;
                int userId = 0;
                Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
                Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
                DbSet<Temperature> dbs = _dbContext.Temperature;
                if (model1.TempUnit == 1)
                {
                    var model = dbs.ToList()
                    .Where(p => p.TDatetime.Year == year)
                        .OrderBy(p => p.TDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.TemperatureId;
                        d.level = String.Format("{0:0.00}", g.TLevel);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.TempUnit == 2)
                {
                    var model = dbs.ToList()
                        .Where(p => p.TDatetime.Year == year)
                        .OrderBy(p => p.TDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.TemperatureId;
                        d.level = String.Format("{0:0.00}", g.TLevel + 273.15);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.TempUnit == 3)
                {
                    var model = dbs.ToList()
                        .Where(p => p.TDatetime.Year == year)
                        .OrderBy(p => p.TDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.TemperatureId;
                        d.level = String.Format("{0:0.00}", (g.TLevel * 1.8) + 32);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }

            }
            else if (id == 2)
            {
                DbSet<Humidity> dbs = _dbContext.Humidity;
                var model = dbs.ToList()
                    .Where(p => p.HDatetime.Year == year)
                    .OrderBy(p => p.HDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.HumidityId;
                        d.level = String.Format("{0:0.00}", g.HLevel);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.HDatetime);
                        return d;
                    });
                if (model != null)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else if (id == 3)
            {
                DbSet<Light> dbs = _dbContext.Light;
                var model = dbs.ToList()
                    .Where(p => p.LDatetime.Year == year)
                    .OrderBy(p => p.LDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.LightId;
                        d.level = String.Format("{0:0.00}", g.LLevel);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.LDatetime);
                        return d;
                    });
                if (model != null)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else if (id == 4)
            {
                DbSet<Preference> dbs1 = _dbContext.Preference;
                int userId = 0;
                Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
                Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
                DbSet<Weight> dbs = _dbContext.Weight;
                if (model1.WeightUnit == 1)
                {
                    var model = dbs.ToList()
                    .Where(p => p.WDatetime.Year == year)
                    .OrderBy(p => p.WDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.WeightId;
                        d.level = String.Format("{0:0.00}", g.WLevel);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.WeightUnit == 2)
                {
                    var model = dbs.ToList()
                        .Where(p => p.WDatetime.Year == year)
                    .OrderBy(p => p.WDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.WeightId;
                        d.level = String.Format("{0:0.00}", g.WLevel * 101.97);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.TempUnit == 3)
                {
                    var model = dbs.ToList()
                        .Where(p => p.WDatetime.Year == year)
                    .OrderBy(p => p.WDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.WeightId;
                        d.level = String.Format("{0:0.00}", g.WLevel * 0.101972);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.TempUnit == 4)
                {
                    var model = dbs.ToList()
                        .Where(p => p.WDatetime.Year == year)
                    .OrderBy(p => p.WDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.WeightId;
                        d.level = String.Format("{0:0.00}", g.WLevel * 0.224809);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }
                
            }
            else
            {
                return BadRequest();
            }
        }


        [HttpGet("Month/{id}/{year}/{month}")]
        //[Authorize()]
        public IActionResult GetDay(int id, int year, int month)
        {
            if (id == 1)
            {
                DbSet<Temperature> dbs = _dbContext.Temperature;
                var model = dbs.ToList()
                    .Where(p => p.TDatetime.Year == year && p.TDatetime.Month == month)
                    .Select(p => p.TDatetime.Day)
                    .Distinct();
                if (model != null)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else if (id == 2)
            {
                DbSet<Humidity> dbs = _dbContext.Humidity;
                var model = dbs.ToList()
                    .Where(p => p.HDatetime.Year == year && p.HDatetime.Month == month)
                    .Select(p => p.HDatetime.Day)
                    .Distinct();
                if (model != null)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else if (id == 3)
            {
                DbSet<Light> dbs = _dbContext.Light;
                var model = dbs.ToList()
                    .Where(p => p.LDatetime.Year == year && p.LDatetime.Month == month)
                    .Select(p => p.LDatetime.Day)
                    .Distinct();
                if (model != null)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else if (id == 4)
            {
                DbSet<Weight> dbs = _dbContext.Weight;
                var model = dbs.ToList()
                    .Where(p => p.WDatetime.Year == year && p.WDatetime.Month == month)
                    .Select(p => p.WDatetime.Day)
                    .Distinct();
                if (model != null)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("GetByMonth/{id}/{year}/{month}")]
        //[Authorize()]
        public IActionResult GetByMonth(int id, int year, int month)
        {
            if (id == 1)
            {
                DbSet<Preference> dbs1 = _dbContext.Preference;
                int userId = 0;
                Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
                Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
                DbSet<Temperature> dbs = _dbContext.Temperature;
                if (model1.TempUnit == 1)
                {
                    var model = dbs.ToList()
                    .Where(p => p.TDatetime.Year == year && p.TDatetime.Month == month)
                    .OrderBy(p => p.TDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.TemperatureId;
                        d.level = String.Format("{0:0.00}", g.TLevel);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.TempUnit == 2)
                {
                    var model = dbs.ToList()
                        .Where(p => p.TDatetime.Year == year && p.TDatetime.Month == month)
                    .OrderBy(p => p.TDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.TemperatureId;
                        d.level = String.Format("{0:0.00}", g.TLevel + 273.15);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.TempUnit == 3)
                {
                    var model = dbs.ToList()
                        .Where(p => p.TDatetime.Year == year && p.TDatetime.Month == month)
                    .OrderBy(p => p.TDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.TemperatureId;
                        d.level = String.Format("{0:0.00}", (g.TLevel * 1.8) + 32);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }
                
            }
            else if (id == 2)
            {
                DbSet<Humidity> dbs = _dbContext.Humidity;
                var model = dbs.ToList()
                    .Where(p => p.HDatetime.Year == year && p.HDatetime.Month == month)
                    .OrderBy(p => p.HDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.HumidityId;
                        d.level = String.Format("{0:0.00}", g.HLevel);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.HDatetime);
                        return d;
                    });
                if (model != null)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else if (id == 3)
            {
                DbSet<Light> dbs = _dbContext.Light;
                var model = dbs.ToList()
                    .Where(p => p.LDatetime.Year == year && p.LDatetime.Month == month)
                    .OrderBy(p => p.LDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.LightId;
                        d.level = String.Format("{0:0.00}", g.LLevel);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.LDatetime);
                        return d;
                    });
                if (model != null)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else if (id == 4)
            {
                DbSet<Preference> dbs1 = _dbContext.Preference;
                int userId = 0;
                Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
                Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
                DbSet<Weight> dbs = _dbContext.Weight;
                if (model1.WeightUnit == 1)
                {
                    var model = dbs.ToList()
                    .Where(p => p.WDatetime.Year == year && p.WDatetime.Month == month)
                    .OrderBy(p => p.WDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.WeightId;
                        d.level = String.Format("{0:0.00}", g.WLevel);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.WeightUnit == 2)
                {
                    var model = dbs.ToList()
                        .Where(p => p.WDatetime.Year == year && p.WDatetime.Month == month)
                    .OrderBy(p => p.WDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.WeightId;
                        d.level = String.Format("{0:0.00}", g.WLevel * 101.97);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.TempUnit == 3)
                {
                    var model = dbs.ToList()
                        .Where(p => p.WDatetime.Year == year && p.WDatetime.Month == month)
                    .OrderBy(p => p.WDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.WeightId;
                        d.level = String.Format("{0:0.00}", g.WLevel * 0.101972);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.TempUnit == 4)
                {
                    var model = dbs.ToList()
                        .Where(p => p.WDatetime.Year == year && p.WDatetime.Month == month)
                    .OrderBy(p => p.WDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.WeightId;
                        d.level = String.Format("{0:0.00}", g.WLevel * 0.224809);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }
                
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("GetByDay/{id}/{year}/{month}/{day}")]
        //[Authorize()]
        public IActionResult GetByDay(int id, int year, int month, int day)
        {
            if (id == 1)
            {
                DbSet<Preference> dbs1 = _dbContext.Preference;
                int userId = 0;
                Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
                Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
                DbSet<Temperature> dbs = _dbContext.Temperature;
                if (model1.TempUnit == 1)
                {
                    var model = dbs.ToList()                    
                    .Where(p => p.TDatetime.Year == year && p.TDatetime.Month == month && p.TDatetime.Day == day)
                    .OrderBy(p => p.TDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.TemperatureId;
                        d.level = String.Format("{0:0.00}", g.TLevel);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.TempUnit == 2)
                {
                    var model = dbs.ToList()
                        .Where(p => p.TDatetime.Year == year && p.TDatetime.Month == month && p.TDatetime.Day == day)
                    .OrderBy(p => p.TDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.TemperatureId;
                        d.level = String.Format("{0:0.00}", g.TLevel + 273.15);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.TempUnit == 3)
                {
                    var model = dbs.ToList()
                        .Where(p => p.TDatetime.Year == year && p.TDatetime.Month == month && p.TDatetime.Day == day)
                    .OrderBy(p => p.TDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.TemperatureId;
                        d.level = String.Format("{0:0.00}", (g.TLevel * 1.8) + 32);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }
                
            }
            else if (id == 2)
            {
                DbSet<Humidity> dbs = _dbContext.Humidity;
                var model = dbs.ToList()
                    .Where(p => p.HDatetime.Year == year && p.HDatetime.Month == month && p.HDatetime.Day == day)
                    .OrderBy(p => p.HDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.HumidityId;
                        d.level = String.Format("{0:0.00}", g.HLevel);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.HDatetime);
                        return d;
                    });
                if (model != null)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else if (id == 3)
            {
                DbSet<Light> dbs = _dbContext.Light;
                var model = dbs.ToList()
                    .Where(p => p.LDatetime.Year == year && p.LDatetime.Month == month && p.LDatetime.Day == day)
                    .OrderBy(p => p.LDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.LightId;
                        d.level = String.Format("{0:0.00}", g.LLevel);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.LDatetime);
                        return d;
                    });
                if (model != null)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else if (id == 4)
            {
                DbSet<Preference> dbs1 = _dbContext.Preference;
                int userId = 0;
                Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
                Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
                DbSet<Weight> dbs = _dbContext.Weight;
                if (model1.WeightUnit == 1)
                {
                    var model = dbs.ToList()
                    .Where(p => p.WDatetime.Year == year && p.WDatetime.Month == month && p.WDatetime.Day == day)
                    .OrderBy(p => p.WDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.WeightId;
                        d.level = String.Format("{0:0.00}", g.WLevel);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.WeightUnit == 2)
                {
                    var model = dbs.ToList()
                        .Where(p => p.WDatetime.Year == year && p.WDatetime.Month == month && p.WDatetime.Day == day)
                    .OrderBy(p => p.WDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.WeightId;
                        d.level = String.Format("{0:0.00}", g.WLevel * 101.97);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.TempUnit == 3)
                {
                    var model = dbs.ToList()
                        .Where(p => p.WDatetime.Year == year && p.WDatetime.Month == month && p.WDatetime.Day == day)
                    .OrderBy(p => p.WDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.WeightId;
                        d.level = String.Format("{0:0.00}", g.WLevel * 0.101972);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.TempUnit == 4)
                {
                    var model = dbs.ToList()
                       .Where(p => p.WDatetime.Year == year && p.WDatetime.Month == month && p.WDatetime.Day == day)
                    .OrderBy(p => p.WDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.WeightId;
                        d.level = String.Format("{0:0.00}", g.WLevel * 0.224809);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }
                
            }
            else
            {
                return BadRequest();
            }
        }



        [HttpGet("GetByDate/{id}/{start}/{end}")]
        //[Authorize()]
        public IActionResult GetByDate(int id, DateTime start, DateTime end)
        {
            //20-06-2008 12:15:16 AM
            //if (Utils.TryParseDate(s, out DateTime start) && Utils.TryParseDate(e, out DateTime end))
            //{
            if (id == 1)
            {
                DbSet<Preference> dbs1 = _dbContext.Preference;
                int userId = 0;
                Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
                Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
                DbSet<Temperature> dbs = _dbContext.Temperature;
                if (model1.TempUnit == 1)
                {
                    var model = dbs.ToList()                    
                    .Where(p => p.TDatetime >= start && p.TDatetime <= end)
                    .OrderBy(p => p.TDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.TemperatureId;
                        d.level = String.Format("{0:0.00}", g.TLevel);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
                        return d;
                    })
                    .ToList();
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.TempUnit == 2)
                {
                    var model = dbs.ToList()
                        .Where(p => p.TDatetime >= start && p.TDatetime <= end)
                    .OrderBy(p => p.TDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.TemperatureId;
                        d.level = String.Format("{0:0.00}", g.TLevel + 273.15);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.TempUnit == 3)
                {
                    var model = dbs.ToList()
                       .Where(p => p.TDatetime >= start && p.TDatetime <= end)
                    .OrderBy(p => p.TDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.TemperatureId;
                        d.level = String.Format("{0:0.00}", (g.TLevel * 1.8) + 32);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }
                
            }
            else if (id == 2)
            {
                DbSet<Humidity> dbs = _dbContext.Humidity;
                var model = dbs.ToList()
                .OrderBy(p => p.HDatetime)
                    .Where(p => p.HDatetime >= start && p.HDatetime <= end)
                    .OrderBy(p => p.HDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.HumidityId;
                        d.level = String.Format("{0:0.00}", g.HLevel);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.HDatetime);
                        return d;
                    })
                    .ToList();
                if (model != null)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else if (id == 3)
            {
                DbSet<Light> dbs = _dbContext.Light;
                var model = dbs.ToList()
                .Where(p => p.LDatetime >= start && p.LDatetime <= end)
                .OrderBy(p => p.LDatetime)                    
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.LightId;
                        d.level = String.Format("{0:0.00}", g.LLevel);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.LDatetime);
                        return d;
                    })
                    .ToList();
                if (model != null)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else if (id == 4)
            {
                DbSet<Preference> dbs1 = _dbContext.Preference;
                int userId = 0;
                Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid).Value, out userId);
                Preference model1 = dbs1.Where(p => p.UserId == userId).FirstOrDefault();
                DbSet<Weight> dbs = _dbContext.Weight;
                if (model1.WeightUnit == 1)
                {
                    var model = dbs.ToList()                
                    .Where(p => p.WDatetime >= start && p.WDatetime <= end)
                    .OrderBy(p => p.WDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.WeightId;
                        d.level = String.Format("{0:0.00}", g.WLevel);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
                        return d;
                    })
                    .ToList();
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.WeightUnit == 2)
                {
                    var model = dbs.ToList()
                        .Where(p => p.WDatetime >= start && p.WDatetime <= end)
                    .OrderBy(p => p.WDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.WeightId;
                        d.level = String.Format("{0:0.00}", g.WLevel * 101.97);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.TempUnit == 3)
                {
                    var model = dbs.ToList()
                        .Where(p => p.WDatetime >= start && p.WDatetime <= end)
                    .OrderBy(p => p.WDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.WeightId;
                        d.level = String.Format("{0:0.00}", g.WLevel * 0.101972);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else if (model1.TempUnit == 4)
                {
                    var model = dbs.ToList()
                        .Where(p => p.WDatetime >= start && p.WDatetime <= end)
                    .OrderBy(p => p.WDatetime)
                    .Select(g =>
                    {
                        dynamic d = new ExpandoObject();
                        d.id = g.WeightId;
                        d.level = String.Format("{0:0.00}", g.WLevel * 0.224809);
                        d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
                        return d;
                    });
                    if (model != null)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }                
            }
            else
            {
                return BadRequest();
            }
        }

        ////Sort by Id
        //[HttpGet("Sort/Id/{id}/{sort}")]
        //public IActionResult SortId(int id, Boolean sort)
        //{
        //    var model = new List<dynamic>();
        //    if (id == 1)
        //    {
        //        DbSet<Temperature> dbs = _dbContext.Temperature;
        //        if (sort == true)
        //        {
        //            model = dbs.ToList()
        //            .OrderBy(p => p.TemperatureId)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.TemperatureId;
        //                d.level = String.Format("{0:0.00}", g.TLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
        //                return d;
        //            })
        //            .ToList();
        //        }
        //        else 
        //        {
        //            model = dbs.ToList()
        //            .OrderByDescending(p => p.TemperatureId)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.TemperatureId;
        //                d.level = String.Format("{0:0.00}", g.TLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
        //                return d;
        //            })
        //            .ToList();
        //        }

        //        if (model != null)
        //        {
        //            return Ok(model);
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    else if (id == 2)
        //    {
        //        DbSet<Humidity> dbs = _dbContext.Humidity;
        //        if (sort == true)
        //        {
        //            model = dbs.ToList()
        //                .OrderBy(p => p.HumidityId)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.HumidityId;
        //                d.level = String.Format("{0:0.00}", g.HLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.HDatetime);
        //                return d;
        //            })
        //            .ToList();
        //        }
        //        else 
        //        {
        //            model = dbs.ToList()
        //                .OrderByDescending(p =>p.HumidityId)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.HumidityId;
        //                d.level = String.Format("{0:0.00}", g.HLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.HDatetime);
        //                return d;
        //            })
        //            .ToList();
        //        }
        //        if (model != null)
        //        {
        //            return Ok(model);
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    else if (id == 3)
        //    {
        //        DbSet<Light> dbs = _dbContext.Light;
        //        if (sort == true)
        //        {
        //            model = dbs.ToList()
        //        .OrderBy(p => p.LightId)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.LightId;
        //                d.level = String.Format("{0:0.00}", g.LLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.LDatetime);
        //                return d;
        //            })
        //            .ToList();
        //        }
        //        else
        //        {
        //            model = dbs.ToList()
        //        .OrderByDescending(p => p.LightId)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.LightId;
        //                d.level = String.Format("{0:0.00}", g.LLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.LDatetime);
        //                return d;
        //            })
        //            .ToList();
        //        }
                
        //        if (model != null)
        //        {
        //            return Ok(model);
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    else if (id == 4)
        //    {
        //        DbSet<Weight> dbs = _dbContext.Weight;
        //        if (sort == true)
        //        {
        //            model = dbs.ToList()
        //       .OrderBy(p => p.WeightId)
        //           .Select(g =>
        //           {
        //               dynamic d = new ExpandoObject();
        //               d.id = g.WeightId;
        //               d.level = String.Format("{0:0.00}", g.WLevel);
        //               d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
        //               return d;
        //           })
        //           .ToList();
        //        }
        //        else
        //        {
        //            model = dbs.ToList()
        //       .OrderByDescending(p => p.WeightId)
        //           .Select(g =>
        //           {
        //               dynamic d = new ExpandoObject();
        //               d.id = g.WeightId;
        //               d.level = String.Format("{0:0.00}", g.WLevel);
        //               d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
        //               return d;
        //           })
        //           .ToList();
        //        }
        //        if (model != null)
        //        {
        //            return Ok(model);
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

        ////Sort by Level
        //[HttpGet("Sort/Level/{id}/{sort}")]
        //public IActionResult SortType(int id, Boolean sort)
        //{
        //    var model = new List<dynamic>();
        //    if (id == 1)
        //    {
        //        DbSet<Temperature> dbs = _dbContext.Temperature;
        //        if (sort == true)
        //        {
        //            model = dbs.ToList()
        //            .OrderBy(p => p.TLevel)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.TemperatureId;
        //                d.level = String.Format("{0:0.00}", g.TLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
        //                return d;
        //            })
        //            .ToList();
        //        }
        //        else
        //        {
        //            model = dbs.ToList()
        //            .OrderByDescending(p => p.TLevel)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.TemperatureId;
        //                d.level = String.Format("{0:0.00}", g.TLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
        //                return d;
        //            })
        //            .ToList();
        //        }

        //        if (model != null)
        //        {
        //            return Ok(model);
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    else if (id == 2)
        //    {
        //        DbSet<Humidity> dbs = _dbContext.Humidity;
        //        if (sort == true)
        //        {
        //            model = dbs.ToList()
        //                .OrderBy(p => p.HLevel)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.HumidityId;
        //                d.level = String.Format("{0:0.00}", g.HLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.HDatetime);
        //                return d;
        //            })
        //            .ToList();
        //        }
        //        else
        //        {
        //            model = dbs.ToList()
        //                .OrderByDescending(p => p.HLevel)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.HumidityId;
        //                d.level = String.Format("{0:0.00}", g.HLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.HDatetime);
        //                return d;
        //            })
        //            .ToList();
        //        }
        //        if (model != null)
        //        {
        //            return Ok(model);
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    else if (id == 3)
        //    {
        //        DbSet<Light> dbs = _dbContext.Light;
        //        if (sort == true)
        //        {
        //            model = dbs.ToList()
        //        .OrderBy(p => p.LLevel)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.LightId;
        //                d.level = String.Format("{0:0.00}", g.LLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.LDatetime);
        //                return d;
        //            })
        //            .ToList();
        //        }
        //        else
        //        {
        //            model = dbs.ToList()
        //        .OrderByDescending(p => p.LLevel)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.LightId;
        //                d.level = String.Format("{0:0.00}", g.LLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.LDatetime);
        //                return d;
        //            })
        //            .ToList();
        //        }

        //        if (model != null)
        //        {
        //            return Ok(model);
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    else if (id == 4)
        //    {
        //        DbSet<Weight> dbs = _dbContext.Weight;
        //        if (sort == true)
        //        {
        //            model = dbs.ToList()
        //       .OrderBy(p => p.WLevel)
        //           .Select(g =>
        //           {
        //               dynamic d = new ExpandoObject();
        //               d.id = g.WeightId;
        //               d.level = String.Format("{0:0.00}", g.WLevel);
        //               d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
        //               return d;
        //           })
        //           .ToList();
        //        }
        //        else
        //        {
        //            model = dbs.ToList()
        //       .OrderByDescending(p => p.WLevel)
        //           .Select(g =>
        //           {
        //               dynamic d = new ExpandoObject();
        //               d.id = g.WeightId;
        //               d.level = String.Format("{0:0.00}", g.WLevel);
        //               d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
        //               return d;
        //           })
        //           .ToList();
        //        }
        //        if (model != null)
        //        {
        //            return Ok(model);
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

        ////Sort by Date
        //[HttpGet("Sort/Date/{id}/{sort}")]
        //public IActionResult SortDate(int id, Boolean sort)
        //{
        //    var model = new List<dynamic>();
        //    if (id == 1)
        //    {
        //        DbSet<Temperature> dbs = _dbContext.Temperature;
        //        if (sort == true)
        //        {
        //            model = dbs.ToList()
        //            .OrderBy(p => p.TDatetime)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.TemperatureId;
        //                d.level = String.Format("{0:0.00}", g.TLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
        //                return d;
        //            })
        //            .ToList();
        //        }
        //        else
        //        {
        //            model = dbs.ToList()
        //            .OrderByDescending(p => p.TDatetime)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.TemperatureId;
        //                d.level = String.Format("{0:0.00}", g.TLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.TDatetime);
        //                return d;
        //            })
        //            .ToList();
        //        }

        //        if (model != null)
        //        {
        //            return Ok(model);
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    else if (id == 2)
        //    {
        //        DbSet<Humidity> dbs = _dbContext.Humidity;
        //        if (sort == true)
        //        {
        //            model = dbs.ToList()
        //                .OrderBy(p => p.HDatetime)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.HumidityId;
        //                d.level = String.Format("{0:0.00}", g.HLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.HDatetime);
        //                return d;
        //            })
        //            .ToList();
        //        }
        //        else
        //        {
        //            model = dbs.ToList()
        //                .OrderByDescending(p => p.HDatetime)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.HumidityId;
        //                d.level = String.Format("{0:0.00}", g.HLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.HDatetime);
        //                return d;
        //            })
        //            .ToList();
        //        }
        //        if (model != null)
        //        {
        //            return Ok(model);
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    else if (id == 3)
        //    {
        //        DbSet<Light> dbs = _dbContext.Light;
        //        if (sort == true)
        //        {
        //            model = dbs.ToList()
        //        .OrderBy(p => p.LDatetime)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.LightId;
        //                d.level = String.Format("{0:0.00}", g.LLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.LDatetime);
        //                return d;
        //            })
        //            .ToList();
        //        }
        //        else
        //        {
        //            model = dbs.ToList()
        //        .OrderByDescending(p => p.LDatetime)
        //            .Select(g =>
        //            {
        //                dynamic d = new ExpandoObject();
        //                d.id = g.LightId;
        //                d.level = String.Format("{0:0.00}", g.LLevel);
        //                d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.LDatetime);
        //                return d;
        //            })
        //            .ToList();
        //        }

        //        if (model != null)
        //        {
        //            return Ok(model);
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    else if (id == 4)
        //    {
        //        DbSet<Weight> dbs = _dbContext.Weight;
        //        if (sort == true)
        //        {
        //            model = dbs.ToList()
        //       .OrderBy(p => p.WDatetime)
        //           .Select(g =>
        //           {
        //               dynamic d = new ExpandoObject();
        //               d.id = g.WeightId;
        //               d.level = String.Format("{0:0.00}", g.WLevel);
        //               d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
        //               return d;
        //           })
        //           .ToList();
        //        }
        //        else
        //        {
        //            model = dbs.ToList()
        //       .OrderByDescending(p => p.WDatetime)
        //           .Select(g =>
        //           {
        //               dynamic d = new ExpandoObject();
        //               d.id = g.WeightId;
        //               d.level = String.Format("{0:0.00}", g.WLevel);
        //               d.datetime = String.Format("{0:dd-MM-yyyy hh:mm:ss tt}", g.WDatetime);
        //               return d;
        //           })
        //           .ToList();
        //        }
        //        if (model != null)
        //        {
        //            return Ok(model);
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

        //[HttpGet("Sort/Date/{id}/{sort}")]
        //public IActionResult Test(string format, int id)
        //{
            //SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            //SpreadsheetInfo.FreeLimitReached += (sender, e) => e.FreeLimitReachedAction = FreeLimitReachedAction.ContinueAsTrial;
            //GemBox.Spreadsheet.SaveOptions options = GetSaveOptions(format);
            //ExcelFile book = new ExcelFile();
            //ExcelWorksheet sheet = book.Worksheets.Add("Sheet1");

            //CellStyle style = sheet.Rows[0].Style;
            //style.Font.Weight = ExcelFont.BoldWeight;
            //style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            //sheet.Columns[0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

            //sheet.Columns[0].SetWidth(100, LengthUnit.Pixel);
            //sheet.Columns[1].SetWidth(50, LengthUnit.Pixel);
            //sheet.Columns[2].SetWidth(100, LengthUnit.Pixel);


            //sheet.Cells["A1"].Value = "ID";
            //sheet.Cells["B1"].Value = "Level";
            //sheet.Cells["C1"].Value = "Datetime";
            //if (id == 1)
            //{
            //DbSet<Temperature> dbs = _dbContext.Temperature;
            //    for (int i = 1; i < dbs.Count(); i++)
            //    {
            //        sheet.Cells[i, 0].Value = dbs.Select(b => b.TemperatureId).ToList()[i];
            //        sheet.Cells[i, 1].Value = dbs.Select(b => b.TLevel).ToList()[i];
            //        sheet.Cells[i, 2].Value = dbs.Select(b => b.TDatetime).ToList()[i];
            //    }
            //    File(GetBytes(book, options), options.ContentType, "Temperature " + DateTime.Now + "." + format);
            //return Json(dbs);

            //}
            //else if (id == 2)
            //{
            //    return Ok();
            //}
            //else if (id == 3)
            //{
            //    return Ok();
            //}
            //else if (id == 4)
            //{
            //    return Ok();
            //}
            //return BadRequest();

        //}

        //[HttpGet("GetByKeyword/{id}/{searchId}/{keyword}")]
        ////[Authorize()]
        //public IActionResult Keyword(int id, int searchId, string keyword)
        //{
        //    if (id == 1)
        //    {
        //        DbSet<Temperature> dbs = _dbContext.Temperature;
        //        var model = "";
        //        if (searchId == 1)
        //        {
        //            model = dbs.ToList()
        //            .Where(p => Convert.ToString(p.TemperatureId).Contains(keyword))
        //            .Select(p => p.TDatetime.Day)
        //            .Distinct();
        //        }

        //        if (model != null)
        //        {
        //            return Ok(model);
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    else if (id == 2)
        //    {
        //        DbSet<Humidity> dbs = _dbContext.Humidity;
        //        var model = dbs.ToList()
        //            .Where(p => p.HDatetime.Year == year && p.HDatetime.Month == month)
        //            .Select(p => p.HDatetime.Day)
        //            .Distinct();
        //        if (model != null)
        //        {
        //            return Ok(model);
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    else if (id == 3)
        //    {
        //        DbSet<Light> dbs = _dbContext.Light;
        //        var model = dbs.ToList()
        //            .Where(p => p.LDatetime.Year == year && p.LDatetime.Month == month)
        //            .Select(p => p.LDatetime.Day)
        //            .Distinct();
        //        if (model != null)
        //        {
        //            return Ok(model);
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    else if (id == 4)
        //    {
        //        DbSet<Weight> dbs = _dbContext.Weight;
        //        var model = dbs.ToList()
        //            .Where(p => p.WDatetime.Year == year && p.WDatetime.Month == month)
        //            .Select(p => p.WDatetime.Day)
        //            .Distinct();
        //        if (model != null)
        //        {
        //            return Ok(model);
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

    }
}