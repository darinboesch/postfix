using System.Security.Claims;
using System.Threading.Tasks;
using postfix.Models.User;

namespace postfix.Shared.DataAccess
{
    public interface IPostfixRepository
    {
        Task<ClaimsIdentity> GetClaimsIdentity(PostfixUser user, string password);
        Task AddUser(PostfixUser user, string password);
        PostfixUser GetUserByName(string userName);
    }
}