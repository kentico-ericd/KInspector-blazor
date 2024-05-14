using KInspector.Actions.DisableStagingServers.Models;
using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;

namespace KInspector.Actions.DisableStagingServers
{
    public class Action : AbstractAction<Terms, Options>
    {
        private readonly IDatabaseService databaseService;

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("12", "13");

        public override IList<string> Tags => new List<string> {
            ModuleTags.Configuration,
            ModuleTags.Staging
        };

        public Action(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override ModuleResults Execute(Options? options)
        {
            if (!ServerIsValid(options?.ServerId))
            {
                return GetInvalidOptionsResult();
            }

            databaseService.ExecuteSqlFromFileGeneric(Scripts.DisableServer, new { ServerID = options?.ServerId });
            var result = ExecuteListing();
            result.Status = ResultsStatus.Good;
            result.Summary = Metadata.Terms.ServerDisabled?.With(new
            {
                serverId = options?.ServerId
            });

            return result;
        }

        public override ModuleResults ExecutePartial(Options? options)
        {
            // All options are required for this action
            throw new NotImplementedException();
        }

        public override ModuleResults ExecuteListing()
        {
            var servers = databaseService.ExecuteSqlFromFile<StagingServer>(Scripts.GetStagingServerSummary);
            var result = new ModuleResults
            {
                Type = ResultsType.TableList,
                Status = ResultsStatus.Information,
                Summary = Metadata.Terms.ListSummary
            };
            result.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.TableTitle,
                Rows = servers
            });

            return result;
        }

        public override ModuleResults GetInvalidOptionsResult()
        {
            var result = ExecuteListing();
            result.Status = ResultsStatus.Error;
            result.Summary = Metadata.Terms.InvalidOptions;

            return result;
        }

        private bool ServerIsValid(int? serverId)
        {
            var servers = databaseService.ExecuteSqlFromFile<StagingServer>(Scripts.GetStagingServerSummary);

            return serverId > 0 && servers.Any(s => s.ID == serverId);
        }
    }
}