using DocumentsAPI.Models;
using DocumentsAPI.Repositories;
using FluentValidation;
using FluentValidation.Results;
using static DocumentsAPI.Models.Archive;

namespace DocumentsAPI.Services
{
    public class ArchivesService : IArchivesService
    {
        private readonly ILogger<ArchivesService> _logger;
        private readonly IValidator<Archive> _archiveValidator;
        private readonly IArchiveRepository _archiveRepository;
        private readonly IFolderRepository _folderRepository;

        public ArchivesService(IArchiveRepository archiveRepository, ILogger<ArchivesService> logger, IFolderRepository folderRepository)
        {
            _archiveRepository = archiveRepository;
            _logger = logger;
            _folderRepository = folderRepository;
        }

        public async Task<Archive> CreateArchiveAsync(Archive archive, ARCHIVE_TYPE type = ARCHIVE_TYPE.NONE, Guid? objectId = null)
        {
            // Validations
            if (type != ARCHIVE_TYPE.NONE && objectId == null) {
                throw new Exception($"Cannot create a {type.ToString().ToLowerInvariant()} archive without the corresponding {type.ToString().ToLowerInvariant()} Guid");
            }

            ValidationResult validationResult = await _archiveValidator.ValidateAsync(archive);
            if (!validationResult.IsValid) throw new ValidationException(validationResult.ToString("~"));

            //create archive
            archive.Id = Guid.NewGuid();
            archive.ArchiveType = type;
            archive.RelatedItemId = objectId;
            await _archiveRepository.CreateArchiveAsync(archive);

            if (type != ARCHIVE_TYPE.NONE) await _folderRepository.CreateDefaultFolders(archive);

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
