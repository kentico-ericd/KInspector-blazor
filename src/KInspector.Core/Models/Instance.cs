namespace KInspector.Core.Models
{
    public class Instance
    {
        public DatabaseSettings? DatabaseSettings { get; set; }

        public Guid? Guid { get; set; }

        public string? Name { get; set; }

        public string? AdministrationPath { get; set; }

        public string? AdministrationUrl { get; set; }
    }
}