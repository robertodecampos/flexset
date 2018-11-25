using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Models.UserViewModels
{
    public class AlterViewModel : PersonalInformationsAndFunctionsViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}
