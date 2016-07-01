using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bookings.DAL;
using Bookings.Models;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Bookings.Controllers
{
    public class UsersController : Controller
    {
        private MyContext db = new MyContext();

        public async Task<ActionResult> Index(int? id)
        {
            TempData["BookindId"] = id;
            var booking = await db.Bookings.Where(m => m.BookingId == id).FirstOrDefaultAsync();
            List<User> users = new List<User>();
            if (booking.Members != null)
            {
                string[] members = booking.Members.Split(',');
                for (int i = 0; i < members.Count() - 1; i++)
                {
                    int userId = Int32.Parse(members[i]);
                    users.Add(await db.Users.Where(m => m.UserId == userId).FirstOrDefaultAsync());
                }
            }
            return View(users);
        }

        public async Task<ActionResult> IndexAdmin()
        {
            if (Session["user"] != null && ((User)Session["user"]).Role == "admin")
            {
                return View(await db.Users.Where(m => m.Role != "admin").ToListAsync());
            }
            else
            {
                return RedirectToAction("Error", "Bookings");
            }
        }

        public async Task<ActionResult> CheckUsers(int? id)
        {
            if ((User)Session["user"] != null)
            {
                TempData["BookindId"] = id;
                var booking = db.Bookings.Where(m => m.BookingId == id).FirstOrDefault();
                if (booking.Members != "")
                {
                    TempData["Members"] = booking.Members;
                }
                return View(await db.Users.Where(m => m.Active == true && m.Block == false).ToListAsync());
            }
            else
            {
                return RedirectToAction("Error", "Bookings");
            }
        }

        [HttpPost]
        public async Task<ActionResult> CheckUsers(int[] deletedItems)
        {
            int bookingId = (int)TempData["BookindId"];
            string members = "";
            if (deletedItems != null)
            {
                foreach (var item in deletedItems)
                {
                    members = members + item + ",";
                }
            }
            Booking booking = await db.Bookings.FindAsync(bookingId);
            if (booking.DateOf < DateTime.Now)
            {
                TempData["error"] = "Spotkanie już się zaczęło";
                return RedirectToAction("CheckUsers", new { id = booking.BookingId });
            }
            else
            {
                TempData["ok"] = "Zaktualizowano listę uczestników";
                booking.Members = members;
                db.Entry(booking).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("IndexUser", "Bookings");
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "UserId,Nick,Name,Surname,Role,Password,PasswordRepeat,Email,Active,Block,ImageLink")] User user, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                user.Password = encrypt(user.Password);
                user.PasswordRepeat = encrypt(user.PasswordRepeat);
                user.Role = "user";
                user.Active = false;
                user.Block = false;
                var email = db.Users.Where(m => m.Email == user.Email).FirstOrDefault();
                if (email != null)
                {
                    ViewBag.Email = "Podany email jest już zajęty!";
                    return View(user);
                }
                db.Users.Add(user);
                await db.SaveChangesAsync();

                if (file != null && file.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Images/"), user.UserId.ToString() + "2.jpg");
                    file.SaveAs(path);
                    System.Drawing.Bitmap OriginalBM = new System.Drawing.Bitmap(Server.MapPath(@"~/Images/" + user.UserId.ToString() + "2.jpg"));
                    System.Drawing.Size newSize = new System.Drawing.Size(140, 140);
                    System.Drawing.Bitmap ResizedBM = new System.Drawing.Bitmap(OriginalBM, newSize);
                    ResizedBM.Save(Server.MapPath(@"~/Images/" + user.UserId.ToString() + ".jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);

                    OriginalBM.Dispose();
                    System.IO.File.Delete(path);

                    user.ImageLink = @"~/Images/" + user.UserId + ".jpg";

                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                }
                @TempData["ok"] = "Twoje konto zostało założone. Administrator musie je aktywować";
                return RedirectToAction("LogIn");
            }

            return View(user);
        }
        public string encrypt(string password)
        {
            byte[] passByte = Encoding.UTF8.GetBytes(password);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] passMD5 = md5.ComputeHash(passByte);
            string passString = BitConverter.ToString(passMD5).Replace("-", "").ToLower();
            return passString;
        }

        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(User user)
        {
            var userDB = db.Users.Where(u => u.Email == user.Email).FirstOrDefault();
            if (userDB == null)
            {
                ViewData["login"] = "Dane są niepoprawne";
            }
            else
            {
                if (userDB.Block == true)
                {
                    ViewData["login"] = "Twoje konto jest zablokowane";
                }
                else if (userDB.Active == false)
                {
                    ViewData["login"] = "Twoje konto nie zostało aktywowane";
                }
                else
                {
                    if (isValid(user.Password, userDB))
                    {
                        return RedirectToAction("Index", "Bookings");
                    }
                    else
                    {
                        ViewData["login"] = "Dane są niepoprawne";
                    }
                }
            }
            return View(user);
        }
        private bool isValid(string password, User user)
        {
            bool isValid = false;

            if (user != null)
            {
                if (user.Password == encrypt(password))
                {
                    Session["user"] = user;
                    isValid = true;
                }
            }
            return isValid;
        }

        public ActionResult LogOut()
        {
            Session.Remove("user");
            return RedirectToAction("LogIn");
        }

        public async Task<ActionResult> Active(int? id)
        {
            if (Session["user"] != null && ((User)Session["User"]).Role == "admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (ModelState.IsValid)
                {
                    User user = await db.Users.FindAsync(id);
                    user.Active = true;
                    user.PasswordRepeat = user.Password;
                    db.Entry(user).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    TempData["ok"] = "Konto zostało aktywowane";
                    return RedirectToAction("IndexAdmin");
                }
            }
            return View();
        }
        public async Task<ActionResult> Block(int? id)
        {
            if (Session["user"] != null && ((User)Session["User"]).Role == "admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (ModelState.IsValid)
                {
                    User user = await db.Users.FindAsync(id);
                    user.Block = true;
                    user.PasswordRepeat = user.Password;
                    db.Entry(user).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    TempData["ok"] = "Konto zostało zablokowane";
                    return RedirectToAction("IndexAdmin");
                }
            }
            return View();
        }
        public async Task<ActionResult> UnBlock(int? id)
        {
            if (Session["user"] != null && ((User)Session["User"]).Role == "admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (ModelState.IsValid)
                {
                    User user = await db.Users.FindAsync(id);
                    user.Block = false;
                    user.PasswordRepeat = user.Password;
                    db.Entry(user).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    TempData["ok"] = "Konto zostało odblokowane";
                    return RedirectToAction("IndexAdmin");
                }
            }
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
