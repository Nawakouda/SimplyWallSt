using System;

namespace SimplyWallSt.Listing.Repository.CompanyPriceClose
{
    /// <summary>
    /// An object representing the closing price of a company on a given day
    /// </summary>
    public class CompanyPriceClose
    {
        /// <summary>
        /// The close date for this entry
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Company's unique identifier
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Closing price of the company's stock on the given close date
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Date when the row was created in source database
        /// </summary>
        public DateTime DateCreated { get; set; }
    }
}
