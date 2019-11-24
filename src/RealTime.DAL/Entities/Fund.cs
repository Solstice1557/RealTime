namespace RealTime.DAL.Entities
{
    using System.ComponentModel.DataAnnotations;
    
    public class Fund
    {
        [Key]
        public int FundId { get; set; }

        [StringLength(50)]
        public string Symbol { get; set; }

        [StringLength(300)]
        public string Name { get; set; }

        public int Volume { get; set; }
    }
}
