using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimplyWallSt.Listing.Repository.CompanyPriceClose
{
    /// <summary>
    /// Interface for repositories from which we can query company prices
    /// </summary>
    public interface ICompanyPriceCloseRepository
    {
        /// <summary>
        /// Get closing prices by company's id
        /// </summary>
        /// <returns>An enumerable list of closing price objects, which has - as a property - the stock's closing price on the given date</returns>
        Task<IEnumerable<CompanyPriceClose>> GetPricesByCompanyId(Guid companyId);
    }
}
