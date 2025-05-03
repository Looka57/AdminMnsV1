namespace AdminMnsV1.Models
{
    public class Administrator : User
    {
        public string Service { get; set; }
        public string Role { get; set; }
    }
}
