using InvoiceItemAnalyzerAPI.DTOs;
using InvoiceItemAnalyzerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace InvoiceItemAnalyzerAPI.Controllers
{
    [Authorize]
    public class InvoiceItemAnalyzerController : Controller
    {
        private IItemAnalyzerService _itemClassifierService;

        public InvoiceItemAnalyzerController(IItemAnalyzerService itemClassifierService)
        {
            _itemClassifierService = itemClassifierService;
        }

        // POST: Predict Invoice Item Categories
        [HttpPost]
        [Route("Predict")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<ItemCategoryPredictionDTO>>> PredictInvoiceItemCategories([FromBody] List<ItemDTO> invoiceItemDTOs)
        {
            return Ok(_itemClassifierService.PredictList(invoiceItemDTOs));
        }
    }
}
