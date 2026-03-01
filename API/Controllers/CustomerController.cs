using API.Helpers;
using API.Model;
using API.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController(
        ICustomerService customerService
    ) : BaseController
    {

        private readonly ICustomerService _customerService = customerService;

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try
            {
                if (currentUser.UserType == ConstantaData.EXTERNAL)
                {
                    return BadRequest("Not Access");
                }
                var result = await _customerService.GetCodeCustomer(currentUser);
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

    }
}
