using System;
using System.ComponentModel.DataAnnotations;

namespace StudentAttendence.Models
{
    public class LocationSetting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string PlaceName { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        /// <summary>Allowed radius in metres. Default 200 m.</summary>
        [Required]
        [Range(10, 100000)]
        public int RadiusMeters { get; set; } = 200;

        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}
