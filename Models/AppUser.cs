using Microsoft.AspNetCore.Identity;

namespace Center_Management.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = "";

        // For student role: link to Student record
        public int? StudentId { get; set; }
        public Student? Student { get; set; }
    }
}
