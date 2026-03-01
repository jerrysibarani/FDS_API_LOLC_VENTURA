using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    public class IndexController : ControllerBase
    {
        [HttpGet("")]
        public ActionResult Index()
        {
            return Ok("FDS API");
        }
    }
}
