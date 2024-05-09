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

            return report.GetResults();
        }

        public IEnumerable<IReport> GetReports()
        {
            return reportRepository.GetReports();
        }
    }
}