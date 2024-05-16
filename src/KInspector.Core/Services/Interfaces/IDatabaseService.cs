using System.Data;

using KInspector.Core.Models;

namespace KInspector.Core.Services.Interfaces
{
    public interface IDatabaseService : IService
    {
        void Configure(DatabaseSettings databaseSettings);

        Task<IEnumerable<T>> ExecuteSqlFromFile<T>(string relativeFilePath);

        Task<IEnumerable<T>> ExecuteSqlFromFile<T>(string relativeFilePath, dynamic parameters);

        Task<IEnumerable<T>> ExecuteSqlFromFile<T>(string relativeFilePath, IDictionary<string, string> literalReplacements);

        Task<IEnumerable<T>> ExecuteSqlFromFile<T>(string relativeFilePath, IDictionary<string, string> literalReplacements, dynamic parameters);

        [Obsolete("A last resort when it is impossible to create a data model or use one of the generic options.")]
        Task<DataTable> ExecuteSqlFromFileAsDataTable(string relativeFilePath);

        Task<IEnumerable<IDictionary<string, object>>> ExecuteSqlFromFileGeneric(string relativeFilePath);

        Task<IEnumerable<IDictionary<string, object>>> ExecuteSqlFromFileGeneric(string relativeFilePath, dynamic parameters);

        Task<IEnumerable<IDictionary<string, object>>> ExecuteSqlFromFileGeneric(string relativeFilePath, IDictionary<string, string> literalReplacements);

        Task<IEnumerable<IDictionary<string, object>>> ExecuteSqlFromFileGeneric(string relativeFilePath, IDictionary<string, string> literalReplacements, dynamic parameters);

        Task<T> ExecuteSqlFromFileScalar<T>(string relativeFilePath);

        Task<T> ExecuteSqlFromFileScalar<T>(string relativeFilePath, dynamic parameters);

        Task<T> ExecuteSqlFromFileScalar<T>(string relativeFilePath, IDictionary<string, string> literalReplacements);

        Task<T> ExecuteSqlFromFileScalar<T>(string relativeFilePath, IDictionary<string, string> literalReplacements, dynamic parameters);
    }
}