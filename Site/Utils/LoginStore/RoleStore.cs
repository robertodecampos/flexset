using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Site.DAO;
using Site.Models;

namespace Site.Utils.LoginStore
{
    public class RoleStore : IRoleStore<RoleModel>
    {
        private readonly Connection _connection;

        public RoleStore(Connection connection)
        {
            _connection = connection;
        }

        public async Task<IdentityResult> CreateAsync(RoleModel role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var dao = new RoleDAO(_connection))
            {
                dao.Insert(role, null);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(RoleModel role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var dao = new RoleDAO(_connection))
            {
                dao.Remove(role, null);
            }

            return IdentityResult.Success;
        }

        public async Task<RoleModel> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var role = new RoleModel();
            role.GetById(int.Parse(roleId), _connection, null);

            return role;
        }

        public async Task<RoleModel> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = new RoleModel();
            user.GetByNomeNormalized(normalizedRoleName, _connection, null);

            return user;
        }

        public async Task<string> GetNormalizedRoleNameAsync(RoleModel role, CancellationToken cancellationToken)
        {
            return role.NormalizedNome;
        }

        public async Task<string> GetRoleIdAsync(RoleModel role, CancellationToken cancellationToken)
        {
            return role.Id.ToString();
        }

        public async Task<string> GetRoleNameAsync(RoleModel role, CancellationToken cancellationToken)
        {
            return role.Nome;
        }

        public async Task SetNormalizedRoleNameAsync(RoleModel role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedNome = normalizedName;
        }

        public async Task SetRoleNameAsync(RoleModel role, string roleName, CancellationToken cancellationToken)
        {
            role.Nome = roleName;
        }

        public async Task<IdentityResult> UpdateAsync(RoleModel role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var dao = new RoleDAO(_connection))
            {
                dao.Insert(role, null);
            }

            return IdentityResult.Success;
        }

        public void Dispose()
        {
            // Destruir componentes internos
        }
    }
}
