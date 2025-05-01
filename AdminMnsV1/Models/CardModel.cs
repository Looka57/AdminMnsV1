// Models/CardModel.cs
namespace AdminMnsV1.Models
{
    //Creation des cards selon les besoins
    public class CardModel
    {
        //Cards du Dashboard
        public string Url { get; set; }
        public string Number { get; set; }
        public string Title { get; set; }
        public string IconUrl { get; set; }
        public string AltText { get; set; }

        //Cards des Classes

        public int MyProperty { get; set; }
    }
}
