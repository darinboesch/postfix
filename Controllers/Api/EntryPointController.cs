using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace postfix.Controllers.Api
{
    [EnableCors("CorsPolicy")]
    public class EntryPointController : Controller
    {
        [HttpGet()]
        public IActionResult Get()
        {
            return Ok("yada yada");
        }
    }
}