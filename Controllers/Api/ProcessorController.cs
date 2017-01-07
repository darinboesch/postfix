using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using postfix.Models;
using postfix.ViewModels;

namespace postfix.Controllers.Api
{
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    public class ProcessorController : Controller
    {
        [HttpPost()]
        [Authorize(Policy = "PostfixAdmins")]
        public JsonResult Post([FromBody] ExecStackViewModel vm)
        {
            var stack = Mapper.Map<ExecutionStack>(vm);

            return Json(Mapper.Map<ExecStackViewModel>(stack));
        }
    }
}