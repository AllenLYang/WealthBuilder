using Microsoft.AspNetCore.Components;
using WealthBuilder.Models;
using WealthBuilder.Services;

namespace WealthBuilder.Pages
{
    public partial class RetirementPhase2
    {
        [Inject]
        public WealthForecastService ForecastService { get; set; }
        
        [Inject]
        public InvestorProfile InvestorProfile { get; set; }

        private List<WealthForecast> PortfolioForecasts;
        
        protected override async Task OnInitializedAsync()
        {   
            PortfolioForecasts = ForecastService.GetRetirementAccountBalanceWithDistributionsForPhase2Retirement(InvestorProfile.AnnualWithdrawalAmountPV);
        }

    }
}
