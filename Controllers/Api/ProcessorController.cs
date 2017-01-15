using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using postfix.Models;
using postfix.Shared.DataAccess;
using postfix.Models.Processor;
using postfix.ViewModels;

namespace postfix.Controllers.Api
{
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    public class ProcessorController : Controller
    {
        private IPostfixRepository _repository;
        private ILogger<ProcessorController> _logger;

        public ProcessorController(IPostfixRepository repository, ILogger<ProcessorController> logger) {
            _repository = repository;
            _logger = logger;
        }

        [HttpPost()]
        [Authorize(Policy = "PostfixAdmins")]
        public JsonResult Post([FromBody] ExecStackViewModel vm)
        {
            var stack = Mapper.Map<ExecutionStack>(vm);

            return Json(Mapper.Map<ExecStackViewModel>(stack));
        }
    }
}