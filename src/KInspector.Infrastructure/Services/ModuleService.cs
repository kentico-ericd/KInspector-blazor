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

        public ActionResults ExecuteAction(string codename, Guid instanceGuid, string optionsJson)
        {
            var action = actionRepository.GetAction(codename);
            var instance = configService.SetCurrentInstance(instanceGuid);
            databaseService.Configure(instance.DatabaseSettings);

            return action.Execute(optionsJson);
        }

        public IAction GetAction(string codename) => actionRepository.GetAction(codename);

        public IEnumerable<IAction> GetActions(Guid instanceGuid)
        {
            configService.SetCurrentInstance(instanceGuid);
            return actionRepository.GetActions();
        }

        public IReport GetReport(string codename) => reportRepository.GetReport(codename);

        public ReportResults GetReportResults(string reportCodename, Guid instanceGuid)
        {
            var report = reportRepository.GetReport(reportCodename);
            var instance = configService.SetCurrentInstance(instanceGuid);

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