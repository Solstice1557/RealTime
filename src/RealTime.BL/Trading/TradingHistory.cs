namespace RealTime.BL.Trading
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class TradingHistory
    {
        public List<TradingHistoryItem> Items { get; } = new List<TradingHistoryItem>();

        public void Buy(DateTime date, decimal price, int amount)
        {
            this.AddItem(date, price, amount);
        }

        public void Sell(DateTime date, decimal price, int amount)
        {
            if (amount > this.GetCurrentAmount())
            {
                throw new InvalidOperationException("You can't sell more than you have");
            }

            this.AddItem(date, price, -amount);
        }

        public int GetCurrentAmount()
        {
            return this.Items.Sum(x => x.Amount);
        }

        public decimal GetCurrentProfit(decimal currentPrice)
        {
            var currentAmount = this.GetCurrentAmount();
            var curentMoneySpended = this.Items.Sum(x => x.Sum);
            return currentAmount * currentPrice - curentMoneySpended;
        }

        private void AddItem(DateTime date, decimal price, int amount)
        {
            var item = this.Items.FirstOrDefault(x => x.Date == date);
            if (item != null)
            {
                this.Items.Remove(item);
            }

            item = new TradingHistoryItem(date, price, amount);
            this.Items.Add(item);
            this.Items.Sort(new TradingHistoryItemDateComparer());
        }

        private class TradingHistoryItemDateComparer : IComparer, IComparer<TradingHistoryItem>
        {
            public int Compare(TradingHistoryItem a, TradingHistoryItem b)
            {
                if (a == null )
                {
                    return b == null ? 0 : -1;
                }

                if (b == null)
                {
                    return 1;
                }

                if (a.Date == b.Date)
                {
                    return 0;
                }

                return a.Date > b.Date ? 1 : -1;
            }

            int IComparer.Compare(object q, object r)
            {
                return Compare(q as TradingHistoryItem, r as TradingHistoryItem);
            }
        }
    }
}

