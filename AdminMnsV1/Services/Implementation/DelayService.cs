using AdminMnsV1.Repositories.Interfaces;
using AdminMnsV1.Services.Interfaces;

namespace AdminMnsV1.Services.Implementation
{
    public class DelayService : GenericService<Delay>, IDelayService
    {
        private readonly IDelayRepository _delayRepository;
        private readonly IStatusRepository _statusRepository;
        public DelayService(IDelayRepository repository, IStatusRepository statusRepository) : base(repository)
        {
            _delayRepository = repository;
            _statusRepository = statusRepository;
        }

        public void Add(Delay entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<Delay> entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(Delay entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Delay>> GetAllAbsenceAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Delay>> GetPendingDelaysAsync()
        {
            return await _delayRepository.GetPendingDelaysAsync();
        }


        public void RemoveRange(IEnumerable<Delay> entities)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public void Update(Delay entity)
        {
            throw new NotImplementedException();
        }

        public async Task ValidateDelayAsync(int delayId, string administratorId, int statusValidatedId)
        {
            var delay = await _delayRepository.GetByIdAsync(delayId);
            if (delay == null)
            {
                throw new KeyNotFoundException($"Delay with ID {delayId} not found.");
            }

            var statusValidated = await _statusRepository.GetByIdAsync(statusValidatedId);
            if (statusValidated == null)
            {
                throw new KeyNotFoundException($"Status with ID {statusValidatedId} not found.");
            }
            delay.Status = statusValidated;
            delay.AdministratorId = administratorId;
            delay.DateValidated = DateTime.UtcNow;
            delay.JustificationStatus = "Validated";
            _delayRepository.Update(delay);
            await _delayRepository.SaveChangesAsync();
        }

        public Task RejectDelayAsync(int delayId, string administratorId, int statusRejectedId, string reason)
        {
            var delay = _delayRepository.GetByIdAsync(delayId).Result;
            if (delay == null)
            {
                throw new KeyNotFoundException($"Delay with ID {delayId} not found.");
            }
            var statusRejected = _statusRepository.GetByIdAsync(statusRejectedId).Result;
            if (statusRejected == null)
            {
                throw new KeyNotFoundException($"Status with ID {statusRejectedId} not found.");
            }
            delay.Status = statusRejected;
            delay.AdministratorId = administratorId;
            delay.DateValidated = DateTime.UtcNow;
            delay.JustificationStatus = reason;
            _delayRepository.Update(delay);
            return _delayRepository.SaveChangesAsync();
$        }
    }
}