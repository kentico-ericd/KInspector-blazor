using KInspector.Actions.StopRunningSites;
using KInspector.Actions.StopRunningSites.Models;
using KInspector.Core.Constants;

using Moq;

using Newtonsoft.Json;

using NUnit.Framework;

using Action = KInspector.Actions.StopRunningSites.Action;

namespace KInspector.Tests.Common.Actions
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class StopRunningSitesTests : AbstractActionTest<Action, Terms, Options>
    {
        private readonly Action _mockAction;

        public StopRunningSitesTests(int majorVersion) : base(majorVersion)
        {
            _mockAction = new Action(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [Test]
        public void Should_NotModifyData_When_OptionsNull()
        {
            // Arrange
            var options = new Options { SiteId = null };
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsSite>(Scripts.GetSiteSummary))
                .Returns(tableResults);

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = _mockAction.Execute(optionJson);

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count() == 2);
            Assert.That(results.Status == ResultsStatus.Information);
            _mockDatabaseService.Verify(m => m.ExecuteSqlFromFileGeneric(Scripts.StopSite, It.IsAny<object>()), Times.Never());
        }

        [TestCase(0)]
        [TestCase(3)]
        public void Should_NotModifyData_When_InvalidOptions(int siteId)
        {
            // Arrange
            var options = new Options { SiteId = siteId };
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsSite>(Scripts.GetSiteSummary))
                .Returns(tableResults);

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = _mockAction.Execute(optionJson);

            // Assert
            Assert.That(results.Status == ResultsStatus.Error);
            _mockDatabaseService.Verify(m => m.ExecuteSqlFromFileGeneric(Scripts.StopSite, It.IsAny<object>()), Times.Never());
        }

        [Test]
        public void Should_ModifyData_When_ValidOptions()
        {
            // Arrange
            var options = new Options { SiteId = 1 };
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsSite>(Scripts.GetSiteSummary))
                .Returns(tableResults);

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileGeneric(Scripts.StopSite, It.IsAny<object>()))
                .Returns(It.IsAny<IEnumerable<Dictionary<string, object>>>());

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = _mockAction.Execute(optionJson);

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count() == 2);
            Assert.That(results.Status == ResultsStatus.Good);
            _mockDatabaseService.Verify(m => m.ExecuteSqlFromFileGeneric(Scripts.StopSite, It.IsAny<object>()), Times.Once());
        }

        private List<CmsSite> GetCleanTableResults()
        {
            return new List<CmsSite>
            {
                new() {
                    ID = 1,
                    Running = true
                },
                new() {
                    ID = 2,
                    Running = false
                }
            };
        }
    }
}
