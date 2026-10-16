using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendence.Models
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "Subject Name is required")]
        [Display(Name = "Subject Name")]
        [StringLength(100)]
        public string SubjectName { get; set; }

        [Required(ErrorMessage = "Subject Code is required")]
        [Display(Name = "Subject Code")]
        [StringLength(50)]
        public string SubjectCode { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [Display(Name = "Department")]
        [StringLength(100)]
        public string Department { get; set; }

        [Display(Name = "Stream")]
        [StringLength(100)]
        public string Stream { get; set; }

        [Display(Name = "Year")]
        [StringLength(50)]
        public string Year { get; set; }

        [Required(ErrorMessage = "No of Lectures in semester is required")]
        [Display(Name = "No of Lectures in Semester")]
        public int LecturesInSemester { get; set; }

        [Required(ErrorMessage = "Lecture Time is required")]
        [Display(Name = "Lecture Time")]
        [StringLength(100)]
        public string LectureTime { get; set; }

        [Display(Name = "Has Practical")]
        public bool HasPractical { get; set; }

        // Practical specific details
        [Display(Name = "Practical Subject Name")]
        [StringLength(100)]
        public string PracticalSubjectName { get; set; }

        [Display(Name = "Practical Subject Code")]
        [StringLength(50)]
        public string PracticalSubjectCode { get; set; }

        [Display(Name = "No of Practicals in Semester")]
        public int? PracticalsInSemester { get; set; }

        [Display(Name = "Practical Timing")]
        [StringLength(100)]
        public string PracticalTiming { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
