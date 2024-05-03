using KInspector.Core.Models;

namespace KInspector.Core.Services.Interfaces
{
    public interface IConfigService : IService
    {
        bool DeleteInstance(Guid guid);

        Instance? GetInstance(Guid guid);

        InspectorConfig GetConfig();

        void UpsertInstance(Instance instance);

        Instance? GetCurrentInstance();

        Instance? SetCurrentInstance(Guid? guid);
    }
}