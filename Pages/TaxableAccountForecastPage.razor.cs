using Microsoft.AspNetCore.Components;
using WealthBuilder.Models;
using WealthBuilder.Services;

namespace WealthBuilder.Pages
{
    public partial class TaxableAccountForecastPage
    {
        [Inject]
        public WealthForecastService ForecastService { get; set; }

        [Inject]
        public InvestorProfile InvestorProfile { get; set; }

        public decimal AnnualSavingAmount { get; set; }

        private List<WealthForecast> PortfolioForecasts;
        private string LeadingMessage;

        protected override async Task OnInitializedAsync()
        {
            LeadingMessage = $"Here is how your annual savings of {InvestorProfile.AnnualTaxableSavingAmountPV.ToString("C0")} would grow with a low cost index fund that tracks the S&P 500 index in the next {InvestorProfile.NumberOfWorkingYears} years.";
            PortfolioForecasts = ForecastService.RunWealthForecastOnTaxableAccount();
        }                
    }
}
