using System.Runtime.Serialization;

namespace SimplyWallSt.Models
{
    [DataContract]
    public class CompanyPriceCloseViewModel
    {
        /// <summary>
        /// Date
        /// </summary>
        [DataMember(Name = "date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// closing price
        /// </summary>
        [DataMember(Name = "price")]
        public decimal Price { get; set; }
    }
}
