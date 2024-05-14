using KInspector.Actions.DisableSmtpServers;
using KInspector.Actions.DisableSmtpServers.Models;
using KInspector.Core.Constants;

using Moq;

using Newtonsoft.Json;

using NUnit.Framework;

using Action = KInspector.Actions.DisableSmtpServers.Action;

namespace KInspector.Tests.Common.Actions
{
    [TestFixture(12)]
    [TestFixture(13)]
    public class DisableSmtpServersTests : AbstractActionTest<Action, Terms, Options>
    {
        private readonly Action _mockAction;

        public DisableSmtpServersTests(int majorVersion) : base(majorVersion)
        {
            _mockAction = new Action(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [Test]
        public void Should_NotModifyData_When_OptionsNull()
        {
            // Arrange
            var options = new Options { ServerId = null, SiteId = null };
            var settingsResults = GetCleanSettingsResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SmtpFromSettings>(Scripts.GetSmtpFromSettingsKeys))
                .Returns(settingsResults);

            var smtpResults = GetCleanSmtpResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SmtpFromSmtpServers>(Scripts.GetSmtpFromSmtpServers))
                .Returns(smtpResults);

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = _mockAction.Execute(optionJson);
            var smtpTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockAction.Metadata.Terms.ServersFromSmtpTable) ?? false);
            var settingsTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockAction.Metadata.Terms.ServersFromSettingsTable) ?? false);

            // Assert
            Assert.That(smtpTable, Is.Not.Null);
            Assert.That(settingsTable, Is.Not.Null);
            Assert.That(settingsTable?.Rows.Count() == 3);
            Assert.That(smtpTable?.Rows.Count() == 2);
            Assert.That(results.Status == ResultsStatus.Information);
            _mockDatabaseService.Verify(m => m.ExecuteSqlFromFileGeneric(It.IsAny<string>()), Times.Never());
        }

        [TestCase(1, 1)]
        [TestCase(2, null)]
        [TestCase(5, null)]
        [TestCase(null, 1)]
        [TestCase(null, 5)]
        public void Should_NotModifyData_When_InvalidOptions(int serverId, int siteId)
        {
            // Arrange
            var options = new Options { ServerId = serverId, SiteId = siteId };
            var settingsResults = GetCleanSettingsResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SmtpFromSettings>(Scripts.GetSmtpFromSettingsKeys))
                .Returns(settingsResults);

            var smtpResults = GetCleanSmtpResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SmtpFromSmtpServers>(Scripts.GetSmtpFromSmtpServers))
                .Returns(smtpResults);

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = _mockAction.Execute(optionJson);
            var smtpTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockAction.Metadata.Terms.ServersFromSmtpTable) ?? false);
            var settingsTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockAction.Metadata.Terms.ServersFromSettingsTable) ?? false);

            // Assert
            Assert.That(smtpTable, Is.Not.Null);
            Assert.That(settingsTable, Is.Not.Null);
            Assert.That(settingsTable?.Rows.Count() == 3);
            Assert.That(smtpTable?.Rows.Count() == 2);
            Assert.That(results.Status == ResultsStatus.Error);
            _mockDatabaseService.Verify(m => m.ExecuteSqlFromFileGeneric(It.IsAny<string>()), Times.Never());
        }

        [TestCase(0)]
        [TestCase(2)]
        public void Should_ModifySiteSettings_When_ValidOptions(int siteId)
        {
            // Arrange
            var options = new Options { SiteId = siteId };
            var settingsResults = GetCleanSettingsResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SmtpFromSettings>(Scripts.GetSmtpFromSettingsKeys))
                .Returns(settingsResults);

            var smtpResults = GetCleanSmtpResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SmtpFromSmtpServers>(Scripts.GetSmtpFromSmtpServers))
                .Returns(smtpResults);

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileGeneric(Scripts.DisableSiteSmtpServer, It.IsAny<object>()))
                .Returns(It.IsAny<IEnumerable<Dictionary<string, object>>>());

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = _mockAction.Execute(optionJson);
            var smtpTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockAction.Metadata.Terms.ServersFromSmtpTable) ?? false);
            var settingsTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockAction.Metadata.Terms.ServersFromSettingsTable) ?? false);

            // Assert
            Assert.That(smtpTable, Is.Not.Null);
            Assert.That(settingsTable, Is.Not.Null);
            Assert.That(settingsTable?.Rows.Count() == 3);
            Assert.That(smtpTable?.Rows.Count() == 2);
            Assert.That(results.Status == ResultsStatus.Good);
            _mockDatabaseService.Verify(m => m.ExecuteSqlFromFileGeneric(Scripts.DisableSiteSmtpServer, It.IsAny<object>()), Times.Once());
        }

        [Test]
        public void Should_ModifySmtpServer_When_ValidOptions()
        {
            // Arrange
            var options = new Options { ServerId = 1 };
            var settingsResults = GetCleanSettingsResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SmtpFromSettings>(Scripts.GetSmtpFromSettingsKeys))
                .Returns(settingsResults);

            var smtpResults = GetCleanSmtpResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<SmtpFromSmtpServers>(Scripts.GetSmtpFromSmtpServers))
                .Returns(smtpResults);

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileGeneric(Scripts.DisableSmtpServer, It.IsAny<object>()))
                .Returns(It.IsAny<IEnumerable<Dictionary<string, object>>>());

            // Act
            var optionJson = JsonConvert.SerializeObject(options);
            var results = _mockAction.Execute(optionJson);
            var smtpTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockAction.Metadata.Terms.ServersFromSmtpTable) ?? false);
            var settingsTable = results.TableResults.FirstOrDefault(t => t.Name?.Equals(_mockAction.Metadata.Terms.ServersFromSettingsTable) ?? false);

            // Assert
            Assert.That(smtpTable, Is.Not.Null);
            Assert.That(settingsTable, Is.Not.Null);
            Assert.That(settingsTable?.Rows.Count() == 3);
            Assert.That(smtpTable?.Rows.Count() == 2);
            Assert.That(results.Status == ResultsStatus.Good);
            _mockDatabaseService.Verify(m => m.ExecuteSqlFromFileGeneric(Scripts.DisableSmtpServer, It.IsAny<object>()), Times.Once());
        }

        private List<SmtpFromSmtpServers> GetCleanSmtpResults()
        {
            return new List<SmtpFromSmtpServers>
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

        private List<SmtpFromSettings> GetCleanSettingsResults()
        {
            return new List<SmtpFromSettings>
            {
                new() {
                    SiteID = 0,
                    Server = "localhost"
                },
                new() {
                    SiteID = 1,
                    Server = "mysite.com.disabled"
                },
                new() {
                    SiteID = 2,
                    Server = "othersite.com"
                }
            };
        }
    }
}
