using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;
using DocumentAnalyzerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DocumentAnalyzerAPI.Controllers
{
    public class DocumentAnalyzerController : Controller
    {
        private readonly ILogger<DocumentAnalyzerController> _logger;
        private readonly IAPInvoiceAnalyzerService _documentAnalyzerService;

        public DocumentAnalyzerController(IAPInvoiceAnalyzerService documentAnalyzerService, ILogger<DocumentAnalyzerController> logger)
        {
            _logger = logger;
            _documentAnalyzerService = documentAnalyzerService;
        }

        // POST: analyze document
        [HttpPost]
        [Route("{modelId}/analyze")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.MultiStatus)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]

        public async Task<ActionResult<DocumentAnalysisDTO>> AnalyzeDocumentAsync(IFormFile file, string modelId)
        {

            return Ok(await _documentAnalyzerService.AnalyzeDocumentAsync(file.OpenReadStream(), modelId));
        }
    }
}
