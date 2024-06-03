using SimplyWallSt.Listing.Repository.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimplyWallSt.Listing.Repository.Company
{
    /// <summary>
    /// Interface for repositories from which we can query company data
    /// </summary>
    public interface ICompanyRepository
    {
        /// <summary>
        /// Get company by its unique symbol
        /// </summary>
        /// <param name="uniqueSymbol">The company's unique symbol</param>
        /// <returns>The company, mapped</returns>
        Task<Company> GetByUniqueSymbol(string uniqueSymbol);

        /// <summary>
        /// Search companies with given criteria. Can filter by score and exchangeSymbol. Can sort by volatility or score (default volatility)
        /// </summary>
        /// <param name="sortBy">Which value to sort by. By default, we sort by volatility</param>
        /// <param name="exchangeSymbolFilter">only include results with provided exchange symbol. If we do not want to filter by this, leave it as default value</param>
        /// <param name="lowestScore">only include companies with score higher than this value</param>
        /// <param name="pageStartingOffset">offset for pagination purposes</param>
        /// <param name="pageSize">size of page for pagination purposes</param>
        /// <returns>a list of Company search results</returns>
        Task<IEnumerable<Company>> Search(
            CompanySearchSortbyEnum sortBy = CompanySearchSortbyEnum.Volatility,
            string exchangeSymbolFilter = default,
            decimal lowestScore = 0,
            int pageStartingOffset = 0,
            int pageSize = 0);
    }
}
