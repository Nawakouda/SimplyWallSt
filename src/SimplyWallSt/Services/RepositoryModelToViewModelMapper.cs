using SimplyWallSt.Enums;
using SimplyWallSt.Listing.Repository.Company;
using SimplyWallSt.Listing.Repository.CompanyPriceClose;
using SimplyWallSt.Listing.Repository.CompanyScore;
using SimplyWallSt.Models;

namespace SimplyWallSt.Services
{
    public class RepositoryModelToViewModelMapper : IRepositoryModelToViewModelMapper
    {
        public RepositoryModelToViewModelMapper()
        {
        }

        public CompanyViewModel MapRepositoryCompanyToViewCompany(Company databaseModel, IEnumerable<CompanyPriceClose> prices, CompanyScore score)
        {
            return new CompanyViewModel
            {
                Name = databaseModel.Name,
                ListingCurrencyISO = databaseModel.ListingCurrencyISO,
                UniqueSymbol = databaseModel.UniqueSymbol,
                Prices = prices.Select(MapRepositoryPriceToViewPrice),
                TotalScore = score.Total
            };
        }

        public Listing.Repository.Enums.CompanySearchSortbyEnum MapViewCompanySortByEnumToRepositorySortByEnum(CompanySearchSortbyEnum databaseModel)
        {
            return databaseModel switch
            {
                CompanySearchSortbyEnum.Volatility => Listing.Repository.Enums.CompanySearchSortbyEnum.Volatility,
                CompanySearchSortbyEnum.CompanyScore => Listing.Repository.Enums.CompanySearchSortbyEnum.CompanyScore,
                _ => throw new ArgumentOutOfRangeException(nameof(databaseModel)),
            };
        }

        public CompanyPriceCloseViewModel MapRepositoryPriceToViewPrice(CompanyPriceClose price)
        {
            return new CompanyPriceCloseViewModel
            {
                Date = price.Date,
                Price = price.Price
            };
        }
    }
}
