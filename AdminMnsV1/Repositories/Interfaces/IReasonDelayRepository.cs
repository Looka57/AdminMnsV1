using AdminMnsV1.Models.Delays;

namespace AdminMnsV1.Repositories.Interfaces
{
    public interface IReasonDelayRepository : IGenericRepository<ReasonDelay>
    {
        Task<IEnumerable<ReasonDelay>> GetAllReasonDelaysAsync();
        Task<ReasonDelay> GetReasonDelayByIdAsync(int id);
    }
}
        
