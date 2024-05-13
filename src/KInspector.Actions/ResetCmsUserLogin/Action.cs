using KInspector.Actions.ResetCmsUserLogin.Models;
using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;

namespace KInspector.Actions.ResetCmsUserLogin
{
    public class Action : AbstractAction<Terms, Options>
    {
        private readonly IDatabaseService databaseService;

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string> {
            ModuleTags.Reset,
            ModuleTags.User
        };

        public Action(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override ModuleResults Execute(Options? options)
        {
            if (!UserIsValid(options?.UserId))
            {
                return GetInvalidOptionsResult();
            }

            // Reset provided user
            databaseService.ExecuteSqlFromFileGeneric(Scripts.ResetAndEnableUser, new { UserID = options?.UserId });
            var result = ExecuteListing();
            result.Status = ResultsStatus.Good;
            result.Summary = Metadata.Terms.UserReset?.With(new {
                userId = options?.UserId
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

        public override ModuleResults GetInvalidOptionsResult()
        {
            var result = ExecuteListing();
            result.Status = ResultsStatus.Error;
            result.Summary = Metadata.Terms.InvalidOptions;

            return result;
        }

        private bool UserIsValid(int? userId)
        {
            var users = databaseService.ExecuteSqlFromFile<CmsUser>(Scripts.GetAdministrators);

            return userId > 0 && users.Any(u => u.UserID == userId);
        }
    }
}
