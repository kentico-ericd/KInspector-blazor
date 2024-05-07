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

        public ModuleService(IReportRepository reportRepository, IActionRepository actionRepository, IConfigService configService, IDatabaseService databaseService)
        {
            this.reportRepository = reportRepository;
            this.actionRepository = actionRepository;
            this.configService = configService;
            this.databaseService = databaseService;
        }

        public ActionResults ExecuteAction(string actionCodename, Guid instanceGuid, string optionsJson)
        {
            var action = actionRepository.GetAction(actionCodename);
            if (action is null)
            {
                throw new InvalidOperationException($"No action with code name '{actionCodename}.'");
            }

            var instance = configService.SetCurrentInstance(instanceGuid);
            if (instance is null)
            {
                throw new InvalidOperationException($"No instance with GUID '{instanceGuid}.'");
            }

            databaseService.Configure(instance.DatabaseSettings);

            return action.Execute(optionsJson);
        }

        public IAction? GetAction(string codename) => actionRepository.GetAction(codename);

        public IEnumerable<IAction> GetActions(Guid instanceGuid)
        {
            configService.SetCurrentInstance(instanceGuid);
            return actionRepository.GetActions();
        }

        public IReport? GetReport(string codename) => reportRepository.GetReport(codename);

        public ReportResults GetReportResults(string reportCodename, Guid instanceGuid)
        {
            var report = reportRepository.GetReport(reportCodename);
            if (report is null)
            {
                throw new InvalidOperationException($"No report with code name '{reportCodename}.'");
            }

            var instance = configService.SetCurrentInstance(instanceGuid);
            if (instance is null)
            {
                throw new InvalidOperationException($"No instance with GUID '{instanceGuid}.'");
            }

            databaseService.Configure(instance.DatabaseSettings);

            return report.GetResults();
        }

        public IEnumerable<IReport> GetReports(Guid instanceGuid)
        {
            configService.SetCurrentInstance(instanceGuid);
            return reportRepository.GetReports();
        }
    }
}