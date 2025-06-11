using AdminMnsV1.Models.Abscences;

namespace AdminMnsV1.Repositories.Interfaces
{
    public interface IAbsenceRepository:IGenericRepository<Absence>
    {
        Task<IEnumerable<Absence>> GetPendingAbsencesAsync(); // Pour les absences à valider

    }
}
