using System;
using System.Collections.Generic;

namespace StudentAttendence.Models
{
    // Filter & selection parameters
    public class AttendanceReportViewModel
    {
        public string ReportType { get; set; } = "StudentWise"; // "StudentWise" or "ClassDayWise"

        // Student-wise filters
        public int? SelectedStudentId { get; set; }
        public int SelectedMonth { get; set; } = DateTime.Today.Month;
        public int SelectedYear { get; set; } = DateTime.Today.Year;
        public StudentMonthlySummary StudentSummary { get; set; }

        // Class day-wise filters
        public string SelectedDepartment { get; set; }
        public int? SelectedClassYear { get; set; }
        public DateTime SelectedDate { get; set; } = DateTime.Today;
        public ClassDayWiseSummary ClassSummary { get; set; }

        // Select lists
        public List<Student> AllStudents { get; set; } = new List<Student>();
        public List<string> Departments { get; set; } = new List<string>();
        public List<int> ClassYears { get; set; } = new List<int>();
    }

    // Report Type 1: Particular student monthly report
    public class StudentMonthlySummary
    {
        public Student Student { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; }
        public int TotalWorkingDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public double PresentPercentage { get; set; }
        public double AbsentPercentage { get; set; }
        public List<StudentDayAttendanceItem> DailyBreakdown { get; set; } = new List<StudentDayAttendanceItem>();
    }

    public class StudentDayAttendanceItem
    {
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
        public string Time { get; set; }
        public string Subject { get; set; }
    }

    // Report Type 2: Particular class day-wise report
    public class ClassDayWiseSummary
    {
        public string Department { get; set; }
        public int? ClassYear { get; set; }
        public DateTime Date { get; set; }
        public int TotalStudents { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public double PresentPercentage { get; set; }
        public double AbsentPercentage { get; set; }
        public List<ClassStudentAttendanceItem> Students { get; set; } = new List<ClassStudentAttendanceItem>();
    }

    public class ClassStudentAttendanceItem
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public string PRN { get; set; }
        public string Department { get; set; }
        public int Year { get; set; }
        public bool IsPresent { get; set; }
        public string MarkedTime { get; set; }
        public string PhotoBase64 { get; set; }
    }
}
