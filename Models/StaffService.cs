using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hairstories.Models
{
    /// <summary>
    /// Junction entity for many-to-many relationship between Staff and Services
    /// </summary>
    public class StaffService
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StaffId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.Now;

        // Foreign Keys & Navigation properties
        [ForeignKey("StaffId")]
        public Staff? Staff { get; set; }

        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }
    }
}
