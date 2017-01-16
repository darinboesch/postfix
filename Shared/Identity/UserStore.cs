using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace postfix.Shared.Identity
{
    public class UserStore<T> : IUserStore<T>,
                                IUserPasswordStore<T>,
                                IUserRoleStore<T>,
                                IUserLoginStore<T>,
                                IUserSecurityStampStore<T>,
                                IUserEmailStore<T>,
                                IUserClaimStore<T>,
                                IUserPhoneNumberStore<T>,
                                IUserTwoFactorStore<T>,
                                IUserLockoutStore<T>,
                                IQueryableUserStore<T>
                    where T : IdentityUser
    {
        private readonly IMongoCollection<T> _users;
        public virtual IQueryable<T> Users => _users.AsQueryable();

        public void Dispose() { }

        public UserStore(IMongoCollection<T> users)
		{
			_users = users;
		}
        
        public Task<IdentityResult> CreateAsync(T user, CancellationToken cancellationToken)
        {
            _users.InsertOneAsync(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(T user, CancellationToken cancellationToken)
        {
            _users.ReplaceOneAsync(u => u.Id == user.Id, user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(T user, CancellationToken cancellationToken)
        {
            _users.DeleteOneAsync(u => u.Id == user.Id);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<T> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        }

        public Task<T> FindByNameAsync(string userName, CancellationToken cancellationToken)
        {
            return _users.Find(u => u.UserName == userName).FirstOrDefaultAsync();
        }

        public Task<string> GetNormalizedUserNameAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(T user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(T user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(T user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(T user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(T user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(T user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(T user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.HasPassword());
        }

        public Task AddToRoleAsync(T user, string roleName, CancellationToken cancellationToken)
        {
            user.AddRole(roleName);
            return Task.FromResult(0);
        }

        public Task RemoveFromRoleAsync(T user, string roleName, CancellationToken cancellationToken)
        {
            user.RemoveRole(roleName);
            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(T user, CancellationToken cancellationToken)
        {
            return Task.FromResult((IList<string>) user.Roles);
        }

        public Task<bool> IsInRoleAsync(T user, string roleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Roles.Contains(roleName));
        }

        public Task<IList<T>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AddLoginAsync(T user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            user.AddLogin(login);
            return Task.FromResult(0);
        }

        public Task RemoveLoginAsync(T user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            // not entirely sure about this one
            var login = user.Logins.Find(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey);
            user.RemoveLogin(login);
            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
			return _users
				.Find(u => u.Logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey))
				.FirstOrDefaultAsync();
        }

        public Task SetSecurityStampAsync(T user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(T user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public Task<bool> GetEmailConfirmedAsync(T user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(T user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }
        
        public Task SetEmailAsync(T user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.FromResult(0);
        }
        
        public Task<string> GetEmailAsync(T user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<T> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return Task.FromResult(_users.Find(u => u.Email == email).FirstOrDefault());
        }

        public Task<string> GetNormalizedEmailAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(T user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.FromResult(0);
        }

        public Task<IList<Claim>> GetClaimsAsync(T user, CancellationToken cancellationToken)
        {
            return Task.FromResult((IList<Claim>) user.Claims.Select(c => c.ToSecurityClaim()).ToList());
        }

        public Task AddClaimsAsync(T user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            user.AddClaims(claims);
            return Task.FromResult(0);
        }

        public Task ReplaceClaimAsync(T user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            user.RemoveClaim(claim);
            user.AddClaim(newClaim);
            return Task.FromResult(0);
        }

        public Task RemoveClaimsAsync(T user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            user.RemoveClaims(claims);
            return Task.FromResult(0);
        }

        public Task<IList<T>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberAsync(T user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(T user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(T user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(T user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task SetTwoFactorEnabledAsync(T user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(T user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(T user, CancellationToken cancellationToken)
        {
            // not sure about this one
            return Task.FromResult(user.LockoutEndDateUtc == null ? (DateTimeOffset?) null : new DateTimeOffset());
        }

        public Task SetLockoutEndDateAsync(T user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.LockoutEndDateUtc = new DateTime(lockoutEnd.Value.Ticks, DateTimeKind.Utc);
            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(T user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount ++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(T user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(T user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(T user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(T user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }
    }
}