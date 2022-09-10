using WealthBuilder.Services;

namespace WealthBuilder.Models
{
    public class InvestorProfile
    {
        public const int NumberOfPhase1RetirementYears = 18;   //This can be exposed as user input.
        public const int LifeSpanMaxAge = 101;

        public int WorkStartAge { get; set; }
        public int NumberOfWorkingYears { get; set; }

        public LifeStyle RetirementLifeStyle { get; set; }

        //The definition of savings and withdrawal amounts is in present value.
        public int AnnualTaxableSavingAmountPV { get; set; }
        public int AnnualRetirementSavingAmountPV { get; set; }
        public int AnnualWithdrawalAmountPV { get; set; }

        //The actual withdrawal amounts in the future are in future value, which factors in inflation.
        public int AnnualWithdrawalAmountFVPhase1Retirement
        {
            get
            {
                return AnnualWithdrawalAmountPV.ToFutureInflatedAmount(NumberOfWorkingYears + 1);
            }
        }

        public int AnnualWithdrawalAmountFVPhase2Retirement
        {
            get
            {
                return AnnualWithdrawalAmountPV.ToFutureInflatedAmount(NumberOfWorkingYears + NumberOfPhase1RetirementYears + 1);
            }
        }

        //The idea is to keep the retirement savings invested until phase 2 of the retirement
        public int YearsToInvestOnRetirementAccount
        {
            get
            {
                return NumberOfWorkingYears + NumberOfPhase1RetirementYears;
            }
        }

        public int Phase1RetirementStartAge
        {
            get
            {
                return WorkStartAge + NumberOfWorkingYears;
            }
        }

        public int Phase1RetirementStartYear
        {
            get
            {
                return DateTime.Now.Year + NumberOfWorkingYears;
            }
        }

        public int Phase2RetirementStartAge
        {
            get
            {
                return Phase1RetirementStartAge + NumberOfPhase1RetirementYears;
            }
        }

        public int Phase2RetirementStartYear
        {
            get
            {
                return Phase1RetirementStartYear + NumberOfPhase1RetirementYears;
            }
        }

    }
}
