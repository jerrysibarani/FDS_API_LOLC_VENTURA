using API.Helpers;
using API.Model;
using API.IServices;
using API.Models.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController(
        IFileService fileService
    ) : BaseController
    {

        private readonly IFileService _fileService = fileService;

        [HttpPost("Certificate")]
        public async Task<ActionResult> Certificate([FromBody] ParamFile param, CancellationToken cancellationToken)
        {
            try
            {
                if (param == null) return BadRequest("Request data is null.");
                var result = await _fileService.SaveCertificate(CurrentUser, param, cancellationToken);
                if (result)
                {
                    return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", 1, string.Empty), Formatting.Indented));
                }
                else
                {
                    return BadRequest("Failed Upload File!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Minuta")]
        public async Task<ActionResult> Minuta([FromBody] ParamFile param, CancellationToken cancellationToken)
        {
            try
            {
                if (param == null) return BadRequest("Request data is null.");
                var result = await _fileService.SaveMinuta(CurrentUser, param, cancellationToken);
                if (result)
                {
                    return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", 1, string.Empty), Formatting.Indented));
                }
                else
                {
                    return BadRequest("Failed Upload File!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }

    }
}
