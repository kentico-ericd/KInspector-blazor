using KInspector.Core.Models;
using System.Collections.Generic;

namespace KInspector.Core.Repositories.Interfaces
{
    public interface ISiteRepository : IRepository
    {
        Site GetSite(Instance instance, int siteId);

        IList<Site> GetSites(Instance instance);
    }
}