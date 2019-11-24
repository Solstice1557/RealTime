namespace RealTime.DAL.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class DailyPrice : PriceBase
    {
        [Key]
        public long DailyPriceId { get; set; }
    }
}
