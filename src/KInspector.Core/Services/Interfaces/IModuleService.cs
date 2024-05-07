using KInspector.Core.Models;
using KInspector.Core.Modules;

namespace KInspector.Core.Services.Interfaces
{
    public interface IModuleService : IService
    {
        IReport? GetReport(string codename);

        ReportResults GetReportResults(string reportCodename, Guid instanceGuid);

        IEnumerable<IReport> GetReports(Guid instanceGuid);

        IEnumerable<IAction> GetActions(Guid instanceGuid);

        IAction? GetAction(string codename);

        ActionResults ExecuteAction(string actionCodename, Guid instanceGuid, string optionsJson);
    }
}