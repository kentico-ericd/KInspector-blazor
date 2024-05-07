using KInspector.Core.Models;
using KInspector.Core.Repositories.Interfaces;
using KInspector.Core.Services.Interfaces;

namespace KInspector.Infrastructure.Services
{
    public class InstanceService : IInstanceService
    {
        private readonly IConfigService _configService;
        private readonly ISiteRepository _siteRepository;
        private readonly IVersionRepository _versionRepository;
        private readonly IDatabaseService _databaseService;

        public InstanceService(
            IConfigService configService,
            IVersionRepository versionRepository,
            ISiteRepository siteRepository,
            IDatabaseService databaseService)
        {
            _configService = configService;
            _versionRepository = versionRepository;
            _siteRepository = siteRepository;
            _databaseService = databaseService;
        }

        public InstanceDetails GetInstanceDetails(Guid instanceGuid)
        {
            var instance = _configService.GetInstance(instanceGuid);
            if (instance is null)
            {
                throw new InvalidOperationException($"No instance with GUID '{instanceGuid}.'");
            }

            return GetInstanceDetails(instance);
        }

        public InstanceDetails GetInstanceDetails(Instance? instance)
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var guid = instance.Guid ?? Guid.Empty;
            if (guid == Guid.Empty)
            {
                throw new InvalidOperationException("Instance missing GUID.");
            }

            var instanceDetails = new InstanceDetails
            {
                Guid = guid,
                AdministrationVersion = _versionRepository.GetKenticoAdministrationVersion(instance),
                AdministrationDatabaseVersion = _versionRepository.GetKenticoDatabaseVersion(instance.DatabaseSettings),
                Sites = _siteRepository.GetSites(instance.DatabaseSettings)
            };

            _databaseService.Configure(instance.DatabaseSettings);

            return instanceDetails;
        }
    }
}