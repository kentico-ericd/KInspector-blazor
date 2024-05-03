using KInspector.Core.Constants;

using System.Xml;

namespace KInspector.Core.Services.Interfaces
{
    public interface ICmsFileService : IService
    {
        Dictionary<string, string> GetResourceStringsFromResx(string instanceRoot, string relativeResxFilePath = DefaultKenticoPaths.PrimaryResxFile);

        XmlDocument GetXmlDocument(string instanceRoot, string relativeFilePath);
    }
}