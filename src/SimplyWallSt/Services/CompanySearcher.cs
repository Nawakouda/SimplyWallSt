using SimplyWallSt.Listing.Repository.Company;
using SimplyWallSt.Listing.Repository.CompanyPriceClose;
using SimplyWallSt.Listing.Repository.CompanyScore;

namespace SimplyWallSt.Services
{
    public class CompanySearcher : ICompanySearcher
    {
        IRepositoryModelToViewModelMapper _RepositoryModelToViewModelMapper { get; }
        ICompanyRepository _CompanyRepository { get; }
        ICompanyPriceCloseRepository _CompanyPriceCloseRepository { get; }
        ICompanyScoreRepository _CompanyScoreRepository { get; }

        public CompanySearcher(IRepositoryModelToViewModelMapper repositoryModelToViewModelMapper,
                               ICompanyRepository companyRepository,
                               ICompanyPriceCloseRepository companyPriceCloseRepository,
                               ICompanyScoreRepository companyScoreRepository)
        {
            _RepositoryModelToViewModelMapper = repositoryModelToViewModelMapper ?? throw new ArgumentNullException(nameof(repositoryModelToViewModelMapper));
            _CompanyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _CompanyPriceCloseRepository = companyPriceCloseRepository ?? throw new ArgumentNullException(nameof(companyPriceCloseRepository));
            _CompanyScoreRepository = companyScoreRepository ?? throw new ArgumentNullException(nameof(companyScoreRepository));
        }

        public async Task<IEnumerable<(Company company, IEnumerable<CompanyPriceClose> prices, CompanyScore score)>> Search(
                                                         Enums.CompanySearchSortbyEnum sortBy,
                                                         string exchangeSymbolFilter,
                                                         decimal filterByLowestScore,
                                                         int pageOffset,
                                                         int pageSize)
        {
            var searchResults = await _CompanyRepository.Search(
                _RepositoryModelToViewModelMapper.MapViewCompanySortByEnumToRepositorySortByEnum(sortBy),
                exchangeSymbolFilter,
                filterByLowestScore,
                pageOffset,
                pageSize);

            var resultsWithPrice = new List<(Company, IEnumerable<CompanyPriceClose>, CompanyScore)>();

            foreach (var searchResult in searchResults)
            {
                var prices = await _CompanyPriceCloseRepository.GetPricesByCompanyId(searchResult.ID);
                var companyScore = await _CompanyScoreRepository.GetByScoreId(searchResult.ScoreId);

                resultsWithPrice.Add((searchResult, prices, companyScore));
            }

            return resultsWithPrice;
        }
    }
}
