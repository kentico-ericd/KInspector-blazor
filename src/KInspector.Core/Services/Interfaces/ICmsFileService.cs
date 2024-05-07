using KInspector.Core.Constants;

using System.Xml;

namespace KInspector.Core.Services.Interfaces
{
    public interface ICmsFileService : IService
    {
        string? GetFaviconPath(string instanceRoot, string relativeFaviconPath = DefaultKenticoPaths.Favicon);

        Dictionary<string, string> GetResourceStringsFromResx(string? instanceRoot, string relativeResxFilePath = DefaultKenticoPaths.PrimaryResxFile);

        string? GetCMSConnectionString(string? instanceRoot, string relativeWebConfigFilePath = DefaultKenticoPaths.WebConfigFile);

        XmlDocument? GetXmlDocument(string? instanceRoot, string relativeFilePath);
    }
}