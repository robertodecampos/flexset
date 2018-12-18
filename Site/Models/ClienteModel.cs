using MySql.Data.MySqlClient;
using Site.DAO;
using Site.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Models
{
    public enum TipoPessoa { tpFisica, tpJuridica };

    public class ClienteModel
    {
        public int Id { get; set; }
        public TipoPessoa TipoPessoa { get; set; }
        public string Cnpj { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public string NomeFantasia { get; set; }
        public string Telefone { get; set; }
        public string TelefoneAlternativo { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
        public string InscricaoEstadual { get; set; }
        public string InscricaoMunicial { get; set; }
        public string Ramo { get; set; }
        public string Observacao { get; set; }
        public int IdEndereco { get; set; }

        public IList<string> IsValid()
        {
            var erros = new List<string>();

            if (Nome.Trim() == string.Empty)
                erros.Add("O campo nome é obrigatório!");
            if (Telefone.Trim() == string.Empty)
                erros.Add("O campo telefone é obrigatório!");
            if (Email.Trim() == string.Empty)
                erros.Add("O campo e-mail é obrigatório!");

            return erros;
        }

        public void Salvar(Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new ClienteDAO(conn))
            {
                if (Id == 0)
                    dao.Insert(this, transaction);
                else
                    dao.Update(this, transaction);
            }
        }

        public void Remove(Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new ClienteDAO(conn))
            {
                dao.Remove(this, transaction);
            }
        }
    }
}
