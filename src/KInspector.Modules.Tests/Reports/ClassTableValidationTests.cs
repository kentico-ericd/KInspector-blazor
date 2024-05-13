using KInspector.Core.Constants;
using KInspector.Reports.ClassTableValidation;
using KInspector.Reports.ClassTableValidation.Models;

using NUnit.Framework;

namespace KInspector.Tests.Common.Reports
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class ClassTableValidationTests : AbstractModuleTest<Report, Terms>
    {
        private readonly Report _mockReport;

        public ClassTableValidationTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockInstanceService.Object, _mockModuleMetadataService.Object, _mockConfigService.Object);
        }

        [Test]
        public void Should_ReturnCleanResult_When_DatabaseIsClean()
        {
            // Arrange
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<TableWithNoClass>(Scripts.TablesWithNoClass))
                .Returns(tableResults);

            var classResults = GetCleanClassResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ClassWithNoTable>(Scripts.ClassesWithNoTable))
                .Returns(classResults);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(!results.TableResults.Any());
            Assert.That(results.Status == ResultsStatus.Good);
        }

        [Test]
        public void Should_ReturnErrorResult_When_DatabaseHasClassWithNoTable()
        {
            // Arrange
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<TableWithNoClass>(Scripts.TablesWithNoClass))
                .Returns(tableResults);

            var classResults = GetCleanClassResults();
            classResults.Add(new ClassWithNoTable
            {
                ClassDisplayName = "Has no table",
                ClassName = "HasNoTable",
                ClassTableName = "Custom_HasNoTable"
            });

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ClassWithNoTable>(Scripts.ClassesWithNoTable))
                .Returns(classResults);

            // Act
            var results = _mockReport.GetResults();
            var tableResultsTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockReport.Metadata.Terms.DatabaseTablesWithMissingKenticoClasses) ?? false);
            var classResultsTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockReport.Metadata.Terms.KenticoClassesWithMissingDatabaseTables) ?? false);

            // Assert
            Assert.That(tableResultsTable, Is.Not.Null);
            Assert.That(classResultsTable, Is.Not.Null);
            Assert.That(tableResultsTable?.Rows.Count() == 0);
            Assert.That(classResultsTable?.Rows.Count() == 1);
            Assert.That(results.Status == ResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnErrorResult_When_DatabaseHasTableWithNoClass()
        {
            // Arrange
            var tableResults = GetCleanTableResults(false);
            tableResults.Add(new TableWithNoClass
            {
                TableName = "HasNoClass"
            });

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<TableWithNoClass>(Scripts.TablesWithNoClass))
                .Returns(tableResults);

            var classResults = GetCleanClassResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<ClassWithNoTable>(Scripts.ClassesWithNoTable))
                .Returns(classResults);

            // Act
            var results = _mockReport.GetResults();
            var tableResultsTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockReport.Metadata.Terms.DatabaseTablesWithMissingKenticoClasses) ?? false);
            var classResultsTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockReport.Metadata.Terms.KenticoClassesWithMissingDatabaseTables) ?? false);

            // Assert
            Assert.That(tableResultsTable, Is.Not.Null);
            Assert.That(classResultsTable, Is.Not.Null);
            Assert.That(tableResultsTable?.Rows.Count() == 1);
            Assert.That(classResultsTable?.Rows.Count() == 0);
            Assert.That(results.Status == ResultsStatus.Error);
        }

        private List<ClassWithNoTable> GetCleanClassResults()
        {
            return new List<ClassWithNoTable>();
        }

        private List<TableWithNoClass> GetCleanTableResults(bool includeWhitelistedTables = true)
        {
            var tableResults = new List<TableWithNoClass>();
            if (includeWhitelistedTables && _mockInstanceDetails?.AdministrationDatabaseVersion?.Major >= 10)
            {
                tableResults.Add(new TableWithNoClass() { TableName = "CI_Migration" });
            }

            return tableResults;
        }
    }
}