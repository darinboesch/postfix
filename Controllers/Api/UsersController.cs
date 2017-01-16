using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using postfix.Shared.DataAccess;
using postfix.Models.User;
using postfix.ViewModels;

namespace postfix.Controllers.Api
{
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    public class UsersController : Controller
    {
        private IPostfixRepository _repository;
        private ILogger<UsersController> _logger;

        public UsersController(IPostfixRepository repository, ILogger<UsersController> logger) {
            _repository = repository;
            _logger = logger;
        }
        
        [HttpPost()]
        [Authorize(Policy = "PostfixAdmins")]
        public async Task<IActionResult> Post([FromBody] UserViewModel vm)
        {
            var user = Mapper.Map<PostfixUser>(vm);
            if (_repository.GetUserByName(user.UserName) == null) {
                await _repository.AddUser(user, vm.Password);
                return Created($"api/users/{user.UserName}", Mapper.Map<UserViewModel>(user));
            }

            return BadRequest($"Unable to add user. User '{user.UserName}' already exists.");
        }
    }
}