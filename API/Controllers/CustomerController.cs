using API.Model;
using API.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult> Get(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _customerService.GetCodeCustomer(CurrentUser, cancellationToken);

                if (result == null || !result.Any()) return NotFound("Data not found");

                return Ok(new ResponseModel(
                            ResponseCode.OK,
                            "Success",
                            result.Count(),
                            result.ToList()
                        ));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
