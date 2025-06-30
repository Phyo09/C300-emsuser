using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using C300.Models;

namespace C300.Controllers
{
    public class CaptchaController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.IsVerified = "";
            return View();
        }

        [HttpPost]
        public IActionResult Index(string CaptchaCode)
        {
            if (CaptchaCode == HttpContext.Session.GetString("CaptchaCode"))
            {
                ViewBag.IsVerified = "Verified";
            }
            else
            {
                ViewBag.IsVerified = "Verify failed";
            }

            return View();
        }

        public FileStreamResult GetImage()
        {
            int width = 200;
            int height = 60;
            var captchaCode = Captcha.GenerateCaptchaCode();

            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);

            HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);

            Stream s = new MemoryStream(result.CaptchaByteData);

            return new FileStreamResult(s, "image/png");
        }

    }
}