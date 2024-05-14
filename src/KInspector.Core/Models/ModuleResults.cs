using KInspector.Core.Constants;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KInspector.Core.Models
{
    public class ModuleResults
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ResultsStatus Status { get; set; } = ResultsStatus.NotRun;

        public string? Summary { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ResultsType Type { get; set; }

        public IList<TableResult> TableResults { get; } = new List<TableResult>();

        public IList<string> StringResults { get; } = new List<string>();
    }
}