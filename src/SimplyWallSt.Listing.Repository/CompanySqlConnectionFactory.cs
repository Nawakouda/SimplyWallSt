using Microsoft.Extensions.Options;
using System;
using System.Data.SQLite;

namespace SimplyWallSt.Listing.Repository
{
    public class CompanySqlConnectionFactory : ICompanySqlConnectionFactory
    {
        IOptions<CompanyRepositoryConfigs> _Configs { get; }

        public CompanySqlConnectionFactory(IOptions<CompanyRepositoryConfigs> configs)
        {
            _Configs = configs ?? throw new ArgumentNullException(nameof(configs));
        }

        public SQLiteConnection GetConnection()
        {
            // The library itself should handle connection pooling
            var connection = new SQLiteConnection(new SQLiteConnectionStringBuilder
            {
                DataSource = _Configs.Value.SqliteDatabasePath,
                BinaryGUID = false,
                Version = _Configs.Value.SqliteDatabaseVersion
            }.ToString());

            return connection;
        }
    }
}
