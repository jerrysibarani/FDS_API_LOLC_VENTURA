using API.IServices;
using API.Model;
using API.Models.Params;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DataController(
        IDataService dataService
    ) : BaseController
    {

        private readonly IDataService _dataService = dataService;


        [HttpPost("ForAhu")]
        public async Task<ActionResult> ForAhu(ParamData param, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _dataService.GetForAHU_Keyset(CurrentUser, param, cancellationToken);
                if (result == null) return NotFound("Data not found");
                return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("ForCertificate")]
        public async Task<ActionResult> ForCertificate(ParamData param, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _dataService.GetForCertificate_Keyset(CurrentUser, param, cancellationToken);
                if (result == null) return NotFound("Data not found");
                return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("ForMinuta")]
        public async Task<ActionResult> ForMinuta(ParamData param, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _dataService.GetForMinuta_Keyset(CurrentUser, param, cancellationToken);
                if (result == null) return NotFound("Data not found");
                return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("ForHistoriesCertificate")]
        public async Task<ActionResult> ForHistoriesCertificate(ParamData param, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _dataService.GetForHistoryCertificates_Keyset(CurrentUser, param, cancellationToken);
                if (result == null) return NotFound("Data not found");
                return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
