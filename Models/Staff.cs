using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hairstories.Models
{
    /// <summary>
    /// Staff model for salon employees
    /// </summary>
    public class Staff
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public StaffRole Role { get; set; }

        [StringLength(500)]
        public string? WorkingHours { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }

        // Foreign Key
        [ForeignKey("ApplicationUser")]
        public string? ApplicationUserId { get; set; }

        // Navigation property
        public ApplicationUser? ApplicationUser { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
        public ICollection<StaffService>? StaffServices { get; set; }

        // Display properties
        public string FullName => $"{FirstName} {LastName}";
    }

    /// <summary>
    /// Enum for Staff Roles
    /// </summary>
    public enum StaffRole
    {
        Admin,
        Receptionist,
        Stylist
    }
}
