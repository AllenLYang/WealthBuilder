using System.Globalization;
using WealthBuilder.Models;

namespace WealthBuilder.Services;

public static class FinanceUtils
{
    private const decimal AnnualInflationRate = 0.03M;      //assuming a 3% annual inflation rate
                
    public static string ToUSDollar(this int amount)
    {
        var context = new CultureInfo("en-US");

        return amount.ToString("C0", context);
    }

    public static string ToUSDollar(this decimal amount)
    {
        var context = new CultureInfo("en-US");

        return amount.ToString("C0", context);
    }

    public static int ToFutureInflatedAmount(this int originalAmount, int numberOfYears)
    {
        var inflatedAmount = (decimal)originalAmount;
        for (int i = 0; i < numberOfYears; i++)
        {
            inflatedAmount = inflatedAmount * (1 + AnnualInflationRate);
        }

        return (int)inflatedAmount;
    }

    public static int ToFutureInflatedAmount(this decimal originalAmount, int numberOfYears)
    {
        var inflatedAmount = (decimal)originalAmount;
        for (int i = 0; i < numberOfYears; i++)
        {
            inflatedAmount = inflatedAmount * (1 + AnnualInflationRate);
        }

        return (int)inflatedAmount;
    }

    public static List<LifeStyle> GetRetirementLifestyleList()
    {
        var retirementLifeStyleList = new List<LifeStyle>();
        var lifeStyle = new LifeStyle()
        {
            Id = 1,                
            AnnualRetirementSavingAmount = 5000,
            AnnualTaxableSavingAmount = 1000,
            AnnualRetirementWithdrawalAmount = 50000,
            Name = "Working Middle Class",                
            Message = "Cannot afford to save much. Live paycheck to paycheck. Need to work into one's old age."                
        };
        retirementLifeStyleList.Add(lifeStyle);

        lifeStyle = new LifeStyle()
        {
            Id = 2,
            AnnualRetirementSavingAmount = 10000,
            AnnualTaxableSavingAmount = 3000,
            AnnualRetirementWithdrawalAmount = 75000,
            Name = "Middle Middle Class",
            Message = "Typically save 5% to 6% of one's income, which is the percentage needed to get the full company matching in a 401(K) plan."
        };
        retirementLifeStyleList.Add(lifeStyle);

        lifeStyle = new LifeStyle()
        {
            Id = 3,
            AnnualRetirementSavingAmount = 20500,
            AnnualTaxableSavingAmount = 30000,
            AnnualRetirementWithdrawalAmount = 200000,
            Name = "Upper Middle Class - Silver Medallion",
            Message = "Typically maxes out the 401(K) and ROTH IRA limits. Primary residence is in one of the nicest neighborhoods. Net worth ranges from $1.2 to $10 million. The low end of the 9.9-percenters."
        };
        retirementLifeStyleList.Add(lifeStyle);

        lifeStyle = new LifeStyle()
        {
            Id = 4,
            AnnualRetirementSavingAmount = 20500,
            AnnualTaxableSavingAmount = 60000,
            AnnualRetirementWithdrawalAmount = 250000,
            Name = "Upper Middle Class - Gold Medallion",
            Message = "Typically maxes out the 401(K) and ROTH IRA limits. Primary residence is in one of the nicest neighborhoods. Net worth ranges from $10 to $20 million. The high end of the 9.9-percenters."
        };
        retirementLifeStyleList.Add(lifeStyle);

        lifeStyle = new LifeStyle()
        {
            Id = 5,
            AnnualRetirementSavingAmount = 27500,
            AnnualTaxableSavingAmount = 80000,
            AnnualRetirementWithdrawalAmount = 350000,
            Name = "Upper Middle Class - Platinum Medallion",
            Message = "Typically maxes out the 401(K) and ROTH IRA limits. Primary residence is in one of the nicest neighborhoods. Net worth exceeds $20 million. The top 0.1-percenters"
        };
        retirementLifeStyleList.Add(lifeStyle);

        return retirementLifeStyleList;
    }

}
