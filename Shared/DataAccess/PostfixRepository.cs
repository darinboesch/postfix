using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using postfix.Models.User;

namespace postfix.Shared.DataAccess
{
    public class PostfixRepository : IPostfixRepository
    {
        private PostfixContext _context;
        private ILogger<PostfixRepository> _logger;

        public PostfixRepository(PostfixContext context, ILogger<PostfixRepository> logger) {
            _context = context;
            _logger = logger;
        }

        public Task<ClaimsIdentity> GetClaimsIdentity(PostfixUser user, string password)
        {
            // todo
            throw new NotImplementedException();
        }

        public Task AddUser(PostfixUser user, string password) {
            _context.UserManager.CreateAsync(user);
            _context.UserManager.AddPasswordAsync(user, password);
            return Task.FromResult(0);
        }

        public PostfixUser GetUserByName(string userName) {
            return _context.UserManager.FindByNameAsync(userName).Result;
        }
    }
}