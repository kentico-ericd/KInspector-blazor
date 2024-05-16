using KInspector.Core.Models;

namespace KInspector.Core.Modules
{
    public interface IReport : IModule
    {
        Task<ModuleResults> GetResults();
    }
}