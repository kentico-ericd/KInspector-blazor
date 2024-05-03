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

        public Instance? CurrentInstance { get; private set; }

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

            return GetInstanceDetails(instance);
        }

        public InstanceDetails GetInstanceDetails(Instance instance)
        {
            var guid = instance.Guid ?? Guid.Empty;
            if (guid == Guid.Empty)
            {
                throw new InvalidOperationException("Instance missing GUID.");
            }

            if (instance.DatabaseSettings is null)
            {
                throw new InvalidOperationException("Instance missing database settings.");
            }

            _databaseService.Configure(instance.DatabaseSettings);

            return new InstanceDetails
            {
                Guid = guid,
                AdministrationVersion = _versionRepository.GetKenticoAdministrationVersion(instance),
                DatabaseVersion = _versionRepository.GetKenticoDatabaseVersion(instance),
                Sites = _siteRepository.GetSites(instance)
            };
        }
    }
}