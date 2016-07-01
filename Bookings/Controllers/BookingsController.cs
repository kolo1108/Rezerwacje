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

namespace Bookings.Controllers
{
    public class BookingsController : Controller
    {
        private MyContext db = new MyContext();
        private DateTime dateOf;
        private DateTime dateTo;

        public async Task<ActionResult> Index()
        {
            var bookings = db.Bookings.Where(m => m.DateOf > DateTime.Now);
            return View(await bookings.OrderBy(m => m.DateOf).ToListAsync());
        }
        public async Task<ActionResult> IndexUser()
        {
            if (Session["user"] != null)
            {
                int userId = (int)((User)Session["user"]).UserId;
                var bookings = db.Bookings.Where(m => m.UserId == userId).OrderBy(m => m.DateOf);
                return View(await bookings.ToListAsync());
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public ActionResult Create()
        {
            if (Session["user"] != null)
            {
                ViewBag.UserId = new SelectList(db.Users, "UserId", "Nick");
                return View();
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "BookingId,UserId,Name,Description,CreateBook")] Booking booking)
        {
            if (Session["user"] != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (Session["user"] != null)
                        {
                            var HourOf = Request.Params["Hour"];
                            var MinutsOf = Request.Params["Minuts"];
                            var HourTo = Request.Params["Hour2"];
                            var MinutsTo = Request.Params["Minuts2"];
                            dateOf = new DateTime();
                            dateTo = new DateTime();

                            dateOf = DateTime.Parse(Request.Form["CreateBook"]);
                            TimeSpan ts = new TimeSpan(Convert.ToInt16(HourOf), Convert.ToInt16(MinutsOf), 0);
                            dateOf = dateOf.Date + ts;

                            dateTo = DateTime.Parse(Request.Form["CreateBook"]);
                            ts = new TimeSpan(Convert.ToInt16(HourTo), Convert.ToInt16(MinutsTo), 0);
                            dateTo = dateTo.Date + ts;

                            var book = db.Bookings.Where(m => m.DateOf <= dateTo && m.DateTo >= dateOf).FirstOrDefault();
                            if (dateOf < DateTime.Now)
                            {
                                ViewBag.Error = "Ten dzień już był";
                            }
                            else if (dateOf > dateTo)
                            {
                                ViewBag.Error = "Data początkowa jest większa od końcowej";
                            }
                            else if (book != null)
                            {
                                ViewBag.Error = "Ten termin jest już zajęty";
                            }

                            else
                            {
                                booking.UserId = ((User)Session["user"]).UserId;
                                booking.CreateBook = DateTime.Now;
                                booking.DateOf = dateOf;
                                booking.DateTo = dateTo;
                                db.Bookings.Add(booking);
                                await db.SaveChangesAsync();
                                return RedirectToAction("CheckUsers", "Users", new { id = booking.BookingId });
                            }
                        }
                    }
                    catch
                    {
                        ViewBag.Error = "Data została źle wprowadzona";
                    }
                }

            }
            else
            {
                return View("Error");
            }

            return View(booking);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (Session["user"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Booking booking = await db.Bookings.FindAsync(id);
                if (booking == null)
                {
                    return HttpNotFound();
                }
                ViewBag.UserId = new SelectList(db.Users, "UserId", "Nick", booking.UserId);
                return View(booking);
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "BookingId,UserId,Name,Description,CreateBook,DateOf,DateTo,Members")] Booking booking)
        {
            if (Session["user"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (booking.DateOf < DateTime.Now)
                    {
                        TempData["error"] = "Spotkanie już się zaczęło/odbyło";
                        return RedirectToAction("Edit", new { id = booking.BookingId });
                    }
                    else
                    {
                        @TempData["ok"] = "Edycja przebiegła pomyślnie";
                        db.Entry(booking).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        return RedirectToAction("IndexUser");
                    }
                }
            }
            else
            {
                return View("Error");
            }
            return View(booking);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (Session["user"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Booking booking = await db.Bookings.FindAsync(id);
                if (booking == null)
                {
                    return HttpNotFound();
                }
                return View(booking);
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Booking booking = await db.Bookings.FindAsync(id);
            db.Bookings.Remove(booking);
            await db.SaveChangesAsync();
            @TempData["ok"] = "Usunięto rezerwację";
            return RedirectToAction("IndexUser");
        }

        public ActionResult Error()
        {
            if (Session["user"] != null)
            {
                return View();
            }
            else
            {
                return View("Error");
            }
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
