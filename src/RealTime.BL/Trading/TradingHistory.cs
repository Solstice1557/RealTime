namespace RealTime.BL.Trading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TradingHistory
    {
        public List<TradingHistoryItem> Items { get; } = new List<TradingHistoryItem>();

        private int currentAmount = 0;

        public void Buy(DateTime date, decimal price, int amount)
        {
            this.AddItem(date, price, amount);
        }

        public void Sell(DateTime date, decimal price, int amount)
        {
            if (amount > currentAmount)
            {
                throw new InvalidOperationException("You can't sell more than you have");
            }

            this.AddItem(date, price, -amount);
        }

        public int GetCurrentAmount()
        {
            return currentAmount;
        }

        public decimal GetCurrentProfit(decimal currentPrice)
        {
            var curentMoneySpent = this.Items.Sum(x => x.Sum);
            return currentAmount * currentPrice - curentMoneySpent;
        }

        private void AddItem(DateTime date, decimal price, int amount)
        {
            var item = new TradingHistoryItem(date, price, amount);
            currentAmount = currentAmount + amount;
            this.Items.Add(item);
        }
    }
}

