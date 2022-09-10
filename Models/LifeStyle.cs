namespace WealthBuilder.Models
{
    public class LifeStyle
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public int AnnualRetirementSavingAmount { get; set; }
        public int AnnualTaxableSavingAmount { get; set; }
        public int AnnualRetirementWithdrawalAmount { get; set; }
    }
}
