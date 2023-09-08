using CoreAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CoreAPI.Controllers
{
    [ApiController]
    [Route("core")]
    public class CoreController : ControllerBase
    {
        private readonly ICoreService _coreService;

        public CoreController(ICoreService coreService)
        {
            _coreService = coreService;
        }


        [HttpGet]
        [Route("contacts/{Id}")]
        public async Task<ActionResult<string>> GetContact(Guid Id)
        {
            var contact = await _coreService.GetContact(Id);
            return Ok(contact);
        }

        [HttpGet]
        [Route("company/{Id}")]
        public async Task<ActionResult<string>> GetCompany(Guid Id)
        {
            var contact = await _coreService.GetContact(Id);
            return Ok(contact);
        }

        [HttpGet]
        [Route("properties/{Id}")]
        public async Task<ActionResult<string>> GetProperty(Guid Id)
        {
            var property = await _coreService.GetProperty(Id);
            return Ok(property);
        }

        [HttpPost("properties")]
        //[Route]
        public async Task<ActionResult<string>> CreateProperty([FromBody] object requestBody)
        {
            var property = await _coreService.CreateProperty(requestBody.ToString());



            return Ok(property);
        }
    }
}