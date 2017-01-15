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

            await _repository.AddUser(user, vm.Password);
            return Created($"api/users/{vm.UserName}", Mapper.Map<UserViewModel>(user));
        }
/*
        [HttpPost()]
        [Authorize(Policy = "PostfixAdmins")]
        [Route("adduserclaim")]
        public IActionResult AddUserClaim([FromBody] UserClaimViewModel vm)
        {
            var user = _repository.GetUserByName(vm.UserName);
            if (user != null) {
                return Json(user);
            }

            return Json.NotFound();
        }
*/
    }
}