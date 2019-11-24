namespace RealTime.DAL.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class Price : PriceBase
    {
        [Key]
        public long PriceId { get; set; }
    }
}
