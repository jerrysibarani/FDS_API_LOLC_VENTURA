using API.Data;
using API.Helpers;
using API.Model;
using API.Models.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using API.Data.Entities;
using API.Models.Views;
using System.Globalization;
using API.IServices;

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
        public async Task<ActionResult> ForAhu(ParamData param)
        {
            if (currentUser.UserType == ConstantaData.EXTERNAL)
            {
                return BadRequest("Not Access");
            }
            try
            {
                var result = await _dataService.GetForAHU(currentUser, param);
                return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("ForCertificate")]
        public async Task<ActionResult> ForCertificate(ParamData param)
        {
            if (currentUser.UserType == ConstantaData.EXTERNAL)
            {
                return BadRequest("Not Access");
            }
            try
            {
                var result = await _dataService.GetForCertificate(currentUser, param);
                return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("ForMinuta")]
        public async Task<ActionResult> ForMinuta(ParamData param)
        {
            if (currentUser.UserType == ConstantaData.EXTERNAL)
            {
                return BadRequest("Not Access");
            }
            try
            {
                var result = await _dataService.GetForMinuta(currentUser, param);
                return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
