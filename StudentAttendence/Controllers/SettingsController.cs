using System;
using System.Linq;
using System.Web.Mvc;
using StudentAttendence.Models;

namespace StudentAttendence.Controllers
{
    public class SettingsController : Controller
    {
        private readonly AttendanceDbContext _db = new AttendanceDbContext();

        // GET: Settings/Location
        public ActionResult Location()
        {
            var setting = _db.LocationSettings.OrderByDescending(s => s.Id).FirstOrDefault();
            if (setting == null)
            {
                setting = new LocationSetting
                {
                    PlaceName = "College Campus",
                    Latitude = 0.0,
                    Longitude = 0.0,
                    RadiusMeters = 200
                };
            }
            return View(setting);
        }

        // POST: Settings/Location
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Location(LocationSetting model)
        {
            if (ModelState.IsValid)
            {
                var existing = _db.LocationSettings.OrderByDescending(s => s.Id).FirstOrDefault();
                if (existing != null)
                {
                    existing.PlaceName = model.PlaceName;
                    existing.Latitude = model.Latitude;
                    existing.Longitude = model.Longitude;
                    existing.RadiusMeters = model.RadiusMeters;
                    existing.UpdatedOn = DateTime.Now;
                }
                else
                {
                    model.UpdatedOn = DateTime.Now;
                    _db.LocationSettings.Add(model);
                }

                _db.SaveChanges();
                TempData["SuccessMessage"] = "College location settings updated successfully!";
                return RedirectToAction("Location");
            }

            return View(model);
        }

        // API to fetch current active location setting (used by Attendance scan)
        [HttpGet]
        public ActionResult GetCurrentLocationSetting()
        {
            var setting = _db.LocationSettings.OrderByDescending(s => s.Id).FirstOrDefault();
            if (setting == null)
            {
                return Json(new { enabled = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                enabled = true,
                placeName = setting.PlaceName,
                latitude = setting.Latitude,
                longitude = setting.Longitude,
                radiusMeters = setting.RadiusMeters
            }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
