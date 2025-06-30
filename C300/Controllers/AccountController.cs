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
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace C300.Controllers
{
    public class AccountController : Controller
    {
        private static string AuthScheme = "UserSecurity";
        private AppDbContext _dbContext;
        private IHttpContextAccessor _contextAccessor;
        private HttpContext _context { get { return _contextAccessor.HttpContext; } }

        public AccountController(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
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
            PreparePreference();
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

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            PreparePreference();
            ViewData["Layout"] = "_Layout";
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Login(string CaptchaCode, LoginUser user,
                                    string returnUrl = null)
        {
            PreparePreference();
            ViewData["ReturnUrl"] = returnUrl;
            ClaimsPrincipal principal = null;
            bool isCaptchaValid = false;

            if (HttpContext.Session.GetString("CaptchaCode") != null && CaptchaCode == HttpContext.Session.GetString("CaptchaCode"))
            {
                isCaptchaValid = true;
            }

            if (SecureValidUser(user.Email, user.Password, out principal) && isCaptchaValid == true)
            {
                HttpContext.Authentication.SignInAsync(AuthScheme, principal);
                if (returnUrl == null)
                {
                    BacklogActivity("Logged In");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var list = returnUrl.Split('/');
                    return RedirectToAction(list[2], list[1]);
                }

            }

            else if (!SecureValidUser(user.Email, user.Password, out principal) && isCaptchaValid == false)
            {
                ViewData["Message"] = "Captcha and Email address or Password is incorrect!";
                ViewData["Layout"] = "_Layout";
                return View("Login");
            }

            else if (isCaptchaValid == false)
            {
                ViewData["Message"] = "Captcha did not match! Please try again";
                ViewData["Layout"] = "_Layout";
                return View("Login");

            }

            else
            {
                ViewData["Message"] = "Incorrect Email address or Password";
                ViewData["Layout"] = "_Layout";
                return View("Login");
            }
        }

        public IActionResult Logoff(string returnUrl = null)
        {
            PreparePreference();
            var backlog = new Backlog()
            {
                Name = userEmail,
                Action = "Logged Out",
                Datetime = DateTime.Now,
                UserId = Convert.ToInt32(User.FindFirst(ClaimTypes.Sid).Value)
            };

            _dbContext.Backlog.Add(backlog);

            _dbContext.SaveChanges();
            HttpContext.Authentication.SignOutAsync(AuthScheme);


            return RedirectToAction("Index", "Home");
        }

        public IActionResult Forbidden(string returnUrl = null)
        {
            PreparePreference();
            ViewData["Layout"] = "_Layout";
            return View();
        }

        private bool SecureValidUser(string uid,
                                     string pw,
                                     out ClaimsPrincipal principal)
        {
            string returnUrl = ViewData["ReturnUrl"] as string;

            string sql = "";
            sql = $"SELECT * FROM Emsuser WHERE Email='{uid}' AND Password = HASHBYTES('SHA1','{pw}')";

            DbSet<Emsuser> dbs = _dbContext.Emsuser;
            Emsuser user = dbs.FromSql(sql)
                                  .FirstOrDefault();


            principal = null;
            if (user != null)
            {

                principal =
                   new ClaimsPrincipal(
                   new ClaimsIdentity(
                      new Claim[] {
                     new Claim(ClaimTypes.NameIdentifier,User.ToString()),
                     new Claim(ClaimTypes.Sid, user.UserId.ToString()),
                     new Claim(ClaimTypes.Name, user.Name),
                     new Claim(ClaimTypes.GivenName,user.Name),
                     new Claim(ClaimTypes.Role, user.Role),
                     new Claim(ClaimTypes.Email,user.Email.ToString())
                      },
                      "Basic"));
                return true;
            }
            else
            {
                return false;
            }
        }


        public IActionResult Registration()
        {
            PreparePreference();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registration(Emsuser newUser, IFormFile photo)
        {
            PreparePreference();
            if (ModelState.IsValid)
            {
                DbSet<Emsuser> dbs = _dbContext.Emsuser;
                var tnewUser = dbs.Where(r => r.Email == newUser.Email).FirstOrDefault();
                if (tnewUser != null)
                    ViewData["Msg"] = "same email address has been in the system";

                else
                {
                    //generate activationcode
                    newUser.ActivationCode = Guid.NewGuid();

                    //password hashing
                    //newUser.Password = Crypto.Hash(newUser.Password);
                    // newUser.ConfirmPassword = Crypto.Hash(newUser.ConfirmPassword);
                    // Hash the password before inserting
                    //using (var sha1 = SHA1.Create())
                    //{
                     //   var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(newUser.Password));
                     //   newUser.Password = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                    //}
                    newUser.IsVerified = false;
                    newUser.Role = "Member";
                    if (HttpContext.Request.Form.Files != null)
                    {
                        var files = HttpContext.Request.Form.Files;
                        string base64string = string.Empty;
                        foreach (var file in files)
                        {
                            if (file.Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    file.CopyTo(ms);
                                    var fileBytes = ms.ToArray();
                                    base64string = Convert.ToBase64String(fileBytes);
                                }
                            }
                        }
                        newUser.Picture = base64string;

                    }


                    // dbs.Add(newUser);
                    string sql = @"INSERT INTO Emsuser (Name , Role , Password , Country, Dob , Email , Picture , ActivationCode , IsVerified , ResetPasswordCode,Status,Phone,UpdatedBy) VALUES('{0}' , '{1}', HASHBYTES('SHA1','{2}'), '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}','Active',null,null)";
                    //string sql = @"INSERT INTO Emsuser (Name , Role , Password , Country, Dob , Email , Picture , ActivationCode , IsVerified , ResetPasswordCode,Status,Phone,UpdatedBy) VALUES('{0}' , '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}','Active',null,null)";
                    Console.WriteLine(string.Format(sql, newUser.Name, newUser.Role, newUser.Password, newUser.Country, newUser.Dob, newUser.Email, newUser.Picture, newUser.ActivationCode, newUser.IsVerified, newUser.ResetPasswordCode));
                    try
                    {
                        int rowAffected = DBUtl.ExecSQL(sql, newUser.Name, newUser.Role, newUser.Password, newUser.Country, newUser.Dob, newUser.Email, newUser.Picture, newUser.ActivationCode, newUser.IsVerified, newUser.ResetPasswordCode);
                        if (rowAffected == 1)
                        {
                            BacklogActivity("User Registered");
                            //SendVerificationLinkEmail(newUser.Email, newUser.ActivationCode.ToString());// Service ended
                            ViewData["Msg"] = "Account Activation Link has been sent to your email ";
                        }
                        else
                        {
                            ViewData["Msg"] = "Failed to register into the system."+ rowAffected;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error to console or file for debugging
                        Console.WriteLine("Error during user registration: " + ex.Message);
                        ViewData["Msg"] = "An error occurred: " + ex.Message;

                        // Optional: log full details
                        // Console.WriteLine(ex.StackTrace);
                    }
                }

            }

            else
                ViewData["Msg"] = "Invalid information entered";

            return View();







        }

        //private bool UploadFile(IFormFile ufile, string fname)
        //{

        //    if (ufile.Length > 0)
        //    {
        //        string fullpath = Path.Combine(_env.WebRootPath, fname);
        //        using (var fileStream = new FileStream(fullpath, FileMode.Create))

        //        {
        //            ufile.CopyTo(fileStream);
        //        }
        //        return true;
        //    }
        //    return false;
        //}

        [HttpGet]
        public IActionResult VerifyAccount(string id)
        {
            PreparePreference();
            DbSet<Emsuser> dbs = _dbContext.Emsuser;

            var v = dbs.Where(x => x.ActivationCode == new Guid(id)).FirstOrDefault();
            if (v != null)
            {

                
                string sql = @"INSERT INTO Preference(
UserId,Lowest_temp,Highest_temp,Temp_unit,Temp_noti,
Lowest_humidity,Highest_humidity,Hum_unit,Hum_noti,
Lowest_light,Highest_light,Light_unit,Light_noti,
Lowest_weight,Highest_weight,Weight_unit,Weight_noti) 
VALUES
({0},10,40,1,1,30,50,1,1,20,40,1,1,10,20,1,1)";
                if (DBUtl.ExecSQL(sql,v.UserId) == 1)
                {
                    string sql1 = @"INSERT INTO Dash(User_id,Cur,Tdy,Yes,LW,L15,LM,LT,LH,LL,LF,LA,AH1,AH2,AH3,AH4,AH5,OTH,OTL,OHH,OHL,OLH,OLL,OWH,OWL) VALUES('{0}',1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1);";
                    if (DBUtl.ExecSQL(sql1, v.UserId) == 1)
                    {
                        v.IsVerified = true;
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        ViewBag.Message = "Invalid Dashboard";
                    }
                }
                else
                {
                    ViewBag.Message = "Invalid Preference";
                }
            }



            else
            {
                ViewBag.Message = "Invalid Request";
            }


            return View();
        }

        [NonAction]
        public void SendVerificationLinkEmail(string Email, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/Account/" + emailFor + "/" + activationCode;
            //var link = Request.Url.AbsoluteUri.Replace(Request.Url)
            string link = ControllerContext.HttpContext.Request.Scheme.ToString() + "://" + ControllerContext.HttpContext.Request.Host.Value.ToString()
                    + verifyUrl;

            var fromEmail = new MailAddress("hyejinfyp98@gmail.com");
            var toEmail = new MailAddress(Email);
            var fromEmailPassword = "Iloveusehun1@";
            string subject = "";
            string body = "";


            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";

                body = "<br/><br/>We are excited to tell you that your account is" +
           " successfully created. Please click on the below link to verify your account" +
           " <br/><br/><a href='" + link + "'>Verify Account Link</a> ";
            }

            else if (emailFor == "Reset")
            {
                subject = "Reset Password";
                body = "Hi,<br/><br/>We got request for reset your account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";
            }


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(fromEmail.Address, fromEmailPassword)

            };

            // Service ended
            /*using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);*/


        }

        public IActionResult ForgetPassword()
        {
            PreparePreference();
            return View();
        }


        [HttpPost]
        public IActionResult ForgetPassword(string Email)
        {
            PreparePreference();
            DbSet<Emsuser> dbs = _dbContext.Emsuser;
            var account = dbs.Where(x => x.Email == Email).FirstOrDefault();
            if (account != null)
            {

                if (account.IsVerified == true)
                {
                    string resetCode = Guid.NewGuid().ToString();
                    //SendVerificationLinkEmail(account.Email, resetCode, "Reset"); //Service Ended
                    account.ResetPasswordCode = resetCode;
                    _dbContext.SaveChanges();
                    ViewData["Msg"] = "Reset password link has been sent to your email id";
                }

                else
                    ViewData["Msg"] = "Your account hasn't been verified yet.Check your Email to access our verification link.";


            }

            else
            {
                ViewData["Msg"] = "Account not found!";
            }

            return View();
        }

        public IActionResult Reset(string id)
        {
            PreparePreference();
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            DbSet<Emsuser> dbs = _dbContext.Emsuser;
            var user = dbs.Where(x => x.ResetPasswordCode == id).FirstOrDefault();
            if (user != null)
            {
                ResetPasswordModel model = new ResetPasswordModel();
                model.ResetCode = id;
                return View(model);
            }


            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reset(ResetPasswordModel model)
        {
            PreparePreference();
            if (ModelState.IsValid)
            {
                DbSet<Emsuser> dbs = _dbContext.Emsuser;
                var user = dbs.Where(x => x.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                if (user != null)
                {
                    if (DBUtl.ExecSQL(@"UPDATE Emsuser SET Password= HASHBYTES('SHA1' , '{0}') WHERE Email='{1}' ", model.NewPassword, user.Email) == 1)
                    {
                        user.ResetPasswordCode = "";
                        ViewData["Msg"] = "New password updated successfully";
                    }
                    //user.Password = model.NewPassword;

                    //_dbContext.SaveChanges();

                }
            }

            else
                ViewData["Msg"] = "Invalid Input Information";
            return View(model);
        }

    }

}