using Microsoft.Extensions.Options;
using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace SimplyWallSt.Listing.Repository.CompanyScore
{
    public class CachedCompanyScoreRepository : ICompanyScoreRepository
    {
        ICompanyScoreRepository _UnderlyingCompanyScoreRepository { get; }
        ObjectCache _Cache { get; }
        CacheItemPolicy _CacheItemPolicy { get; }

        public CachedCompanyScoreRepository(ICompanyScoreRepository underlyingCompanyScoreRepository, IOptions<CompanyRepositoryConfigs> configs)
        {
            _UnderlyingCompanyScoreRepository = underlyingCompanyScoreRepository ?? throw new ArgumentNullException(nameof(underlyingCompanyScoreRepository));
            _Cache = MemoryCache.Default;
            _CacheItemPolicy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromSeconds(configs.Value.SlidingCacheExpirationTimeInSeconds)
            };
        }

        public async Task<CompanyScore> GetByScoreId(int scoreId)
        {
            var key = GetKey(scoreId);
            var value = _Cache[key];

            if (value != default)
            {
                return value as CompanyScore;
            }

            var fetchedValue = await _UnderlyingCompanyScoreRepository.GetByScoreId(scoreId);
            _Cache.Set(key, fetchedValue, _CacheItemPolicy);
            return fetchedValue;
        }

        private string GetKey(int scoreId)
        {
            return nameof(GetByScoreId) + scoreId;
        }
    }
}
