using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using StudentAttendence.Models;

namespace StudentAttendence.Controllers
{
    public class StudentController : Controller
    {
        private readonly AttendanceDbContext _db = new AttendanceDbContext();
        private const double FaceMatchThreshold = 0.50; // lower = stricter

        // GET: Student/Register
        public ActionResult Register()
        {
            ViewBag.Departments = _db.Students.Select(s => s.Department).Distinct().OrderBy(d => d).ToList();
            ViewBag.Years = _db.Students.Select(s => s.Year).Distinct().OrderBy(y => y).ToList();
            return View();
        }

        // POST: Student/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Student student, string PhotoBase64, string FaceDescriptor)
        {
            ViewBag.Departments = _db.Students.Select(s => s.Department).Distinct().OrderBy(d => d).ToList();
            ViewBag.Years = _db.Students.Select(s => s.Year).Distinct().OrderBy(y => y).ToList();

            // ── Require face capture ──────────────────────────────────────────
            if (string.IsNullOrWhiteSpace(PhotoBase64) || string.IsNullOrWhiteSpace(FaceDescriptor))
            {
                ViewBag.DuplicateError = null;
                ViewBag.FaceError = "Please capture your face photo before submitting the form.";
                return View(student);
            }

            if (ModelState.IsValid)
            {
                // ── Check 1: Duplicate Roll No ─────────────────────────
                bool prnExists = _db.Students.Any(s => s.PRN.Trim() == student.PRN.Trim());
                if (prnExists)
                {
                    var existing = _db.Students.FirstOrDefault(s => s.PRN.Trim() == student.PRN.Trim());
                    ViewBag.DuplicateError =
                        $"A student with Roll No \"{student.PRN}\" is already registered" +
                        (existing != null ? $" as \"{existing.FullName}\"." : ".");
                    return View(student);
                }

                // ── Check 2: Duplicate Full Name + Email ──────────────────────
                bool nameEmailExists = _db.Students.Any(s =>
                    s.Email.Trim().ToLower()    == student.Email.Trim().ToLower());
                if (nameEmailExists)
                {
                    ViewBag.DuplicateError =
                        $"A student with email \"{student.Email}\" is already registered.";
                    return View(student);
                }

                // ── Check 3: Duplicate Face ───────────────────────────────────
                float[] newDesc = ParseDescriptor(FaceDescriptor);
                if (newDesc != null)
                {
                    var studentsWithFace = _db.Students
                        .Where(s => s.FaceDescriptor != null && s.FaceDescriptor != "")
                        .ToList();

                    foreach (var s in studentsWithFace)
                    {
                        float[] existingDesc = ParseDescriptor(s.FaceDescriptor);
                        if (existingDesc != null)
                        {
                            double dist = EuclideanDistance(newDesc, existingDesc);
                            if (dist < FaceMatchThreshold)
                            {
                                ViewBag.DuplicateError =
                                    $"This face is already registered as \"{s.FullName}\" (Roll No: {s.PRN}). " +
                                    $"Duplicate face registration is not allowed.";
                                return View(student);
                            }
                        }
                    }
                }

                // ── Save ──────────────────────────────────────────────────────
                student.RegisteredOn  = DateTime.Now;
                student.PhotoBase64   = PhotoBase64;
                student.FaceDescriptor = FaceDescriptor;
                _db.Students.Add(student);
                _db.SaveChanges();

                TempData["SuccessMessage"] = $"Student \"{student.FullName}\" registered successfully with Roll No {student.PRN}...";
                return RedirectToAction("Register");
            }

            return View(student);
        }

        // GET: Student/List
        public ActionResult List()
        {
            var students = _db.Students.OrderBy(s => s.FullName).ToList();
            return View(students);
        }

        // POST: Student/RemoveByPrn  (Remove a particular student using Roll No)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveByPrn(string prn)
        {
            if (string.IsNullOrWhiteSpace(prn))
                return Json(new { success = false, message = "Please enter a valid Roll Number." });

            var student = _db.Students.FirstOrDefault(s => s.PRN.Trim() == prn.Trim());
            if (student == null)
            {
                return Json(new { success = false, message = $"No student found with Roll No \"{prn.Trim()}\"." });
            }

            string studentName = student.FullName;
            string studentPrn = student.PRN;
            string dept = student.Department;
            int yr = student.Year;

            // Remove associated attendance records first
            var attendances = _db.Attendances.Where(a => a.StudentId == student.StudentId).ToList();
            if (attendances.Any())
            {
                _db.Attendances.RemoveRange(attendances);
            }

            _db.Students.Remove(student);
            _db.SaveChanges();

            return Json(new
            {
                success = true,
                message = $"Student \"{studentName}\" (Roll No: {studentPrn}, {dept}, Year {yr}) removed successfully from the database."
            });
        }

        // POST: Student/RemoveClass  (Remove all students belonging to a class)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveClass(string department, int? year)
        {
            if (string.IsNullOrWhiteSpace(department) || !year.HasValue)
            {
                return Json(new { success = false, message = "Please select both Department and Year." });
            }

            var classStudents = _db.Students
                .Where(s => s.Department.Trim() == department.Trim() && s.Year == year.Value)
                .ToList();

            if (!classStudents.Any())
            {
                return Json(new { success = false, message = $"No students found in {department} (Year {year.Value})." });
            }

            int count = classStudents.Count;
            var studentIds = classStudents.Select(s => s.StudentId).ToList();

            // Remove all associated attendance records for these students
            var attendances = _db.Attendances.Where(a => studentIds.Contains(a.StudentId)).ToList();
            if (attendances.Any())
            {
                _db.Attendances.RemoveRange(attendances);
            }

            _db.Students.RemoveRange(classStudents);
            _db.SaveChanges();

            return Json(new
            {
                success = true,
                message = $"All {count} student(s) from class {department} (Year {year.Value}) removed successfully from the database."
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static float[] ParseDescriptor(string json)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                // face-api.js sends Float32Array serialised as a plain JS object {0:v,1:v,...}
                // or a JSON array [v,v,...] — handle both
                if (json.TrimStart().StartsWith("["))
                {
                    return serializer.Deserialize<float[]>(json);
                }
                // object form: {"0":0.12,"1":-0.03,...}
                var dict = serializer.Deserialize<Dictionary<string, float>>(json);
                var arr = new float[dict.Count];
                for (int i = 0; i < dict.Count; i++) arr[i] = dict[i.ToString()];
                return arr;
            }
            catch { return null; }
        }

        private static double EuclideanDistance(float[] a, float[] b)
        {
            if (a.Length != b.Length) return double.MaxValue;
            double sum = 0;
            for (int i = 0; i < a.Length; i++)
                sum += (a[i] - b[i]) * (a[i] - b[i]);
            return Math.Sqrt(sum);
        }
    }
}
