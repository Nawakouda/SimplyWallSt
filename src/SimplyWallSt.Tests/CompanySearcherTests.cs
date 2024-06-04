using SimplyWallSt.Listing.Repository.Company;
using SimplyWallSt.Listing.Repository.CompanyPriceClose;
using SimplyWallSt.Listing.Repository.CompanyScore;
using SimplyWallSt.Services;
using FakeItEasy;
using SimplyWallSt.Listing.Repository.Enums;
using NUnit.Framework.Constraints;

namespace SimplyWallSt.Tests
{
    public class CompanySearcherTests
    {
        IRepositoryModelToViewModelMapper _RepositoryModelToViewModelMapper { get; set; }
        ICompanyRepository _CompanyRepository { get; set; }
        ICompanyPriceCloseRepository _CompanyPriceCloseRepository { get; set; }
        ICompanyScoreRepository _CompanyScoreRepository { get; set; }

        CompanySearcher _CompanySearcher { get; set; }


        [SetUp]
        public void Setup()
        {
            _CompanyRepository = A.Fake<ICompanyRepository>();
            _CompanyPriceCloseRepository = A.Fake<ICompanyPriceCloseRepository>();
            _CompanyScoreRepository = A.Fake<ICompanyScoreRepository>();

            // We will test the mapper along with company searcher
            _RepositoryModelToViewModelMapper = new RepositoryModelToViewModelMapper();

            _CompanySearcher = new CompanySearcher(_RepositoryModelToViewModelMapper, _CompanyRepository, _CompanyPriceCloseRepository, _CompanyScoreRepository);
        }

        [Test]
        public async Task CompanySearcher_ValidParameters_ReturnsValidSearchResults()
        {
            var companyId = Guid.NewGuid();
            var scoreId = 1234;
            var company = new Company()
            {
                ID = companyId,
                ScoreId = scoreId
            };
            var companySearchResult = new List<Company> { company };

            var now = DateTime.UtcNow;
            var companySearchPricesResult = new List<CompanyPriceClose> {
                new CompanyPriceClose
                {
                    CompanyId = companyId,
                    Date = now,
                    DateCreated = now,
                    Price = 123
                }
            };

            var scoreValue = 20;
            var companyScore = new CompanyScore
            {
                CompanyId = companyId,
                Id = scoreId,
                Total = scoreValue
            };

            A.CallTo(() => _CompanyRepository.Search(A<CompanySearchSortbyEnum>._, A<string>._, A<decimal>._, A<int>._, A<int>._)).Returns(companySearchResult);
            A.CallTo(() => _CompanyPriceCloseRepository.GetPricesByCompanyId(A<Guid>._)).Returns(companySearchPricesResult);
            A.CallTo(() => _CompanyScoreRepository.GetByScoreId(A<int>._)).Returns(companyScore);

            var result = await _CompanySearcher.Search(Enums.CompanySearchSortbyEnum.CompanyScore, "ASX", default, default, 10);

            A.CallTo(() => _CompanyRepository.Search(CompanySearchSortbyEnum.CompanyScore, "ASX", default, default, 10)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _CompanyPriceCloseRepository.GetPricesByCompanyId(companyId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _CompanyScoreRepository.GetByScoreId(scoreId)).MustHaveHappenedOnceExactly();

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().company.ID, Is.EqualTo(companyId));
            Assert.That(result.First().company.ScoreId, Is.EqualTo(scoreId));
            Assert.That(result.First().score.Total, Is.EqualTo(scoreValue));
        }

        [Test]
        public async Task CompanySearcher_TwoCompanies_ReturnsValidSearchResults()
        {
            var companyId1 = Guid.NewGuid();
            var scoreId1 = 1234;
            var company1 = new Company()
            {
                ID = companyId1,
                ScoreId = scoreId1
            };
            var companyId2 = Guid.NewGuid();
            var scoreId2 = 4321;
            var company2 = new Company()
            {
                ID = companyId2,
                ScoreId = scoreId2
            };
            var companySearchResult = new List<Company> { company1, company2 };

            var now = DateTime.UtcNow;
            var companySearchPricesResult1 = new List<CompanyPriceClose> {
                new CompanyPriceClose
                {
                    CompanyId = companyId1,
                    Date = now,
                    DateCreated = now,
                    Price = 123
                }
            };

            var companySearchPricesResult2 = new List<CompanyPriceClose> {
                new CompanyPriceClose
                {
                    CompanyId = companyId2,
                    Date = now,
                    DateCreated = now,
                    Price = 23
                },
                new CompanyPriceClose
                {
                    CompanyId = companyId2,
                    Date = now.AddDays(-1),
                    DateCreated = now.AddDays(-1),
                    Price = 20
                }
            };

            var companyScore1 = new CompanyScore
            {
                CompanyId = companyId1,
                Id = scoreId1,
                Total = 20
            };

            var companyScore2 = new CompanyScore
            {
                CompanyId = companyId2,
                Id = scoreId2,
                Total = 17
            };

            A.CallTo(() => _CompanyRepository.Search(A<CompanySearchSortbyEnum>._, A<string>._, A<decimal>._, A<int>._, A<int>._)).Returns(companySearchResult);
            A.CallTo(() => _CompanyPriceCloseRepository.GetPricesByCompanyId(companyId1)).Returns(companySearchPricesResult1);
            A.CallTo(() => _CompanyScoreRepository.GetByScoreId(scoreId1)).Returns(companyScore1);
            A.CallTo(() => _CompanyPriceCloseRepository.GetPricesByCompanyId(companyId2)).Returns(companySearchPricesResult2);
            A.CallTo(() => _CompanyScoreRepository.GetByScoreId(scoreId2)).Returns(companyScore2);

            var result = await _CompanySearcher.Search(Enums.CompanySearchSortbyEnum.Volatility, "NYSE", default, default, 10);

            A.CallTo(() => _CompanyRepository.Search(CompanySearchSortbyEnum.Volatility, "NYSE", default, default, 10)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _CompanyPriceCloseRepository.GetPricesByCompanyId(A<Guid>._)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _CompanyScoreRepository.GetByScoreId(A<int>._)).MustHaveHappenedTwiceExactly();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().company.ID, Is.EqualTo(companyId1));
            Assert.That(result.First().company.ScoreId, Is.EqualTo(scoreId1));
            Assert.That(result.First().score.Total, Is.EqualTo(20));
        }
    }
}