using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hairstories.Models
{
    /// <summary>
    /// Appointment model for salon bookings
    /// </summary>
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer is required")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Service is required")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Staff is required")]
        public int StaffId { get; set; }

        [Required(ErrorMessage = "Appointment Date and Time is required")]
        public DateTime AppointmentDateTime { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        [Required]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }

        // Foreign Keys
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }

        [ForeignKey("StaffId")]
        public Staff? Staff { get; set; }

        // Display/Calculated properties
        public decimal ServicePrice => Service?.Price ?? 0;
        
        public DateTime AppointmentEndTime => 
            AppointmentDateTime.AddMinutes(Service?.DurationMinutes ?? 0);
    }

    /// <summary>
    /// Enum for Appointment Status
    /// </summary>
    public enum AppointmentStatus
    {
        Scheduled,
        Completed,
        Cancelled,
        NoShow
    }

    /// <summary>
    /// Enum for Payment Status
    /// </summary>
    public enum PaymentStatus
    {
        Pending,
        Paid,
        Refunded
    }
}
