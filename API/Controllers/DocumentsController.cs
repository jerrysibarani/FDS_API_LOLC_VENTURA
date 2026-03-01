using API.Data;
using API.Data.Entities;
using API.Helpers;
using API.Model;
using API.IServices;
using API.Models.Params;
using API.Models.Views;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController(
        IDocumentService documentService,
        IMapper _mapper,
        ILogger<DocumentsController> _logger
     ) : BaseController
    {

        private readonly IMapper _mapper = _mapper;
        private readonly IDocumentService _documentService = documentService;
        private readonly ILogger<DocumentsController> _logger = _logger;



        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, DataModels models)
        {
            if (currentUser.UserType == ConstantaData.EXTERNAL)
            {
                return BadRequest("Not Access");
            }
            try
            {
                var result = await _documentService.PutDocument(currentUser, id, models);
                if (result)
                {
                    return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "Success", 0, string.Empty), Formatting.Indented));
                }else
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
        public async Task<IActionResult> UpdateDocument(DataModels models)
        {
            if (currentUser.UserType == ConstantaData.EXTERNAL)
            {
                return BadRequest("Not Access");
            }
            try
            {
                var result = await _documentService.UpdateDocument(currentUser, models);
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
        public async Task<IActionResult> Delete(int id)
        {
            if (currentUser.UserType == ConstantaData.EXTERNAL)
            {
                return BadRequest("Not Access");
            }
            try
            {
                var result = await _documentService.DeleteDocument(currentUser, id);
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
        public async Task<ActionResult> StatusAhu(ParamStatus param)
        {
            if (currentUser.UserType == ConstantaData.EXTERNAL)
            {
                return BadRequest("Not Access");
            }
            try
            {
                var result = await _documentService.StatusAhuDocument(currentUser, param);
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
        public async Task<ActionResult> UpdateNotaris(ParamNotaris param)
        {
            if (currentUser.UserType == ConstantaData.EXTERNAL)
            {
                return BadRequest("Not Access");
            }
            try
            {
                var result = await _documentService.UpdateNotarisDocument(currentUser, param);
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



        [HttpPost("UpdateCertificate")]
        public async Task<ActionResult> UpdateCertificate(List<CertificateModel> param)
        {
            if (param == null || param.Count < 0)
            {
                return BadRequest("Request data is null.");
            }
            if (currentUser.UserType == ConstantaData.EXTERNAL)
            {
                return BadRequest("Not Access");
            }
            try
            {
                var result = await _documentService.UpdateCertificateDocument(currentUser, param);                
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



        [HttpPost("UpdateCertificateByModels")]
        public async Task<ActionResult> UpdateCertificateByModels(List<CertificateModel> param)
        {
            if (param == null || param.Count < 0)
            {
                return BadRequest("Request data is null.");
            }
            if (currentUser.UserType == ConstantaData.EXTERNAL)
            {
                return BadRequest("Not Access");
            }
            try
            {
                var result = await _documentService.UpdateCertificateDocumentModel(currentUser, param);
                return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));               
            }
            catch (Exception)
            {
                return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "ERROR", param.Count, param), Formatting.Indented));
                //return BadRequest(ex.Message);
            }
        }


        [HttpPost("UpdateVoucherByModels")]
        public async Task<ActionResult> UpdateVoucherByModels(List<CertificateModel> param)
        {
            if (param == null || param.Count < 0)
            {
                return BadRequest("Request data is null.");
            }
            if (currentUser.UserType == ConstantaData.EXTERNAL)
            {
                return BadRequest("Not Access");
            }
            try
            {
                var result = await _documentService.UpdateVoucherDocumentModel(currentUser, param);
                return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
            }
            catch (Exception)
            {
                return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.OK, "ERROR", param.Count, param), Formatting.Indented));
                //return BadRequest(ex.Message);
            }
        }




        [HttpPost("GetByModels")]
        public async Task<ActionResult> GetByModels(List<CertificateModel> param)
        {
            if (param == null || param.Count < 0)
            {
                return BadRequest("Request data is null.");
            }

            if (currentUser.UserType == ConstantaData.EXTERNAL)
            {
                return BadRequest("Not Access");
            }
            try
            {
                var result = await _documentService.GetDocumentModel(currentUser, param);
                return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
            }
            catch (Exception ex)
            {
                return Ok(JsonConvert.SerializeObject(new ResponseModel(ResponseCode.Error, "ERROR :"+ ex.Message.ToString(), param.Count, param), Formatting.Indented));
                //return BadRequest(ex.Message);
            }
        }


    }
}
