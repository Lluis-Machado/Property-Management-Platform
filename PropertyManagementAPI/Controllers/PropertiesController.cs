using Microsoft.AspNetCore.Mvc;

namespace PropertyManagementAPI.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly ILogger<PropertiesController> _logger;
        private readonly IPropertiesRepository _declarantRepo;
        private readonly IValidator<Declarant> _declarantValidator;
        public PropertiesController(IDeclarantRepository declarantRepo, IValidator<Declarant> declarantValidator, ILogger<DeclarantsController> logger)
        {
            _declarantRepo = declarantRepo;
            _declarantValidator = declarantValidator;
            _logger = logger;
        }
    }
}
