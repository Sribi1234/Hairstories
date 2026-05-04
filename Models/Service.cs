using System.ComponentModel.DataAnnotations;

namespace Hairstories.Models
{
    /// <summary>
    /// Service model for salon services offered
    /// </summary>
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Service Name is required")]
        [StringLength(100)]
        public string ServiceName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(5, 480, ErrorMessage = "Duration must be between 5 and 480 minutes")]
        public int DurationMinutes { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }

        // Navigation property
        public ICollection<Appointment>? Appointments { get; set; }
        public ICollection<StaffService>? StaffServices { get; set; }
    }
}
