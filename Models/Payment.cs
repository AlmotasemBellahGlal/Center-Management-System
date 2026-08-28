using System.ComponentModel.DataAnnotations;

namespace Center_Management.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        
        // إضافات جديدة - Requirements 7.1
        public int GroupId { get; set; }
        public Group? Group { get; set; }
        
        [Range(1, 12)]
        public int Month { get; set; }
        
        [Range(2000, 2100)]
        public int Year { get; set; }
    }
}
