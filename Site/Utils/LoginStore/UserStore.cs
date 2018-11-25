using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Site.DAO;
using Site.Exceptions;
using Site.Models;

namespace Site.Utils.LoginStore
{
    public class UserStore : IUserStore<UserModel>, IUserPasswordStore<UserModel>, IUserRoleStore<UserModel>
    {

        private readonly Connection _connection;

        public UserStore(Connection connection)
        {
            _connection = connection;
        }

        public async Task<IdentityResult> CreateAsync(UserModel user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var dao = new UserDAO(_connection))
            {
                dao.Insert(user, null);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(Models.UserModel user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var dao = new UserDAO(_connection))
            {
                dao.Remove(user, null);
            }

            return IdentityResult.Success;
        }

        public async Task<UserModel> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = new UserModel();
            user.GetById(int.Parse(userId), _connection, null);

            return user;
        }

        public Task<UserModel> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = new UserModel();
            user.GetByNormalizedUserName(normalizedUserName, _connection, null);

            return Task.FromResult(user);
        }

        public Task<string> GetNormalizedUserNameAsync(UserModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(UserModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(UserModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(UserModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Cpf);
        }

        public Task<bool> HasPasswordAsync(UserModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetNormalizedUserNameAsync(UserModel user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetPasswordHashAsync(UserModel user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(UserModel user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(UserModel user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var dao = new UserDAO(_connection))
            {
                dao.Update(user, null);
            }

            return IdentityResult.Success;
        }

        public void Dispose()
        {
            // Destruir este objeto
        }

        public async Task AddToRoleAsync(UserModel user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var role = new RoleModel())
            using (var roleUser = new UserRoleModel())
            {
                if (!(new RoleDAO(_connection).GetByNomeNormalized(role, roleName)))
                    throw new SiteException($"Não foi encontrado nenhum nível de acesso com o nome '{roleName}'!");

                roleUser.IdRole = role.Id;
                roleUser.IdUsuario = user.Id;

                roleUser.Salvar(_connection, null);
            }
        }

        public async Task RemoveFromRoleAsync(UserModel user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var role = new RoleModel())
            using (var roleUser = new UserRoleModel())
            {
                if (!(new RoleDAO(_connection).GetByNomeNormalized(role, roleName)))
                    throw new SiteException($"Não foi encontrado nenhum nível de acesso com o nome '{roleName}'!");

                if (!roleUser.GetByIdUsuarioAndIdRole(user.Id, role.Id, _connection, null))
                    throw new SiteException($"Não foi encontrado nenhum nível de acesso com o nome '{roleName}' para o usuário '{user.Nome}'!");

                roleUser.Remover(_connection, null);
            }
        }

        public Task<IList<string>> GetRolesAsync(UserModel user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(UserRoleModel.GetNameRolesByIdUsuario(user.Id, _connection, null));
        }

        public Task<bool> IsInRoleAsync(UserModel user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var role = new RoleModel())
            {
                if (!role.GetByNomeNormalized(roleName.ToUpper(), _connection, null))
                    throw new SiteException($"Não foi encontrado nenhum nível de acesso com o nome '{roleName}'!");

                return Task.FromResult(UserRoleModel.UserInRole(user.Id, role.Id, _connection, null));
            }
        }

        public Task<IList<UserModel>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var role = new RoleModel())
            {
                if (!role.GetByNomeNormalized(roleName.ToUpper(), _connection, null))
                    throw new SiteException($"Não foi encontrado nenhum nível de acesso com o nome '{roleName}'!");

                return Task.FromResult(UserModel.GetUsersInRole(role.Id, _connection, null));
            }
        }
    }
}
