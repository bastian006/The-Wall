using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TheWall.Models;
using models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace TheWall.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in user");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                dbContext.Add(user);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("LoggedUser", user.UserId);
                return RedirectToAction("Wall");
            }
            return View("Index");
        }
        [HttpPost("logging")]
        public IActionResult Logging(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u=> u.Email == userSubmission.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalide Email/Password");
                    return View("Index");

                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("Email", "Invalide Email/Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("LoggedUser", userInDb.UserId);
                return RedirectToAction("Wall");
            }
            return View("Index");
        }
        [HttpGet("wall")]
        public IActionResult Wall(){
            if(HttpContext.Session.GetInt32("LoggedUser") == null){
                return RedirectToAction("Index");
            }
            ViewBag.msgs = dbContext.Messages.Include(u => u.WroteBy).Include(c => c.Comments).ThenInclude(i => i.User).OrderByDescending(time => time.CreatedAt);
            ViewBag.me = (int)HttpContext.Session.GetInt32("LoggedUser");
            return View();
        }
        [HttpPost("createMsg")]
        public IActionResult CreateMsg(Message msg)
        {
            dbContext.Add(msg);
            dbContext.SaveChanges();
            return RedirectToAction("Wall");
        }
        [HttpPost("createCmnt")]
        public IActionResult CreateCmnt(int UId, int MId, string text)
        {
            Comment cmnt = new Comment(){
                UserId = UId,
                MessageId = MId,
                Text = text
            };
            dbContext.Add(cmnt);
            dbContext.SaveChanges();
            return RedirectToAction("Wall");
        }
    }
}
