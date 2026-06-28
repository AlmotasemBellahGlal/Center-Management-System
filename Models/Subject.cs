using System.ComponentModel.DataAnnotations;

namespace Center_Management.Models
{
    public class Subject
    {
        public int Id { get; set; }
        [MaxLength(30)]
        [Required]
        public string Name { get; set; }
        public ICollection<AcademicYear>? AcademicYears { get; set; }
        public ICollection<Matrial>? Matrials { get; set; }
    }
}
