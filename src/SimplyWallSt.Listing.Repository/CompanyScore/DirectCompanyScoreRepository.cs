using System.Data.SQLite;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Data.Common;

namespace SimplyWallSt.Listing.Repository.CompanyScore
{
    public class DirectCompanyScoreRepository : ICompanyScoreRepository
    {
        ICompanySqlConnectionFactory _CompanySqlConnectionFactory { get; }
        ILogger<DirectCompanyScoreRepository> _Logger { get; }

        public DirectCompanyScoreRepository(ICompanySqlConnectionFactory companySqlConnectionFactory, ILogger<DirectCompanyScoreRepository> logger)
        {
            _CompanySqlConnectionFactory = companySqlConnectionFactory;
            _Logger = logger;
        }

        public async Task<CompanyScore> GetByScoreId(int scoreId)
        {
            var connection = _CompanySqlConnectionFactory.GetConnection();
            using (connection)
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    // Ideally, this should be implemented as stored procedures/prepared statements created on the DB to be accessed by stored procedure name.
                    command.CommandText = @"
                            SELECT
                                id,
                                company_id,
                                dividend,
                                future,
                                health,
                                management,
                                past,
                                value,
                                misc,
                                total,
                                sentence,
                                date_generated
                            FROM swsCompanyScore
                            WHERE
                                id = @score_id";
                    command.Parameters.AddWithValue("@score_id", scoreId.ToString());
                    var reader = await command.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        return MapCompany(reader);
                    }

                    return default;
                }
            }
        }

        private CompanyScore MapCompany(DbDataReader reader)
        {
            return new CompanyScore
            {
                Id = Convert.ToInt32(reader["id"]),
                CompanyId = Guid.Parse(Convert.ToString(reader["company_id"])),
                Dividend = Convert.ToInt32(reader["dividend"]),
                Future = Convert.ToInt32(reader["future"]),
                Health = Convert.ToInt32(reader["health"]),
                Management = Convert.ToInt32(reader["management"]),
                Past = Convert.ToInt32(reader["past"]),
                Value = Convert.ToInt32(reader["value"]),
                Misc = Convert.ToInt32(reader["misc"]),
                Total = Convert.ToInt32(reader["total"]),
                Sentence = Convert.ToString(reader["sentence"]),
                DateGenerated = DateTime.SpecifyKind(Convert.ToDateTime(reader["date_generated"]), DateTimeKind.Utc) // Assuming the sqlite database is providing utc dates
            };
        }
    }
}
