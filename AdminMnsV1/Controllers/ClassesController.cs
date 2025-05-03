using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Data;
using AdminMnsV1.Models;

namespace AdminMnsV1.Controllers
{
    public class ClassesController : Controller
    {
        public IActionResult Class()
        {
            var classCards = new List<CardModel>
        {
            new CardModel { Title = "CDA", Number = "30", IconUrl = "https://img.icons8.com/external-tal-revivo-bold-tal-revivo/48/external-cda-is-a-file-extension-for-a-cd-audio-shortcut-file-format-audio-bold-tal-revivo.png", AltText = "cda icon" },
            new CardModel { Title = "C#", Number = "13", IconUrl = "https://img.icons8.com/ios-filled/50/c-sharp-logo.png" , AltText = "csharp icon" },
            new CardModel { Title = "Java", Number = "17", IconUrl = "https://img.icons8.com/ios-filled/50/java-coffee-cup-logo--v1.png", AltText = "java icon" },
            new CardModel { Title = "DevWeb 1", Number = "15", IconUrl = "https://img.icons8.com/ios-filled/50/web.png", AltText = "web icon" },
            new CardModel { Title = "DevWeb 2", Number = "15", IconUrl = "https://img.icons8.com/ios-filled/50/web.png", AltText = "web icon" },
            new CardModel { Title = "Réseau", Number = "25", IconUrl = "https://img.icons8.com/ios-filled/50/thin-client.png", AltText = "network icon" },
            new CardModel { Title = "RAN 1", Number = "15", IconUrl = "https://img.icons8.com/ios/50/laptop-coding.png" , AltText = "ran icon" },
            new CardModel { Title = "RAN 2", Number = "25", IconUrl ="https://img.icons8.com/ios/50/laptop-coding.png", AltText = "ran icon" },
            new CardModel { Title = "RAN 3", Number = "15", IconUrl ="https://img.icons8.com/ios/50/laptop-coding.png", AltText = "ran icon" },
            new CardModel { Title = "RAN 4", Number = "25", IconUrl ="https://img.icons8.com/ios/50/laptop-coding.png", AltText = "ran icon" }
            // ... ajoutez toutes vos 15 classes ici
        };
            return View(classCards);
        }
    }
}
