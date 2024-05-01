using KInspector.Core.Models;

namespace KInspector.Core.Modules
{
    public interface IWithModuleMetadata<T> where T : new()
    {
        ModuleMetadata<T> Metadata { get; }
    }
}