using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.ApplicationRestartAnalysis.Models;
using KInspector.Reports.ApplicationRestartAnalysis.Models.Data;

namespace KInspector.Reports.ApplicationRestartAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.EventLog,
            ReportTags.Health
        };

        public Report(
            IDatabaseService databaseService,
            IModuleMetadataService moduleMetadataService
            ) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override ReportResults GetResults()
        {
            var cmsEventLogs = databaseService.ExecuteSqlFromFile<CmsEventLog>(Scripts.GetCmsEventLogsWithStartOrEndCode);

            return CompileResults(cmsEventLogs);
        }

        private ReportResults CompileResults(IEnumerable<CmsEventLog> cmsEventLogs)
        {
            if (!cmsEventLogs.Any())
            {
                return new ReportResults
                {
                    Status = ResultsStatus.Good,
                    Summary = Metadata.Terms.Summaries?.Good
                };
            }

            var totalEvents = cmsEventLogs.Count();
            var totalStartEvents = cmsEventLogs.Count(e => e.EventCode == "STARTAPP");
            var totalEndEvents = cmsEventLogs.Count(e => e.EventCode == "ENDAPP");
            var earliestTime = totalEvents > 0
                ? cmsEventLogs.Min(e => e.EventTime)
                : new DateTime();

            var latestTime = totalEvents > 0
                ? cmsEventLogs.Max(e => e.EventTime)
                : new DateTime();

            var summary = Metadata.Terms.Summaries?.Information?.With(new
            {
                earliestTime,
                latestTime,
                totalEndEvents,
                totalEvents,
                totalStartEvents
            });

            var results = new ReportResults
            {
                Summary = summary,
                Type = ResultsType.TableList,
                Status = ResultsStatus.Information
            };
            results.TableResults.Add(new TableResult()
            {
                Name = Metadata.Terms.TableTitles?.ApplicationRestartEvents,
                Rows = cmsEventLogs
            });

            return results;
        }
    }
}