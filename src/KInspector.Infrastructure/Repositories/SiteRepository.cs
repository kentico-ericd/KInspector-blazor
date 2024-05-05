﻿using Dapper;

using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Repositories.Interfaces;

namespace KInspector.Infrastructure.Repositories
{
    public class SiteRepository : ISiteRepository
    {
        public Site GetSite(Instance instance, int siteId)
        {
            throw new NotImplementedException();
        }

        public IList<Site> GetSites(DatabaseSettings? databaseSettings, string? connectionString)
        {
            try
            {
                var query = @"
                    SELECT
                        SiteId as Id,
                        SiteName as Name,
                        SiteGUID as Guid,
                        SiteDomainName as DomainName,
                        SitePresentationURL as PresentationUrl
                    FROM CMS_Site";
                var connection = DatabaseHelper.GetSqlConnection(databaseSettings, connectionString);
                var sites = connection.Query<Site>(query).ToList();

                return sites;
            }
            catch
            {
                return new Site[] { };
            }
        }
    }
}