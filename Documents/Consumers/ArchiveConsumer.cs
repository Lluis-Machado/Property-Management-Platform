using DocumentsAPI.Services;
using MassTransit;
using MessagingContracts;

namespace DocumentsAPI.Consumers
{
    public class ArchiveConsumer : IConsumer<MessageContract>
    {
        private readonly IArchivesService _archivesService;

        public ArchiveConsumer(IArchivesService archiveService)
        {
            _archivesService = archiveService;
        }

        public async Task Consume(ConsumeContext<MessageContract> context)
        {
            var message = context.Message;
            // Todo
            //message.Payload;


        }
    }
}
