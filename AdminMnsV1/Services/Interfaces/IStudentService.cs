// Services/Interfaces/IStudentService.cs
using AdminMnsV1.Models.Students;
using AdminMnsV1.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminMnsV1.Services.Interfaces
{
    public interface IStudentService
    {
        Task<StudentListPageViewModel> GetStudentListPageViewModelAsync();
        Task<StudentCreateViewModel> GetStudentCreateViewModelAsync();
        Task<(IdentityResult Result, string? ErrorMessage)> CreateStudentAsync(StudentCreateViewModel model);
        Task<(IdentityResult Result, string? ErrorMessage)> UpdateStudentAsync(StudentEditViewModel model);
        Task<(IdentityResult Result, string? ErrorMessage)> SoftDeleteStudentAsync(string id);
        Task<StudentEditViewModel?> GetStudentEditViewModelAsync(string id); // Pour l'édition
    }
}