using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.UnusedPageTypeSummary.Models;

namespace KInspector.Reports.UnusedPageTypeSummary
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Information
        };

        public override ReportResults GetResults()
        {
            var unusedPageTypes = databaseService.ExecuteSqlFromFile<PageType>(Scripts.GetUnusedPageTypes);
            var countOfUnusedPageTypes = unusedPageTypes.Count();

            return new ReportResults
            {
                Type = ResultsType.Table,
                Status = ResultsStatus.Information,
                Summary = Metadata.Terms.CountUnusedPageType?.With(new { count = countOfUnusedPageTypes }),
                Data = new TableResult<PageType>()
                {
                    Name = Metadata.Terms.UnusedPageTypes,
                    Rows = unusedPageTypes
                }
            };
        }
    }
}