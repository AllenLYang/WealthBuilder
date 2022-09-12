using Microsoft.AspNetCore.Components;
using WealthBuilder.Models;
using WealthBuilder.Services;

namespace WealthBuilder.Pages
{
    public partial class EstatePlanning
    {
        [Inject]
        public WealthForecastService ForecastService { get; set; }

        [Inject]
        public InvestorProfile InvestorProfile { get; set; }

        public decimal AnnualSavingAmount { get; set; }

        private List<WealthForecast> PortfolioForecasts;
        private string LeadingMessage;
        private string FutureAndPresentValueMessage;
        private bool DisplayTaxableAccountGrowthInPhase2Retirement = true;

        protected override async Task OnInitializedAsync()
        {
            PortfolioForecasts = ForecastService.RunWealthForecastOnTaxableAccountForPhase2Retirement();

            if (ForecastService.RetirementAccountValueEndingPhase2Retirement == null)
            {
                ForecastService.GetRetirementAccountBalanceWithDistributionsForPhase2Retirement(InvestorProfile.AnnualWithdrawalAmountPV);
            }

            LeadingMessage = $"At age {InvestorProfile.LifeSpanMaxAge}, the future value of your taxable account is {ForecastService.TaxableAccountValueEndingPhase2Retirement.FutureValue.ToUSDollar()}. " +
                $"In addition, the future value of your retirement account is {ForecastService.RetirementAccountValueEndingPhase2Retirement.FutureValue.ToUSDollar()}.";                

            FutureAndPresentValueMessage = $"Your estate is the sum of both accounts, which stands at {ForecastService.CombinedAccountValueEndingPhase2Retirement.FutureValue.ToUSDollar()} in future value, or about {ForecastService.CombinedAccountValueEndingPhase2Retirement.PresentValue.ToUSDollar()} in present value.";

            if (ForecastService.TaxableAccountValueEndingPhase2Retirement.FutureValue <= 0)
            {
                DisplayTaxableAccountGrowthInPhase2Retirement = false;
            }            
        }                
    }
}
