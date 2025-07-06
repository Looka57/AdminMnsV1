namespace AdminMnsV1.Models.Students
{
    public class StudentSearchViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty; // Sera rempli depuis SchoolClass
    }
}
