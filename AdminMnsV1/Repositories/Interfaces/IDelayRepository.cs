namespace AdminMnsV1.Repositories.Interfaces
{
    public interface IDelayRepository: IGenericRepository<Delay>
    {
        Task<IEnumerable<Delay>> GetPendingDelaysAsync(); // Pour les retards à valider
    }
}
