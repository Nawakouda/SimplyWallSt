using System;
using System.Threading.Tasks;

namespace SimplyWallSt.Listing.Repository.CompanyScore
{
    /// <summary>
    /// Interface for repositories from which we can query company scores
    /// </summary>
    public interface ICompanyScoreRepository
    {
        /// <summary>
        /// Get company score by score Id
        /// </summary>
        /// <param name="scoreId">Score Id</param>
        /// <returns>Company score result</returns>
        Task<CompanyScore> GetByScoreId(int scoreId);
    }
}
