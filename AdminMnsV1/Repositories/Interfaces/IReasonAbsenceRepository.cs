using AdminMnsV1.Models.Absences;

namespace AdminMnsV1.Repositories.Interfaces
{
    public interface IReasonAbsenceRepository: IGenericRepository<ReasonAbsence>
    {
        Task<IEnumerable<ReasonAbsence>> GetPendingReasonsAsync(); // Pour les raisons d'absence à valider
        Task<IEnumerable<ReasonAbsence>> GetAllReasonsAsync(); // Pour obtenir toutes les raisons d'absence
        Task<ReasonAbsence> GetReasonAbsenceByIdAsync(int id);
    }
}
