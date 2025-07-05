using AdminMnsV1.Repositories.Interfaces;

namespace AdminMnsV1.Services.Interfaces
{
    public interface IDelayService : IGenericRepository<Delay>
    {
       Task<IEnumerable<Delay>> GetPendingDelaysAsync();
         Task ValidateDelayAsync(int delayId, string administratorId, int statusValidatedId);
        Task RejectDelayAsync(int delayId, string administratorId, int statusRejectedId, string reason);
      
    }
}
