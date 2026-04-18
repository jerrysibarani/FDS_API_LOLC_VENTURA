using API.IServices;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryCertificateController(
        IHistoryCertificateService historyCertificateService,
        ILogger<DocumentsController> _logger
    ) : BaseController
    {

        private readonly IHistoryCertificateService _historyCertificateService = historyCertificateService;
        private readonly ILogger<DocumentsController> _logger = _logger;

        [HttpGet("ByBatch/{id:int}")]
        public async Task<ActionResult> GetByBatch(int id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _historyCertificateService.GetHistoriesCertificateByBatch(id, CurrentUser, cancellationToken);

                if (result == null) return NotFound("Data not found");

                return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("BatchHistoriesCertificate")]
        public async Task<ActionResult> BatchHistoriesCertificate(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _historyCertificateService.GetBatchHistoriesCertificate(CurrentUser, cancellationToken);

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
