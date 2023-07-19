using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;
using DocumentAnalyzerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static DocumentAnalyzerAPI.Utilities.Document;

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

        // POST: analyze document
        [HttpPost]
        [Route("DocumentAnalyzer/APInvoice")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.MultiStatus)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]

        public async Task<ActionResult<DocumentAnalysisDTO<APInvoiceDTO>>> AnalyzeDocumentAsync(IFormFile file, string modelId)
        {
            return Ok(await _documentAnalyzerService.AnalyzeDocumentAsync<APInvoiceDTO>(file.OpenReadStream()));
        }
    }
}
