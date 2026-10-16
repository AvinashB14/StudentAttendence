using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendence.Models
{
    [Table("Streams")]
    public class AcademicStream
    {
        [Key]
        public int StreamId { get; set; }

        [Required]
        [StringLength(100)]
        public string StreamName { get; set; }
    }
}
