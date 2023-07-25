using DocumentsAPI.Models;
using DocumentsAPI.Repositories;

namespace DocumentsAPI.Services
{
    public class ArchivesService : IArchivesService
    {
        private readonly ILogger<ArchivesService> _logger;
        private readonly IArchiveRepository _archiveRepository;
        private readonly IFolderRepository _folderRepository;

        public ArchivesService(IArchiveRepository archiveRepository, ILogger<ArchivesService> logger, IFolderRepository folderRepository)
        {
            _archiveRepository = archiveRepository;
            _logger = logger;
            _folderRepository = folderRepository;
        }

        public async Task<Archive> CreateArchiveAsync(Archive archive)
        {
            //create archive
            archive.Id = Guid.NewGuid();
            await _archiveRepository.CreateArchiveAsync(archive);

            return archive;
        }

        public async Task<IEnumerable<Archive>> GetArchivesAsync(bool includeDeleted = false)
        {
            return await _archiveRepository.GetArchivesAsync(100, includeDeleted);
        }

        public async Task UpdateArchiveAsync(Guid archiveId, string newName)
        {
            await _archiveRepository.UpdateArchiveAsync(archiveId, newName);
        }

        public async Task DeleteArchiveAsync(Guid archiveId)
        {
            Task.WaitAll(_archiveRepository.DeleteArchiveAsync(archiveId),
                          _folderRepository.DeleteFoldersByArchiveAsync(archiveId));
        }

        public async Task UndeleteArchiveAsync(Guid archiveId)
        {
            Task.WaitAll(_archiveRepository.UndeleteArchiveAsync(archiveId),
                          _folderRepository.UndeleteFoldersByArchiveAsync(archiveId));
        }
    }
}
