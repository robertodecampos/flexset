using MySql.Data.MySqlClient;
using Site.DAO;
using Site.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Models
{
    public class EnderecoModel : IDisposable
    {
        public int Id { get; set; }
        public string Uf { get; set; }
        public int IdCidade { get; set; }
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }

        public IList<string> IsValid()
        {
            var erros = new List<string>();

            if (Uf.Trim() == "")
                erros.Add("O estado é obrigatório!");
            if (IdCidade == 0)
                erros.Add("A cidade é obrigatória!");
            if (Bairro.Trim() == "")
                erros.Add("O bairro é obrigatório!");
            if (Logradouro.Trim() == "")
                erros.Add("O logradouro é obrigatório!");
            if (Numero.Trim() == "")
                erros.Add("O número é obrigatório!");
            if (Cep.Trim() == "")
                erros.Add("O CEP é obrigatório!");

            return erros;
        }

        public bool GetById(int id, Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new EnderecoDAO(conn))
            {
                return dao.GetById(this, id, transaction);
            }
        }

        public bool SearchByCep(string cep, Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new EnderecoDAO(conn))
            {
                return dao.SearchByCep(this, cep, transaction);
            }
        }

        public void Dispose() { }
    }
}
