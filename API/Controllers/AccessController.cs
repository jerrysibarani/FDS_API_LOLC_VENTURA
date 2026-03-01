using API.Data;
using API.Data.Models;
using API.Helpers;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        public async Task<ActionResult> Get()
        {
            try
            {
                if (currentUser.UserType == ConstantaData.EXTERNAL)
                {
                    return BadRequest("Not Access");
                }
                var result = await _userAccessService.GetAllAccessUser();
                if (result == null || result.Count <= 0)
                {
                    return NotFound("Data not found");
                }
                else
                {
                    return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", result.Count(), result.ToList()), Formatting.Indented));
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/<AccessController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(string id)
        {
            try
            {
                if (currentUser.UserType == ConstantaData.EXTERNAL)
                {
                    return BadRequest("Not Access");
                }
                var result = await _userAccessService.GetAccessUserByPage(id);
                
                if (result == null || result.ACCESS_ID <= 0)
                {
                    return NotFound("Data not found");
                }
                else
                {
                    return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", 1, result), Formatting.Indented));
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
