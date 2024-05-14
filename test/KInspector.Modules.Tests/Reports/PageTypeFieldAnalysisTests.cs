using KInspector.Core.Constants;
using KInspector.Reports.PageTypeFieldAnalysis.Models;
using KInspector.Reports.PageTypeFieldAnalysis;

using NUnit.Framework;

namespace KInspector.Tests.Common.Reports
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class PageTypeFieldAnalysisTests : AbstractModuleTest<Report, Terms>
    {
        private readonly Report mockReport;

        private List<CmsPageTypeField> CmsPageTypeFieldsWithoutIssues => new();

        private List<CmsPageTypeField> CmsPageTypeFieldsWithIdenticalNamesAndDifferentDataTypes => new()
        {
            new()
            {
                PageTypeCodeName = "DancingGoatMvc.Article",
                FieldName = "ArticleText",
                FieldDataType = "varchar"
            },
            new()
            {
                PageTypeCodeName = "DancingGoatMvc.AboutUs",
                FieldName = "ArticleText",
                FieldDataType = "int"
            }
        };

        public PageTypeFieldAnalysisTests(int majorVersion) : base(majorVersion)
        {
            mockReport = new Report(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [TestCase(Category = "Matching fields have save data types", TestName = "Page type fields with matching names and data types produce a good result")]
        public void Should_ReturnGoodResult_When_FieldsHaveNoIssues()
        {
            // Arrange
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsPageTypeField>(Scripts.GetCmsPageTypeFields))
                .Returns(CmsPageTypeFieldsWithoutIssues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Good));
        }

        [TestCase(Category = "Matching fields have different data types", TestName = "Page type fields with matching names and different data types produce an information result")]
        public void Should_ReturnInformationResult_When_FieldsWithMatchingNamesHaveDifferentDataTypes()
        {
            // Arrange
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsPageTypeField>(Scripts.GetCmsPageTypeFields))
                .Returns(CmsPageTypeFieldsWithIdenticalNamesAndDifferentDataTypes);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count(), Is.EqualTo(2));
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Information));
        }
    }
}