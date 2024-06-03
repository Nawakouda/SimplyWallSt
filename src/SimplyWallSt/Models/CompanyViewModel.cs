using System.Runtime.Serialization;

namespace SimplyWallSt.Models
{
    [DataContract]
    public class CompanyViewModel
    {
        /// <summary>
        /// Company Name
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }
        /// <summary>
        /// Ticker that uniquely identifies the company's stock across all exchanges
        /// </summary>
        [DataMember(Name = "unique_symbol")]
        public string UniqueSymbol { get; set; }

        /// <summary>
        /// ISO code for the currency that the company trades in
        /// </summary>
        [DataMember(Name = "listing_currency_iso")]
        public string ListingCurrencyISO { get; set; }

        /// <summary>
        /// Snowflake score
        /// </summary>
        [DataMember(Name = "total_score")]
        public decimal TotalScore { get; set; }

        /// <summary>
        /// Price History. Sorted descending
        /// </summary>
        [DataMember(Name = "prices")]
        public IEnumerable<CompanyPriceCloseViewModel> Prices { get; set; }
    }
}
