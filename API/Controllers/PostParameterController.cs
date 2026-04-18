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
    public class PostParameterController(
        IPostsService postsService
    ) : BaseController
    {

        private readonly IPostsService _postsService = postsService;

        [HttpGet]
        public async Task<ActionResult> Get(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _postsService.GetListPost(cancellationToken);

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
        public async Task<ActionResult> ByGroup(string parameter, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _postsService.GetListPostByGroup(parameter, cancellationToken);

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
        public async Task<ActionResult> ByHeader(string parameter, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _postsService.GetListPostByHeader(parameter, cancellationToken);

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
        public async Task<ActionResult> ByType(string parameter, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _postsService.GetListPostByType(parameter, cancellationToken);

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
