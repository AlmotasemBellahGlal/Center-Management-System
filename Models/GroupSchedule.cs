using System.ComponentModel.DataAnnotations.Schema;

namespace Center_Management.Models
{
    public enum DaysOfWeek
    {
        السبت,
        الأحد,
        الإثنين,
        الثلاثاء,
        الأربعاء,
        الخميس,
        الجمعة
    }
    [Table("GroupSchedule")]
    public class GroupSchedule
    {
        public int Id { get; set; }

        public DaysOfWeek Day { get; set; }
        public TimeOnly StartTime { get; set; }

        public int GroupId { get; set; }

        public Group? Group { get; set; }
    }
}
