using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.DatabaseTableSizeAnalysis.Models;

namespace KInspector.Reports.DatabaseTableSizeAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string> {
            ReportTags.Health
        };

        public override ReportResults GetResults()
        {
            var top25LargestTables = databaseService.ExecuteSqlFromFile<DatabaseTableSizeResult>(Scripts.GetTop25LargestTables);

            return new ReportResults
            {
                Type = top25LargestTables.Any() ? ResultsType.Table : ResultsType.NoResults,
                Status = ResultsStatus.Information,
                Summary = Metadata.Terms.CheckResultsTableForAnyIssues,
                Data = new TableResult<DatabaseTableSizeResult>()
                {
                    Name = Metadata.Terms.Top25Results,
                    Rows = top25LargestTables
                }
            };
        }
    }
}