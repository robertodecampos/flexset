using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Models.UserViewModels
{
    public class FiltroViewModel
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public int Dia { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public DateTime? DataNascimentoInicial { get; set; } = null;
        public DateTime? DataNascimentoFinal { get; set; } = null;
        public List<int> Funcoes { get; set; }
    }
}
