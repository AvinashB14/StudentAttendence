using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendence.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [Required]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Present";

        [Display(Name = "Subject")]
        [StringLength(100)]
        public string Subject { get; set; }

        public DateTime MarkedOn { get; set; } = DateTime.Now;
    }
}
