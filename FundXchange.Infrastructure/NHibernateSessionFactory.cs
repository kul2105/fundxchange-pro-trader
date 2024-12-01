using System;
using NHibernate;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;
using NHibernate.Cache;
using NHibernate.Tool.hbm2ddl;

namespace FundXchange.Infrastructure
{
    internal class NHibernateSessionFactory
    {
        internal static ISessionFactory CreateSessionFactory(Assembly assembly)
        {
            return Fluently.Configure()
              .Database(MsSqlConfiguration.MsSql2005
              .ConnectionString(c => c
                  .FromAppSetting("connectionString"))
                .Cache(c => c
                  .UseQueryCache()
                  .ProviderClass<HashtableCacheProvider>())
                .ShowSql())
              .Mappings(m => m
                .FluentMappings.AddFromAssembly(assembly))
                .ExposeConfiguration(BuildSchema)
              .BuildSessionFactory();
        }

        internal static void BuildSchema(Configuration config)
        {
            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            string createDB = System.Configuration.ConfigurationManager.AppSettings["CreateDatabase"];
            bool createDatabase = (createDB == "1");

            new SchemaExport(config).Create(false, createDatabase);
        }
    }
}
