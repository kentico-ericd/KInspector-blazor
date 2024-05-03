using KInspector.Core.Modules;

namespace KInspector.Core.Repositories.Interfaces
{
    public interface IReportRepository : IRepository
    {
        IEnumerable<IReport> GetReports();

        IReport? GetReport(string codename);
    }
}