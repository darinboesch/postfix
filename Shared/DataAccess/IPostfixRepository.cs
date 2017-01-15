using System.Security.Claims;
using System.Threading.Tasks;
using postfix.Models;
using postfix.Models.User;

namespace postfix.Shared.DataAccess
{
    public interface IPostfixRepository
    {
        Task<ClaimsIdentity> GetClaimsIdentity(ApplicationUser user);
        Task AddUser(PostfixUser user, string password);
        PostfixUser GetUserByName(string userName);
    }
}