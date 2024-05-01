using KInspector.Core.Modules;

using System.Collections.Generic;

namespace KInspector.Core.Repositories.Interfaces
{
    public interface IReportRepository : IRepository
    {
        IEnumerable<IReport> GetReports();

        IReport GetReport(string codename);
    }
}