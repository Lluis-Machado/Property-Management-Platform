using DocumentsAPI.Services;
using MassTransit;
using MessagingContracts;
using Newtonsoft.Json;

namespace DocumentsAPI.Consumers
{
    public class ArchiveConsumer : IConsumer<MessageContract>
    {
        private readonly IArchivesService _archivesService;
        private readonly ILogger<ArchiveConsumer> _logger;

        public ArchiveConsumer(IArchivesService archiveService, ILogger<ArchiveConsumer> logger)
        {
            _archivesService = archiveService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MessageContract> context)
        {
            var message = context.Message;

            try
            {
                dynamic payload = JsonConvert.DeserializeObject(context.Message.Payload);

                // Now you can access the properties of the payload dynamically
                string name = payload.name;
                Guid id = payload.id;
                string type = payload.type;

                switch (type)
                {
                    case "property":
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
