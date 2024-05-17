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
        private readonly IModuleMetadataService moduleMetadataService;

        public ModuleService(
            IReportRepository reportRepository,
            IActionRepository actionRepository,
            IConfigService configService,
            IDatabaseService databaseService,
            IInstanceService instanceService,
            IModuleMetadataService moduleMetadataService)
        {
            this.reportRepository = reportRepository;
            this.actionRepository = actionRepository;
            this.configService = configService;
            this.databaseService = databaseService;
            this.instanceService = instanceService;
            this.moduleMetadataService = moduleMetadataService;
        }

        public async Task ExecuteAction(IAction action, string optionsJson, Action<ModuleResults> callback)
        {
            var instance = await configService.GetCurrentInstance() ?? throw new InvalidOperationException($"There is no connected instance.'");
            databaseService.Configure(instance.DatabaseSettings);

            var results = await action.Execute(optionsJson);
            callback(results);
        }

        public IAction? GetAction(string codename) => actionRepository.GetAction(codename);

        public async Task<IEnumerable<IAction>> GetActions(bool getUntested = false, bool getIncompatible = false, string? tag = null, string? name = null)
        {
            var instance = await configService.GetCurrentInstance() ?? throw new InvalidOperationException("An instance must be connected.");
            var instanceDetails = await instanceService.GetInstanceDetails(instance);
            var dbMajorVersion = instanceDetails?.AdministrationDatabaseVersion?.Major ?? 0;
            var actions = actionRepository.GetActions();
            var filtered = actions.Where(r => r.CompatibleVersions.Select(v => v.Major).Contains(dbMajorVersion)).ToList();
            if (getUntested)
            {
                filtered = filtered.Union(actions.Where(r =>
                    !r.CompatibleVersions.Select(v => v.Major).Contains(dbMajorVersion) &&
                    !r.IncompatibleVersions.Select(v => v.Major).Contains(dbMajorVersion)
                )).ToList();
            }

            if (getIncompatible)
            {
                filtered = filtered.Union(actions.Where(r => r.IncompatibleVersions.Select(v => v.Major).Contains(dbMajorVersion))).ToList();
            }

            if (!string.IsNullOrEmpty(tag))
            {
                filtered = filtered.Where(r => r.Tags.Contains(tag)).ToList();
            }

            if (!string.IsNullOrEmpty(name))
            {
                foreach (var action in filtered)
                {
                    var details = await moduleMetadataService.GetModuleDetails(action.Codename);
                    if (!details.Name?.Contains(name, StringComparison.InvariantCultureIgnoreCase) ?? true)
                    {
                        filtered.Remove(action);
                    }
                }
            }

            return filtered.OrderBy(r => r.Codename);
        }

        public IReport? GetReport(string codename) => reportRepository.GetReport(codename);

        public async Task GetReportResults(IReport report, Action<ModuleResults> callback)
        {
            var instance = await configService.GetCurrentInstance() ?? throw new InvalidOperationException($"There is no connected instance.'");
            databaseService.Configure(instance.DatabaseSettings);

            try
            {
                var results = await report.GetResults();
                callback(results);
            }
            catch (Exception ex)
            {
                callback(new ModuleResults
                {
                    Status = ResultsStatus.Error,
                    Summary = ex.Message,
                    Type = ResultsType.NoResults
                });
            }
        }

        public async Task<IEnumerable<IReport>> GetReports(bool getUntested = false, bool getIncompatible = false, string? tag = null, string? name = null)
        {
            var instance = await configService.GetCurrentInstance() ?? throw new InvalidOperationException("An instance must be connected.");
            var instanceDetails = await instanceService.GetInstanceDetails(instance);
            var dbMajorVersion = instanceDetails?.AdministrationDatabaseVersion?.Major ?? 0;
            var reports = reportRepository.GetReports();
            var filtered = reports.Where(r => r.CompatibleVersions.Select(v => v.Major).Contains(dbMajorVersion)).ToList();
            if (getUntested)
            {
                filtered = filtered.Union(reports.Where(r =>
                    !r.CompatibleVersions.Select(v => v.Major).Contains(dbMajorVersion) &&
                    !r.IncompatibleVersions.Select(v => v.Major).Contains(dbMajorVersion)
                )).ToList();
            }

            if (getIncompatible)
            {
                filtered = filtered.Union(reports.Where(r => r.IncompatibleVersions.Select(v => v.Major).Contains(dbMajorVersion))).ToList();
            }

            if (!string.IsNullOrEmpty(tag))
            {
                filtered = filtered.Where(r => r.Tags.Contains(tag)).ToList();
            }

            if (!string.IsNullOrEmpty(name))
            {
                foreach (var report in filtered)
                {
                    var details = await moduleMetadataService.GetModuleDetails(report.Codename);
                    if (!details.Name?.Contains(name, StringComparison.InvariantCultureIgnoreCase) ?? true)
                    {
                        filtered.Remove(report);
                    }
                }
            }

            return filtered.OrderBy(r => r.Codename);
        }
    }
}