using Center_Management.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Center_Management.View_Models
{
    public class CreateGroupVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int AcademicYearId { get; set; }

        public List<GroupScheduleVM> Schedules { get; set; } = new();
    }
}
