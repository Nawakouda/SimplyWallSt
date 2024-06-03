using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace SimplyWallSt.Listing.Repository.CompanyPriceClose
{
    public class CachedCompanyPriceCloseRepository : ICompanyPriceCloseRepository
    {
        ICompanyPriceCloseRepository _UnderlyingCompanyPriceCloseRepository { get; }
        ObjectCache _Cache { get; }
        CacheItemPolicy _CacheItemPolicy { get; }

        public CachedCompanyPriceCloseRepository(ICompanyPriceCloseRepository underlyingCompanyPriceCloseRepository, IOptions<CompanyRepositoryConfigs> configs)
        {
            _UnderlyingCompanyPriceCloseRepository = underlyingCompanyPriceCloseRepository ?? throw new ArgumentNullException(nameof(underlyingCompanyPriceCloseRepository));
            _Cache = MemoryCache.Default;
            _CacheItemPolicy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromSeconds(configs.Value.SlidingCacheExpirationTimeInSeconds)
            };
        }

        public async Task<IEnumerable<CompanyPriceClose>> GetPricesByCompanyId(Guid companyId)
        {
            var key = GetKey(companyId);
            var value = _Cache[key];

            if (value != default)
            {
                return value as IEnumerable<CompanyPriceClose>;
            }

            var fetchedValue = await _UnderlyingCompanyPriceCloseRepository.GetPricesByCompanyId(companyId);
            _Cache.Set(key, fetchedValue, _CacheItemPolicy);
            return fetchedValue;
        }

        private string GetKey(Guid companyId)
        {
            return nameof(GetPricesByCompanyId) + companyId.ToString();
        }
    }
}
