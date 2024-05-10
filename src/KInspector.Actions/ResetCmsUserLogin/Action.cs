using KInspector.Actions.ResetCmsUserLogin.Models;
using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;

namespace KInspector.Actions.ResetCmsUserLogin
{
    public class Action : AbstractAction<Terms,Options>
    {
        private readonly IDatabaseService databaseService;

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string> {
            ActionTags.Reset,
            ActionTags.User
        };

        public Action(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override ModuleResults Execute(Options options)
        {
            if (options.UserId < 0)
            {
                return GetInvalidOptionsResult();
            }

            // No user provided, list users
            if (options.UserId == 0)
            {
                return GetListingResult();
            }

            // Reset provided user
            databaseService.ExecuteSqlFromFileGeneric(Scripts.ResetAndEnableUser, new { UserID = options.UserId });
            var result = GetListingResult();
            result.Summary = Metadata.Terms.UserReset?.With(new {
                userId = options.UserId
            });

            return result;
        }

        public override ModuleResults GetInvalidOptionsResult()
        {
            return new ModuleResults {
                Status = ResultsStatus.Error,
                Summary = Metadata.Terms.InvalidOptions
            };
        }

        private ModuleResults GetListingResult()
        {
            var administratorUsers = databaseService.ExecuteSqlFromFile<CmsUser>(Scripts.GetAdministrators);
            var results = new ModuleResults
            {
                Type = ResultsType.TableList,
                Status = ResultsStatus.Information,
                Summary = Metadata.Terms.ListSummary
            };
            results.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.TableTitle,
                Rows = administratorUsers
            });

            return results;
        }
    }
}
