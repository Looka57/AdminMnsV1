using AdminMnsV1.Models.Abscences;
using AdminMnsV1.Repositories.Implementation;
using AdminMnsV1.Repositories.Interfaces;
using AdminMnsV1.Services.Interfaces;

namespace AdminMnsV1.Services.Implementation
{
    public class AbsenceService : GenericService<Absence>, IAbsenceService
    {
        private readonly IAbsenceRepository _absenceRepository;
        private readonly IAbsenceService _absenceService;
        private readonly IStatusRepository _statusRepository;

        // private readonly IEmailService _emailService; //  envoyez des mails

        public AbsenceService(IAbsenceRepository repository, IAbsenceService absence, IStatusRepository statusRepository
            ) : base(repository)

        {
            _absenceRepository = repository;
            _absenceService = absence;
            _statusRepository = statusRepository;


        }

        public async Task<IEnumerable<Absence>> GetPendingAbsencesAsync()
        {
            return await _absenceRepository.GetPendingAbsencesAsync();
        }

        public async Task ValidateAbsenceAsync(int absenceId, string administratorId, int statusValidatedId)
        {
            var absence = await _absenceRepository.GetByIdAsync(absenceId);
            if (absence == null)
            {
                throw new KeyNotFoundException($"Absence N° {absenceId} pas trouvé.");
            }
            var statusValidated = await _statusRepository.GetByIdAsync(statusValidatedId);
            if (statusValidated == null)
            {
                throw new KeyNotFoundException($"Statut: {statusValidatedId} pas trouvé.");
            }

            absence.StatusId = statusValidatedId;
            absence.AdministratorId = administratorId;
            absence.DateValidated = DateTime.UtcNow;
            absence.JustificationStatus = "Validée";

            _absenceRepository.UpdateAbsenceAsync(absence);
            await _absenceRepository.SaveChangesAsync();
            // await _emailService.SendAbsenceValidationEmailAsync(absence); // Envoyer un email de validation si nécessaire

        }
        public async Task RejectAbsenceAsync(int absenceId, string administratorId, int statusRejectedId, string reason)
        {
            var absence = await _absenceRepository.GetByIdAsync(absenceId);
            if (absence == null)
            {
                throw new KeyNotFoundException($"Absence N° {absenceId} pas trouvé.");
            }

            var statusRejected = await _statusRepository.GetByIdAsync(statusRejectedId);
            if (statusRejected == null)
            {
                throw new KeyNotFoundException($"Statut: {statusRejectedId} pas trouvé.");
            }

            absence.StatusId = statusRejectedId;
            absence.AdministratorId = administratorId;
            absence.DateValidated = DateTime.UtcNow;
            absence.JustificationStatus = reason; // Justification du rejet

            _absenceRepository.UpdateAbsenceAsync(absence);
            await _absenceRepository.SaveChangesAsync();
            // await _emailService.SendAbsenceRejectionEmailAsync(absence); // Envoyer un email de rejet si nécessaire
        }
    }
}