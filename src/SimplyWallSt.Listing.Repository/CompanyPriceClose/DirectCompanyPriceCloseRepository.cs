using System.Data.SQLite;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Data.Common;
using System.Collections.Generic;

namespace SimplyWallSt.Listing.Repository.CompanyPriceClose
{
    public class DirectCompanyPriceCloseRepository : ICompanyPriceCloseRepository
    {
        ICompanySqlConnectionFactory _CompanySqlConnectionFactory { get; }
        ILogger<DirectCompanyPriceCloseRepository> _Logger { get; }

        public DirectCompanyPriceCloseRepository(ICompanySqlConnectionFactory companySqlConnectionFactory, ILogger<DirectCompanyPriceCloseRepository> logger)
        {
            _CompanySqlConnectionFactory = companySqlConnectionFactory;
            _Logger = logger;
        }

        public async Task<IEnumerable<CompanyPriceClose>> GetPricesByCompanyId(Guid companyId)
        {
            var connection = _CompanySqlConnectionFactory.GetConnection();
            using (connection)
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    // Ideally, this should be implemented as stored procedures/prepared statements created on the DB. Accessed by stored procedure name.
                    command.CommandText =
                        @"
                            SELECT
                                date,
                                company_id,
                                price,
                                date_created
                            FROM swsCompanyPriceClose
                            WHERE company_id = @companyId
                            ORDER BY date DESC";
                    command.Parameters.AddWithValue("@companyId", companyId.ToString().ToUpperInvariant());
                    var reader = await command.ExecuteReaderAsync();

                    var prices = new List<CompanyPriceClose>();
                    while (await reader.ReadAsync())
                    {
                        prices.Add(MapCompanyPriceClose(reader));
                    }

                    return prices;
                }
            }
        }

        private CompanyPriceClose MapCompanyPriceClose(DbDataReader reader)
        {
            return new CompanyPriceClose
            {
                CompanyId = Guid.Parse(Convert.ToString(reader["company_id"])),
                Date = DateTime.SpecifyKind(Convert.ToDateTime(reader["date"]), DateTimeKind.Utc),
                Price = Convert.ToDecimal(reader["price"]),
                DateCreated = DateTime.SpecifyKind(Convert.ToDateTime(reader["date_created"]), DateTimeKind.Utc)
            };
        }
    }
}
