using MySql.Data.MySqlClient;
using Site.Exceptions;
using Site.Models;
using Site.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site.DAO
{
    public class ClienteDAO : IDao<ClienteModel, MySqlTransaction>
    {
        private Connection _connection { get; set; }

        public ClienteDAO(Connection conn) => _connection = conn;

        public int Insert(ClienteModel model, MySqlTransaction transaction)
        {
            if (model.Id != 0)
                throw new SiteException("Não é possível inserir um registro que já possui um identificador!");

            if (model.IsValid().Count > 0)
                throw new SiteValidateException("O cliente contém algumas inconsistências!");

            string sql = "INSERT INTO `Cliente` (" +
                         "`tipoPessoa`, `nome`, `telefone`, `telefoneAlternativo`, `celular`, `email`, `obs`, `ramo`," +
                         " `idEndereco`, `cpf`, `cnpj`, `nomeFantasia`, `inscricaoEstadual`, `inscricaoMunicipal`" +
                         ") VALUES(" +
                         "@tipoPessoa, @nome, @telefone, @telefoneAlternativo, @celular, @email, @obs, @ramo," +
                         " @idEndereco, @cpf, @cnpj, @nomeFantasia, @inscricaoEstadual, @inscricaoMunicipal" +
                         ")";

            var parameters = GetParametersByModel(model);

            int linhasAfetadas = _connection.Execute(sql, parameters, transaction);

            if (linhasAfetadas != 1)
                return linhasAfetadas;

            model.Id = _connection.UltimoIdInserido();

            return linhasAfetadas;
        }

        public int Remove(ClienteModel model, MySqlTransaction transaction)
        {
            if (model.Id == 0)
                throw new SiteException("Não é possível remover um registro que não possui um identificador!");

            string sql = "UPDATE `Endereco` SET";
            sql += " removido = 1";
            sql += " WHERE id = @id";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32) { Value = model.Id });

            return _connection.Execute(sql, parameters, transaction);
        }

        public int Update(ClienteModel model, MySqlTransaction transaction)
        {
            if (model.Id == 0)
                throw new SiteException("Não é possível alterar um registro que não possui um identificador!");

            if (model.IsValid().Count > 0)
                throw new SiteValidateException("O cliente contém algumas inconsistências!");

            string sql = "UPDATE `Cliente` SET" +
                         " `tipoPessoa` = @tipoPessoa, `nome` = @nome, `telefone` = @telefone, `telefoneAlternativo` = @telefoneAlternativo," +
                         " `celular` = @celular, `email` = @email, `obs` = @obs, `ramo` = @ramo, `idEndereco` = @idEndereco, `cpf` = @cpf," +
                         " `cnpj` = @cnpj, `nomeFantasia` = @nomeFantasia, `inscricaoEstadual` = @inscricaoEstadual, `inscricaoMunicipal` = @inscricaoMunicipal" +
                         " WHERE id = @id";

            var parameters = GetParametersByModel(model);
            parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32) { Value = model.Id });

            return _connection.Execute(sql, parameters, transaction);
        }

        private IList<MySqlParameter> GetParametersByModel(ClienteModel model)
        {
            var parameters = new List<MySqlParameter>();

            string cnpj = null, cpf = null, tipoPessoa = null, nomeFantasia = null, inscricaoEstadual = null, inscricaoMunicipal = null;

            if (model.TipoPessoa == TipoPessoa.tpFisica)
            {
                tipoPessoa = "F";
                cpf = model.Cpf;
            } else if (model.TipoPessoa == TipoPessoa.tpJuridica)
            {
                tipoPessoa = "J";
                cnpj = model.Cnpj;
                nomeFantasia = model.NomeFantasia;
                inscricaoEstadual = model.InscricaoEstadual;
                inscricaoMunicipal = model.InscricaoMunicial;
            }

            parameters.Add(new MySqlParameter("@tipoPessoa", MySqlDbType.String) { Value = tipoPessoa });
            parameters.Add(new MySqlParameter("@nome", MySqlDbType.String) { Value = model.Nome });
            parameters.Add(new MySqlParameter("@telefone", MySqlDbType.String) { Value = model.Telefone });
            parameters.Add(new MySqlParameter("@telefoneAlternativo", MySqlDbType.String) { Value = model.TelefoneAlternativo });
            parameters.Add(new MySqlParameter("@celular", MySqlDbType.String) { Value = model.Celular });
            parameters.Add(new MySqlParameter("@email", MySqlDbType.String) { Value = model.Email });
            parameters.Add(new MySqlParameter("@obs", MySqlDbType.String) { Value = model.Observacao });
            parameters.Add(new MySqlParameter("@ramo", MySqlDbType.String) { Value = model.Ramo });
            parameters.Add(new MySqlParameter("@idEndereco", MySqlDbType.Int32) { Value = model.IdEndereco });
            parameters.Add(new MySqlParameter("@cpf", MySqlDbType.String) { Value = cpf });
            parameters.Add(new MySqlParameter("@cnpj", MySqlDbType.String) { Value = "F" });
            parameters.Add(new MySqlParameter("@nomeFantasia", MySqlDbType.String) { Value = model.NomeFantasia });
            parameters.Add(new MySqlParameter("@inscricaoEstadual", MySqlDbType.String) { Value = model.InscricaoEstadual });
            parameters.Add(new MySqlParameter("@inscricaoMunicipal", MySqlDbType.String) { Value = model.InscricaoMunicial });

            return parameters;
        }

        public void Dispose() { }
    }
}
