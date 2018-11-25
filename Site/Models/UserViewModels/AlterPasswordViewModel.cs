using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Models.UserViewModels
{
    public class AlterPasswordViewModel : CreatePasswordViewModel
    {
        [Display(Name = "Senha Atual")]
        [Required(ErrorMessage = "Por favor, informe sua {0}!")]
        [DataType(DataType.Password)]
        public string SenhaAtual { get; set; }
    }
}
