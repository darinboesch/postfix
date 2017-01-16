using System;
using System.Security.Claims;
using System.Threading.Tasks;
using postfix.Models.User;
using postfix.Shared.Identity;

namespace postfix.Shared.DataAccess
{
    public interface IPostfixRepository
    {
        Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password);
        Task AddUser(PostfixUser user, string password);
        PostfixUser GetUserByName(string userName);
        Task<IAsyncResult> AddUserClaim(PostfixUser user, IdentityUserClaim userClaim);
    }
}