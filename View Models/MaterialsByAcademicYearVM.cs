namespace Center_Management.View_Models
{
    public class MaterialsByAcademicYearVM
    {
        public string AcademicYearName { get; set; } = "";

        public List<MaterialGroupVM> SubjectGroups { get; set; } = new();
    }
}
