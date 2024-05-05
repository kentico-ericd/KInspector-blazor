namespace KInspector.Core.Models
{
    public class InstanceDetails
    {
        public Guid Guid { get; set; }

        public Version? AdministrationVersion { get; set; }

        public Version? AdministrationDatabaseVersion { get; set; }

        public string? AdministrationConnectionString { get; set; }

        public IEnumerable<Site>? Sites { get; set; }
    }
}