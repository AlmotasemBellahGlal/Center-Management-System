using Center_Management.Models;

namespace Center_Management.View_Models
{
    public class MaterialGroupVM
    {
        public string SubjectName { get; set; } = "";

        public List<Matrial> Materials { get; set; } = new();
    }
}
