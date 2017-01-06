using Microsoft.AspNetCore.Mvc;

namespace postfix.Controllers.Api
{
    public class EntryPointController : Controller
    {
        [HttpGet()]
        public IActionResult Get()
        {
            return Ok("yada yada");
        }
    }
}