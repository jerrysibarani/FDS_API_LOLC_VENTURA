using API.Model;
using API.IServices;
using API.Models.Params;
using API.Models.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController(
        IDocumentService documentService,
        ILogger<DocumentsController> _logger
     ) : BaseController
    {

        private readonly IDocumentService _documentService = documentService;
        private readonly ILogger<DocumentsController> _logger = _logger;



        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, DataModels models, CancellationToken cancellationToken)
        {
            try
            {
                if (models == null || id<= 0)
                {
                    return BadRequest("Parameters data is null.");
                }

                var result = await _documentService.PutDocument(CurrentUser, id, models, cancellationToken);

                if (result)
                {
                    return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", 0, string.Empty), Formatting.Indented));
                }
                else
                {
                    return BadRequest("Failed Update Documents");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document with ID {DocumentId}", id);
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("UpdateDocument")]
        public async Task<IActionResult> UpdateDocument(DataModels models, CancellationToken cancellationToken)
        {
            try
            {
                if (models == null)
                {
                    return BadRequest("Parameters data is null.");
                }

                var result = await _documentService.UpdateDocument(CurrentUser, models, cancellationToken);

                if (result)
                {
                    return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", 0, string.Empty), Formatting.Indented));
                }
                else
                {
                    return BadRequest("Failed Update Documents");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Parameters data is null.");
                }

                var result = await _documentService.DeleteDocument(CurrentUser, id, cancellationToken);

                if (result)
                {
                    return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", 0, string.Empty), Formatting.Indented));
                }
                else
                {
                    return BadRequest("Failed Update Documents");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("StatusAhu")]
        public async Task<ActionResult> StatusAhu(ParamStatus param, CancellationToken cancellationToken)
        {
            try
            {
                if (param == null)
                {
                    return BadRequest("Parameters data is null.");
                }

                var result = await _documentService.StatusAhuDocument(CurrentUser, param, cancellationToken);

                if (result)
                {
                    return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", 0, string.Empty), Formatting.Indented));
                }
                else
                {
                    return BadRequest("Failed Update Documents");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        
        [HttpPost("UpdateNotaris")]
        public async Task<ActionResult> UpdateNotaris(ParamNotaris param, CancellationToken cancellationToken)
        {
            try
            {
                if (param == null)
                {
                    return BadRequest("Parameters data is null.");
                }

                var result = await _documentService.UpdateNotarisDocument(CurrentUser, param, cancellationToken);

                if (result)
                {
                    return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", 0, string.Empty), Formatting.Indented));
                }
                else
                {
                    return BadRequest("Failed Update Documents");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //NEW

        [HttpPost("GenerateCertificates")]
        public async Task<ActionResult> GenerateCertificates(List<CertificateModel> param, CancellationToken cancellationToken)
        {
            try
            {
                if (param == null || param.Count <= 0)
                {
                    return BadRequest("Parameters data is null.");
                }

                var result = await _documentService.GenerateCertificates(CurrentUser, param, cancellationToken);

                if (result == null)
                {
                    return BadRequest("Failed Update Certificate");
                }
                else
                {
                    return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("GenerateVouchers")]
        public async Task<ActionResult> GenerateVouchers(List<CertificateModel> param, CancellationToken cancellationToken)
        {
            try
            {
                if (param == null || param.Count <= 0)
                {
                    return BadRequest("Parameters data is null.");
                }

                var result = await _documentService.UpdateCertificates(CurrentUser, param, cancellationToken);

                if (result == null)
                {
                    return BadRequest("Failed Update Certificate");
                }
                else
                {
                    return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        [HttpPost("UpdateCertificates")]
        public async Task<ActionResult> UpdateCertificates(List<CertificateModel> param, CancellationToken cancellationToken)
        {
            try
            {
                if (param == null || param.Count <= 0)
                {
                    return BadRequest("Parameters data is null.");
                }

                var result = await _documentService.UpdateCertificates(CurrentUser, param, cancellationToken);

                if (result == null)
                {
                    return BadRequest("Failed Update Certificate");
                }
                else
                {
                    return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
