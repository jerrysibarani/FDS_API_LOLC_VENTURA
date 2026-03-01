using API.Data;
using API.Helpers;
using API.Model;
using API.IServices;
using API.Models.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PostParameterController(
        IPostsService postsService
    ) : BaseController
    {

        private readonly IPostsService _postsService = postsService;


        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try
            {
                if (currentUser.UserType == ConstantaData.EXTERNAL)
                {
                    return BadRequest("Not Access");
                }
                var result = await _postsService.GetListPost();

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
        
        
        [HttpGet("ByGroup")]
        public async Task<ActionResult> ByGroup(string parameter)
        {
            try
            {
                if (currentUser.UserType == ConstantaData.EXTERNAL)
                {
                    return BadRequest("Not Access");
                }
                var result = await _postsService.GetListPostByGroup(parameter);

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


        [HttpGet("ByHeader")]
        public async Task<ActionResult> ByHeader(string parameter)
        {
            try
            {
                if (currentUser.UserType == ConstantaData.EXTERNAL)
                {
                    return BadRequest("Not Access");
                }
                var result = await _postsService.GetListPostByHeader(parameter);

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


        [HttpGet("ByType")]
        public async Task<ActionResult> ByType(string parameter)
        {
            try
            {
                if (currentUser.UserType == ConstantaData.EXTERNAL)
                {
                    return BadRequest("Not Access");
                }
                var result = await _postsService.GetListPostByType(parameter);

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
