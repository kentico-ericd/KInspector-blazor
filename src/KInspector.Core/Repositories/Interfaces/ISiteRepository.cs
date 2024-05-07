using KInspector.Core.Models;

namespace KInspector.Core.Repositories.Interfaces
{
    public interface ISiteRepository : IRepository
    {
        Site GetSite(Instance instance, int siteId);

        IList<Site> GetSites(DatabaseSettings databaseSettings);
    }
}