using Microsoft.AspNetCore.Identity;

namespace Hairstories.Models
{
    /// <summary>
    /// Extended Identity User for the application
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation property
        public Staff? Staff { get; set; }

        // Display property
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
