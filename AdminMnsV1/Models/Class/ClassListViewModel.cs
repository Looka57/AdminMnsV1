// Models/ViewModels/ClassListViewModel.cs
using AdminMnsV1.Models; // Pour CardModel
using AdminMnsV1.Models.Students; // Pour Student
using System.Collections.Generic;

namespace AdminMnsV1.Models.ViewModels
{
    // Ce ViewModel global est ce que votre vue "Classes" recevra
    public class ClassListViewModel
    {
        public List<ClassWithStudentsViewModel> ClassesWithStudents { get; set; } = new List<ClassWithStudentsViewModel>();
    }

    // Ce ViewModel représente une seule classe avec ses élèves
    public class ClassWithStudentsViewModel
    {
        public CardModel ClassCard { get; set; } // Les infos de la carte pour cette classe
        public List<Student> StudentsInClass { get; set; } = new List<Student>(); // La liste des élèves de cette classe
        public int ClassId { get; set; } // L'ID de la classe, utile pour le filtrage
    }
}