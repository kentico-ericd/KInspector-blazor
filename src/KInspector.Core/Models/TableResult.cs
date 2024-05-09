namespace KInspector.Core.Models
{
    public class TableResult
    {
        public string? Name { get; set; }

        public IEnumerable<object> Rows { get; set; } = Enumerable.Empty<object>();
    }
}