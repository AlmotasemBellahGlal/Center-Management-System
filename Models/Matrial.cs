using System.ComponentModel.DataAnnotations;

namespace Center_Management.Models
{
    public enum MaterialType
    {
        PDF,
        Video
    }
    public class Matrial
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public MaterialType Type { get; set; }
        [Required]
        public string FileUrl { get; set; }
        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; }
    }
}
