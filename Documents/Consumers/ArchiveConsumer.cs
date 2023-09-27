using AutoMapper;
using DocumentsAPI.DTOs;
using DocumentsAPI.Models;
using DocumentsAPI.Services;
using MassTransit;
using MessagingContracts;
using Newtonsoft.Json;
using System.Dynamic;
using static DocumentsAPI.Models.Archive;

namespace DocumentsAPI.Consumers
{
    public class ArchiveConsumer : IConsumer<MessageContract>
    {
        private readonly IArchivesService _archivesService;
        private readonly ILogger<ArchiveConsumer> _logger;
        private readonly IMapper _mapper;

        public ArchiveConsumer(IArchivesService archiveService, ILogger<ArchiveConsumer> logger, IMapper mapper)
        {
            _archivesService = archiveService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<MessageContract> context)
        {
            var message = context.Message;

            _logger.LogInformation($"Documents Message Consumer - Received message - Source: {context.SourceAddress}\t{context.Message.Payload}");

            if (string.IsNullOrEmpty(context.Message.Destination) || (bool)!context.Message?.Destination?.Contains("documents")) { return; }

            try
            {
                dynamic payload = JsonConvert.DeserializeObject<dynamic>(context.Message.Payload);

                // Now you can access the properties of the payload dynamically
                string name = payload.name;
                Guid id;
                Guid.TryParse(payload.id, out id);
                string type = payload.type;

                switch (type)
                {
                    case "property":

                        var archiveDTO = new CreateArchiveDTO() { Name = name };

                        var archive = _mapper.Map<CreateArchiveDTO, Archive>(archiveDTO);

                        Archive createdArchive = await _archivesService.CreateArchiveAsync(archive, ARCHIVE_TYPE.PROPERTY, id);
                        //return Created($"archives/property/{createdArchive.Name}", createdArchive);

                        break;
                    case "contact":
                        break;
                    case "company":
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming Archive message contract");
            }


        }
    }
}
