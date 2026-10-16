using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using StudentAttendence.Models;

namespace StudentAttendence.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly AttendanceDbContext _db = new AttendanceDbContext();
        private const double FaceMatchThreshold = 0.50;

        // GET: Attendance/Mark
        public ActionResult Mark()
        {
            return View();
        }

        // POST: Attendance/ScanAndMark  (called via AJAX with face descriptor and GPS coords)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ScanAndMark(string faceDescriptor, double? latitude, double? longitude)
        {
            if (string.IsNullOrWhiteSpace(faceDescriptor))
                return Json(new { success = false, error = "No face descriptor received." });

            // ── Validate Location Constraint (Web.config or Database) ─────────
            string campusName = ConfigurationManager.AppSettings["CampusName"] ?? "College Campus";
            double campusLat = 0, campusLon = 0;
            int campusRadius = 200;

            bool hasConfigLoc = double.TryParse(ConfigurationManager.AppSettings["CampusLatitude"], NumberStyles.Float, CultureInfo.InvariantCulture, out campusLat)
                             && double.TryParse(ConfigurationManager.AppSettings["CampusLongitude"], NumberStyles.Float, CultureInfo.InvariantCulture, out campusLon);
            int.TryParse(ConfigurationManager.AppSettings["CampusRadiusMeters"], out campusRadius);
            if (campusRadius <= 0) campusRadius = 200;

            // If not found in Web.config, check LocationSettings table
            if (!hasConfigLoc || (campusLat == 0 && campusLon == 0))
            {
                var dbLoc = _db.LocationSettings.OrderByDescending(s => s.Id).FirstOrDefault();
                if (dbLoc != null && (dbLoc.Latitude != 0 || dbLoc.Longitude != 0))
                {
                    campusName = dbLoc.PlaceName;
                    campusLat = dbLoc.Latitude;
                    campusLon = dbLoc.Longitude;
                    campusRadius = dbLoc.RadiusMeters;
                    hasConfigLoc = true;
                }
            }

            if (hasConfigLoc && (campusLat != 0 || campusLon != 0))
            {
                if (!latitude.HasValue || !longitude.HasValue)
                {
                    return Json(new
                    {
                        success = false,
                        outOfLocation = true,
                        error = "Location permission is required to mark attendance. Please enable device GPS."
                    });
                }

                double distanceMeters = CalculateDistanceMeters(
                    campusLat, campusLon,
                    latitude.Value, longitude.Value
                );

                if (distanceMeters > campusRadius)
                {
                    return Json(new
                    {
                        success = false,
                        outOfLocation = true,
                        distance = Math.Round(distanceMeters),
                        allowedRadius = campusRadius,
                        campus = campusName,
                        error = "You are out of the location! Allowed within " + campusRadius + "m of " + campusName + ", but you are " + Math.Round(distanceMeters) + "m away."
                    });
                }
            }

            // ── Parse incoming descriptor ─────────────────────────────────────
            float[] incoming = ParseDescriptor(faceDescriptor);
            if (incoming == null)
                return Json(new { success = false, error = "Invalid face data. Please try again." });

            // ── Compare against every registered student ───────────────────────
            var studentsWithFace = _db.Students
                .Where(s => s.FaceDescriptor != null && s.FaceDescriptor != "")
                .ToList();

            if (!studentsWithFace.Any())
                return Json(new { success = false, notFound = true,
                    error = "No registered students with face data found. Please register students first." });

            Student bestMatch = null;
            double  bestDist  = double.MaxValue;

            foreach (var s in studentsWithFace)
            {
                float[] stored = ParseDescriptor(s.FaceDescriptor);
                if (stored == null) continue;

                double dist = EuclideanDistance(incoming, stored);
                if (dist < bestDist)
                {
                    bestDist  = dist;
                    bestMatch = s;
                }
            }

            if (bestMatch == null || bestDist >= FaceMatchThreshold)
                return Json(new { success = false, notFound = true,
                    error = "Face not recognised. Make sure you are registered. (Distance: " + bestDist.ToString("F3") + ")" });

            // ── Fetch subjects for student's department (or program prefix) ──
            var studentDept = (bestMatch.Department ?? "").Trim();
            var subjectsQuery = _db.Subjects.AsQueryable();
            if (!string.IsNullOrEmpty(studentDept))
            {
                var deptSubjects = subjectsQuery.Where(s => s.Department != null && 
                    (s.Department.Trim() == studentDept || 
                     s.Department.Trim().StartsWith(studentDept) || 
                     studentDept.StartsWith(s.Department.Trim()))).ToList();

                // If department-specific subjects exist, use them; otherwise fallback to all subjects
                if (deptSubjects.Any())
                    subjectsQuery = deptSubjects.AsQueryable();
            }

            var subjectList = subjectsQuery
                .OrderBy(s => s.SubjectName)
                .Select(s => new
                {
                    subjectId = s.SubjectId,
                    subjectName = s.SubjectName,
                    subjectCode = s.SubjectCode,
                    lectureTime = s.LectureTime,
                    department = s.Department
                })
                .ToList();

            if (!subjectList.Any())
            {
                // Fallback: check all subjects in database
                subjectList = _db.Subjects
                    .OrderBy(s => s.SubjectName)
                    .Select(s => new
                    {
                        subjectId = s.SubjectId,
                        subjectName = s.SubjectName,
                        subjectCode = s.SubjectCode,
                        lectureTime = s.LectureTime,
                        department = s.Department
                    })
                    .ToList();
            }

            return Json(new
            {
                success = true,
                faceVerified = true,
                studentId = bestMatch.StudentId,
                studentName = bestMatch.FullName,
                prn = bestMatch.PRN,
                department = bestMatch.Department,
                year = bestMatch.Year,
                photo = bestMatch.PhotoBase64 ?? "",
                subjects = subjectList
            });
        }

        // POST: Attendance/MarkAttendanceForSubject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkAttendanceForSubject(int studentId, int subjectId)
        {
            var student = _db.Students.Find(studentId);
            if (student == null)
                return Json(new { success = false, message = "Student record not found." });

            var subject = _db.Subjects.Find(subjectId);
            if (subject == null)
                return Json(new { success = false, message = "Selected subject not found in the database." });

            // ── Lecture timing check ──────────────────────────────────────────
            var now = DateTime.Now;
            var currentTime = now.TimeOfDay;

            TimeSpan startTime, endTime;
            bool parsedTiming = TryParseLectureTime(subject.LectureTime, out startTime, out endTime);

            if (!parsedTiming)
            {
                return Json(new
                {
                    success = false,
                    timingError = true,
                    message = $"Invalid lecture time format configured for '{subject.SubjectName}' ({subject.LectureTime})."
                });
            }

            // Check if current time is before start time or after end time
            if (currentTime < startTime)
            {
                string scheduledStart = DateTime.Today.Add(startTime).ToString("hh:mm tt");
                string scheduledEnd = DateTime.Today.Add(endTime).ToString("hh:mm tt");
                string currentStr = now.ToString("hh:mm tt");

                return Json(new
                {
                    success = false,
                    timingError = true,
                    isEarly = true,
                    subjectName = subject.SubjectName,
                    lectureTime = subject.LectureTime,
                    currentTime = currentStr,
                    message = $"Lecture has not started yet! Attendance for '{subject.SubjectName}' can only be marked during lecture time ({scheduledStart} to {scheduledEnd}). Current time is {currentStr}."
                });
            }

            if (currentTime > endTime)
            {
                string scheduledStart = DateTime.Today.Add(startTime).ToString("hh:mm tt");
                string scheduledEnd = DateTime.Today.Add(endTime).ToString("hh:mm tt");
                string currentStr = now.ToString("hh:mm tt");

                return Json(new
                {
                    success = false,
                    timingError = true,
                    isLate = true,
                    subjectName = subject.SubjectName,
                    lectureTime = subject.LectureTime,
                    currentTime = currentStr,
                    message = $"Lecture has already ended! Attendance for '{subject.SubjectName}' closed at {scheduledEnd}. Current time is {currentStr}."
                });
            }

            // ── Duplicate attendance check for this subject on today ──────────
            var today = DateTime.Today;
            var existingRecord = _db.Attendances.FirstOrDefault(a =>
                a.StudentId == student.StudentId &&
                a.Date == today &&
                a.Subject == subject.SubjectName);

            if (existingRecord != null)
            {
                return Json(new
                {
                    success = false,
                    alreadyMarked = true,
                    studentName = student.FullName,
                    prn = student.PRN,
                    department = student.Department,
                    year = student.Year,
                    date = today.ToString("dd MMMM yyyy"),
                    markedAt = existingRecord.MarkedOn.ToString("hh:mm:ss tt"),
                    subject = existingRecord.Subject,
                    photo = student.PhotoBase64 ?? "",
                    message = $"Attendance for '{subject.SubjectName}' has already been marked today at {existingRecord.MarkedOn:hh:mm:ss tt}."
                });
            }

            // ── Save Attendance Record ─────────────────────────────────────────
            var attendance = new Attendance
            {
                StudentId = student.StudentId,
                Date = today,
                Status = "Present",
                Subject = subject.SubjectName,
                MarkedOn = now
            };
            _db.Attendances.Add(attendance);
            _db.SaveChanges();

            return Json(new
            {
                success = true,
                studentName = student.FullName,
                prn = student.PRN,
                department = student.Department,
                year = student.Year,
                date = today.ToString("dd MMMM yyyy"),
                time = now.ToString("hh:mm:ss tt"),
                subject = subject.SubjectName,
                lectureTime = subject.LectureTime,
                photo = student.PhotoBase64 ?? "",
                message = $"Attendance marked successfully for '{subject.SubjectName}'!"
            });
        }

        /// <summary>
        /// Parses lecture time strings such as "10:00 AM - 11:00 AM" or "10:00 AM to 11:00 AM".
        /// </summary>
        private static bool TryParseLectureTime(string lectureTimeStr, out TimeSpan startTime, out TimeSpan endTime)
        {
            startTime = TimeSpan.Zero;
            endTime = TimeSpan.Zero;

            if (string.IsNullOrWhiteSpace(lectureTimeStr))
                return false;

            string normalized = lectureTimeStr.Replace(" to ", " - ");
            var parts = normalized.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                return false;

            DateTime dtStart, dtEnd;
            string startStr = parts[0].Trim();
            string endStr = parts[1].Trim();

            string[] formats = { "h:mm tt", "hh:mm tt", "H:mm", "HH:mm", "h:mmtt", "hh:mmtt" };

            bool parseStart = DateTime.TryParseExact(startStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dtStart)
                           || DateTime.TryParse(startStr, out dtStart);

            bool parseEnd = DateTime.TryParseExact(endStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dtEnd)
                         || DateTime.TryParse(endStr, out dtEnd);

            if (parseStart && parseEnd)
            {
                startTime = dtStart.TimeOfDay;
                endTime = dtEnd.TimeOfDay;
                return true;
            }

            return false;
        }

        // GET: Attendance/Confirm/{studentId}  (kept for backward compat)
        public ActionResult Confirm(int studentId)
        {
            var student = _db.Students.Find(studentId);
            if (student == null) return RedirectToAction("Mark");

            var studentDept = (student.Department ?? "").Trim();
            var subjects = _db.Subjects
                .Where(s => string.IsNullOrEmpty(studentDept) || s.Department == null || s.Department.Trim() == studentDept)
                .OrderBy(s => s.SubjectName)
                .ToList();

            if (!subjects.Any())
            {
                subjects = _db.Subjects.OrderBy(s => s.SubjectName).ToList();
            }

            ViewBag.Subjects = subjects;
            return View(student);
        }

        // POST: Attendance/Confirm  (kept for backward compat)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(int studentId, int? subjectId)
        {
            var student = _db.Students.Find(studentId);
            if (student == null) return RedirectToAction("Mark");

            if (!subjectId.HasValue)
            {
                TempData["WarningMessage"] = "Please select a subject to mark attendance.";
                return RedirectToAction("Confirm", new { studentId = studentId });
            }

            var subject = _db.Subjects.Find(subjectId.Value);
            if (subject == null)
            {
                TempData["WarningMessage"] = "Selected subject not found in database.";
                return RedirectToAction("Confirm", new { studentId = studentId });
            }

            // ── Validate timing ──
            var now = DateTime.Now;
            var currentTime = now.TimeOfDay;
            TimeSpan startTime, endTime;
            if (TryParseLectureTime(subject.LectureTime, out startTime, out endTime))
            {
                if (currentTime < startTime)
                {
                    string scheduledStart = DateTime.Today.Add(startTime).ToString("hh:mm tt");
                    string scheduledEnd = DateTime.Today.Add(endTime).ToString("hh:mm tt");
                    TempData["WarningMessage"] = $"Lecture has not started yet! Attendance for '{subject.SubjectName}' can only be marked between {scheduledStart} and {scheduledEnd}.";
                    return RedirectToAction("Confirm", new { studentId = studentId });
                }
                if (currentTime > endTime)
                {
                    string scheduledEnd = DateTime.Today.Add(endTime).ToString("hh:mm tt");
                    TempData["WarningMessage"] = $"Lecture has already ended! Attendance for '{subject.SubjectName}' closed at {scheduledEnd}.";
                    return RedirectToAction("Confirm", new { studentId = studentId });
                }
            }

            var today = DateTime.Today;
            bool alreadyMarked = _db.Attendances.Any(a => a.StudentId == studentId && a.Date == today && a.Subject == subject.SubjectName);
            if (alreadyMarked)
            {
                TempData["WarningMessage"] = $"Attendance for \"{student.FullName}\" for subject '{subject.SubjectName}' is already marked for today.";
                return RedirectToAction("Mark");
            }

            var attendance = new Attendance
            {
                StudentId = studentId,
                Date      = today,
                Status    = "Present",
                Subject   = subject.SubjectName,
                MarkedOn  = now
            };
            _db.Attendances.Add(attendance);
            _db.SaveChanges();

            TempData["SuccessMessage"] = $"Attendance marked for \"{student.FullName}\" in '{subject.SubjectName}' — {today:dd MMM yyyy}.";
            return RedirectToAction("Mark");
        }

        // GET: Attendance/Report
        public ActionResult Report(string reportType, int? studentId, int? month, int? year, string department, int? classYear, DateTime? date)
        {
            var vm = new AttendanceReportViewModel();
            vm.ReportType = string.IsNullOrEmpty(reportType) ? "StudentWise" : reportType;

            // Load filter master data
            vm.AllStudents = _db.Students.OrderBy(s => s.FullName).ToList();
            vm.Departments = _db.Students.Select(s => s.Department).Distinct().OrderBy(d => d).ToList();
            vm.ClassYears  = _db.Students.Select(s => s.Year).Distinct().OrderBy(y => y).ToList();

            // Set filter defaults
            int targetMonth = month.HasValue && month.Value >= 1 && month.Value <= 12 ? month.Value : DateTime.Today.Month;
            int targetYear  = year.HasValue && year.Value >= 2000 ? year.Value : DateTime.Today.Year;
            DateTime targetDate = date.HasValue ? date.Value.Date : DateTime.Today;

            vm.SelectedMonth = targetMonth;
            vm.SelectedYear = targetYear;
            vm.SelectedDate = targetDate;
            vm.SelectedDepartment = department ?? (vm.Departments.FirstOrDefault() ?? "");
            vm.SelectedClassYear = classYear;

            // Select default student if none selected
            if (!studentId.HasValue && vm.AllStudents.Any())
            {
                studentId = vm.AllStudents.First().StudentId;
            }
            vm.SelectedStudentId = studentId;

            // ─────────────────────────────────────────────────────────────
            // 1. Particular Student Monthly Percentage Report
            // ─────────────────────────────────────────────────────────────
            if (vm.SelectedStudentId.HasValue)
            {
                var student = _db.Students.Find(vm.SelectedStudentId.Value);
                if (student != null)
                {
                    var summary = new StudentMonthlySummary
                    {
                        Student = student,
                        Month = targetMonth,
                        Year = targetYear,
                        MonthName = new DateTime(targetYear, targetMonth, 1).ToString("MMMM yyyy")
                    };

                    int daysInMonth = DateTime.DaysInMonth(targetYear, targetMonth);
                    DateTime startOfMonth = new DateTime(targetYear, targetMonth, 1);
                    DateTime endOfMonth = new DateTime(targetYear, targetMonth, daysInMonth);

                    // Fetch attendance records of this student in the selected month
                    var studentMonthlyRecords = _db.Attendances
                        .Where(a => a.StudentId == student.StudentId && a.Date >= startOfMonth && a.Date <= endOfMonth)
                        .ToList();

                    // Count working days (exclude Sundays, or count distinct attendance days recorded in system)
                    var distinctCollegeDays = _db.Attendances
                        .Where(a => a.Date >= startOfMonth && a.Date <= endOfMonth)
                        .Select(a => a.Date)
                        .Distinct()
                        .ToList();

                    // If college had recorded sessions, use that count; otherwise use days up to today or daysInMonth (excluding Sunday)
                    int workingDaysCount = 0;
                    DateTime limitDate = (targetYear == DateTime.Today.Year && targetMonth == DateTime.Today.Month)
                        ? DateTime.Today
                        : endOfMonth;

                    var dayList = new List<StudentDayAttendanceItem>();
                    for (DateTime d = startOfMonth; d <= limitDate; d = d.AddDays(1))
                    {
                        if (d.DayOfWeek != DayOfWeek.Sunday)
                        {
                            workingDaysCount++;
                            var rec = studentMonthlyRecords.FirstOrDefault(r => r.Date == d.Date);
                            dayList.Add(new StudentDayAttendanceItem
                            {
                                Date = d,
                                IsPresent = rec != null,
                                Time = rec != null ? rec.MarkedOn.ToString("hh:mm tt") : "-",
                                Subject = rec != null ? (rec.Subject ?? "-") : "-"
                            });
                        }
                    }

                    int presentCount = studentMonthlyRecords.Select(r => r.Date).Distinct().Count();
                    if (workingDaysCount < presentCount) workingDaysCount = presentCount;
                    if (workingDaysCount == 0) workingDaysCount = 1;

                    int absentCount = Math.Max(0, workingDaysCount - presentCount);
                    double presentPct = Math.Round(((double)presentCount / workingDaysCount) * 100.0, 1);
                    double absentPct = Math.Round(100.0 - presentPct, 1);

                    summary.TotalWorkingDays = workingDaysCount;
                    summary.PresentDays = presentCount;
                    summary.AbsentDays = absentCount;
                    summary.PresentPercentage = presentPct;
                    summary.AbsentPercentage = absentPct;
                    summary.DailyBreakdown = dayList.OrderByDescending(x => x.Date).ToList();

                    vm.StudentSummary = summary;
                }
            }

            // ─────────────────────────────────────────────────────────────
            // 2. Particular Class Day-Wise Attendance Percentage Report
            // ─────────────────────────────────────────────────────────────
            var classQuery = _db.Students.AsQueryable();
            if (!string.IsNullOrEmpty(vm.SelectedDepartment))
            {
                classQuery = classQuery.Where(s => s.Department == vm.SelectedDepartment);
            }
            if (vm.SelectedClassYear.HasValue)
            {
                classQuery = classQuery.Where(s => s.Year == vm.SelectedClassYear.Value);
            }

            var classStudents = classQuery.OrderBy(s => s.PRN).ToList();
            int totalClassStudents = classStudents.Count;

            // Fetch attendances marked on this specific date for these students
            var dayAttendances = _db.Attendances
                .Where(a => a.Date == targetDate)
                .ToList();

            var studentItems = new List<ClassStudentAttendanceItem>();
            int classPresentCount = 0;

            foreach (var st in classStudents)
            {
                var att = dayAttendances.FirstOrDefault(a => a.StudentId == st.StudentId);
                bool isPres = att != null;
                if (isPres) classPresentCount++;

                studentItems.Add(new ClassStudentAttendanceItem
                {
                    StudentId = st.StudentId,
                    FullName = st.FullName,
                    PRN = st.PRN,
                    Department = st.Department,
                    Year = st.Year,
                    IsPresent = isPres,
                    MarkedTime = att != null ? att.MarkedOn.ToString("hh:mm tt") : "-",
                    PhotoBase64 = st.PhotoBase64
                });
            }

            int classAbsentCount = totalClassStudents - classPresentCount;
            double classPresentPct = totalClassStudents > 0
                ? Math.Round(((double)classPresentCount / totalClassStudents) * 100.0, 1)
                : 0.0;
            double classAbsentPct = totalClassStudents > 0
                ? Math.Round(100.0 - classPresentPct, 1)
                : 0.0;

            vm.ClassSummary = new ClassDayWiseSummary
            {
                Department = vm.SelectedDepartment,
                ClassYear = vm.SelectedClassYear,
                Date = targetDate,
                TotalStudents = totalClassStudents,
                PresentCount = classPresentCount,
                AbsentCount = classAbsentCount,
                PresentPercentage = classPresentPct,
                AbsentPercentage = classAbsentPct,
                Students = studentItems
            };

            return View(vm);
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
                var ser = new JavaScriptSerializer();
                if (json.TrimStart().StartsWith("["))
                    return ser.Deserialize<float[]>(json);

                var dict = ser.Deserialize<Dictionary<string, float>>(json);
                var arr  = new float[dict.Count];
                for (int i = 0; i < dict.Count; i++) arr[i] = dict[i.ToString()];
                return arr;
            }
            catch { return null; }
        }

        private static double EuclideanDistance(float[] a, float[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return double.MaxValue;
            double sum = 0;
            for (int i = 0; i < a.Length; i++) sum += (a[i] - b[i]) * (a[i] - b[i]);
            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Haversine formula to compute great-circle distance between two GPS coordinates in meters.
        /// </summary>
        private static double CalculateDistanceMeters(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Earth radius in meters
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }
    }
}
