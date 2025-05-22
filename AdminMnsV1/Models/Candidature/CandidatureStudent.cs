using AdminMnsV1.Models.Candidature;
using AdminMnsV1.Models.Classes;
using AdminMnsV1.Models;

namespace AdminMnsV1.Models.Candidature

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
