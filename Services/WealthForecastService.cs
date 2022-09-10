using WealthBuilder.Models;

namespace WealthBuilder.Services
{
    public class WealthForecastService
    {
        /******************************************************************************************************
        Data Source: 
        S & P 500 Historical Annualized Return Rates
        https://www.investopedia.com/ask/answers/042415/what-average-annual-return-sp-500.asp

        S & P 500 Historical Annual Dividend Yields
        https://www.investopedia.com/articles/markets/071616/history-sp-500-dividend-yield.asp

        S&P 500 historical return rates since 1920's.
        Two periods - from 1926 to December 2021 and from 1957 to December 2021 - have the similar annualized 
        return rates of 10.49% and 10.67%.
        
        From 2020 onward, the dividend yield fell below 2%, ranging between 1.4-1.5%.

        Inflation historical data and calculation
        https://smartasset.com/investing/inflation-calculator#INukYazZob
        ******************************************************************************************************/
        private const decimal AnnualInflationRate = 0.03M;      //3% 
        private const decimal AnnualReturnRate = 0.105M;        //10.5%
        private const decimal AnnualReturnRateInflationAdjusted = AnnualReturnRate - AnnualInflationRate;
                

        public readonly InvestorProfile InvestorProfile;

        public WealthForecastService(InvestorProfile profile)
        {
            InvestorProfile = profile;
        }
        
        //To-do: retirement disrtribution needs to be deducted at the beginning of the year
        //before the account growth. Check the first retirement year.

        private WealthForecast TaxableAccountValueOnLastWorkingYear;
        private WealthForecast TaxableAccountValueOnEndingPhase1RetirementYear;
        public WealthForecast TaxableAccountValueEndingPhase2Retirement;
        private WealthForecast RetirementAccountValueStartingPhase2Retirement;
        public WealthForecast RetirementAccountValueEndingPhase2Retirement;
        public WealthForecast CombinedAccountValueEndingPhase2Retirement
        {
            get
            {
                return new WealthForecast()
                {
                    FutureValue = TaxableAccountValueEndingPhase2Retirement.FutureValue + RetirementAccountValueEndingPhase2Retirement.FutureValue,
                    PresentValue = ComputePresentValue(TaxableAccountValueEndingPhase2Retirement.FutureValue + RetirementAccountValueEndingPhase2Retirement.FutureValue,
                        InvestorProfile.WorkStartAge, InvestorProfile.LifeSpanMaxAge)
                };
            }
        }

        private decimal ComputePresentValue(decimal futureValue, int startYear, int endYear)
        {
            var presentValue = futureValue;

            for (var i = startYear; i < endYear; i++)
            {
                presentValue *= 1 - AnnualInflationRate;
            }

            return presentValue;
        }

        public List<WealthForecast> RunWealthForecastOnTaxableAccount()
        {
            var resultList = RunWealthForecast(Convert.ToDecimal(InvestorProfile.AnnualTaxableSavingAmountPV),
                InvestorProfile.WorkStartAge, InvestorProfile.NumberOfWorkingYears);

            TaxableAccountValueOnLastWorkingYear = resultList.LastOrDefault();

            return resultList;
        }

        public List<WealthForecast> RunWealthForecastOnRetirementAccount()
        {
            var resultList = RunWealthForecast(Convert.ToDecimal(InvestorProfile.AnnualRetirementSavingAmountPV),
                InvestorProfile.WorkStartAge, InvestorProfile.YearsToInvestOnRetirementAccount);

            RetirementAccountValueStartingPhase2Retirement = resultList.LastOrDefault();
            return resultList;
        }

        public List<WealthForecast> RunWealthForecastOnTaxableAccountForPhase2Retirement()
        {
            if (TaxableAccountValueOnEndingPhase1RetirementYear == null)
            {
                //Init TaxableAccountValueOnEndingPhase1RetirementYear if user didn't follow the chronological path
                var results = GetTaxableAccountBalanceWithDistributionsForPhase1Retirement(InvestorProfile.AnnualWithdrawalAmountPV);
            }

            var startAge = InvestorProfile.Phase2RetirementStartAge;
            var numberOfYearsToInvest = InvestorProfile.LifeSpanMaxAge - startAge;
            var resultList = RunWealthForecast(0, startAge, numberOfYearsToInvest,
                TaxableAccountValueOnEndingPhase1RetirementYear.PresentValue,
                TaxableAccountValueOnEndingPhase1RetirementYear.FutureValue);

            TaxableAccountValueEndingPhase2Retirement = resultList.LastOrDefault();

            //If the ending balance is 0 or less, that means the account was insufficient to fund
            //phase 1 retirement. Thus, take the last year of phase 1 retirement value, which is a negative number,
            //when adding the total estate value.
            if (TaxableAccountValueEndingPhase2Retirement.FutureValue <= 0)
            {
                TaxableAccountValueEndingPhase2Retirement = TaxableAccountValueOnEndingPhase1RetirementYear;
            }


            return resultList;
        }

        private List<WealthForecast> RunWealthForecast(
            decimal annualSavingAmount, int startAge, int numberOfYearsToInvest,
            decimal existingPVBalance = 0.00M, decimal existingFVBalance = 0.00M)
        {
            var resultList = new List<WealthForecast>();
            var previousTotalAmountPV = existingPVBalance;
            var previousTotalAmountFV = existingFVBalance;

            for (int i = 0; i < numberOfYearsToInvest; i++)
            {
                //Assumption - the annual amount is invested at
                //the end of the year. Try to be less idealistic about 
                //the return.
                var wealth = new WealthForecast()
                {
                    Age = startAge + i,
                };

                if (previousTotalAmountPV < 0)
                {
                    previousTotalAmountPV = 0;
                }

                if (previousTotalAmountFV < 0)
                {
                    previousTotalAmountFV = 0;
                }

                wealth.PresentValue = previousTotalAmountPV * (1 + AnnualReturnRateInflationAdjusted) + annualSavingAmount;
                previousTotalAmountPV = wealth.PresentValue;

                wealth.FutureValue = previousTotalAmountFV * (1 + AnnualReturnRate) + annualSavingAmount;
                previousTotalAmountFV = wealth.FutureValue;

                resultList.Add(wealth);
            }

            return resultList;
        }

        public List<WealthForecast> GetTaxableAccountBalanceWithDistributionsForPhase1Retirement(int annualDistributionAmount)
        {
            if (TaxableAccountValueOnLastWorkingYear == null)
            {
                RunWealthForecastOnTaxableAccount();
            }

            if (TaxableAccountValueOnLastWorkingYear == null ||
                TaxableAccountValueOnLastWorkingYear.PresentValue == 0 ||
                TaxableAccountValueOnLastWorkingYear.FutureValue == 0)
            {
                throw new Exception("Working years portfolio value wasn't computed successfully.");
            }
                        
            decimal annualAmountWithInflation = annualDistributionAmount.ToFutureInflatedAmount(InvestorProfile.NumberOfWorkingYears);

            var resultList = ComputeAccountBalanceWithDistributions(InvestorProfile.Phase1RetirementStartAge,
                InvestorProfile.NumberOfPhase1RetirementYears, annualAmountWithInflation,
                TaxableAccountValueOnLastWorkingYear.FutureValue);

            TaxableAccountValueOnEndingPhase1RetirementYear = resultList.LastOrDefault();

            return resultList;
        }

        public List<WealthForecast> GetRetirementAccountBalanceWithDistributionsForPhase2Retirement(int annualDistributionAmount)
        {            
            if (RetirementAccountValueStartingPhase2Retirement == null)
            {
                RunWealthForecastOnRetirementAccount();
            }

            decimal annualAmountWithInflation = annualDistributionAmount.ToFutureInflatedAmount(InvestorProfile.NumberOfWorkingYears + InvestorProfile.NumberOfPhase1RetirementYears);

            var startAge = InvestorProfile.Phase2RetirementStartAge;
            var numberOfDistributionYears = InvestorProfile.LifeSpanMaxAge - startAge;     //Fix this later

            var resultList = ComputeAccountBalanceWithDistributions(startAge,
                numberOfDistributionYears, annualAmountWithInflation,
                RetirementAccountValueStartingPhase2Retirement.FutureValue);

            RetirementAccountValueEndingPhase2Retirement = resultList.LastOrDefault();

            return resultList;
        }

        private List<WealthForecast> ComputeAccountBalanceWithDistributions(
            int startAge, int numberOfDistributionYears,
            decimal annualWithdrawInflationAmount,
            decimal portfolioTotalAmountFV)
        {
            var resultList = new List<WealthForecast>();
            var inflatedAmount = annualWithdrawInflationAmount;
            var previousTotalAmountFV = portfolioTotalAmountFV;

            for (int i = 0; i < numberOfDistributionYears; i++)
            {
                //Assumption - the annual withdrawal happens at the beginning of the year.
                var wealth = new WealthForecast()
                {
                    Age = startAge + i,
                };

                inflatedAmount = inflatedAmount.ToFutureInflatedAmount(1);
                wealth.AnnualWithdrawalAmountFV = inflatedAmount;

                //overdraft already
                if (previousTotalAmountFV - inflatedAmount < 0)
                {
                    wealth.FutureValue = previousTotalAmountFV - inflatedAmount;
                }
                else
                {
                    wealth.FutureValue = (previousTotalAmountFV - inflatedAmount) * (1 + AnnualReturnRate);
                }

                previousTotalAmountFV = wealth.FutureValue;

                resultList.Add(wealth);
            }

            return resultList;
        }
    }
}