namespace SimplyWallSt.Listing.Repository
{
    public class CompanyRepositoryConfigs
    {
        public const string ConfigName = "CompanyRepository";

        public long SlidingCacheExpirationTimeInSeconds { get; set; }
        public long AbsoluteCacheExpirationTimeInSeconds { get; set; }
        public string SqliteDatabasePath { get; set; }
        public int SqliteDatabaseVersion { get; set; }
        public int MaximumPricePageSize{ get; set; }
    }
}
