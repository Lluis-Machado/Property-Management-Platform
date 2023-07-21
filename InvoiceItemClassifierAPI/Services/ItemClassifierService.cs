using AutoMapper;
using InvoiceItemClassifierAPI.DTOs;
using System.Collections.Generic;
using static InvoiceItemClassifierAPI.InvoiceLine;

namespace InvoiceItemClassifierAPI.Services
{
    public class ItemClassifierService :IItemClassifierService
    {
        private readonly IMapper _mapper;

        public ItemClassifierService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IEnumerable<InvoiceItemCategoryPredictionDTO> PredictList(List<InvoiceItemDTO> invoiceItemDTOs)
        {
            List<InvoiceItemCategoryPredictionDTO> modelOutputs = new();
            foreach(InvoiceItemDTO invoiceItemDTO in invoiceItemDTOs)
            {
                ModelInput modelInput = _mapper.Map<ModelInput>(invoiceItemDTO);
                ModelOutput modelOutput = Predict(modelInput);
                InvoiceItemCategoryPredictionDTO invoiceItemCategoryPredictionDTO = _mapper.Map<InvoiceItemCategoryPredictionDTO>(modelOutput);
                modelOutputs.Add(invoiceItemCategoryPredictionDTO);
            }
            return modelOutputs;
        }
    }
}
