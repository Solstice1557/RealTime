namespace RealTime.BL.Trading
{
    using System;

    public class TradingHistoryItem
    {
        public TradingHistoryItem(DateTime date, decimal price, int amount)
        {
            this.Date = date;
            this.Price = price;
            this.Amount = amount;
        }

        public DateTime Date { get; }

        public decimal Price { get; }

        public int Amount { get; }

        public decimal Sum => this.Price * this.Amount;
    }
}
