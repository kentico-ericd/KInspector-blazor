namespace KInspector.Core.Models
{
    public class InspectorConfig
    {
        public Guid? CurrentInstance { get; set; }

        public List<Instance> Instances { get; set; } = new List<Instance>();
    }
}
