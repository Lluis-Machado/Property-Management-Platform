using LogsAPI.Models;

namespace LogsAPI.Services
{
    public class LogsService
    {
        private readonly LogsRepository _logsRepository;

        public LogsService(LogsRepository logsRepository)
        {
            _logsRepository = logsRepository;
        }

        public async Task<List<Log>> GetLogsAsync(DateTime? periodStart, DateTime? periodEnd)
        {
            if (periodStart != null && periodEnd != null && periodEnd < periodStart)
            {
                throw new Exception("Period End cannot be before Period Start");
            }
            // You can add any additional business logic or data processing here
            return await _logsRepository.GetAsync(periodStart, periodEnd);
        }
    }
}
