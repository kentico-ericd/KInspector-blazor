using KInspector.Core.Models;
using KInspector.Core.Repositories.Interfaces;
using KInspector.Core.Services.Interfaces;

namespace KInspector.Infrastructure.Services
{
    public class InstanceService : IInstanceService
    {
        private readonly IConfigService _configService;
        private readonly ICmsFileService _fileService;
        private readonly ISiteRepository _siteRepository;
        private readonly IVersionRepository _versionRepository;
        private readonly IDatabaseService _databaseService;

        public InstanceService(
            IConfigService configService,
            ICmsFileService fileService,
            IVersionRepository versionRepository,
            ISiteRepository siteRepository,
            IDatabaseService databaseService)
        {
            _configService = configService;
            _fileService = fileService;
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

        public InstanceDetails GetInstanceDetails(Instance instance)
        {
            var guid = instance.Guid ?? Guid.Empty;
            if (guid == Guid.Empty)
            {
                throw new InvalidOperationException("Instance missing GUID.");
            }

            var connectionString = _fileService.GetCMSConnectionString(instance.AdministrationPath);
            var instanceDetails = new InstanceDetails
            {
                Guid = guid,
                AdministrationVersion = _versionRepository.GetKenticoAdministrationVersion(instance),
                AdministrationDatabaseVersion = _versionRepository.GetKenticoDatabaseVersion(instance.DatabaseSettings, connectionString),
                AdministrationConnectionString = connectionString,
                Sites = _siteRepository.GetSites(instance.DatabaseSettings, connectionString)
            };

            _databaseService.Configure(instance.DatabaseSettings, instanceDetails.AdministrationConnectionString);

            return instanceDetails;
        }
    }
}