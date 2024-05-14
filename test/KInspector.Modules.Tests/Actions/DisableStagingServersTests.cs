using KInspector.Actions.DisableStagingServers;
using KInspector.Actions.DisableStagingServers.Models;
using KInspector.Core.Constants;

using Moq;

using Newtonsoft.Json;

using NUnit.Framework;

using Action = KInspector.Actions.DisableStagingServers.Action;

namespace KInspector.Tests.Common.Actions
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class DisableStagingServersTests : AbstractActionTest<Action, Terms, Options>
    {
        private readonly Action _mockAction;

        public DisableStagingServersTests(int majorVersion) : base(majorVersion)
        {
            _mockAction = new Action(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [Test]
        public void Should_NotModifyData_When_OptionsNull()
        {
            // Arrange
            var options = new Options { ServerId = null };
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<StagingServer>(Scripts.GetStagingServerSummary))
                .Returns(tableResults);

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = _mockAction.Execute(optionJson);

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count() == 2);
            Assert.That(results.Status == ResultsStatus.Information);
            _mockDatabaseService.Verify(m => m.ExecuteSqlFromFileGeneric(It.IsAny<string>()), Times.Never());
        }

        [TestCase(0)]
        [TestCase(2)]
        [TestCase(3)]
        public void Should_NotModifyData_When_InvalidOptions(int serverId)
        {
            // Arrange
            var options = new Options { ServerId = serverId };
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<StagingServer>(Scripts.GetStagingServerSummary))
                .Returns(tableResults);

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = _mockAction.Execute(optionJson);

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count() == 2);
            Assert.That(results.Status == ResultsStatus.Error);
            _mockDatabaseService.Verify(m => m.ExecuteSqlFromFileGeneric(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Should_ModifyData_When_ValidOptions()
        {
            // Arrange
            var options = new Options { ServerId = 1 };
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<StagingServer>(Scripts.GetStagingServerSummary))
                .Returns(tableResults);

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileGeneric(Scripts.DisableServer, It.IsAny<object>()))
                .Returns(It.IsAny<IEnumerable<Dictionary<string, object>>>());

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = _mockAction.Execute(optionJson);

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count() == 2);
            Assert.That(results.Status == ResultsStatus.Good);
            _mockDatabaseService.Verify(m => m.ExecuteSqlFromFileGeneric(It.IsAny<string>(), It.IsAny<object>()), Times.Once());
        }

        private List<StagingServer> GetCleanTableResults()
        {
            return new List<StagingServer>
            {
                new() {
                    ID = 1,
                    Enabled = true
                },
                new() {
                    ID = 2,
                    Enabled = false
                }
            };
        }
    }
}
