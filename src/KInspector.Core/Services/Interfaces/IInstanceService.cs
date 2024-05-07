using KInspector.Core.Models;

namespace KInspector.Core.Services.Interfaces
{
    public interface IInstanceService : IService
    {
        InstanceDetails GetInstanceDetails(Guid instanceGuid);

        InstanceDetails GetInstanceDetails(Instance? instance);
    }
}