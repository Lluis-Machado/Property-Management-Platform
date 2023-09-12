using DocumentsAPI.Models;
using DocumentsAPI.Services;
using MassTransit;
using MessagingContracts;
using System.Dynamic;

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
                dynamic archive = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(message?.Payload!);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming Archive message contract");
            }


        }
    }
}
