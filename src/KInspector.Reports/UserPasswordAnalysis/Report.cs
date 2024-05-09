using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.UserPasswordAnalysis.Models;
using KInspector.Reports.UserPasswordAnalysis.Models.Data;
using KInspector.Reports.UserPasswordAnalysis.Models.Data.Results;

namespace KInspector.Reports.UserPasswordAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService)
            : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12", "13");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Security,
            ReportTags.Configuration
        };

        public static IEnumerable<string> ExcludedUserNames => new List<string>
        {
            "public"
        };

        public override ReportResults GetResults()
        {
            var users = databaseService.ExecuteSqlFromFile<CmsUser>(
                Scripts.GetEnabledAndNotExternalUsers,
                new { ExcludedUserNames }
                );

            var usersWithEmptyPasswords = GetUsersWithEmptyPasswords(users);
            var usersWithPlaintextPasswords = GetUsersWithPlaintextPasswords(users);

            return CompileResults(usersWithEmptyPasswords, usersWithPlaintextPasswords);
        }

        private static IEnumerable<CmsUserResultWithPasswordFormat> GetUsersWithEmptyPasswords(
            IEnumerable<CmsUser> users)
        {
            return users
                .Where(user => string.IsNullOrEmpty(user.UserPassword))
                .Select(user => new CmsUserResultWithPasswordFormat(user));
        }

        private static IEnumerable<CmsUserResult> GetUsersWithPlaintextPasswords(IEnumerable<CmsUser> users)
        {
            return users
                .Where(user => string.IsNullOrEmpty(user.UserPasswordFormat))
                .Select(user => new CmsUserResult(user));
        }

        private ReportResults CompileResults(
            IEnumerable<CmsUserResult> usersWithEmptyPasswords,
            IEnumerable<CmsUserResult> usersWithPlaintextPasswords)
        {
            if (!usersWithEmptyPasswords.Any() && !usersWithPlaintextPasswords.Any())
            {
                return new ReportResults
                {
                    Type = ResultsType.NoResults,
                    Status = ResultsStatus.Good,
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var errorReportResults = new ReportResults
            {
                Type = ResultsType.TableList,
                Status = ResultsStatus.Error
            };

            var emptyCount = IfAnyAddTableResult(
                errorReportResults.TableResults,
                usersWithEmptyPasswords,
                Metadata.Terms.TableTitles?.EmptyPasswords
                );

            var plaintextCount = IfAnyAddTableResult(
                errorReportResults.TableResults,
                usersWithPlaintextPasswords,
                Metadata.Terms.TableTitles?.PlaintextPasswords
                );

            errorReportResults.Summary = Metadata.Terms.ErrorSummary?.With(new { emptyCount, plaintextCount });

            return errorReportResults;
        }

        private static int IfAnyAddTableResult(IList<TableResult> tables, IEnumerable<object> results, Term? tableNameTerm)
        {
            if (results.Any())
            {
                tables.Add(new TableResult
                {
                    Name = tableNameTerm,
                    Rows = results
                });
            }

            return results.Count();
        }
    }
}