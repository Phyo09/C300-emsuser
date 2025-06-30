using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using C300.Models;

namespace C300.Controllers
{
    [Route("api/sql")]
    public class SqlApiController : Controller
    {

        private AppDbContext _dbContext;

        public SqlApiController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("upload/{temp}/{hum}/{light}/{weight}/{date}")]
        public IActionResult uploadSQL(float temp, float hum, float light, float weight, string date)
        {/*,float hum,float light,float weight*/
         //12 - 12 - 2012 % 2012:12:13
            DateTime date1 = Convert.ToDateTime(date);
            DateTime now = DateTime.Now;
            //DateTime date = DateTime.Parse(date1);
            if (date1 <= now)
            {
                int json = 0;
                string sql1 = @"SELECT TR_datetime FROM temperature WHERE TR_datetime = '{0}'";
                var output1 = DBUtl.GetList(sql1, date);
                if (output1.Count > 0)
                {
                    //string value = output1[0];
                    json = 0;
                }
                else
                {
                    string sql = @"Insert into temperature(T_level,T_datetime,TR_datetime) 
                            Values ({0},'{1}','{2}')";
                    if (DBUtl.ExecSQL(sql, temp, string.Format("{0:yyyy-MM-dd hh:mm:ss}", now), string.Format("{0:yyyy-MM-dd hh:mm:ss}", date)) == 1)
                    {
                        json = 1;
                    }
                    else
                    {
                        json = 0;
                    }
                }

                string sql2 = @"SELECT HR_datetime FROM humidity WHERE HR_datetime = '{0}'";
                var output2 = DBUtl.GetList(sql2, date);
                if (output2.Count > 0)
                {
                    //string value = output1[0];
                    json = 0;
                }
                else
                {
                    string sql = @"Insert into humidity(H_level,H_datetime,HR_datetime) 
                            Values ({0},'{1}','{2}')";
                    if (DBUtl.ExecSQL(sql, hum, string.Format("{0:yyyy-MM-dd hh:mm:ss}", now), string.Format("{0:yyyy-MM-dd hh:mm:ss}", date)) == 1)
                    {
                        json = 1;
                    }
                    else
                    {
                        json = 0;
                    }
                }

                string sql3 = @"SELECT LR_datetime FROM light WHERE LR_datetime = '{0}'";
                var output3 = DBUtl.GetList(sql3, date);
                if (output3.Count > 0)
                {
                    //string value = output1[0];
                    json = 0;
                }
                else
                {
                    string sql = @"Insert into light(L_level,L_datetime,LR_datetime) 
                            Values ({0},'{1}','{2}')";
                    if (DBUtl.ExecSQL(sql, light, string.Format("{0:yyyy-MM-dd hh:mm:ss}", now), string.Format("{0:yyyy-MM-dd hh:mm:ss}", date)) == 1)
                    {
                        json = 1;
                    }
                    else
                    {
                        json = 0;
                    }
                }

                string sql4 = @"SELECT WR_datetime FROM weight WHERE WR_datetime = '{0}'";
                var output4 = DBUtl.GetList(sql4, date);
                if (output4.Count > 0)
                {
                    //string value = output1[0];
                    json = 0;
                }
                else
                {
                    string sql = @"Insert into weight(W_level,W_datetime,WR_datetime) 
                            Values ({0},'{1}','{2}')";
                    if (DBUtl.ExecSQL(sql, weight, string.Format("{0:yyyy-MM-dd hh:mm:ss}", now), string.Format("{0:yyyy-MM-dd hh:mm:ss}", date)) == 1)
                    {
                        json = 1;
                    }
                    else
                    {
                        json = 0;
                    }
                }
                return Json(json);
            }
            else
            {
                return Json("big");
            }

            //int json = 0;
            //string sql1 = @"SELECT T_datetime FROM temperature WHERE T_datetime = '{0}'";
            //var output1 = DBUtl.GetList(sql1, date);
            //if (output1.Count > 0)
            //{
            //    //string value = output1[0];
            //    json = 0;
            //}
            //else
            //{
            //    string sql = @"Insert into temperature(T_level,T_datetime) 
            //            Values ({0},'{1}')";
            //    if (DBUtl.ExecSQL(sql, temp, string.Format("{0:yyyy-MM-dd hh:mm:ss}", date)) == 1)
            //    {
            //        json = 1;
            //    }
            //    else
            //    {
            //        json = 0;
            //    }
            //}

            //string sql2 = @"SELECT H_datetime FROM humidity WHERE H_datetime = '{0}'";
            //var output2 = DBUtl.GetList(sql2, date);
            //if (output2.Count > 0)
            //{
            //    //string value = output1[0];
            //    json = 0;
            //}
            //else
            //{
            //    string sql = @"Insert into humidity(H_level,H_datetime) 
            //            Values ({0},'{1}')";
            //    if (DBUtl.ExecSQL(sql, hum, string.Format("{0:yyyy-MM-dd hh:mm:ss}", date)) == 1)
            //    {
            //        json = 1;
            //    }
            //    else
            //    {
            //        json = 0;
            //    }
            //}

            //string sql3 = @"SELECT L_datetime FROM light WHERE L_datetime = '{0}'";
            //var output3 = DBUtl.GetList(sql3, date);
            //if (output3.Count > 0)
            //{
            //    //string value = output1[0];
            //    json = 0;
            //}
            //else
            //{
            //    string sql = @"Insert into light(L_level,L_datetime) 
            //            Values ({0},'{1}')";
            //    if (DBUtl.ExecSQL(sql, light, string.Format("{0:yyyy-MM-dd hh:mm:ss}", date)) == 1)
            //    {
            //        json = 1;
            //    }
            //    else
            //    {
            //        json = 0;
            //    }
            //}

            //string sql4 = @"SELECT W_datetime FROM weight WHERE W_datetime = '{0}'";
            //var output4 = DBUtl.GetList(sql4, date);
            //if (output4.Count > 0)
            //{
            //    //string value = output1[0];
            //    json = 0;
            //}
            //else
            //{
            //    string sql = @"Insert into weight(W_level,W_datetime) 
            //            Values ({0},'{1}')";
            //    if (DBUtl.ExecSQL(sql, weight, string.Format("{0:yyyy-MM-dd hh:mm:ss}", date)) == 1)
            //    {
            //        json = 1;
            //    }
            //    else
            //    {
            //        json = 0;
            //    }
            //}

            //if (date <= DateTime.Now)
            //{                
            //    string sql1 = @"SELECT T_datetime FROM temperature WHERE T_datetime = '{0}'";

            //    var output1 = DBUtl.GetList(sql1, date);
            //    if (output1.Count > 0)
            //    {
            //        //string value = output1[0];
            //        json = 0;
            //    }
            //    else
            //    {
            //        string sql = @"Insert into temperature(T_level,T_datetime) 
            //            Values ({0},'{1}')";
            //        if (DBUtl.ExecSQL(sql, temp, string.Format("{0:yyyy-MM-dd hh:mm:ss}", date)) == 1)
            //        {
            //            json = 1;
            //        }
            //        else
            //        {
            //            json = 0;
            //        }
            //    }

            //    string sql2 = @"SELECT H_datetime FROM humidity WHERE H_datetime = '{0}'";
            //    var output2 = DBUtl.GetList(sql2, date);
            //    if (output2.Count > 0)
            //    {
            //        //string value = output1[0];
            //        json = 0;
            //    }
            //    else
            //    {
            //        string sql = @"Insert into humidity(H_level,H_datetime) 
            //            Values ({0},'{1}')";
            //        if (DBUtl.ExecSQL(sql, hum, string.Format("{0:yyyy-MM-dd hh:mm:ss}", date)) == 1)
            //        {
            //            json = 1;
            //        }
            //        else
            //        {
            //            json = 0;
            //        }
            //    }

            //    string sql3 = @"SELECT L_datetime FROM light WHERE L_datetime = '{0}'";
            //    var output3 = DBUtl.GetList(sql3, date);
            //    if (output3.Count > 0)
            //    {
            //        //string value = output1[0];
            //        json = 0;
            //    }
            //    else
            //    {
            //        string sql = @"Insert into light(L_level,L_datetime) 
            //            Values ({0},'{1}')";
            //        if (DBUtl.ExecSQL(sql, light, string.Format("{0:yyyy-MM-dd hh:mm:ss}", date)) == 1)
            //        {
            //            json = 1;
            //        }
            //        else
            //        {
            //            json = 0;
            //        }
            //    }

            //    string sql4 = @"SELECT W_datetime FROM weight WHERE W_datetime = '{0}'";
            //    var output4 = DBUtl.GetList(sql4, date);
            //    if (output4.Count > 0)
            //    {
            //        //string value = output1[0];
            //        json = 0;
            //    }
            //    else
            //    {
            //        string sql = @"Insert into weight(W_level,W_datetime) 
            //            Values ({0},'{1}')";
            //        if (DBUtl.ExecSQL(sql, weight, string.Format("{0:yyyy-MM-dd hh:mm:ss}", date)) == 1)
            //        {
            //            json = 1;
            //        }
            //        else
            //        {
            //            json = 0;
            //        }
            //    }

            //}
            //else
            //{
            //    json = 0;
            //}
            //return Json(json);
        }

        [HttpGet("checkstatus/{date}/{field1}")]
        public IActionResult checkStatus(string date,float field1)
        {
            DateTime date1 = Convert.ToDateTime(date);
            //DateTime date = DateTime.Parse(date1);
            if ((DateTime.Now - date1).TotalSeconds <= 31 && field1!=0.0)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
        [HttpGet("checkstatus2/{date}/{field3}")]
        public IActionResult checkStatus2(string date, float field3)
        {
            DateTime date1 = Convert.ToDateTime(date);
            //DateTime date = DateTime.Parse(date1);
            if ((DateTime.Now - date1).TotalSeconds <= 31 && field3 != 0.0)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
        [HttpGet("checkstatus3/{date}/{field2}")]
        public IActionResult checkStatus3(string date, float field2)
        {
            DateTime date1 = Convert.ToDateTime(date);
            //DateTime date = DateTime.Parse(date1);
            if ((DateTime.Now - date1).TotalSeconds <= 31 && field2 != 0.0) 
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
        [HttpGet("checkstatus4/{date}/{field4}")]
        public IActionResult checkStatus4(string date,float field4)
        {
            DateTime date1 = Convert.ToDateTime(date);
            //DateTime date = DateTime.Parse(date1);
            if ((DateTime.Now - date1).TotalSeconds <= 31 && field4 != 0.0)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
    }
}