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
        private readonly IDocumentAnalyzerService _documentAnalyzerService;

        public DocumentAnalyzerController(IDocumentAnalyzerService documentAnalyzerService, ILogger<DocumentAnalyzerController> logger)
        {
            _logger = logger;
            _documentAnalyzerService = documentAnalyzerService;
        }

        // POST: analyze ap invoice
        [HttpPost]
        [Route("DocumentAnalyzer/APInvoice")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]

        public async Task<ActionResult<DocumentAnalysisDTO<APInvoiceDTO>>> AnalyzeAPInvoiceAsync(IFormFile file)
        {
            return Ok(await _documentAnalyzerService.AnalyzeDocumentAsync<APInvoiceDTO>(file.OpenReadStream()));
        }

        // POST: analyze ar invoice
        [HttpPost]
        [Route("DocumentAnalyzer/ARInvoice")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]

        public async Task<ActionResult<DocumentAnalysisDTO<ARInvoiceDTO>>> AnalyzeARInvoiceAsync(IFormFile file)
        {
            return Ok(await _documentAnalyzerService.AnalyzeDocumentAsync<ARInvoiceDTO>(file.OpenReadStream()));
        }
    }
}
