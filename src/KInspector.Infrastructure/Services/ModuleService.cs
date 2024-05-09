using KInspector.Core.Constants;
using KInspector.Core.Models;
using KInspector.Core.Modules;
using KInspector.Core.Repositories.Interfaces;
using KInspector.Core.Services.Interfaces;

namespace KInspector.Infrastructure.Services
{
    public class ModuleService : IModuleService
    {
        private readonly IDatabaseService databaseService;
        private readonly IConfigService configService;
        private readonly IReportRepository reportRepository;
        private readonly IActionRepository actionRepository;
        private readonly IInstanceService instanceService;

        public ModuleService(IReportRepository reportRepository, IActionRepository actionRepository, IConfigService configService, IDatabaseService databaseService, IInstanceService instanceService)
        {
            this.reportRepository = reportRepository;
            this.actionRepository = actionRepository;
            this.configService = configService;
            this.databaseService = databaseService;
            this.instanceService = instanceService;
        }

        public ActionResults ExecuteAction(IAction action, string optionsJson)
        {
            var instance = configService.GetCurrentInstance();
            if (instance is null)
            {
                throw new InvalidOperationException($"There is no connected instance.'");
            }

            databaseService.Configure(instance.DatabaseSettings);

            return action.Execute(optionsJson);
        }

        public IAction? GetAction(string codename) => actionRepository.GetAction(codename);

        public IEnumerable<IAction> GetActions()
        {
            return actionRepository.GetActions();
        }

        public IReport? GetReport(string codename) => reportRepository.GetReport(codename);

        public ReportResults GetReportResults(IReport report)
        {
            var instance = configService.GetCurrentInstance();
            if (instance is null)
            {
                throw new InvalidOperationException($"There is no connected instance.'");
            }

            databaseService.Configure(instance.DatabaseSettings);

            try
            {
                return report.GetResults();
            }
            catch (Exception ex)
            {
                return new ReportResults
                {
                    Status = ResultsStatus.Error,
                    Summary = ex.Message,
                    Type = ResultsType.NoResults
                };
            }
        }

        public IEnumerable<IReport> GetReports(bool getUntested = false, bool getIncompatible = false)
        {
            var instance = configService.GetCurrentInstance();
            if (instance is null)
            {
                throw new InvalidOperationException("An instance must be connected.");
            }

            var instanceDetails = instanceService.GetInstanceDetails(instance);
            var dbMajorVersion = instanceDetails?.AdministrationDatabaseVersion?.Major ?? 0;
            var reports = reportRepository.GetReports();
            var filtered = reports.Where(r => r.CompatibleVersions.Select(v => v.Major).Contains(dbMajorVersion));
            if (getUntested)
            {
                filtered = filtered.Union(reports.Where(r =>
                    !r.CompatibleVersions.Select(v => v.Major).Contains(dbMajorVersion) &&
                    !r.IncompatibleVersions.Select(v => v.Major).Contains(dbMajorVersion)
                ));
            }

            if (getIncompatible)
            {
                filtered = filtered.Union(reports.Where(r => r.IncompatibleVersions.Select(v => v.Major).Contains(dbMajorVersion)));
            }

            return filtered.OrderBy(r => r.Codename);
        }
    }
}