using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace postfix.Shared.Identity
{
    public class RoleStore<T> : IRoleStore<T>, IQueryableRoleStore<T>
                    where T : IdentityRole
    {
        private readonly IMongoCollection<T> _roles;
        public virtual IQueryable<T> Roles => _roles.AsQueryable();

        public RoleStore(IMongoCollection<T> roles) {
            _roles = roles;
        }

        public virtual void Dispose() { }

        public Task<IdentityResult> CreateAsync(T role, CancellationToken cancellationToken)
        {
            _roles.InsertOneAsync(role);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(T role, CancellationToken cancellationToken)
        {
            _roles.ReplaceOneAsync(r => r.Id == role.Id, role);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(T role, CancellationToken cancellationToken)
        {
            _roles.DeleteOneAsync(r => r.Id == role.Id);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<T> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return _roles.Find(r => r.Id == roleId).FirstOrDefaultAsync();
        }

        public Task<T> FindByNameAsync(string roleName, CancellationToken cancellationToken)
        {
            return _roles.Find(r => r.Name == roleName).FirstOrDefaultAsync();
        }

        public Task<string> GetNormalizedRoleNameAsync(T role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(T role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleNameAsync(T role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(T role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(T role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
}