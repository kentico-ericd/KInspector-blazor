using KInspector.Core.Models;

namespace KInspector.Tests.Common.Helpers
{
    public static class MockInstanceDetails
    {
        public static InstanceDetails Kentico9 = new()
        {
            AdministrationVersion = new Version("9.0"),
            AdministrationDatabaseVersion = new Version("9.0"),
            Sites = new List<Site>
            {
                new() { DomainName = "kentico9.com" }
            }
        };

        public static InstanceDetails Kentico10 = new()
        {
            AdministrationVersion = new Version("10.0"),
            AdministrationDatabaseVersion = new Version("10.0"),
            Sites = new List<Site>
            {
                new() { DomainName = "kentico10.com" }
            }
        };

        public static InstanceDetails Kentico11 = new()
        {
            AdministrationVersion = new Version("11.0"),
            AdministrationDatabaseVersion = new Version("11.0"),
            Sites = new List<Site>
            {
                new() { DomainName = "kentico11.com" }
            }
        };

        public static InstanceDetails Kentico12 = new()
        {
            AdministrationVersion = new Version("12.0"),
            AdministrationDatabaseVersion = new Version("12.0"),
            Sites = new List<Site>
            {
                new() { DomainName = "kentico12.com" }
            }
        };

        public static InstanceDetails Kentico13 = new()
        {
            AdministrationVersion = new Version("13.0"),
            AdministrationDatabaseVersion = new Version("13.0"),
            Sites = new List<Site>
            {
                new() { DomainName = "kentico13.com" }
            }
        };

        public static InstanceDetails Get(int majorVersion, Instance instance)
        {
            InstanceDetails? instanceDetails = null;
            switch (majorVersion)
            {
                case 9:
                    instanceDetails = Kentico9;
                    break;
                case 10:
                    instanceDetails = Kentico10;
                    break;
                case 11:
                    instanceDetails = Kentico11;
                    break;
                case 12:
                    instanceDetails = Kentico12;
                    break;
                case 13:
                    instanceDetails = Kentico13;
                    break;
            }

            if (instanceDetails == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                instanceDetails.Guid = instance.Guid ?? Guid.Empty;
            }

            return instanceDetails;
        }
    }
}