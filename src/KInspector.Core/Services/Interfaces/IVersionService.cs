using KInspector.Core.Models;

namespace KInspector.Core.Services.Interfaces
{
    /// <summary>
    /// Contains methods for getting version information from a Kentico instance.
    /// </summary>
    public interface IVersionService : IService
    {
        /// <summary>
        /// Gets the version of the Kentico DLLs in the administration website, or <c>null</c> if not found.
        /// </summary>
        Task<Version?> GetKenticoAdministrationVersion(Instance instance);

        /// <summary>
        /// Gets the version of the Kentico DLLs in the administration website, or <c>null</c> if not found.
        /// </summary>
        /// <param name="rootPath">The root of the Kentico administration website.</param>
        Task<Version?> GetKenticoAdministrationVersion(string rootPath);

        /// <summary>
        /// Gets the version in the Kentico database's CMS_SettingsKey table, or <c>null</c> if not found.
        /// </summary>
        Task<Version?> GetKenticoDatabaseVersion(DatabaseSettings databaseSettings);
    }
}