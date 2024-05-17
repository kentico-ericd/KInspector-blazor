using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;

using Newtonsoft.Json;

using System.Text;

namespace KInspector.Infrastructure.Services
{
    public class ConfigService : IConfigService
    {
        private readonly string _saveFileLocation = $"{Directory.GetCurrentDirectory()}\\KInspector.config";

        public async Task<bool> DeleteInstance(Guid? guid)
        {
            var config = await GetConfig();
            var totalRemoved = config.Instances.RemoveAll(i => i.Guid.Equals(guid));
            await SaveConfig(config);

            return totalRemoved > 0;
        }

        public async Task<Instance?> GetInstance(Guid guid)
        {
            var config = await GetConfig();

            return config.Instances.FirstOrDefault(i => i.Guid.Equals(guid));
        }

        public async Task<InspectorConfig> GetConfig()
        {
            var saveFileExists = File.Exists(_saveFileLocation);
            if (saveFileExists)
            {
                var saveFileContents = await ReadTextAsync(_saveFileLocation);
                var config = JsonConvert.DeserializeObject<InspectorConfig>(saveFileContents);

                return config ?? new InspectorConfig();
            }

            var newConfig = new InspectorConfig();
            await SaveConfig(newConfig);

            return newConfig;
        }

        public async Task<Instance?> GetCurrentInstance()
        {
            var config = await GetConfig();

            return config.Instances.FirstOrDefault(i => i.Guid == config.CurrentInstance);
        }

        public async Task<Instance?> SetCurrentInstance(Guid? guid)
        {
            var config = await GetConfig();
            var selectedInstance = config.Instances.FirstOrDefault(i => i.Guid.Equals(guid));
            config.CurrentInstance = guid;
            await SaveConfig(config);

            return selectedInstance;
        }

        public async Task UpsertInstance(Instance instance)
        {
            instance.Guid = instance.Guid == Guid.Empty ? Guid.NewGuid() : instance.Guid;
            var config = await GetConfig();
            var existingSettingsIndex = config.Instances.FindIndex(x => x.Guid.Equals(instance.Guid));
            if (existingSettingsIndex == -1)
            {
                config.Instances.Add(instance);
            }
            else
            {
                config.Instances[existingSettingsIndex] = instance;
            }

            await SaveConfig(config);
        }

        private Task SaveConfig(InspectorConfig config)
        {
            var jsonText = JsonConvert.SerializeObject(config, Formatting.Indented);
            byte[] encodedText = Encoding.UTF8.GetBytes(jsonText);

            using var sourceStream =
                new FileStream(
                    _saveFileLocation,
                    FileMode.Create, FileAccess.Write, FileShare.None,
                    bufferSize: 4096, useAsync: true);

            return sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        }

        private static async Task<string> ReadTextAsync(string filePath)
        {
            using var sourceStream =
                new FileStream(
                    filePath,
                    FileMode.Open, FileAccess.Read, FileShare.Read,
                    bufferSize: 4096, useAsync: true);

            var sb = new StringBuilder();
            byte[] buffer = new byte[0x1000];
            int numRead;
            while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                string text = Encoding.UTF8.GetString(buffer, 0, numRead);
                sb.Append(text);
            }

            return sb.ToString();
        }
    }
}