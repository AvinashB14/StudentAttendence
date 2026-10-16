using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendence.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Full Name")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Roll No is required")]
        [Display(Name = "Roll No")]
        [StringLength(20)]
        [Column("RollNo")]
        public string PRN { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [StringLength(150)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [StringLength(100)]
        public string Department { get; set; }

        [Required(ErrorMessage = "Year is required")]
        public int Year { get; set; }

        public DateTime RegisteredOn { get; set; } = DateTime.Now;

        // ── Face data ────────────────────────────────────────────────────────
        /// <summary>Base64-encoded JPEG snapshot of the student's face.</summary>
        [Column(TypeName = "nvarchar(MAX)")]
        public string PhotoBase64 { get; set; }

        /// <summary>JSON-serialised float[128] face descriptor from face-api.js.</summary>
        [Column(TypeName = "nvarchar(MAX)")]
        public string FaceDescriptor { get; set; }
    }
}
