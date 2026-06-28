namespace Center_Management.View_Models
{
    public class AcademicYearGroupsVM
    {
        public int AcademicYearId { get; set; }

        public string? AcademicYearName { get; set; }

        public string? SubjectName { get; set; }

        public int? SelectedGroupId { get; set; }

        public List<GroupSelectionVM> Groups { get; set; } = new();
    }
}
