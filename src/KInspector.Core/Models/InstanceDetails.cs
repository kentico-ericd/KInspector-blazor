namespace KInspector.Core.Models
{
    public class InstanceDetails
    {
        public Guid Guid { get; set; }

        public Version? AdministrationVersion { get; set; }

        public Version? DatabaseVersion { get; set; }

        public IEnumerable<Site>? Sites { get; set; }
    }
}