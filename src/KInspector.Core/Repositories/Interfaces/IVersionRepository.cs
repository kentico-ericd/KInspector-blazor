using KInspector.Core.Models;

namespace KInspector.Core.Repositories.Interfaces
{
    public interface IVersionRepository : IRepository
    {
        Version? GetKenticoAdministrationVersion(Instance instance);

        Version? GetKenticoAdministrationVersion(string rootPath);

        Version? GetKenticoDatabaseVersion(DatabaseSettings? databaseSettings, string? connectionString);
    }
}