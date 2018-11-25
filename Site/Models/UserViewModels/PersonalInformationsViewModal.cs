using System;
using System.ComponentModel.DataAnnotations;

namespace Site.Models.UserViewModels
{
    public class PersonalInformationsViewModal
    {
        [Required(ErrorMessage = "Por favor, informe o nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Por favor, informe o e-mail")]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Por favor, informe a data de nascimento")]
        public DateTime? DataNascimento { get; set; }
    }
}
