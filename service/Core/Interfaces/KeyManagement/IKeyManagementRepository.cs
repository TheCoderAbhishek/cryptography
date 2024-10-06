using service.Core.Entities.KeyManagement;

namespace service.Core.Interfaces.KeyManagement
{
    public interface IKeyManagementRepository
    {
        Task<List<Keys>> GetKeysListAsync();
    }
}
