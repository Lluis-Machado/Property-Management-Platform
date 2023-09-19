using AutoMapper;
using InvoiceItemAnalyzerAPI.DTOs;
using System.Collections.Generic;
using static InvoiceItemAnalyzerAPI.InvoiceItemModel;

namespace InvoiceItemAnalyzerAPI.Services
{
    public class ItemAnalyzerService : IItemAnalyzerService
    {
        private readonly IMapper _mapper;

        public ItemAnalyzerService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IEnumerable<ItemCategoryPredictionDTO> PredictList(List<ItemDTO> invoiceItemDTOs)
        {
            List<ItemCategoryPredictionDTO> modelOutputs = new();
            foreach (ItemDTO invoiceItemDTO in invoiceItemDTOs)
            {
                ModelInput modelInput = _mapper.Map<ModelInput>(invoiceItemDTO);
                ModelOutput modelOutput = Predict(modelInput);
                ItemCategoryPredictionDTO invoiceItemCategoryPredictionDTO = _mapper.Map<ItemCategoryPredictionDTO>(modelOutput);
                modelOutputs.Add(invoiceItemCategoryPredictionDTO);
            }
            return modelOutputs;
        }
    }
}
