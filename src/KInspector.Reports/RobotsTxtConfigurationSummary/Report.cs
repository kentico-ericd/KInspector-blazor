using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.RobotsTxtConfigurationSummary.Models;

using System.Net;
using System.Net.Security;

namespace KInspector.Reports.RobotsTxtConfigurationSummary
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IConfigService configService;
        private readonly HttpClient _httpClient = new HttpClient();

        public Report(
            IConfigService configService,
            IModuleMetadataService moduleMetadataService,
            HttpClient? httpClient = null
        ) : base(moduleMetadataService)

        {
            this.configService = configService;

            if (httpClient is not null)
            {
                _httpClient = httpClient;
            }
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12");

        public override IList<Version> IncompatibleVersions => VersionHelper.GetVersionList("13");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Information,
            ReportTags.SEO,
        };

        public override ModuleResults GetResults()
        {
            var adminUrl = configService.GetCurrentInstance()?.AdministrationUrl;
            if (adminUrl is null)
            {
                return new ModuleResults
                {
                    Status = ResultsStatus.Warning,
                    Summary = Metadata.Terms.RobotsTxtNotFound,
                    Type = ResultsType.NoResults
                };
            }

            var instanceUri = new Uri(adminUrl);
            var testUri = new Uri(instanceUri, Constants.RobotsTxtRelativePath);

            return GetUriResults(testUri);
        }

        private ModuleResults GetUriResults(Uri testUri)
        {
            try
            {
                // Ignore invalid certificates
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => { return true; });
                HttpResponseMessage response = _httpClient.GetAsync(testUri).ConfigureAwait(false).GetAwaiter().GetResult();
                var found = response.StatusCode == HttpStatusCode.OK;

                return new ModuleResults
                {
                    Status = found ? ResultsStatus.Good : ResultsStatus.Warning,
                    Summary = found ? Metadata.Terms.RobotsTxtFound : Metadata.Terms.RobotsTxtNotFound,
                    Type = ResultsType.NoResults
                };
            }
            catch (Exception ex)
            {
                return new ModuleResults
                {
                    Status = ResultsStatus.Error,
                    Summary = ex.Message,
                    Type = ResultsType.NoResults
                };
            }
        }
    }
}