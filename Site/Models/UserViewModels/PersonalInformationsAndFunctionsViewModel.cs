using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Models.UserViewModels
{
    public class PersonalInformationsAndFunctionsViewModel : PersonalInformationsViewModal
    {
        [Required(ErrorMessage = "Por favor, marque ao menos uma função!")]
        [MinLength(length: 1, ErrorMessage = "Por favor, marque ao menos uma função!")]
        public IList<string> Funcoes { get; set; }
    }
}
