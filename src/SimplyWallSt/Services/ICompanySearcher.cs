using SimplyWallSt.Listing.Repository.Company;
using SimplyWallSt.Listing.Repository.CompanyPriceClose;
using SimplyWallSt.Listing.Repository.CompanyScore;

namespace SimplyWallSt.Services
{
    public interface ICompanySearcher
    {
        Task<IEnumerable<(Company company, IEnumerable<CompanyPriceClose> prices, CompanyScore score)>> Search(Enums.CompanySearchSortbyEnum sortBy,
                                                         string exchangeSymbolFilter,
                                                         decimal filterByLowestScore,
                                                         int pageOffset,
                                                         int pageSize);
    }
}
