using Center_Management.Models;
using System.ComponentModel.DataAnnotations;

namespace Center_Management.View_Models
{
    public class CreateMaterialVM
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = "";

        [Required]
        public MaterialType Type { get; set; }

        // اختيار نوع الرفع - ملف محلي أو رابط
        public bool IsLocalFile { get; set; } = true;

        // لرفع الملف المحلي
        public IFormFile? LocalFile { get; set; }

        // للرابط الخارجي
        [Url]
        public string? FileUrl { get; set; }

        [Required]
        public int AcademicYearId { get; set; }

        public List<AcademicYear> AcademicYears { get; set; } = new();
    }
}
