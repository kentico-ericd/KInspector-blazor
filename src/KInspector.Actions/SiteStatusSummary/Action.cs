using KInspector.Actions.SiteStatusSummary.Models;
using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;

namespace KInspector.Actions.SiteStatusSummary
{
    public class Action : AbstractAction<Terms, Options>
    {
        private readonly IDatabaseService databaseService;

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("12", "13");

        public override IList<string> Tags => new List<string> {
            ModuleTags.Site,
            ModuleTags.Configuration
        };

        public Action(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override ModuleResults Execute(Options? options)
        {
            if (options?.SiteId == 0)
            {
                return ExecuteListing();
            }

            if (!SiteIsValid(options?.SiteId))
            {
                return GetInvalidOptionsResult();
            }

            databaseService.ExecuteSqlFromFileGeneric(Scripts.StopSite, new { SiteID = options?.SiteId });
            var result = ExecuteListing();
            result.Status = ResultsStatus.Good;
            result.Summary = Metadata.Terms.SiteStopped?.With(new
            {
                siteId = options?.SiteId
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
            var sites = databaseService.ExecuteSqlFromFile<CmsSite>(Scripts.GetSiteSummary);
            var results = new ModuleResults
            {
                Type = ResultsType.TableList,
                Status = ResultsStatus.Information,
                Summary = Metadata.Terms.ListSummary
            };
            results.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.TableTitle,
                Rows = sites
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

        private bool SiteIsValid(int? siteId)
        {
            var sites = databaseService.ExecuteSqlFromFile<CmsSite>(Scripts.GetSiteSummary);

            return siteId > 0 &&
                sites.Any(s => s.ID == siteId) &&
                (sites.FirstOrDefault(s => s.ID == siteId)?.Running ?? true);
        }
    }
}