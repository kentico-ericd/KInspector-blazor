using KInspector.Core.Modules;

namespace KInspector.Core.Repositories.Interfaces
{
    public interface IActionRepository : IRepository
    {
        IEnumerable<IAction> GetActions();

        IAction? GetAction(string codename);
    }
}