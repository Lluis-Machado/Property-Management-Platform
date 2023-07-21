using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static InvoiceLine;

namespace InvoiceItemClassifierAPI.Controllers
{

    public class InvoiceItemClassifierController : Controller
    {
        // POST: Predict Invoice Item Categories
        [HttpPost]
        [Route("Predict")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<ModelOutput>>> PredictInvoiceItemCategories([FromBody] List<ModelInput> modelInputs)
        {
            return Ok(PredictList(modelInputs));
        }
    }
}
