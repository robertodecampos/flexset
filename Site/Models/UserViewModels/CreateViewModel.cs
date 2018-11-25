using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Site.Models.UserViewModels
{
    public class CreateViewModel : PersonalInformationsAndFunctionsViewModel
    {
        [Required(ErrorMessage = "Por favor, informe o CPF")]
        public string Cpf { get; set; }
    }
}
