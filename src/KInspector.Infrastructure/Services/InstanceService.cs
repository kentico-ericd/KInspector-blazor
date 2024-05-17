using Dapper;

using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;

namespace KInspector.Infrastructure.Services
{
    public class InstanceService : IInstanceService
    {
        private readonly IConfigService _configService;
        private readonly IVersionService _versionService;
        private readonly IDatabaseService _databaseService;

        public InstanceService(
            IConfigService configService,
            IVersionService versionService,
            IDatabaseService databaseService)
        {
            _configService = configService;
            _versionService = versionService;
            _databaseService = databaseService;
        }

        public async Task<InstanceDetails> GetInstanceDetails(Guid instanceGuid)
        {
            var instance = await _configService.GetInstance(instanceGuid);

            return instance is null
                ? throw new InvalidOperationException($"No instance with GUID '{instanceGuid}.'")
                : await GetInstanceDetails(instance);
        }

        public async Task<InstanceDetails> GetInstanceDetails(Instance? instance)
        {
            ArgumentNullException.ThrowIfNull(instance);
            var instanceDetails = new InstanceDetails
            {
                AdministrationVersion = await _versionService.GetKenticoAdministrationVersion(instance),
                AdministrationDatabaseVersion = await _versionService.GetKenticoDatabaseVersion(instance.DatabaseSettings),
                Sites = await GetSites(instance.DatabaseSettings)
            };

            _databaseService.Configure(instance.DatabaseSettings);

            return instanceDetails;
        }

        private async static Task<IList<Site>> GetSites(DatabaseSettings databaseSettings)
        {
            try
            {
                var query = @"
                    SELECT
                        SiteId as Id,
                        SiteName as Name,
                        SiteGUID as Guid,
                        SiteDomainName as DomainName,
                        SitePresentationURL as PresentationUrl
                    FROM CMS_Site";
                var connection = DatabaseHelper.GetSqlConnection(databaseSettings);
                var sites = await connection.QueryAsync<Site>(query);

                return sites.ToList();
            }
            catch
            {
                return Array.Empty<Site>();
            }
        }
    }
}