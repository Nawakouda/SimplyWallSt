using System;

namespace SimplyWallSt.Listing.Repository.Company
{
    /// <summary>
    /// An object representing a publicly listed company
    /// </summary>
    public class Company
    {
        /// <summary>
        /// Unique identifier for the company
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Company Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The company's ticker symbol
        /// </summary>
        public string TickerSymbol { get; set; }

        /// <summary>
        /// Symbol for the stock exchange on which the company is listed
        /// </summary>
        public string ExchangeSymbol { get; set; }

        /// <summary>
        /// Ticker that uniquely identifies the company's stock across all exchanges
        /// </summary>
        public string UniqueSymbol { get; set; }

        /// <summary>
        /// Date when the row was created in source database
        /// </summary>
        public DateTime DateGenerated { get; set; }

        /// <summary>
        /// ISO code for country that the company's exchange operates in
        /// </summary>
        public string ExchangeCountryISO { get; set; }

        /// <summary>
        /// ISO code for the currency that the company trades in
        /// </summary>
        public string ListingCurrencyISO { get; set; }

        /// <summary>
        /// Canonical URL
        /// </summary>
        public string CanonicalURL { get; set; }

        /// <summary>
        /// Unique Symbol Slug
        /// </summary>
        public string UniqueSymbolSlug { get; set; }

        /// <summary>
        /// Score Id
        /// </summary>
        public int ScoreId { get; set; }
    }
}
