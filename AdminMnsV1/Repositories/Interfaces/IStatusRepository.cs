using AdminMnsV1.Models;

namespace AdminMnsV1.Repositories.Interfaces
{
    public interface IStatusRepository : IGenericRepository<DelayAbsStatus>
    {
        Task<IEnumerable<DelayAbsStatus>> GetAllStatusesAsync(); // Pour obtenir tous les statut
        Task<DelayAbsStatus> GetStatusByIdAsync(int id);
    }
}
