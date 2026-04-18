using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using API.IServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController(
        IUserAccessService userAccessService
    ) : BaseController
    {

        private readonly IUserAccessService _userAccessService = userAccessService;

        [HttpGet]
        public async Task<ActionResult> Get(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userAccessService.GetAllAccessUser(CurrentUser, cancellationToken);

                if (result == null || !result.Any()) return NotFound("Data not found");
                
                return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", result.Count(), result.ToList()), Formatting.Indented));
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/<AccessController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(string id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userAccessService.GetAccessUserByPage(id, CurrentUser, cancellationToken);     
                
                if (result == null) return NotFound("Data not found");

                return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", 1, result), Formatting.Indented));
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
