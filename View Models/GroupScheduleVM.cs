using Center_Management.Models;

namespace Center_Management.View_Models
{
    public class GroupScheduleVM
    {
        public int Id { get; set; }
        public DaysOfWeek Day { get; set; }

        public TimeOnly StartTime { get; set; }
    }
}
