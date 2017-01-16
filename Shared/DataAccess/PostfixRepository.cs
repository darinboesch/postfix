using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using postfix.Models.User;
using postfix.Shared.Identity;

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

        public Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            var user = GetUserByName(userName);
            var validated = (user != null) &&
                            (_context.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password)
                                != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed);
            if (validated)
            {
                // todo: PostfixUserLevel, "1"
                return Task.FromResult(new ClaimsIdentity(new GenericIdentity(user.UserName, "Token"),
                    user.Claims.Select(x => new Claim(x.Type, x.Value ?? string.Empty)).ToArray()));
            }

    //   if (username is found and password is found, but only guest access)
    //   {
    //     return Task.FromResult(new ClaimsIdentity(new GenericIdentity(user.UserName, "Token"),
    //       new Claim[] { }));
    //   }

            // Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }

        public Task AddUser(PostfixUser user, string password) {
            _context.UserManager.CreateAsync(user);
            _context.UserManager.AddPasswordAsync(user, password);
            return Task.FromResult(0);
        }

        public async Task<IAsyncResult> AddUserClaim(PostfixUser user, IdentityUserClaim userClaim) {
            var claim = new Claim(userClaim.Type, userClaim.Value);
            var claims = await _context.UserManager.GetClaimsAsync(user);
            var existingClaim = claims.FirstOrDefault(c => c.Type == userClaim.Type);

            if (existingClaim != null) {
                await _context.UserManager.ReplaceClaimAsync(user, existingClaim, claim);
            }
            else {
                await _context.UserManager.AddClaimAsync(user, claim);
            }

            return Task.FromResult(0);
        }

        public PostfixUser GetUserByName(string userName) {
            return _context.UserManager.FindByNameAsync(userName).Result;
        }
    }
}