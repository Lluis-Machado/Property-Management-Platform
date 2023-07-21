using InvoiceItemClassifierAPI.DTOs;
using InvoiceItemClassifierAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace InvoiceItemClassifierAPI.Controllers
{
    [Authorize]
    public class InvoiceItemClassifierController : Controller
    {
        private IItemClassifierService _itemClassifierService;

        public InvoiceItemClassifierController(IItemClassifierService itemClassifierService)
        {
            _itemClassifierService = itemClassifierService;
        }


        // POST: Predict Invoice Item Categories
        [HttpPost]
        [Route("Predict")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<InvoiceItemCategoryPredictionDTO>>> PredictInvoiceItemCategories([FromBody] List<InvoiceItemDTO> invoiceItemDTOs)
        {
            return Ok(_itemClassifierService.PredictList(invoiceItemDTOs));
        }
    }
}
