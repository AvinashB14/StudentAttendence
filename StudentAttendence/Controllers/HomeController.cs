using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using StudentAttendence.Models;

namespace StudentAttendence.Controllers
{
    public class HomeController : Controller
    {
        private readonly AttendanceDbContext _db = new AttendanceDbContext();

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        // GET: Home/GetStreams
        [HttpGet]
        public JsonResult GetStreams()
        {
            try
            {
                var streamList = _db.Streams.OrderBy(s => s.StreamId).ToList();
                var streams = streamList.Select(s => new { streamId = s.StreamId, streamName = s.StreamName }).ToList();
                return Json(new { success = true, streams = streams }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Home/GetDepartmentsByStream?streamName=Science
        [HttpGet]
        public JsonResult GetDepartmentsByStream(string streamName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(streamName))
                    return Json(new { success = true, departments = new string[0] }, JsonRequestBehavior.AllowGet);

                // Resolve stream name → id first, then filter departments by StreamId
                string nameLower = streamName.Trim().ToLower();
                var stream = _db.Streams.ToList()
                                .FirstOrDefault(s => s.StreamName.ToLower() == nameLower);

                if (stream == null)
                    return Json(new { success = true, departments = new string[0] }, JsonRequestBehavior.AllowGet);

                int sid = stream.StreamId;
                var depts = _db.Departments.ToList()
                                .Where(d => d.StreamId == sid)
                                .OrderBy(d => d.DepartmentId)
                                .Select(d => d.DepartmentName)
                                .ToList();

                return Json(new { success = true, departments = depts }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Home/AddSubject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddSubject(Subject model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.SubjectName))
                    return Json(new { success = false, message = "Subject name is required." });

                if (string.IsNullOrWhiteSpace(model.SubjectCode))
                    return Json(new { success = false, message = "Subject code is required." });

                if (string.IsNullOrWhiteSpace(model.Department))
                    return Json(new { success = false, message = "Please select a department." });

                if (model.LecturesInSemester <= 0)
                    return Json(new { success = false, message = "Please enter a valid number of lectures in semester." });

                if (string.IsNullOrWhiteSpace(model.LectureTime))
                    return Json(new { success = false, message = "Lecture time is required." });

                if (model.HasPractical)
                {
                    if (string.IsNullOrWhiteSpace(model.PracticalSubjectName))
                        return Json(new { success = false, message = "Practical subject name is required." });

                    if (string.IsNullOrWhiteSpace(model.PracticalSubjectCode))
                        return Json(new { practical = true, success = false, message = "Practical subject code is required." });

                    if (!model.PracticalsInSemester.HasValue || model.PracticalsInSemester.Value <= 0)
                        return Json(new { success = false, message = "Please enter a valid number of practicals in semester." });

                    if (string.IsNullOrWhiteSpace(model.PracticalTiming))
                        return Json(new { success = false, message = "Practical timing is required." });
                }
                else
                {
                    model.PracticalSubjectName = null;
                    model.PracticalSubjectCode = null;
                    model.PracticalsInSemester = null;
                    model.PracticalTiming = null;
                }

                model.CreatedOn = DateTime.Now;
                _db.Subjects.Add(model);
                _db.SaveChanges();

                return Json(new { success = true, message = "Subject added successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error saving subject: " + ex.Message });
            }
        }

        // POST: Home/AddPractical
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddPractical(Subject model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.SubjectName))
                    return Json(new { success = false, message = "Practical subject name is required." });

                if (string.IsNullOrWhiteSpace(model.SubjectCode))
                    return Json(new { success = false, message = "Practical subject code is required." });

                if (string.IsNullOrWhiteSpace(model.Department))
                    return Json(new { success = false, message = "Please select a department." });

                if (model.LecturesInSemester <= 0)
                    return Json(new { success = false, message = "Please enter a valid number of practicals in semester." });

                if (string.IsNullOrWhiteSpace(model.LectureTime))
                    return Json(new { success = false, message = "Practical timing is required." });

                // Mark as practical subject in database
                model.HasPractical = true;
                model.PracticalSubjectName = model.SubjectName;
                model.PracticalSubjectCode = model.SubjectCode;
                model.PracticalsInSemester = model.LecturesInSemester;
                model.PracticalTiming = model.LectureTime;
                model.CreatedOn = DateTime.Now;

                _db.Subjects.Add(model);
                _db.SaveChanges();

                return Json(new { success = true, message = "Practical subject added successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error saving practical subject: " + ex.Message });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
