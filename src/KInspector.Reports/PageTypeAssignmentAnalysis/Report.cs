﻿using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.PageTypeAssignmentAnalysis.Models;

namespace KInspector.Reports.PageTypeAssignmentAnalysis
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
            ReportTags.Health,
            ReportTags.Consistency
        };

        public override ReportResults GetResults()
        {
            var unassignedPageTypes = databaseService.ExecuteSqlFromFile<PageType>(Scripts.GetPageTypesNotAssignedToSite);

            return CompileResults(unassignedPageTypes);
        }

        private ReportResults CompileResults(IEnumerable<PageType> unassignedPageTypes)
        {
            var results = new ReportResults
            {
                Status = ResultsStatus.Good,
                Summary = Metadata.Terms.NoIssuesFound,
                Type = ResultsType.Table,
                Data = new TableResult<PageType>()
                {
                    Name = Metadata.Terms.UnassignedPageTypesTableHeader,
                    Rows = unassignedPageTypes
                }
            };

            var unassignedPageTypeCount = unassignedPageTypes.Count();
            if (unassignedPageTypeCount > 0)
            {
                results.Status = ResultsStatus.Warning;
                results.Summary = Metadata.Terms.WarningSummary?.With(new { unassignedPageTypeCount });
            }

            return results;
        }
    }
}