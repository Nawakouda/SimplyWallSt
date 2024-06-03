using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimplyWallSt.Models;
using SimplyWallSt.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimplyWallSt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        ICompanySearcher _CompanySearcher { get; }
        IRepositoryModelToViewModelMapper _RepositoryModelToViewModelMapper { get; }
        ILogger<CompaniesController> _Logger { get; }
        IOptions<CompanyApiConfigs> _Configs { get; }

        public CompaniesController(ICompanySearcher companySearcher, IRepositoryModelToViewModelMapper repositoryModelToViewModelMapper, ILogger<CompaniesController> logger, IOptions<CompanyApiConfigs> configs)
        {
            _CompanySearcher = companySearcher ?? throw new ArgumentNullException(nameof(companySearcher));
            _RepositoryModelToViewModelMapper = repositoryModelToViewModelMapper ?? throw new ArgumentNullException(nameof(repositoryModelToViewModelMapper));
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _Configs = configs ?? throw new ArgumentNullException(nameof(configs));
        }

        [HttpGet("/api/companies")]
        public async Task<ActionResult<IEnumerable<CompanyViewModel>>> Search(Enums.CompanySearchSortbyEnum sortBy = Enums.CompanySearchSortbyEnum.Volatility,
                                                                string exchangeSymbolFilter = "",
                                                                decimal filterByLowestScore = default,
                                                                int pageOffset = default,
                                                                int pageSize = default)
        {
            if (!Enum.IsDefined(sortBy))
            {
                return BadRequest();
            }

            if (pageSize == default)
            {
                pageSize = _Configs.Value.DefaultPageSize;
            }

            var searchResults = await _CompanySearcher.Search(sortBy, exchangeSymbolFilter, filterByLowestScore, pageOffset, pageSize);

            return Ok(searchResults.Select((result) => _RepositoryModelToViewModelMapper.MapRepositoryCompanyToViewCompany(result.company, result.prices, result.score)));
        }
    }
}
