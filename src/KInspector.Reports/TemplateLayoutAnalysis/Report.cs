using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.TemplateLayoutAnalysis.Models;

namespace KInspector.Reports.TemplateLayoutAnalysis
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
            ReportTags.Information,
            ReportTags.PortalEngine
        };

        public override ReportResults GetResults()
        {
            var identicalLayouts = databaseService.ExecuteSqlFromFile<IdenticalPageLayouts>(Scripts.GetIdenticalLayouts);

            return CompileResults(identicalLayouts);
        }

        private ReportResults CompileResults(IEnumerable<IdenticalPageLayouts> identicalPageLayouts)
        {
            var countIdenticalPageLayouts = identicalPageLayouts.Count();
            var results = new ReportResults
            {
                Status = ResultsStatus.Information,
                Type = ResultsType.Table,
                Data = new TableResult<dynamic>()
                {
                    Name = Metadata.Terms.IdenticalPageLayouts,
                    Rows = identicalPageLayouts
                }
            };

            if (countIdenticalPageLayouts == 0)
            {
                results.Summary = Metadata.Terms.NoIdenticalPageLayoutsFound;
            }
            else
            {
                results.Summary = Metadata.Terms.CountIdenticalPageLayoutFound?.With(new { count = countIdenticalPageLayouts });
            }

            return results;
        }
    }
}