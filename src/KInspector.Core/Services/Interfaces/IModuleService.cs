using KInspector.Core.Models;
using KInspector.Core.Modules;

namespace KInspector.Core.Services.Interfaces
{
    public interface IModuleService : IService
    {
        IReport? GetReport(string codename);

        ModuleResults GetReportResults(IReport report);

        IEnumerable<IReport> GetReports(bool getUntested = false, bool getIncompatible = false, string? tag = null);

        IEnumerable<IAction> GetActions(bool getUntested = false, bool getIncompatible = false, string? tag = null);

        IAction? GetAction(string codename);

        ModuleResults ExecuteAction(IAction action, string optionsJson);
    }
}