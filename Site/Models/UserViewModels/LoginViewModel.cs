using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Models.UserViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "CPF")]
        [Required(ErrorMessage = "Por favor, informe seu {0}!")]
        public string Cpf { get; set; }

        [Display(Name = "Senha")]
        [Required(ErrorMessage = "Por favor, informe sua {0}!")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        public bool Lembrar { get; set; }
    }
}
