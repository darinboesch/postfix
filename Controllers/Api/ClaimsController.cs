using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using postfix.Shared.Identity;
using postfix.Shared.DataAccess;
using postfix.ViewModels;
using System.Threading.Tasks;

namespace postfix.Controllers.Api
{
    [Route("api/users/{id}/[controller]")]
    [EnableCors("CorsPolicy")]
    public class ClaimsController : Controller
    {
        private IPostfixRepository _repository;
        private ILogger<ProcessorController> _logger;

        public ClaimsController(IPostfixRepository repository, ILogger<ProcessorController> logger) {
            _repository = repository;
            _logger = logger;
        }

        [HttpPost()]
        [Authorize(Policy = "PostfixAdmins")]
        public async Task<IActionResult> Post([FromBody] ClaimViewModel vm, string id)
        {
            var claim = Mapper.Map<IdentityUserClaim>(vm);
            var user = _repository.GetUserByName(id);

            if (user != null) {
                await _repository.AddUserClaim(user, claim);
                return Created($"api/users/{id}/claims/{claim.Type}", Mapper.Map<ClaimViewModel>(claim));
            }

            return BadRequest($"Unable to add claim.");
        }
    }
}