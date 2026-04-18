using API.Model;
using API.Models.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using API.IServices;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AhuController (
        IAHUService ahuService
     ) : BaseController
    {

        private readonly IAHUService _ahuService = ahuService;

        [HttpGet]
        public async Task<ActionResult> Get(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _ahuService.GetAhuAccounts(CurrentUser, cancellationToken);
                
                if (result == null || !result.Any()) return NotFound("Data not found");
                
                return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", result.Count, result), Formatting.Indented));
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("password")]
        public async Task<ActionResult> Password(ParamPasswordAhu param, CancellationToken cancellationToken)
        {
            try
            {
                bool result = await _ahuService.ChangePasswordAHU(CurrentUser, param, cancellationToken);
                
                if(!result) return NotFound("Data not found");
                 
                return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", 0, string.Empty), Formatting.Indented));
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}
