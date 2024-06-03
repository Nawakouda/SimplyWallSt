using SimplyWallSt.Enums;
using SimplyWallSt.Listing.Repository.Company;
using SimplyWallSt.Listing.Repository.CompanyPriceClose;
using SimplyWallSt.Listing.Repository.CompanyScore;
using SimplyWallSt.Models;

namespace SimplyWallSt.Services
{
    public interface IRepositoryModelToViewModelMapper
    {
        CompanyViewModel MapRepositoryCompanyToViewCompany(Company databaseModel, IEnumerable<CompanyPriceClose> prices, CompanyScore score);
        Listing.Repository.Enums.CompanySearchSortbyEnum MapViewCompanySortByEnumToRepositorySortByEnum(CompanySearchSortbyEnum databaseModel);
    }
}