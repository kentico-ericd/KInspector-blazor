using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.DatabaseConsistencyCheck.Models;

using System.Data;

namespace KInspector.Reports.DatabaseConsistencyCheck
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
#pragma warning disable 0618 // This is a special exemption as the results of CheckDB are unknown
            var checkDbResults = databaseService.ExecuteSqlFromFileAsDataTable(Scripts.GetCheckDbResults);
#pragma warning restore 0618

            return CompileResults(checkDbResults);
        }

        private ReportResults CompileResults(DataTable checkDbResults)
        {
            var hasIssues = checkDbResults.Rows.Count > 0;

            if (hasIssues)
            {
                return new ReportResults
                {
                    Type = ResultsType.Table,
                    Status = ResultsStatus.Error,
                    Summary = Metadata.Terms.CheckResultsTableForAnyIssues,
                    Data = checkDbResults
                };
            }
            else
            {
                return new ReportResults
                {
                    Type = ResultsType.String,
                    Status = ResultsStatus.Good,
                    Summary = Metadata.Terms.NoIssuesFound,
                    Data = string.Empty
                };
            }
        }
    }
}