using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.ClassTableValidation.Models;

namespace KInspector.Reports.ClassTableValidation
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;
        private readonly IInstanceService instanceService;
        private readonly IConfigService configService;

        public Report(
            IDatabaseService databaseService,
            IInstanceService instanceService,
            IModuleMetadataService moduleMetadataService,
            IConfigService configService
            ) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
            this.instanceService = instanceService;
            this.configService = configService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string> {
            ReportTags.Health,
        };

        public override ReportResults GetResults()
        {
            var instance = configService.GetCurrentInstance();
            var instanceDetails = instanceService.GetInstanceDetails(instance);
            var tablesWithMissingClass = GetResultsForTables(instanceDetails);
            var classesWithMissingTable = GetResultsForClasses();

            return CompileResults(tablesWithMissingClass, classesWithMissingTable);
        }

        private ReportResults CompileResults(IEnumerable<TableWithNoClass> tablesWithMissingClass, IEnumerable<ClassWithNoTable> classesWithMissingTable)
        {
            var tableErrors = tablesWithMissingClass.Count();
            var tableResults = new TableResult<dynamic>()
            {
                Name = Metadata.Terms.DatabaseTablesWithMissingKenticoClasses,
                Rows = tablesWithMissingClass
            };

            var classErrors = classesWithMissingTable.Count();
            var classResults = new TableResult<dynamic>()
            {
                Name = Metadata.Terms.KenticoClassesWithMissingDatabaseTables,
                Rows = classesWithMissingTable
            };

            var totalErrors = tableErrors + classErrors;

            var results = new ReportResults
            {
                Type = ResultsType.TableList
            };

            results.Data.TableResults = tableResults;
            results.Data.ClassResults = classResults;

            switch (totalErrors)
            {
                case 0:
                    results.Status = ResultsStatus.Good;
                    results.Summary = Metadata.Terms.NoIssuesFound;
                    break;

                default:
                    results.Status = ResultsStatus.Error;
                    results.Summary = Metadata.Terms.CountIssueFound?.With(new { count = totalErrors });
                    break;
            }

            return results;
        }

        private IEnumerable<ClassWithNoTable> GetResultsForClasses()
        {
            var classesWithMissingTable = databaseService.ExecuteSqlFromFile<ClassWithNoTable>(Scripts.ClassesWithNoTable);
            return classesWithMissingTable;
        }

        private IEnumerable<TableWithNoClass> GetResultsForTables(InstanceDetails instanceDetails)
        {
            var tablesWithMissingClass = databaseService.ExecuteSqlFromFile<TableWithNoClass>(Scripts.TablesWithNoClass);

            var tableWhitelist = GetTableWhitelist(instanceDetails.AdministrationDatabaseVersion);
            if (tableWhitelist.Count > 0)
            {
                tablesWithMissingClass = tablesWithMissingClass.Where(t => !tableWhitelist.Contains(t.TableName ?? string.Empty)).ToList();
            }

            return tablesWithMissingClass;
        }

        private List<string> GetTableWhitelist(Version? version)
        {
            var whitelist = new List<string>();

            if (version?.Major >= 10)
            {
                whitelist.Add("CI_Migration");
            }

            return whitelist;
        }
    }
}