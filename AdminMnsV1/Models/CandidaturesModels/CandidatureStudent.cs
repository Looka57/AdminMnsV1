using AdminMnsV1.Models.CandidaturesModels;
using AdminMnsV1.Models.Classes;
using AdminMnsV1.Models;

namespace AdminMnsV1.Models.ViewModels

{
    public class CandidatureStudent
    {
        public List<Candidature> EnCoursCandidatures { get; set; } = new List<Candidature>();
        public List<Candidature> ValideesCandidatures { get; set;  } = new List<Candidature>();
        public List<Candidature> RefuseesCandidatures { get; set; } = new List<Candidature>();


        //Nombre de dossier par classes
        public Dictionary<string, int> DossierCountsByClass { get; set; } = new Dictionary<string, int>();
    }
}
