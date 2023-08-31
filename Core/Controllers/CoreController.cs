using CoreAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoreAPI.Controllers
{
    [ApiController]
    [Route("core")]
    public class CoreController : ControllerBase
    {
        private readonly ICoreService _coreService;

        CoreController(ICoreService coreService)
        {
            _coreService = coreService;
        }


        [HttpGet]
        public IActionResult GetContactData()
        {
            var contactData = new { FirstName = "John", LastName = "Doe", Age = 30 };
            return Ok(contactData);
        }
    }
}