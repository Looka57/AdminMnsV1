using AdminMnsV1.Models.Candidature;

namespace AdminMnsV1.Repositories.Interfaces
{
    public interface ICandidatureRepository : IGenericRepository<Candidature>
    {
        Task<IEnumerable<Candidature>> GetAllCandidaturesWithDetailsAsync();
        Task<Candidature> GetCandidatureByIdWithDetailsAsync(int id);

        Task<int?> GetCandidatureStatusIdByName(string statusName); // Retourne l'ID du statut, ou null si non trouvé
    }
}
