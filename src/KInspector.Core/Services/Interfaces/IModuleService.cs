using KInspector.Core.Models;
using KInspector.Core.Modules;

namespace KInspector.Core.Services.Interfaces
{
    public interface IModuleService : IService
    {
        IReport? GetReport(string codename);

        ReportResults GetReportResults(IReport report);

        IEnumerable<IReport> GetReports(bool getUntested = false, bool getIncompatible = false, string? tag = null);

        IEnumerable<IAction> GetActions();

        IAction? GetAction(string codename);

        ActionResults ExecuteAction(IAction action, string optionsJson);
    }
}