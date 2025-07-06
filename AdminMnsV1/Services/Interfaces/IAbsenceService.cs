using AdminMnsV1.Models.Abscences;

namespace AdminMnsV1.Services.Interfaces
{
    public interface IAbsenceService : IGenericService<Absence>
    {
        Task<IEnumerable<Absence>> GetPendingAbsencesAsync(); // Pour les absences à valider
        Task ValidateAbsenceAsync(int absenceId, string administratorId, int statusValidatedId ); // Pour valider une absence
        Task RejectAbsenceAsync(int absenceId, string administratorId, int statusRejectedId, string reason); // Pour rejeter une absence
    }
    
}