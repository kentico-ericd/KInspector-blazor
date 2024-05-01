using KInspector.Core.Models;

namespace KInspector.Core.Repositories.Interfaces
{
    public interface IInstanceRepository : IRepository
    {
        bool DeleteInstance(Guid guid);

        Instance? GetInstance(Guid guid);

        IList<Instance> GetInstances();

        Instance UpsertInstance(Instance instance);
    }
}