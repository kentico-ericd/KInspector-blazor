using KInspector.Core.Models;

namespace KInspector.Core.Modules
{
    public interface IAction : IModule
    {
        Task<ModuleResults> Execute(string optionsJson);

        Type GetOptionsType();
    }
}