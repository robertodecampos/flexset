using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Models.UserViewModels
{
    public class CreatePasswordViewModel
    {
        [Required(ErrorMessage = "Por favor, informe sua senha")]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Por favor, confirme sua senha")]
        [Compare("Senha", ErrorMessage = "As senhas não são iguais")]
        [DataType(DataType.Password)]
        public string ConfirmacaoSenha { get; set; }
    }
}
