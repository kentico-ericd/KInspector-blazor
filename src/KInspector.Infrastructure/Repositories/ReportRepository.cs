using KInspector.Core.Modules;
using KInspector.Core.Repositories.Interfaces;

namespace KInspector.Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly IEnumerable<IReport> reports;

        public ReportRepository(IEnumerable<IReport> reports)
        {
            this.reports = reports;
        }

        public IReport? GetReport(string codename)
        {
            var allReports = LoadReports();
            return allReports.FirstOrDefault(x => x.Codename.ToLower() == codename.ToLower());
        }

        public IEnumerable<IReport> GetReports()
        {
            return LoadReports();
        }

        private IEnumerable<IReport> LoadReports()
        {
            return reports.OrderBy(r => r.Codename);
        }
    }
}