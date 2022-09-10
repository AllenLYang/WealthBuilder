namespace WealthBuilder.Models
{
    public class WealthForecast
    {
        public int Age { get; set; }
        public Decimal PresentValue { get; set; }
        public Decimal FutureValue { get; set; }

        public Decimal AnnualWithdrawalAmountFV { get; set; }
    }
}