using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TaxManagement.Models;
using TaxManagement.Repositories;
using TaxManagementAPI.DTOs;

namespace TaxManagementAPI.Services
{
    public class DeclarantService : IDeclarantService
    {
        private readonly IDeclarantRepository _declarantRepo;
        private readonly IMapper _mapper;

        public DeclarantService(IDeclarantRepository declarantRepo, IMapper mapper)
        {
            _declarantRepo = declarantRepo;
            _mapper = mapper;
        }

        public async Task<ActionResult<DeclarantDTO>> UpdateDeclarantAsync(UpdateDeclarantDTO updateDeclarantDTO, Guid declarantId, string lastUpdateByUser)
        {
            var declarant = _mapper.Map<UpdateDeclarantDTO, Declarant>(updateDeclarantDTO);

            declarant.Id = declarantId;
            declarant.LastUpdateByUser = lastUpdateByUser;
            declarant.LastUpdateAt = DateTime.Now;

            var result = await _declarantRepo.UpdateDeclarantAsync(declarant);
            if (result == null)
                return new NotFoundObjectResult("Declarant not found");

            var declarantDTO = _mapper.Map<Declarant, DeclarantDTO>(result);

            return new ActionResult<DeclarantDTO>(declarantDTO);
        }

        public async Task<DeclarantDTO> DeleteDeclarantAsync(Guid declarantId, string lastUserName)
        {
            var result = await _declarantRepo.SetDeleteDeclarantAsync(declarantId, true, lastUserName);
            var declarantDTO = _mapper.Map<Declarant, DeclarantDTO>(result);

            return declarantDTO;
        }

        public async Task<DeclarantDTO> UndeleteDeclarantAsync(Guid declarantId, string lastUserName)
        {
            var result = await _declarantRepo.SetDeleteDeclarantAsync(declarantId, false, lastUserName);
            var declarantDTO = _mapper.Map<Declarant, DeclarantDTO>(result);

            return declarantDTO;
        }

        public async Task<IEnumerable<DeclarantDTO>> GetPaginatedDeclarantsAsync(int pageNumber, int pageSize)
        {
            // Calculate the number of items to skip based on the page number and size
            int itemsToSkip = (pageNumber - 1) * pageSize;

            // Retrieve the declarants based on the pagination parameters
            IEnumerable<Declarant> declarants = await _declarantRepo.GetDeclarantsAsync();

            // Apply pagination by skipping the required number of items and taking only the specified page size
            IEnumerable<Declarant> paginatedDeclarants = declarants.Skip(itemsToSkip).Take(pageSize);
            IEnumerable<DeclarantDTO> paginatedDeclarantsDTO = _mapper.Map<IEnumerable<Declarant>, IEnumerable<DeclarantDTO>>(paginatedDeclarants);

            return paginatedDeclarantsDTO;
        }

        public async Task<IEnumerable<DeclarantDTO>> GetDeclarantsAsync(bool includeDeleted = false)
        {
            var result = await _declarantRepo.GetDeclarantsAsync();
            IEnumerable<DeclarantDTO> resultDTO = _mapper.Map<IEnumerable<Declarant>, IEnumerable<DeclarantDTO>>(result);

            return resultDTO;
        }

        public async Task<DeclarantDTO> CreateDeclarantAsync(CreateDeclarantDTO createDeclarantDTO, string userName)
        {
            var declarant = _mapper.Map<CreateDeclarantDTO, Declarant>(createDeclarantDTO);

            declarant.CreatedByUser = userName;
            declarant.LastUpdateByUser = userName;
            declarant.LastUpdateAt = DateTime.UtcNow;

            declarant = await _declarantRepo.InsertDeclarantAsync(declarant);

            var declarantDTO = _mapper.Map<Declarant, DeclarantDTO>(declarant);

            return declarantDTO;
        }

        public async Task<DeclarantDTO?> DeclarantExists(Guid declarantId)
        {
            Declarant? declarant = await _declarantRepo.GetDeclarantByIdAsync(declarantId);
            var declarantDTO = declarant != null ? _mapper.Map<Declarant, DeclarantDTO>(declarant) : null;

            return declarantDTO;
        }

        /**
         * Removed to avoid compilation issues - Izar

        public MessageContract CreateContract(Guid id, string action, object oldObject, object newObject, string userName)
        {
            var auditDTO = new Audit
            {
                Id = new Guid(),
                EntityId = id,
                Action = action,
                OldObject = oldObject,
                NewObject = newObject,
                Timestamp = DateTime.UtcNow,
                ChangedBy = userName
            };

            string json = JsonConvert.SerializeObject(auditDTO);

            var message = new MessageContract { Payload = json };

            return message;
        }

        */
    }
}

