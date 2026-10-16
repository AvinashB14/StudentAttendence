using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendence.Models
{
    [Table("Departments")]
    public class AcademicDepartment
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(150)]
        public string DepartmentName { get; set; }

        public int StreamId { get; set; }

        [ForeignKey("StreamId")]
        public virtual AcademicStream Stream { get; set; }
    }
}
