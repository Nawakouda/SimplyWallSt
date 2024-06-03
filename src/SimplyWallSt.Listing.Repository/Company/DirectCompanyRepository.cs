using System.Data.SQLite;
using Microsoft.Extensions.Logging;
using SimplyWallSt.Listing.Repository.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;

namespace SimplyWallSt.Listing.Repository.Company
{
    public class DirectCompanyRepository : ICompanyRepository
    {
        ICompanySqlConnectionFactory _CompanySqlConnectionFactory { get; }
        ILogger<DirectCompanyRepository> _Logger { get; }

        public DirectCompanyRepository(ICompanySqlConnectionFactory companySqlConnectionFactory, ILogger<DirectCompanyRepository> logger) {
            _CompanySqlConnectionFactory = companySqlConnectionFactory;
            _Logger = logger;
        }

        public async Task<Company> GetByUniqueSymbol(string uniqueSymbol)
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
                                name,
                                ticker_symbol,
                                exchange_symbol,
                                unique_symbol,
                                date_generated,
                                security_name,
                                exchange_country_iso,
                                listing_currency_iso,
                                canonical_url,
                                unique_symbol_slug,
                                score_id
                            FROM swsCompany
                            WHERE
                                uniqueSymbol = @uniqueSymbol";
                    command.Parameters.AddWithValue("@uniqueSymbol", uniqueSymbol);
                    var reader = await command.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        return MapCompany(reader);
                    }

                    return default;
                }
            }
        }

        public Task<IEnumerable<Company>> Search(CompanySearchSortbyEnum sortBy, string exchangeSymbolFilter = default, decimal lowestScore = 0, int pageStartingOffset = 0, int pageSize = 0)
        {
            switch (sortBy)
            {
                case CompanySearchSortbyEnum.CompanyScore:
                    return SearchSortedByScore(exchangeSymbolFilter, lowestScore, pageStartingOffset, pageSize);
                case CompanySearchSortbyEnum.Volatility:
                    return SearchSortedByVolatility(exchangeSymbolFilter, lowestScore, pageStartingOffset, pageSize);
                default:
                    _Logger.LogWarning("Search initiated without providing sortBy criteria. Defaulting to searching by volatility");
                    return SearchSortedByVolatility(exchangeSymbolFilter, lowestScore, pageStartingOffset, pageSize);
            }
        }

        private async Task<IEnumerable<Company>> SearchSortedByVolatility(string exchangeSymbolFilter, decimal lowestScore, int pageStartingOffset, int pageSize)
        {
            using (var connection = _CompanySqlConnectionFactory.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        CREATE TEMP TABLE CompanyLast90Prices AS
                            SELECT
                                company_id,
                                price
                            FROM swsCompanyPriceClose
                            ORDER BY [date] DESC LIMIT 90;
                        CREATE TEMP TABLE CompanyAverages AS
                            SELECT
                                company_id,
                                AVG(price) AS avg_price
                            FROM CompanyLast90Prices
                            GROUP BY company_id;
                        CREATE TEMP TABLE CompanyVolatility AS 
                            SELECT
                                a.company_id,
                                ABS(SUM(p.price - a.avg_price)/a.avg_price) AS variance
                            FROM
                                CompanyAverages a
                                LEFT JOIN swsCompanyPriceClose p ON a.company_id = p.company_id
                            GROUP BY a.company_id, a.avg_price;

                        SELECT
                            c.id,
                            name,
                            ticker_symbol,
                            exchange_symbol,
                            unique_symbol,
                            c.date_generated,
                            security_name,
                            exchange_country_iso,
                            listing_currency_iso,
                            canonical_url,
                            unique_symbol_slug,
                            score_id
                        FROM
                            swsCompany c
                            LEFT JOIN CompanyVolatility v ON c.id = v.company_id
                            LEFT JOIN swsCompanyScore s ON c.score_id = s.id
                        WHERE
                            s.total >= @lowestScore
                            AND (@exchangeSymbolFilter = '' OR c.exchange_symbol = @exchangeSymbolFilter)
                        ORDER BY v.variance ASC
                        LIMIT @pageSize OFFSET @pageIndex;";
                    command.Parameters.AddWithValue("@lowestScore", lowestScore);
                    command.Parameters.AddWithValue("@exchangeSymbolFilter", exchangeSymbolFilter ?? "");
                    command.Parameters.AddWithValue("@pageIndex", pageStartingOffset);
                    command.Parameters.AddWithValue("@pageSize", pageSize);
                    var reader = await command.ExecuteReaderAsync();
                    var results = new List<Company>();
                    while (reader.Read())
                    {
                        results.Add(MapCompany(reader));
                    }

                    return results;
                }
            }
        }

        private async Task<IEnumerable<Company>> SearchSortedByScore(string exchangeSymbolFilter, decimal lowestScore, int pageStartingOffset, int pageSize)
        {
            using (var connection = _CompanySqlConnectionFactory.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT
                            c.id,
                            name,
                            ticker_symbol,
                            exchange_symbol,
                            unique_symbol,
                            c.date_generated,
                            security_name,
                            exchange_country_iso,
                            listing_currency_iso,
                            canonical_url,
                            unique_symbol_slug,
                            score_id
                        FROM
                            swsCompany c
                            LEFT JOIN swsCompanyScore s ON c.score_id = s.id
                        WHERE s.total >= @lowestScore AND (@exchangeSymbolFilter = '' OR c.exchange_symbol = @exchangeSymbolFilter)
                        ORDER BY s.total DESC
                        LIMIT @pageSize OFFSET @pageIndex;";
                    command.Parameters.AddWithValue("@lowestScore", lowestScore);
                    command.Parameters.AddWithValue("@exchangeSymbolFilter", exchangeSymbolFilter ?? "");
                    command.Parameters.AddWithValue("@pageIndex", pageStartingOffset);
                    command.Parameters.AddWithValue("@pageSize", pageSize);
                    var reader = await command.ExecuteReaderAsync();
                    var results = new List<Company>();
                    while (reader.Read())
                    {
                        results.Add(MapCompany(reader));
                    }

                    return results;
                }
            }
        }

        private Company MapCompany(DbDataReader reader)
        {
            return new Company
            {
                CanonicalURL = Convert.ToString(reader["canonical_url"]),
                ExchangeCountryISO = Convert.ToString(reader["exchange_country_iso"]),
                ExchangeSymbol = Convert.ToString(reader["exchange_symbol"]),
                ID = Guid.Parse(Convert.ToString(reader["id"])),
                ListingCurrencyISO = Convert.ToString(reader["listing_currency_iso"]),
                Name = Convert.ToString(reader["name"]),
                ScoreId = Convert.ToInt32(reader["score_id"]),
                TickerSymbol = Convert.ToString(reader["ticker_symbol"]),
                UniqueSymbol = Convert.ToString(reader["unique_symbol"]),
                UniqueSymbolSlug = Convert.ToString(reader["unique_symbol_slug"]),
                DateGenerated = DateTime.SpecifyKind(Convert.ToDateTime(reader["date_generated"]), DateTimeKind.Utc) // Assuming the sqlite database is providing utc dates
            };
        }
    }
}
