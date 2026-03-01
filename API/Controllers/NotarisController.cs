using API.Data.Models;
using API.Data;
using API.Helpers;
using API.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using API.IServices;
using API.Services;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotarisController(
        INotarisService notarisService
    ) : BaseController
    {

        private readonly INotarisService _notarisService = notarisService;


        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try
            {
                if (currentUser.UserType == ConstantaData.EXTERNAL)
                {
                    return BadRequest("Not Access");
                }
                var result = await _notarisService.GetCodeNotaris(currentUser);
                if (result == null)
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id)
        {
            try
            {
                if (currentUser.UserType == ConstantaData.EXTERNAL)
                {
                    return BadRequest("Not Access");
                }
                bool result = await _notarisService.ChangeStatusNotaris(currentUser, id);
                if (result)
                {
                    return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", 0, string.Empty), Formatting.Indented));
                }
                else
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
