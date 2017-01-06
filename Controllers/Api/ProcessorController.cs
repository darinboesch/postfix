using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace postfix.Controllers.Api
{
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    public class ProcessorController : Controller
    {
        [HttpPost()]
        [Authorize(Policy = "PostfixAdmins")]
        public JsonResult Post()
        {
            return Json(new { response = "yada yada" });
        }
    }
}