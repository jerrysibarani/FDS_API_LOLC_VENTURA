using API.Model;
using API.Models.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using API.Helpers;
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
        public async Task<ActionResult> Get()
        {
            try
            {
                if(currentUser.UserType == ConstantaData.EXTERNAL)
                {
                    return BadRequest("Not Access");
                }
                var result = await _ahuService.GetAhuAccounts(currentUser);
                if (result == null)
                {
                    return NotFound("Data not found");
                } else
                {
                    return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", result.Count, result), Formatting.Indented));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("password")]
        public async Task<ActionResult> Password(ParamPasswordAhu param)
        {
            try
            {
                if (currentUser.UserType == ConstantaData.EXTERNAL)
                {
                    return BadRequest("Not Access");
                }
                bool result = await _ahuService.ChangePasswordAHU(currentUser, param);
                if (result) { 
                        return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", 0, string.Empty), Formatting.Indented));
                } else
                {
                    return NotFound("Data not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}
